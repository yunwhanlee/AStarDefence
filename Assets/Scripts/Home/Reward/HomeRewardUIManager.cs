using System;
using System.Collections;
using System.Collections.Generic;
using Inventory.Model;
using UnityEngine;

public class HomeRewardUIManager : MonoBehaviour {
    void Update() {
        if(Input.GetKeyDown(KeyCode.A)) {
            Debug.Log("REWARD TEST");

            //* リワードリスト (最大８個)
            var rewardList = new List<RewardItem> {
                //* インベントリー以外で扱うアイテム
                // new (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.GoldKey], 1),
                // new (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Coin], 1000),
                // new (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Diamond], 500),
                // new (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Exp], 500),
                // new (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Goblin0]),
                // new (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Goblin1]),
                // new (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Goblin2]),
                // new (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Goblin3]),
                // new (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Goblin4]),
                // new (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Goblin5]),
                // new (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Goblin6]),
                // new (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Ore0]),
                // new (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Ore1]),
                // new (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Ore2]),
                // new (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Ore3]),
                // new (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Ore4]),
                // new (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Ore5]),
                // new (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Ore6]),
                // new (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Ore7]),
                // new (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Ore8]),
                // new (EtcNoShowInvDatas[(int)Etc.NoshowInvItem.SkillPoint]),

                //* インベントリーへ表示するアイテム
                // new (EtcConsumableDatas[(int)Etc.ConsumableItem.BizzardScroll]),        // ✓ Info(Ingame)
                // new (EtcConsumableDatas[(int)Etc.ConsumableItem.LightningScroll]),   // ✓ Info(Ingame)
                // new (EtcConsumableDatas[(int)Etc.ConsumableItem.SteamPack0]),        // ✓ Info(Ingame)
                // new (EtcConsumableDatas[(int)Etc.ConsumableItem.SteamPack1]),        // ✓ Info(Ingame)
                // new (EtcConsumableDatas[(int)Etc.ConsumableItem.MagicStone]),        // ✓ Info (Reset Relic Abilities)
                // new (EtcConsumableDatas[(int)Etc.ConsumableItem.SoulStone]),         // ✓ Info (Equipment Secret Ability Active)
                // new (HM._.rwlm.RwdItemDt.EtcConsumableDatas[(int)Etc.ConsumableItem.ChestCommon]),          // Chest Open
                // new (HM._.rwlm.RwdItemDt.EtcConsumableDatas[(int)Etc.ConsumableItem.ChestDiamond]),         // Chest Open
                // new (HM._.rwlm.RwdItemDt.EtcConsumableDatas[(int)Etc.ConsumableItem.ChestEquipment]),       // Chest Open
                // new (HM._.rwlm.RwdItemDt.EtcConsumableDatas[(int)Etc.ConsumableItem.ChestGold]),         // Chest Open
                // new (HM._.rwlm.RwdItemDt.EtcConsumableDatas[(int)Etc.ConsumableItem.ChestPremium]),         // Chest Open
                // new (EtcConsumableDatas[(int)Etc.ConsumableItem.Clover]),               // ✓ Active
                // new (EtcConsumableDatas[(int)Etc.ConsumableItem.GoldClover]),           // ✓ Active
                // new (HM._.rwlm.RwdItemDt.EtcConsumableDatas[(int)Etc.ConsumableItem.Present0],4),          // Reward Open
                // new (HM._.rwlm.RwdItemDt.EtcConsumableDatas[(int)Etc.ConsumableItem.Present1],5),          // Reward Open
                // new (HM._.rwlm.RwdItemDt.EtcConsumableDatas[(int)Etc.ConsumableItem.Present2],6),          // Reward Open

                // new (HM._.rwlm.RwdItemDt.WeaponDatas[5]),
                // new (shoesDatas[3]),
                new (HM._.rwlm.RwdItemDt.WeaponDatas[0], quantity: 8),
                new (HM._.rwlm.RwdItemDt.WeaponDatas[2], quantity: 10),
                new (HM._.rwlm.RwdItemDt.WeaponDatas[3], quantity: 9),
                // new (HM._.rwlm.RwdItemDt.RelicDatas[0], 1, HM._.ivCtrl.InventoryData.CheckRelicAbilitiesData(HM._.rwlm.RwdItemDt.RelicDatas[0])),

            };
            HM._.rwlm.ShowReward(rewardList);
            // CoUpdateInventoryAsync(rewardList);
        }
    }

    #region FUNC
        public IEnumerator CoUpdateInventoryAsync(List<RewardItem> rewardList) {
            if(rewardList.Count > 0) {
                foreach (RewardItem rwdItem in rewardList) {
                    //* リワード処理：インベントリーへ表示しないアイテム
                    if(rwdItem.Data.IsNoshowInventory) {
                        Etc.NoshowInvItem enumVal = Util.FindEnumVal(rwdItem.Data.name);
                        rwdItem.UpdateNoShowItemData(enumVal, rwdItem.Quantity);
                    }
                    //* リワード処理：インベントリーへ表示する物
                    else {
                        Debug.Log($"<color=green>CoUpdateInventoryAsync():: AddItem() -> {rwdItem.Data.name}</color>");
                        int reminder = HM._.ivCtrl.InventoryData.AddItem (
                            rwdItem.Data, 
                            rwdItem.Quantity, 
                            lv: 1, 
                            rwdItem.RelicAbilities,
                            isEquip: false,
                            isNewAlert: true
                        );
                        rwdItem.Quantity = reminder;
                    }
                    yield return null;
                }
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
