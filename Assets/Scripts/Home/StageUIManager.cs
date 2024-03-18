using System;
using System.Collections;
using System.Collections.Generic;
using Assets.PixelFantasy.PixelHeroes.Common.Scripts.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageUIManager : MonoBehaviour {
    [field:SerializeField] public GameObject StageGroup;
    [field:SerializeField] public GameObject[] StagePopUps;

    [field:SerializeField] public GameObject DifficultyWindow;
    [field:SerializeField] public GameObject WholeLockedFrame;
    [field:SerializeField] public GameObject NormalLockedFrame;
    [field:SerializeField] public GameObject HardLockedFrame;

    void Start() {

    }

#region EVENT
    public void OnClickStartBtn() {
        DifficultyWindow.SetActive(true);
        NormalLockedFrame.SetActive(DM._.DB.StageLockedDatas[HM._.SelectedStage].Normal);
        HardLockedFrame.SetActive(DM._.DB.StageLockedDatas[HM._.SelectedStage].Hard);
    }

    public void OnClickDifficultyBtn(int diffIdx) {
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
        WholeLockedFrame.SetActive(DM._.DB.StageLockedDatas[hm.SelectedStage].Easy);
    }
    public void OnClickBackBtn() { //* 閉じるボタン
        if(DifficultyWindow.activeSelf) {
            DifficultyWindow.SetActive(false);
            return;
        }

        StageGroup.SetActive(false);
        Array.ForEach(StagePopUps, popUp => popUp.SetActive(false));
    }
#endregion
}
