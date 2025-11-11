

using UnityEngine;
using BookCurlPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using System;
using System.Collections;
using Unity.VisualScripting;

namespace English.Readbook {
    public struct BookDetailProps {
        public string BookName;
    }

    public class BookDetailPanel : MonoBehaviour {
        private BookPro _bookPro;
        private PageFlipper _pageFlipper;
        private AutoFlipBook _autoFlipBook;

        private BookDetailProps _props;
        private Book _book;
        private int _lineIndex;

        private void Awake() {
            _bookPro = transform.Find("BookPro").GetComponent<BookPro>();
            _pageFlipper = _bookPro.gameObject.AddComponent<PageFlipper>();
            _autoFlipBook = _bookPro.gameObject.AddComponent<AutoFlipBook>();

            _pageFlipper.book = _bookPro;
            _autoFlipBook.Book = _bookPro;

            Register();
        }

        private void OnDestroy() {
            UnRegister();
        }

        private void Register() {
            BookSystem.EventCloseBookDetail += OnCloseBook;

            _bookPro.OnFlip.AddListener(OnFlip);
        }

        private void UnRegister() {
            BookSystem.EventCloseBookDetail -= OnCloseBook;

            _bookPro.OnFlip.RemoveListener(OnFlip);
        }

        public void Init(BookDetailProps props) {
            BookSystem.IsAutoRead = true;
            _props = props;
            _book = BookSystem.GetBook(_props.BookName);

            AddPages();

            OnFlip();
        }

        private void OnCloseBook() {
            Destroy(gameObject);
        }

        private void OnFlip() {
            // Debug.Log("OnFlip =>" + _bookPro.CurrentPaper);
            StopCoroutine(nameof(PlayAudioComplete));
            BookSystem.OnFlipPage(_bookPro.CurrentPaper);

            _lineIndex = 0;

            BookSystem.OnPageReadStart(_bookPro.CurrentPaper);

            PlayAudio();
        }

        private void PlayAudio() {
            Page page = _book.Pages[_bookPro.CurrentPaper - 1];
            if (_lineIndex >= page.Lines.Count) {
                // Debug.Log("OnPageReadComplete =>" + _bookPro.CurrentPaper);
                BookSystem.OnPageReadComplete(_bookPro.CurrentPaper);
                return;
            }

            Line line = page.Lines[_lineIndex];
            float duration = BookSystem.PlayAudio(_book.Name, line.Audio);
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
            for (int i = 0; i < _book.Pages.Count; ++i) {
                int pageIndex = i + 1;

                for (int side = 0; side < 2; ++side) {
                    // Debug.Log($"创建页:i:{i}  pageIndex:{pageIndex}  side:{(Define.PageSide)side}");
                    PageItem page = CreatePage(pageIndex, side);

                    if (side == 0) {
                        paper.Back = page.gameObject;
                    }
                    else {
                        if (pageIndex >= _book.Pages.Count) {
                            paper = _bookPro.papers[^1];
                        }
                        else {
                            paper = new();
                            _bookPro.papers.Insert(pageIndex, paper);
                        }
                        paper.Front = page.gameObject;
                    }

                    PageItemProps props = new();
                    props.BookName = _book.Name;
                    props.Page = _book.Pages[i];
                    props.Keys = _book.Keys;
                    props.Frequently = _book.Frequently;
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
