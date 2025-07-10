using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buttonHighlighter : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject highlightSoundObj;

    public void mouseEnter()
    {

        animator.Play("highlight", 0, .65f);
        GameObject snd = Instantiate(highlightSoundObj, transform.position, Quaternion.identity);
        soundObject sndObjScrpt = snd.GetComponent<soundObject>();
        sndObjScrpt.playSound(1f);
    }
    public void mouseLeave()
    {
        animator.Play("dehighlight", 0, .65f);

    }
}
