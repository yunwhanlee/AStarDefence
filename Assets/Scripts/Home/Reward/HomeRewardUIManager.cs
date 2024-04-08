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

            //* リワードリスト
            var rewardList = new List<RewardItem> {
                new (EtcConsumableDatas[0]),
                new (EtcConsumableDatas[2]),
                new (EtcConsumableDatas[7]),
                new (EtcNoShowInvDatas[1]),
                new (EtcNoShowInvDatas[6]),
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
                foreach (RewardItem item in rewardList) {
                    int reminder = HM._.ivCtrl.InventoryData.AddItem(item.Data, item.Quantity, lv: 1, item.RelicAbilities);
                    item.Quantity = reminder;
                }
            }
        }
    #endregion
}
