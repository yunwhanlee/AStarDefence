using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Inventory.UI;

public class GameRewardListUIManager : MonoBehaviour {
    [Header("REWARD LIST POPUP")]
    public GameObject VictoryPopUpObj;
    public Transform Content;

    [Header("REWARD DATA")]
    public InventoryUIItem rwdItemPf;
    [field: SerializeField] public RewardItemSO RwdItemDt {get; private set;}

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
            InventoryUIItem rwdItemUI = Instantiate(rwdItemPf.gameObject, Content).GetComponent<InventoryUIItem>();
            rwdItemUI.SetUIData(rewardItem.Data.Type, rewardItem.Data.Grade, rewardItem.Data.ItemImg, rewardItem.Quantity, lv: 1);
            //* Particle UI Effect 1
            rwdItemUI.PlayScaleUIEF(rwdItemUI, rewardItem.Data.ItemImg);
            rwdItemUI.ItemImgScaleUIEF.startDelay = 0.5f + i * 0.1f;
            //* Particle UI Effect 2
            rwdItemUI.WhiteDimScaleUIEF.lifetime = 0.5f + i * 0.1f;
            rwdItemUI.WhiteDimScaleUIEF.Play();
        }
    }
    public void ShowReward(List<RewardItem> itemList) {
        VictoryPopUpObj.SetActive(true);
        DeleteAll();
        DisplayRewardList(itemList);
    }
#endregion
}
