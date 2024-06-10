using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
// using UnityEditor.SceneManagement;
// using UnityEditor.PackageManager.UI;

public class BossRewardUIManager : MonoBehaviour {

    [field: SerializeField] public GameObject WindowObj {get; set;}
    [field: SerializeField] public DOTweenAnimation DOTAnim {get; set;}
    [field: SerializeField] public TextMeshProUGUI SelectCntTxt {get; set;}
    [SerializeField] int selectCnt; public int SelectCnt {
        get => selectCnt;
        set {
            selectCnt = value;
            SelectCntTxt.text = $"선택횟수:{selectCnt}";
        }
    }

    [field: SerializeField] public TextMeshProUGUI[] CurrentCntTxts {get; private set;}

#region FUNC
    public void SetCurValueTxt(Enum.BossRwd enumIdx, int val) {
        string infoStr = "";
        switch(enumIdx) {
            case Enum.BossRwd.SuccessionTicket:       infoStr = $"( {val}개 소지 )"; break;
            case Enum.BossRwd.ChangeTypeTicket:       infoStr = $"( {val}개 소지 )"; break;
            case Enum.BossRwd.IncreaseLife:           infoStr = $"( 현재 HP{val} )"; break;
            case Enum.BossRwd.IncreaseCCTowerCnt:     infoStr = $"( 최대 {val}개 )"; break;
            case Enum.BossRwd.SwitchTowerPositionCnt: infoStr = $"( {val}개 소지 )"; break;
        }
        CurrentCntTxts[(int)enumIdx].text = infoStr;
    }
    public void Active(int cnt) {
        GM._.gui.Pause();
        WindowObj.SetActive(true);
        DOTAnim.DORestart();
        SelectCnt = cnt;
        
        //* 現在の数値 表示
        SetCurValueTxt(Enum.BossRwd.SuccessionTicket, GM._.actBar.SuccessionTicket);
        SetCurValueTxt(Enum.BossRwd.ChangeTypeTicket, GM._.actBar.ChangeTypeTicket);
        SetCurValueTxt(Enum.BossRwd.IncreaseLife, GM._.Life);
        SetCurValueTxt(Enum.BossRwd.IncreaseCCTowerCnt, GM._.tm.CCTowerMax);
        SetCurValueTxt(Enum.BossRwd.SwitchTowerPositionCnt, GM._.actBar.SwitchCnt);
    }

    private void UpdateData(string msg) {
        SM._.SfxPlay(SM.SFX.ItemPickSFX);
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
        UpdateData("계승권 획득");
    }
    public void OnClickTypeSuccessionInfoIconBtn() {
        Debug.Log("OnClickTypeSuccessionInfoIconBtn()::");
        TutoM._.ShowTutoPopUp(TutoM.ENEMY_IFNO, pageIdx: 1); //TutoM._.ShowEnemyInfoPopUp(page: 1);
    }
    /// <summary>タワーのタイプ変更券</summary>
    public void OnClickChangeTypeTicket() {
        Debug.Log("OnClickChangeTypeTicket()::");
        GM._.actBar.ChangeTypeTicket++;
        UpdateData("변경권 획득");
    }
    public void OnClickChangeTypeInfoIconBtn() {
        Debug.Log("OnClickChangeTypeInfoIconBtn()::");
        TutoM._.ShowTutoPopUp(TutoM.ENEMY_IFNO, pageIdx: 2); //TutoM._.ShowEnemyInfoPopUp(page: 2);
    }
    /// <summary>ライフの増加</summary>
    public void OnClickIncreaseLife() {
        Debug.Log("OnClickIncreaseLife()::");
        GM._.Life++;

        if(GM._.Life > Config.INC_MAX_LIFE) {
            GM._.Life--;
            GM._.gui.ShowMsgError("라이프가 최대입니다.");
            return;
        }
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
        UpdateData("CC타워 설치 수 +1 획득");
    }
    /// <summary>タワーの位置変更券</summary>
    public void OnClickSwitchTowerPositionCnt() {
        Debug.Log("OnClickSwitchTowerPositionCnt()::");
        GM._.actBar.SwitchCnt++;
        UpdateData("타워 위치변경 +1 획득");
    }
#endregion
}
