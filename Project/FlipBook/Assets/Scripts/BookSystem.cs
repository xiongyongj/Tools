

using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

namespace English.Readbook {
    public struct Book {
        public int ID;
        public string Name;
        public string Icon;
        public bool IsUnlocked;
        public bool IsFree;
        public List<Page> Pages;
    }

    public struct Page {
        public string Background;
        public string Content;
    }

    public class BookSystem {
        public static Canvas Canvas;

        private static Dictionary<string, List<Book>> _map = new();

        public static void Initialize() {
            Canvas = GameObject.Find("Canvas").GetComponent<Canvas>();

            TextAsset text = Resources.Load<TextAsset>("Config/Config");
            JsonData data = JsonMapper.ToObject(text.text);

            ICollection keys = (data as IDictionary).Keys;
            foreach (string key in keys) {
                JsonData data1 = data[key];

                List<Book> books = new();
                for (int i = 0; i < data1.Count; ++i) {
                    JsonData data2 = data1[i];

                    Book book = new();
                    book.ID = (int)data2["ID"];
                    book.Name = (string)data2["Name"];
                    book.Icon = (string)data2["Icon"];
                    book.IsUnlocked = i == 0;
                    book.IsFree = i == 0;

                    book.Pages = new();
                    JsonData data3 = data2["Pages"];
                    for (int j = 0; j < data3.Count; ++j) {
                        JsonData data4 = data3[j];
                        Page page = new();
                        page.Background = (string)data4["Background"];
                        page.Content = (string)data4["Content"];
                        book.Pages.Add(page);
                    }

                    books.Add(book);
                }
                _map.TryAdd(key, books);
            }
        }

        public static List<Book> GetBooks(string key) {
            if (_map.TryGetValue(key, out List<Book> books)) {
                return books;
            }
            return new();
        }
    }
}
