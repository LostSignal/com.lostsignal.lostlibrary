
* Advanced Canvas Scaler
  * Safe Area System (for iPhone X)
  * Dialog Update (DialogLogic, DialogReference, Covered/Uncovered events, screen analytics, transition manager)
    * Find Unity Blog about guid based references (create my own)
    * https://github.com/Unity-Technologies/guid-based-reference
    * https://blogs.unity3d.com/2018/07/19/spotlight-team-best-practices-guid-based-references/

* What is the best way to add logic to dialogs (like a visual scripting language) 
  * ScriptableObject event system?  
  * Have a Script Event, and every time you add an event it makes a child scriptable object under it?
  * They can inhert from a base that has a postiion, width height so we can display them on the canvas
  * Make a Dialog one, and make one for every dialog type
  * Or we could have a generic one, that you just drag a dailog onto and we can use UnityEvents to call code

* Dialog Transition System
  * Similar to how Cinemachine camera transition works

* Investigate using Canvas + CanvasGroup.alpha = 0 is bettern than enabling/disabling Canvas component

* Give Debug Menu and TextOverlay their own cameras?
  * Actuall make a layer called DebugInfo
  * Combine DebugMenu and TextOverlay

* Have Dialog class have a Initialize() function that's called before Show/Hide?
  * Make sure it's only called once
  * This way you don't have to worry about waiting for the Awake() to be called?

* What spikes the game more, toggling the Canvas component on/off, or setting all objects to an "Invisible" layer?
  * Even more interested if my PRs go through

  * Better Dialog management (Don't inherit from Dialog, instead use a Dialog Logic class)
    * Asset for Dialog Transistions (like Cinemachine)
    * Dialog have Ids that you reference
    * Make a dialog reference class so you can have prefabs reference other prefabs

* Can I make a little more code be shared between LostButton and LostToggle?
* Also make a FloatText?  With percision option. (needs to account for localization)

* Verify that BetterStringBuilder works and update IntText to use it
  * Consolidate TiemspanText and CountDownTimeText into one (update to use BetterStringBuilder)
    * Add Formats to BetterStringBuilder
    * TimespanText should have DaysLeftFormat, HoursLeftFormat, MinutesLeftFormat, SecondsLeftFormat

* Draggable Lists (Idio and Blueprint need this)

* RateApp Dialog (with Analytics, Slack, Charting, Jira integration)
  * With Feedback Section
  * Talks to Custom Server


### Dialog System Improvements
--------------------------------------
* Definitely need a DialogCovered and DialogUncovered evetnts
  * This way we can do disable canvases and particle effects to save on resources
  * Also, then Uncovered, shoudl it fire off a ScreenVisit event?

* Should I rename ShowAndHide to TransitionTo
  * Then I could make a Dialog Transition asset that specifies timeline overrides for specific transistions?
  * If not specified it just calls show, then hide

* Should I have you not inherit from Dialog?
  * Specify Dialog Name or use gameobject.name? (need validators for no two dialogs with same name)
  * This would mean I need events for OnWillShow, OnWillHide, OnShown, OnHidden
  * OnWillShow and OnWillHide will need abilty to cancel the hide/show if needed

* Dialog should have a dialog name, and whenever you change the name/layer it updates the gameobject name
  * "Layer Number - Dialog Name"

* Remove the MessageBox/Spinner/etc generation (using templates instead)

* Make all screens fire analytic event when shown (check box to opt out)
  * When a screen hides, the ScreenManager should re-fire the screen that's being revealed
 
* Auto Setting The Order in Layer?  Based on hierarchy ordering (but breaks if have custom orderings)
  * Possibly make my own canvas override that looks that the parent and then does offset of that

* Mark a dialog as Completely covers so it disables everything below it

* Make a blur component that takes a screen shot then uses that as the blocker, so almost every 
  dialog can have "Completely Covers" flag turned on.
    
* Dailog can specify that it's fullscreen, if so, then grabs all Dialogs below 
  and calls OnCovered() and when revealed again calls OnUncovered().
  * This could do a few things, disable the canvas
  * Or it could place everything on an "Invisible" layer
  * Should Definitely Fire off Analytics Screen event in OnUncovered().

* Update InputBlocker to have option to "Screenshot" background so it's always
  covering up lower dialogs.
  * Could even do something cool like do a render target of the new showing dialog
    and then do a cool wipe animation in?

* InputBlocker needs to add this code
```csharp
#if UNITY_2018_2_OR_NEWER

private void Awake()
{
    this.GetComponent<CanvasRenderer>().cullTransparentMesh = true;
}

#else

protected override void OnPopulateMesh(VertexHelper toFill)
{
    if (this.color.a == 0)
    {
        toFill.Clear();
    }
    else
    {
        base.OnPopulateMesh(toFill);
    }
}

#endif
```

* Add more Dialog Animation Types
  * SlideInFromRight, SlideInFromTop, FadeAndScaleIn, Etc

-----------------------

* StringInputDialog and RateAppDialog


* OptimizedImage?
  * Tool for repalcing Image component with OptimizedImage?
* Make RectSprite
* Make sure IntText, FloatText, TimerText all exist
  * Make handy helper class for taking numbers and a char[] and filling it out
    * Update FPS counter to use char[] instead

* Make Dialog a Sealed Class?
  * Have it create a Dialog Component and set the variables for you

* Dialog Update (DialogLogic, DialogReference, Covered/Uncovered events, screen analytics, transition manager)
  * Find Unity Blog about guid based references (create my own)
* Make an edtior script for Dailogs that lets you disable Content/Blocker with one click
* Try RectSprite system for the Main Venues Selection Dialog
* Seal the Dialog class and make a DialogLogic class
  * Add checkbox to Dialogs [x] Covers Whole screen, this way we can send events to dialogs (OnCovered, OnUncovered)
  * So when you make a new dialog, you inhere from that and it registers for all 
    the things you care about Show, OnShow, OnHide, Hide, etc
  * Give Dailogs a unique id when created
  * Give Dialogs a unique name as well (throw exception at startup if that isn't the 
  * When they Awake, have them register that unique id
  * Make a Dialog Reference class, so you can have Cross Scene references
  * Make a Cinimachine like asset that determines how Dialogs should be showm/hidden
    * Animation 1 Name -> (float delay, or wait till finished) -> Animation 2 Name
    * Also allow for playing a timeline so you can have crazy amounts of control
