
* These are tools that live in the default BuildConfig and are there by default
  
* Generate Files (Writes file everytime there is a reload, if the contents are different)
  * [ ] Editor Config
  * [ ] .p4Ignore
  * [ ] Set p4ignore environment variable
  * [ ] .gitignore
  * [ ] .collabignore

* Analyzers
  * List of Analyzers
    * Name
    * List of DLLs
    * List of Ruleset files
    * List of CSProject files to include in
  * [ ] Editor Config
  * [ ] .p4Ignore
  * [ ] Set p4ignore environment variable
  * [ ] .gitignore
  * [ ] .collabignore

* Remove Empty Directory Tool
    * Promts the user if it finds directories that we think shoudl be deleted
    * Ignore them if the directory is less than an hour old

* Fix Line Endings
    * Prompts the user for a list of files with bad Line endings  
    * Scan all code bad line endings and print warnings if find files with bad line endings
      or messed up line endings "\r\r\n"

* Print Warning if "Force Text Serialization" is off
* Print Warning if "Visible Meta Files" is off (and not using P4 or something similar)
* Change Playmode Tint (if it hasn't been already)
* Detect AndroidManifest to make sure bundle identifier is set corrctly???

* Tool for adding .p4config files?
  * If user runs editor and there is no .p4config file, then prompt them to create one
  * A sample P4CONFIG file might contain the following lines:
    * P4CLIENT=joes_client
    * P4USER=joe
    * P4PORT=ssl:ida:3548
    * P4IGNORE=.p4ignore
