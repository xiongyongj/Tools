using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LyricsItem : MonoBehaviour {
    private Text _text;

    private ClientData.LyricItemProps _props;

    private void Awake() {
        _text = GetComponent<Text>();
    }

    public void Init(ClientData.LyricItemProps props) {
        _props = props;

        Refresh();
    }

    private void Refresh() {
        _text.text = _props.Lyric.lyric;

        _text.fontSize = _props.Index == 0 ? 51 : 35;
    }
}
