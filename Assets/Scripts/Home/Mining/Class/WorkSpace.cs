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
    public bool IsLock;
    public SpotData GoblinSpotDt;
    public SpotData OreSpotDt;

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
        int goblinLvIdx = HM._.wsm.CurWorkSpace.GoblinSpotDt.LvIdx;
        int oreLvIdx = HM._.wsm.CurWorkSpace.OreSpotDt.LvIdx;

        float spdPer = HM._.mnm.GoblinDataSO.Datas[goblinLvIdx].SpeedPer;
        int time = HM._.mnm.OreDataSO.Datas[oreLvIdx].TimeSec;

        //* ゴブリンのMiningSpeed％ 適用
        int decSec = Mathf.RoundToInt(time * (spdPer - 1));
        time -= decSec;
        int max = time;
        Debug.Log($"CoTimerStart():: goblin SpdPer= {spdPer}, time= {time}, decSec= {decSec}");

        Sprite[] oreSprs = HM._.mnm.OreDataSO.Datas[oreLvIdx].Sprs;
        HM._.mtm.SetTimer(isOn: true);
        HM._.wsm.GoblinChrCtrl.MiningAnim(goblinLvIdx);

        while(0 < time) {
            //* 時間表示
            time -= 1;
            int sec = time % 60;
            int min = time / 60;
            int hour = min / 60;
            string hourStr = (hour == 0)? "" : $"{hour:00} : ";
            HM._.mtm.SetTimerSlider($"{hourStr} {min:00} : {sec:00}", (float)(max - time) / max);

            //* ORE 壊れるイメージ変更
            HM._.wsm.OreSpot.OreImg.sprite = oreSprs[
                time < (max * 0.3f)? (int)ORE_SPRS.PIECE
                : time <= (max * 0.6f)? (int)ORE_SPRS.HALF
                : (int)ORE_SPRS.DEF
            ];

            yield return Util.RealTime1;
        }

        //* リワード受け取れる
        HM._.wsm.OreSpot.OreImg.sprite = HM._.rwm.PresentSpr;
        HM._.mtm.IsFinish = true;
        HM._.mtm.RewardAuraEF.SetActive(true);
        HM._.mtm.SetTimerSlider("보상받기", 1);
        HM._.wsm.GoblinChrCtrl.GoblinHappyAnim();
    }
}