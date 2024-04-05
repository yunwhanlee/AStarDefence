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

    private void SetRewardItemList(List<RewardItem> itemList) {
        //* リワードリストへオブジェクト生成・追加
        for(int i = 0; i < itemList.Count; i++) {
            RewardItem rwdItem = itemList[i];
            InventoryUIItem rwdUIItem = Instantiate(rwdItemPf.gameObject, HM._.rwlm.Content).GetComponent<InventoryUIItem>();
            rwdUIItem.SetData(rwdItem.Data.Type, rwdItem.Data.Grade, rwdItem.Data.ItemImg, rwdItem.Val, lv: 1);
        }
    }

    public void ShowReward(List<RewardItem> itemList) {
        WindowObj.SetActive(true);
        DeleteAll();
        SetRewardItemList(itemList);
    }
#endregion
}
