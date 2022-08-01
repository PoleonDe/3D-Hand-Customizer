using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class SingletonObject<T> : SerializedMonoBehaviour where T : SingletonObject<T>
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<T>();

                if (instance == null)
                {
                    GameObject container = new GameObject(typeof(T).ToString());
                    instance = container.AddComponent<T>();
                    DontDestroyOnLoad(instance);
                }
            }

            return instance;
        }
    }
}
