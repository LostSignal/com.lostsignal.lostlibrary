### Remote Debugger
* Connects to unity and gets all objects in teh scene
* Lets you remotely enable/disable gameobjects
* If you put [RemoteFuntion("Blah")] over any methods, this will make it accessible to call from the RemoteInspector
  * Will need to make sure you put a [Preserve] attribute as well so the code is not stripped
  * TO DO
    * Add my own PreserveAtribute class, because it will also be respected by the Unity code stipper 
 