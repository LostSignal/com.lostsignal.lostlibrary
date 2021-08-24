//-----------------------------------------------------------------------
// <copyright file="FloatConversion.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.Networking
{
    internal class FloatConversion
    {
        public static float ToSingle(uint value)
        {
            UIntFloat uf = default;
            uf.intValue = value;
            return uf.floatValue;
        }

        public static double ToDouble(ulong value)
        {
            UIntFloat uf = default;
            uf.longValue = value;
            return uf.doubleValue;
        }

        public static decimal ToDecimal(ulong value1, ulong value2)
        {
            UIntDecimal uf = default;
            uf.longValue1 = value1;
            uf.longValue2 = value2;
            return uf.decimalValue;
        }
    }
}
