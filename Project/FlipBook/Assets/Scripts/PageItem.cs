using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;


namespace English.Readbook {
    public struct PageItemProps {
        public string BookName;
        public Page Page;
        public string Keys;
        public string Frequently;
        public int PageIndex;
        public Define.PageSide Side;
    }

    public class PageItem : MonoBehaviour {
        private Image _background;

        private PageItemProps _props;
        private int _lineIndex;

        private void Awake() {
            _background = transform.Find("Background").GetComponent<Image>();

            Register();
        }

        private void OnDestroy() {
            Unregister();
        }

        private void Register() {
        }

        private void Unregister() {
        }

        public void Init(PageItemProps props) {
            _props = props;

            Refresh();
        }

        private void Refresh() {
            string name = _props.Page.Background[(int)_props.Side];
            string path = $"{_props.BookName}/Background/{name}";
            // Debug.Log($"path: {path}");
            _background.sprite = Resources.Load<Sprite>(path);

            PageContent pageContent = CreatePageContent();
            PageContentProps props = new();
            props.PageIndex = _props.PageIndex;
            props.Page = _props.Page;
            props.Keys = _props.Keys;
            props.Frequently = _props.Frequently;
            pageContent.Init(props);
        }

        private PageContent CreatePageContent() {
            GameObject go = Resources.Load<GameObject>($"Prefabs/PageContent");
            go = Instantiate(go, transform);
            go.name = $"Page_{_props.PageIndex}_{_props.Side}";
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

            PageContent pageContent = go.AddComponent<PageContent>();
            return pageContent;
        }
    }
}
