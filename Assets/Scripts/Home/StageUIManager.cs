using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.PixelFantasy.PixelHeroes.Common.Scripts.UI;
using DG.Tweening;
using Inventory.Model;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
public struct StageClearRewardItemUI {
    public GameObject Obj;
    public Image IconImg;
    public TMP_Text Txt; //? OreIconは名前, BonusItemIconは数量

    public void SetUI(Sprite iconSpr, string txt) {
        Obj.SetActive(true);
        IconImg.sprite = iconSpr;
        Txt.text = txt;
    }
}

public class StageUIManager : MonoBehaviour {
    [Header("GOBLIN DUNGEON")]
    [field:SerializeField] public GameObject DungeonSelectPopUp;
    [field:SerializeField] public GameObject GoblinDungeonWindow;
    [field:SerializeField] public TMP_Text GoldKeyTxt;
    [field:SerializeField] public GameObject InfiniteDungeonWindow;
    [field:SerializeField] public TMP_Text InfiniteDungeonGoldKeyTxt;
    [field: SerializeField] public TMP_Text InfiniteBestWaveScoreTxt;
    [field:SerializeField] public GameObject DungeonAlertDot;

    [field: Header("STAGE")]
    [field: SerializeField] public SettingEnemyData[] StageEnemyDatas {get; set;}
    [field:SerializeField] public int StageAlertIdx;
    [field:SerializeField] public GameObject NewStageAlertBtnObj;
    [field:SerializeField] public Sprite[] MapIconSprs;
    [field:SerializeField] public Image NewStageAlertMapImg;
    [field:SerializeField] public TMP_Text NewStageAlertTxt;

    [field:SerializeField] public GameObject StageGroup;
    [field:SerializeField] public GameObject[] StagePopUps;
    [field:SerializeField] public GameObject StageUnlockBonusChestBtnObj;
    [field:SerializeField] public TMP_Text StageUnlockBonusChestBtnTxt;
    [field:SerializeField] public Image StageUnlockBonusChestImg;
    [field:SerializeField] public GameObject SelectStageNumWindow;
    [field:SerializeField] public GameObject[] StageNewLabels;
    [field:SerializeField] public TMP_Text[] StageNumBtnTxts;
    [field:SerializeField] public TMP_Text[] StageNumBtnHpRatioTxts;
    
    [field:SerializeField] public GameObject WholeLockedFrame;
    [field:SerializeField] public GameObject Stage1_2LockedFrame;
    [field:SerializeField] public GameObject Stage1_3LockedFrame;

    [field: Header("STAGE INFO POPUP")]
    [field:SerializeField] public Color[] TopColors;
    [field:SerializeField] public Color[] TopLabelOutsideColors;
    [field:SerializeField] public Color[] TopLabelInsideColors;
    [field:SerializeField] public Color[] MiddleColors;
    [field:SerializeField] public Color[] BgColors;
    [field:SerializeField] public Sprite ConsumeItemsSpr;


    [field:SerializeField] public GameObject ClearRewardInfoPopUp;
    [field:SerializeField] public DOTweenAnimation ClearRewardInfoBodyDOTAnim;
    [field:SerializeField] public Image ClearRewardInfoTopImg;
    [field:SerializeField] public Image ClearRewardInfoTopLabelOutsideImg;
    [field:SerializeField] public Image ClearRewardInfoTopLabelInsideImg;
    [field:SerializeField] public Image ClearRewardInfoMiddleImg;
    [field:SerializeField] public Image ClearRewardInfoBgImg;
    [field:SerializeField] public TMP_Text ClearRewawrdInfoTitleTxt;
    [field:SerializeField] public TMP_Text ClearRewardInfoStageNumTxt;
    [field:SerializeField] public TMP_Text[] ClearRewardInfoCttQuantityTxts;
    [field:SerializeField] public StageClearRewardItemUI[] ClearRewardOreIconUIs;
    [field:SerializeField] public StageClearRewardItemUI[] ClearRewardBonusItemIconUIs;

    void Start() {
        DungeonAlertDot.SetActive(HM._.GoldKey > 0);
        NewStageAlertBtnObj.SetActive(false);
        InfiniteBestWaveScoreTxt.text =  $"최대돌파: {DM._.DB.InfiniteUpgradeDB.MyBestWaveScore}층";

        //* New Stage Alert 表示
        for(int i = 0; i < DM._.DB.StageLockedDBs.Length; i++) {
            StageLockedDB stageDb = DM._.DB.StageLockedDBs[i];
            int stgNumIdx = Array.FindIndex(stageDb.StageRewards, stgRwd => stgRwd.IsUnlockAlert);
            if(stgNumIdx != -1) {
                int stageStr = i + 1;
                int stgNumStr = stgNumIdx + 1;

                //* お知らせアンロックのトリガー OFF
                stageDb.StageRewards[stgNumIdx].IsUnlockAlert = false;
                StageNewLabels[stgNumIdx].SetActive(true);
                //* お知らせステージのIndex 設定
                StageAlertIdx = i;

                //* Alert UI
                NewStageAlertBtnObj.SetActive(true);
                NewStageAlertMapImg.sprite = MapIconSprs[StageAlertIdx];
                NewStageAlertTxt.text = $"스테이지{stageStr}-{stgNumStr} 가능!";
                HM._.hui.ShowMsgNotice($"축하합니다! 새로운 스테이지{stageStr}-{stgNumStr}가 열렸습니다!");
                break;
            }
        }

        UpdateStageBonusChestIcon();
    }

#region DUNGEON EVENT 
    public void OnClickDungeonIconBtn() {
        HM._.hui.IsActivePopUp = true;
        SM._.SfxPlay(SM.SFX.ItemPickSFX);

        DungeonSelectPopUp.SetActive(true);
        DungeonSelectPopUp.GetComponent<DOTweenAnimation>().DORestart();
        
    }
    public void OnClickOpenSelectDungeonWindow(int idx) {
        SM._.SfxPlay(SM.SFX.ClickSFX);
        if(idx == 0) {
            GoblinDungeonWindow.SetActive(true);
            GoldKeyTxt.text = $"{HM._.GoldKey}";
        }

        if(idx == 1) {
            InfiniteDungeonWindow.SetActive(true);
            InfiniteDungeonGoldKeyTxt.text = $"{HM._.GoldKey}";
        }
    }

    public void OnClickInfiniteUpgradeIcon() {
        SM._.SfxPlay(SM.SFX.ClickSFX);
        InfiniteDungeonWindow.SetActive(false);
        HM._.ifum.WindowObj.SetActive(true);
    }

    public void OnClickDungeonDifficultyBtn(int diffIdx) {
        if(HM._.GoldKey <= 0) {
            HM._.hui.ShowMsgError("황금열쇠가 있어야 입장가능합니다.");
            return;
        }
        --HM._.GoldKey;
        DM._.SelectedStage = Config.Stage.STG_GOBLIN_DUNGEON;

        HM._.ivCtrl.CheckActiveClover();

        //* ホーム ➡ ゲームシーン移動の時、インベントリのデータを保存
        DM._.Save();

        SM._.SfxPlay(SM.SFX.StageSelectSFX);

        //* 難易度 データ設定
        DM._.SelectedStageNum = (diffIdx == 0)? Enum.StageNum.Stage1_1
            : (diffIdx == 1)? Enum.StageNum.Stage1_2
            : Enum.StageNum.Stage1_3;

        //* ➡ ゲームシーンロード
        SceneManager.LoadScene(Enum.Scene.Game.ToString());
    }

    public void OnClickInfiniteDungeonEnterBtn() {
        if(HM._.GoldKey <= 0) {
            HM._.hui.ShowMsgError("황금열쇠가 있어야 입장가능합니다.");
            return;
        }
        --HM._.GoldKey;
        DM._.SelectedStage = Config.Stage.STG_INFINITE_DUNGEON;

        //* ホーム ➡ ゲームシーン移動の時、インベントリのデータを保存
        DM._.Save();

        SM._.SfxPlay(SM.SFX.StageSelectSFX);

        DM._.SelectedStageNum = 0;

        //* ➡ ゲームシーンロード
        SceneManager.LoadScene(Enum.Scene.Game.ToString());
    }
#endregion
#region STAGE EVENT
    private (Sprite, ItemSO) GetStageBonusRewardChestData(int stg, int stgNum) {
        var consumeItemDts = HM._.rwlm.RwdItemDt.EtcConsumableDatas;
        ItemSO commonChest = consumeItemDts[(int)Etc.ConsumableItem.ChestCommon];
        ItemSO equipChest = consumeItemDts[(int)Etc.ConsumableItem.ChestEquipment];
        ItemSO goldChest = consumeItemDts[(int)Etc.ConsumableItem.ChestGold];
        ItemSO diamondChest = consumeItemDts[(int)Etc.ConsumableItem.ChestDiamond];
        ItemSO PremiumChest = consumeItemDts[(int)Etc.ConsumableItem.ChestPremium];

        switch(stg) {
            case Config.Stage.STG1_FOREST:
                if(stgNum == 0)
                    return (commonChest.ItemImg, commonChest);
                else if(stgNum == 1)
                    return (equipChest.ItemImg, equipChest);
                else
                    return (goldChest.ItemImg, goldChest);
            case Config.Stage.STG2_DESERT:
                if(stgNum == 0)
                    return (commonChest.ItemImg, commonChest);
                else if(stgNum == 1)
                    return (equipChest.ItemImg, equipChest);
                else
                    return (diamondChest.ItemImg, diamondChest);
            case Config.Stage.STG3_SEA:
                if(stgNum == 0)
                    return (goldChest.ItemImg, goldChest);
                else if(stgNum == 1)
                    return (equipChest.ItemImg, equipChest);
                else
                    return (equipChest.ItemImg, equipChest);
            case Config.Stage.STG4_UNDEAD:
            case Config.Stage.STG5_HELL:
                if(stgNum == 0)
                    return (equipChest.ItemImg, equipChest);
                else if(stgNum == 1)
                    return (equipChest.ItemImg, equipChest);
                else
                    return (PremiumChest.ItemImg, PremiumChest);
            default:
                return (null, null); // 기본값으로 리턴할 값 지정
        }
    }
    private void UpdateStageBonusChestIcon() {
        StageUnlockBonusChestBtnObj.SetActive(false);

        int i = 0;
        foreach (var stageDb in DM._.DB.StageLockedDBs) {
            int stgNumIdx = Array.FindIndex(stageDb.StageRewards, stgRwd => stgRwd.IsActiveBonusReward);
            Debug.Log("UpdateStageBonusChestIcon():: stgNumIdx= " + stgNumIdx);

            if(stgNumIdx != -1) {
                int stage = i + 1;
                int stgNum = stgNumIdx + 1;

                //* Bonus Reward Chest UI
                StageUnlockBonusChestBtnObj.SetActive(true);
                StageUnlockBonusChestBtnTxt.text = $"클리어 보너스:{stage}-{stgNum}";
                StageUnlockBonusChestImg.sprite = GetStageBonusRewardChestData(i, stgNumIdx).Item1;
                return;
            }
            i++;
        }
    }
    public void OnClickStageUnlockBonusChestIconBtn() {
        string stageInfoStr = StageUnlockBonusChestBtnTxt.text.Split(":")[1];
        string[] splitStr = stageInfoStr.Split("-");
        int stgIdx = int.Parse(splitStr[0]) - 1;
        int stgNumIdx = int.Parse(splitStr[1]) - 1;
        Debug.Log($"OnClickStageUnlockBonusChestIconBtn():: STAGE: stgIdx={stgIdx}, stgNumIdx={stgNumIdx}");

        //* Activeトリガー OFF
        DM._.DB.StageLockedDBs[stgIdx].StageRewards[stgNumIdx].IsActiveBonusReward = false;

        //* 最新化
        UpdateStageBonusChestIcon();

        //* Reward Chest
        var rewardList = new List<RewardItem> {
            new (GetStageBonusRewardChestData(stgIdx, stgNumIdx).Item2)
        };
        HM._.rwlm.ShowReward(rewardList);
        // HM._.rwm.CoUpdateInventoryAsync(rewardList);
    }
    public void OnClickNewStageAlertBtn() {
        Debug.Log($"OnClickNewStageAlertBtn():: NewStageAlertIdx= {StageAlertIdx}");
        HM._.SelectedStage = StageAlertIdx;
        HM._.hui.OnClickPlayBtn();
    }
    public void OnClickStartBtn() {
        var selectStage = HM._.SelectedStage;

        HM._.hui.IsActivePopUp = true;
        SM._.SfxPlay(SM.SFX.ClickSFX);
        SelectStageNumWindow.SetActive(true);

        StageNumBtnTxts[0].text = $"{selectStage + 1} - 1";
        StageNumBtnTxts[1].text = $"{selectStage + 1} - 2";
        StageNumBtnTxts[2].text = $"{selectStage + 1} - 3";

        //* StageNum Enemy Hp Ratio Txt 表示
        StageNumBtnHpRatioTxts[0].text = $"몬스터 체력 {StageEnemyDatas[3 * selectStage + 0].HpRatio * 100}%";
        StageNumBtnHpRatioTxts[1].text = $"몬스터 체력 {StageEnemyDatas[3 * selectStage + 1].HpRatio * 100}%";
        StageNumBtnHpRatioTxts[2].text = $"몬스터 체력 {StageEnemyDatas[3 * selectStage + 2].HpRatio * 100}%";

        //* StageNum Btns Unlock 表示
        Stage1_2LockedFrame.SetActive(DM._.DB.StageLockedDBs[selectStage].IsLockStage1_2);
        Stage1_3LockedFrame.SetActive(DM._.DB.StageLockedDBs[selectStage].IsLockStage1_3);

        DM._.SelectedStage = selectStage;
    }
    public void OnClickPlayBtn() {
        SM._.SfxPlay(SM.SFX.StageSelectSFX);

        HM._.ivCtrl.CheckActiveClover();

        //* ホーム ➡ ゲームシーン移動の時、インベントリのデータを保存
        DM._.Save();

        //* ➡ ゲームシーンロード
        SceneManager.LoadScene(Enum.Scene.Game.ToString());
    }

    /// <summary>
    /// 矢印でステージ移動
    /// </summary>
    /// <param name="dir">方向(-1：左、1：右)</param>
    public void OnClickArrowBtn(int dir) {
        SM._.SfxPlay(SM.SFX.ClickSFX);
        var hm = HM._;
        hm.SelectedStage += dir;

        //* ページ Min & Max 繰り返す
        if(hm.SelectedStage < 0) hm.SelectedStage = StagePopUps.Length - 1;
        else if(hm.SelectedStage > StagePopUps.Length - 1) hm.SelectedStage = 0;

        //* UI表示
        // 初期化(全て非表示)
        Array.ForEach(StagePopUps, popUp => popUp.SetActive(false)); 
        // 選択したステージ 表示
        StagePopUps[hm.SelectedStage].SetActive(true);
        //　ロック状況 表示
        WholeLockedFrame.SetActive(DM._.DB.StageLockedDBs[hm.SelectedStage].IsLockStage1_1);
    }
    public void OnClickBackBtn() { //* 閉じるボタン(StageとGoblin Dungeon全て)
        HM._.hui.IsActivePopUp = false;
        SM._.SfxPlay(SM.SFX.ClickSFX);
        if(SelectStageNumWindow.activeSelf) {
            SelectStageNumWindow.SetActive(false);
            return;
        }

        StageGroup.SetActive(false);
        GoblinDungeonWindow.SetActive(false);
        InfiniteDungeonWindow.SetActive(false);
        DungeonSelectPopUp.SetActive(false);
        Array.ForEach(StagePopUps, popUp => popUp.SetActive(false));
    }
#endregion
#region STAGE CLEAR INFO EVENT
    /// <summary>
    /// ステージクリアー情報POPUP 表示
    /// </summary>
    /// <param name="idxNum">ステージINDEX 1-1, 1-2, 1-3</param>
    public void OnClickStageClearInfoIconBtn(int idxNum) {
    #region BONUS REWAWR ICON UI
        Debug.Log($"OnClickStageClearInfoIconBtn(SelectedStage= {HM._.SelectedStage}):: idxNum= {idxNum}");
        //* ステージ
        int stageIdx = HM._.SelectedStage;

        //* ステージ 難易度
        DM._.SelectedStageNum = (idxNum == 0)? Enum.StageNum.Stage1_1
            : (idxNum == 1)? Enum.StageNum.Stage1_2
            : Enum.StageNum.Stage1_3;

        SM._.SfxPlay(SM.SFX.StageSelectSFX);
        ClearRewardInfoPopUp.SetActive(true);
        ClearRewardInfoBodyDOTAnim.DORestart();

        //* UI色
        ClearRewardInfoTopImg.color = TopColors[idxNum];
        ClearRewardInfoTopLabelOutsideImg.color = TopLabelOutsideColors[idxNum];
        ClearRewardInfoTopLabelInsideImg.color = TopLabelInsideColors[idxNum];
        ClearRewardInfoMiddleImg.color = MiddleColors[idxNum];
        ClearRewardInfoBgImg.color = BgColors[idxNum];

        ClearRewawrdInfoTitleTxt.text = (stageIdx == Config.Stage.STG1_FOREST)? "초원맵 클리어 보상"
            : (stageIdx == Config.Stage.STG2_DESERT)? "사막맵 클리어 보상"
            : (stageIdx == Config.Stage.STG3_SEA)? "바다맵 클리어 보상"
            : (stageIdx == Config.Stage.STG4_UNDEAD)? "언데드맵 클리어 보상"
            : (stageIdx == Config.Stage.STG5_HELL)? "지옥맵 클리어 보상"
            : "";

        ClearRewardInfoStageNumTxt.text = $"{stageIdx + 1} - {idxNum + 1}";
        
        var rwDt = HM._.rwlm.RwdItemDt;
        RewardContentSO stgClrRwDt = rwDt.Rwd_StageClearDts[stageIdx * 3 + idxNum];

        //* リワード 数量UI表示
        for(int i = 0; i < ClearRewardInfoCttQuantityTxts.Length; i++) {
            if(i == 0)
                ClearRewardInfoCttQuantityTxts[i].text = $"{stgClrRwDt.ExpMax}";
            else if(i == 1)
                ClearRewardInfoCttQuantityTxts[i].text = $"{stgClrRwDt.CoinMax}";
            else if(i == 2)
                ClearRewardInfoCttQuantityTxts[i].text = $"{stgClrRwDt.OreMin}~{stgClrRwDt.OreMax}";
            else if(i == 3)
                ClearRewardInfoCttQuantityTxts[i].text = $"{stgClrRwDt.FameMax}";
        }

        //* もらえる ORE UI
        const int MAX_ORE_ICON_CNT = 4;
        const int OFFSET_OREIDX = (int)Etc.NoshowInvItem.Ore0;
        var orePercentList = stgClrRwDt.RwdGradeTb.OrePerList;

        //* 初期化
        for(int i = 0; i < ClearRewardOreIconUIs.Length; i++)
            ClearRewardOreIconUIs[i].Obj.SetActive(false);

        //* Ore アイコン
        int oreIconIdx = 0;
        for(int i = 0; i < orePercentList.Count; i++) {
            // 用意したIcon数を超えたら、For文終了
            if(oreIconIdx == MAX_ORE_ICON_CNT) return;
            // UI 設定
            var orePer = orePercentList[i];
            if(orePer != 0) {
                ClearRewardOreIconUIs[oreIconIdx++].SetUI(
                    rwDt.EtcNoShowInvDatas[i + OFFSET_OREIDX].ItemImg
                    , rwDt.EtcNoShowInvDatas[i + OFFSET_OREIDX].Name
                );
            }
        }
    #endregion

    #region BONUS REWAWR ICON UI
        const int CHEST_COMMON = (int)Etc.ConsumableItem.ChestCommon;
        const int CHEST_EQUIP = (int)Etc.ConsumableItem.ChestEquipment;
        const int GOLDKEY = (int)Etc.NoshowInvItem.GoldKey;
        const int MAGIC_STONE = (int)Etc.ConsumableItem.MagicStone;
        const int SOUL_STONE = (int)Etc.ConsumableItem.SoulStone;
        const int PREMIUM_CHEST = (int)Etc.ConsumableItem.ChestPremium;

        //* 初期化
        for(int i = 0; i < ClearRewardBonusItemIconUIs.Length; i++)
            ClearRewardBonusItemIconUIs[i].Obj.SetActive(false);

        //* Bonus アイコン
        RewardPercentTable tb = stgClrRwDt.ItemPerTb;
        int bonusIconIdx = 0;
        if(tb.ChestCommon > 0) {
            ClearRewardBonusItemIconUIs[bonusIconIdx++].SetUI(
                rwDt.EtcConsumableDatas[CHEST_COMMON].ItemImg
                , stgClrRwDt.GetChestCommonQuantityTxt()
            );
        }
        if(tb.ConsumeItem > 0) {
            ClearRewardBonusItemIconUIs[bonusIconIdx++].SetUI(
                ConsumeItemsSpr
                , stgClrRwDt.GetConsumeItemQuantityTxt()
            );
        }
        if(tb.ChestEquipment > 0) {
            ClearRewardBonusItemIconUIs[bonusIconIdx++].SetUI(
                rwDt.EtcConsumableDatas[CHEST_EQUIP].ItemImg
                , stgClrRwDt.GetChestEquipmentQuantityTxt()
            );
        }
        if(tb.GoldKey > 0) {
            ClearRewardBonusItemIconUIs[bonusIconIdx++].SetUI(
                rwDt.EtcNoShowInvDatas[GOLDKEY].ItemImg
                , stgClrRwDt.GetGoldKeyQuantityTxt()
            );
        }
        if(tb.MagicStone > 0) {
            ClearRewardBonusItemIconUIs[bonusIconIdx++].SetUI(
                rwDt.EtcConsumableDatas[MAGIC_STONE].ItemImg
                , stgClrRwDt.GetMagicStoneQuantityTxt()
            );
        }
        if(tb.SoulStone > 0) {
            ClearRewardBonusItemIconUIs[bonusIconIdx++].SetUI(
                rwDt.EtcConsumableDatas[SOUL_STONE].ItemImg
                , stgClrRwDt.GetSoulStoneQuantityTxt()
            );
        }
        if(tb.ChestPremium > 0) {
            ClearRewardBonusItemIconUIs[bonusIconIdx++].SetUI(
                rwDt.EtcConsumableDatas[PREMIUM_CHEST].ItemImg
                , $"{1}"
            );
        }
    #endregion
    }
    public void OnClickStageClearInfoPopUpCloseBtn() {
        SM._.SfxPlay(SM.SFX.ClickSFX);
        ClearRewardInfoPopUp.SetActive(false);
    }
#endregion
}
