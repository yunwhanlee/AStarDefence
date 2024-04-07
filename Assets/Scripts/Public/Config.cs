using System.Collections;
using System.Collections.Generic;
using Assets.PixelFantasy.PixelHeroes.Common.Scripts.CollectionScripts;
using UnityEngine;

public static class Config {
#region HOME
    public const int MAX_GOBLINKEY = 5;
    public const int EQUIP_UPGRADE_MAX = 10;
    public const int RELIC_UPGRADE_MAX = 5;

    /// <summary>
    /// Enum.AbilityTypeと連結
    /// </summary>
    public static string[] ABILITY_TYPE_DECS_MSGS = {
        "공격력 V% 증가",
        "공격속도 V% 증가",
        "공격범위 V% 증가",
        "치명타 V% 증가",
        "치명타데미지 V% 증가",
        "클리어 경험치 V% 추가",
        "클리어 코인 V% 추가",
        "시작코인 +V% 지원",
        "시작체력 +V% 지원",
        "아이템 획득 +V% 추가",
        "추가 스킬포인트 +V% 추가",
        "10킬당 V코인 추가 획득 ",
        "전사타워 공격력 V% 증가",
        "궁수타워 공격력 V% 증가",
        "법사타워 공격력 V% 증가",
        "전사타워 업글 비용 V% 감소",
        "궁수타워 업글 비용 V% 감소",
        "법사타워 업글 비용 V% 감소",
    };

#endregion

#region GAME
    public readonly static Vector2Int START_POS = new Vector2Int(-8, 3);
    public readonly static Vector2Int GOAL_POS = new Vector2Int(8, -3);

    public const int FREE_BOARD_CNT = 5;
    public const int FREE_BREAKROCK_CNT = 2;

    public const int DEFAULT_MONEY = 100;
    public const int DEFAULT_LIFE = 10;
    public const int DEFAULT_RESET_CNT = 10;

    public const int INC_MAX_LIFE = 15;
#endregion

#region PRICE
    public class G_PRICE {
        public const int BREAK = 20;
        public const int BOARD = 10;
        public const int TOWER = 15;
        public const int CCTOWER = 25;
        public const int CC_UPG = 30;
        public const int MERGE = 5;
        public const float DELETE_REFUND_PER = 0.5f;
        public const int BOSS_KILL_MONEY_BONUS = 65;
    }
    public class H_PRICE {
        public static readonly int[] WORKSPACE_PRICES = {0, 1000, 5000, 12500, 30000};
        public readonly struct EQUIP_UPG {
            public static readonly int[] PRICES = {100, 200, 250, 300, 400, 500, 600, 800, 1000};
            public static readonly int[] PERS = {90, 80, 70, 60, 45, 35, 25, 20, 10, 5};
        }
        public readonly struct RELIC_UPG {
            public static readonly int[] PRICES = {200, 450, 500, 750, 1000};
            public static readonly int[] PERS = {70, 40, 20, 5};
        }
    }
#endregion
}