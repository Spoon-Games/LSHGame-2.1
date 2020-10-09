using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneM
{
    public static class SceneUtil
    {
        public static List<T> FindAllOfTypeInScene<T>(Scene scene)
        {
            List<T> result = new List<T>();
            Transform[] roots = scene.GetRootGameObjects().Select(g => g.transform).ToArray();
            foreach (Transform child in roots)
            {
                AddChildrenTransform<T>(result, child);
            }
            return result;
        }

        private static void AddChildrenTransform<T>(List<T> result, Transform parent)
        {
            Component[] components = parent.GetComponents<Component>();
            foreach (var c in components)
                if (c is T r)
                    result.Add(r);
            foreach (Transform child in parent)
                AddChildrenTransform<T>(result, child);
        }

        public static IEnumerable<T> Distinct<T, U>(
        this IEnumerable<T> seq, Func<T, U> getKey)
        {
            return
                from item in seq
                group item by getKey(item) into gp
                select gp.First();
        }
    }
}
