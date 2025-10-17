using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ClientData {

    public struct AudioData {
        public string ID;
        public string Name;
        public int Order;
    }

    public struct AudioItemProps {
        public bool IsSelected;
        public AudioData Data;
        public UnityAction<int> OnClick;
    }
}
