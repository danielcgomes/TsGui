Documentation is currently a work in progress. For usage information, also see the [How to videos](https://www.youtube.com/playlist?list=PLbymiOxRQJvIS6BGPJ6ggKaU90QheXgV8)



## Concepts

* [Layout](/documentation/Layout.md)
* [TsGui Options](/documentation/options/README.md) (UI controls)
* [Prebuilt configuration](/documentation/PrebuiltConfiguration.md)
* [Configuring the main window](/documentation/MainWindow.md)
* [Configuration guidelines](/documentation/ConfigGuidelines.md)
* [Configuration imports](/documentation/ConfigImports.md) (import from external config files)
* [Authentication](/documentation/Authentication/README.md)

## Features
* [Compliance (pre-flight checks)](/documentation/features/Compliance.md)
* [Groups and Toggles](/documentation/features/GroupsAndToggles.md)
* [Option Linking](/documentation/features/OptionLinking.md)
* [Queries](/documentation/features/Queries.md)
* [Scripts](/documentation/features/Scripts.md)
* [Styles (previously Formatting)](/documentation/features/Styles.md)
* [TsGui Output](/documentation/features/TsGuiOutput.md)
* [Validation](/documentation/features/Validation.md)

## Command line options
tsgui.exe -config [filepath] -webconfig [url] -hash [password]

 **-config [filepath]** where [filepath] is the path to a config XML file on the file system<br>
 **-webconfig [url]** where [url] is the URL to a config XML file on a web server. Takes precedence over **-config**. Note that this only downloads the config, not any dependencies e.g. images<br>
 **-hash [password]** where [password] is a password to be hashed for use in a [Local Config Authentication](/documentation/Authentication/LocalConfigAuthentication.md) block<br>
 **-key [keystring]** where [keystring] is the key to be used during the hash operation. Must be used with **-hash**

 \* If both -config and -webconfig are ommitted, TsGui will look for Config.xml in the same folder as tsgui.exe


 ## Code

 Source documentation can be found in the [code](/documentation/code/README.md) documentation.