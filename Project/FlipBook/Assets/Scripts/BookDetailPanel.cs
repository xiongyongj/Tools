

using UnityEngine;
using BookCurlPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using System;
using System.Collections;

namespace English.Readbook {
    public struct BookDetailProps {
        public Book Book;
    }

    public class BookDetailPanel : MonoBehaviour {
        private BookPro _bookPro;
        private Paper _first;
        private Paper _finally;

        private BookDetailProps _props;
        private bool _isAutoRead = true;
        private int _lineIndex;

        private void Awake() {
            _bookPro = transform.Find("BookPro").GetComponent<BookPro>();
            _first = _bookPro.papers[0];
            _finally = _bookPro.papers[_bookPro.papers.Count - 1];

            Register();
        }

        private void OnDestroy() {
            UnRegister();
        }

        private void Register() {
            BookSystem.EventCloseBookDetail += OnCloseBook;
            BookSystem.EventSwtichAutoRead += OnSwitchAutoRead;

            _bookPro.OnFlip.AddListener(OnFlip);
        }

        private void UnRegister() {
            BookSystem.EventCloseBookDetail -= OnCloseBook;
            BookSystem.EventSwtichAutoRead -= OnSwitchAutoRead;

            _bookPro.OnFlip.RemoveListener(OnFlip);
        }

        public void Init(BookDetailProps props) {
            _props = props;
            AddPages();

            OnFlip();
        }

        private void OnCloseBook() {
            Destroy(gameObject);
        }

        private void OnSwitchAutoRead(bool isOn) {
            _isAutoRead = isOn;
            _bookPro.Flip();
        }

        private void OnFlip() {
            // Debug.Log("OnFlip =>" + _bookPro.CurrentPaper);

            StopCoroutine(nameof(PlayAudioComplete));

            _lineIndex = 0;

            PlayAudio();
        }

        private void PlayAudio() {
            Page page = _props.Book.Pages[_bookPro.CurrentPaper - 1];
            if (_lineIndex >= page.Lines.Count) {
                return;
            }

            Line line = page.Lines[_lineIndex];
            float duration = BookSystem.PlayAudio(_props.Book.Name, line.Audio);
            BookSystem.OnPlayAudioStart(_bookPro.CurrentPaper, _lineIndex, duration);

            StartCoroutine(nameof(PlayAudioComplete), duration);
        }

        private IEnumerator PlayAudioComplete(float duration) {
            yield return new WaitForSeconds(duration);
            BookSystem.OnPlayAudioComplete(_bookPro.CurrentPaper, _lineIndex);

            ++_lineIndex;
            PlayAudio();
        }

        private void AddPages() {
            Paper paper = _bookPro.papers[0];
            for (int i = 0; i < _props.Book.Pages.Count; ++i) {
                int pageIndex = i + 1;

                for (int side = 0; side < 2; ++side) {
                    // Debug.Log($"创建页:i:{i}  pageIndex:{pageIndex}  side:{(Define.PageSide)side}");
                    PageItem page = CreatePage(pageIndex, side);

                    if (side == 0) {
                        paper.Back = page.gameObject;
                    }
                    else {
                        if (pageIndex >= _props.Book.Pages.Count) {
                            paper = _bookPro.papers[^1];
                        }
                        else {
                            paper = new();
                            _bookPro.papers.Insert(pageIndex, paper);
                        }
                        paper.Front = page.gameObject;
                    }

                    PageItemProps props = new();
                    props.BookName = _props.Book.Name;
                    props.Page = _props.Book.Pages[i];
                    props.Keys = _props.Book.Keys;
                    props.Frequently = _props.Book.Frequently;
                    props.PageIndex = pageIndex;
                    props.Side = (Define.PageSide)side;
                    page.Init(props);
                }
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
