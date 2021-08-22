//-----------------------------------------------------------------------
// <copyright file="ISetting.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;

    public interface ISetting<T>
    {
        T Value { get; set; }

        //// TODO [bgish]: Later add OnChanged event (whenever the value changes)
        //// TODO [bgish]: Later add OnUpdated event (whenever a new value is committed/saved)
    }

    public interface IFloatSetting : ISetting<float>
    {
    }

    public interface IBoolSetting : ISetting<bool>
    {
    }

    public interface IIntSetting : ISetting<int>
    {
    }

    public interface IStringSetting : ISetting<string>
    {
    }

    public interface IDateTimeSetting : ISetting<DateTime>
    {
    }
}
