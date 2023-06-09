# XIVLauncherRu [![Actions Status](https://img.shields.io/github/actions/workflow/status/moiseer/FFXIVQuickLauncherRu/ci-workflow.yml?branch=master)](https://github.com/moiseer/FFXIVQuickLauncherRu/actions) [![GitHub release (latest by date)](https://img.shields.io/github/v/release/moiseer/FFXIVQuickLauncherRu)](https://github.com/moiseer/FFXIVQuickLauncherRu/releases/latest) <a href="https://github.com/moiseer/FFXIVQuickLauncherRu/releases"><img src="https://github.com/moiseer/FFXIVQuickLauncherRu/raw/master/src/XIVLauncher/Resources/logo.png" alt="XL logo" width="100" align="right"/></a> 

XIVLauncher (сокращенно XL) - это более быстрый лаунчер на русском для FINAL FANTASY XIV, с различными доступными аддонами и улучшениями для игры!

<p align="center">
  <a href="https://github.com/moiseer/FFXIVQuickLauncherRu/releases">
    <img src="https://raw.githubusercontent.com/moiseer/FFXIVQuickLauncherRu/master/misc/screenshot.png" alt="drawing" width="500"/>
  </a>
</p>

## О лаунчере

Оригинальный лаунчер работает медленно и не может сохранить ваш пароль. Данный проект призван исправить это и добавить в игру некоторые полезные функции, такие как:
* Возможность запоминать пароль (вход по одноразовому паролю тоже поддерживается)
* Более быстрое обновление игры за счёт параллельной загрузки файлов патча
* Более быстрая починка файлов игры за счёт загрузки только испорченных файлов, а не всей игры
* Dalamud — система внутриигровых плагинов, которые улучшают и расширяют игровой процесс.

## Установка лаунчера <!-- TODO: Перевод Antivirus FAQ -->

[Скачайте последнюю версию "Setup.exe" со страницы релизов](https://github.com/moiseer/FFXIVQuickLauncherRu/releases/latest) и запустите ее. XIVLauncher запустится и будет установлен в ваше стартовое меню.  
Для удаления вы можете воспользоваться меню Windows Programs & Apps или щелкнуть XIVLauncher правой кнопкой мыши в вашем стартовом меню.

❗<b>Внимание!</b> Если вы получаете ошибки во время установки или если программа запуска работает некорректно, обязательно <b>проверьте свой антивирус</b> и отключите его для XIVLauncher. Многие коммерческие антивирусные программы определяют XIVLauncher как ложное срабатывание. Вы можете проверить это сами на VirusTotal. Если вам нужна помощь, пожалуйста, [ознакомьтесь с FAQ](https://goatcorp.github.io/faq/xl_troubleshooting#q-how-do-i-whitelist-xivlauncher-and-dalamud-so-my-antivirus-leaves-them-alone).

## Steam Deck и Desktop Linux <!-- TODO: Перевод Steam Deck FAQ -->

XIVLauncher теперь имеет собственную версию для Linux, которая работает на Steam Deck и Desktop Linux - больше не нужно возиться со скриптами и командными строками, просто несколько простых шагов для установки игры и добавления ее в Steam, с версией wine, специально настроенной для XIV.

Пожалуйста, ознакомьтесь с [руководством](https://goatcorp.github.io/faq/steamdeck).

## Установка плагинов

XIVLauncher поддерживает установку __плагинов/аддонов__, созданных __сторонними разработчиками__.

Откройте системное меню в игре, нажав "escape", и выберите в нем "Dalamud Plugins". Вы также можете использовать команду ``/xlplugins`` в чате.  
Если команда не найдена или опции не отображаются, убедитесь, что внутриигровой аддон включен в настройках лаунчера.

## Нужна помощь? <!-- TODO: Перевод FAQ -->

[Пожалуйста, ознакомьтесь с FAQ](https://goatcorp.github.io/faq/), возможно, вы найдете там то, что вам нужно.

## Безопасен ли XIVLauncher в использовании? <!-- TODO: Перевод Safe to use FAQ -->

Мы приложили много усилий для того, чтобы XIVLauncher был безопасен в использовании для всех.  
Если вы хотите узнать больше, ознакомьтесь с [разделом FAQ](https://goatcorp.github.io/faq/xl_troubleshooting#q-are-xivlauncher-dalamud-and-dalamud-plugins-safe-to-use) по этому вопросу.

<br>
<br>

## API плагинов

XIVLauncher позволяет вам использовать множество созданных сообществом плагинов, которые улучшают вашу игру. Их список можно найти на [этом сайте](https://goatcorp.github.io/DalamudPlugins/plugins).  
Чтобы создать собственные внутриигровые плагины для XIVLauncher, ознакомьтесь с [документацией по API](https://goatcorp.github.io/Dalamud/api/index.html) и [примером плагина](https://github.com/goatcorp/SamplePlugin).

### Это читерство?

Мы так не считаем - наши официальные правила для плагинов этого лаунчера таковы:

Убедитесь, что ваш плагин не взаимодействует напрямую с игровыми серверами таким образом, что...
* *выходит за рамки спецификации*, например, позволяет игроку делать вещи или отправлять данные на сервер, которые были бы невозможны обычными средствами или обычным игроком на PC или PS4.
* *автоматизирует процессы*, например, опрашивает данные или делает запросы без непосредственного участия пользователя, что может создать нежелательную нагрузку на сервер или выдать использование XIVLauncher.
* *обходит платежные ограничения*, например, любые попытки обойти ограничения, для снятия которых требуется покупка, например, открыть доступ к платному контенту.

Нам кажется, что это дает разработчикам __свободу улучшать функциональность игры__ так, как не может SE, при этом официально запрещая плагины, которые могут дать __нечестные преимущества игрокам на других платформах__.

<br>
<br>

## Disclaimer <!-- TODO: Перевод Safe to use FAQ -->

XIVLauncher не соответствует правилам игры. Мы делаем все возможное, чтобы сделать его безопасным для всех, и, насколько нам известно, ни у кого никогда не возникало проблем из-за использования XIVLauncher, но, пожалуйста, имейте в виду, что такая возможность существует.  
Если вы хотите узнать больше, вы можете прочитать [раздел FAQ](https://goatcorp.github.io/faq/xl_troubleshooting#q-are-xivlauncher-dalamud-and-dalamud-plugins-safe-to-use) по этому вопросу.

##### FINAL FANTASY является зарегистрированной торговой маркой Square Enix Holdings Co., Ltd. FINAL FANTASY XIV © 2010-2022 SQUARE ENIX CO., LTD. Все права защищены. Мы никак не связаны с компанией SQUARE ENIX CO., LTD.
