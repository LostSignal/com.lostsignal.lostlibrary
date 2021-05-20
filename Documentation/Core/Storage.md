
### Lost LocalStore
------------------------
* LocalStore
  * Use LocalStore for garunteed delivery of PlayFab events to server
  * Use LocalStore for garunteed delivery of Client Logs to custom server fucntions
  * Add basic obfuscation?

### Lost PlayerPrefs
--------------------------
* LostPlayerPrefs
  * Add basic obfuscation
  * Add encription hashes (https://www.youtube.com/watch?v=_hAzWgQupms)
  * Add multiple save files that you rotate between
  * Autosave on Pause/Quite, or maybe save every 5 seconds if dirty
  * On Android there is internal and external data paths, should support both
* Update LostPlayerPrefs to save to disk, or at least save to PlayerPrefs
  with some obfuscation.
  * Make sure it works on XboxOne, WebGL and UWP
  * Will need to reserect the Platform file saving code (still not sure what to
    do for XboxOne, WebGL and UWP

