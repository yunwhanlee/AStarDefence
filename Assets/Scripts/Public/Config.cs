using System.Collections;
using System.Collections.Generic;
using Assets.PixelFantasy.PixelHeroes.Common.Scripts.CollectionScripts;
using UnityEngine;

public static class Config{
    public readonly static Vector2Int START_POS = new Vector2Int(-8, 3);
    public readonly static Vector2Int GOAL_POS = new Vector2Int(8, -3);

#region GAME PRICE
    public class PRICE {
        public const int BREAK = 20;
        public const int BOARD = 10;
        public const int TOWER = 15;
        public const int CCTOWER = 20;
        public const int CC_UPG = 20;
        public const int MERGE = 5;
        public const float DELETE_REFUND_PER = 0.5f;
    }
#endregion    
}