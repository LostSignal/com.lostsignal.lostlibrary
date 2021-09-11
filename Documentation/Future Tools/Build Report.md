* Make Lost Build Reprot Tool
  * After every build it warns you, or sends slack messages of things 
    you need to fix.  Like assets with different casing or duplicate guids
	
* Maybe this just reports what the Build Validation tool has found	
  * Example Issues
    * Bad GUIDs
    * Base File Casing
    * Bad Colliders
    * Finding PSDs in Project (using PNGs isntead)
    * Track Unused Assets List
    * Track Unused Assets Size
    * Track Existence Resource Files (They should not exist)
    * Materials referencing normal map textures that aren't marked as normal maps in import settings
    * Materials that are being reference that are not in our "Approved" folder
    * Code Warnings
    * Very big audio files that aren't marked as Streaming
    * Scenes
      * Negative / Non-Uniform Scaling on Objects
      * Too Many Graphics Raycasters
