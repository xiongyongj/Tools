using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component {
    private static T _instance;

    public static T GetInstance() {
        if (_instance == null) {
            string name = typeof(T).Name;
            GameObject go = GameObject.Find(name);
            if (go == null) {
                go = new GameObject(name);
                DontDestroyOnLoad(go);
            }

            _instance = go.GetComponent<T>();
            if (_instance == null) {
                _instance = go.AddComponent<T>();
            }
        }

        return _instance;
    }
}
