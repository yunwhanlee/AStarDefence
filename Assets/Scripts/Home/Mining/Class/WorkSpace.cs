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
        //* 両方活性化しないと、以下処理しない
        // if(!GoblinSpotDt.IsActive || !OreSpotDt.IsActive) {
        //     HM._.wsm.GoblinChrCtrl.GoblinStopMiningAnim();
        //     HM._.wsm.GoblinChrCtrl.SpawnAnim();
        //     return false;
        // }

        if(!GoblinSpotDt.IsActive) return false;
        if(!OreSpotDt.IsActive) return false;
        
        //* タイマー保存
        TimeSpan timestamp = DateTime.UtcNow - new DateTime(1970,1,1,0,0,0);
        string key = $"WorkSpace{idx + 1}";
        int past = PlayerPrefs.GetInt(key, (int)timestamp.TotalSeconds);
        PlayerPrefs.SetInt(key, (int)timestamp.TotalSeconds);
        int passedSec = (int)timestamp.TotalSeconds - past;
        Debug.Log("passedSec=> " + passedSec);

        // HM._.wsm.GoblinChrCtrl.GoblinMiningAnim();

        return true;
    }

    public IEnumerator CoTimerStart(int time) {
        int cnt = 0;
        while(cnt < time) {
            yield return new WaitForSecondsRealtime(1);
            cnt++;
            HM._.wsm.SetTimerSlider($"{cnt}", (float)cnt / time);
        }
    }
}