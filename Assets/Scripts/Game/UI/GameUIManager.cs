using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour {
    //* Outside
    public TowerStateUIManager tsm;
    public EnemyStateUIManager esm;

    [Header("ゲーム状況により変わるUIグループ")]
    public Transform GameStateUIGroup;

    [Header("ERROR MSG POPUP")]
    public GameObject topMsgError;
    public TextMeshProUGUI MsgErrorTxt;

    [Header("INFO MSG POPUP")]
    public GameObject topMsgInfo;
    public TextMeshProUGUI MsgInfoTxt;

    void Awake() {
        tsm = GameObject.Find("TowerStateUIManager").GetComponent<TowerStateUIManager>();
        esm = GameObject.Find("EnemyStateUIManager").GetComponent<EnemyStateUIManager>();
    }

    void Start() {
        topMsgError.SetActive(false);
    }

#region FUNC
    public IEnumerator CoShowMsgError(string msg) {
        topMsgError.SetActive(true);
        MsgErrorTxt.text = msg;
        yield return Util.time1;
        topMsgError.SetActive(false);
    }
    public bool ShowErrMsgCreateTowerAtPlayState() {
        if(GM._.state == GM.State.Play) {
            StartCoroutine(GM._.gui.CoShowMsgError("레이드 진행중에는 업그레이드만 가능합니다!"));
            return true;
        }
        return false;
    }
    public bool ShowErrMsgCCTowerLimit() {
        if(GM._.actBar.CCTowerCnt >= GM._.actBar.CCTowerMax) {
            StartCoroutine(GM._.gui.CoShowMsgError($"CC타워는 {GM._.actBar.CCTowerMax}개까지 가능합니다."));
            return true;
        }
        return false;
    }
    /// <summary>
    /// 情報メッセージ表示のポップアップ（ON、OFF形式）
    /// </summary>    
    public void ShowMsgInfo(bool isActive, string msg = "") {
        MsgInfoTxt.text = isActive? msg : "";
        topMsgInfo.SetActive(isActive);
    }

    public void SwitchGameStateUI(GM.State gameState) {
        GameStateUIGroup.GetChild((int)GM.State.Ready).gameObject.SetActive(gameState == GM.State.Ready);
        GameStateUIGroup.GetChild((int)GM.State.Play).gameObject.SetActive(gameState == GM.State.Play);
    }
#endregion
}
