using System.Collections.Generic;
using UnityEngine;

namespace TagInterpreterR
{
    public sealed class TagReference : BaseTag
    {
        public Dictionary<string, string> Attributes = new Dictionary<string, string>();

        public override string ToString()
        {
            string s = "Tag: " + Name ;
            if (Attributes.Count > 0)
            {
                foreach (var p in Attributes)
                {
                    s += "\n  "+p.Key + ": " + p.Value;
                }
            }
            return s;
        }

        public bool IsAttribute(string attribute,string value)
        {
            return Attributes.TryGetValue(attribute, out string v) && Equals(value, v);
        }
    }
}
