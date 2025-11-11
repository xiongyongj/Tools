

using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using UnityEngine.Events;

namespace English.Readbook {
    public struct BookCover {
        public int ID;
        public string Name;
        public string Icon;
        public bool IsUnlocked;
        public bool IsFree;
    }

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
        public static bool IsAutoRead = true;
        public static bool IsPageFlipping = false;
        public static bool IsReading = false;

        private static Dictionary<string, List<BookCover>> _map = new();

        public static UnityAction EventCloseBookDetail;
        public static UnityAction<int> EventOnFlipPage;
        public static UnityAction EventSwtichAutoRead;
        public static UnityAction<int, int, float> EventPlayAudioStart;
        public static UnityAction<int, int> EventPlayAudioComplete;
        public static UnityAction<int> EventPageReadStart;
        public static UnityAction<int> EventPageReadComplete;

        public static void OnCloseBookDetail() {
            EventCloseBookDetail?.Invoke();
        }

        public static void OnFlipPage(int pageIndex) {
            EventOnFlipPage?.Invoke(pageIndex);
        }

        public static void OnSwitchAutoRead() {
            EventSwtichAutoRead?.Invoke();
        }

        public static void OnPlayAudioStart(int pageIndex, int lineIndex, float duration) {
            EventPlayAudioStart?.Invoke(pageIndex, lineIndex, duration);
        }

        public static void OnPlayAudioComplete(int pageIndex, int lineIndex) {
            EventPlayAudioComplete?.Invoke(pageIndex, lineIndex);
        }

        public static void OnPageReadStart(int pageIndex) {
            IsReading = true;
            EventPageReadStart?.Invoke(pageIndex);
        }

        public static void OnPageReadComplete(int pageIndex) {
            IsReading = false;
            EventPageReadComplete?.Invoke(pageIndex);
        }

        public static void Initialize() {
            Canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
            AudioSource = GameObject.Find("Audio").GetComponent<AudioSource>();

            TextAsset text = Resources.Load<TextAsset>("Config/Config");
            JsonData data = JsonMapper.ToObject(text.text);

            ICollection keys = (data as IDictionary).Keys;
            foreach (string key in keys) {
                JsonData data1 = data[key];

                List<BookCover> covers = new();
                for (int i = 0; i < data1.Count; ++i) {
                    JsonData data2 = data1[i];

                    BookCover bookCover = new();
                    bookCover.ID = (int)data2["ID"];
                    bookCover.Name = (string)data2["Name"];
                    bookCover.Icon = (string)data2["Icon"];
                    bookCover.IsUnlocked = i == 0;
                    bookCover.IsFree = i == 0;

                    covers.Add(bookCover);
                }
                _map.TryAdd(key, covers);
            }
        }

        public static List<BookCover> GetCovers(string key) {
            if (_map.TryGetValue(key, out List<BookCover> covers)) {
                return covers;
            }
            return new();
        }

        public static Book GetBook(string bookName) {
            TextAsset text = Resources.Load<TextAsset>($"{bookName}/Config/Config");
            JsonData data = JsonMapper.ToObject(text.text);

            Book book = new();
            book.ID = (int)data["ID"];
            book.Name = (string)data["Name"];
            book.Icon = (string)data["Icon"];
            book.Keys = (string)data["Keys"];
            book.Frequently = (string)data["Frequently"];

            book.Pages = new();
            JsonData data1 = data["Pages"];
            for (int j = 0; j < data1.Count; ++j) {
                JsonData data4 = data1[j];
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

            return book;
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
