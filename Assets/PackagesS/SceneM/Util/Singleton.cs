using UnityEngine;

namespace SceneM
{
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {

        public static T Instance { get; protected set; }

        public virtual void Awake()
        {
            T[] objects = FindObjectsOfType<T>();
            if(objects.Length > 1)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = (T)this;
            }
        }
    }

    public class ScriptableSingleton<T> : ScriptableObject where T : ScriptableSingleton<T>
    {
        private static T instance;
        public static T Instance {
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
                }
                return instance;
            }
        }
    }
}
