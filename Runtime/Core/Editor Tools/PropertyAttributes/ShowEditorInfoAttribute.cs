#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="ShowEditorInfoAttribute.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------


namespace Lost
{
    public class ShowEditorInfoAttribute : System.Attribute
    {
        public string Text { get; private set; }

        public ShowEditorInfoAttribute(string text) => this.Text = text;

        public ShowEditorInfoAttribute() => this.Text = null;
    }
}
