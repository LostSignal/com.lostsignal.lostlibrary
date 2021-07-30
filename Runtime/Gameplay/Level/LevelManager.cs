//-----------------------------------------------------------------------
// <copyright file="LevelManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    //// * All level loads should not activate, and LevelManager should activate them manually and report stats for how long that takes.
    //// * SceneQueue
    //// * IsLevelLoaded Event
    ////   *  SceneQueue.Count == 0, LoadBalancer.IsBusy == false && SceneManager.IsLevelLoading == false
    ////
    //// * Takes a Level Asset that has a main LazyScene and a list of chunk scenes
    //// * Also has a Initial Chunks list
    //// * Level Load Process (should be an event fired so a Visual Scripting Graph can do all this
    ////   * Fades Down
    ////   * CollectGarbage
    ////   * Fades Up to Loading Environment (like Altspace/VR Chat)
    ////   * Unloads Previous Level
    ////   * Downloads Next Level (if applicable)
    ////   * Loads Next Level
    ////   * Waits for LoadBallancer.IsBusy to be false
    ////   * Fades Down
    ////   * CollectGarbage
    ////   * Fades up to new Level based on LevelStart script
    //// * LevelManager can manipulate the LoadBalancer?
    ////   * MaxMillsecondsPerFrame (in milliseconds)
    ////     * When doing a level load, it's higher than doing a hard load
    //// 
    //// * Level Manager won't load levels/chunks untill Awake/Start Managers have completed?
    ////   * Load Level -> Wait for all managers to finish ->
    ////
    //// * Should we have a "loading" layer, and during level load we only render stuff on that layer?
    ////

    //// Full Fade Down = Fade Down + Incremetnal GC + Unload Unused Assets

    //// * LevelManager
    ////   * LoadLevel(blah, skipStagging = false)
    ////   * If Editor
    ////     * Instant Fade Down
    ////     * Wait for Managers
    ////     * Run Level Load Requested with currently active scene as the arguments
    ////       * It won't actually load it since it's already loaded
    //// 
    //// ---
    //// 
    //// Level Load Requested
    ////   Full Fade Down
    ////   Turn on Loading Area
    ////   Teleport to Loading Area
    ////   Fade Up
    ////   On Complete => Level Transition Begin
    ////   
    //// Level Load Begin
    ////   Download
    ////   Load Level
    ////   Activate Level
    ////   Wait for OnManagersReady To Complete?  Or just till there are no load balancers done?
    ////     Some system will need to load chuncks depending on the players position, what will they hook into to load the chuncks?
    ////     The chunk system will need to look at the Starting position to figure out what chunks to load
    ////   Full Fade Down
    ////   Teleport to starting posotion
    ////   Fade Up
    ////   On Complete => Level Transition Finish
    ////     
    //// Level Load Finish
    ////   ??? (AI Start?)
  











    public class LevelManager : Manager<LevelManager>
    {
        public override void Initialize()
        {
            this.SetInstance(this);
        }
    }
}

#endif
