using System;
using System.Collections;
using System.Collections.Generic;
using Assets.PixelFantasy.PixelHeroes.Common.Scripts.UI;
using Inventory.Model;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageUIManager : MonoBehaviour {
    [Header("GOBLIN DUNGEON")]
    [field:SerializeField] public GameObject GoblinDungeonPopUp;

    [Header("STAGE")]
    [field:SerializeField] public GameObject StageGroup;
    [field:SerializeField] public GameObject[] StagePopUps;

    [field:SerializeField] public GameObject DifficultyWindow;
    [field:SerializeField] public GameObject WholeLockedFrame;
    [field:SerializeField] public GameObject NormalLockedFrame;
    [field:SerializeField] public GameObject HardLockedFrame;

#region GOBLIN DUNGEON EVENT 
    public void OnClickDungeonIconBtn() {
        SM._.SfxPlay(SM.SFX.ClickSFX);
        GoblinDungeonPopUp.SetActive(true);
    }

    public void OnClickDungeonDifficultyBtn(int diffIdx) {
        DM._.SelectedStage = 5;

        HM._.ivCtrl.CheckActiveClover();

        //* ホーム ➡ ゲームシーン移動の時、インベントリのデータを保存
        DM._.Save();

        SM._.SfxPlay(SM.SFX.StageSelectSFX);

        //* 難易度 データ設定
        DM._.SelectedDiff = (diffIdx == 0)? Enum.Difficulty.Easy
            : (diffIdx == 1)? Enum.Difficulty.Normal
            : Enum.Difficulty.Hard;

        //* ➡ ゲームシーンロード
        SceneManager.LoadScene(Enum.Scene.Game.ToString());
    }

#endregion

#region STAGE EVENT
    public void OnClickStartBtn() {
        SM._.SfxPlay(SM.SFX.ClickSFX);
        DifficultyWindow.SetActive(true);
        //* Diff Btns
        NormalLockedFrame.SetActive(DM._.DB.StageLockedDBs[HM._.SelectedStage].IsLockNormal);
        HardLockedFrame.SetActive(DM._.DB.StageLockedDBs[HM._.SelectedStage].IsLockHard);

        DM._.SelectedStage = HM._.SelectedStage;
    }

    public void OnClickDifficultyBtn(int diffIdx) {
        HM._.ivCtrl.CheckActiveClover();

        //* ホーム ➡ ゲームシーン移動の時、インベントリのデータを保存
        DM._.Save();

        SM._.SfxPlay(SM.SFX.StageSelectSFX);
        //* 難易度 データ設定
        DM._.SelectedDiff = (diffIdx == 0)? Enum.Difficulty.Easy
            : (diffIdx == 1)? Enum.Difficulty.Normal
            : Enum.Difficulty.Hard;

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
        WholeLockedFrame.SetActive(DM._.DB.StageLockedDBs[hm.SelectedStage].IsLockEasy);
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
