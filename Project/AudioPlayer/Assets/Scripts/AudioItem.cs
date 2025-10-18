using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public class AudioItem : MonoBehaviour {
    private RectTransform _rt;
    private HorizontalLayoutGroup _horizontalLayout;
    private GameObject _selected;
    private Text _order;
    private RectTransform _animation;
    private RectTransform _mask;
    private Text _name;
    private Button _button;

    private ClientData.AudioItemProps _props;
    private bool _isScroll;
    private float _speed = 70;

    private void Awake() {
        _rt = transform.GetComponent<RectTransform>();
        _horizontalLayout = transform.GetComponent<HorizontalLayoutGroup>();
        _selected = transform.Find("Selected").gameObject;
        _order = transform.Find("Order").GetComponent<Text>();
        _animation = transform.Find("Animation").GetComponent<RectTransform>();
        _mask = transform.Find("Mask").GetComponent<RectTransform>();
        _name = transform.Find("Mask/Name").GetComponent<Text>();
        _button = transform.Find("Button").GetComponent<Button>();

        _button.onClick.AddListener(OnClick);
    }

    private void Update() {
        if (!_isScroll || !_props.IsSelected) {
            return;
        }

        _name.rectTransform.anchoredPosition -= new Vector2(_speed * Time.deltaTime, 0);
        if (_name.rectTransform.anchoredPosition.x < -_name.rectTransform.rect.width) {
            _name.rectTransform.anchoredPosition = new(_mask.rect.width, 0);
        }
    }

    public void Init(ClientData.AudioItemProps props) {
        _props = props;

        Refresh();
    }

    public List<Image> GetBars() {
        Image[] gos = _animation.GetComponentsInChildren<Image>();
        List<Image> bars = new();
        bars.AddRange(gos);
        return bars;
    }

    private void OnClick() {
        _props.OnClick?.Invoke(_props.Data.Order - 1);
    }

    private void Refresh() {
        _order.text = $"{_props.Data.Order}.";
        _name.text = $"{_props.Data.Name}";

        _selected.SetActive(_props.IsSelected);
        _order.gameObject.SetActive(!_props.IsSelected);
        _animation.gameObject.SetActive(_props.IsSelected);

        _name.color = _props.IsSelected ? new Color32(93, 254, 233, 255) : Color.white;
        _name.rectTransform.anchoredPosition = Vector2.zero;

        StartCoroutine(nameof(CalculateLayout));
    }

    private IEnumerator CalculateLayout() {
        yield return new WaitForEndOfFrame();

        // 左边距
        float x = _horizontalLayout.padding.left;

        if (_props.IsSelected) {
            x += _animation.rect.width;
        }
        else {
            x += _order.rectTransform.rect.width;
        }

        // 间距
        x += _horizontalLayout.spacing;

        _mask.anchoredPosition = new(x, 0);

        // 右边距
        x += 7;

        float width = _rt.rect.width - x;
        _mask.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);

        _isScroll = _name.rectTransform.rect.width > width;
    }
}
