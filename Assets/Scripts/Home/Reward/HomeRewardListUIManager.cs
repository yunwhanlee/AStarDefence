using System.Collections;
using System.Collections.Generic;
using Inventory.UI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class HomeRewardListUIManager : MonoBehaviour {
    [Header("REWARD LIST POPUP")]
    public GameObject WindowObj;
    public Transform Content;

    [Header("REWARD LIST POPUP")]
    public Action OnClickOpenChest = () => {};
    public Sprite[] TitleRibbonSprs;
    public GameObject RewardChestPopUp;
    public TMP_Text ChestTitleTxt;
    public TMP_Text ChestAlertCntTxt;
    public Image ChestTitleRibbonImg;
    public Image ChestImg;


    [Header("REWARD DATA")]
    public InventoryUIItem rwdItemPf;
    [field: SerializeField] public RewardItemSO RwdItemDt {get; private set;}

#region EVENT
    public void OnClickOpenChestImgBtn() => OnClickOpenChest?.Invoke();
#endregion

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
            InventoryUIItem rwdItemUI = Instantiate(rwdItemPf.gameObject, HM._.rwlm.Content).GetComponent<InventoryUIItem>();
            rwdItemUI.SetUIData(rewardItem.Data.Type, rewardItem.Data.Grade, rewardItem.Data.ItemImg, rewardItem.Quantity, lv: 1);
            //* Particle UI Effect
            rwdItemUI.ItemImgScaleUIEF.sprite = rewardItem.Data.ItemImg;
            rwdItemUI.ItemImgScaleUIEF.startDelay = 0.5f + i * 0.1f;
            rwdItemUI.ItemImgScaleUIEF.Play();
            rwdItemUI.WhiteDimScaleUIEF.lifetime = 0.5f + i * 0.1f;
            rwdItemUI.WhiteDimScaleUIEF.Play();

        }
    }
    public void ShowReward(List<RewardItem> itemList) {
        WindowObj.SetActive(true);
        DeleteAll();
        DisplayRewardList(itemList);
    }
    /// <summary>
    /// Chestを開いた後、カウント減った状況 最新化
    /// </summary>
    public void UpdateChestPopUpUI() {
        var ivm = HM._.ivm;
        ivm.CurInvItem = ivm.GetCurItemUIFromIdx(ivm.CurItemIdx);
        ChestAlertCntTxt.text = $"{ivm.CurInvItem.Quantity}";
    }
    private void SetChestPopUpUI(Etc.ConsumableItem enumChestIdx, int quantity) {
        const int OFFSET = (int)Etc.ConsumableItem.ChestCommon;
        RewardChestPopUp.SetActive(true);
        ChestTitleRibbonImg.sprite = TitleRibbonSprs[(int)enumChestIdx - OFFSET];
        ChestImg.sprite = RwdItemDt.EtcConsumableDatas[(int)enumChestIdx].ItemImg;
        ChestTitleTxt.text = RwdItemDt.EtcConsumableDatas[(int)enumChestIdx].Name;
        ChestAlertCntTxt.text = quantity.ToString();
    }
    /// <summary>
    /// ChestをTapして開くPopUp 表示
    /// </summary>  
    public void ShowChestPopUp(Etc.ConsumableItem type, int quantity) {
        RewardContentSO chestDt = (type == Etc.ConsumableItem.ChestCommon)? RwdItemDt.Rwd_ChestCommon
            : (type == Etc.ConsumableItem.ChestDiamond)? RwdItemDt.Rwd_ChestDiamond
            : (type == Etc.ConsumableItem.ChestEquipment)? RwdItemDt.Rwd_ChestEquipment
            : (type == Etc.ConsumableItem.ChestGold)? RwdItemDt.Rwd_ChestGold
            : RwdItemDt.Rwd_ChestPremium;
        SetChestPopUpUI(type, quantity);

        //* 次の開くイベント登録
        OnClickOpenChest = () => RwdItemDt.OpenRewardContent(chestDt);
    }
#endregion
}
