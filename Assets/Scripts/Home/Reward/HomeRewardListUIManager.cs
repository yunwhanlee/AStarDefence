using System.Collections;
using System.Collections.Generic;
using Inventory.UI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using AssetKits.ParticleImage;

public class HomeRewardListUIManager : MonoBehaviour {
    [Header("REWARD LIST POPUP")]
    public GameObject WindowObj;
    public Transform Content;
    public bool IsFinishSlotsSpawn = false;

    [Header("REWARD CHEST POPUP")]
    public Sprite[] TitleRibbonSprs;
    public GameObject RewardChestPopUp;
    public TMP_Text ChestTitleTxt;
    public TMP_Text ChestAlertCntTxt;
    public Image ChestTitleRibbonImg;
    public Image ChestImg;

    public Action OnClickOpenChest = () => {};

    [Header("REWARD DATA")]
    public InventoryUIItem rwdItemPf;
    [field: SerializeField] public RewardItemSO RwdItemDt {get; private set;}

#region EVENT
    public void OnClickOpenChestImgBtn() => OnClickOpenChest?.Invoke();
    public void OnClickCloseDimBtn() {
        //* リワードスロットのアニメーションが全部終わるまで待つ
        if(IsFinishSlotsSpawn)
            return;

        SM._.SfxPlay(SM.SFX.ClickSFX);
        HM._.lvm.CheckLevelUp();
        WindowObj.SetActive(false);
    }
#endregion

#region FUNC
    private void DeleteAll() {
        foreach (Transform child in Content)
            Destroy(child.gameObject);
    }

    IEnumerator CoPlayRewardSlotSpawnSFX(int cnt) {
        IsFinishSlotsSpawn = true;
        yield return Util.Time0_5;
        for(int i = 0; i < cnt; i++) {
            SM._.SfxPlay(SM.SFX.InvUnEquipSFX);
            yield return Util.Time0_1;
        }
        IsFinishSlotsSpawn = false;
    }

    /// <summary>
    /// リワードリスト表示
    /// </summary>
    private void DisplayRewardList(List<RewardItem> rewardList) {
        StartCoroutine(CoPlayRewardSlotSpawnSFX(rewardList.Count));
        //* リワードリストへオブジェクト生成・追加
        for(int i = 0; i < rewardList.Count; i++) {
            RewardItem rewardItem = rewardList[i];
            InventoryUIItem rwdItemUI = Instantiate(rwdItemPf.gameObject, Content).GetComponent<InventoryUIItem>();
            rwdItemUI.SetUI(rewardItem.Data.Type, rewardItem.Data.Grade, rewardItem.Data.ItemImg, rewardItem.Quantity, lv: 1);
            // rwdItemUI.IsNewAlert = true;
            //* Particle UI Effect 1
            rwdItemUI.PlayScaleUIEF(rwdItemUI, rewardItem.Data.ItemImg);
            rwdItemUI.ItemImgScaleUIEF.startDelay = 0.5f + i * 0.1f;
            //* Particle UI Effect 2
            rwdItemUI.WhiteDimScaleUIEF.lifetime = 0.5f + i * 0.1f;
            rwdItemUI.WhiteDimScaleUIEF.Play();
            //* Particle UI Equipment High Grade Effect 3
            if(rewardItem.Data.Grade >= Enum.Grade.Unique) {
                SM._.SfxPlay(SM.SFX.Merge3SFX, 0.5f + i * 0.1f);
                rwdItemUI.HighGradeSpawnUIEF.Play();

                //* High Grade Nice Effect 4
                rwdItemUI.HighGradeNiceUIEF.startDelay = 0.5f + i * 0.1f;
                rwdItemUI.HighGradeNiceUIEF.GetComponentsInChildren<ParticleImage>()[1].startDelay = 0.5f + i * 0.1f;
                rwdItemUI.HighGradeNiceUIEF.GetComponentsInChildren<ParticleImage>()[2].startDelay = 0.5f + i * 0.1f;
                rwdItemUI.HighGradeNiceUIEF.Play();
            }
        }
    }
    public void ShowReward(List<RewardItem> itemList) {
        SM._.SfxPlay(SM.SFX.RewardSFX);
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
        SM._.SfxPlay(SM.SFX.CreateTowerSFX);
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
