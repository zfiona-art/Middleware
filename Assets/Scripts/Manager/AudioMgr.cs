using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMgr : SingletonMono<AudioMgr>
{
    private AudioSource bg_source;
    private List<AudioSource> other_sources;
    private Dictionary<string, AudioClip> audioClipDic;
    private const int SourceCnt = 8;
    
    public void Init()
    {
        audioClipDic = new Dictionary<string, AudioClip>();

        GameObject bgmusic = new GameObject("bgmusic");
        bgmusic.transform.parent = transform;
        bg_source = bgmusic.AddComponent<AudioSource>();
        bg_source.playOnAwake = false;
        bg_source.loop = true;

        other_sources = new List<AudioSource>();
        for (int i = 0; i < SourceCnt; i++)
        {
            GameObject other = new GameObject("othermusic" + i);
            other.transform.parent = transform;
            var source = other.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = false;
            other_sources.Add(source);
        }
        

        SetVoice(PlayerPrefs.GetInt("tog_sound", 1) == 1);
    }

    private AudioClip GetClip(string name)
    {
        name = "audio/" + name;
        if (!audioClipDic.ContainsKey(name))
        {
            AudioClip clip = Resources.Load<AudioClip>(name);
            audioClipDic.Add(name, clip);
        }
        return audioClipDic[name];
    }

    private bool canVoice;
    public void SetVoice(bool val)
    {
        if (val)
        {
            //PlayBg();
            canVoice = true;
        }
        else
        {
            //StopBg();
            canVoice = false;
        }
    }

    public void PlayBg()
    {
        string name = "audio/bg";
        bg_source.clip = GetClip(name);
        bg_source.volume = 1;
        bg_source.Play();
    }

    public void StopBg()
    {
        bg_source.Pause();
    }

    public void PlayClip(string name)
    {
        if (!canVoice) return;
        
        AudioClip clip = GetClip(name);
        if(clip == null)
            return;
        for (int i = 0; i < SourceCnt; i++)
        {
            var source = other_sources[i];
            if (source.clip == null)
            {
                source.clip = clip;
                source.volume = 1;           
                source.Play();
                vp_Timer.In(clip.length, () =>
                {
                    source.clip = null;
                });
                return;
            }
        }
    }
    
    
}
