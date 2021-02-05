using System;
using UnityEngine;

namespace UINavigation
{
    public static class Util
    {
        internal static bool IsBackKey(this string s)
        {
            return Equals(s, BackKey);
        }

        internal static string BackKey => "back";

        internal static void GetChildren<Component,Group>(this Transform parent,Action<Component> AddChild)
        {
            foreach (Transform child in parent)
            {
                if (child.TryGetComponent<Component>(out Component c))
                {
                    AddChild.Invoke(c);
                    
                }

                if (child.TryGetComponent<Group>(out Group g))
                {
                    GetChildren<Component,Group>(child,AddChild);
                }
            }
        }

        public static Action<string> SetInputController;

        public static bool IsNullEmptyOrEqual(this string s, string comparison)
        {
            return string.IsNullOrEmpty(s) || Equals(s, comparison);
        }
    }
}
