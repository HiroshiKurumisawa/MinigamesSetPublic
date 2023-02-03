using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace SoundSystem
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        // BGM・SEのAudioClipリスト
        public List<AudioClip> bgmAudioClipList = new List<AudioClip>();
        public List<AudioClip> seAudioClipList = new List<AudioClip>();

        [SerializeField, Header("Audio Mixer")]
        public AudioMixer audioMixer;
        public AudioMixerGroup bgmAMG, seAMG;

        // BGM・SEの名AudioSource
        List<AudioSource> bgmAudioSourceList = new List<AudioSource>();
        AudioSource seAudioSource;

        List<IEnumerator> fadeCoroutines = new List<IEnumerator>();

        const int BGMAudioSourceNum = 2;
        const string MasterVolumeParamName = "MasterVolume";
        const string BGMVolumeParamName = "BGMVolume";
        const string SEVolumeParamName = "SEVolume";

        // 一時停止中か
        public bool IsPaused { get; private set; }

        public float MasterVolume
        {
            get { return audioMixer.GetVolumeByLinear(MasterVolumeParamName); }
            set { audioMixer.SetVolumeByLinear(MasterVolumeParamName, value); }
        }
        public float SEVolume
        {
            get { return audioMixer.GetVolumeByLinear(SEVolumeParamName); }
            set { audioMixer.SetVolumeByLinear(SEVolumeParamName, value); }
        }
        public float BGMVolume
        {
            get { return audioMixer.GetVolumeByLinear(BGMVolumeParamName); }
            set { audioMixer.SetVolumeByLinear(BGMVolumeParamName, value); }
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
                return;
            }
            seAudioSource = InitializeAudioSource(gameObject, false, seAMG);
            bgmAudioSourceList = InitializeAudioSources(gameObject, true, bgmAMG, BGMAudioSourceNum);

            IsPaused = false;
        }

        private List<AudioSource> InitializeAudioSources(GameObject parentGameObject, bool isLoop = false, AudioMixerGroup amg = null, int count = 1)
        {
            List<AudioSource> audioSources = new List<AudioSource>();

            for (int i = 0; i < count; i++)
            {
                var audioSource = InitializeAudioSource(parentGameObject, isLoop, amg);
                audioSources.Add(audioSource);
            }

            return audioSources;
        }

        private AudioSource InitializeAudioSource(GameObject parentGameObject, bool isLoop = false, AudioMixerGroup amg = null, int bGMAudioSourceNum = 0)
        {
            var audioSource = parentGameObject.AddComponent<AudioSource>();

            audioSource.loop = isLoop;
            audioSource.playOnAwake = false;

            if (amg != null)
            {
                audioSource.outputAudioMixerGroup = amg;
            }

            return audioSource;
        }

        public void PlayOneShotSe(string clipName)
        {
            var audioClip = seAudioClipList.FirstOrDefault(clip => clip.name == clipName);

            if (audioClip == null)
            {
                Debug.Log(clipName + "は見つかりません");
                return;
            }

            seAudioSource.PlayOneShot(audioClip);
        }

        public void StopSe()
        {
            seAudioSource.Stop();
            seAudioSource.Play();
        }

        public void PlayBGMWithFadeIn(string clipName, float fadeTime = 2f)
        {
            if (IsPaused) { return; }

            var audioClip = bgmAudioClipList.FirstOrDefault(clip => clip.name == clipName);

            if (audioClip == null)
            {
                Debug.Log(clipName + "は見つかりません");
                return;
            }

            if (bgmAudioSourceList.Any(source => source.clip == audioClip))
            {
                Debug.Log(clipName + "はすでに再生されています");
                return;
            }

            StopBGMWithFadeOut(fadeTime); // 現在再生中のBGMをフェードアウトする

            AudioSource audioSource = bgmAudioSourceList.FirstOrDefault(asb => asb.isPlaying == false);

            if (audioSource != null)
            {
                IEnumerator routine = audioSource.PlayWithFadeIn(audioClip, fadeTime);
                fadeCoroutines.Add(routine);
                StartCoroutine(routine);
            }
        }

        public void StopBGMWithFadeOut(string clipName, float fadeTime = 2f)
        {
            if (IsPaused) { return; }

            AudioSource audioSource = bgmAudioSourceList.FirstOrDefault(bas => bas.clip.name == clipName);
            
            if (audioSource == null || audioSource.isPlaying == false)
            {
                Debug.Log(clipName + "は再生されていません");
                return;
            }

            IEnumerator routine = audioSource.StopWithFadeOut(fadeTime);
            StartCoroutine(routine);
            fadeCoroutines.Add(routine);
        }

        public void StopBGMWithFadeOut(float fadeTime = 2f)
        {
            if (IsPaused) { return; }

            fadeCoroutines.ForEach(StopCoroutine);
            fadeCoroutines.Clear();

            fadeCoroutines = bgmAudioSourceList.Where(asb => asb.isPlaying)
                .ToList()
                .ConvertAll(asb =>
                {
                    IEnumerator routine = asb.StopWithFadeOut(fadeTime);
                    StartCoroutine(routine);
                    return routine;
                });
        }
    }
}