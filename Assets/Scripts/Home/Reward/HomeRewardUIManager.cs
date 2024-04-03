using System.Collections;
using System.Collections.Generic;
using Inventory.Model;
using UnityEngine;

public class HomeRewardUIManager : MonoBehaviour {
    public List<RewardItem> RewardList;
    [SerializeField] private InventorySO InventoryData;



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
                    int reminder = InventoryData.AddItem(item.InventoryItem, item.Val);
                    item.Val = reminder;
                }
            }
        }
    #endregion
}
