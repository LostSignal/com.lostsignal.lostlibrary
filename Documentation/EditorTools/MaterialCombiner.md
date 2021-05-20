
* Create New Shadergraph Shaders
* Setup Texture Array Creators (List of Material Combiners?)
* Setup OptimizerSettings Asset
  * Material Remap (Optional)  
  * Scene Output location  "{scene_directory}/Optimized Meshes/{gameobject_name}"
  * Prefab Ouput location  "{prefab_directory}/{prefab_name}_LOD{LOD}_Submesh{Submesh}"
  * LODs
    * string name;
    * float quality;
    * float screenPercentage;
	* bool generateLocalMesh;  // Is this neccessary???
    * MeshSimplifier simplifier;  // UnityMeshSimplifier or Simplygon
   

* If One material uses normals, but another does, is it worth it to combine or keep seperate?
   
* https://docs.unity3d.com/Manual/StandardShaderMaterialParameterNormalMap.html
  * For example, an RGB value of (0.5, 0.5, 1) or #8080FF in hex results in a vector of (0,0,1) which is “up” for the purposes of normal-mapping
   
   
If a Slice has multiple materials, then it will atlas them!
   * https://docs.unity3d.com/ScriptReference/Texture2D.PackTextures.html
 
 
Make Scnee Material Replacer Tool (More or less for debug)
  * Place in scene and it should give you stats (unique material count)
  * You drag in your MaterialCombiner tool
  * You can then do "replace all materials" this will create unique meshes for everything though
  * Or, it can just tell you how much it thinks it can save you
   
Lit Metalic Opaque Base, Normal
Lit Metalic Transparent Base, Metalic, Smoothness  


Check box for Generate Metalic/Smoothness Maps
Check box for Generate Normal Maps
It matches the BaseMap 1 to 1, but if it detect that there are no actual textures in the Metalic/Normal, then 
it will give the option to override the size of the TextureArray????

Smoothness Source = Metalic Alpha or Albedo Alpha

Tool needs to make sure all Source art doesn't have Ocluder/Ocludee static, only optimized assets should have that
   
Material Replacer Tool
  Slice
    List<Material>
   
  Input Materials
  New Material
  
Material Combiner?
  MatInfo GetMaterialInfo(Material) // bool Exists, Slice Index, UV Offset, UV Scale
  Input Materials
  string[] TextureProperties (_BaseMap (format, generate mips, etc), _BumpMap (format, generate mips, etc))
  Spits out warnings
    Texture Size Mismatch
	Texture Format Mismatch
  New Shader 
  Output Results Material
  This will create TextureArray assets next to the output material and set those values for you


struct
  