### Scene Auditor
* Make tool that goes over every Canvas in scene and makes sure every RectTransform as 1,1,1 cale and the Z < 100
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

