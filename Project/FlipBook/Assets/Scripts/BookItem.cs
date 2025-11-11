using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace English.Readbook {
    public struct BookItemProps {
        public BookCover Cover;
    }
    public class BookItem : MonoBehaviour {
        private Image _icon;
        private Image _tomorrow;
        private GameObject _free;
        private GameObject _lock;
        private Button _button;

        private BookItemProps _props;

        private void Awake() {
            _icon = transform.Find("Icon").GetComponent<Image>();
            _tomorrow = transform.Find("Tomorrow").GetComponent<Image>();
            _free = transform.Find("Free").gameObject;
            _lock = transform.Find("Lock").gameObject;
            _button = transform.Find("Button").GetComponent<Button>();

            Register();
        }

        private void Register() {
            _button.onClick.AddListener(OnClick);
        }

        public void Init(BookItemProps props) {
            _props = props;

            Refresh();
        }

        private void OnClick() {
            if (!_props.Cover.IsUnlocked) {
                // 未解锁
                return;
            }
            // 已解锁
            // BookSystem.OpenBook(_props.Book.Title);
            Debug.Log($"打开书籍：{_props.Cover.Name}");

            GameObject go = Resources.Load<GameObject>($"Prefabs/BookDetailPanel");
            go = Instantiate(go, BookSystem.Canvas.transform);
            BookDetailPanel panel = go.AddComponent<BookDetailPanel>();

            BookDetailProps props = new();
            props.BookName = _props.Cover.Name;
            panel.Init(props);
        }

        private void Refresh() {
            _icon.gameObject.SetActive(_props.Cover.IsUnlocked);
            _tomorrow.gameObject.SetActive(!_props.Cover.IsUnlocked);
            _lock.SetActive(!_props.Cover.IsUnlocked);
            _free.SetActive(_props.Cover.IsFree);
            _icon.sprite = Resources.Load<Sprite>($"Icon/{_props.Cover.Icon}");
        }
    }
}
