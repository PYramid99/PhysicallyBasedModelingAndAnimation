using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBMA.Core
{
    public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance = null;

        public static T Instance
        {
            get
            {
                if (isExpired)
                {
                    return null;
                }

                if (instance == null)
                {
                    instance = FindObjectOfType<T>();
                    if (instance == null)
                    {
                        var go = new GameObject(typeof(T).Name);
                        instance = go.AddComponent<T>();
                    }
                }

                return instance;
            }
        }

        private static bool isExpired = false;

        private void OnDestroy()
        {
            isExpired = true;
        }
    }
}