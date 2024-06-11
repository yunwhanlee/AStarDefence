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
    [Header("DUNGEON")]
    [field:SerializeField] public GameObject DungeonSelectPopUp;

    [field:SerializeField] public GameObject GoblinDungeonWindow;
    [field:SerializeField] public GameObject GoblinDungeonNormalLockedFrame;
    [field:SerializeField] public Button GoblinDungeonNormalEnterBtn;
    [field:SerializeField] public GameObject GoblinDungeonHardLockedFrame;
    [field:SerializeField] public Button GoblinDungeonHardEnterBtn;
    [field:SerializeField] public TMP_Text GoldKeyTxt;

    [field:SerializeField] public GameObject InfiniteDungeonWindow;
    [field:SerializeField] public GameObject InfiniteDungeonLockedFrame;
    [field:SerializeField] public Button InfiniteDungeonEnterBtn;
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
    [field:SerializeField] public TMP_Text StageUnlockBonusChestQuantityTxt;
    [field:SerializeField] public GameObject SelectStageNumWindow;
    [field:SerializeField] public GameObject[] StageNewLabels;
    [field:SerializeField] public TMP_Text[] StageNumBtnTxts;
    [field:SerializeField] public TMP_Text[] StageNumBtnHpRatioTxts;
    
    [field:SerializeField] public GameObject WholeLockedFrame;
    [field:SerializeField] public GameObject Stage1_2LockedFrame;
    [field:SerializeField] public GameObject Stage1_3LockedFrame;

    [field:SerializeField] public Transform[] StageUnlockSigns;

    [field: Header("STAGE INFO POPUP")]
    [field:SerializeField] public Color[] TopColors;
    [field:SerializeField] public Color[] TopLabelOutsideColors;
    [field:SerializeField] public Color[] TopLabelInsideColors;
    [field:SerializeField] public Color[] MiddleColors;
    [field:SerializeField] public Color[] BgColors;
    [field:SerializeField] public Sprite ConsumeItemsSpr;

    [field:SerializeField] public GameObject ClearRewardInfoPopUp;
    [field:SerializeField] public DOTweenAnimation ClearRewardInfoBodyDOTAnim;

    [field:SerializeField] public Button ClearRewardInfoStagePlayBtn;
    [field:SerializeField] public Button ClearRewardInfoGoblinDungeonPlayBtn;

    [field:SerializeField] public Image ClearRewardInfoTopImg;
    [field:SerializeField] public Image ClearRewardInfoTopLabelOutsideImg;
    [field:SerializeField] public Image ClearRewardInfoTopLabelInsideImg;
    [field:SerializeField] public Image ClearRewardInfoMiddleImg;
    [field:SerializeField] public Image ClearRewardInfoBgImg;
    [field:SerializeField] public TMP_Text ClearRewawrdInfoTitleTxt;
    [field:SerializeField] public TMP_Text ClearRewardInfoStageNumTxt;
    [field:SerializeField] public Image FixedRwdIcon3Img;
    [field:SerializeField] public TMP_Text FixedRwdIcon3NameTxt;
    [field:SerializeField] public Image FixedRwdIcon4Img;
    [field:SerializeField] public TMP_Text[] ClearRewardInfoCttQuantityTxts;

    [field:SerializeField] public TMP_Text ClearRewardInfoMiningRwdDetailTitleTxt;
    [field:SerializeField] public StageClearRewardItemUI[] ClearRewardMiningItemIconUIs;

    [field:SerializeField] public TMP_Text ClearRewardInfoBonusRewardTitleCntTxt;
    [field:SerializeField] public StageClearRewardItemUI[] ClearRewardBonusItemIconUIs;

    void Start() {
        DungeonAlertDot.SetActive(HM._.GoldKey > 0);
        NewStageAlertBtnObj.SetActive(false);
        InfiniteBestWaveScoreTxt.text =  $"최대 돌파웨이브 : {DM._.DB.InfiniteUpgradeDB.MyBestWaveScore}층";

        //* ダンジョンのアンロック状態
        GoblinDungeonNormalLockedFrame.SetActive(!DM._.DB.DungeonLockedDB.IsLockGoblinNormal);
        GoblinDungeonNormalEnterBtn.interactable = DM._.DB.DungeonLockedDB.IsLockGoblinNormal;

        GoblinDungeonHardLockedFrame.SetActive(!DM._.DB.DungeonLockedDB.IsLockGoblinHard);
        GoblinDungeonHardEnterBtn.interactable = DM._.DB.DungeonLockedDB.IsLockGoblinHard;

        InfiniteDungeonLockedFrame.SetActive(!DM._.DB.DungeonLockedDB.IsLockInfinite);
        InfiniteDungeonEnterBtn.interactable = DM._.DB.DungeonLockedDB.IsLockInfinite;

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
                string newStageStr = $"{stageStr}-{stgNumStr}";
                string dungeinUnlockMsg = (newStageStr == "1-3")? " 및 고블린던전 노말"
                    : (newStageStr == "2-3")? " 및 무한균열던전"
                    : (newStageStr == "3-3")? " 및 고블린던전 하드"
                    : "";
                NewStageAlertTxt.text = $"스테이지{newStageStr} 플레이!";
                HM._.hui.ShowMsgNotice($"스테이지{newStageStr}{dungeinUnlockMsg} 플레이 가능!");
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
    /// <summary>
    /// ゴブリンダンジョン
    /// </summary>
    public void OnClickGoblinDungeonEnterBtn() {
        int neededGoldKey = 1;

        if(HM._.GoldKey < neededGoldKey) {
            HM._.hui.ShowMsgError("황금열쇠가 부족합니다.");
            return;
        }
        HM._.GoldKey -= neededGoldKey;
        DM._.SelectedStage = Config.Stage.STG_GOBLIN_DUNGEON;

        HM._.ivCtrl.CheckActiveClover();

        //* ホーム ➡ ゲームシーン移動の時、インベントリのデータを保存
        DM._.Save();

        SM._.SfxPlay(SM.SFX.StageSelectSFX);

        //* ➡ ゲームシーンロード
        SceneManager.LoadScene(Enum.Scene.Game.ToString());
    }

    /// <summary>
    /// 無限ダンジョン
    /// </summary>
    public void OnClickInfiniteDungeonEnterBtn() {
        int neededGoldKey = 1;
        if(HM._.GoldKey < neededGoldKey) {
            HM._.hui.ShowMsgError("황금열쇠가 부족합니다.");
            return;
        }
        HM._.GoldKey -= neededGoldKey;
        DM._.SelectedStage = Config.Stage.STG_INFINITE_DUNGEON;

        if(DM._.DB.InfiniteTileMapSaveDt.IsSaved) {
            HM._.hui.ShowAgainAskMsg($"게임을 이어서 하시겠습니까?\n<size=90%>(현재 층 : {DM._.DB.InfiniteTileMapSaveDt.Wave})</size>", isActiveNoBtn: true, "처음부터");
            //* YES (CONTINUE)
            HM._.hui.OnClickAskConfirmAction = () => {
                DM._.DB.InfiniteTileMapSaveDt.LoadStageValDt();
                PlayGame();
            };
            //* NO (NEW START)
            HM._.hui.OnClickAskCloseExtraAction = () => {
                DM._.DB.InfiniteTileMapSaveDt.Reset();
                PlayGame();
            };
        }
        else {
            PlayGame();
        }
    }

    public void PlayGame() {
        //* ホーム ➡ ゲームシーン移動の時、インベントリのデータを保存
        DM._.Save();
        SM._.SfxPlay(SM.SFX.StageSelectSFX);
        DM._.SelectedStageNum = 0;

        //* ➡ ゲームシーンロード
        SceneManager.LoadScene(Enum.Scene.Game.ToString());
    }
#endregion

#region STAGE EVENT
    private (Sprite, ItemSO, int) GetStageBonusRewardChestData(int stg, int stgNum) {
        var consumeItemDts = HM._.rwlm.RwdItemDt.EtcConsumableDatas;
        ItemSO commonChest = consumeItemDts[(int)Etc.ConsumableItem.ChestCommon];
        ItemSO equipChest = consumeItemDts[(int)Etc.ConsumableItem.ChestEquipment];
        ItemSO goldChest = consumeItemDts[(int)Etc.ConsumableItem.ChestGold];
        ItemSO diamondChest = consumeItemDts[(int)Etc.ConsumableItem.ChestDiamond];
        ItemSO PremiumChest = consumeItemDts[(int)Etc.ConsumableItem.ChestPremium];

        switch(stg) {
            case Config.Stage.STG1_FOREST:
                if(stgNum == 0)
                    return (commonChest.ItemImg, commonChest, 1);
                else if(stgNum == 1)
                    return (equipChest.ItemImg, equipChest, 1);
                else
                    return (goldChest.ItemImg, goldChest, 1);
            case Config.Stage.STG2_DESERT:
                if(stgNum == 0)
                    return (commonChest.ItemImg, commonChest, 2);
                else if(stgNum == 1)
                    return (equipChest.ItemImg, equipChest, 2);
                else
                    return (diamondChest.ItemImg, diamondChest, 1);
            case Config.Stage.STG3_SEA:
                if(stgNum == 0)
                    return (goldChest.ItemImg, goldChest, 1);
                else if(stgNum == 1)
                    return (equipChest.ItemImg, equipChest, 3);
                else
                    return (equipChest.ItemImg, equipChest, 4);
            case Config.Stage.STG4_UNDEAD:
            case Config.Stage.STG5_HELL:
                if(stgNum == 0)
                    return (equipChest.ItemImg, equipChest, 5);
                else if(stgNum == 1)
                    return (equipChest.ItemImg, equipChest, 6);
                else
                    return (PremiumChest.ItemImg, PremiumChest, 1);
            default:
                return (null, null, 1); // 기본값으로 리턴할 값 지정
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
                
                (Sprite, ItemSO, int) rwd = GetStageBonusRewardChestData(i, stgNumIdx);
                //* Bonus Reward Chest UI
                StageUnlockBonusChestBtnObj.SetActive(true);
                StageUnlockBonusChestBtnTxt.text = $"클리어 보너스:{stage}-{stgNum}";
                StageUnlockBonusChestImg.sprite = GetStageBonusRewardChestData(i, stgNumIdx).Item1;
                StageUnlockBonusChestQuantityTxt.text = $"X{rwd.Item3}";
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
        (Sprite, ItemSO, int) rwd = GetStageBonusRewardChestData(stgIdx, stgNumIdx);
        ItemSO itemDt = rwd.Item2;
        int quantity = rwd.Item3;

        var rewardList = new List<RewardItem> { new (itemDt, quantity) };
        HM._.rwlm.ShowReward(rewardList);
    }
    public void OnClickNewStageAlertBtn() {
        Debug.Log($"OnClickNewStageAlertBtn():: NewStageAlertIdx= {StageAlertIdx}");
        HM._.SelectedStageIdx = StageAlertIdx;
        HM._.hui.OnClickPlayBtn();
    }
    /// <summary>
    /// ステージ選択
    /// </summary>
    public void OnClickSelectStageBtn() {
        var selectStage = HM._.SelectedStageIdx;

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
    /// <summary>
    /// 選んだステージをプレイ From StageClearInfoPopUp
    /// </summary>
    public void OnClickPlayBtn(bool isLoadSaveDt = false) {
        SM._.SfxPlay(SM.SFX.StageSelectSFX);
        HM._.ivCtrl.CheckActiveClover();

        //* ステージの保存データ リセット？
        if(!isLoadSaveDt) { // && DM._.DB.StageTileMapSaveDt.IsSaved
            DM._.DB.StageTileMapSaveDt.Reset();
        }

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
        hm.SelectedStageIdx += dir;

        //* ページ Min & Max 繰り返す
        if(hm.SelectedStageIdx < 0) hm.SelectedStageIdx = StagePopUps.Length - 1;
        else if(hm.SelectedStageIdx > StagePopUps.Length - 1) hm.SelectedStageIdx = 0;

        //* UI表示
        // 初期化(全て非表示)
        Array.ForEach(StagePopUps, popUp => popUp.SetActive(false)); 
        // 選択したステージ 表示
        StagePopUps[hm.SelectedStageIdx].SetActive(true);
        // StageSignGroup表示
        UpdateStageUnlockSignGroup();
        //　ロック状況 表示
        WholeLockedFrame.SetActive(DM._.DB.StageLockedDBs[hm.SelectedStageIdx].IsLockStage1_1);
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
        Debug.Log($"OnClickStageClearInfoIconBtn(SelectedStage= {HM._.SelectedStageIdx}):: idxNum= {idxNum}");
        SM._.SfxPlay(SM.SFX.StageSelectSFX);
        ClearRewardInfoStagePlayBtn.gameObject.SetActive(true);
        ClearRewardInfoGoblinDungeonPlayBtn.gameObject.SetActive(false);
        ClearRewardInfoPopUp.SetActive(true);
        ClearRewardInfoBodyDOTAnim.DORestart();

        //* プレイボタンの活性化
        ClearRewardInfoStagePlayBtn.interactable = true;
        if((idxNum == 1 && Stage1_2LockedFrame.activeSelf)
        || (idxNum == 2 && Stage1_3LockedFrame.activeSelf)) {
            ClearRewardInfoStagePlayBtn.interactable = false;
        }

        //* ステージ
        int stageIdx = HM._.SelectedStageIdx;

        //* ステージ 難易度
        DM._.SelectedStageNum = (idxNum == 0)? Enum.StageNum.Stage_1
            : (idxNum == 1)? Enum.StageNum.Stage_2
            : Enum.StageNum.Stage_3;

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
            : (stageIdx == Config.Stage.STG5_HELL)? "지옥맵 클리어 보상" : "";

        ClearRewardInfoStageNumTxt.text = $"{stageIdx + 1} - {idxNum + 1}";
        
        var rwDt = HM._.rwlm.RwdItemDt;
        int stgDtIdx = Config.Stage.GetCurStageDtIdx(stageIdx, idxNum);
        Debug.Log($"OnClickStageClearInfoIconBtn():: StageDtIdx= {stgDtIdx}");
        RewardContentSO stgClrRwdDt = rwDt.Rwd_StageClearDts[stgDtIdx];

        //* 固定の４つリワードアイコン
        ClearRewardInfoMiningRwdDetailTitleTxt.text = "광석 종류";
        for(int i = 0; i < ClearRewardInfoCttQuantityTxts.Length; i++) {
            if(i == 0) // EXP
                ClearRewardInfoCttQuantityTxts[i].text = $"{stgClrRwdDt.ExpMax}";
            else if(i == 1) // COIN
                ClearRewardInfoCttQuantityTxts[i].text = $"{stgClrRwdDt.CoinMax}";
            else if(i == 2) { // MINING : ORE
                FixedRwdIcon3Img.sprite = rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Ore3].ItemImg;
                FixedRwdIcon3NameTxt.text = "광석";
                ClearRewardInfoCttQuantityTxts[i].text = $"{stgClrRwdDt.OreMin}~{stgClrRwdDt.OreMax}";
            }
            else if(i == 3) { // FAME
                FixedRwdIcon4Img.sprite = rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Fame].ItemImg;
                ClearRewardInfoCttQuantityTxts[i].text = $"{stgClrRwdDt.FameMax}";
            }
        }
    
        //* 習得できるORE Detail UI
        const int DETAIL_ICON_MAX_CNT = 4;
        const int OFFSET_ORE_IDX = (int)Etc.NoshowInvItem.Ore0;
        var orePercentList = stgClrRwdDt.RwdGradeTb.OrePerList;

        //* 初期化
        for(int i = 0; i < ClearRewardMiningItemIconUIs.Length; i++)
            ClearRewardMiningItemIconUIs[i].Obj.SetActive(false);

        //* Ore アイコン
        int oreIconIdx = 0;
        for(int i = 0; i < orePercentList.Count; i++) {
            // 用意したIcon数を超えたら、For文終了
            if(oreIconIdx == DETAIL_ICON_MAX_CNT)
                break;

            // UI 設定
            if(orePercentList[i] != 0) {
                ItemSO oreDt = rwDt.EtcNoShowInvDatas[i + OFFSET_ORE_IDX];
                string name = oreDt.Name.Split("광석")[0]; //* 名前短縮
                ClearRewardMiningItemIconUIs[oreIconIdx++].SetUI(oreDt.ItemImg, name);
            }
        }

        //* ボーナスリワードアイコン UI *//
        const int CHEST_COMMON = (int)Etc.ConsumableItem.ChestCommon;
        const int CHEST_EQUIP = (int)Etc.ConsumableItem.ChestEquipment;
        const int GOLDKEY = (int)Etc.NoshowInvItem.GoldKey;
        const int MAGIC_STONE = (int)Etc.ConsumableItem.MagicStone;
        const int SOUL_STONE = (int)Etc.ConsumableItem.SoulStone;
        const int PREMIUM_CHEST = (int)Etc.ConsumableItem.ChestPremium;

        //* ボーナスリワード数
        int cnt = stgClrRwdDt.Cnt - Config.Stage.CLEAR_REWARD_FIX_CNT;
        ClearRewardInfoBonusRewardTitleCntTxt.text = $"보너스 보상 (랜덤 {cnt}종류)";

        //* 初期化
        ClearRewardInfoBonusRewardTitleCntTxt.gameObject.SetActive(true);
        for(int i = 0; i < ClearRewardBonusItemIconUIs.Length; i++)
            ClearRewardBonusItemIconUIs[i].Obj.SetActive(false);

        //* Bonus アイコン
        RewardPercentTable tb = stgClrRwdDt.ItemPerTb;
        var consumeDts = rwDt.EtcConsumableDatas;
        var noShowDts = rwDt.EtcNoShowInvDatas;
        int bonusIconIdx = 0;

        if(tb.ChestCommon > 0)
            ClearRewardBonusItemIconUIs[bonusIconIdx++].SetUI(consumeDts[CHEST_COMMON].ItemImg, stgClrRwdDt.GetChestCommonQuantityTxt());
        if(tb.ConsumeItem > 0)
            ClearRewardBonusItemIconUIs[bonusIconIdx++].SetUI(ConsumeItemsSpr, stgClrRwdDt.GetConsumeItemQuantityTxt());
        if(tb.ChestEquipment > 0)
            ClearRewardBonusItemIconUIs[bonusIconIdx++].SetUI(consumeDts[CHEST_EQUIP].ItemImg, stgClrRwdDt.GetChestEquipmentQuantityTxt());
        if(tb.GoldKey > 0)
            ClearRewardBonusItemIconUIs[bonusIconIdx++].SetUI(noShowDts[GOLDKEY].ItemImg, stgClrRwdDt.GetGoldKeyQuantityTxt());
        if(tb.MagicStone > 0)
            ClearRewardBonusItemIconUIs[bonusIconIdx++].SetUI(consumeDts[MAGIC_STONE].ItemImg, stgClrRwdDt.GetMagicStoneQuantityTxt());
        if(tb.SoulStone > 0)
            ClearRewardBonusItemIconUIs[bonusIconIdx++].SetUI(consumeDts[SOUL_STONE].ItemImg, stgClrRwdDt.GetSoulStoneQuantityTxt());
        if(tb.ChestPremium > 0)
            ClearRewardBonusItemIconUIs[bonusIconIdx++].SetUI(consumeDts[PREMIUM_CHEST].ItemImg, "1");
    }

    /// <summary>
    /// ゴブリンダンジョンのクリアー情報POPUP 表示
    /// </summary>
    /// <param name="idxNum">ステージ EASY, NORMAL, HARD</param>
    public void OnClickGoblinDungeonStageBtn(int idxNum) {
        if(DM._.DB.TutorialDB.IsActiveEnemyInfo) {
            HM._.hui.ShowMsgError("먼저 게임플레이 튜토리얼을 완료해주세요.");
            return;
        }

        HM._.SelectedStageIdx = Config.Stage.STG_GOBLIN_DUNGEON;
        Debug.Log($"OnClickGoblinDungeonStageBtn(SelectedStage= {HM._.SelectedStageIdx}):: diffIdx= {idxNum}");
        SM._.SfxPlay(SM.SFX.StageSelectSFX);
        ClearRewardInfoStagePlayBtn.gameObject.SetActive(false);
        ClearRewardInfoGoblinDungeonPlayBtn.gameObject.SetActive(true);
        ClearRewardInfoPopUp.SetActive(true);
        ClearRewardInfoBodyDOTAnim.DORestart();

        //* ステージ 難易度
        DM._.SelectedStageNum = (idxNum == 0)? Enum.StageNum.Stage_1
            : (idxNum == 1)? Enum.StageNum.Stage_2
            : Enum.StageNum.Stage_3;
        
        //* UI色
        ClearRewardInfoTopImg.color = TopColors[idxNum];
        ClearRewardInfoTopLabelOutsideImg.color = TopLabelOutsideColors[idxNum];
        ClearRewardInfoTopLabelInsideImg.color = TopLabelInsideColors[idxNum];
        ClearRewardInfoMiddleImg.color = MiddleColors[idxNum];
        ClearRewardInfoBgImg.color = BgColors[idxNum];

        ClearRewawrdInfoTitleTxt.text = "고블린던젼 클리어 보상";
        ClearRewardInfoStageNumTxt.text = $"1 - {idxNum + 1}";

        var rwDt = HM._.rwlm.RwdItemDt;
        RewardContentSO stgClrRwdDt = rwDt.Rwd_GoblinDungeonClearDts[idxNum];

        //* 固定の４つリワードアイコン
        ClearRewardInfoMiningRwdDetailTitleTxt.text = "고블린 종류";
        for(int i = 0; i < ClearRewardInfoCttQuantityTxts.Length; i++) {
            if(i == 0) // EXP
                ClearRewardInfoCttQuantityTxts[i].text = $"{stgClrRwdDt.ExpMax}";
            else if(i == 1) // COIN
                ClearRewardInfoCttQuantityTxts[i].text = $"{stgClrRwdDt.CoinMax}";
            else if(i == 2) { // MINING : GOBLIN
                FixedRwdIcon3Img.sprite = rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Goblin0].ItemImg;
                FixedRwdIcon3NameTxt.text = "고블린";
                ClearRewardInfoCttQuantityTxts[i].text = $"{stgClrRwdDt.GoblinMin}~{stgClrRwdDt.GoblinMax}";
            }
            else if(i == 3) { // CHEST GOLD
                FixedRwdIcon4Img.sprite = rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.ChestGold].ItemImg;
                ClearRewardInfoCttQuantityTxts[i].text = $"{stgClrRwdDt.ChestGoldMin}~{stgClrRwdDt.ChestGoldMax}";
            }
        }

        //* 習得できるGOBLIN Detail UI
        const int DETAIL_ICON_MAX_CNT = 4;
        const int OFFSET_GOBLIN_IDX = (int)Etc.NoshowInvItem.Goblin0;
        var goblinPercentList = stgClrRwdDt.RwdGradeTb.GoblinPerList;

        //* 初期化
        for(int i = 0; i < ClearRewardMiningItemIconUIs.Length; i++)
            ClearRewardMiningItemIconUIs[i].Obj.SetActive(false);

        //* Goblin アイコン
        int oreIconIdx = 0;
        for(int i = 0; i < goblinPercentList.Count; i++) {
            // 用意したIcon数を超えたら、For文終了
            if(oreIconIdx == DETAIL_ICON_MAX_CNT)
                break;

            // UI 設定
            if(goblinPercentList[i] != 0) {
                ItemSO goblinDt = rwDt.EtcNoShowInvDatas[i + OFFSET_GOBLIN_IDX];
                string name = goblinDt.Name.Split("고블린")[0]; //* 名前短縮
                ClearRewardMiningItemIconUIs[oreIconIdx++].SetUI(goblinDt.ItemImg, name);
            }
        }

        //* ボーナスリワード 非表示
        ClearRewardInfoBonusRewardTitleCntTxt.gameObject.SetActive(false);
        for(int i = 0; i < ClearRewardBonusItemIconUIs.Length; i++)
            ClearRewardBonusItemIconUIs[i].Obj.SetActive(false);
    }

    public void OnClickStageClearInfoPopUpCloseBtn() {
        SM._.SfxPlay(SM.SFX.ClickSFX);
        ClearRewardInfoPopUp.SetActive(false);
    }

#region FUNC
    public void UpdateStageUnlockSignGroup() {
        const int FIRST_STG = 0, SECOND_STG = 1, THIRD_STG = 2;
        const int STG_SIGN_ICON = 0, LOCK_ICON = 1;

        var selectStage = HM._.SelectedStageIdx;
        var isLockStage1_1 = DM._.DB.StageLockedDBs[selectStage].IsLockStage1_1;
        var isLockStage1_2 = DM._.DB.StageLockedDBs[selectStage].IsLockStage1_2;
        var isLockStage1_3 = DM._.DB.StageLockedDBs[selectStage].IsLockStage1_3;
        StageUnlockSigns[FIRST_STG].GetChild(STG_SIGN_ICON).gameObject.SetActive(!isLockStage1_1);
        StageUnlockSigns[FIRST_STG].GetChild(LOCK_ICON).gameObject.SetActive(isLockStage1_1);
        StageUnlockSigns[SECOND_STG].GetChild(STG_SIGN_ICON).gameObject.SetActive(!isLockStage1_2);
        StageUnlockSigns[SECOND_STG].GetChild(LOCK_ICON).gameObject.SetActive(isLockStage1_2);
        StageUnlockSigns[THIRD_STG].GetChild(STG_SIGN_ICON).gameObject.SetActive(!isLockStage1_3);
        StageUnlockSigns[THIRD_STG].GetChild(LOCK_ICON).gameObject.SetActive(isLockStage1_3);
    }
#endregion
#endregion
}
