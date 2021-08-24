using System.Collections.Generic;
using UnityEngine;

namespace WKC
{
    public class AudioManager : Singleton<AudioManager>
    {
        private AudioSource bgmSource;
        private List<AudioSource> sourceDic;
        private Dictionary<string, AudioClip> clipDic;
        private string CurrentBGM;

        public float VolumeBGM = 1;
        public float VolumeSound = 1;

        private void Awake()
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
            sourceDic = new List<AudioSource>();
            clipDic = new Dictionary<string, AudioClip>();
        }

        #region BGM
        public void PlayBGM(string name, bool loop = true)
        {
            if (CurrentBGM == name)
            {
                return;
            }
            CurrentBGM = name;
            bgmSource.clip = GetClip(name);
            bgmSource.Play();
            bgmSource.volume = VolumeBGM;
            bgmSource.loop = loop;
        }
        
        public void PauseBGM()
        {
            if (bgmSource.clip != null)
            {
                bgmSource.Pause();
            }
        }
        
        public void StopBGM()
        {
            if (bgmSource.clip != null)
            {
                bgmSource.Stop();
            }
        }
        
        public void ResumeBGM()
        {
            if (bgmSource.clip != null && !bgmSource.isPlaying)
            {
                bgmSource.Play();
            }
        }
        #endregion

        public AudioClip GetClip(string name)
        {
            if (!clipDic.ContainsKey(name))
            {
                LoadAudio(name);
            }
            return clipDic[name];
        }

        private void LoadAudio(string name)
        {
            AudioClip audioClip = null;
            switch (GameConfig.Instance.loadType)
            {
                case AssetLoadType.Resources:
                    audioClip = Resources.Load<AudioClip>("Sounds/" + name);
                    break;
                case AssetLoadType.AssetBundle:
                    audioClip = AssetBundleManager.Instance.LoadRes<AudioClip>("sounds", name);
                    break;
            }
            clipDic.Add(name, audioClip);
        }

        
        public void PlaySound(string name, bool loop = false)
        {
            bool isPlayed = false;
            foreach (AudioSource item in sourceDic)
            {
                if (!item.isPlaying)
                {
                    isPlayed = true;

                    item.clip = GetClip(name);
                    item.loop = loop;
                    item.volume = VolumeSound;
                    item.Play();
                    break;
                }
            }
            if (!isPlayed)
            {
                AudioSource temp = gameObject.AddComponent<AudioSource>();
                temp.clip = GetClip(name);
                temp.loop = loop;
                temp.volume = VolumeSound;
                temp.Play();
                sourceDic.Add(temp);
            }
        }
    }
}