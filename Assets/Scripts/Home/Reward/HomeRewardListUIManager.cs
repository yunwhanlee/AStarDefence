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
    public GridLayoutGroup ContentGrid;
    public List<InventoryUIItem> rwdSlotList;

    public bool IsFinishSlotsSpawn = false;

    [Header("REWARD CHEST POPUP")]
    public Sprite[] TitleRibbonSprs;
    public GameObject RewardChestPopUp;
    public TMP_Text ChestTitleTxt;
    public TMP_Text ChestAlertCntTxt;
    public Image ChestTitleRibbonImg;
    public Image ChestImg;
    public Action OnClickOpenChest = () => {};

    [field: Header("REWARD DATA")]
    // public InventoryUIItem rwdItemPf;
    [field: SerializeField] public RewardItemSO RwdItemDt {get; private set;}

    void Start() {
        ContentGrid = Content.GetComponent<GridLayoutGroup>();

        //* 初期化 スロットリスト
        for(int i = 0; i < Content.childCount; i++) {
            rwdSlotList.Add(Content.GetChild(i).GetComponent<InventoryUIItem>());
            rwdSlotList[i].gameObject.SetActive(false);
        }
    }

#region EVENT
    public void OnClickOpenChestImgBtn() => OnClickOpenChest?.Invoke();
    public void OnClickCloseScreenBtn() {
        //* リワードスロットのアニメーションが全部終わるまで待つ
        if(IsFinishSlotsSpawn)
            return;
        
        if(HM._.shopMg.OnClickEquipPackage != null) {
            HM._.shopMg.OnClickEquipPackage?.Invoke();
            return;
        }

        //* LuckySpinをしたら、リセット処理をする
        if(HM._.lspm.OnClickCloseRewardScreen != null)
            HM._.lspm.OnClickCloseRewardScreen?.Invoke();

        HM._.hui.IsActivePopUp = false;
        SM._.SfxPlay(SM.SFX.ClickSFX);
        HM._.lvm.CheckLevelUp();
        WindowObj.SetActive(false);
    }
#endregion

#region FUNC
    private void ReleaseAll() {
        rwdSlotList.ForEach(rwdSlot => {
            rwdSlot.ResetUI();
            rwdSlot.gameObject.SetActive(false);
        });
        // foreach (Transform child in Content)
        //     child.gameObject.SetActive(false); // Destroy(child.gameObject);
    }

    IEnumerator CoPlayRewardSlotSpawnSFX(int cnt) {
        IsFinishSlotsSpawn = true;
        yield return Util.Time0_5;
        for(int i = 0; i < cnt; i++) {
            SM._.SfxPlay(SM.SFX.ItemPickSFX);
            yield return Util.Time0_1;
        }
        IsFinishSlotsSpawn = false;
    }

    /// <summary>
    /// リワードリスト表示
    /// </summary>
    private void DisplayRewardList(List<RewardItem> rewardList) {
        //* 数量によって、スロットサイズ
        bool isUnder10 = rewardList.Count <= 10;
        ContentGrid.constraintCount = isUnder10? 5 : 10;
        Content.localScale = Vector3.one * (isUnder10? 1 : 0.8f);

        //* サウンド
        StartCoroutine(CoPlayRewardSlotSpawnSFX(rewardList.Count));

        //* リワードリストへオブジェクト生成・追加
        for(int i = 0; i < rewardList.Count; i++) {
            float delay = 0.5f + i * 0.1f;
            RewardItem rewardItem = rewardList[i];
            rwdSlotList[i].gameObject.SetActive(true);
            rwdSlotList[i].SetUI(rewardItem.Data.Type, rewardItem.Data.Grade, rewardItem.Data.ItemImg, rewardItem.Quantity, lv: 1);
            //* Particle UI Effect 1
            rwdSlotList[i].PlayScaleUIEF(rwdSlotList[i], rewardItem.Data.ItemImg);
            rwdSlotList[i].ItemImgScaleUIEF.startDelay = delay;
            //* Particle UI Effect 2
            rwdSlotList[i].WhiteDimScaleUIEF.lifetime = delay;
            rwdSlotList[i].WhiteDimScaleUIEF.Play();
            
            //* UNIQUE等級なら
            if(rewardItem.Data.Grade >= Enum.Grade.Unique) {
                //* 音
                switch(rewardItem.Data.Grade) {
                    case Enum.Grade.Unique: SM._.SfxPlay(SM.SFX.Merge2SFX, delay); break;
                    case Enum.Grade.Legend: SM._.SfxPlay(SM.SFX.Merge3SFX, delay); break;
                    case Enum.Grade.Myth:   SM._.SfxPlay(SM.SFX.Merge4SFX, delay); break;
                    case Enum.Grade.Prime:  SM._.SfxPlay(SM.SFX.Merge5SFX, delay); break;
                }
                //* Particle UI Equipment Effect 3
                rwdSlotList[i].HighGradeSpawnUIEF.enabled = true; //Play();
                rwdSlotList[i].Twincle1UIEF.enabled = true;
                rwdSlotList[i].Twincle2UIEF.enabled = true;
                //* High Grade Nice Effect 4
                rwdSlotList[i].HighGradeNiceUIEF.startDelay = delay;
                rwdSlotList[i].HighGradeRayUIEF.startDelay = delay;
                rwdSlotList[i].HighGradeHandUIEF.startDelay = delay;
                rwdSlotList[i].HighGradeBurstBlueUIEF.startDelay = delay;
                rwdSlotList[i].HighGradeBurstYellowUIEF.startDelay = delay;
                rwdSlotList[i].HighGradeNiceUIEF.Play();
            }
        }
    }
    public void ShowReward(List<RewardItem> itemList) {
        SM._.SfxPlay(SM.SFX.RewardSFX);
        HM._.hui.IsActivePopUp = true;
        WindowObj.SetActive(true);
        ReleaseAll();
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

        var invItemList = HM._.ivCtrl.InventoryData.ItemList;
        //* EquipChestで有れば、最大１２個まで一緒に開く
        if(type == Etc.ConsumableItem.ChestEquipment) {
            //* 次の開くイベント登録
            OnClickOpenChest = () => RwdItemDt.OpenRewardContent(
                chestDt, 
                Mathf.Min(invItemList.Find(item => !item.IsEmpty && item.Data.name == $"{Etc.ConsumableItem.ChestEquipment}").Quantity, Config.MAX_REWARD_SLOT) //equipChestCnt
            );
        }
        else {
            OnClickOpenChest = () => RwdItemDt.OpenRewardContent(chestDt);
        }


    }
#endregion
}
