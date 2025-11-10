

using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using UnityEngine.Events;

namespace English.Readbook {
    public struct Book {
        public int ID;
        public string Name;
        public string Icon;
        public bool IsUnlocked;
        public bool IsFree;
        public List<Page> Pages;
        public string Keys;
        public string Frequently;
    }

    public struct Page {
        public List<string> Background;
        public List<Line> Lines;
    }

    public struct Line {
        public string Text;
        public string Audio;
    }

    public class BookSystem {
        public static Canvas Canvas;
        public static AudioSource AudioSource;

        private static Dictionary<string, List<Book>> _map = new();

        public static UnityAction EventCloseBookDetail;
        public static UnityAction<bool> EventSwtichAutoRead;
        public static UnityAction<int, int, float> EventPlayAudioStart;
        public static UnityAction<int, int> EventPlayAudioComplete;

        public static void OnCloseBookDetail() {
            EventCloseBookDetail?.Invoke();
        }

        public static void OnSwitchAutoRead(bool isOn) {
            EventSwtichAutoRead?.Invoke(isOn);
        }

        public static void OnPlayAudioStart(int pageIndex, int lineIndex, float duration) {
            EventPlayAudioStart?.Invoke(pageIndex, lineIndex, duration);
        }

        public static void OnPlayAudioComplete(int pageIndex, int lineIndex) {
            EventPlayAudioComplete?.Invoke(pageIndex, lineIndex);
        }

        public static void Initialize() {
            Canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
            AudioSource = GameObject.Find("Audio").GetComponent<AudioSource>();

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
                    book.Keys = (string)data2["Keys"];
                    book.Frequently = (string)data2["Frequently"];
                    book.IsUnlocked = i == 0;
                    book.IsFree = i == 0;

                    book.Pages = new();
                    JsonData data3 = data2["Pages"];
                    for (int j = 0; j < data3.Count; ++j) {
                        JsonData data4 = data3[j];
                        Page page = new();
                        page.Background = new();

                        JsonData data5 = data4["Background"];
                        if (data5.IsArray) {
                            for (int m = 0; m < data5.Count; ++m) {
                                page.Background.Add((string)data5[m]);
                            }
                        }

                        page.Lines = new();
                        data5 = data4["Lines"];
                        if (data5.IsArray) {
                            for (int m = 0; m < data5.Count; ++m) {
                                JsonData data6 = data5[m];

                                Line line = new();
                                line.Text = (string)data6["Text"];
                                line.Audio = (string)data6["Audio"];
                                page.Lines.Add(line);
                            }
                        }
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

        public static float PlayAudio(string bookName, string audioName) {
            AudioClip clip = Resources.Load<AudioClip>($"{bookName}/Audios/{audioName}");
            if (clip == null) {
                Debug.LogError($"AudioClip not found: {bookName}/{audioName}");
                return 0;
            }
            AudioSource.clip = clip;
            AudioSource.Play();
            return clip.length;
        }
    }
}
