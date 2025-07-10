using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tutorialHandler : MonoBehaviour
{
    [SerializeField] private GameObject[] tutorialStuffHolders, levelOneGraphics, levelTwoGraphics;
    public void activateLevelTutorial(int selection)
    {
        if (selection < tutorialStuffHolders.Length)
        {
            tutorialStuffHolders[selection].SetActive(true);
        }
    }

    public void checkForUpdateGraphic(int level)
    {
        if (level == 1)
        {
            levelOneGraphics[0].SetActive(false);
            levelOneGraphics[1].SetActive(true);
        }
        if (level == 2)
        {
            levelTwoGraphics[0].SetActive(false);
        }
    }
}
