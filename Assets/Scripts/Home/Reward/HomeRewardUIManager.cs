using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Inventory.Model;
using UnityEngine;

public class HomeRewardUIManager : MonoBehaviour {
    // void Update() {
    //     if(Input.GetKeyDown(KeyCode.A)) {
    //         Debug.Log("REWARD TEST");

    //         //* リワードリスト (最大８個)
    //         var rewardList = new List<RewardItem> {
    //             //* インベントリー以外で扱うアイテム
    //             // new (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.GoldKey], 1),
    //             // new (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Coin], 1000),
    //             // new (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Diamond], 500),
    //             // new (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Exp], 500),
    //             // new (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Goblin0]),
    //             // new (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Goblin1]),
    //             // new (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Goblin2]),
    //             // new (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Goblin3]),
    //             // new (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Goblin4]),
    //             // new (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Goblin5]),
    //             // new (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Goblin6]),
    //             // new (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Ore0]),
    //             // new (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Ore1]),
    //             // new (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Ore2]),
    //             // new (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Ore3]),
    //             // new (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Ore4]),
    //             // new (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Ore5]),
    //             // new (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Ore6]),
    //             // new (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Ore7]),
    //             // new (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Ore8]),
    //             // new (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.SkillPoint]),

    //             //* インベントリーへ表示するアイテム
    //             // new (EtcConsumableDatas[(int)Etc.ConsumableItem.BizzardScroll]),        // ✓ Info(Ingame)
    //             // new (EtcConsumableDatas[(int)Etc.ConsumableItem.LightningScroll]),   // ✓ Info(Ingame)
    //             // new (EtcConsumableDatas[(int)Etc.ConsumableItem.SteamPack0]),        // ✓ Info(Ingame)
    //             // new (EtcConsumableDatas[(int)Etc.ConsumableItem.SteamPack1]),        // ✓ Info(Ingame)
    //             // new (EtcConsumableDatas[(int)Etc.ConsumableItem.MagicStone]),        // ✓ Info (Reset Relic Abilities)
    //             // new (EtcConsumableDatas[(int)Etc.ConsumableItem.SoulStone]),         // ✓ Info (Equipment Secret Ability Active)
    //             // new (HM._.rwlm.RwdItemDt.EtcConsumableDatas[(int)Etc.ConsumableItem.ChestCommon]),          // Chest Open
    //             // new (HM._.rwlm.RwdItemDt.EtcConsumableDatas[(int)Etc.ConsumableItem.ChestDiamond]),         // Chest Open
    //             // new (HM._.rwlm.RwdItemDt.EtcConsumableDatas[(int)Etc.ConsumableItem.ChestEquipment]),       // Chest Open
    //             // new (HM._.rwlm.RwdItemDt.EtcConsumableDatas[(int)Etc.ConsumableItem.ChestGold]),         // Chest Open
    //             // new (HM._.rwlm.RwdItemDt.EtcConsumableDatas[(int)Etc.ConsumableItem.ChestPremium]),         // Chest Open
    //             // new (EtcConsumableDatas[(int)Etc.ConsumableItem.Clover]),               // ✓ Active
    //             // new (EtcConsumableDatas[(int)Etc.ConsumableItem.GoldClover]),           // ✓ Active
    //             // new (HM._.rwlm.RwdItemDt.EtcConsumableDatas[(int)Etc.ConsumableItem.Present0],4),          // Reward Open
    //             // new (HM._.rwlm.RwdItemDt.EtcConsumableDatas[(int)Etc.ConsumableItem.Present1],5),          // Reward Open
    //             // new (HM._.rwlm.RwdItemDt.EtcConsumableDatas[(int)Etc.ConsumableItem.Present2],6),          // Reward Open

    //             // new (HM._.rwlm.RwdItemDt.WeaponDatas[0], 10),
    //             // new (HM._.rwlm.RwdItemDt.WeaponDatas[1], 9),
    //             // new (HM._.rwlm.RwdItemDt.WeaponDatas[2], 9),
    //             // new (HM._.rwlm.RwdItemDt.WeaponDatas[3], 9),
    //             // new (HM._.rwlm.RwdItemDt.WeaponDatas[4], 9),
    //             // new (HM._.rwlm.RwdItemDt.WeaponDatas[5], 9),
    //             // new (HM._.rwlm.RwdItemDt.WeaponDatas[6], 9),
    //             // new (HM._.rwlm.RwdItemDt.ShoesDatas[6]),
    //             // new (HM._.rwlm.RwdItemDt.RingDatas[3], quantity: 10),
    //             // new (HM._.rwlm.RwdItemDt.RingDatas[4], quantity: 9),
    //             // new (HM._.rwlm.RwdItemDt.WeaponDatas[3], quantity: 9),
    //             new (HM._.rwlm.RwdItemDt.RelicDatas[0], 14, HM._.ivCtrl.InventoryData.CheckRelicAbilitiesData(HM._.rwlm.RwdItemDt.RelicDatas[0])),

    //         };
    //         HM._.rwlm.ShowReward(rewardList);
    //         // CoUpdateInventoryAsync(rewardList);
    //     }
    // }

    #region FUNC
        /// <summary>
        /// リワードアイテムデータ アップデート
        /// </summary>
        public IEnumerator CoUpdateInventoryAsync(List<RewardItem> rewardList) {
            if(rewardList.Count > 0) {
                foreach (RewardItem rwdItem in rewardList) {
                    //* 表示しないアイテムデータ 項目
                    if(rwdItem.Data.IsNoshowInventory) {
                        Etc.NoshowInvItem enumVal = Util.FindEnumVal(rwdItem.Data.name);
                        rwdItem.UpdateNoShowItemData(enumVal, rwdItem.Quantity);
                    }
                    //* 表示するアイテムデータ 項目
                    else {
                        Debug.Log($"<color=green>CoUpdateInventoryAsync():: AddItem() -> {rwdItem.Data.name}</color>");

                        try {
                            //* 인벤토리 아이템 레벨 유지
                            var invItem = HM._.ivCtrl.InventoryData.InvArr[rwdItem.Data.ID];
                            Debug.Log($"CoUpdateInventoryAsync():: invItem.Data.Name= {invItem.Data.Name}, Lv= {invItem.Lv}, relicAbilities= {invItem.RelicAbilities}");

                            //! 인벤토리 장비 레벨0인 경우 버그 방지
                            int itemLv = ((invItem.Data.Type == Enum.ItemType.Weapon
                                        || invItem.Data.Type == Enum.ItemType.Shoes
                                        || invItem.Data.Type == Enum.ItemType.Ring
                                        || invItem.Data.Type == Enum.ItemType.Relic
                                        ) && invItem.Lv == 0)? 1 : invItem.Lv;

                            int reminder = HM._.ivCtrl.InventoryData.AddItem (
                                rwdItem.Data, 
                                rwdItem.Quantity,
                                lv: itemLv,
                                // 새롭게 추가되는거면 유물능력이 0임으로, rwdItem으로 새롭게 능력을 적용히고 그게 아니라면 이전 유물능력 유지.
                                invItem.RelicAbilities.Count() == 0? rwdItem.RelicAbilities : invItem.RelicAbilities, 
                                isEquip: false,
                                isNewAlert: true
                            );
                            rwdItem.Quantity = reminder;
                        }
                        catch(Exception e) {
                            Debug.LogError("error= " + e);
                        }
                    }

                    yield return null; // 1FRAME 待機
                }

                // インベントリーUIスロット 最新化
                HM._.ivCtrl.OnInventoryUIUpdated?.Invoke();
            }
        }

        public List<RewardItem> BuildVictoryRewardList(int exp, int coin, Etc.NoshowInvItem oreIdx, int oreCnt, int fame) {
            Debug.Log($"BuildVictoryRewardList():: HM._.rwlm.RwdItemDt= {HM._.rwlm.RwdItemDt}");
            RewardItemSO rwDt = HM._.rwlm.RwdItemDt;
            return new List<RewardItem>() {
                new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Exp], exp),
                new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Coin], coin),
                new (rwDt.EtcNoShowInvDatas[(int)oreIdx], oreCnt),
                new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Fame], fame)
            };
        }
    #endregion
}
