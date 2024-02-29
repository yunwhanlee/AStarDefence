using System;
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

    [Header("STATIC UI")]
    public Image playSpeedBtnImg;
    public Sprite[] playSpeedBtnSprs;

    [Tooltip("ゲーム状況により変わるUIグループ")]
    public Transform GameStateUIGroup;
    public TextMeshProUGUI StageTxt;
    public TextMeshProUGUI EnemyCntTxt;
    public TextMeshProUGUI MoneyTxt;
    public TextMeshProUGUI[] TowerUpgLvTxts;

    [Header("PAUSE POPUP")]
    public GameObject PausePopUp;
    private GameState previousState;
    private float previousTimeScale;

    [Header("AGAIN ASK POPUP")]
    public GameObject AgainAskPopUp;
    public TextMeshProUGUI AgainAskMsgTxt;    

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
        StageTxt.text = $"STAGE {GM._.Stage}";
        EnemyCntTxt.text = "0 / 0";
        MoneyTxt.text = $"{GM._.Money}";

        for(int i = 0; i < TowerUpgLvTxts.Length; i++) 
            TowerUpgLvTxts[i].text = $"LV {GM._.tm.TowerCardUgrLvs[i]}";
    }

#region EVENT
    public void OnClickPlaySpeedBtn() {
        Debug.Log($"OnClickPlaySpeedBtn()::");
        const int OFF = 0, ON = 1;
        var time = Time.timeScale;
        Time.timeScale = time == 1? 2 : 1;
        playSpeedBtnImg.sprite = time == 1? playSpeedBtnSprs[ON] : playSpeedBtnSprs[OFF];
    }
    #region PAUSE POPUP
    public void OnClickPauseBtn() {
        previousState = GM._.State;
        previousTimeScale = Time.timeScale;
        Time.timeScale = 0;
        GM._.State = GameState.Pause;
        PausePopUp.SetActive(true);
    }
    public void OnClickPausePopUp_ContinueBtn() {
        Time.timeScale = previousTimeScale;
        GM._.State = previousState;
        PausePopUp.SetActive(false);
    }
    public void OnClickPausePopUp_ExitGameBtn() {
        PausePopUp.SetActive(false);
        AgainAskPopUp.SetActive(true);
    }
    #endregion
    #region AGAIN ASK POPUP
    public void OnClickAgainAskPopUp_ConfirmBtn() {
        Debug.Log("GO TO HOME");
    }
    public void OnClickAgainAskPopUp_CloseBtn() {
        Time.timeScale = previousTimeScale;
        GM._.State = previousState;
        AgainAskPopUp.SetActive(false);
    }
    #endregion
    #region UPGRADE TOWER CARDS
    public void OnClickUpgradeTowerCard(TowerKind kind) {
        //TODO if MONEY

        Debug.Log($"OnClickUpgradeTowerCard({kind}):: UPGRADE!");
        switch(kind) {
            case TowerKind.Warrior:
                break;
            case TowerKind.Archer:

                break;
            case TowerKind.Magician:

                break;
        }
    }
    #endregion
#endregion

#region FUNC
    public IEnumerator CoShowMsgError(string msg) {
        topMsgError.SetActive(true);
        MsgErrorTxt.text = msg;
        yield return Util.time1;
        topMsgError.SetActive(false);
    }
    public bool ShowErrMsgCreateTowerAtPlayState() {
        if(GM._.State == GameState.Play) {
            StartCoroutine(GM._.gui.CoShowMsgError("몬스터가 있을때는 타워생성 및 업그레이드만 가능합니다!"));
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

    public void SwitchGameStateUI(GameState gameState) {
        GameStateUIGroup.GetChild((int)GameState.Ready).gameObject.SetActive(gameState == GameState.Ready);
        GameStateUIGroup.GetChild((int)GameState.Play).gameObject.SetActive(gameState == GameState.Play);
    }
#endregion
}
