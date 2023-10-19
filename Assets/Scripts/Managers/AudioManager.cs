using UnityEngine;
using UnityEngine.Audio;

namespace Managers
{
    public class AudioManager : Singleton<AudioManager>
    {
        [SerializeField] 
        private AudioMixer audioMixer;
        
        [SerializeField]
        private AudioClip clickButtonSound;
        
        [SerializeField]
        private AudioClip clickTileSound;
        
        
        private AudioSource _audioSource;

        public void Initialize()
        {
            _audioSource = gameObject.GetComponent<AudioSource>();
        }
        
        public AudioSource GetAudioSource()
        {
            return _audioSource;
        }
        
        public void PlayAudio(AudioSource source, AudioClip clip)
        {
            if (source.clip == clip) return;
            source.clip = clip;
            source.Play();
        }
        
        public void PlayAudioOneShot(AudioSource source, AudioClip clip)
        {
            source.PlayOneShot(clip);
        }
        
        public void PlayClickButton()
        {
            PlayAudioOneShot(_audioSource, clickButtonSound);
        }
        
        public void PlayClickTile()
        {
            PlayAudioOneShot(_audioSource, clickTileSound);
        }
        
        public void ChangeAudioValue(string groupName, float value)
        {
            float mappedValue = Mathf.Lerp(-80f, 10f, value / 100f);
            mappedValue = Mathf.Clamp(mappedValue, -80f, 10f);
            audioMixer.SetFloat(groupName, mappedValue);
        }

        public float GetMusicVolume()
        {
            float musicValue;
            audioMixer.GetFloat("Music Volume", out musicValue);
            return musicValue;
        }
        
        public float GetSfxVolume()
        {
            float sfxValue;
            audioMixer.GetFloat("SFX Volume", out sfxValue);
            return sfxValue;
        }
    }
}
