
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.Mathematics;
using System;

public class AudioVisualizer : MonoBehaviour {
    private List<Image> _bars; // 4根柱子对象
    private AudioSource _audioSource;
    private float _heightMultiplier = 500f;
    private float _sensitivity = 50f;
    private AnimationCurve[] _frequencyCurves;

    private float[] _spectrum = new float[512];
    private Image[] _barRenderers;

    public void Init(List<Image> gos, AudioSource audio) {
        _bars = gos;
        _audioSource = audio;

        _barRenderers = new Image[_bars.Count];
        _frequencyCurves = new AnimationCurve[_bars.Count];

        for (int i = 0; i < _bars.Count; i++) {
            _barRenderers[i] = _bars[i].GetComponent<Image>();
            _bars[i].transform.localScale = new Vector3(1, 0.1f, 1);

            _frequencyCurves[i] = new AnimationCurve();
        }

        StopCoroutine(nameof(UpdateBars));
        StartCoroutine(nameof(UpdateBars));
    }

    IEnumerator UpdateBars() {
        while(true) {
            _audioSource.GetSpectrumData(_spectrum, 0, FFTWindow.BlackmanHarris);

            // 计算4个频段能量值
            float[] bandEnergy = new float[4];
            for(int b=0; b<4; b++) {
                float sum = 0;
                int sampleCount = 128;
                for(int s = 0; s < sampleCount; ++s) {
                    sum += _spectrum[b * sampleCount + s];
                }
                bandEnergy[b] = Mathf.Log(sum * _sensitivity + 1) * _heightMultiplier;
            }

            // 应用动画曲线并更新柱子高度
            for(int i = 0; i < _bars.Count; ++i) {
                // float targetHeight = _frequencyCurves[i].Evaluate(bandEnergy[i]);
                float targetHeight = Math.Clamp(bandEnergy[i] / 128f, 0.1f, 1);
                Vector3 newScale = _bars[i].transform.localScale;
                newScale.y = Mathf.Lerp(newScale.y, targetHeight, Time.deltaTime * 10f);
                _bars[i].transform.localScale = newScale;
            }

            yield return new WaitForSeconds(0.033f); // 30fps更新
        }
    }
}
