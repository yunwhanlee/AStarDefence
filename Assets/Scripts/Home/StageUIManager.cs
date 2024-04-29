using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.PixelFantasy.PixelHeroes.Common.Scripts.UI;
using Inventory.Model;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageUIManager : MonoBehaviour {
    [Header("GOBLIN DUNGEON")]
    [field:SerializeField] public GameObject GoblinDungeonPopUp;
    [field:SerializeField] public TMP_Text GoldKeyTxt;
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
    [field:SerializeField] public TMP_Text[] StageNumBtnTxts;
    [field:SerializeField] public TMP_Text[] StageNumBtnHpRatioTxts;
    
    [field:SerializeField] public GameObject WholeLockedFrame;
    [field:SerializeField] public GameObject Stage1_2LockedFrame;
    [field:SerializeField] public GameObject Stage1_3LockedFrame;

    void Start() {
        DungeonAlertDot.SetActive(HM._.GoldKey > 0);
        NewStageAlertBtnObj.SetActive(false);

        //* New Stage Alert 表示
        for(int i = 0; i < DM._.DB.StageLockedDBs.Length; i++) {
            StageLockedDB stageDb = DM._.DB.StageLockedDBs[i];
            int stgNumIdx = Array.FindIndex(stageDb.StageRewards, stgRwd => stgRwd.IsUnlockAlert);
            if(stgNumIdx != -1) {
                int stageStr = i + 1;
                int stgNumStr = stgNumIdx + 1;

                //* お知らせアンロックのトリガー OFF
                stageDb.StageRewards[stgNumIdx].IsUnlockAlert = false;
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

#region GOBLIN DUNGEON EVENT 
    public void OnClickDungeonIconBtn() {
        SM._.SfxPlay(SM.SFX.ClickSFX);
        GoblinDungeonPopUp.SetActive(true);
        GoldKeyTxt.text = $"{HM._.GoldKey}/{Config.MAX_GOBLINKEY}";
    }

    public void OnClickDungeonDifficultyBtn(int diffIdx) {
        if(HM._.GoldKey <= 0) {
            HM._.hui.ShowMsgError("황금열쇠가 있어야 입장가능합니다.");
            return;
        }
        --HM._.GoldKey;

        DM._.SelectedStage = Config.GOBLIN_DUNGEON_STAGE;

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
            case 0:
                if(stgNum == 0)
                    return (commonChest.ItemImg, commonChest);
                else if(stgNum == 1)
                    return (equipChest.ItemImg, equipChest);
                else
                    return (goldChest.ItemImg, goldChest);
            case 1:
                if(stgNum == 0)
                    return (commonChest.ItemImg, commonChest);
                else if(stgNum == 1)
                    return (equipChest.ItemImg, equipChest);
                else
                    return (diamondChest.ItemImg, diamondChest);
            case 2:
                if(stgNum == 0)
                    return (goldChest.ItemImg, goldChest);
                else if(stgNum == 1)
                    return (equipChest.ItemImg, equipChest);
                else
                    return (equipChest.ItemImg, equipChest);
            case 3:
            case 4:
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
        HM._.rwm.UpdateInventory(rewardList);
    }
    public void OnClickNewStageAlertBtn() {
        Debug.Log($"OnClickNewStageAlertBtn():: NewStageAlertIdx= {StageAlertIdx}");
        HM._.SelectedStage = StageAlertIdx;
        HM._.hui.OnClickPlayBtn();
    }
    public void OnClickStartBtn() {
        var selectStage = HM._.SelectedStage;

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
    public void OnClickStageNumBtn(int stageNumIdx) {
        HM._.ivCtrl.CheckActiveClover();

        //* ホーム ➡ ゲームシーン移動の時、インベントリのデータを保存
        DM._.Save();

        SM._.SfxPlay(SM.SFX.StageSelectSFX);
        //* 難易度 データ設定
        DM._.SelectedStageNum = (stageNumIdx == 0)? Enum.StageNum.Stage1_1
            : (stageNumIdx == 1)? Enum.StageNum.Stage1_2
            : Enum.StageNum.Stage1_3;

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
        SM._.SfxPlay(SM.SFX.ClickSFX);
        if(SelectStageNumWindow.activeSelf) {
            SelectStageNumWindow.SetActive(false);
            return;
        }

        StageGroup.SetActive(false);
        GoblinDungeonPopUp.SetActive(false);
        Array.ForEach(StagePopUps, popUp => popUp.SetActive(false));
    }
#endregion
}
