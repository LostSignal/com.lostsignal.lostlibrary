//-----------------------------------------------------------------------
// <copyright file="ShowEditorErrorAttribute.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    public class ShowEditorErrorAttribute : System.Attribute
    {
        public ShowEditorErrorAttribute(string text) => this.Text = text;

        public ShowEditorErrorAttribute() => this.Text = null;

        public string Text { get; private set; }
    }
}
