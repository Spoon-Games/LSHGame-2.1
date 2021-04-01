using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TagInterpreterR
{
    internal static class TagTypeLoader
    {
        #region LoadTagTypes
        public static Dictionary<string, TagTypeInfo> LoadTagTypes()
        {
            List<Type> AssTypes = new List<Type>();

            foreach (Assembly item in AppDomain.CurrentDomain.GetAssemblies())
            {
                AssTypes.AddRange(item.GetTypes());
            }

            var querry = from type in AssTypes
                         where type.GetCustomAttribute<TagAttribute>() != null && !type.IsAbstract && type.IsSubclassOf(typeof(BaseTag))
                         select new { type, attribute = type.GetCustomAttribute<TagAttribute>() };

            var tagTypes = new Dictionary<string, TagTypeInfo>();

            foreach (var q in querry)
            {
                string s = q.attribute.Name;
                if (string.IsNullOrEmpty(s))
                    s = q.type.Name;

                if (tagTypes.ContainsKey(s))
                    throw new InterpreterException("Tag with name: " + s + " already exists");

                tagTypes.Add(s, new TagTypeInfo(q.type, q.attribute, GetTagFields(q.type)));
            }

            return tagTypes;
        }

        private static Dictionary<string, TagFieldInfo> GetTagFields(Type type)
        {
            Dictionary<string, TagFieldInfo> tagFields = new Dictionary<string, TagFieldInfo>();

            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var fieldInfo in fieldInfos)
            {
                var attribute = fieldInfo.GetCustomAttribute<TagFieldAttribute>();
                if (attribute != null)
                {
                    string s = fieldInfo.Name;
                    if (!string.IsNullOrEmpty(attribute.Name))
                        s = attribute.Name;

                    if (tagFields.ContainsKey(s))
                        throw new InterpreterException("TagField: " + s + " already exists");

                    tagFields.Add(s, new TagFieldInfo(fieldInfo, attribute));
                }
            }

            return tagFields;
        }

        
        #endregion
    }

    internal class TagTypeInfo
    {
        public Type Type;
        public TagAttribute Attribute;
        public Dictionary<string, TagFieldInfo> Fields;

        public TagTypeInfo(Type type, TagAttribute attribute, Dictionary<string, TagFieldInfo> fields)
        {
            Type = type;
            Attribute = attribute;
            Fields = fields;
        }
    }

    internal class TagFieldInfo
    {
        public FieldInfo FieldInfo;
        public TagFieldAttribute Attribute;

        public TagFieldInfo(FieldInfo fieldInfo, TagFieldAttribute attribute)
        {
            FieldInfo = fieldInfo;
            Attribute = attribute;
        }
    }
}
