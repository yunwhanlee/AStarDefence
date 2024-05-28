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
    public bool IsSkip = false;

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
        if(!IsFinishSlotsSpawn) {
            IsSkip = true;
            return;
        }
        
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
    IEnumerator CoReleaseAll() {
        rwdSlotList.ForEach(rwdSlot => {
            rwdSlot.ResetUI();
            rwdSlot.gameObject.SetActive(false);
        });
        yield return null;
        // foreach (Transform child in Content)
        //     child.gameObject.SetActive(false); // Destroy(child.gameObject);
    }

    /// <summary>
    /// リワードリスト表示
    /// </summary>
    private IEnumerator CoDisplayRewardList(List<RewardItem> rewardList) {
        for(int i = 0; i < rewardList.Count; i++)
            Debug.Log($"CoDisplayRewardList():: itemList[{i}].quantity= {rewardList[i].Quantity}");

        //* 数量によって、スロットサイズ
        bool isUnder10 = rewardList.Count <= 10;
        ContentGrid.constraintCount = isUnder10? 5 : 10;
        Content.localScale = Vector3.one * (isUnder10? 1 : 0.8f);        

        //* 事前 Set UI
        for(int i = 0; i < rewardList.Count; i++) {
            RewardItem rewardItem = rewardList[i];
            rwdSlotList[i].gameObject.SetActive(true);
            rwdSlotList[i].SetUI(rewardList[i].Data.Type, rewardItem.Data.Grade, rewardItem.Data.ItemImg, rewardItem.Quantity, lv: 1);
            //* アイテム 表示 OFF
            rwdSlotList[i].BgImg.enabled = false;
            rwdSlotList[i].ItemImg.enabled = false;
            rwdSlotList[i].LvTxt.enabled = false;
            rwdSlotList[i].QuantityTxt.enabled = false;
            rwdSlotList[i].TypeBgImg.enabled = false;
            rwdSlotList[i].TypeIconImg.enabled = false;
        }        

        // IsFinishSlotsSpawn = false;
        yield return Util.Time0_2;        

        //* UI EF
        for(int i = 0; i < rewardList.Count; i++) {
            RewardItem rewardItem = rewardList[i];

            if(!IsSkip) SM._.SfxPlay(SM.SFX.ItemPickSFX);
            //* Particle UI Effect 1
            rwdSlotList[i].PlayScaleUIEF(rwdSlotList[i], rewardItem.Data.ItemImg);
            //* Particle UI Effect 2
            rwdSlotList[i].WhiteDimScaleUIEF.Play();
            //* アイテム 表示 ON
            rwdSlotList[i].BgImg.enabled = true;
            rwdSlotList[i].ItemImg.enabled = true;
            rwdSlotList[i].LvTxt.enabled = true;
            rwdSlotList[i].QuantityTxt.enabled = true;
            rwdSlotList[i].TypeBgImg.enabled = !(rewardItem.Data.Type == Enum.ItemType.Etc);
            rwdSlotList[i].TypeIconImg.enabled = !(rewardItem.Data.Type == Enum.ItemType.Etc);
            
            //* UNIQUE等級なら
            if(rewardItem.Data.Grade >= Enum.Grade.Unique) {
                switch(rewardItem.Data.Grade) {
                    case Enum.Grade.Unique:
                        SM._.SfxPlay(SM.SFX.Merge2SFX);
                        break;
                    case Enum.Grade.Legend:
                        SM._.SfxPlay(SM.SFX.Merge3SFX);
                        if(rwdSlotList[i].LegendSpawnUIEF) rwdSlotList[i].LegendSpawnUIEF.Play();
                        if(rwdSlotList[i].DOTAnim) rwdSlotList[i].DOTAnim.DORestart();
                        break;
                    case Enum.Grade.Myth:
                        SM._.SfxPlay(SM.SFX.Merge4SFX);
                        SM._.SfxPlay(SM.SFX.RoarASFX);
                        if(rwdSlotList[i].MythSpawnUIEF) rwdSlotList[i].MythSpawnUIEF.Play();
                        if(rwdSlotList[i].DOTAnim) rwdSlotList[i].DOTAnim.DORestart();
                        break;
                    case Enum.Grade.Prime:
                        SM._.SfxPlay(SM.SFX.Merge5SFX);
                        SM._.SfxPlay(SM.SFX.RoarASFX);
                        SM._.SfxPlay(SM.SFX.InvStoneSFX);
                        if(rwdSlotList[i].PrimeSpawnUIEF) rwdSlotList[i].PrimeSpawnUIEF.Play();
                        if(rwdSlotList[i].DOTAnim) rwdSlotList[i].DOTAnim.DORestart();
                        break;
                }
                //* Particle UI Equipment Effect 3
                rwdSlotList[i].HighGradeSpawnUIEF.enabled = true; //Play();
                rwdSlotList[i].Twincle1UIEF.enabled = true;
                rwdSlotList[i].Twincle2UIEF.enabled = true;
            }

            if(!IsSkip)
                yield return Util.Time0_05;
            else
                yield return null;
        }
        yield return Util.Time0_5;
        IsFinishSlotsSpawn = true;
        IsSkip = false;
    }
    /// <summary>
    /// リワードスロットUIリスト 表示
    /// </summary>
    /// <param name="itemList">リワードでもらえるアイテムリスト</param> <summary>
    public void ShowReward(List<RewardItem> itemList) => StartCoroutine(CoShowRewardProccess(itemList));
    IEnumerator CoShowRewardProccess(List<RewardItem> itemList) {
        for(int i = 0; i < itemList.Count; i++)
            Debug.Log($"ShowReward():: itemList[{i}].quantity= {itemList[i].Quantity}");

            SM._.SfxPlay(SM.SFX.RewardSFX);
            HM._.hui.IsActivePopUp = true;
            IsFinishSlotsSpawn = false;
            WindowObj.SetActive(true);
            yield return CoReleaseAll();
            yield return CoDisplayRewardList(itemList);
            yield return HM._.rwm.CoUpdateInventoryAsync(itemList);
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
    /// インベントリー　ChestをTapして開くPopUp 表示
    /// </summary>  
    public void ShowChestPopUp(Etc.ConsumableItem type, int quantity) {
        SM._.SfxPlay(SM.SFX.CreateTowerSFX);
        RewardContentSO chestDt = (type == Etc.ConsumableItem.ChestCommon)? RwdItemDt.Rwd_ChestCommon
            : (type == Etc.ConsumableItem.ChestDiamond)? RwdItemDt.Rwd_ChestDiamond
            : (type == Etc.ConsumableItem.ChestEquipment)? RwdItemDt.Rwd_ChestEquipment
            : (type == Etc.ConsumableItem.ChestGold)? RwdItemDt.Rwd_ChestGold
            : RwdItemDt.Rwd_ChestPremium;
        SetChestPopUpUI(type, quantity);

        var invItemList = HM._.ivCtrl.InventoryData.invList;
        //* EquipChestで有れば、最大40個まで一緒に開く
        if(type == Etc.ConsumableItem.ChestEquipment) {
            //* 次の開くイベント登録
            OnClickOpenChest = () => RwdItemDt.OpenRewardContent(
                isOpenByInv: true,
                chestDt, 
                Mathf.Min(invItemList.Find(item => !item.IsEmpty &&
                    item.Data.name == $"{Etc.ConsumableItem.ChestEquipment}").Quantity
                    , Config.MAX_REWARD_SLOT) //equipChestCnt
            );
        }
        else if(type == Etc.ConsumableItem.ChestCommon) {
            //* 次の開くイベント登録
            OnClickOpenChest = () => RwdItemDt.OpenRewardContent(
                isOpenByInv: true,
                chestDt, 
                Mathf.Min(invItemList.Find(item => !item.IsEmpty && 
                    item.Data.name == $"{Etc.ConsumableItem.ChestCommon}").Quantity
                    , Config.MAX_REWARD_SLOT) //equipChestCnt
            );
        }
        else {
            OnClickOpenChest = () => RwdItemDt.OpenRewardContent(
                isOpenByInv: true,
                chestDt
            );
        }


    }
#endregion
}
