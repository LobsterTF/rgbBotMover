// Eyrie inspired puzzle game november 2024
// Visual aid showing tile information

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class pawnIndividualVisual : MonoBehaviour
{
    public Vector3Int coordinates;
    [SerializeField] private GameObject pawn, factory, deposite, yellowCommand;
    [SerializeField] private GameObject[] wall, autoMoverArrows;
    [SerializeField] private SpriteRenderer depositeSR;
    [SerializeField] private TMP_Text pawnCountText, depositeText;
    [SerializeField] private Color inactiveColor, activeColor;
    [SerializeField] private Animator pawnAnimator;
    [SerializeField] private GameObject highlightSoundObj;

    public void setCoords(int x, int y)
    {
        coordinates = new Vector3Int(x, y,0);
    }
    public void UpdateVisuals(int pawnCount, bool hasFactory, int depositeCount, bool isFinalTurn, int autoMoverDir, bool hasYellowCommand)
    {
        
        if (pawnCount == 0)
        {
            pawn.SetActive(false);
        }
        else
        {
            pawn.SetActive(true);
            pawnCountText.text = "x" + pawnCount.ToString();
        }
        if (hasFactory) { factory.SetActive(true); }else { factory.SetActive(false); }
        if (isFinalTurn) { depositeSR.color = activeColor; } else { depositeSR.color = inactiveColor; }
        if (depositeCount == 0) {deposite.SetActive(false); } else
        {
            depositeText.text = depositeCount.ToString(); 
        }
        if (autoMoverDir != 0)
        {
            autoMoverArrows[autoMoverDir - 1].SetActive(true);
        }
        yellowCommand.SetActive(hasYellowCommand);
    }
    public void setWalls(int wallNumber)
    {
        wall[wallNumber].SetActive(true);
    }
    public void mouseEnter()
    {
        
        if (pawn.activeSelf)
        {
            pawnAnimator.Play("pawnHighlight", 0, .65f);
            GameObject snd = Instantiate(highlightSoundObj, transform.position, Quaternion.identity);
            soundObject sndObjScrpt = snd.GetComponent<soundObject>();
            sndObjScrpt.playSound(1.4f);
        }
    }
    public void mouseLeave()
    {
        if (pawn.activeSelf)
        {
            pawnAnimator.Play("pawnDehighlight", 0, .65f);
        }
    }
}
