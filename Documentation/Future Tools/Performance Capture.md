* Tool uses the new runtime profiler API to collect stats while playing a level.
  * You select what you want it to collect and it will serialize it to a binary file every tick
  * Once you switch levels the file will be uploaded to blob storage/s3
  * It will also insert a line in the database with meta data about the playthrough for later searching for relavant data
  
  * Example Stats
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
	  * Garbage Collected
	  * Garbage Created
	  * Debug.Log, LogWarning, LogError, LogException, LogAssert
	  * Level Activation times for chunks