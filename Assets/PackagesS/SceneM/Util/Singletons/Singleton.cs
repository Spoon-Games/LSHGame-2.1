using UnityEngine;

namespace SceneM
{
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static bool instanceWasDestroied = false;

        private static T _instance;
        public static T Instance { get {
                if(_instance == null)
                {
                    T[] objects = Resources.FindObjectsOfTypeAll<T>();
                    int count = 0;
                    foreach(var o in objects)
                    {
                        if (o.gameObject.scene.rootCount > 0)
                            count++;
                    }

                    if (count > 1)
                        Debug.Log("There are more than one singleton of type " + typeof(T).ToString() + " in the scene");

                    if (count == 0)
                        Debug.LogError("There is no singleton of type " + typeof(T).ToString() + " in the scene");
                    else
                        _instance = objects[0];
                    instanceWasDestroied = false;
                    
                    
                }
                return _instance;
            }}

        public virtual void Awake()
        {
            T[] objects = FindObjectsOfType<T>();
            if(objects.Length > 1 )//|| (_instance != null && !instanceWasDestroied))
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = (T)this;
                instanceWasDestroied = false;
            }
        }

        private void OnDestroy()
        {
            if(_instance == this)
            {
                instanceWasDestroied = true;
            }
        }
    }
}
