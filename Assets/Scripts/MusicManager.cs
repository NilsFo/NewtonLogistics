using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public GameObject temporalAudioPlayerPrefab;
    public static float userDesiredMusicVolume = 0.35f;
    public static float userDesiredSoundVolume = 0.5f;
    public readonly float GLOBAL_MUSIC_VOLUME_MULT = 0.5f;
    public readonly float GLOBAL_SOUND_VOLUME_MULT = 1.5f;

    private AudioListener _listener;
    public AudioSource song;
    private List<AudioSource> playList;

    private void Awake()
    {
        playList = new List<AudioSource>();
        playList.Add(song);

        _listener = FindObjectOfType<AudioListener>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    private void Play(int i)
    {
        foreach (AudioSource audioSource in playList)
        {
            audioSource.Stop();
        }

        playList[i].Play();
    }

    public void PlaySongLoop()
    {
        Play(0);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = _listener.transform.position;
        userDesiredSoundVolume = MathF.Min(userDesiredMusicVolume * 3.5f, 1.0f);
    }

    public float GetVolumeMusic()
    {
        return userDesiredMusicVolume * GLOBAL_MUSIC_VOLUME_MULT;
    }

    public float GetVolumeSound()
    {
        return userDesiredSoundVolume * GLOBAL_SOUND_VOLUME_MULT;
    }

    public void CreateAudioClip(AudioClip audioClip, float volumeMult = 1.0f)
    {
        CreateAudioClip(audioClip, transform.position, volumeMult);
    }

    public void CreateAudioClip(AudioClip audioClip, Vector3 position, float volumeMult = 1.0f)
    {
        GameObject adp = Instantiate(temporalAudioPlayerPrefab);
        adp.transform.position = position;
        AudioSource source = adp.GetComponent<AudioSource>();
        source.clip = audioClip;
        source.volume = MathF.Min(GetVolumeSound() * volumeMult, 1.0f);
        source.Play();
    }
}