using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardBookPanel : MonoBehaviour {
    private RectTransform _content;
    private Button _close;
    private Button _introduction;
    private GameObject _context;

    private bool _showIntroduction;

    private void Awake() {
        _content = transform.Find("ScrollView/Viewport/Content").GetComponent<RectTransform>();
        _close = transform.Find("Close").GetComponent<Button>();
        _introduction = transform.Find("Introduction").GetComponent<Button>();
        _context = transform.Find("Introduction/Context").gameObject;

        Register();
    }

    private void Start() {
        Refresh();
    }

    private void Register() {
        _close.onClick.AddListener(OnClickClose);
        _introduction.onClick.AddListener(OnClickIntroduction);
    }

    private void OnClickClose() {

    }

    private void OnClickIntroduction() {
        _showIntroduction = !_showIntroduction;
        RefreshIntroduction();
    }

    private void Refresh() {
        RefreshIntroduction();
    }

    private void RefreshIntroduction() {
        _context.SetActive(_showIntroduction);
    }
}
