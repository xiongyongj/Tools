using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioPlayerPanel : MonoBehaviour {
    private AudioSource _audioSource;
    private Button _introduction;
    private GameObject _introductionDetail;
    private Text _introductionText;
    private Button _back;

    private RectTransform _songContent;
    private RectTransform _lyricsContent;

    private Toggle _loop;
    private Button _loopButton;
    private Text _loopLabel;

    private Button _previousButton;
    private Button _nextButton;

    private Button _playButton;
    private GameObject _pause;
    private GameObject _resume;

    private Toggle _lyrics;
    private Button _lyricsButton;

    private bool _showIntroduction;
    private List<ClientData.AudioData> _audioList = new();
    private int _selectedIndex;
    private AudioPlayer _audioPlayer;

    private void Awake() {
        _audioSource = transform.Find("Audio").GetComponent<AudioSource>();
        _introduction = transform.Find("Introduction").GetComponent<Button>();
        _introductionDetail = transform.Find("Introduction/Detail").gameObject;
        _introductionText = transform.Find("Introduction/Detail/Text").GetComponent<Text>();
        _back = transform.Find("Back").GetComponent<Button>();
        _songContent = transform.Find("ScrollView/Viewport/Content").GetComponent<RectTransform>();
        _lyricsContent = transform.Find("Body/ScrollView/Viewport/Content").GetComponent<RectTransform>();
        _loop = transform.Find("Body/Loop").GetComponent<Toggle>();
        _loopButton = transform.Find("Body/Loop/Button").GetComponent<Button>();
        _loopLabel = transform.Find("Body/Loop/Label").GetComponent<Text>();
        _previousButton = transform.Find("Body/Previous").GetComponent<Button>();
        _nextButton = transform.Find("Body/Next").GetComponent<Button>();
        _playButton = transform.Find("Body/Play").GetComponent<Button>();
        _pause = transform.Find("Body/Play/Pause").gameObject;
        _resume = transform.Find("Body/Play/Resume").gameObject;
        _lyrics = transform.Find("Body/Lyrics").GetComponent<Toggle>();
        _lyricsButton = transform.Find("Body/Lyrics/Button").GetComponent<Button>();

        _audioPlayer = AudioPlayer.Get(_audioSource);

        RegisterEvent();
    }

    private void Start() {
        Refresh();
    }

    private void RegisterEvent() {
        _introduction.onClick.AddListener(OnClickIntroduction);
    }

    public void SetData(List<ClientData.AudioData> list) {
        _audioList = list;
        _audioList.Sort((a, b) => a.Order - b.Order);

        RefreshSongs();
        RefreshBody();
    }

    #region 事件

    private void OnClickIntroduction() {
        _showIntroduction = !_showIntroduction;
        RefreshIntroduction();
    }

    private void OnClickAudioItem(int index) {
        if (_selectedIndex == index) {
            return;
        }

        _selectedIndex = index;
        RefreshSongs();
        RefreshBody();
    }

    #endregion

    #region 界面刷新

    private void Refresh() {
        RefreshIntroduction();
        RefreshSongs();
        RefreshBody();
    }

    private void RefreshIntroduction() {
        _introductionDetail.SetActive(_showIntroduction);

        _introductionText.text = "精选英文儿歌，让孩子在轻松有趣的旋律中，自然而然提升英语听力与语感。";
    }

    private void RefreshSongs() {
        int index = 0;
        for (; index < _audioList.Count; ++index) {
            ClientData.AudioData data = _audioList[index];

            AudioItem item;
            if (index < _songContent.childCount) {
                item = _songContent.GetChild(index).GetComponent<AudioItem>();
            }
            else {
                GameObject go = Resources.Load<GameObject>("Prefabs/AudioItem");
                go = Instantiate(go, _songContent);
                go.transform.localScale = Vector3.one;
                item = go.AddComponent<AudioItem>();
            }
            item.gameObject.SetActive(true);

            ClientData.AudioItemProps props = new();
            props.IsSelected = index == _selectedIndex;
            props.Data = data;
            props.OnClick = OnClickAudioItem;
            item.Init(props);
        }

        for (; index < _songContent.childCount; ++index) {
            _songContent.GetChild(index).gameObject.SetActive(false);
        }
    }

    #region Body

    private void RefreshBody() {
        RefreshLyrics();
    }

    private List<AudioPlayer.AudioLyric> GetLyrics() {
        ClientData.AudioData audioData = _selectedIndex < _audioList.Count ? _audioList[_selectedIndex] : default;

        TextAsset textAsset = Resources.Load<TextAsset>($"Lyrics/{audioData.Name}");
        List<AudioPlayer.AudioLyric> lyrics = new();
        if (textAsset != null) {
            string[] lines = textAsset.text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            for (int i = 0; i < lines.Length; ++i) {
                string line = lines[i];

                if (string.IsNullOrEmpty(line)) {
                    continue;
                }

                string[] strs = line.Split(new[] { "[", "]" }, StringSplitOptions.None);
                for (int j = 0; j < strs.Length; ++j) {
                    if (string.IsNullOrEmpty(strs[j])) {
                        continue;
                    }
                    Debug.Log("Lyrics =>" + strs[j]);
                    AudioPlayer.AudioLyric lyric = new();
                    lyric.StartTime = 0;
                    lyric.Lyric = "";
                    lyrics.Add(lyric);
                }
            }
        }

        return lyrics;
    }

    private void RefreshLyrics() {
        List<AudioPlayer.AudioLyric> lyrics = GetLyrics();
        int index = 0;
        for (; index < lyrics.Count; ++index) {
            AudioPlayer.AudioLyric lyric = lyrics[index];

        }
    }

    #endregion

    #endregion
}
