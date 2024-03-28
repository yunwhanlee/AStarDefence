using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

[Serializable]
/// <summary>
/// GoblinとかOreのスポットの情報
/// </summary>
public struct SpotData {
    public bool IsActive; // 活性化
    public int LvIdx; // 配置したGoblinとかOreのレベル

    public SpotData(bool isActive, int lvIdx) {
        IsActive = isActive;
        LvIdx = lvIdx;
    }

#region FUNC
    public void Init() {
        IsActive = false;
        LvIdx = -1;
    }
    public void SetData(bool isActive , int lvIdx) {
        IsActive = isActive;
        LvIdx = lvIdx;
    }
#endregion
}

[Serializable]
public class WorkSpace {
    public int Id;
    public bool IsLock;
    [SerializeField] bool isFinishWork; public bool IsFinishWork { //* 採掘完了のトリガー
        get => isFinishWork;
        set {
            isFinishWork = value;
            if(value == true) {
                SM._.SfxPlay(SM.SFX.WaveStartSFX);
                HM._.hui.ShowMsgNotice($"작업장{Id + 1}의 채광이 완료되었습니다!");
            }
        }
    } 
    public SpotData GoblinSpotDt;
    public SpotData OreSpotDt;
    public int MiningMax;
    public int MiningTime;
    public Coroutine CorTimerID;

    public WorkSpace(int id, bool isLock, int lvIdx) {
        Id = id;
        IsLock = isLock;
        GoblinSpotDt = new SpotData(false, lvIdx);
        OreSpotDt = new SpotData(false, lvIdx);
    }

    /// <summary>
    /// UI最新化（現在状況に関して）
    /// </summary>
    public void UpdateUI(Transform workAreaTf, int price = -1) {
        //* 作業場（アンロック Or Not）
        const int WORK_SPOT = 0, PURCHASE_BTN = 1;
        var workSpotGroupObj = workAreaTf.GetChild(WORK_SPOT).gameObject;
        var purchaseBtnObj = workAreaTf.GetChild(PURCHASE_BTN).gameObject;
        workSpotGroupObj.SetActive(!IsLock);
        purchaseBtnObj.SetActive(IsLock);

        //* アンロックされたら、値段表示
        if(IsLock && price != -1)
        purchaseBtnObj.GetComponentInChildren<TextMeshProUGUI>().text = $"{price}";

        //* 現在採掘中なら
        if(GoblinSpotDt.IsActive && OreSpotDt.IsActive) {
            //* ゴブリンのMiningアニメー
            if(HM._.wsm.GoblinSpot.DisplayObj.activeSelf) {
                int goblinLvIdx = HM._.wsm.CurWorkSpace.GoblinSpotDt.LvIdx;
                HM._.wsm.GoblinChrCtrl.MiningAnim(goblinLvIdx);
            }
            //* スライダーUI最新化
            HM._.mtm.SetTimer(isOn: true);
        }
        else 
            HM._.mtm.InitSlider();

        FinishWork();
    }

    /// <summary>
    /// 採掘開始(Mining)
    /// </summary>
    /// <param name="idx">現在表示しているWorkSpaceIdx</param>
    /// <returns>OREのレベルによって、掛かる時間を返す(-1なら、Falseという意味)</returns>
    public bool StartMining(int idx) {
        if(!GoblinSpotDt.IsActive) return false;
        if(!OreSpotDt.IsActive) return false;
        
        //* タイマー保存
        TimeSpan timestamp = DateTime.UtcNow - new DateTime(1970,1,1,0,0,0);
        string key = $"WorkSpace{idx + 1}";

        //* 過去に保存した時間（無かったら、現在の時間に初期化）
        int past = PlayerPrefs.GetInt(key, defaultValue: (int)timestamp.TotalSeconds);

        //* WorkSpace{N}キー 時間を最新化
        PlayerPrefs.SetInt(key, (int)timestamp.TotalSeconds);

        int passedSec = (int)timestamp.TotalSeconds - past;
        Debug.Log("StartMining():: passedSec=> " + passedSec);

        // HM._.wsm.GoblinChrCtrl.GoblinMiningAnim();
        return true;
    }

    public IEnumerator CoTimerStart() {
        WorkSpace curWS = HM._.wsm.CurWorkSpace;

        //* ゴブリンと鉱石のレベルによる、速度と時間を適用する変数用意
        int goblinLvIdx = curWS.GoblinSpotDt.LvIdx;
        int oreLvIdx = curWS.OreSpotDt.LvIdx;
        float spdPer = HM._.mnm.GoblinDataSO.Datas[goblinLvIdx].SpeedPer;
        int time = HM._.mnm.OreDataSO.Datas[oreLvIdx].TimeSec;

        //* ゴブリンのMiningSpeed％ 適用
        int decSec = Mathf.RoundToInt(time * (spdPer - 1));
        time -= decSec;

        //* WorkSpace毎に時間データ 設定
        curWS.MiningMax = time;
        curWS.MiningTime = time;
        Debug.Log($"CoTimerStart():: goblin SpdPer= {spdPer}, time= {time}, decSec= {decSec}");

        Sprite[] oreSprs = HM._.mnm.OreDataSO.Datas[oreLvIdx].Sprs;
        HM._.mtm.SetTimer(isOn: true);
        HM._.wsm.GoblinChrCtrl.MiningAnim(goblinLvIdx);

        //* タイマー開始
        while(0 < curWS.MiningTime) {
            // Debug.Log($"curWS.Id= {curWS.Id}, time= {curWS.MiningTime} / {curWS.MiningMax}");
            //* 時間表示
            curWS.MiningTime -= 1;
            int sec = curWS.MiningTime % 60;
            int min = curWS.MiningTime / 60;
            int hour = min / 60;
            string hourStr = (hour == 0)? "" : $"{hour:00} : ";

            //* 現在見ているWorkSpaceページだけ 最新化
            if(Id == HM._.wsm.CurIdx) {
                // スライダー UI
                HM._.mtm.SetTimerSlider($"{hourStr} {min:00} : {sec:00}", (float)(curWS.MiningMax - curWS.MiningTime) / curWS.MiningMax);

                // ORE 壊れるイメージ変更
                HM._.wsm.OreSpot.OreImg.sprite = oreSprs[
                    curWS.MiningTime < (curWS.MiningMax * 0.3f)? (int)ORE_SPRS.PIECE
                    : curWS.MiningTime <= (curWS.MiningMax * 0.6f)? (int)ORE_SPRS.HALF
                    : (int)ORE_SPRS.DEF
                ];
            }

            yield return Util.Time1;
        }

        //* リワード受け取れる
        IsFinishWork = true;
        FinishWork();
    }

    /// <summary>
    /// 採掘がおわったこと確認出来たら、完了作業を実行
    /// </summary>
    public void FinishWork() {
        //* 現在見ているWorkSpaceではないと、実行しない
        if(Id != HM._.wsm.CurIdx) return;

        if(IsFinishWork) {
            HM._.wsm.OreSpot.OreImg.sprite = HM._.rwm.PresentSpr;
            HM._.mtm.RewardAuraEF.SetActive(true);
            HM._.mtm.SetTimerSlider("보상받기", 1);
            HM._.wsm.GoblinChrCtrl.HappyAnim();
        }
        else {
            HM._.mtm.RewardAuraEF.SetActive(false);
        }
    }
}