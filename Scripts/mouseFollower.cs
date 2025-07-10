// Eyrie inspired puzzle game november 2024
// visual aid following mouse

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class mouseFollower : MonoBehaviour
{
    
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject botCarryHolder;
    [SerializeField] private TMP_Text carryAmount;
    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mousePos.x,mousePos.y, 0);
    }
    public void setCarryNum(int number)
    {
        if (number == 0) 
        {
            botCarryHolder.SetActive(false);
        }
        else
        {
            botCarryHolder.SetActive(true);
            carryAmount.text = "x" + number.ToString();
        }
    }
}
