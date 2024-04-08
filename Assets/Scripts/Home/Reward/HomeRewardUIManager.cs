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
                // new (EtcNoShowInvDatas[0]),
                // new (EtcNoShowInvDatas[1]),
                // new (EtcNoShowInvDatas[2]),
                // new (EtcNoShowInvDatas[3]),
                // new (EtcNoShowInvDatas[4]),
                // new (EtcNoShowInvDatas[5]),
                // new (EtcNoShowInvDatas[6]),
                // new (EtcNoShowInvDatas[7]),
                // new (EtcNoShowInvDatas[8]),
                // new (EtcNoShowInvDatas[9]),
                // new (EtcNoShowInvDatas[10]),
                // new (EtcNoShowInvDatas[11]),
                // new (EtcNoShowInvDatas[12]),
                // new (EtcNoShowInvDatas[13]),
                // new (EtcNoShowInvDatas[14]),
                // new (EtcNoShowInvDatas[15]),
                // new (EtcNoShowInvDatas[16]),
                // new (EtcNoShowInvDatas[17]),
                // new (EtcNoShowInvDatas[18]),
                // new (EtcNoShowInvDatas[19]),

                // new (EtcConsumableDatas[0]),
                // new (EtcConsumableDatas[1]),
                // new (EtcConsumableDatas[2]),
                // new (EtcConsumableDatas[3]),
                // new (EtcConsumableDatas[4]),
                // new (EtcConsumableDatas[5]),
                // new (EtcConsumableDatas[6]),
                new (EtcConsumableDatas[7]),
                new (EtcConsumableDatas[8]),
                new (EtcConsumableDatas[9]),
                new (EtcConsumableDatas[10]),
                new (EtcConsumableDatas[11]),
                new (EtcConsumableDatas[12]),

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
