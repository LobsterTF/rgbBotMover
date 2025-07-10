// Eyrie inspired puzzle game november 2024
// All Data in a tile

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class tileData
{
    [SerializeField] public tileColor color;
    [SerializeField] public int pawnCount, depositAmount;
    [SerializeField] public bool hasFactory, exists, hasYellowCommand;
    [Tooltip(" 0 bottom, 1 right, 2 up, 3 left")] [SerializeField] public bool[] hasWall = new bool[4]; // 0 bottom, 1 right, 2 up, 3 left
    [SerializeField] public conveyorBelt autoMover;
}
public enum tileColor { Red, Green, Blue, Yellow }
public enum conveyorBelt { None, Down, Right, Up, Left }