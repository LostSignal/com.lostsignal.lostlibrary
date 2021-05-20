
* Bootloader Localization Table
* Localization Manager
* 
* Whenever localization changes, make sure to send perferred language up to playfab

* PlayFab
  * Locale.getDefault.getLanguage() - Android
  * SetProfileLanguage
  * ISO 639-1 format ("es" or "ja")

* Need to support multple localization tables
  * One that is always embedded in the project that has text like 
    * Connecting to server...
    * Downloading Update...
    * All error handling messages
  * Also, connecting to Server should probably be apart of the app
  * Same with the MessageBox

* Update Lost.Localization to handle language switching and return the correct thousands seperator
  * Also they should regenerate their text
  * Would it need a callback that people can subscribe too?

* LocalizationTable
  * Need to make a LocalizedString type
  * Need a validation system to make sure all LocalizedStrings are correct
  * Custom Inspector that has a "+" button or a search button


Durring the build process, we must have a step that scans all prefabs, scripable objects and scenes for localization components and make sure that all ids they point to work.

* When you specify your laguages, auto generate an Editor C# file that adds Menu Items for them

* Localization (PlayFab based?) - Uploads each language to playfab?
  * When creating a localization table, have some hard coded values in there (Yes/No/Cancel/Loading)
    * Also PlayFab ones?

* Localization (Use Edtior Configs and generate file menu options C# class)
  * GetContentDownloadUrl

* Use LostPlayerPrefs to store the app langauge.  If the user changes the app language, or changes
  the language in teh settings, we should send a "language_changed" analytic event.



### Localization
--------------------
* Should have IntText (and any others that are applicible) listen for localization updates and refresh

* Localization
  * You input the ID, if ID doesn't exist, then it's red
  * Has button to edit in localization editor
  * If you hover over it, it shows you string value

* LocalizedString and LocalizedText (MonoBehaviour)
  * Should use the ObjectTracker class and so when we switch languages we call
    Update on all instances and auto update the on the fly and don't require a 
    restart of the app.
  
  * Lost -> Language -> English/Spanish/Etc should be menu options for switching
    between all these.


### Localization Characters
----------------------------------
```
!"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~¡¢£€¥Š§š©ª«¬®¯°±²³Žµ¶·ž¹º»ŒœŸ¿ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞßàáâãäåæçèéêëìíîïðñòóôõö÷øùúûüýþÿ 

https://en.wikipedia.org/wiki/ISO/IEC_8859-15

// ascii 20 thru 126
 !"#$%&'()*+,-./
0123456789:;<=>?
@ABCDEFGHIJKLMNO
PQRSTUVWXYZ[\]^_
`abcdefghijklmno
pqrstuvwxyz{|}~

// ascii 160 thru 255
¡¢£€¥Š§š©ª«¬®¯
°±²³Žµ¶·ž¹º»ŒœŸ¿
ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏ
ÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞß
àáâãäåæçèéêëìíîï
ðñòóôõö÷øùúûüýþÿ

// missing 160 or 00A0 (non breaking space)
// missing 173 or 00AD (soft hyphen)
```

Vietnamese (https://vietnamesetypography.com/alphabet/)
AaĂăÂâBbCcDdĐđEeÊêGgHhIiKkLlMmNnOoÔôƠơPpQqRrSsTtUuƯưVvXxYyÁáÀàẢảÃãẠạĂăẮắẰằẲẳẴẵẶặÂâẤấẦầẨẩẪẫẬậĐđÉéÈèẺẻẼẽẸẹÊêẾếỀềỂểỄễỆệÍíÌìỈỉĨĩỊịÓóÒòỎỏÕõỌọÔôỐốỒồỔổỖỗỘộƠơỚớỜờỞởỠỡỢợÚúÙùỦủŨũỤụƯưỨứỪừỬửỮữỰựÝýỲỳỶỷỸỹỴỵ
