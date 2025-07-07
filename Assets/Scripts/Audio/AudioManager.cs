using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace AudioManagement
{
    public class AudioManager : MonoBehaviour
    {
        static AudioManager instance;
        public static AudioManager Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new GameObject("AudioManager").AddComponent<AudioManager>();
                    DontDestroyOnLoad(instance);
                }
                return instance;
            }
        }
        readonly List<AudioSource> audioSourcePool = new();
        AudioSource GetSource()
        {
            if(audioSourcePool.Count == 0)
            {
                var tmp = new GameObject() { hideFlags = HideFlags.HideInHierarchy }.AddComponent<AudioSource>();
                DontDestroyOnLoad(tmp);
                return tmp;
            }
            else
            {
                var tmp = audioSourcePool[0];
                audioSourcePool.RemoveAt(0);
                return tmp;
            }
        }
        void ReleaseSource(AudioSource source) => audioSourcePool.Add(source);

        readonly List<PlayingAudio> playing = new(), removeQueue = new();

        PlayingAudio music;
        public event Action<PlayingAudio> onMusicChange;
        bool paused = false;

        private void OnApplicationPause(bool pause)
        {
            paused = pause;
        }
        private void Update()
        {
            foreach (var i in playing)
            {
                i.UpdateVolume();
                if (!paused && !i.source.isPlaying)
                {
                    i.Expire();
                    removeQueue.Add(i);
                    ReleaseSource(i.source);
                }
            }
            foreach (var i in removeQueue) playing.Remove(i); removeQueue.Clear();
        }
        public PlayingAudio PlaySound(Sound sound, VolumeGroup volumeGroup = VolumeGroup.None)
        {
            PlayingAudio tmp = new(sound, GetSource(), volumeGroup);
            tmp.source.loop = false;
            tmp.Play();
            playing.Add(tmp);
            return tmp;
        }
        public static float GetVolume(VolumeGroup volumeGroup)
        {
            return 1.0f;
        }
        private void OnDestroy()
        {
            foreach (var i in playing) i.Expire();
        }
        public void ChangeMusic(Sound nextMusic, float musicFadeTime = 1.0f)
        {
            if(music != null) music.FadeOut(musicFadeTime, () =>
            {
                if (nextMusic != null)
                {
                    music = PlaySound(nextMusic, VolumeGroup.Music).SetLoop(true);
                    onMusicChange?.Invoke(music);
                }
                else music = null;
            });
            else
            {
                if (nextMusic != null)
                {
                    music = PlaySound(nextMusic, VolumeGroup.Music).SetLoop(true);
                    onMusicChange?.Invoke(music);
                }
                else music = null;
            }
        }
    }
    [Serializable, Flags]
    public enum VolumeGroup
    {
        None = 0,
        Music = 1<<0
    }
    public class PlayingAudio
    {
        public AudioClip clip => sound.clip;

        internal readonly Sound sound;
        internal readonly AudioSource source;
        internal readonly VolumeGroup volumeGroup;
        bool expired = false;
        public PlayingAudio(Sound sound, AudioSource source, VolumeGroup volumeGroup)
        {
            this.sound = sound;
            this.source = source;
            source.playOnAwake = false;
            this.volumeGroup = volumeGroup;
        }

        float volume = 1.0f;

        bool fadingOut = false;
        float fadeOutTime, fadeOutCounter;
        Action onFadeoutFinish;

        Func<Vector3> locationGetter;
        float distMin, distMax;
        bool is2D;
        internal void UpdateVolume()
        {
            source.volume = volume * sound.volume * AudioManager.GetVolume(volumeGroup);
            if (locationGetter != null)
            {
                Vector3 location = locationGetter.Invoke();
                float dist = is2D ? Vector2.Distance(Camera.main.transform.position, location) : Vector3.Distance(Camera.main.transform.position, location);

                if (dist <= distMin) source.volume *= 1.0f;
                else if (dist < distMax) source.volume *= 1.0f - (dist - distMin) / (distMax - distMin);
                else source.volume *= 0.0f;
            }
            if (fadingOut)
            {
                fadeOutCounter -= Time.deltaTime;
                if (fadeOutCounter <= 0.0f)
                {
                    fadingOut = false;
                    Stop();
                    onFadeoutFinish?.Invoke();
                }
                else volume *= (fadeOutCounter / fadeOutTime);
            }
        }
        internal void Expire()
        {
            expired = true;
        }
        public void Play()
        {
            if (expired) return;
            source.clip = sound.clip;
            UpdateVolume();
            source.Play();
        }
        public void Stop()
        {
            source.Stop();
        }
        public void FadeOut(float time, Action onFinish = null)
        {
            if (fadingOut) return;
            fadingOut = true;
            fadeOutTime = time;
            fadeOutCounter = time;
            onFadeoutFinish = onFinish;
        }
        public PlayingAudio SetVolume(float volume)
        {
            this.volume = volume;
            UpdateVolume();
            return this;
        }
        public PlayingAudio SetLoop(bool loop)
        {
            source.loop = loop;
            return this;
        }
        public PlayingAudio SetLocation(Func<Vector3> locationGetter, float distMin, float distMax, bool is2D = false)
        {
            if (distMax < distMin) distMax = distMin;
            this.locationGetter = locationGetter;
            this.distMin = distMin;
            this.distMax = distMax;
            this.is2D = is2D;
            UpdateVolume();
            return this;
        }
    }
}
