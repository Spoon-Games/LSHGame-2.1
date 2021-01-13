using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourT
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ElementFieldConversion : Attribute
    {
        public int Order = 0;
        public ElementFieldConversion(int order = 0)
        {
            Order = order;
        }
    }

    public static class ElementFieldConverter
    {
        private readonly static Dictionary<InputType, MethodInfo> converters = new Dictionary<InputType, MethodInfo>();

        static ElementFieldConverter()
        {
            List<Type> AssTypes = new List<Type>();

            foreach (Assembly item in AppDomain.CurrentDomain.GetAssemblies())
            {
                AssTypes.AddRange(item.GetTypes());
            }

            var querry = from type in AssTypes
                         from method in type.GetMethods(BindingFlags.Static | BindingFlags.Public)
                         from attribute in method.GetCustomAttributes(typeof(ElementFieldConversion), false)
                         select new { method, attribute };

            foreach (var q in querry)
            {
                Type outputType = q.method.ReturnType;
                if (!outputType.Equals(typeof(VisualElement)) && !outputType.IsSubclassOf(typeof(VisualElement)))
                    continue;

                var types = q.method.GetParameters().Select(p => p.ParameterType).ToArray();
                if (types.Count() != 4)
                    continue;

                Type inputType = types[0];

                if (!types[1].Equals(typeof(string)))
                    continue;

                if (!types[2].Equals(typeof(Action<object>)))
                    continue;

                if (!types[3].Equals(typeof(Action<object>)))
                    continue;


                if (!(q.attribute is ElementFieldConversion attribute))
                    continue;

                converters[new InputType(inputType, attribute.Order)] = q.method;
            }
        }

        public static VisualElement GetVisualElement(Type type, object startValue, string label, Action<object> setValue,ref Action<object> getValue)
        {
            if (TryGetVisualElement(type,startValue, label, setValue,getValue, out VisualElement element))
                return element;
            return new Label("Conversion of " + type.Name + " is not implemented");
        }

        public static bool TryGetVisualElement(Type type,object startValue,string label,Action<object> setValue, Action<object> getValue, out VisualElement element)
        {
            if(setValue == null)
            {
                element = null;
                return false;
            }

            if (type.IsSubclassOf(typeof(Enum)))
            {
                element = LogicExtensions.GetElementOfEnumField((Enum)startValue, label, setValue, getValue);
                return true;
            }

            if(!converters.Any(p => p.Key.From.Equals(type)))
            {
                element = null;
                return false;
            }

            var c = converters.Where(p => p.Key.From.Equals(type))
                .Aggregate((p1, p2) => p1.Key.Order > p2.Key.Order ? p1 : p2);


            if (!Equals(c, null))
            {
                object[] pars = new object[] { startValue, label, setValue ,getValue};
                element = (VisualElement)c.Value.Invoke(null, pars);
                getValue += (Action<object>)pars[3];
                //getValue.Invoke("Test");
                //Debug.Log("GetValue: " + (getValue != null));
                return true;
            }

            element = null;
            return false;
        }

        private struct InputType
        {
            public Type From;
            public int Order;

            public InputType(Type from, int order)
            {
                From = from;
                Order = order;
            }

        }

    }

    public static class LogicExtensions
    {
        [ElementFieldConversion]
        public static VisualElement GetElementOfStringField(string startValue, string fieldName, Action<object> setValue,Action<object> getValue)
        {
            TextField textField = new TextField(fieldName);
            textField.SetValueWithoutNotify(startValue);
            textField.RegisterValueChangedCallback(s => setValue.Invoke(s.newValue)); 
            getValue += o => { textField.SetValueWithoutNotify((string)o); Debug.Log("SetValue: " + (string)o); } ;
            return textField;
        }

        [ElementFieldConversion]
        public static VisualElement GetElementOfIntField(int startValue, string fieldName, Action<object> setValue, Action<object> getValue)
        {
            IntegerField field = new IntegerField(fieldName);
            field.SetValueWithoutNotify(startValue);
            field.RegisterValueChangedCallback(s => setValue.Invoke(s.newValue));
            getValue += o => field.SetValueWithoutNotify((int)o);
            return field;
        }

        [ElementFieldConversion]
        public static VisualElement GetElementOfFloatField(float startValue, string fieldName, Action<object> setValue, Action<object> getValue)
        {
            FloatField field = new FloatField(fieldName);
            field.SetValueWithoutNotify(startValue);
            field.RegisterValueChangedCallback(s => setValue.Invoke(s.newValue));
            getValue += o => field.SetValueWithoutNotify((float)o);
            return field;
        }

        [ElementFieldConversion]
        public static VisualElement GetElementOfVector2Field(Vector2 startValue, string fieldName, Action<object> setValue, Action<object> getValue)
        {
            Vector2Field field = new Vector2Field(fieldName);
            field.SetValueWithoutNotify(startValue);
            field.RegisterValueChangedCallback(s => setValue.Invoke(s.newValue));
            getValue += o => field.SetValueWithoutNotify((Vector2)o);
            return field;
        }

        [ElementFieldConversion]
        public static VisualElement GetElementOfVector3Field(Vector3 startValue, string fieldName, Action<object> setValue, Action<object> getValue)
        {
            Vector3Field field = new Vector3Field(fieldName);
            field.SetValueWithoutNotify(startValue);
            field.RegisterValueChangedCallback(s => setValue.Invoke(s.newValue));
            getValue += o => field.SetValueWithoutNotify((Vector3)o);
            return field;
        }

        [ElementFieldConversion]
        public static VisualElement GetElementOfVector4Field(Vector4 startValue, string fieldName, Action<object> setValue, Action<object> getValue)
        {
            Vector4Field field = new Vector4Field(fieldName);
            field.SetValueWithoutNotify(startValue);
            field.RegisterValueChangedCallback(s => setValue.Invoke(s.newValue));
            getValue += o => field.SetValueWithoutNotify((Vector4)o);
            return field;
        }

        [ElementFieldConversion]
        public static VisualElement GetElementOfEnumField(Enum startValue, string fieldName, Action<object> setValue, Action<object> getValue)
        {
            EnumField field = new EnumField(fieldName, startValue);
            field.RegisterValueChangedCallback(s => setValue.Invoke(s.newValue));
            getValue += o => field.SetValueWithoutNotify((Enum)o);
            return field;
        }

        [ElementFieldConversion]
        public static VisualElement GetElementOfColorField(Color startValue, string fieldName, Action<object> setValue, Action<object> getValue)
        {
            ColorField field = new ColorField(fieldName);
            field.SetValueWithoutNotify(startValue);
            field.RegisterValueChangedCallback(s => setValue.Invoke(s.newValue));
            getValue += o => field.SetValueWithoutNotify((Color)o);
            return field;
        }

        [ElementFieldConversion]
        public static VisualElement GetElementOfBooleanField(bool startValue, string fieldName, Action<object> setValue, Action<object> getValue)
        {
            Toggle field = new Toggle(fieldName);
            field.SetValueWithoutNotify(startValue);
            field.RegisterValueChangedCallback(s => setValue.Invoke(s.newValue));
            getValue += o => field.SetValueWithoutNotify((bool)o);
            return field;
        }

        [ElementFieldConversion]
        public static VisualElement GetElementOfLayerMaskField(LayerMask startValue, string fieldName, Action<object> setValue, Action<object> getValue)
        {
            LayerMaskField field = new LayerMaskField(fieldName, startValue);
            field.RegisterValueChangedCallback(s => setValue.Invoke(new LayerMask() { value = s.newValue }));
            return field;
        }
    }
}
