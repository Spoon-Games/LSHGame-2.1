using System;

namespace TagInterpreterR
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TagAttribute : Attribute
    {
        public string Name;
        public enum TagType { Area,Single,Both};
        public TagType Tag;

        public TagAttribute(string name = "",TagType tagType = TagType.Both)
        {
            this.Name = name;
            Tag = tagType;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class TagFieldAttribute : Attribute
    {
        public string Name;
        public bool IsDefault;

        public TagFieldAttribute(string name = "",bool isDefault = false)
        {
            Name = name;
            IsDefault = isDefault;
        }
    }

}