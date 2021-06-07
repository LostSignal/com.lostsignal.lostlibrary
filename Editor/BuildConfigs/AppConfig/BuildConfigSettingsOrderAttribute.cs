//-----------------------------------------------------------------------
// <copyright file="BuildConfigSettingsOrderAttribute.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.BuildConfig
{
    using System;

    public class BuildConfigSettingsOrderAttribute : Attribute
    {
        public int Order { get; private set; }

        public BuildConfigSettingsOrderAttribute(int order)
        {
            this.Order = order;
        }
    }
}
