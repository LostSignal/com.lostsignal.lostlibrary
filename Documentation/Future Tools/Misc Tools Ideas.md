

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

* Could I make an attribute [GetComponent], and put that on serialized fields and in Editor (OnAfterDeserialize), if that component is null then set it, and in editor, OnBeforeSerialize if it's null then set it.  If the component doesn't 
  * Would require us to have a custom MonoBehaviour we inherit from
  * Other Attributes
    * [FindObjectOfType]
    * [GetComponentInParent]
    * [AssertNotNull]
  * Should throw errors in the editor if it couldn't find the component/type it was looking for

