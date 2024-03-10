using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager.UI;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class BossRewardUIManager : MonoBehaviour {
    [field: SerializeField] public GameObject WindowObj {get; set;}
    [field: SerializeField] public int SelectCnt {get; set;}

    [field: SerializeField] public TextMeshProUGUI SelectCntTxt {get; set;}


    void Start() {
        // GM._.State = GameState.Pause;
        SelectCnt = 5;
        SelectCntTxt.text = $"선택횟수:{SelectCnt}";
    }

#region FUNC
    public void Active(int selectCnt) {
        GM._.gui.Pause();
        WindowObj.SetActive(true);
        SelectCnt = selectCnt;
    }
    private void UpdateData() {
        SelectCnt--;
        SelectCntTxt.text = $"선택횟수:{SelectCnt}";
        

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
        UpdateData();
    }
    /// <summary>タワーのタイプ変更券</summary>
    public void OnClickChangeTypeTicket() {
        Debug.Log("OnClickChangeTypeTicket()::");
        GM._.actBar.ChangeTypeTicket++;
        UpdateData();
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

        UpdateData();
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

        UpdateData();
    }
    /// <summary>タワーの位置変更券</summary>
    public void OnClickSwitchTowerPositionCnt() {
        Debug.Log("OnClickSwitchTowerPositionCnt()::");
        GM._.actBar.SwitchCnt++;

        UpdateData();
    }
#endregion
}
