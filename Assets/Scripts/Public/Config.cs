using System.Collections;
using System.Collections.Generic;
using Assets.PixelFantasy.PixelHeroes.Common.Scripts.CollectionScripts;
using UnityEngine;

public class Config : MonoBehaviour
{
    public readonly static Vector2Int START_POS = new Vector2Int(-8, 4);
    public readonly static Vector2Int GOAL_POS = new Vector2Int(8, -3);
    public readonly static int CREATE_ENEMY_CNT = 10;
}