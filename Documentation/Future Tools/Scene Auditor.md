### Scene Auditor

* Physics
  * Find all MeshColliders with transforms, or parent transforms with negative scale (these are baked at runtime)
  * Find all MeshColliders with meshes that have more than 256 polygons (can't create convex meshes out of these)
  * Maybe even listen for Debug.LogWarning "Couldn't create a Convex Mesh from source mesh "Blah""
  * Print warning if object is on the Default layer (should really make specific layers and stick to them, Default
    as a catch all leads to issues.
  
* UI Checks
  * Find All Invalid Raycast Targets (Image, Text, TMP_Text, Etc)
  * CanvasRenderer CullTransparentMesh != true
  * Make sure Dont Render Objects with Alpha 0 is checked
  * Make tool that goes over every Canvas in scene and makes sure every RectTransform has scale 1,1,1 and the Z < 100
  * Give warnings about odd scaling

* General Checks
  * Warnings
    * Give warning about any object with non-uniform scaling
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

