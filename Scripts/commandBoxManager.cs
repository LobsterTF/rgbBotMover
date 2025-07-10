// Eyrie inspired puzzle game november 2024
// All visual aids found around the command box

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class commandBoxManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] boxSpriteRenderer, pawnsInSupply, forecastSpriteRenderers;
    private bool colorblindMode = true;
    [SerializeField] private Color[] colors;
    [SerializeField] private Sprite[] tile, cbTile;
    public void setColorBlind(bool isOn)
    {
        colorblindMode = isOn;
    }
    public void setColor(int selection, int color)
    {
        //boxSpriteRenderer[selection].color = colors[color];
        if (colorblindMode)
        {
            boxSpriteRenderer[selection].sprite = cbTile[color];
        }
        else
        {
            boxSpriteRenderer[selection].sprite = tile[color];
        }
        
    }
    public void setForecastColor(int selection, int color)
    {
        if (colorblindMode && color != 4)
        {
            forecastSpriteRenderers[selection].sprite = cbTile[color];
        }
        else
        {
            forecastSpriteRenderers[selection].sprite = tile[color];

        }
    }
    public void setExhausted(int selection)
    {
        if (colorblindMode)
        {
            boxSpriteRenderer[selection].sprite = cbTile[4];
        }
        else
        {
            boxSpriteRenderer[selection].sprite = tile[4];
        }
    }
    public void setInactive(int selection)
    {
        if (colorblindMode)
        {
            boxSpriteRenderer[selection].sprite = cbTile[5];
        }
        else
        {
            boxSpriteRenderer[selection].sprite = tile[5];

        }

    }
    public void enableDisablePawn(int selection, bool enabled)
    {
        pawnsInSupply[selection].enabled = enabled;
    }
}
