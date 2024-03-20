using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
// using UnityEditor.SceneManagement;
// using UnityEditor.PackageManager.UI;

public class BossRewardUIManager : MonoBehaviour {
    [field: SerializeField] public GameObject WindowObj {get; set;}

    [SerializeField] int selectCnt; public int SelectCnt {
        get => selectCnt;
        set {
            selectCnt = value;
            SelectCntTxt.text = $"선택횟수:{selectCnt}";
        }
    }

    [field: SerializeField] public TextMeshProUGUI SelectCntTxt {get; set;}

#region FUNC
    public void Active(int cnt) {
        GM._.gui.Pause();
        WindowObj.SetActive(true);
        SelectCnt = cnt;
    }
    private void UpdateData(string msg) {
        SelectCnt--;

        //* 習得メッセージ表示
        GM._.gui.ShowMsgNotice(msg);

        //* 選択券全て消費したら、終了
        if(SelectCnt <= 0) {
            GM._.gui.Play();
            WindowObj.SetActive(false);
        }
    }
#endregion

#region EVENT
    /// <summary>タワーのタイプ継承券</summary>
    public void OnClickTypeSuccessionTicket() {
        Debug.Log("OnClickTypeSuccessionTicket()::");
        GM._.actBar.SuccessionTicket++;
        GM._.actBar.SuccessionTicketCntTxt.text = $"{GM._.actBar.SuccessionTicket}";
        UpdateData("계승권 획득");
    }
    /// <summary>タワーのタイプ変更券</summary>
    public void OnClickChangeTypeTicket() {
        Debug.Log("OnClickChangeTypeTicket()::");
        GM._.actBar.ChangeTypeTicket++;
        GM._.actBar.ChangeTypeTicketCntTxt.text = $"{GM._.actBar.ChangeTypeTicket}";
        UpdateData("변경권 획득");
    }
    /// <summary>ライフの増加</summary>
    public void OnClickIncreaseLife() {
        Debug.Log("OnClickIncreaseLife()::");
        GM._.Life++;

        if(GM._.Life > GM._.MaxLife) {
            GM._.Life--;
            GM._.gui.ShowMsgError("라이프가 최대입니다.");
            return;
        }

        GM._.gui.LifeTxt.text = $"{GM._.Life}";
        UpdateData("라이프 +1 획득");
    }
    /// <summary>CCタワー数の増加</summary>
    public void OnClickIncreaseCCTowerCnt() {
        Debug.Log("OnClickIncreaseCCTowerCnt()::");
        GM._.tm.CCTowerMax++;

        if(GM._.tm.CCTowerMax > TowerManager.CC_TOWER_INC_LIMIT_MAX) {
            GM._.tm.CCTowerMax--;
            GM._.gui.ShowMsgError("CC타워 설치개수가 최대입니다.");
            return;
        }

        GM._.actBar.CCTowerCntTxt.text = $"CC : {GM._.tm.CCTowerMax}";
        UpdateData("CC타워 설치 수 +1 획득");
    }
    /// <summary>タワーの位置変更券</summary>
    public void OnClickSwitchTowerPositionCnt() {
        Debug.Log("OnClickSwitchTowerPositionCnt()::");
        GM._.actBar.SwitchCnt++;
        GM._.actBar.SwitchCntTxt.text = $"{GM._.actBar.SwitchCnt}";
        UpdateData("타워 위치변경 +1 획득");
    }
#endregion
}
