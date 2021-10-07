using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField]
    AudioClip _sfxButtonClick = null;

    AudioSource _aSource;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        _aSource = GetComponent<AudioSource>();
    }

    public void PlaySFX(AudioClip pAudioClip, bool RandomizePitch = false)
    {
        if (RandomizePitch)
            _aSource.pitch = Random.Range(0.8f, 1.2f);
        else
            _aSource.pitch = 1f;

        _aSource.PlayOneShot(pAudioClip);
    }

    public void PlayButtonSFX() => PlaySFX(_sfxButtonClick);
}
