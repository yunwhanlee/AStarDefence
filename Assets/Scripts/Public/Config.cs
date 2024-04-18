using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.PixelFantasy.PixelHeroes.Common.Scripts.CollectionScripts;
using UnityEngine;

public static class Config {
#region HOME
    public const int MAX_GOBLINKEY = 5;
    public const int EQUIP_UPGRADE_MAX = 11;
    public const int RELIC_UPGRADE_MAX = 6;
    public const float CLOVER_BONUS_EXP_PER = 0.2f;
    public const float GOLDCLOVER_BONUS_EXP_PER = 0.5f;

    /// <summary>
    /// Enum.AbilityTypeと連結
    /// </summary>
    public static string[] ABILITY_DECS = {
        "공격력 V% 증가",
        "공격속도 V% 증가",
        "공격범위 V% 증가",
        "치명타 V% 증가",
        "치명타데미지 V% 증가",
        "클리어 경험치 V% 추가",
        "클리어 코인 V% 추가",
        "시작코인 +V 지원",
        "시작체력 +V 지원",
        "아이템 획득 +V% 추가",
        // "추가 스킬포인트 +V개 추가",
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

    public const int CONSUMEITEM_WAIT_TURN = 1;
    public const float STREAMPACK_DURATION = 10;
    public const float STEAMPACK0_EXTRA_DMG_PER = 0.5f;
    public const float STEAMPACK1_EXTRA_SPD_PER = 0.2f;

    public const int BLIZZARDSCROLL_SLOW_SEC = 5;
    public const int LIGHTNINGSCROLL_STUN_SEC = 3;

    public const int GOBLIN_DUNGEON_STAGE = 5;

    public class STAGE {
        public static readonly float[] DIFF_INC_HP_PERS = {
            1, 2.75f, 7
        };
    }
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
        public static readonly int[] WORKSPACE_PRICES = {
            0, 1000, 5000, 12500, 30000
        };
        public readonly struct EQUIP_UPG {
            private static readonly int[] PRICES = {
                100, 200, 250, 300, 400, 500, 600, 800, 1000, 1300
            };
            public static int[] GetEquipUpgradePriceArr(Enum.Grade grade) {
                Debug.Log($"GetEquipUpgradePriceArr(grade= {grade})::");
                int[] prices = PRICES.ToArray(); //* コピー
                
                //* 等級による追加値段％
                float extraPer = (grade == Enum.Grade.Common)? 1
                    : (grade == Enum.Grade.Rare)? 1.2f
                    : (grade == Enum.Grade.Epic)? 1.4f
                    : (grade == Enum.Grade.Unique)? 1.7f
                    : (grade == Enum.Grade.Legend)? 2.1f
                    : (grade == Enum.Grade.Myth)? 2.5f
                    : (grade == Enum.Grade.Prime)? 3
                    : 1;

                //* 結果
                for(int i = 0; i < PRICES.Length; i++) 
                    prices[i] = Mathf.RoundToInt(prices[i] * extraPer);

                return prices;
            }
            public static readonly int[] PERS = {
                90, 80, 70, 60, 45, 35, 25, 15, 10, 5
            };
        }

        public readonly struct RELIC_UPG {
            private static readonly int[] PRICES = {
                200, 400, 700, 1000, 1400
            };
            public static int[] GetRelicUpgradePriceArr(Enum.Grade grade) {
                Debug.Log($"GetRelicUpgradePriceArr(grade= {grade})::");
                int[] prices = PRICES.ToArray(); //* コピー
                
                //* 等級による追加値段％
                float extraPer = (grade == Enum.Grade.Epic)? 1
                    : (grade == Enum.Grade.Unique)? 1.7f
                    : (grade == Enum.Grade.Legend)? 2.1f
                    : (grade == Enum.Grade.Myth)? 2.5f
                    : (grade == Enum.Grade.Prime)? 3
                    : 1;

                //* 結果
                for(int i = 0; i < PRICES.Length; i++) 
                    prices[i] = Mathf.RoundToInt(prices[i] * extraPer);

                return prices;
            }
            public static readonly int[] PERS = {
                70, 40, 20, 5, 3
            };
        }
    }
#endregion
}