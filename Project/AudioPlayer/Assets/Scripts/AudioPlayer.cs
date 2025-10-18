
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



public class AudioPlayer: MonoBehaviour {
    public class AudioLyric {
        public int Index;
        public double begin;
        public double end;
        public string lyric;
    }

    public class AudioDetailInfo {
        public int Index;
        public string Name;
        public string Path;
        public AudioClip Clip;
        public List<AudioLyric> Lyrics;
    }

    public enum SwitchOperate {
        Previous,
        Next,
        RePlay
    }

    private AudioSource _audioSource;
    public AudioSource AudioSource => _audioSource;

    public bool PlayOnAwake {
        get {
            return _audioSource.playOnAwake;
        }
        set {
            _audioSource.playOnAwake = value;
        }
    }

    public bool Loop {
        get {
            return _audioSource.loop;
        }
        set {
            _audioSource.loop = value;
        }
    }

    public float Progress {
        get {
            if (_audioSource.clip == null) {
                return 0;
            }
            return _audioSource.time / _audioSource.clip.length;
        }
    }

    private List<AudioDetailInfo> _audioList = new();
    private bool _isPlaying;
    public bool IsPlaying => _isPlaying;
    private int _audioIndex;
    public int AudioIndex => _audioIndex;
    private int _lyricIndex;
    public int LyricIndex => _lyricIndex;
    public bool ShowLyrics = true;

    private UnityAction<AudioDetailInfo> _onSwitchAudioEvent;
    private UnityAction<AudioLyric> _onSwitchLyricEvent;
    private UnityAction<bool> _onStateChangedEvent;

    public static AudioPlayer Get(AudioSource source) {
        AudioPlayer audioPlayer = source.gameObject.AddComponent<AudioPlayer>();
        audioPlayer._audioSource = source;

        return audioPlayer;
    }

    public void SetSwitchAudioFunc(UnityAction<AudioDetailInfo> action) {
        _onSwitchAudioEvent = action;
    }

    public void SetSwitchLyricFunc(UnityAction<AudioLyric> action) {
        _onSwitchLyricEvent = action;
    }

    public void SetPlayStateFunc(UnityAction<bool> action) {
        _onStateChangedEvent = action;
    }

    private void Update() {
        if (_audioList.Count <= 0) {
            return;
        }

        AudioDetailInfo info = _audioList[_audioIndex];
        if (info.Clip == null) {
            return;
        }

        if (!_isPlaying) {
            return;
        }

        CheckLyric();

        if (_audioSource.time >= _audioSource.clip.length) {
            SwitchAudio(Loop ? SwitchOperate.RePlay : SwitchOperate.Next);
        }
    }

    private void CheckLyric() {
        List<AudioLyric> lyrics = _audioList[_audioIndex].Lyrics;
        int len = ShowLyrics ? lyrics.Count : Math.Min(lyrics.Count, 1);
        for (int i = 0; i < len; ++i) {
            AudioLyric lyric = lyrics[i];

            if (ShowLyrics) {
                if (_audioSource.time < lyric.begin || _audioSource.time > lyric.end) {
                    continue;
                }
            }
            else {
                if (lyric.Index != 0) {
                    continue;
                }
            }

            if (_lyricIndex == lyric.Index) {
                continue;
            }
            _lyricIndex = lyric.Index;

            SwitchLyric();
        }
    }

    public void UpdateData(List<AudioDetailInfo> infos) {
        _audioList.Clear();
        _audioList.AddRange(infos);
    }

    public void SwitchAudio(SwitchOperate operate) {
        Stop();

        if (operate == SwitchOperate.Previous) {
            if (--_audioIndex < 0) {
                _audioIndex = _audioList.Count - 1;
            }
        }
        else if (operate == SwitchOperate.Next) {
            if (++_audioIndex >= _audioList.Count) {
                _audioIndex = 0;
            }
        }

        Play(_audioIndex);

        AudioDetailInfo info = _audioList[_audioIndex];
        Debug.Log("切换音乐 =>" + _audioIndex + " | " + info.Name);
        _onSwitchAudioEvent?.Invoke(info);
    }

    private void SwitchLyric() {
        List<AudioLyric> lyrics = _audioList[_audioIndex].Lyrics;
        if (_lyricIndex >= lyrics.Count) {
            return;
        }
        AudioLyric lyric = lyrics[_lyricIndex];
        Debug.Log("切换歌词 =>" + _lyricIndex + " | " + lyric.lyric);
        _onSwitchLyricEvent?.Invoke(lyric);
    }

    public void Play(int index) {
        if (index < 0 || index >= _audioList.Count) {
            return;
        }
        _audioIndex = index;

        AudioDetailInfo info = _audioList[index];
        if (info.Clip == null) {
            info.Clip = Resources.Load<AudioClip>(info.Path);
        }

        _audioSource.clip = info.Clip;
        _audioSource.Play(0);
        _isPlaying = true;
        _lyricIndex = 0;

        SwitchLyric();

        _onStateChangedEvent?.Invoke(_isPlaying);
    }

    public void Stop() {
        _isPlaying = false;
        _audioSource.Stop();
    }

    public void Pause() {
        _isPlaying = false;
        _audioSource.Pause();

        _onStateChangedEvent?.Invoke(_isPlaying);
    }

    public void Resume() {
        _isPlaying = true;
        _audioSource.UnPause();

        _onStateChangedEvent?.Invoke(_isPlaying);
    }
}
