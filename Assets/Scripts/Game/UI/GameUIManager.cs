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

    Coroutine CorMsgNoticeID;

    [Header("STATIC UI")]
    public Image playSpeedBtnImg;
    public Sprite[] playSpeedBtnSprs;

    [Tooltip("ゲーム状況により変わるUIグループ")]
    public Transform GameStateUIGroup;
    public TextMeshProUGUI StageTxt;
    public TextMeshProUGUI EnemyCntTxt;
    public TextMeshProUGUI MoneyTxt;
    public TextMeshProUGUI[] TowerUpgLvTxts;
    public Image HeartFillImg;
    public TextMeshProUGUI LifeTxt;

    [Header("PAUSE POPUP")]
    public GameObject PausePopUp;
    private GameState previousState;
    private float previousTimeScale;

    [Header("AGAIN ASK POPUP")]
    public GameObject AgainAskPopUp;
    public TextMeshProUGUI AgainAskMsgTxt;    

    [Header("ERROR MSG POPUP")]
    public GameObject TopMsgError;
    public TextMeshProUGUI MsgErrorTxt;

    [Header("INFO MSG POPUP")]
    public GameObject TopMsgInfo;
    public TextMeshProUGUI MsgInfoTxt;
    [Header("NOTICE MSG POPUP")]
    public GameObject BottomMsgNotice;
    public TextMeshProUGUI MsgNoticeTxt;

    void Awake() {
        tsm = GameObject.Find("TowerStateUIManager").GetComponent<TowerStateUIManager>();
        esm = GameObject.Find("EnemyStateUIManager").GetComponent<EnemyStateUIManager>();
    }

    void Start() {
        CorMsgNoticeID = null;
        TopMsgError.SetActive(false);
        StageTxt.text = $"STAGE {GM._.Stage} / {GM._.MaxStage}";
        EnemyCntTxt.text = "0 / 0";
        MoneyTxt.text = $"{GM._.Money}";
        UpdateTowerCardLvUI();
    }

#region EVENT
    public void OnClickPlaySpeedBtn() {
        Debug.Log($"OnClickPlaySpeedBtn()::");
        const int OFF = 0, ON = 1;
        var time = Time.timeScale;
        Time.timeScale = time == 1? 2 : 1;
        playSpeedBtnImg.sprite = time == 1? playSpeedBtnSprs[ON] : playSpeedBtnSprs[OFF];
    }

    #region POPUP
    //* PAUSE
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

    //* AGAIN ASK
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
    public void OnClickUpgradeTowerCard(int kindIdx) {
        //TODO if MONEY
        
        //* 型変換
        TowerKind kind = kindIdx == 0? TowerKind.Warrior
            : kindIdx == 1? TowerKind.Archer
            : kindIdx == 2? TowerKind.Magician
            : TowerKind.None;

        //* アップグレード
        GM._.tm.UpgradeTowerCard(kind);
    }
    #endregion
#endregion

#region FUNC
    /// <summary> 上にへエラーメッセージバー表示（自動OFF）</summary>
    public IEnumerator CoShowMsgError(string msg) {
        TopMsgError.SetActive(true);
        MsgErrorTxt.text = msg;
        yield return Util.Time1;
        TopMsgError.SetActive(false);
    }
    /// <summary> 上にへ情報メッセージバー表示（ON、OFF形式）</summary>
    public void ShowMsgInfo(bool isActive, string msg = "") {
        MsgInfoTxt.text = isActive? msg : "";
        TopMsgInfo.SetActive(isActive);
    }
    /// <summary> 下にお知らせメッセージ表示（自動OFF）</summary>
    public void ShowMsgNotice(string msg) {
        if(CorMsgNoticeID != null) StopCoroutine(CorMsgNoticeID);
        CorMsgNoticeID = StartCoroutine(CoShowMsgNotice(msg));
    }
    IEnumerator CoShowMsgNotice(string msg) {
        BottomMsgNotice.SetActive(true);
        MsgNoticeTxt.text = msg;
        yield return Util.Time1;
        BottomMsgNotice.SetActive(false);
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

    public void SwitchGameStateUI(GameState gameState) {
        GameStateUIGroup.GetChild((int)GameState.Ready).gameObject.SetActive(gameState == GameState.Ready);
        GameStateUIGroup.GetChild((int)GameState.Play).gameObject.SetActive(gameState == GameState.Play);
    }

    public void UpdateTowerCardLvUI() {
        for(int i = 0; i < TowerUpgLvTxts.Length; i++) {
            TowerUpgLvTxts[i].text = $"LV {GM._.tm.TowerCardUgrLvs[i]}";

            if(GM._.tm.TowerCardUgrLvs[i] >= TowerManager.CARD_UPG_LV_MAX) {
                TowerUpgLvTxts[i].text = "MAX";
                TowerUpgLvTxts[i].GetComponentInParent<Button>().interactable = false;
            }
        }
    }
#endregion
}
