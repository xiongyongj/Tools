using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Init : MonoBehaviour {

    private Canvas _canvas;

    private void Awake() {
        _canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        
    }
}