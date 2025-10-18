using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Init : MonoBehaviour {

    private Canvas _canvas;

    private void Awake() {
        _canvas = GameObject.Find("Canvas").GetComponent<Canvas>();

        GameObject go = Resources.Load<GameObject>("Prefabs/AudioPlayerPanel");
        go = Instantiate(go, _canvas.transform);
        AudioPlayerPanel panel = go.AddComponent<AudioPlayerPanel>();


        List<ClientData.AudioData> list = new();
        list.Add(new() {
            ID = "G01",
            Name = "After A While, Crocodile",
            Order = 1
        });
        list.Add(new() {
            ID = "G02",
            Name = "Bath Time Song",
            Order = 2
        });
        panel.SetData(list);
    }
}
