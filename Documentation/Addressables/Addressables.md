
* This is a wrapper around Unity's Addressable system.  They are called Lazy objects.
* The nice thing about these is that they can be serialized and deserialized outside of unity, 
  so you can store them as json on your backend and load them.


# Notes

https://forum.unity.com/threads/change-remote-catalog-load-path-at-runtime.712052/
```
There are a couple ways to do this. Most involve setting your RemoteLoadPath to include a variable you define yourself. Note that we control local builds by setting the load path to
{UnityEngine.AddressableAssets.Addressables.RuntimePath}/[BuildTarget]
The [] brackets are evaluated at build time. the {} at runtime. So if you declared some string MyApp.Something.URL, you could set your load path to:
{MyApp.Something.URL}/whatever (maybe [BuildTarget] or not).
```

### Addressables
--------------------

* Make sure uploading addressables to S3 works
  * Also it should throw a build exception if the folder doesn't exist, but IsBeingUsedByAddressableSystem is true
    * Or will a Debug.LogError work just fine?
  * Make sure Azure and S3 throw build exception if unable to upload a file
  * Remove the EditorApplication.Exit(1); from the azure uploading, should throw build exception instead



  * Loading asset bundles from different locations require you to extend ResourceManager.  If you inherit from IResourceProvider you can provide your own custom providers.  This would be needed for things like PlayFab.

* How do you add more rules to the Addressables Analyze section?

* All code and comments are in the Lost Packages - Build Config project
* Once Build config is working and sharable to the public, make sure S3 uploading works
* Let the people at unity know how I do it and see if this is "OK", or if there is a better way to do it
  * How long till we can override the saving process?  These shouldn't go to disk, but to S3
* Can I change the RemoteBuildPath at runtime and reload addressables
* Prepare Step and Build Step?
* How do I update the catalog location, still fuzzy on this
* How does Adressables point to the catalog, and how do I overwrite it?
  * Can I do it at runtime?
