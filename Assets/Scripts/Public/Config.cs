using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.PixelFantasy.PixelHeroes.Common.Scripts.CollectionScripts;
using UnityEngine;

public static class Config {
#region HOME
    public const int EQUIPITEM_MERGE_CNT = 10;
    public const int MINING_MERGE_CNT = 10;
    public const int MAX_REWARD_SLOT = 40; //* リワードで表示できる最大のスロット数
    // public const int MAX_GOBLINKEY = 5;
    public const int EQUIP_UPGRADE_MAX = 11;
    public const int RELIC_UPGRADE_MAX = 6;
    public const int LUCKYSPIN_FREE_AD_CNT = 3;
    public const int GOLDKEY_FREE_AD_CNT = 2;
    public const int MINING_FREE_AD_CNT = 5;
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
    public const int MONSTER_CNT = 30;
    public const int BOSS_SPAWN_CNT = 8;
    public const int FLIGHT_SPAWN_CNT = 5;

    public const int FREE_BOARD_CNT = 4;
    public const int FREE_BREAKROCK_CNT = 2;

    public const int WALL_SPAWN_MAX = 11; // 壁のMAX数
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

    public const float USER_LV_EXTRA_DMG_PER = 0.04f;

    public class Stage {
        public const int CLEAR_REWARD_FIX_CNT = 4;
        const int STAGE_IDX_MAX = 3;
        public const int STG1_FOREST = 0,
            STG2_DESERT = 1,
            STG3_SEA = 2,
            STG4_UNDEAD = 3,
            STG5_HELL = 4,
            STG_GOBLIN_DUNGEON = 5,
            STG_INFINITE_DUNGEON = 6;
        public static int GetCurStageDtIdx(int stageIdx, int idxNum)
            => stageIdx * STAGE_IDX_MAX + idxNum;
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
        public const int BOSS_KILL_MONEY_BONUS = 40;
    }
    public class H_PRICE {
        public static readonly int[] WORKSPACE_PRICES = {
            0, 15000, 50000
        };

        public readonly struct SHOP {
        #region CHEST PURCHASE
            public const int FREECOMMON = 0, DIAMONDCHEST = 1, COMMON = 2, GOLDCHEST = 3, PREMIUM = 4,
                EQUIP1 = 5, EQUIP5 = 6, EQUIP10 = 7, EQUIP20 = 8, EQUIP40 = 9;

            public static readonly string[] CHEST_PRICES = {
                "Ad_FREE", // FREE COMMON
                "Coin_9500", // DIAMOND CHEST
                "Diamond_40", // COMMON
                "Diamond_250", // GOLD CHEST
                "Diamond_500", // PREMIUM
                "Diamond_30", // EQUIP x 1
                "Diamond_150", // EQUIP x 5
                "Diamond_290", // EQUIP x 10
                "Diamond_570", // EQUIP x 20
                "Diamond_1100", // EQUIP x 40
            };
            public static bool TryPurchaseChest(int chestIdx) {
                //* キーと値段を分ける
                string[] split = CHEST_PRICES[chestIdx].Split("_");
                string key = split[0];
                int price = int.Parse(split[1]);

                //* 購入 処理
                return SetPurchaseData(key, price);
            }
        #endregion
        #region ETC PURCHASE
            public const int GOLDKEY = 0, GOLDKEY_5 = 1, GOLDKEY_10 = 2,
                SOULSTONE = 3, SOULSTONE_5 = 4, SOULSTONE_10 = 5,
                MAGICSTONE = 6, MAGICSTONE_5 = 7, MAGICSTONE_10 = 8,
                RANDOM_CONSUMEITEM = 9, RANDOM_CONSUMEITEM_5 = 10, RANDOM_CONSUMEITEM_10 = 11;

            public static readonly string[] ETC_PRICES = {
                // GOLDKEY
                "Diamond_20", "Diamond_100", "Diamond_180",
                // SOUL STONE
                "Diamond_80", "Diamond_400", "Diamond_750",
                // MAGIC STONE
                "Diamond_80", "Diamond_400", "Diamond_750",
                // RANDOM CONSUME ITEM
                "Coin_500", "Coin_2500", "Coin_5000",
            };

            public static bool TryPurchaseEtc(int etcIdx) {
                //* キーと値段を分ける
                string[] split = ETC_PRICES[etcIdx].Split("_");
                string key = split[0];
                int price = int.Parse(split[1]);

                //* 購入 処理
                return SetPurchaseData(key, price);
            }
        #endregion

        #region COIN PURCHASE
            public const int COIN_TINY = 0, COIN_MEDIUM = 1, COIN_HUGE = 2;
            public static readonly string[] COIN_PRICES = {
                "Diamond_30",
                "Diamond_600",
                "Diamond_2150",
            };

            public static bool TryPurchaseCoinPack(int coinIdx) {
                //* キーと値段を分ける
                string[] split = COIN_PRICES[coinIdx].Split("_");
                string key = split[0];
                int price = int.Parse(split[1]);

                //* 購入 処理
                return SetPurchaseData(key, price);
            }
        #endregion

            private static bool SetPurchaseData(string key, int price) {
                //* 購入 処理
                if(key == "Diamond") {
                    if(HM._.Diamond < price) {
                        HM._.hui.ShowMsgError("다이아가 부족합니다.");
                        return false;
                    }
                    else
                        HM._.Diamond -= price;
                }
                else {
                    if(HM._.Coin < price) {
                        HM._.hui.ShowMsgError("코인이 부족합니다.");
                        return false;
                    }
                    else
                        HM._.Coin -= price;
                }
                return true;
            }
        }

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