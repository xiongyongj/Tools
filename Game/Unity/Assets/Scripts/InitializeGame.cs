using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class InitializeGame : MonoBehaviour {
    
    void Start() {
        // Editor环境下，HotUpdate.dll.bytes已经被自动加载，不需要加载，重复加载反而会出问题。
#if !UNITY_EDITOR
        Assembly hotUpdateAss = Assembly.Load(File.ReadAllBytes($"{Application.streamingAssetsPath}/HotUpdate.dll.bytes"));
#else
        // Editor下无需加载，直接查找获得HotUpdate程序集
        Assembly hotUpdateAss = AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "HotUpdate");
#endif
    
        Type type = hotUpdateAss.GetType("App");
        type.GetMethod("Start")?.Invoke(null, null);
    }

}