using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    public bool isPlaying;
    public void PlayOneShotSe(AudioClip clip)
    {
        gameObject.SetActive(true);
        audioSource.PlayOneShot(clip);
        isPlaying = true;

    }
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (!audioSource.isPlaying)
        {
            isPlaying = false;
            audioSource.clip = null;
            gameObject.SetActive(false);
        }
    }
}
