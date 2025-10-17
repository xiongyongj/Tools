using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AudioPlayer {
    public struct AudioLyric {
        public long StartTime;
        public string Lyric;
    }

    public struct AudioDetailInfo {
        public int Index;
        public string Name;
        public AudioClip Clip;
        public List<AudioLyric> Lyrics;
    }

    private AudioSource _audioSource;

    // public bool PlayOnAwake {
    //     get { return _audioSource.playOnAwake; }
    //     set { _audioSource.playOnAwake = value; }
    // }

    public static AudioPlayer Get(AudioSource source) {
        AudioPlayer audioPlayer = new();
        audioPlayer._audioSource = source;

        return audioPlayer;
    }

    public void UpdateData(AudioDetailInfo info) {

    }

    public void Play(AudioDetailInfo info) {

    }
}
