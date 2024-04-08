using System;
using System.Collections;
using System.Collections.Generic;
using Inventory.Model;
using UnityEngine;

[Serializable]
public class RewardItem {
    [field: SerializeField] public ItemSO Data {get; private set;}
    [field: SerializeField] public int Quantity {get; set;} = 1;
    [field: SerializeField] public AbilityType[] RelicAbilities {get; set;}

    public RewardItem(ItemSO data, int quantity = 1, AbilityType[] relicAblities = null) {
        Data = data;
        Quantity = quantity;
        RelicAbilities = relicAblities;
    }

    /// <summary>
    /// インベントリへ表示されないアイテム 最新化
    /// </summary>
    public void UpdateItemData(Etc.NoshowInvItem noShowInvItem, int quantity) {
        switch (noShowInvItem) {
            case (int)Etc.NoshowInvItem.Coin:
                HM._.Coin += quantity;
                break;
            case Etc.NoshowInvItem.Diamond:
                HM._.Diamond += quantity;
                break;
            case Etc.NoshowInvItem.Exp:
                HM._.Exp += quantity;
                break;
            case Etc.NoshowInvItem.Goblin0: case Etc.NoshowInvItem.Goblin1: case Etc.NoshowInvItem.Goblin2:
            case Etc.NoshowInvItem.Goblin3: case Etc.NoshowInvItem.Goblin4: case Etc.NoshowInvItem.Goblin5: case Etc.NoshowInvItem.Goblin6:
                int goblinLvIdx = int.Parse($"{noShowInvItem}".Split("n")[1]);
                HM._.mnm.GoblinCards[goblinLvIdx].Cnt += quantity;
                break;
            case Etc.NoshowInvItem.Ore0: case Etc.NoshowInvItem.Ore1: case Etc.NoshowInvItem.Ore2:
            case Etc.NoshowInvItem.Ore3: case Etc.NoshowInvItem.Ore4: case Etc.NoshowInvItem.Ore5:
            case Etc.NoshowInvItem.Ore6: case Etc.NoshowInvItem.Ore7: case Etc.NoshowInvItem.Ore8:
                int oreLvIdx = int.Parse($"{noShowInvItem}".Split("e")[1]);
                HM._.mnm.OreCards[oreLvIdx].Cnt += quantity;
                break;
            case Etc.NoshowInvItem.SkillPoint:
                HM._.SkillPoint += quantity;
                break;

        }
    }
}

public class HomeRewardUIManager : MonoBehaviour {
    [Header("ItemSO LIST")]
    [SerializeField] ItemSO[] EtcConsumableDatas;
    [SerializeField] ItemSO[] EtcNoShowInvDatas;
    [SerializeField] ItemSO[] weaponDatas;
    [SerializeField] ItemSO[] shoesDatas;
    [SerializeField] ItemSO[] ringDatas;
    [SerializeField] ItemSO[] RelicDatas;

    // [SerializeField] List<RewardItem> RewardList;

    void Update() {
        if(Input.GetKeyDown(KeyCode.A)) {
            Debug.Log("REWARD TEST");

            //* リワードリスト (最大８個)
            var rewardList = new List<RewardItem> {
                //* インベントリー以外で扱うアイテム
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
                new (EtcConsumableDatas[(int)Etc.ConsumableItem.BizzardScroll]),        // Info(Ingame)
                new (EtcConsumableDatas[(int)Etc.ConsumableItem.ChestCommon]),          // Chest Open
                new (EtcConsumableDatas[(int)Etc.ConsumableItem.ChestDiamond]),         // Chest Open
                new (EtcConsumableDatas[(int)Etc.ConsumableItem.ChestEquipment]),       // Chest Open
                new (EtcConsumableDatas[(int)Etc.ConsumableItem.ChestGoldKey]),         // Chest Open
                new (EtcConsumableDatas[(int)Etc.ConsumableItem.ChestPremium]),         // Chest Open
                new (EtcConsumableDatas[(int)Etc.ConsumableItem.Clover]),               // Active
                new (EtcConsumableDatas[(int)Etc.ConsumableItem.GoldClover]),           // Active
                // new (EtcConsumableDatas[(int)Etc.ConsumableItem.GoldKey]),           // Move Goblin Dungeon PopUp
                // new (EtcConsumableDatas[(int)Etc.ConsumableItem.LightningScroll]),   // Info(Ingame)
                // new (EtcConsumableDatas[(int)Etc.ConsumableItem.MagicStone]),        // Reset Relic Abilities
                // new (EtcConsumableDatas[(int)Etc.ConsumableItem.Present0]),          // Reward Open
                // new (EtcConsumableDatas[(int)Etc.ConsumableItem.Present1]),          // Reward Open
                // new (EtcConsumableDatas[(int)Etc.ConsumableItem.Present2]),          // Reward Open
                // new (EtcConsumableDatas[(int)Etc.ConsumableItem.SoulStone]),         // Equipment Secret Ability Active
                // new (EtcConsumableDatas[(int)Etc.ConsumableItem.SteamPack0]),        // Info(Ingame)
                // new (EtcConsumableDatas[(int)Etc.ConsumableItem.SteamPack1]),        // Info(Ingame)

                // new (weaponDatas[2]),
                // new (shoesDatas[1]),
            };
            HM._.rwlm.ShowReward(rewardList);
            UpdateInventory(rewardList);
        }
    }

    #region FUNC
        public void UpdateInventory(List<RewardItem> rewardList) {
            if(rewardList.Count > 0) {
                foreach (RewardItem rwdItem in rewardList) {
                    //* リワード処理：インベントリーへ表示しないアイテム
                    if(rwdItem.Data.IsNoshowInventory) {
                        Etc.NoshowInvItem enumVal = Util.FindEnumVal(rwdItem.Data.name);
                        rwdItem.UpdateItemData(enumVal, rwdItem.Quantity);
                    }
                    //* リワード処理：インベントリーへ表示する物
                    else {
                        int reminder = HM._.ivCtrl.InventoryData.AddItem(rwdItem.Data, rwdItem.Quantity, lv: 1, rwdItem.RelicAbilities);
                        rwdItem.Quantity = reminder;
                        //TODO インベントリ表示するアイテム処理
                    }
                }
            }
        }
    #endregion
}
