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
        HM._.mtm.SliderBtn.SetActive(!IsLock);

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
    /// 採掘準備（ゴブリンと鉱石の配置状況）を確認
    /// </summary>
    public bool CheckSpotActive() {
        if(!GoblinSpotDt.IsActive) return false;
        if(!OreSpotDt.IsActive) return false;
        
        return true;
    }

    public IEnumerator CoTimerStart(bool isSwitchOre = false, bool isPassedTime = false) {
        //* ゴブリンと鉱石のレベルによる、速度と時間を適用する変数用意
        int gblLv = GoblinSpotDt.LvIdx;
        int oreLv = OreSpotDt.LvIdx;
        float spdPer = HM._.mnm.GoblinDataSO.Datas[gblLv].SpeedPer;
        int time = HM._.mnm.OreDataSO.Datas[oreLv].TimeSec;

        //* ゴブリンのMiningSpeed％ 適用
        // int decSec = Mathf.RoundToInt(time * (spdPer - 1));
        // time -= decSec;

        //* 残る時間（保存した時間が０ではなかったら、まだ仕事が進んでいる中）
        MiningMax = (isSwitchOre || MiningMax == 0)? time : MiningMax;
        MiningTime = (isSwitchOre || MiningTime == 0)? time : MiningTime;

        //* アプリを再起動して、経過時間を減る
        if(isPassedTime) {
            int passedSec = DM._.PassedSec;
            string timeFormat = Util.ConvertTimeFormat(passedSec);
            // int extraDecSec = Mathf.RoundToInt(passedSec * (spdPer - 1));
            // string extraDecPer = $"<color=yellow>{extraDecSec}초 추가감소({(spdPer - 1) * 100}%)</color>";
            HM._.hui.ShowMsgNotice($"{timeFormat}초가 경과했습니다. ", Util.Time3);
            MiningTime -= Mathf.RoundToInt(passedSec * spdPer);
        }

        Debug.Log($"CoTimerStart():: goblin SpdPer= {spdPer}, time= {time}, "); //decSec= {decSec}");

        Sprite[] oreSprs = HM._.mnm.OreDataSO.Datas[oreLv].Sprs;
        HM._.mtm.SetTimer(isOn: true);
        HM._.wsm.GoblinChrCtrl.MiningAnim(gblLv);

        WaitForSeconds waitTime = (gblLv == 0)? Util.Time1
            : (gblLv == 1)? Util.Time0_95 : (gblLv == 2)? Util.Time0_9 : (gblLv == 3)? Util.Time0_8
            : (gblLv == 4)? Util.Time0_7 : (gblLv == 5)? Util.Time0_6 : Util.Time0_5;


        //* タイマー開始
        while(0 < MiningTime) {
            Debug.Log($"curWS.Id= {Id}, time= {MiningTime} / {MiningMax}");

            //* 仕事がもう終わったら、ループをすぐ出る
            if(IsFinishWork) {
                break;
            }

            //* 時間表示
            MiningTime -= 1;
            string timeFormat = Util.ConvertTimeFormat(MiningTime);

            //* 現在見ているWorkSpaceページだけ 最新化
            if(Id == HM._.wsm.CurIdx) {
                // スライダー UI
                HM._.mtm.SetTimerSlider(timeFormat, (float)(MiningMax - MiningTime) / MiningMax);

                // ORE 壊れるイメージ変更
                HM._.wsm.OreSpot.OreImg.sprite = oreSprs[
                    MiningTime < (MiningMax * 0.3f)? (int)ORE_SPRS.PIECE
                    : MiningTime <= (MiningMax * 0.6f)? (int)ORE_SPRS.HALF
                    : (int)ORE_SPRS.DEF
                ];
            }
            yield return waitTime;
        }

        //* 採掘完了！
        IsFinishWork = true;
        FinishWork();
    }

    /// <summary>
    /// 採掘がおわったこと確認出来たら、完了作業を実行
    /// </summary>
    public void FinishWork() {
        //* 現在見ているWorkSpaceではないと、実行しない
        if(Id != HM._.wsm.CurIdx) return;
        Debug.Log($"FinishWork():: WorkSpace Id= {Id}");

        if(IsFinishWork) {
            Debug.Log($"FinishWork():: Award Reward");
            MiningMax = 0;
            MiningTime = 0;
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