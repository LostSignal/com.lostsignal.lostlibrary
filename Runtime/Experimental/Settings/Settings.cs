//-----------------------------------------------------------------------
// <copyright file="Settings.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public abstract class Settings
    {
        private readonly List<ISetting> settings = new List<ISetting>();

        private readonly object readWriteLock = new object();

        [SerializeField]
        private SettingsData data;

        protected Settings(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                this.data = new SettingsData();
            }
            else
            {
                this.data = JsonUtil.Deserialize<SettingsData>(json);
            }
        }

        private interface ISetting
        {
            bool IsDirty { get; }

            void Commit();

            void Revert();
        }

        public bool IsDirty
        {
            get
            {
                lock (this.readWriteLock)
                {
                    foreach (var setting in this.settings)
                    {
                        if (setting.IsDirty)
                        {
                            return true;
                        }
                    }

                    return false;
                }
            }
        }

        public void Commit()
        {
            lock (this.readWriteLock)
            {
                foreach (var setting in this.settings)
                {
                    setting.Commit();
                }
            }
        }

        public void Revert()
        {
            lock (this.readWriteLock)
            {
                foreach (var setting in this.settings)
                {
                    setting.Revert();
                }
            }
        }

        public string SerializeToJson()
        {
            lock (this.readWriteLock)
            {
                // commits everything before serializing
                foreach (var setting in this.settings)
                {
                    setting.Commit();
                }

                return JsonUtil.Serialize(this.data);
            }
        }

        protected IBoolSetting GetBoolSetting(int key, bool defaultValue)
        {
            var setting = new BoolSetting(this.data.BooleanValues, key, defaultValue);
            this.settings.Add(setting);
            return setting;
        }

        protected IStringSetting GetStringSetting(int key, string defaultValue)
        {
            var setting = new StringSetting(this.data.StringValues, key, defaultValue);
            this.settings.Add(setting);
            return setting;
        }

        protected IIntSetting GetIntSetting(int key, int defaultValue)
        {
            var setting = new IntSetting(this.data.IntValues, key, defaultValue);
            this.settings.Add(setting);
            return setting;
        }

        protected IFloatSetting GetFloatSetting(int key, float defaultValue)
        {
            var setting = new FloatSetting(this.data.FloatValues, key, defaultValue);
            this.settings.Add(setting);
            return setting;
        }

        protected IDateTimeSetting GetDateTimeSetting(int key, DateTime defaultValue)
        {
            var setting = new DateTimeSetting(this.data.DateTimeValues, key, defaultValue);
            this.settings.Add(setting);
            return setting;
        }

        /// <summary>
        /// Internal class for storing all our settings data.
        /// </summary>
        private class SettingsData
        {
            private Dictionary<int, bool> booleanValues = new Dictionary<int, bool>();
            private Dictionary<int, string> stringValues = new Dictionary<int, string>();
            private Dictionary<int, int> intValues = new Dictionary<int, int>();
            private Dictionary<int, float> floatValues = new Dictionary<int, float>();
            private Dictionary<int, DateTime> dateTimeValues = new Dictionary<int, DateTime>();

            public Dictionary<int, bool> BooleanValues
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => this.booleanValues;

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                set => this.booleanValues = value;
            }

            public Dictionary<int, string> StringValues
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => this.stringValues;

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                set => this.stringValues = value;
            }

            public Dictionary<int, int> IntValues
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => this.intValues;

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                set => this.intValues = value;
            }

            public Dictionary<int, float> FloatValues
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => this.floatValues;

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                set => this.floatValues = value;
            }

            public Dictionary<int, DateTime> DateTimeValues
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => this.dateTimeValues;

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                set => this.dateTimeValues = value;
            }
        }

        private abstract class Setting<T> : ISetting<T>, ISetting
            where T : System.IComparable<T>
        {
            private readonly Dictionary<int, T> settings;
            private readonly T defaultValue;
            private readonly int key;

            private T currentValue;
            private bool isDirty;

            public Setting(Dictionary<int, T> settings, int key, T defaultValue)
            {
                this.settings = settings;
                this.key = key;
                this.defaultValue = defaultValue;
                this.currentValue = this.GetValue(this.key, this.defaultValue);
                this.isDirty = false;
            }

            public T Value
            {
                get
                {
                    return this.currentValue;
                }

                set
                {
                    if (this.currentValue.CompareTo(value) != 0)
                    {
                        this.currentValue = value;
                        this.isDirty = true;
                    }
                }
            }

            public bool IsDirty
            {
                get { return this.isDirty; }
            }

            public void Revert()
            {
                this.currentValue = this.GetValue(this.key, this.defaultValue);
                this.isDirty = false;
            }

            public void Commit()
            {
                this.SetValue(this.key, this.currentValue);
                this.isDirty = false;
            }

            protected T GetValue(int key, T defaultValue)
            {
                if (this.settings.TryGetValue(key, out T foundValue))
                {
                    return foundValue;
                }
                else
                {
                    return defaultValue;
                }
            }

            protected void SetValue(int key, T value)
            {
                this.settings[key] = value;
            }
        }

        private class FloatSetting : Setting<float>, IFloatSetting
        {
            public FloatSetting(Dictionary<int, float> settings, int key, float defaultValue)
                : base(settings, key, defaultValue)
            {
            }
        }

        private class BoolSetting : Setting<bool>, IBoolSetting
        {
            public BoolSetting(Dictionary<int, bool> settings, int key, bool defaultValue)
                : base(settings, key, defaultValue)
            {
            }
        }

        private class IntSetting : Setting<int>, IIntSetting
        {
            public IntSetting(Dictionary<int, int> settings, int key, int defaultValue)
                : base(settings, key, defaultValue)
            {
            }
        }

        private class StringSetting : Setting<string>, IStringSetting
        {
            public StringSetting(Dictionary<int, string> settings, int key, string defaultValue)
                : base(settings, key, defaultValue)
            {
            }
        }

        private class DateTimeSetting : Setting<DateTime>, IDateTimeSetting
        {
            public DateTimeSetting(Dictionary<int, DateTime> settings, int key, DateTime defaultValue)
                : base(settings, key, defaultValue)
            {
            }
        }
    }
}
