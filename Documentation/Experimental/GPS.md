### GPS Manager
* Android 
  * LocationService.cpp 
  * LocationInput.cpp has LocationTracker class
    * GetLastKnownLocation() returns android::location::Location
      * Which has getLatitude(), getLongitude(), getAltitude()
      * https://developer.android.com/reference/android/location/Location
  * LocationInput.cpp
    * ```RuntimeStatic<LocationTracker> s_LocationTracker(kMemInput);
    * ```const android::location::Location& location = s_LocationTracker->GetLastKnownLocation();
    * In the LocationInput namespace
 * iOS
  * iPhone_Sensors.mm
  * static LocationServiceInfo gLocationServiceStatus;
  * struct LocationServiceInfo
    * GetLocationManager() returns CLLocationManager
    * locationManager.location.coordinate.longitude

  * CLLocation
  * https://developer.apple.com/documentation/corelocation/cllocation
  * **Check this out
    * https://stackoverflow.com/questions/53046670/how-to-get-values-from-methods-written-in-ios-plugin-in-unity
