
* This code is if you're using the 3rd party plugin Dissonance for chat audio in your multiplayer game

* This code also requires my version of Dissonance which I had to made some tweaks to so that it can 
  actually be used by the package manager.

* It has a DissonanceManager that needs to be initialized in unity

* It has a DissonanceClientSubsystem and DissonanceServerSubsystem that need to be registered
  with your Lost Networking Client/Server
  
* Current requires PlayFab as well for determining player Name/Id (will hopefully remove this eventually)

* TODO
  * Remove PlayFab dependency and have a query system for Managers like IUserInfo, which PlayFab
    implements
	
	