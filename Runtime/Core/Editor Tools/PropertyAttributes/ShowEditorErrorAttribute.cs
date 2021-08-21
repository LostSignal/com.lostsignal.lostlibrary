#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="ShowEditorErrorAttribute.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------


namespace Lost
{
    public class ShowEditorErrorAttribute : System.Attribute
    {
        public string Text { get; private set; }

        public ShowEditorErrorAttribute(string text) => this.Text = text;

        public ShowEditorErrorAttribute() => this.Text = null;
    }
}
