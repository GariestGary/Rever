using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T: MonoBehaviour
{
    protected static bool destroyOnLoad = false;
    private static T instance;
    private static object lockObject = new object();

    public static T Instance { get 
        {
            lock (lockObject)
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();

                    if (instance == null)
                    {
                        var singleton = new GameObject("[SINGLETON] " + typeof(T));
                        instance = singleton.AddComponent<T>();
                    }

                    if (!destroyOnLoad)
                    {
                        DontDestroyOnLoad(instance.gameObject);
                    }
                }
                return instance;
            }
        } 
    }

}
