// Eyrie inspired puzzle game november 2024
// Level builder

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Map", menuName = "ScriptableObjects/Map", order = 0)]
public class levelScriptable : ScriptableObject
{
    public Vector2Int dimensions;
    public tileData[] tileData;
    public tileColor[] commands;
    [Tooltip("starts at 0 (0 being 1 pawn)")] public int pawnMax;
}
