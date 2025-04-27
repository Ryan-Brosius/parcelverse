using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using DG.Tweening;

public class SoundManager : MonoBehaviour
{
    private const float DEFAULT_PITCH_VARIATION = 0.1f;
    private const float PITCH_MODIFIER_MULTIPLIER = 0.05f;
    private const float SOUND_CLEANUP_DELAY_MULTIPLIER = 1.5f;

    private bool gameOver;

    private Dictionary<string, List<GameObject>> activeSounds = new Dictionary<string, List<GameObject>>();

    [SerializeField]
    [Range(0, 1)]
    private float volumeMultiplier = 1;

    [SerializeField]
    private List<SoundEffect> soundEffects;

    private static SoundManager instance;
    public static SoundManager Instance => instance;

    private void Awake()
    {
        InitializeSingleton();
    }

    private void InitializeSingleton()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void PlaySoundEffect(string soundName) => PlaySoundEffect(soundName, 0);

    public void PlaySoundEffect(string soundName, int modifier)
    {
        if (gameOver)
            return;

        SoundEffect? soundEffect = GetRandomMatchingSoundEffect(soundName);
        if (!soundEffect.HasValue || !HasClips(soundEffect.Value)) return;

        AudioClip clip = GetRandomClip(soundEffect.Value);
        GameObject soundObject = CreateSoundGameObject(soundName, clip);
        if (soundObject == null) return;

        ConfigureAndPlaySound(soundObject, soundEffect.Value, clip, modifier);

        if (!activeSounds.ContainsKey(soundName))
        {
            activeSounds[soundName] = new List<GameObject>();
        }
        activeSounds[soundName].Add(soundObject);
    }

    public void StopSound(string soundName)
    {
        if (!activeSounds.ContainsKey(soundName)) return;

        foreach (var soundObject in activeSounds[soundName])
        {
            if (soundObject != null)
            {
                var audioSource = soundObject.GetComponent<AudioSource>();
                if (audioSource != null)
                {
                    audioSource.Stop();
                }
                Destroy(soundObject);
            }
        }

        activeSounds[soundName].Clear();
    }

    private void CleanupDestroyedSounds()
    {
        var keysToUpdate = activeSounds.Keys.ToList();
        foreach (var key in keysToUpdate)
        {
            activeSounds[key].RemoveAll(obj => obj == null);
        }
    }

    private void OnDestroy()
    {
        foreach (var soundList in activeSounds.Values)
        {
            foreach (var soundObject in soundList)
            {
                if (soundObject != null)
                {
                    Destroy(soundObject);
                }
            }
        }
        activeSounds.Clear();
    }

    private SoundEffect? GetRandomMatchingSoundEffect(string soundName)
    {
        var matchingEffects = soundEffects
            .Where(s => s.SoundName.Equals(soundName))
            .ToList();

        if (!matchingEffects.Any()) return null;

        return matchingEffects[Random.Range(0, matchingEffects.Count)];
    }

    private bool HasClips(SoundEffect soundEffect)
    {
        return soundEffect.Clips != null && soundEffect.Clips.Count > 0;
    }

    private AudioClip GetRandomClip(SoundEffect soundEffect)
    {
        return soundEffect.Clips[Random.Range(0, soundEffect.Clips.Count)];
    }

    private GameObject CreateSoundGameObject(string soundName, AudioClip clip)
    {
        if(clip == null) {
            Debug.LogWarning("Invalid audio clip");
            return null;
        }

        var soundObject = new GameObject($"Sound: {soundName}, {clip.length}s");
        soundObject.transform.parent = transform;
        StartCoroutine(RemoveFromTrackingAfterPlay(soundName, soundObject, clip.length * SOUND_CLEANUP_DELAY_MULTIPLIER));
        return soundObject;
    }

    private IEnumerator RemoveFromTrackingAfterPlay(string soundName, GameObject soundObject, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (activeSounds.ContainsKey(soundName))
        {
            activeSounds[soundName].Remove(soundObject);
        }
        Destroy(soundObject);
    }

    private void ConfigureAndPlaySound(GameObject soundObject, SoundEffect soundEffect, AudioClip clip, int modifier)
    {
        AudioSource source = AddAndConfigureAudioSource(soundObject, clip, soundEffect);
        ApplyPitchModification(source, soundEffect, modifier);
        source.Play();
    }

    private AudioSource AddAndConfigureAudioSource(GameObject soundObject, AudioClip clip, SoundEffect soundEffect)
    {
        var source = soundObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = soundEffect.Volume * volumeMultiplier;
        return source;
    }

    private void ApplyPitchModification(AudioSource source, SoundEffect soundEffect, int modifier)
    {
        if (soundEffect.Vary)
        {
            source.pitch += Random.Range(-DEFAULT_PITCH_VARIATION, DEFAULT_PITCH_VARIATION);
        }
        source.pitch += PITCH_MODIFIER_MULTIPLIER * modifier;
    }
}

[System.Serializable]
public struct SoundEffect
{
    private string name;

    public string SoundName;
    public List<AudioClip> Clips;

    [Range(0, 1)]
    public float Volume;

    public bool Vary;
}