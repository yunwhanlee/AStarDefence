using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRewardUIManager : MonoBehaviour {
    void Update() {
        if(Input.GetKeyDown(KeyCode.A)) {
            Debug.Log("VICTORY REWARD TEST");
            //* リワード
            var rewardList = new List<RewardItem> {
                new (HM._.rwlm.RwdItemDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Exp], 500),
                new (HM._.rwlm.RwdItemDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Coin], 1000),
                new (HM._.rwlm.RwdItemDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Ore0]),
                new (HM._.rwlm.RwdItemDt.EtcConsumableDatas[(int)Etc.ConsumableItem.ChestCommon]),
            };

            //* 追加コイン＆EXP 適用
            rewardList.ForEach(rwdItem => {
                Etc.NoshowInvItem enumVal = Util.FindEnumVal(rwdItem.Data.name);
                if(enumVal == Etc.NoshowInvItem.Coin && DM._.DB.EquipDB.ClearCoinPer > 0)
                    rwdItem.Quantity = (int)(rwdItem.Quantity * (1 + DM._.DB.EquipDB.ClearCoinPer));
                if(enumVal == Etc.NoshowInvItem.Exp) {
                    float bonusExpPer = 0;
                    //* ボーナスEXP％ 計算
                    if(DM._.DB.EquipDB.ClearExpPer > 0)
                        bonusExpPer += DM._.DB.EquipDB.ClearExpPer;
                    if(DM._.DB.IsCloverActive)
                        bonusExpPer += Config.CLOVER_BONUS_EXP_PER;
                    if(DM._.DB.IsGoldCloverActive)
                        bonusExpPer += Config.GOLDCLOVER_BONUS_EXP_PER;

                    //* 適用
                    if(bonusExpPer > 0)
                        rwdItem.Quantity += Mathf.RoundToInt(rwdItem.Quantity * bonusExpPer);
                }
            });

            GM._.rwlm.ShowReward(rewardList);
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
                    int reminder = HM._.ivCtrl.InventoryData.AddItem (
                        rwdItem.Data, 
                        rwdItem.Quantity, 
                        lv: 1, 
                        rwdItem.RelicAbilities
                    );
                    rwdItem.Quantity = reminder;
                }
            }
        }
    }
#endregion
}
