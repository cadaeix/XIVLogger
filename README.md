# XIVLogger

NOTE: Logs are saved from when this plugin is activated

Very much a work in progress plugin for [XIVQuickLauncher](https://github.com/goatcorp/FFXIVQuickLauncher). 

A simple chat log plugin that logs chat messages from when the plugin is turned on, and then dumps it to a text file in your documents at a press of a button. Now supports copying!

Primarily intended for saving roleplay logs.

Supports saving the following chat types at the moment:

Say, Shout, Yell, Party/CrossParty, Alliance, Tells, Emotes (Custom, Standard), Cross World Linkshells 1 - 8, Linkshells 1 - 8, PVP Team, Novice Network, Free Company


## Commands

* ``/xivlogger`` to bring up settings plus save log button

* ``/savelog`` to save the log from when the plugin is activated
    * ``/savelog <n>`` to save the last ``<n>`` messages, example: ``/savelog 2``

* ``/copylog`` to copy the log from when the plugin is activated
    * ``/copylog <n>`` to copy the last ``<n>`` messages, example: ``/copylog 2``

## To Do

* <strike>Actual documentation and help text</strike>
* <strike>Slash commands</strike> some commands exist now
* <strike>Specify a filepath to save to, instead of assuming a Documents folder exists</strike>
* <strike>Specify names for text files</strike>
* Turning on and off categories of chat texts, like all linkshells
* Saving combinations of different chat types and naming them?
    * Allow this to be used with the slash command, to quickly save logs based on differen combinations
    * I'd love a way to save logs based on chat tab configurations, but this seems much easier said than done
* Do the chat settings actually persist?
* <strike>Timestamps</strike>
    * Adding commands to save all messages from a specified time
* <strike>Adding commands to save the last <i>n</i> amount of messages</strike>
* <strike>Load chat log into copy and paste?</strike>
* Name replacer, specify a character and a name to replace
* Consolidate slash commands into a single slash command with multiple arguments, like ``/xivlogger savelog 2`` to save the last 2 messages
* Automated logging, save log every <i>n</i> minutes, and on exit