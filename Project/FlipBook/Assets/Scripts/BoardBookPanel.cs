using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace English.Readbook {
    public class BoardBookPanel : MonoBehaviour {
        private ScrollRect _scroll;
        private RectTransform _content;
        private Button _close;
        private Button _introduction;
        private GameObject _context;
        private ToggleGroup _toggleGroup;
        private List<Toggle> _toggles = new();

        private bool _showIntroduction;
        private int _selectedLevelIndex = 0;

        private void Awake() {
            _scroll = transform.Find("ScrollView").GetComponent<ScrollRect>();
            _content = transform.Find("ScrollView/Viewport/Content").GetComponent<RectTransform>();
            _close = transform.Find("Close").GetComponent<Button>();
            _introduction = transform.Find("Introduction").GetComponent<Button>();
            _context = transform.Find("Introduction/Context").gameObject;
            _toggleGroup = transform.Find("Group").GetComponent<ToggleGroup>();

            for (int i = 0; i < _toggleGroup.transform.childCount; ++i) {
                int index = i;
                Toggle toggle = _toggleGroup.transform.GetChild(index).GetComponent<Toggle>();
                _toggles.Add(toggle);

                Button button = toggle.transform.Find("Button").GetComponent<Button>();
                button.onClick.AddListener(() => OnClickToggle(index));
            }

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

        private void OnClickToggle(int index) {
            if (_selectedLevelIndex == index) {
                return;
            }
            _selectedLevelIndex = index;

            RefreshToggles();
            RefreshItems();
        }

        private void Refresh() {
            RefreshIntroduction();
            RefreshItems();
        }

        private void RefreshIntroduction() {
            _context.SetActive(_showIntroduction);
        }

        private void RefreshToggles() {
            _toggles[_selectedLevelIndex].isOn = true;
        }

        // 刷新列表，每行 4 个
        private void RefreshItems() {
            _scroll.verticalNormalizedPosition = 1;
            List<Book> books = BookSystem.GetBooks($"Level{_selectedLevelIndex + 1}");

            var (index, rowIndex) = (0, 0);
            List<Book> rowBooks = new(4);
            for (; index < books.Count; ++index) {
                Book book = books[index];

                RowItem item;
                if (rowIndex < _content.childCount) {
                    item = _content.GetChild(rowIndex).GetComponent<RowItem>();
                }
                else {
                    GameObject go = Resources.Load<GameObject>("Prefabs/RowItem");
                    item = Instantiate(go, _content).AddComponent<RowItem>();
                    item.transform.localPosition = Vector3.zero;
                    item.transform.localScale = Vector3.one;
                }
                item.gameObject.SetActive(true);

                rowBooks.Add(book);
                RowItemProps props = new();
                props.Books.AddRange(rowBooks);
                item.Init(props);
            }
        }
    }
}
