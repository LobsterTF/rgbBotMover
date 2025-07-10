using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class infographicManager : MonoBehaviour
{
    [SerializeField] private GameObject infoGraphic;
    private bool infoGraphicActive = false;
    public void toggleInfographic()
    {
        infoGraphicActive = !infoGraphicActive;
        infoGraphic.SetActive(infoGraphicActive);
    }
}
