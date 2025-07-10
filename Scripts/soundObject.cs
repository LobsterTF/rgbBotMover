using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soundObject : MonoBehaviour
{
    public AudioSource audioSource;
    public float timeToDel, volume;
    static public bool soundOff;
    // Start is called before the first frame update
    public void playSound(float pitch)
    {

        audioSource.pitch = pitch;
        if (soundOff == false)
        {
            audioSource.volume = volume;
            audioSource.Play(0);
            StartCoroutine(del());
        }
        else
        {
            Destroy(gameObject);
        }

    }
    IEnumerator del()
    {
        yield return new WaitForSeconds(timeToDel);
        Destroy(gameObject);


    }
}
