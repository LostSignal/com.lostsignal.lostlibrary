### Project Auditor
* Audio, Meshes, Normal Maps, Etc
* Scan all code bad line endings and print warnings if find files with bad line endings or messed up line endings "\r\r\n"


* Asset Import Hierachy System
  * Make it a folder editor (stored in folder meta file)
  * Make sure same tech works for build configuration system)
  * Rig -> Optimize Game Object (true)
  * Particle System -> withChildren (false)
    * If you do have multiple particle system children, make component 
      that will cache them on Awake and then call Start/Stop on the list



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

