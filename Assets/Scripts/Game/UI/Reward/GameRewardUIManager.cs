using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRewardUIManager : MonoBehaviour {
    void Update() {
        if(Input.GetKeyDown(KeyCode.A)) {
            Debug.Log("REWARD TEST");
            //* リワード
            var rewardList = new List<RewardItem> {
                new (HM._.rwlm.RwdItemDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Exp], 500),
                new (HM._.rwlm.RwdItemDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Coin], 1000),
                new (HM._.rwlm.RwdItemDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Ore0]),
                new (HM._.rwlm.RwdItemDt.EtcConsumableDatas[(int)Etc.ConsumableItem.ChestCommon]),
            };
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
