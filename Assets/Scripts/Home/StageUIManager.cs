using System;
using System.Collections;
using System.Collections.Generic;
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

    [Header("STAGE")]
    [field:SerializeField] public int NewStageAlertIdx;
    [field:SerializeField] public GameObject NewStageAlertBtnObj;
    [field:SerializeField] public Sprite[] MapIconSprs;
    [field:SerializeField] public Image NewStageAlertMapImg;
    [field:SerializeField] public TMP_Text NewStageAlertTxt;

    [field:SerializeField] public GameObject StageGroup;
    [field:SerializeField] public GameObject[] StagePopUps;

    [field:SerializeField] public GameObject DifficultyWindow;
    [field:SerializeField] public GameObject WholeLockedFrame;
    [field:SerializeField] public GameObject NormalLockedFrame;
    [field:SerializeField] public GameObject HardLockedFrame;

    void Start() {
        DungeonAlertDot.SetActive(HM._.GoldKey > 0);
        NewStageAlertBtnObj.SetActive(false);

        //* New Stage Alert 表示
        for(int i = 0; i < DM._.DB.StageLockedDBs.Length; i++) {
            StageLockedDB stageDb = DM._.DB.StageLockedDBs[i];
            if(stageDb.IsUnlockAlert) {
                int nextStageIdx = i + 1;
                NewStageAlertIdx = i;
                NewStageAlertBtnObj.SetActive(true);
                stageDb.IsUnlockAlert = false;
                HM._.hui.ShowMsgNotice($"축하합니다! 스테이지{nextStageIdx}가 열렸습니다!");
                NewStageAlertMapImg.sprite = MapIconSprs[i];
                NewStageAlertTxt.text = $"스테이지{nextStageIdx} 플레이 가능";
            }
        }
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
    public void OnClickNewStageAlertBtn() {
        Debug.Log($"OnClickNewStageAlertBtn():: NewStageAlertIdx= {NewStageAlertIdx}");
        HM._.SelectedStage = NewStageAlertIdx;
        HM._.hui.OnClickPlayBtn();
    }
    public void OnClickStartBtn() {
        SM._.SfxPlay(SM.SFX.ClickSFX);
        DifficultyWindow.SetActive(true);
        //* Diff Btns
        NormalLockedFrame.SetActive(DM._.DB.StageLockedDBs[HM._.SelectedStage].IsLockStage1_2);
        HardLockedFrame.SetActive(DM._.DB.StageLockedDBs[HM._.SelectedStage].IsLockStage1_3);

        DM._.SelectedStage = HM._.SelectedStage;
    }

    public void OnClickDifficultyBtn(int diffIdx) {
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
        if(DifficultyWindow.activeSelf) {
            DifficultyWindow.SetActive(false);
            return;
        }

        StageGroup.SetActive(false);
        GoblinDungeonPopUp.SetActive(false);
        Array.ForEach(StagePopUps, popUp => popUp.SetActive(false));
    }
#endregion
}
