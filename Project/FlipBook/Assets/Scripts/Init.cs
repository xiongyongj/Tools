using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Init : MonoBehaviour {
    private BoardBookPanel _panel;

    private void Awake() {
        _panel = transform.Find("BoardBookPanel").gameObject.AddComponent<BoardBookPanel>();
    }
}
