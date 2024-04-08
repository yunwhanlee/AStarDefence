using System.Collections;
using System.Collections.Generic;
using Inventory.UI;
using UnityEngine;

public class HomeRewardListUIManager : MonoBehaviour {
    public GameObject WindowObj;
    public Transform Content;
    public InventoryUIItem rwdItemPf;

#region FUNC
    private void DeleteAll() {
        foreach (Transform child in Content)
            Destroy(child.gameObject);
    }

    /// <summary>
    /// リワードリスト表示
    /// </summary>
    private void DisplayRewardList(List<RewardItem> rewardList) {
        //* リワードリストへオブジェクト生成・追加
        for(int i = 0; i < rewardList.Count; i++) {
            RewardItem rewardItem = rewardList[i];
            InventoryUIItem rewardItemUI = Instantiate(rwdItemPf.gameObject, HM._.rwlm.Content).GetComponent<InventoryUIItem>();
            rewardItemUI.SetData(rewardItem.Data.Type, rewardItem.Data.Grade, rewardItem.Data.ItemImg, rewardItem.Quantity, lv: 1);
        }
    }

    public void ShowReward(List<RewardItem> itemList) {
        WindowObj.SetActive(true);
        DeleteAll();
        DisplayRewardList(itemList);
    }
#endregion
}
