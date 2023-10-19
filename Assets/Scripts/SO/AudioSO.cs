using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SO
{
    [CreateAssetMenu(menuName = "Data/Audios", fileName = "AudioSO")]
    public class AudioSO : ScriptableObject
    {
        [TableList] public List<AudioReference> audioReferences;
    }

    [Serializable]
    public class AudioReference
    {
        public string key;
        public AudioClip audio;
    }
}
