#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="AnonymousCloudFunctionAttribute.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.CloudFunctions
{
    public class AnonymousCloudFunctionAttribute : System.Attribute
    {
        public AnonymousCloudFunctionAttribute(string category, string name)
        {
            this.Category = category;
            this.Name = name;
        }

        public string Category { get; private set; }

        public string Name { get; private set; }
    }
}
