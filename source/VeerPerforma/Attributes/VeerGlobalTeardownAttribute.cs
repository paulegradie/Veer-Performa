using System;

namespace VeerPerforma.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class VeerGlobalTeardownAttribute : Attribute
    {
    }
}