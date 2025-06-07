using UnityEngine;

namespace PBMA.Core
{
    public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_isExpired)
                {
                    return null;
                }

                if (_instance)
                {
                    return _instance;
                }
                
                var go = new GameObject(typeof(T).Name);
                _instance = go.AddComponent<T>();

                return _instance;
            }
        }

        private static bool _isExpired;

        private void OnDestroy()
        {
            _isExpired = true;
            _instance = null;
        }
    }
}