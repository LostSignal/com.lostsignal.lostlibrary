//-----------------------------------------------------------------------
// <copyright file="ExposeInEditorAttribute.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    public sealed class ExposeInEditorAttribute : System.Attribute
    {
        public ExposeInEditorAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }
    }
}
