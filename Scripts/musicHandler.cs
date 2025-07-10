using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class musicHandler : MonoBehaviour
{
    public AudioSource audioSource;
    public float volume;
    bool musicEnabled;
    [SerializeField] private Animator soundAnimator, strippedSoundAnimator;
    bool playStripped = false;
    void Start()
    {
        strippedSoundAnimator.Play("off", 0);
        int musicSet = PlayerPrefs.GetInt("music");
        if (musicSet == 0) { musicEnabled = false; }else { musicEnabled = true; }
    }
    void Update()
    {
        if (!musicEnabled)
        {
            strippedSoundAnimator.Play("off", 0);
            soundAnimator.Play("off", 0);
        }
    }
    public void startStop(bool In)
    {
        if (In)
        {
            if (playStripped)
            {
                strippedSoundAnimator.Play("fadeIn", 0);

            }
            else
            {
                soundAnimator.Play("fadeIn", 0);

            }
        }
        else
        {

            soundAnimator.Play("fade", 0);

        }
    }
    public void swapTrack()
    {
        playStripped = true;
    }
    public void playSound(float pitch)
    {

        audioSource.pitch = pitch;
        

    }
}
