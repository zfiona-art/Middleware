using UnityEngine;

public abstract class SingletonMono<T> : MonoBehaviour where T : SingletonMono<T>
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                var go = FindObjectOfType<T>().gameObject;
                if (go == null)
                {
                    go = new GameObject(typeof(T).Name);
                    DontDestroyOnLoad(go);
                }
                instance = go.TryGetComponent<T>();
            }

            return instance;
        }
    }

    public void OnApplicationQuit()
    {
        instance = null;
    }

    public virtual void Init()
    {
    }
}
