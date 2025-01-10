using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace ParentHouse.Utils {
    public class FpsCounter : MonoBehaviour {
        public TextMeshProUGUI Text;
        private int _averageCounter;
        private const int _averageFromAmount = 30;
        private const int _cacheNumbersAmount = 300;
        private int _currentAveraged;
        private int _maxAchieved;
        private int _maxTotalAchieved;
        private int _minAchieved;
        private List<int> _lastSetOfFrames = new();
        private int _recheckRate = 3;
        private float _minCheckTimeCache;
        private int[] _frameRateSamples;

        private readonly Dictionary<int, string> CachedNumberStrings = new();

        private void Awake() {
            {
                for (var i = 0; i < _cacheNumbersAmount; i++) CachedNumberStrings[i] = i.ToString();
                _frameRateSamples = new int[_averageFromAmount];
            }
        }

        private void Update() {
            {
                var currentFrame =
                    (int) Math.Round(1f /
                                     Time.smoothDeltaTime); // If your game modifies Time.timeScale, use unscaledDeltaTime and smooth manually (or not).
                _frameRateSamples[_averageCounter] = currentFrame;
            }
            
            {
                var average = 0f;

                foreach (var frameRate in _frameRateSamples) average += frameRate;

                _currentAveraged = (int) Math.Round(average / _averageFromAmount);
                _averageCounter = (_averageCounter + 1) % _averageFromAmount;
            }
            
            {
                Text.text = "FPS: " + _currentAveraged switch {
                    var x when x >= 0 && x < _cacheNumbersAmount => CachedNumberStrings[x],
                    var x when x >= _cacheNumbersAmount => $"> {_cacheNumbersAmount}",
                    var x when x < 0 => "< 0",
                    _ => "?"
                };
                if (_minCheckTimeCache < Time.time) {
                    _minAchieved = _lastSetOfFrames.Min();
                    _maxAchieved = _lastSetOfFrames.Max();
                    _lastSetOfFrames.Clear();
                    _minCheckTimeCache = Time.time + _recheckRate;
                }

                _lastSetOfFrames.Add(_currentAveraged);
                _maxTotalAchieved = _currentAveraged > _maxTotalAchieved ? _currentAveraged : _maxTotalAchieved;
                Text.text += $"{Environment.NewLine}Min in {_recheckRate}: {_minAchieved}";
                Text.text += $"{Environment.NewLine}Max in {_recheckRate}: {_maxAchieved}";
                Text.text += $"{Environment.NewLine}Max total: {_maxTotalAchieved}";
            }
        }
    }
}