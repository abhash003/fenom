using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace FenomPlus.Helpers
{
    internal class DebugHelper
    {
        internal static string GetCallingMethodBaseNameFileLine(int frameOffset = 0)
        {
            StackFrame frame = new StackFrame(frameOffset + 1);
            var method = frame.GetMethod();
            var type = method.DeclaringType;
            var name = $"{method.ReflectedType.FullName}{method.Name}";

            return $"{name}";
        }
        public static string GetCallingMethodString(int frameOffset = 0)
        {
            var methodNameFileLine = GetCallingMethodBaseNameFileLine(frameOffset);
            return $"{methodNameFileLine}";
        }
    }
}
