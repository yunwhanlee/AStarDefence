using System;
using System.Collections;
using System.Collections.Generic;
using Inventory.Model;
using UnityEngine;

[Serializable]
public class RewardItem {
    [field: SerializeField] public ItemSO Data {get; private set;}
    [field: SerializeField] public int Val {get; set;} = 1;
    [field: SerializeField] public Ability[] Abilities {get; set;}
}

public class HomeRewardUIManager : MonoBehaviour {
    public List<RewardItem> RewardList;

    void Update() {
        if(Input.GetKeyDown(KeyCode.A)) {
            Debug.Log("REWARD TEST");
            HM._.rwlm.ShowReward(RewardList);
            UpdateInventory();
        }
    }

    #region FUNC
        public void UpdateInventory() {
            if(RewardList.Count > 0) {
                foreach (RewardItem item in RewardList) {
                    int reminder = HM._.ivCtrl.InventoryData.AddItem(item.Data, item.Val, lv: 1, item.Abilities);
                    item.Val = reminder;
                }
            }
        }
    #endregion
}
