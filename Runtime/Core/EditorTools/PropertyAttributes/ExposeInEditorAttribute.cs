//-----------------------------------------------------------------------
// <copyright file="ExposeInEditorAttribute.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------


namespace Lost
{
    public class ExposeInEditorAttribute : System.Attribute
    {
        public string Name { get; set; }


        public ExposeInEditorAttribute(string name)
        {
            this.Name = name;
        }
    }
}
