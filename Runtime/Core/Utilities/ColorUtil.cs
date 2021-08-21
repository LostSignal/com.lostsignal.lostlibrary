#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="ColorUtil.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using System.Text;

    public static class ColorUtil
    {
        private static readonly Dictionary<char, int> hexToDecimal = new Dictionary<char, int>()
        {
            { '0', 0 }, { '1', 1 }, { '2', 2 }, { '3', 3 }, { '4', 4 },
            { '5', 5 }, { '6', 6 }, { '7', 7 }, { '8', 8 }, { '9', 9 },
            { 'a', 10 }, { 'A', 10 },
            { 'b', 11 }, { 'B', 11 },
            { 'c', 12 }, { 'C', 12 },
            { 'd', 13 }, { 'D', 13 },
            { 'e', 14 }, { 'E', 14 },
            { 'f', 15 }, { 'F', 15 },
        };

        private static readonly char[] decimalToHex = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

        public static UnityEngine.Color ParseColorHexString(string colorHexString)
        {
            float r = 1.0f;
            float g = 1.0f;
            float b = 1.0f;
            float a = 1.0f;

            if (colorHexString.Length == 3)
            {
                r = hexToDecimal[colorHexString[0]] << 4 | hexToDecimal[colorHexString[0]];
                g = hexToDecimal[colorHexString[1]] << 4 | hexToDecimal[colorHexString[1]];
                b = hexToDecimal[colorHexString[2]] << 4 | hexToDecimal[colorHexString[2]];
            }
            else if (colorHexString.Length == 6)
            {
                r = hexToDecimal[colorHexString[0]] << 4 | hexToDecimal[colorHexString[1]];
                g = hexToDecimal[colorHexString[2]] << 4 | hexToDecimal[colorHexString[3]];
                b = hexToDecimal[colorHexString[4]] << 4 | hexToDecimal[colorHexString[5]];
            }
            else if (colorHexString.Length == 8)
            {
                r = hexToDecimal[colorHexString[0]] << 4 | hexToDecimal[colorHexString[1]];
                g = hexToDecimal[colorHexString[2]] << 4 | hexToDecimal[colorHexString[3]];
                b = hexToDecimal[colorHexString[4]] << 4 | hexToDecimal[colorHexString[5]];
                a = hexToDecimal[colorHexString[6]] << 4 | hexToDecimal[colorHexString[7]];
            }

            return new UnityEngine.Color(r, g, b, a);
        }

        public static string ConvertToHexString(UnityEngine.Color color)
        {
            int r = (int)(color.r * 255.0);
            int g = (int)(color.g * 255.0);
            int b = (int)(color.b * 255.0);
            int a = (int)(color.a * 255.0);

            if (color.a == 1.0f)
            {
                StringBuilder builder = new StringBuilder(7);

                return builder
                    .Append(decimalToHex[r >> 4])
                    .Append(decimalToHex[r & 15])
                    .Append(decimalToHex[g >> 4])
                    .Append(decimalToHex[g & 15])
                    .Append(decimalToHex[b >> 4])
                    .Append(decimalToHex[b & 15])
                    .ToString();
            }
            else
            {
                StringBuilder builder = new StringBuilder(9);

                return builder
                    .Append(decimalToHex[r >> 4])
                    .Append(decimalToHex[r & 15])
                    .Append(decimalToHex[g >> 4])
                    .Append(decimalToHex[g & 15])
                    .Append(decimalToHex[b >> 4])
                    .Append(decimalToHex[b & 15])
                    .Append(decimalToHex[a >> 4])
                    .Append(decimalToHex[a & 15])
                    .ToString();
            }
        }
    }
}
