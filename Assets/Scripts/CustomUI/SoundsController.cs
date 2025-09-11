using System.Collections;
using System.Collections.Generic;
using DuloGames.UI;
using UnityEngine;

[DisallowMultipleComponent]
public class SoundsController : MonoBehaviour
{
    #region Singleton
    public static SoundsController Instance; 
    #endregion

    [SerializeField] private AudioSource loopAudioSource;
    [SerializeField] private AudioSource musicAudioSource;
    public float volume { get { return PlayerPrefs.GetFloat(Config.SOUND_VOLUME_KEY, 1); }}

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            if (Instance.gameObject)
                Destroy(Instance.gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(transform.gameObject);
          
    }


    public void PlayAudio(AudioClip clip,bool isLoop=true,bool stop=false)
    {
        if (this.loopAudioSource != null)
        {
            this.loopAudioSource.clip = clip;
            this.loopAudioSource.volume = volume;
            this.loopAudioSource.loop = isLoop;
            if (stop)
            {
                if(this.loopAudioSource.isPlaying) this.loopAudioSource.Stop();
            }
            else
            {
                if (!this.loopAudioSource.isPlaying) this.loopAudioSource.Play();
            }
        }
    }

    public void PlayOneShot(AudioClip clip)    {
        
        UIAudioSource.Instance.PlayAudio(clip);
    }

    public void PlayOneShot(AudioSource mSource, AudioClip clip)
    {
        if(mSource !=null && clip != null)
        mSource.volume = volume;
        mSource.PlayOneShot(clip); 
    }

    public void PlayAudio(AudioSource mSource)
    {
        if (mSource != null)
        {
            mSource.volume = volume;
            mSource.Play();
        }
    }

    public void StopAudio(AudioSource mSource)
    {
        if (mSource != null && mSource.isPlaying)
        { 
            mSource.Stop();
        }
    }

    public void PlayMusic()
    {
        //if (musicAudioSource != null)
        {
            musicAudioSource.volume = PlayerPrefs.GetFloat(Config.MUSIC_VOLUME_KEY, 1);
            musicAudioSource.Play();
        }
    }

    public void StopMusic()
    {
        if ( musicAudioSource != null)
        {
            musicAudioSource.volume = PlayerPrefs.GetFloat(Config.MUSIC_VOLUME_KEY, 1);
            if (musicAudioSource.isPlaying)
             musicAudioSource.Stop();
        }
    }
}
