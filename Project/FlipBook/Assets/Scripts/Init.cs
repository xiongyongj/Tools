using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace English.Readbook {
    public class Init : MonoBehaviour {
        private BoardBookPanel _panel;

        private void Awake() {
            _panel = transform.Find("BoardBookPanel").gameObject.AddComponent<BoardBookPanel>();
        }

        private void Start() {
            BookSystem.Initialize();
        }
    }
}

