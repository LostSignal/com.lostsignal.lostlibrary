
* Make a custom mesh importer that looks for sub meshes that have a name that ends in _collision_mesh
  and adds a CollisionMesh component to it.
  * _collision_box
  * _collision_sphere
  * etc...
  * Also make sure it doesn't have a mesh renderer

* Can I make a tool to populating the Game window resolutions?

## Tools

* New Project Setup Tool
  * [ ] Add .editorconfig
  * [ ] Add .p4ignore, ignore.conf, .gitignore or something else?
  * [ ] Create Game Server Project Generator
  * [ ] Create Azure Functions Project Generator
  * [ ] Use StyleCop?
    * **How do we even do this?  2020.2+ Only

* Make Lost Build Reprot Tool
  * After every build it warns you, or sends slack messages of things you need to fix.  Like assets with different casing or duplicate guids

* REMOTE TOGGLE TOOL
  * Connects to unity and gets all objects in teh scene
  * Lets you remotely enable/disable gameobjects

* NEED PROJECT AUDITOR TOOL
  * Audio, Meshes, Normal Maps, Etc
  * Scan all code bad line endings and print warnings if find files with bad line endings or messed up line endings "\r\r\n"

* NEED SCENE REPORT TOOL
  * Make tool that goes over every Canvas in scene and makes sure every RectTransform as 1,1,1 scale and the Z < 100
  * Warnings
    * Objects with disabled mesh filters (for what seems to be no reason)
    * Objects with textures that have no compression
  * Errors
    * Objects with normal textures, but textures aren't marked as normal, mip maps
  * Objects Page
    * A list of all the objects with tabs for vertex count and tri count (if you click it will sort by it)
  * Baking Page
    * Breakdown of all objects that can be "baked" together
  * Lighting Page
    * Breakdown of lights and how many object they affect?
  * Badness Heatmap (counts verts/tris in an heatmap to find problem areas)
  * OVERLAPPING TRIGGERS???
  * https://www.youtube.com/watch?v=i2IpJHUyZLM
  * Does timeline fly through of level and records stats
    * Fixed Update Calls
    * Fixed Update, Animator Update, GameObject Update
    * Render Time, CPU Time
    * FPS, Memroy, Verts, Tris
    * SetPass Calls
    * Draw Calls
    * Memory
    * Overdraw levels
    * Etc
    * Active Animators, Timelines, Skined Mesh Renderers
    * Physics
      * Active Rigidbodies (non-kinematic)






# Add Code Analysis To Project
* Just adding this to the csproj will run stylecop
  <ItemGroup>
    <AdditionalFiles Include="stylecop.json" />
  </ItemGroup>
  <ItemGroup>
    <Analyzer Include="packages\StyleCop.Analyzers.1.0.2\analyzers\dotnet\cs\StyleCop.Analyzers.CodeFixes.dll" />
    <Analyzer Include="packages\StyleCop.Analyzers.1.0.2\analyzers\dotnet\cs\StyleCop.Analyzers.dll" />
  </ItemGroup>
* If I put my analyzer in a asembly definition file, can I link it?



  <ItemGroup>
    <Analyzer Include="obj\Debug\Lost.Analyzers.dll" />
    <Analyzer Include="C:\Users\User\Desktop\CodeAnalysisTest\Library\PackageCache\com.unity.code-analysis@0.0.1-preview.3\Plugins\Editor\Microsoft.CodeAnalysis.dll" />
  </ItemGroup>

{
  "dependencies": {
    "com.unity.code-analysis": "0.0.1-preview.3"
  }
}

* Enable XML Documentation is difficult with Unity
  * https://answers.unity.com/questions/1140063/cant-open-project-property-in-visual-studio-2015-w.html
  * Must allow Unity VS Plugin to let you edit a csproj properties
  * In the csproj properties, go to Build section and enable xml output path

* Can Unity point to NuGet as a dependency?
  * https://www.nuget.org/packages/stylecop.analyzers/
  * https://github.com/DotNetAnalyzers/StyleCopAnalyzers

https://github.com/meng-hui/UnityEngineAnalyzer
https://github.com/vad710?tab=repositories

* CS Proj needs the following
```
  <ItemGroup>
    <None Include="app.config" />
    <None Include="packages.config" />
    <AdditionalFiles Include="stylecop.json" />
  </ItemGroup>
```

* packages.config
```xml
<?xml version="1.0" encoding="utf-8"?>
<packages>
  <package id="StyleCop.Analyzers" version="1.0.2" targetFramework="net471" developmentDependency="true" />
  <package id="Visual-StyleCop.MSBuild" version="4.7.59.0" targetFramework="net471" />
</packages>
```

* stylecop.json
```json
{
  // ACTION REQUIRED: This file was automatically added to your project, but it
  // will not take effect until additional steps are taken to enable it. See the
  // following page for additional information:
  //
  // https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/EnableConfiguration.md

  "$schema": "https://raw.githubusercontent.com/DotNetAnalyzers/StyleCopAnalyzers/master/StyleCop.Analyzers/StyleCop.Analyzers/Settings/stylecop.schema.json",
  "settings": {
    "documentationRules": {
      "companyName": "Lost Signal LLC"
    }
  }
}
```



--------------------------------------------------------------------------------------------













* Lost Tools
  * UI
    * Import All
    * Import What's New Dialog
    * Import Rate Me Dialog
    * Import Debug Menu Dialog
    * Import MessageBox Dialog
    * Import Loading Dialog
    * Import Generic Dialog Animations
  * Find All Invalid Raycast Targets
  * Generate Default Best Practices Presets
  * Remove Empty Directories
  * Fix Line Endings
  * Validate Scene
  * Validate Game


* Tool (Editor) to search for Cache Server on local network
  * Can you ping CacheServer?
  * Test if Port 8126 is open http://stackoverflow.com/questions/11837541/check-if-a-port-is-open


### Build Validation Tool
----------------------------------
* Check out this validator https://github.com/DarrenTsung/DTValidator

* If there is a UI element who's z != 0, print warning

* Is OnValidate called at build time?  If so, if I throw a build exception will it fail the build?

* CalledByUnityEvent Attribute (See Experimental Folder in Lost Library)

* Validators!!!!
  * Can run on scene open
  * Print warnings/errors
  * Fail builds (must check every ScriptableObject, Prefab, Scene)
  * etc

* No two dialog with the same name (so Screen analytics work)
* Print warning if you have two dialog on the same layer?

* InitializeOnLoad grab every object in the scene and recursively look for these attributes

* Need to make my system that processes all scenes/prefabs and prints build warning/errors if things aren't working
  * RectTransforms scale are 1,1,1
  * Button has no Raycast Targets
  * [CalledByUnity] for UI
  * LazyAssets, Localization Ids, Settings Strings point to real data
  * All MonoBehaviorus with [Validate] methods get called and results are part of build results
  * Have menu items for "Validate Scene", "Validate Folder", "Validate All" so you can get the output without making build
    * Possibly register for Debug.LogError/LogWarning events and throw build exceptions if those things happen?
  * Presets References in folder meta files are valid
  * Validate On Play Mode Start????


### Quick and easy tools to do now
--------------------------------------------
* Create CodeAsset and FolderAsset types
* Make a MenuItem Lost/Logs/Editor Log that open up explorer here: C:\Users\username\AppData\Local\Unity\Editor\Editor.log
* Make a MenuItem Lost/Logs/Player Log that open up explorer here: C:\Users\username\AppData\LocalLow\CompanyName\ProductName\output_log.txt


### CachedUrlImage Utility
---------------------------------
* Create a CachedUrlImage class.  Takes a URL, has a loading animation, and
  caches the result for later. It will most likely be used for profile pictures.


  * URL To Image Util (Does this actually work?)
  ```
  var url = "http://URL to image...";
  var tex = new Texture2D(4, 4, TextureFormat.DXT1, false);
  var www = new WWW(url);
  www.LoadImageIntoTexure(tex);
  spriteRenderer.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height, new Vector2(0.5f, 0.5f), 100.0f);
  ```
  * Texture2D.Compress - https://docs.unity3d.com/ScriptReference/Texture2D.Compress.html
  * https://docs.unity3d.com/ScriptReference/Texture2D.LoadRawTextureData.html


### What's New System
--------------------------
* AppVersion
   * int CloudScriptRevision;
   * string CatalogVersion;
   * bool DataOnlyBuild;
   * LocString Description;
   * LocString[] BulletPoints;
   * bool IsLive;
   * bool ForceToUpdate;
   * Add a Coming soon field?
   * AssetBundle Version?!?!?!?!?
   
* What's New System (apart of AppSettings?)
* Make a what's new Dialog (it can probably take care determining if it has shown that info yet)


### AppSettings Improvements
-----------------------------------
* Make static App Class that tracks login count and last login version
  * Make RateMe Dialog
  * Make What's New

* Would it be worth it to investigate some sort of visual scripting that's downloaded on startup?
  * One use case would be CheckIfShouldShowRateMe().  We may want to change that logic after ship,
    maybe it's when you enter a new PlayFab segment, maybe it's when you just opened your first
    briefcase?
  * It could tap into the analyics system to figure this out.  "OpenBriefcase" event, happened within
    1 minute of "Visit MainMenu" event.

* Have runtime App class that loads the config.json and has useful things like:
  * First time running app
  * App.ShowRateMe();  (Makes sure to check PlayerPrefs for "Never Show This Again")
  * App.ShowWhatsNew(); (Only show's it if FirstTimeRunningApp == false, and FirstTimeRunningThisVersion == true)
  * App.NumberOfTimesOpened;

* Update to use new Editor Config stuff

* Look at Unity source and figure out how to read/write to the ProjectSettings folder
  * Would be nice to make sure that SourceControlProvider works with this too
* If I store AppSettings in ProjectSettings (as json), can I load it in "InitializeOnLoad"?
* Unity Cloud build should just work with this BuildConfiguration object
* Add CommandLine argument for -BuildConfig=Dev
* Should be able to switch configurations from the file menu
  * Create a IAssetBundleSource class
    * Have a default StreamingAssetsSource
    * Have a default S3Source
    * Have a default AzureBlobSource
    * Have a default PlayFabSource
    * MoveAssetBunde(string absoluteFilePath, string assetPath)
    * GetAssetBundle(string assetPath)
    * Definitely make sure my S3 uploader jar only has packages it needs (should be much much smaller)
* Does AppSettings show warnings if meta files aren't visible and assets aren't forced to text?
* Maybe have AppSettings.Orientation set the differnet modes for you?
* Add Cache Server setting to App class.  Can i specify a different IP for every version of Unity that has loaded the project?
* Make Disable BitCode code only run on iOS (no point in showing it on Android)
  * Move it to Platform class?
* bool AppSettings.IsFirstTimeRunningApp
* bool AppSettings.StartupCount
* bool AppSettings.isApplicationQutting?
* Print Warnings if "Force Text" or "Visible Meta Files" is off
* Add P4 Server Url and set for user (Not sure how to do this yet)
* Put Bundle Identifier in Project Settings
* Fix for AppSettings being cached in editor?  Portrait doesn't update correctly because of it
  * Possibly use AssetDatabase.LoadAssetAtPath instead of Resources.Load for accessing AppSettings in the editor.
  * EditorUtility.SetDirty(globals);
  * AssetDatabase.StopAssetEditing();
  * AssetDatabase.SaveAssets();
* Add Development Mode to AppSettings
  * EditorUserBuildSettings.development = true or false
* Print Warning if "Force Text Serialization" is off
* Print Warning if "Visible Meta Files" is off
* Set the editor playmode tint for you!
* Detect AndroidManifest to make sure bundle identifier is set corrctly

* If user runs editor and there is no .p4config file, then prompt them to create one
  * A sample P4CONFIG file might contain the following lines:
    * P4CLIENT=joes_client
    * P4USER=joe
    * P4PORT=ssl:ida:3548
    * P4IGNORE=.p4ignore
  * If this file exists, then make sure to set above P4 editor settings to those values

* Ability to specify Cache Server (Based on Unity Version) - Have it set at startup
  * EditorPrefs.SetBool("CacheServerEnabled", true);
  * EditorPrefs.SetString("CacheServerIPAddress", IPaddr + ":" + portNum);
  * If (!UnityEditorInternal.InternalEditorUtility.CanConnectToCacheServer())

* AssetBundles
  * In the AssetBundle section of AppSettings.  There should be a sectoin of 
    all the asset bundles in the project with check marks next to them.  All 
    the ones you check will be put in the StreamingAssets durring the build 
    process.


### Rosyln Analyzers
--------------------------
* When Unity update to Rosyln, can we have StyleCop, FX Cop and Code analysis
  (including custom code analysis) inside of the unity editor (also, does this
  mean warnings as error can be on by default.
  * Editor flag for easily switching between warnings as error or not
  * Code analysis at cloud build time (failing build)
  * Visual studio 2017, "code style configuration" enforcement (new: on builds)
  * FX Cop Rules as Rosyln code processors (VS Upate 3)
  * https://www.youtube.com/watch?v=VxeC7WFfg3Q&t=1733s
  * Original - https://github.com/meng-hui/UnityEngineAnalyzer
    * Fork - https://github.com/vad710/UnityEngineAnalyzer

* Forbid new WaitForSeconds (tell them to use Lost.WaitFor.Seconds(float)

* Forbid Vector3.one (tell them to use Lost.Constants.Vector3.one)

* Forbid Animator.SetTrigger(string), SetBool(string), SetFloat(string), SetInteger(string)
  * Should cache the string to an int Animator.SetringToHash(string);

* Forbid Material.SetColor(string), SetFloat(string), SetInt(string) or SetTexture(string)
  * Should cache the string to an int Shader.PropertyToID(string)

* Write Rosyln things to detect if someone accidently yeilds on an IAsyncRequest,
  tell them it wont work and add .AsIEnumerator() to the end of it.

* Integrate and write lots of C# Rosyln Analyzers
  * All overrided classes/methods are sealed
  * Dictionaries Keys must have Object.Equals,


### Global PostProcessor
------------------------------
* Editor
  * Presets
    * Defaults
      * Mesh.preset, CanvasRender.preset, TexturePreset.preset, etc
    * Sprite.preset
    * UISprite.preset

* Create Best Practices Default Presets

* Lost -> Presets -> Generate Default Best Practices Presets
  * CanvasRenderer (Cull Transparent Mesh = true)
  * Image (Raycast Target = false)
  * Text (Raycast Target = false)
  * TextMeshPro_UGUI (Raycast Target = false)
  * See below for more

* Asset Import Hierachy System
  * Make it a folder editor (stored in folder meta file)
  * Make sure same tech works for build configuration system)
  * Rig -> Optimize Game Object (true)
  * Particle System -> withChildren (false)
    * If you do have multiple particle system children, make component 
      that will cache them on Awake and then call Start/Stop on the list

* Would Presets help with this?
* Directory Hierarchy (with inheritence?)
  * Scriptable object at the root of the project or store info in directory meta file
* Setting asset bundle names and packing tags (using string format "{ParentDir}\Blah"
  * FileName, DirName, ParentDirName, ParentParentDirName
* Default models to not have animators
* Default models read/write to false
* Default rigs to Optimize Animation (which doesn't expose bones)
* Default optimizing
* Make asset pre-processor to make sure read/write is disabled for textures
  * also make sure texture are compressed
* If it's a mesh, and it doesn't have a MeshCollider, then turn off read/write for those too
* ParticleSystems have a "withChildren" attribute that defaults to true, should be false
  * Look up why ```https://www.youtube.com/watch?v=_wxitgdx-UI``` 4:00ish minutes in
* Meshes should turn off Blend shaps if don't use them (faster import time)
* Optimize Game Objects (True)
* Things to Edit
  * Turn off Rig if not needed
  * Turn off Animator if not needed
  * Animation Comppression
  * Model Compression
  * Texture Compression
  * Optimize Rig
  * Etc



### GUID Reference System
--------------------------------
* Cross Scene/Prefab Guid Reference
  * TransformReference
  * RectTransformReference
  * GameObjectReference
  * DialogReference


### Misc Tools
--------------------------------
* Need to make a suite of SubAnimation tools, Adding/Renaming/Deleting etc.

* Make some sort of Editor Toast system
  * Since artists/designer NEVER look at console errors, I can use this system to promote 
    really bad bugs.

* Find Raycast Targets
  * Scans the Scene
  * Puts all the objects in a list (with check box next to it)
    * You can toggle the Raycast Target property from that check box
  * Clicking an object selects it in the hierarchy)
  * Add Button to Refresh List
  * It should ignore images with buttons components on it

* [ExposeInEditor]
  * A cool Custom Editor to expose a button to Unity Editor so you don't need a custom one

* Could I make an attribute [GetComponent], and put that on serialized fields and in Editor (OnAfterDeserialize), if that component is null then set it, and in editor, OnBeforeSerialize if it's null then set it.  If the component doesn't 
  * Would require us to have a custom MonoBehaviour we inherit from
  * Other Attributes
    * [FindObjectOfType]
    * [GetComponentInParent]
    * [AssertNotNull]
  * Should throw errors in the editor if it couldn't find the component/type it was looking for

