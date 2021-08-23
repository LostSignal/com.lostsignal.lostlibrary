//-----------------------------------------------------------------------
// <copyright file="ShowEditorWarningAttribute.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    public sealed class ShowEditorWarningAttribute : System.Attribute
    {
        public ShowEditorWarningAttribute(string text) => this.Text = text;

        public ShowEditorWarningAttribute() => this.Text = null;

        public string Text { get; private set; }
    }
}
