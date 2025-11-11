using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace English.Readbook {
    public class RowItemProps {
        public List<BookCover> Covers = new();
    }

    public class RowItem : MonoBehaviour {
        private RectTransform _content;

        private RowItemProps _props;

        private void Awake() {
            _content = transform.Find("Content").GetComponent<RectTransform>();
        }

        public void Init(RowItemProps props) {
            _props = props;

            Refresh();
        }

        private void Refresh() {
            RefreshContent();
        }

        private void RefreshContent() {
            int index = 0;
            for (; index < _props.Covers.Count; ++index) {
                BookCover cover = _props.Covers[index];

                BookItem item;
                if (index < _content.childCount) {
                    item = _content.GetChild(index).GetComponent<BookItem>();
                }
                else {
                    GameObject go = Resources.Load<GameObject>("Prefabs/BookItem");
                    item = Instantiate(go, _content).AddComponent<BookItem>();
                    item.transform.localPosition = Vector3.zero;
                    item.transform.localScale = Vector3.one;
                }
                item.gameObject.SetActive(true);

                BookItemProps itemProps = new();
                itemProps.Cover = cover;
                item.Init(itemProps);
            }

            for (; index < _content.childCount; ++index) {
                _content.GetChild(index).gameObject.SetActive(false);
            }
        }
    }
}
