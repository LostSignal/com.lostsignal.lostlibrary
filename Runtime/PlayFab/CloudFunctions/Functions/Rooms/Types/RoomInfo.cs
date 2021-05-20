//-----------------------------------------------------------------------
// <copyright file="RoomInfo.cs" company="Giant Cranium">
//     Copyright (c) Giant Cranium. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.CloudFunctions
{
    using System.Collections.Generic;

    public class RoomInfo
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public List<string> ImageUrls { get; set; }
    }
}
