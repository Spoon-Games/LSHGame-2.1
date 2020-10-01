using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace LogicC
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class LogicConversion : Attribute
    {
        public int Order = 0;
        public LogicConversion(int order = 0)
        {
            this.Order = order;
        }
    }

    public static class LogicConverter
    {
        private readonly static Dictionary<InOutType, MethodInfo> converters = new Dictionary<InOutType, MethodInfo>();

        static LogicConverter()
        {
            List<Type> AssTypes = new List<Type>();

            foreach (Assembly item in AppDomain.CurrentDomain.GetAssemblies())
            {
                AssTypes.AddRange(item.GetTypes());
            }

            var querry = from type in AssTypes
                         from method in type.GetMethods(BindingFlags.Static | BindingFlags.Public)
                         from attribute in method.GetCustomAttributes(typeof(LogicConversion), false)
                         select new { method, attribute };

            foreach (var q in querry)
            {
                Type outputType = q.method.ReturnType;
                if (outputType.Equals(typeof(void)))
                    continue;

                var types = q.method.GetParameters().Select(p => p.ParameterType);
                if (types.Count() != 1)
                    continue;
                Type inputType = types.First();

                if (!(q.attribute is LogicConversion attribute))
                    continue;
                converters[new InOutType(inputType, outputType, attribute.Order)] = q.method;
            }
        }

        public static bool IsCastable(Type from, Type to)
        {
            if (from == to)
                return true;
            if (converters.Any(k => k.Key.IsCastPossible(from, to)))
                return true;
            return IsCastableTo(from,to);
        }

        public static bool TryCast<T>(object o, out T result)
        {
            if(o == null)
            {
                result = default;
                return true;
            }
            if (o.GetType() == typeof(T))
            {
                result = (T)o;
                return true;
            }
            var c = converters.Where(p => p.Key.IsCastPossible(o.GetType(), typeof(T)))
                .Aggregate((p1, p2) => p1.Key.Order > p2.Key.Order ? p1 : p2);


            if (!Equals(c, null))
            {
                result = (T)c.Value.Invoke(null, new object[] { o });
                return true;
            }
            if (o is T r)
            {
                result = r;
                return true;
            }
            result = default;
            return false;
        }

        private struct InOutType
        {
            public Type From;
            public Type To;
            public int Order;

            public InOutType(Type from, Type to, int order)
            {
                From = from;
                To = to;
                Order = order;
            }

            public bool IsCastPossible(Type from, Type to)
            {
                return (from == From || from.IsSubclassOf(From)) && (to == To || To.IsSubclassOf(to));
            }
        }

        public static bool IsCastableTo(Type from, Type to)
        {
            if (to.IsAssignableFrom(from))
                return true;

            if ((from.IsPrimitive || from.IsEnum) && (to.IsPrimitive || to.IsEnum))
            {
                return from == to || (from != typeof(bool) && to != typeof(bool));
            }
            return IsCastDefined(to, m => m.GetParameters()[0].ParameterType, _ => from, false)
                || IsCastDefined(from, _ => to, m => m.ReturnType, true);
        }

        //little irrelevant DRY method
        static bool IsCastDefined(Type type, Func<MethodInfo, Type> baseType, Func<MethodInfo, Type> derivedType,
                                  bool lookInBase)
        {
            var bindinFlags = BindingFlags.Public
                            | BindingFlags.Static
                            | (lookInBase ? BindingFlags.FlattenHierarchy : BindingFlags.DeclaredOnly);
            return type.GetMethods(bindinFlags).Any(m => (m.Name == "op_Implicit" || m.Name == "op_Explicit")
                                                      && baseType(m).IsAssignableFrom(derivedType(m)));
        }
    }

    public static class LogicExtensions{
        [LogicConversion]
        public static float Vector2ToFloat(Vector2 o) => o.x;

        [LogicConversion]
        public static float Vector3ToFloat(Vector3 o) => o.x;

        [LogicConversion]
        public static float Vector4ToFloat(Vector4 o) => o.x;

        [LogicConversion]
        public static Vector2 FloatToVector2(float o) => new Vector2(o, 0);

        [LogicConversion]
        public static Vector3 FloatToVector3(float o) => new Vector3(o, 0,0);

        [LogicConversion]
        public static Vector4 FloatToVector4(float o) => new Vector4(o, 0,0,0);
    }
}
