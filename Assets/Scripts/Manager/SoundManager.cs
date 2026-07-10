using System.Collections.Generic;
using UnityEngine;

public enum SoundId
{
    PracticeBGM,
    ButtonClick,
    ButtonHover,
    RifleFire,
    MissileFire,
    UltimateStart,
    Heal,
    Dash,
    Jump,
    Footstep,
    BotDeath
}

[System.Serializable]
public class SoundData
{
    public SoundId id;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 1f;

    public bool loop;
}

public class SoundManager : baseManager
{
    private SoundManagerConfigSO config;
    private Dictionary<SoundId, SoundData> soundMap;

    private AudioSource bgmSource;
    private AudioSource sfxSource;
    private AudioSource uiSource;
    private AudioSource footstepSource;

    public SoundManager(SoundManagerConfigSO config)
    {
        this.config = config;
    }

    public override void Init()
    {
        soundMap = new Dictionary<SoundId, SoundData>();

        foreach (var sound in config.sounds)
        {
            if (sound == null) continue;
            if (sound.clip == null) continue;

            soundMap[sound.id] = sound;
        }

        GameObject soundObject = new GameObject("SoundManager_AudioSources");
        Object.DontDestroyOnLoad(soundObject);

        bgmSource = soundObject.AddComponent<AudioSource>();
        sfxSource = soundObject.AddComponent<AudioSource>();
        uiSource = soundObject.AddComponent<AudioSource>();
        footstepSource = soundObject.AddComponent<AudioSource>();
    }

    public override void Update()
    {
    }

    public override void Destory()
    {
        if (bgmSource != null)
        {
            Object.Destroy(bgmSource.gameObject);
        }
    }

    public void PlayBGM(SoundId id)
    {
        if (!soundMap.TryGetValue(id, out SoundData data)) return;

        bgmSource.clip = data.clip;
        bgmSource.volume = data.volume;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void PlaySFX(SoundId id)
    {
        if (!soundMap.TryGetValue(id, out SoundData data)) return;

        sfxSource.PlayOneShot(data.clip, data.volume);
    }

    public void PlayUI(SoundId id)
    {
        if (!soundMap.TryGetValue(id, out SoundData data)) return;

        uiSource.PlayOneShot(data.clip, data.volume);
    }

    public void PlayFootstep(SoundId id)
    {
        if (!soundMap.TryGetValue(id, out SoundData data)) return;

        if (footstepSource.isPlaying) return;

        footstepSource.clip = data.clip;
        footstepSource.volume = data.volume;
        footstepSource.loop = true;
        footstepSource.Play();
    }

    public void StopFootstep()
    {
        footstepSource.Stop();
    }
}