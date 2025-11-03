using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace English.Readbook {
    public struct PageItemProps {
        public string BookName;
        public Page Page;
        public int PageIndex;
        public Define.PageSide Side;
    }

    public class PageItem : MonoBehaviour {
        private Image _background;

        private PageItemProps _props;

        private void Awake() {
            _background = transform.Find("Background").GetComponent<Image>();

            Register();
        }

        private void Register() {

        }

        public void Init(PageItemProps props) {
            _props = props;

            Refresh();
        }

        private void Refresh() {
            string path = $"{_props.BookName}/Background/{_props.Page.Background}";
            // Debug.Log($"path: {path}");
            _background.sprite = Resources.Load<Sprite>(path);

            CreatePageContent();
        }

        private GameObject CreatePageContent() {
            GameObject go = Resources.Load<GameObject>($"{_props.BookName}/Pages/{_props.Page.Content}");
            if (go == null) {
                return null;
            }
            go = Instantiate(go, transform);
            go.name = $"Page_{_props.PageIndex - 1}_{_props.Side}";
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.pivot = Vector2.one * 0.5f;
            rt.anchorMin = Vector2.one * 0.5f;
            rt.anchorMax = Vector2.one * 0.5f;
            rt.sizeDelta = new(2450, 1500);

            if (_props.Side == Define.PageSide.Left) {
                rt.anchoredPosition = new(612.5f, 0);
            }
            else {
                rt.anchoredPosition = new(-612.5f, 0);
            }
            return go;
        }
    }
}
