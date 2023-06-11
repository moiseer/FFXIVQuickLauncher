using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using CheapLoc;
using Newtonsoft.Json;
using Serilog;
using Squirrel;
using XIVLauncher.Accounts;
using XIVLauncher.Common;
using XIVLauncher.Common.Util;
using XIVLauncher.Windows;

#nullable enable

namespace XIVLauncher
{
    internal class Updates
    {
        public event Action<bool>? OnUpdateCheckFinished;

#if DEV_SERVER
        private const string LEASE_META_URL = "http://localhost:5025/Launcher/GetLease";
#else
        private const string LEASE_META_URL = "https://kamori.goats.dev/Launcher/GetLease";
#endif

        private const string TRACK_RELEASE = "Release";
        private const string TRACK_PRERELEASE = "Prerelease";

        public static Lease? UpdateLease { get; private set; }

#pragma warning disable CS8618
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
        public class ErrorNewsData
        {
            [JsonPropertyName("until")]
            public uint ShowUntil { get; set; }

            [JsonPropertyName("message")]
            public string Message { get; set; }

            [JsonPropertyName("isError")]
            public bool IsError { get; set; }
        }
#pragma warning restore CS8618

        [Flags]
        public enum LeaseFeatureFlags
        {
            None = 0,
            GlobalDisableDalamud = 1,
            GlobalDisableLogin = 1 << 1,
        }

#pragma warning disable CS8618
        public class Lease
        {
            public bool Success { get; set; }

            public string? Message { get; set; }

            public string? CutOffBootver { get; set; }

            public string FrontierUrl { get; set; }

            public LeaseFeatureFlags Flags { get; set; }

            public string ReleasesList { get; set; }

            public DateTime? ValidUntil { get; set; }
        }
#pragma warning restore CS8618

        public class LeaseAcquisitionException : Exception
        {
            public LeaseAcquisitionException(string message)
                : base($"Couldn't acquire lease: {message}")
            {
            }
        }

        private static async Task<Lease> GetLeaseUpdateAsync(bool prerelease)
        {
            using var client = new HttpClient
            {
                DefaultRequestHeaders =
                {
                    UserAgent = { new ProductInfoHeaderValue("XIVLauncher", "6.3.6-0-g74e3142") }
                }
            };
            client.DefaultRequestHeaders.AddWithoutValidation("X-XL-Track", prerelease ? TRACK_PRERELEASE : TRACK_RELEASE);
            client.DefaultRequestHeaders.AddWithoutValidation("X-XL-LV", "0");
            client.DefaultRequestHeaders.AddWithoutValidation("X-XL-HaveVersion", "6.3.6.0");
            client.DefaultRequestHeaders.AddWithoutValidation("X-XL-HaveAddon", App.Settings.InGameAddonEnabled ? "yes" : "no");
            client.DefaultRequestHeaders.AddWithoutValidation("X-XL-FirstStart", App.Settings.VersionUpgradeLevel == 0 ? "yes" : "no");
            client.DefaultRequestHeaders.AddWithoutValidation("X-XL-HaveWine", EnvironmentSettings.IsWine ? "yes" : "no");

            var response = await client.GetAsync(LEASE_META_URL).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            if (response.Headers.TryGetValues("X-XL-Canary", out var values) &&
                values.FirstOrDefault() == "yes")
            {
                Log.Information("Updates: Received canary track lease!");
            }

            var leaseData = JsonConvert.DeserializeObject<Lease>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));

            if (!leaseData.Success)
                throw new LeaseAcquisitionException(leaseData.Message!);

            return leaseData;
        }

        public static async Task<ErrorNewsData?> GetErrorNews()
        {
            ErrorNewsData? newsData = null;

            try
            {
                const string NEWS_URL = "https://gist.githubusercontent.com/goaaats/5968072474f79b066a60854d38b95280/raw/xl-news.txt";

                using var client = new HttpClient
                {
                    Timeout = TimeSpan.FromSeconds(10),
                };

                var text = await client.GetStringAsync(NEWS_URL).ConfigureAwait(false);
                newsData = JsonConvert.DeserializeObject<ErrorNewsData>(text);
            }
            catch (Exception newsEx)
            {
                Log.Error(newsEx, "Could not get error news");
            }

            return DateTimeOffset.UtcNow.ToUnixTimeSeconds() > newsData?.ShowUntil ? null : newsData;
        }

        public async Task Run(bool downloadPrerelease, ChangelogWindow changelogWindow)
        {
            // GitHub requires TLS 1.2, we need to hardcode this for Windows 7
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            try
            {
                UpdateLease = await GetLeaseUpdateAsync(downloadPrerelease);
                using var updateManager = await UpdateManager.GitHubUpdateManager(App.REPO_URL, prerelease: downloadPrerelease);

                // TODO: is this allowed?
                SquirrelAwareApp.HandleEvents(
                    onInitialInstall: v => updateManager.CreateShortcutForThisExe(),
                    onAppUpdate: v => updateManager.CreateShortcutForThisExe(),
                    onAppUninstall: v =>
                    {
                        updateManager.RemoveShortcutForThisExe();

                        if (CustomMessageBox.Show(Loc.Localize("UninstallQuestion", "Sorry to see you go!\nDo you want to delete all of your saved settings, plugins and passwords?"), "XIVLauncher",
                                MessageBoxButton.YesNo, MessageBoxImage.Question, false, false)
                            == MessageBoxResult.Yes)
                        {
                            try
                            {
                                var mgr = new AccountManager(App.Settings);

                                foreach (var account in mgr.Accounts.ToArray())
                                {
                                    account.Password = null;
                                    mgr.RemoveAccount(account);
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.Error(ex, "Uninstall: Could not delete passwords");
                            }

                            try
                            {
                                // Let's just give this a shot, probably not going to work 100% but
                                // there's not super much we can do about it right now
                                Directory.Delete(Paths.RoamingPath, true);
                            }
                            catch (Exception ex)
                            {
                                Log.Error(ex, "Uninstall: Could not delete roaming directory");
                            }
                        }
                    });

                await updateManager.CheckForUpdate().ConfigureAwait(false);
                ReleaseEntry? newRelease = await updateManager.UpdateApp().ConfigureAwait(false);

                if (newRelease != null)
                {
                    if (changelogWindow == null)
                    {
                        Log.Error("changelogWindow was null");
                        UpdateManager.RestartApp();
                        return;
                    }

                    try
                    {
                        changelogWindow.Dispatcher.Invoke(() =>
                        {
                            changelogWindow.UpdateVersion(newRelease.Version.ToString());
                            changelogWindow.Show();
                            changelogWindow.Closed += (_, _) =>
                            {
                                UpdateManager.RestartApp();
                            };
                        });

                        OnUpdateCheckFinished?.Invoke(false);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Could not show changelog window");
                        UpdateManager.RestartApp();
                    }
                }
#if !XL_NOAUTOUPDATE
                else
                    OnUpdateCheckFinished?.Invoke(true);
#endif
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Update failed");
                var newsData = await GetErrorNews().ConfigureAwait(false);

                if (newsData != null && !string.IsNullOrEmpty(newsData.Message))
                {
                    CustomMessageBox.Show(newsData.Message,
                        "XIVLauncher",
                        MessageBoxButton.OK,
                        newsData.IsError ? MessageBoxImage.Error : MessageBoxImage.Asterisk, showOfficialLauncher: true);
                }
                else
                {
                    CustomMessageBox.Show(Loc.Localize("updatefailureerror", "XIVLauncher failed to check for updates. This may be caused by internet connectivity issues. Wait a few minutes and try again.\nDisable your VPN, if you have one. You may also have to exclude XIVLauncher from your antivirus.\nIf this continues to fail after several minutes, please check out the FAQ."),
                        "XIVLauncher",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error, showOfficialLauncher: true);
                }

                Environment.Exit(1);
            }

            // Reset security protocol after updating
            ServicePointManager.SecurityProtocol = SecurityProtocolType.SystemDefault;
        }
    }
}

#nullable restore