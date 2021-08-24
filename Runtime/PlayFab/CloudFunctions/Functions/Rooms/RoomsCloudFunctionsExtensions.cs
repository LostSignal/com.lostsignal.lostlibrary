// <auto-generated/>
#pragma warning disable

#if UNITY_2018_3_OR_NEWER

namespace Lost.CloudFunctions
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Lost.CloudFunctions.Common;
    using Lost.CloudFunctions.Debug;
    using Lost.CloudFunctions.Login;

    public static class RoomsCloudFunctionsExtensions
    {
        public static Task<ResultT<List<Room>>> Rooms_GetPublicRooms(this CloudFunctionsManager cloudFunctionsManager) => cloudFunctionsManager.Execute<List<Room>>("Rooms_GetPublicRooms");

        public static Task<ResultT<RoomServerInfo>> Rooms_EnterRoom(this CloudFunctionsManager cloudFunctionsManager, string request) => cloudFunctionsManager.Execute<RoomServerInfo>("Rooms_EnterRoom", request);
    }
}

#endif
