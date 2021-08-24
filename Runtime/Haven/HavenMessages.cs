//-----------------------------------------------------------------------
// <copyright file="HavenMessages.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost.Haven
{
    using Lost;

    public static class HavenMessages
    {
        public static UnityTask<StringInputResult> EnterPrivateExpierenceId()
        {
            return StringInputBox.Instance.Show("Room", "Enter the Id of the room you'd like to join", string.Empty, 4);
        }

        public static UnityTask<YesNoResult> ShowWouldYouLikeToQuit()
        {
            return MessageBox.Instance.ShowYesNo("Quit?", "Would you like to quit Hourglass Escapes?");
        }

        public static UnityTask<YesNoResult> ShowUnableToConnectToServer()
        {
            return MessageBox.Instance.ShowYesNo("Server Error", "We're unable to connect to the server at the time.  Would you like to try again?");
        }

        public static void ShowUnknownErrorTryAgainLater()
        {
            MessageBox.Instance.ShowOk("Server Error", "An unknown error occured, please try again later.");
        }

        public static void ShowCouldNotFindRoom()
        {
            MessageBox.Instance.ShowOk("Error", "We couldn't find a game with that room name.");
        }
    }
}

#endif
