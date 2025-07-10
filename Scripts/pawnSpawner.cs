using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pawnSpawner : MonoBehaviour
{
    [SerializeField] private GameObject pawnObject;
    [SerializeField] private float spacing;
    public void spawn(int x, int y)
    {
        Vector3 pos = new Vector3((float)x + transform.position.x, (float)y + transform.position.y, 0);
        pos = pos * spacing;
        GameObject pawn = Instantiate(pawnObject, pos, Quaternion.identity);
        pawnIndividualVisual pawnVis = pawn.GetComponent<pawnIndividualVisual>();
        pawnVis.setCoords(x, y);
    }
}
