using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour {
    //* Outside
    public TowerStateUIManager tsm;
    public EnemyStateUIManager esm;

    Coroutine CorMsgNoticeID;

    [Header("STATIC UI")]
    public Image playSpeedBtnImg;
    public TextMeshProUGUI playSpeedBtnTxt;
    public Sprite[] playSpeedBtnSprs;
    public Image StartBtnImg;
    public Image StartBtnLightImg;
    public TextMeshProUGUI StartBtnTxt;
    public Color[] StartBlueColors;
    public Color[] StartRedColors;

    [Tooltip("ゲーム状況により変わるUIグループ")]
    public Button ResetWallBtn;
    public TextMeshProUGUI ResetWallCntTxt;
    public Transform GameStateUIGroup;
    public TextMeshProUGUI WaveTxt;
    public TextMeshProUGUI EnemyCntTxt;
    public TextMeshProUGUI MoneyTxt;
    public TowerCardUI TowerCardUI;
    public Image HeartFillImg;
    public TextMeshProUGUI LifeTxt;

    [Header("NEXT ENEMY INFO FLAG")]
    public GameObject NextEnemyInfo;
    public Image NextEnemyImg;
    public Image NextEnemyTypeImg;
    public TextMeshProUGUI NextEnemyTxt;
    public Sprite[] NextEnemyTypeIconSprs;

    [Header("PAUSE POPUP")]
    public GameObject PausePopUp;
    public TextMeshProUGUI StageInfoTxt;
    private GameState previousState;
    private float previousTimeScale;

    [Header("GAMEOVER POPUP")]
    public GameObject GameoverPopUp;
    public Button Ads_ReviveBtn;

    [Header("VICTORY POPUP")]
    public GameObject VictoryPopUp;
    public Transform RewardsGroup;
    public Button Ads_ClaimX2Btn;

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
        WaveTxt.text = $"WAVE {GM._.WaveCnt} / {GM._.MaxWave}";
        ResetWallCntTxt.text = $"{GM._.ResetCnt}/{Config.DEFAULT_RESET_CNT}";
        EnemyCntTxt.text = "0 / 0";
        MoneyTxt.text = $"{GM._.Money}";
        LifeTxt.text = $"{GM._.Life}";
        Array.ForEach(TowerCardUI.PriceTxts, txt => txt.text = $"{TowerManager.CARD_UPG_PRICE_START}");
        UpdateTowerCardLvUI();
    }

#region EVENT
    #region DEBUG
    //! DEBUG トップのWAVEタイトルクリックすると、WAVE UP
    Coroutine CorIntervalClickID = null;
    public void Debug_WaveUp() => WaveUp();
    public void Debug_PointerDown_IntervalWaveUp_Off() => CorIntervalClickID = StartCoroutine(CoIntervalWaveUp());
    public void Debug_PointerUp_IntervalWaveUp_On() => StopCoroutine(CorIntervalClickID);
    IEnumerator CoIntervalWaveUp() { //* 日程間隔でWAVEアップ
        yield return Util.Time1;
        while(true) {
            yield return new WaitForSeconds(0.25f);
            WaveUp();
        }
    }
    private void WaveUp() {
        GM._.WaveCnt++;
        if(GM._.WaveCnt > GM._.MaxWave) GM._.WaveCnt = 0;
        WaveTxt.text = $"WAVE {GM._.WaveCnt} / {GM._.MaxWave}";
    }

    public void Debug_MeatUp() {
        GM._.Money += 20;
        MoneyTxt.text = $"{GM._.Money}";
    }
    #endregion

    public void OnClickPlaySpeedBtn() {
        Debug.Log($"OnClickPlaySpeedBtn()::");
        const int OFF = 0, ON = 1;
        var time = Time.timeScale;
        Time.timeScale = (time == 1)? 2 : (time == 2)? 4 : 1;
        playSpeedBtnImg.sprite = (Time.timeScale == 1)? playSpeedBtnSprs[OFF] : playSpeedBtnSprs[ON];
        playSpeedBtnTxt.text = $"X{Time.timeScale}";
    }

    public void OnClickResetWallBtn() {
        Debug.Log("OnClickResetRockBtn()");
        if(GM._.ResetCnt <= 0) {
            ShowMsgError("벽 리셋횟수가 다 소진되었습니다.");
            return;
        }
        
        GM._.ResetCnt--;
        ResetWallCntTxt.text = $"{GM._.ResetCnt}/{Config.DEFAULT_RESET_CNT}";
        GM._.tmc.SpawnWall();
    }

    #region POPUP
    //* PAUSE
    public void OnClickPauseBtn() {
        Pause();
        PausePopUp.SetActive(true);
    }
    public void OnClickPausePopUp_ContinueBtn() {
        Play();
        PausePopUp.SetActive(false);
    }
    public void OnClickPausePopUp_ExitGameBtn() {
        PausePopUp.SetActive(false);
        AgainAskPopUp.SetActive(true);
    }

    //* AGAIN ASK
    public void OnClickAgainAskPopUp_ConfirmBtn() {
        Debug.Log("GO TO HOME");
        Time.timeScale = 1;
        SceneManager.LoadScene(Enum.Scene.Home.ToString());
    }
    public void OnClickAgainAskPopUp_CloseBtn() {
        Time.timeScale = previousTimeScale;
        GM._.State = previousState;
        AgainAskPopUp.SetActive(false);
    }

    //* GAME OVER
    public void OnClickReTryBtn() {
        Time.timeScale = 1;
        SceneManager.LoadScene(Enum.Scene.Game.ToString());
    }
    public void OnClickAdsReviveBtn() {
        //TODO Request Revive Reward Ads
        Debug.Log("REVIVE Ads");
        Time.timeScale = 1;
        GM._.Life = GM._.MaxLife;
        LifeTxt.text = GM._.Life.ToString();
        HeartFillImg.fillAmount = 1;
        GameoverPopUp.SetActive(false);
        GM._.gef.ShowIconTxtEF(HeartFillImg.transform.position, GM._.MaxLife, "Heart");
    }

    //* VICTORY
    public void OnClickConfirmBtn() {
        //TODO Get Reward
        Debug.Log("GO TO HOME");
    }
    public void OnClickExitGameBtn() {
        Debug.Log("GO TO HOME");
    }
    public void OnClickAdsClaimX2Btn() {
        //TODO Request ClaimX2 Reward Ads
        Debug.Log("CLAIM X2 Ads");
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

        int price = TowerManager.CARD_UPG_PRICE_START + GM._.tm.TowerCardUgrLvs[kindIdx] * TowerManager.CARD_UPG_PRICE_UNIT;

        //* 費用チェック
        if(!GM._.CheckMoney(price)) return;

        //* アップグレード
        GM._.tm.UpgradeTowerCard(kind);
    }
    #endregion
#endregion

#region FUNC
    /// <summary>
    /// ゲームを停止状態にする
    /// </summary>
    public void Pause() {
        previousState = GM._.State;
        previousTimeScale = Time.timeScale;
        Time.timeScale = 0;
        GM._.State = GameState.Pause;
    }
    /// <summary>
    /// ゲームを開始状態に戻す
    /// </summary>
    public void Play() {
        Time.timeScale = previousTimeScale;
        GM._.State = previousState;
    }
    public void SetStartBtnUI(bool isReady) {
        const int FRAME = 0, FRAME_LIGHT = 1;
        StartBtnImg.color = isReady? StartRedColors[FRAME] : StartBlueColors[FRAME];
        StartBtnLightImg.color = isReady? StartRedColors[FRAME_LIGHT] : StartBlueColors[FRAME_LIGHT];
        StartBtnTxt.text = isReady? "시작하기" : "웨이브 준비";
    }
    public void InActiveResetWallBtn() {
        if(ResetWallBtn.gameObject.activeSelf)
            ResetWallBtn.gameObject.SetActive(false);
    }
    /// <summary> 上にへエラーメッセージバー表示（自動OFF）</summary>
    public void ShowMsgError(string msg) {
        StartCoroutine(CoShowMsgError(msg));
    }
    IEnumerator CoShowMsgError(string msg) {
        TopMsgError.SetActive(true);
        MsgErrorTxt.text = msg;
        yield return Util.RealTime1;
        TopMsgError.SetActive(false);
    }
    /// <summary> 上にへ情報メッセージバー表示（ON、OFF形式）</summary>
    public void ShowMsgInfo(bool isActive, string msg = "") {
        MsgInfoTxt.text = isActive? msg : "";
        TopMsgInfo.SetActive(isActive);
    }
    /// <summary> 下にお知らせメッセージ表示（自動OFF）</summary>
    public void ShowMsgNotice(string msg, int y = 350) {
        if(CorMsgNoticeID != null) StopCoroutine(CorMsgNoticeID);
        CorMsgNoticeID = StartCoroutine(CoShowMsgNotice(msg, y));
    }
    IEnumerator CoShowMsgNotice(string msg, int y) {
        BottomMsgNotice.SetActive(true);
        BottomMsgNotice.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, y);
        MsgNoticeTxt.text = msg;
        yield return Util.RealTime1;
        BottomMsgNotice.SetActive(false);
    }
    public bool ShowErrMsgCreateTowerAtPlayState() {
        if(GM._.State == GameState.Play) {
            ShowMsgError("몬스터가 있을때는 타워생성 및 업그레이드만 가능합니다!");
            return true;
        }
        return false;
    }
    public bool ShowErrMsgCCTowerLimit() {
        if(GM._.tm.CCTowerCnt >= GM._.tm.CCTowerMax) {
            ShowMsgError($"CC타워는 현재 {GM._.tm.CCTowerMax}개까지 가능합니다.");
            return true;
        }
        return false;
    }

    public void SwitchGameStateUI(GameState gameState) {
        GameStateUIGroup.GetChild((int)GameState.Ready).gameObject.SetActive(gameState == GameState.Ready);
        GameStateUIGroup.GetChild((int)GameState.Play).gameObject.SetActive(gameState == GameState.Play);
        NextEnemyInfo.SetActive(gameState == GameState.Ready);
    }

    public void UpdateTowerCardLvUI() {
        int PRICE = TowerManager.CARD_UPG_PRICE_START;
        int UNIT = TowerManager.CARD_UPG_PRICE_UNIT;
        var cardLvs = GM._.tm.TowerCardUgrLvs;

        for(int i = 0; i < TowerCardUI.Buttons.Length; i++) {
            TowerCardUI.LvTxts[i].text = $"LV {cardLvs[i]}";
            TowerCardUI.PriceTxts[i].text = $"{PRICE + (cardLvs[i] > 0 ? cardLvs[i] * UNIT : 0)}" ;

            if(cardLvs[i] >= TowerManager.CARD_UPG_LV_MAX) {
                TowerCardUI.PriceTxts[i].text = "MAX";
                TowerCardUI.Buttons[i].interactable = false;
            }
        }
    }

    public void SetNextEnemyInfoFlagUI() {
        EnemyData nextEnemyDt = GM._.GetNextEnemyData();
        if(nextEnemyDt == null) return; //* もう敵がなかったら、以下処理しない

        NextEnemyImg.sprite = nextEnemyDt.Spr;
        NextEnemyTypeImg.sprite = NextEnemyTypeIconSprs[(int)nextEnemyDt.Type];
        NextEnemyTxt.text = nextEnemyDt.Type == EnemyType.Boss? "BOSS" : "NEXT";
        NextEnemyTxt.color = nextEnemyDt.Type == EnemyType.Boss? Color.red : Color.white;
    }
#endregion
}
