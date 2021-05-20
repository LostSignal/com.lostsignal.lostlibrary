//-----------------------------------------------------------------------
// <copyright file="UnityEngine.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#pragma warning disable

#if !UNITY

namespace UnityEngine
{
    using System;

    public interface ISerializationCallbackReceiver
    {
        void OnBeforeSerialize();
        void OnAfterDeserialize();
    }

    public class SerializeFieldAttribute : System.Attribute
    {
    }

    public class PropertyAttribute : Attribute
    {
        protected PropertyAttribute()
        {
        }

        //
        // Summary:
        //     Optional field to specify the order that multiple DecorationDrawers should be
        //     drawn in.
        public int order { get; set; }
    }

    public class HeaderAttribute : System.Attribute
    {
        public HeaderAttribute(string name)
        {
        }
    }

    public class SpaceAttribute : System.Attribute
    {
        public SpaceAttribute(float space)
        {
        }
    }

    public class ContextMenuAttribute : System.Attribute
    {
        public ContextMenuAttribute(string menuName)
        {
        }
    }

    public static class Debug
    {
        public static Action<string> OnLog;
        public static Action<string> OnLogWarning;
        public static Action<string> OnLogError;
        public static Action<string> OnLogAssert;
        public static Action<Exception> OnLogException;

        public static void Assert(bool condition, string message)
        {
            if (condition == false)
            {
                OnLogAssert?.Invoke(message);
            }
        }

        public static void AssertFormat(bool condition, string format, params object[] args)
        {
            if (condition == false)
            {
                OnLogAssert?.Invoke(string.Format(format, args));
            }
        }

        public static void LogError(string message)
        {
            OnLogError?.Invoke(message);
        }

        public static void LogErrorFormat(string format, params object[] args)
        {
            OnLogError?.Invoke(string.Format(format, args));
        }

        public static void LogWarning(string message)
        {
            OnLogWarning?.Invoke(message);
        }

        public static void LogWarningFormat(string format, params object[] args)
        {
            OnLogWarning?.Invoke(string.Format(format, args));
        }

        public static void Log(string message)
        {
            OnLog?.Invoke(message);
        }

        public static void LogFormat(string format, params object[] args)
        {
            OnLog?.Invoke(string.Format(format, args));
        }

        public static void LogException(Exception ex)
        {
            OnLogException?.Invoke(ex);
        }
    }

    public struct Vector2
    {
        public float x;
        public float y;

        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public struct Vector3
    {
        public static readonly Vector3 zero = new Vector3(0.0f, 0.0f, 0.0f);

        public float x;
        public float y;
        public float z;

        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    public struct Vector4
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public Vector4(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }
    }

    public struct Color
    {
        public float r;
        public float g;
        public float b;
        public float a;

        public Color(float r, float g, float b, float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }
    }

    public struct Color32
    {
        public byte r;
        public byte g;
        public byte b;
        public byte a;

        public Color32(byte r, byte g, byte b, byte a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }
    }

    public struct Quaternion
    {
        public static readonly Quaternion identity = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);

        public float x;
        public float y;
        public float z;
        public float w;

        public Quaternion(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }
    }

    public struct Rect
    {
        public float xMin;
        public float yMin;
        public float width;
        public float height;

        public Rect(float xMin, float yMin, float width, float height)
        {
            this.xMin = xMin;
            this.yMin = yMin;
            this.width = width;
            this.height = height;
        }
    }

    public struct Plane
    {
        public Vector3 normal;
        public float distance;

        public Plane(Vector3 normal, float distance)
        {
            this.normal = normal;
            this.distance = distance;
        }
    }

    public struct Ray
    {
        public Vector3 origin;
        public Vector3 direction;

        public Ray(Vector3 origin, Vector3 direction)
        {
            this.origin = origin;
            this.direction = direction;
        }
    }

    public class HelpURLAttribute : System.Attribute
    {
        public HelpURLAttribute(string url)
        {
        }
    }

    public static class Mathf
    {
        public static float Clamp(float value, float min, float max)
        {
            return value<min? min :
                         value> max ? max :
                value;
        }

        public static float Sqrt(float value)
        {
            return System.MathF.Sqrt(value);
        }

        public static float Max(float value1, float value2)
        {
            return value1 > value2 ? value1 : value2;
        }
    }
}

#endif
