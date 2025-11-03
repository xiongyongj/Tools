

using UnityEngine;
using BookCurlPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using System;

namespace English.Readbook {
    public struct BookDetailProps {
        public string BookName;
        public List<Page> Pages;
    }

    public class BookDetailPanel : MonoBehaviour {
        private BookPro _bookPro;
        private Paper _first;
        private Paper _finally;

        private BookDetailProps _props;

        private void Awake() {
            _bookPro = transform.Find("BookPro").GetComponent<BookPro>();
            _first = _bookPro.papers[0];
            _finally = _bookPro.papers[_bookPro.papers.Count - 1];
        }

        public void Init(BookDetailProps props) {
            _props = props;
            AddPages();
        }

        private void AddPages() {
            int side = 0;
            Paper paper = _bookPro.papers[0];
            for (int i = 0; i < _props.Pages.Count; ++i) {
                int pageIndex = Mathf.CeilToInt((i + 1) / 2f);
                side %= 2;
                // Debug.Log($"创建页:i:{i}  pageIndex:{pageIndex}  index:{index}");
                PageItem page = CreatePage(pageIndex, side);

                if (side == 0) {
                    paper.Back = page.gameObject;
                }
                else {
                    if (pageIndex >= _props.Pages.Count / 2f) {
                        paper = _bookPro.papers[^1];
                    }
                    else {
                        paper = new();
                        _bookPro.papers.Insert(pageIndex, paper);
                    }
                    paper.Front = page.gameObject;
                }

                PageItemProps props = new();
                props.BookName = _props.BookName;
                props.Page = _props.Pages[i];
                props.PageIndex = pageIndex;
                props.Side = (Define.PageSide)side;
                page.Init(props);

                ++side;
            }

            _bookPro.StartFlippingPaper = 1;
            _bookPro.EndFlippingPaper = _bookPro.papers.Count - 2;
        }

        private PageItem CreatePage(int pageIndex, int index) {
            GameObject go = Resources.Load<GameObject>($"Prefabs/PageItem");
            go = Instantiate(go, _bookPro.transform);
            go.name = $"Page_{pageIndex}_{index}";
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.pivot = Vector2.zero;
            rt.anchorMin = new(index == 0 ? 0 : 0.5f, 0);
            rt.anchorMax = new(index == 0 ? 0.5f : 1, 1);
            rt.sizeDelta = Vector2.zero;
            PageItem page = go.AddComponent<PageItem>();
            return page;
        }
    }
}
