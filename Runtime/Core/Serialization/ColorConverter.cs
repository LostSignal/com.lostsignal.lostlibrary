#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="ColorConverter.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using Newtonsoft.Json;

    public class ColorConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(UnityEngine.Color);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return ColorUtil.ParseColorHexString((string)reader.Value);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var color = (UnityEngine.Color)value;
            serializer.Serialize(writer, ColorUtil.ConvertToHexString(color));
        }
    }
}
