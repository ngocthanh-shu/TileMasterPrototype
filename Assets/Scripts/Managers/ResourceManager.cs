using System.Collections.Generic;
using SO;
using UnityEngine;
using UnityEngine.Serialization;

namespace Managers
{
    public class ResourceManager : Singleton<ResourceManager>
    {
        private Dictionary<string, AudioClip> _dictionaryAudio;
        
        public AudioSO audioSo;
        
        public void Initialize()
        {
            _dictionaryAudio = new Dictionary<string, AudioClip>();
        }
        
        public void LoadAudioData()
        {
            foreach(var audioRef in audioSo.audioReferences)
            {
                _dictionaryAudio.Add(audioRef.key, audioRef.audio);
            }
        }
        
        public Dictionary<string, AudioClip> GetDictionaryAudio()
        {
            return _dictionaryAudio;
        }
    }
}
