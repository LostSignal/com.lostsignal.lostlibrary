//-----------------------------------------------------------------------
// <copyright file="LostMessages.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    public static class LostMessages
    {
        public static UnityTask<YesNoResult> ShowUnableToConnectToServer()
        {
            return MessageBox.Instance.ShowYesNo("Server Error", "We're unable to connect to the server at the time.  Would you like to try again?");
        }

        public static void BootloaderLoggingIn()
        {
            Bootloader.UpdateLoadingText("Logging In...");
        }

        public static void BootloaderDownloadingCatalog()
        {
            Bootloader.UpdateLoadingText("Downloading Catalog...");
        }

        public static void BootloaderLoadingStores()
        {
            Bootloader.UpdateLoadingText("Loading Stores...");
        }

        public static void BootloaderInitializingPurchasing()
        {
            Bootloader.UpdateLoadingText("Initializing Purchasing...");
        }
    }
}
