* PlayFab
  * Talk to them about PlayerInbox and persistent connection
  * Make a better TitleData wrapper (remove version, have PF class cache files)
    * Add PF.GetTitleData(bool usedCachedVersion)
  * Need system where the client queries PlayFab with it's version, and playfab returns
    the title data version, asset bundle location and whether or not an update is required.
    And if an update is required, it should return the links to the app.

* Make a better version of the TitleDataWrapper scriptable object
  * Update PlayFabServer class to have GetJson(bool useCachedIfAvailable)
  * Code should actually point to the Scriptable object, and you call GetVersion(string) to 
    get the latest version of the data.  If game is running, it gets it from PlayFab, else
    it just uses the data on disk.  If you get the json durring login, then it should grab
    that cached version.
  * It would be really really really nice if PlayFab had a good Test -> Dev -> Prod struct, 
    I really don't want to have to worry about versioning Title Data Keys
 

* ScrollList Virtualization
  * PlayFab Leaderboard Helper Code

* CatalogEditor Updates (Some of this work may already be done)
  * New Catalog Button?
  * Get Store Tab Working (if StoreItem refers to item with RM, then don't let 
    item cost be editable in the grid, just show RM)
  * Upload To PlayFab?
  * Add tools to see Estimated Realworld Money Costs for things that cost GM or CN
    * Also tool to view all ItemClass bundle types and show their discount levels
  * Ability to filter view by ItemClass?  Or buy ids that contain "blah"
  * Catalog Validation
    * VirtualCurrency Ad Can't have a value, and if RM, then must use RM defined on CatalogItem/BundleItem
    * VirtualCurrency ID is only 2 characters and not RM
    * All LocalizationStrings have valid ids
    * All CatalogItem and BundleItems have an ItemClass valid list of ItemClasses
    * StoreItem Has valid CurrencyId
    * BundleItem has valid CatalogItemId
    * RM Items only supply RM values in the Store
    * Ability to specify GameComponents to ItemClass, and make sure if they have a 
      LazyGameObject reference, then it must have that type.  Also, make sure the 
      editor has field for a LazyGameObject and filter by the GameComponent.

* If in editor, then use the CatalogVersion/CloudScriptRevision that's specified
  in the latest AppVersion in AppData.  Else, use the latest in the cloud title
  data.
  * Build process needs to Concat all AppVersions to Versions.json and upload to title data.
  * Get AppVersion, find in in Versions.json, if there is a newer version that is a DataOnlyBuild, then use that instead
    * If there is a newer version with "ForceToUpdate", then 
  * What versions does the asset bundle system save to?

* Instead of doing all the Caching in the PF Static class, should I make an 
  abstract PlayFabServer class that my game server will inherit from that will
  do all the caching?
    * Caching Server Time (make sure it gets server time when regains focus?  Make sure it isn't suseptable to clock hacking)
    * Cache Inventory
    * Cache VirtualCurrency
    * Cache Login Result
    * Events for Inventory/VirtualCurrency change
    * Initialize function
    * ExecuteCloudScript function
    * Figures out Catalog/CloudScript version by pinging the server and giving 
      the app version.
    * Caches Server Time
    * Also Cache Statistics (with update functions and events if changed)
    * GetStore(string id) // the store scriptable object will need to live in an 
      asset bundle
    * Etc.

* PF Static Class needs to Cache Virtual Currency
  * Event for when Virtual Currency changes
  * A setter class PF.SetVirutualCurrency(string id, int value);
    * This is used so if you buy something you can update it with the new values 
      without pinging the server.  This means it could be wrong, but unlikely.
  * Maybe also Cache the Inventory and have update methods for updating count 
    or things in the inventory (based off server responses).
    * Invetory Changed Event as well?
    