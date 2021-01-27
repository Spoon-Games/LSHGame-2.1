using UnityEngine;

namespace SceneM
{
    public class ScriptableSingleton : ScriptableObject
    {

    }

    public class ScriptableSingleton<T> : ScriptableSingleton where T : ScriptableSingleton<T>
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    T[] objects = Resources.FindObjectsOfTypeAll<T>();
                    if (objects.Length == 0)
                        throw new System.Exception("Could not find singleton");
                    if (objects.Length != 1)
                        throw new System.Exception("There does exists no instance of the singleton or there are more than one.");
                    instance = objects[0];
                    instance.Awake();
                }
                return instance;
            }
        }

        protected virtual void Awake() { }
    }
}
