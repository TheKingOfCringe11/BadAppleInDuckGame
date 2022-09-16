using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame.BadApple
{
    internal static class Extensions
    {
        public static FieldInfo GetFieldInfo(this Type type, string fieldName)
        {
            return type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Static |  BindingFlags.Public | BindingFlags.NonPublic);
        }

        public static T GetValue<T>(this FieldInfo fieldInfo, object instance)
        {
            return (T)fieldInfo.GetValue(instance);
        }
    }
}
