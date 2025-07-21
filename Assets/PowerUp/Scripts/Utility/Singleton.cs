using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static object _lock = new object();
    private static bool _applicationIsQuitting = false;

    public static T Instance
    {
        get
        {
            if (_applicationIsQuitting)
            {
                Debug.LogWarning($"[Singleton<{typeof(T).Name}>] La aplicación se está cerrando, no se crearán más instancias.");
                return null;
            }

            if (_instance != null)
                return _instance;

            lock (_lock)
            {
                if (_instance == null)
                {
                    // 1) Buscar una instancia ya existente en escena
                    _instance = Object.FindAnyObjectByType<T>();

                    // 2) Comprobar que no haya más de una
#pragma warning disable CS0618    // silencia el "FindObjectsOfType<T>(bool) está obsoleto"
                    T[] all = Object.FindObjectsOfType<T>(false); // false = sólo instancias activas
#pragma warning restore CS0618

                    if (all.Length > 1)
                    {
                        Debug.LogError($"[Singleton<{typeof(T).Name}>] ¡{all.Length} instancias encontradas! Debe haber sólo una.");
                        return _instance;
                    }

                    // 3) Si no había ninguna, crearla
                    if (_instance == null)
                    {
                        var go = new GameObject($"{typeof(T).Name}_Singleton");
                        _instance = go.AddComponent<T>();
                        DontDestroyOnLoad(go);
                    }
                }
            }

            return _instance;
        }
    }

    private void OnApplicationQuit()
    {
        _applicationIsQuitting = true;
    }
}

