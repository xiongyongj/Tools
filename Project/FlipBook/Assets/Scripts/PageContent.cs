using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace English.Readbook {
    public struct PageContentProps {
        public string BookName;
        public int PageIndex;
        public Page Page;
        public string Keys;
        public string Frequently;
    }

    public class PageContent : MonoBehaviour {
        private Button _close;
        // Title
        private GameObject _title;
        private Image _icon;
        private Text _titleText;
        private Text _keys;
        private Text _frequently;

        // Content
        private GameObject _content;
        private Button _auto;
        private RectTransform _autoHandler;
        private Text _autoText1;
        private Text _autoText2;
        private Button _autoButton;
        private RectTransform _context;
        private Text _templeteText;

        private PageContentProps _props;
        private List<Text> _items = new();
        private int _lineIndex;
        private List<string> _words;
        private int _wordIndex;
        private float _duration;
        private float _wordDuration;
        private float _elapsedTime;
        private bool _startPlay;
        private Tweener _handlerTweener;
        private Tweener _contextTweener;

        private void Awake() {
            _close = transform.Find("Close").GetComponent<Button>();

            // Title
            _title = transform.Find("Title").gameObject;
            _icon = transform.Find("Title/Icon").GetComponent<Image>();
            _titleText = transform.Find("Title/Title").GetComponent<Text>();
            _keys = transform.Find("Title/Keys/BG/Text").GetComponent<Text>();
            _frequently = transform.Find("Title/Frequently/BG/Text").GetComponent<Text>();

            // Content
            _content = transform.Find("Content").gameObject;
            _auto = transform.Find("Content/Auto").GetComponent<Button>();
            _autoHandler = transform.Find("Content/Auto/Handler").GetComponent<RectTransform>();
            _autoText1 = transform.Find("Content/Auto/Text1").GetComponent<Text>();
            _autoText2 = transform.Find("Content/Auto/Text2").GetComponent<Text>();
            _autoButton = transform.Find("Content/Auto/Button").GetComponent<Button>();
            _context = transform.Find("Content/Words/Mask/Context").GetComponent<RectTransform>();
            _templeteText = transform.Find("Content/Words/Mask/Text").GetComponent<Text>();

            _templeteText.gameObject.SetActive(false);

            Register();
        }

        private void OnDestroy() {
            Unregister();
        }

        private void Update() {
            if (!_startPlay) {
                return;
            }

            if (_words == null || _words.Count == 0) {
                return;
            }

            if (_wordIndex >= _words.Count) {
                return;
            }

            _elapsedTime += Time.deltaTime;
            if (_elapsedTime >= _wordDuration) {
                if (++_wordIndex >= _words.Count) {
                    _wordIndex = _words.Count - 1;
                }
                RefreshText();
                _elapsedTime -= _wordDuration;
            }
        }

        private void Register() {
            _close.onClick.AddListener(OnClickClose);
            _autoButton.onClick.AddListener(OnClickAuto);

            BookSystem.EventOnFlipPage += OnFlipPage;
            BookSystem.EventPlayAudioStart += OnPlayAudioStart;
            BookSystem.EventPlayAudioComplete += OnPlayAudioComplete;
            BookSystem.EventSwtichAutoRead += OnSwitchAutoRead;
        }

        private void Unregister() {
            BookSystem.EventOnFlipPage -= OnFlipPage;
            BookSystem.EventPlayAudioStart -= OnPlayAudioStart;
            BookSystem.EventPlayAudioComplete -= OnPlayAudioComplete;
            BookSystem.EventSwtichAutoRead -= OnSwitchAutoRead;
        }

        public void Init(PageContentProps props) {
            _props = props;

            Refresh();
        }

        private void OnClickClose() {
            BookSystem.OnCloseBookDetail();
        }

        private void OnClickAuto() {
            BookSystem.IsAutoRead = !BookSystem.IsAutoRead;
            BookSystem.OnSwitchAutoRead();
        }

        private void OnFlipPage(int pageIndex) {
            if (_props.PageIndex != pageIndex) {
                return;
            }

            MoveContext(0);
        }

        private void OnPlayAudioStart(int pageIndex, int lineIndex, float duration) {
            if (_props.PageIndex != pageIndex) {
                return;
            }

            if (pageIndex == 1) {
                return;
            }

            _lineIndex = lineIndex;
            _words = StringUtils.WordsSmart(_props.Page.Lines[lineIndex].Text);
            _wordIndex = 0;
            _elapsedTime = 0;
            _duration = duration;
            _wordDuration = _duration / _words.Count;
            _startPlay = true;
            RefreshText();
        }

        private void OnPlayAudioComplete(int pageIndex, int lineIndex) {
            if (_props.PageIndex != pageIndex) {
                return;
            }

            _startPlay = false;
            _wordIndex = 0;
            _words?.Clear();

            List<Line> lines = _props.Page.Lines;
            if (lineIndex < lines.Count - 1) {
                MoveContext(lineIndex + 1);
            }
        }

        private void OnSwitchAutoRead() {
            RefreshAuto();
        }

        private void MoveContext(int lineIndex) {
            float width = _templeteText.rectTransform.rect.width;
            _contextTweener?.Kill();

            float x = -width * lineIndex;
            _contextTweener = DOTween.To(val => _context.anchoredPosition = new(val, 0), _context.anchoredPosition.x, x, 0.2f);
        }

        private void Refresh() {
            _title.SetActive(_props.PageIndex == 1);
            _content.SetActive(_props.PageIndex != 1);

            if (_props.PageIndex == 1) {
                RefreshTitle();
            }
            else {
                RefreshContent();
            }

            RefreshAuto();
        }

        private void RefreshTitle() {
            _titleText.text = _props.Page.Lines[0].Text;
            _keys.text = _props.Keys;
            _frequently.text = _props.Frequently;
        }

        private void RefreshContent() {
            for (int i = 0; i < _props.Page.Lines.Count; ++i) {
                Line line = _props.Page.Lines[i];
                Text text = Instantiate(_templeteText.gameObject, _context).GetComponent<Text>();
                text.gameObject.SetActive(true);
                text.text = line.Text;

                _items.Add(text);
            }
        }

        private void RefreshAuto() {
            _autoText1.gameObject.SetActive(!BookSystem.IsAutoRead);
            _autoText2.gameObject.SetActive(BookSystem.IsAutoRead);

            MoveHandler();
        }

        private void MoveHandler() {
            _handlerTweener?.Kill();
            _handlerTweener = _autoHandler.DOLocalMoveX(BookSystem.IsAutoRead ? 57 : -57, 0.2f);
        }

        private void RefreshText() {
            string str = "<color=#B678F8>";
            for (int i = 0; i < _words.Count; ++i) {
                string word = _words[i];
                str += $" {word}";

                if (i == _wordIndex) {
                    str += "</color>";
                }
            }
            _items[_lineIndex].text = str;
        }
    }
}

