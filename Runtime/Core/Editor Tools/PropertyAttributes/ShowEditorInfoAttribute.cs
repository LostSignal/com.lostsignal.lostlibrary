//-----------------------------------------------------------------------
// <copyright file="ShowEditorInfoAttribute.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    public sealed class ShowEditorInfoAttribute : System.Attribute
    {
        public ShowEditorInfoAttribute(string text) => this.Text = text;

        public ShowEditorInfoAttribute() => this.Text = null;

        public string Text { get; private set; }
    }
}
