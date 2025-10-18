using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using LitJson;
using UnityEngine;
using UnityEngine.UI;

public class AudioPlayerPanel : MonoBehaviour {
    private AudioSource _audioSource;
    private Button _introduction;
    private GameObject _introductionDetail;
    private Text _introductionText;
    private Button _back;

    private RectTransform _songContent;
    private ScrollRect _lyricsScroll;
    private RectTransform _lyricsContent;
    private VerticalLayoutGroup _lyricsLayout;

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
    private AudioVisualizer _audioVisualizer;

    private void Awake() {
        _audioSource = transform.Find("Audio").GetComponent<AudioSource>();
        _introduction = transform.Find("Introduction").GetComponent<Button>();
        _introductionDetail = transform.Find("Introduction/Detail").gameObject;
        _introductionText = transform.Find("Introduction/Detail/Text").GetComponent<Text>();
        _back = transform.Find("Back").GetComponent<Button>();
        _songContent = transform.Find("ScrollView/Viewport/Content").GetComponent<RectTransform>();
        _lyricsScroll = transform.Find("Body/ScrollView").GetComponent<ScrollRect>();
        _lyricsContent = transform.Find("Body/ScrollView/Viewport/Content").GetComponent<RectTransform>();
        _lyricsLayout = _lyricsContent.GetComponent<VerticalLayoutGroup>();
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
        _audioPlayer.SetSwitchAudioFunc(OnSwitchAudio);
        _audioPlayer.SetSwitchLyricFunc(OnSwitchLyric);
        _audioPlayer.SetPlayStateFunc(OnStateChanged);

        _audioVisualizer = _audioPlayer.gameObject.AddComponent<AudioVisualizer>();

        RegisterEvent();
    }

    private void Start() {
        Refresh();
    }

    private void RegisterEvent() {
        _introduction.onClick.AddListener(OnClickIntroduction);
        _loopButton.onClick.AddListener(OnClickLoop);
        _lyricsButton.onClick.AddListener(OnClickLyrics);
        _previousButton.onClick.AddListener(OnClickPrevious);
        _nextButton.onClick.AddListener(OnClickNext);
        _playButton.onClick.AddListener(OnClickPlay);
    }

    public void SetData(List<ClientData.AudioData> list) {
        _audioList = list;
        _audioList.Sort((a, b) => a.Order - b.Order);

        UpdateAudiosToPlayer();

        RefreshSongs();
        RefreshBody();

        StartCoroutine(Play(0));
    }

    private void UpdateAudiosToPlayer() {
        List<AudioPlayer.AudioDetailInfo> infos = new();
        for (int i = 0; i < _audioList.Count; ++i) {
            ClientData.AudioData data = _audioList[i];
            AudioPlayer.AudioDetailInfo info = new();
            info.Index = i;
            info.Name = data.Name;
            info.Path = $"Audios/{data.ID}";
            info.Lyrics = GetLyrics(data.ID);
            infos.Add(info);
        }
        _audioPlayer.UpdateData(infos);
    }

    private IEnumerator Play(int index) {
        yield return new WaitForEndOfFrame();
        _audioPlayer.Play(index);
    }

    #region 事件

    private void OnClickIntroduction() {
        _showIntroduction = !_showIntroduction;
        RefreshIntroduction();
    }

    private void OnClickLoop() {
        _audioPlayer.Loop = !_audioPlayer.Loop;
        RefreshLyricsButtons();
    }

    private void OnClickLyrics() {
        _audioPlayer.ShowLyrics = !_audioPlayer.ShowLyrics;
        RefreshLyrics();
        StartCoroutine(nameof(RefreshLyricsPosition));
        RefreshLyricsButtons();
    }

    private void OnClickPrevious() {
        _audioPlayer.SwitchAudio(AudioPlayer.SwitchOperate.Previous);
    }

    private void OnClickNext() {
        _audioPlayer.SwitchAudio(AudioPlayer.SwitchOperate.Next);
    }

    private void OnClickPlay() {
        if (_audioPlayer.IsPlaying) {
            _audioPlayer.Pause();
        }
        else {
            _audioPlayer.Resume();
        }
    }

    private void OnClickAudioItem(int index) {
        if (_selectedIndex == index) {
            return;
        }

        _selectedIndex = index;
        RefreshSongs();
        RefreshBody();

        StartCoroutine(Play(_selectedIndex));
    }

    private void OnSwitchAudio(AudioPlayer.AudioDetailInfo info) {
        _selectedIndex = info.Index;
        RefreshSongs();
        RefreshLyrics();
    }

    private void OnSwitchLyric(AudioPlayer.AudioLyric lyric) {
        StartCoroutine(nameof(RefreshLyricsPosition));
    }

    private void OnStateChanged(bool isPlaying) {
        RefreshLyricsButtons();
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

            if (props.IsSelected) {
                _audioVisualizer.Init(item.GetBars(), _audioSource);
            }
        }

        for (; index < _songContent.childCount; ++index) {
            _songContent.GetChild(index).gameObject.SetActive(false);
        }
    }

    #region Body

    private void RefreshBody() {
        RefreshLyrics();
        RefreshLyricsButtons();
    }

    private List<AudioPlayer.AudioLyric> GetLyrics(string id) {
        List<AudioPlayer.AudioLyric> lyrics = new();

        TextAsset asset = Resources.Load<TextAsset>($"Lyrics/{id}");
        if (asset != null) {
            List<AudioPlayer.AudioLyric> data = JsonMapper.ToObject<List<AudioPlayer.AudioLyric>>(asset.text);
            for (int i = 0; i < data.Count; ++i) {
                data[i].Index = i;
                lyrics.Add(data[i]);
            }
        }

        return lyrics;
    }

    private void RefreshLyrics() {
        _lyricsScroll.verticalNormalizedPosition = 1;
        ClientData.AudioData audioData = _selectedIndex < _audioList.Count ? _audioList[_selectedIndex] : default;
        List<AudioPlayer.AudioLyric> lyrics = GetLyrics(audioData.ID);
        int len = _audioPlayer.ShowLyrics ? lyrics.Count : Math.Min(lyrics.Count, 1);
        int index = 0;
        for (; index < len; ++index) {
            AudioPlayer.AudioLyric lyric = lyrics[index];

            LyricsItem item;
            if (index < _lyricsContent.childCount) {
                item = _lyricsContent.GetChild(index).GetComponent<LyricsItem>();
            }
            else {
                GameObject go = Resources.Load<GameObject>("Prefabs/LyricItem");
                go = Instantiate(go, _lyricsContent);
                go.transform.localScale = Vector3.one;
                item = go.AddComponent<LyricsItem>();
            }
            item.gameObject.SetActive(true);
            item.name = index == 0 ? "Title" : "Lyric";

            ClientData.LyricItemProps props = new();
            props.Index = index;
            props.Lyric = lyric;
            item.Init(props);
        }

        for (; index < _lyricsContent.childCount; ++index) {
            _lyricsContent.GetChild(index).gameObject.SetActive(false);
        }
    }

    private IEnumerator RefreshLyricsPosition() {
        yield return new WaitForEndOfFrame();
        int index = _audioPlayer.LyricIndex;
        RectTransform node = null;
        for (int i = 0; i < _lyricsContent.childCount; ++i) {
            RectTransform child = _lyricsContent.GetChild(i).GetComponent<RectTransform>();
            Text text = child.GetComponent<Text>();

            if (i != index) {
                text.color = new Color32(255, 255, 255, 150);
                text.fontSize = i == 0 ? 51 : 35;
                continue;
            }
            text.color = new Color32(255, 255, 255, 255);
            text.fontSize += 5;

            node = child;

        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(_lyricsContent);

        if (node != null) {
            float y = Mathf.Abs(node.anchoredPosition.y) - _lyricsLayout.padding.top - (node.rect.height / 2);
            _lyricsContent.DOKill();
            _lyricsContent.DOLocalMoveY(y, 0.2f);
        }
    }

    private void RefreshLyricsButtons() {
        bool isLoop = _audioPlayer.Loop;
        _loop.isOn = isLoop;
        _loopLabel.color = isLoop ? new Color32(17, 144, 243, 255) : Color.white;

        bool isShow = _audioPlayer.ShowLyrics;
        _lyrics.isOn = isShow;

        _pause.SetActive(!_audioPlayer.IsPlaying);
        _resume.SetActive(_audioPlayer.IsPlaying);
    }

    #endregion

    #endregion
}
