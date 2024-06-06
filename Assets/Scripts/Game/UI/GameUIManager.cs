using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour {
    public Action OnClickAskConfirmAction = () => {};  //* もう一度聞く Confirmボタンイベント
    public Action OnClickAskCloseAction = () => {}; //* もう一度聞く Closeボタンイベント

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
    public GameObject ReviveSpawnUIEF;

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
    [SerializeField] private GameState previousState;
    [SerializeField] private float previousTimeScale;

    [Header("GAMEOVER POPUP")]
    public GameObject GameoverPopUp;
    public TMP_Text GameoverExitBtnTxt;
    public Button Ads_ReviveBtn;
    public Button RetryBtn;

    [Header("VICTORY POPUP")]
    public GameObject VictoryPopUp;
    public TMP_Text VictoryTitleTxt;
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
        previousState = GameState.Ready;
        previousTimeScale = 1;
        TopMsgError.SetActive(false);
        ResetWallBtn.gameObject.SetActive(true);
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

    /// <summary> もう一度確認するPOPUP：★OnClickAskConfirmActionへ確認ボタン押してから、処理するメソッドを購読すること！</summary>
    public void ShowAgainAskMsg(string msg = "") {
        AgainAskPopUp.SetActive(true);
        AgainAskMsgTxt.text = msg;
    }

    public void OnClickAgainAskPopUp_ConfirmBtn() {
        OnClickAskConfirmAction?.Invoke();
    }
    public void OnClickAgainAskPopUp_CloseBtn() {
        OnClickAskCloseAction?.Invoke();
    }

    public void OnClickPausePopUp_ContinueBtn() {
        Play();
        PausePopUp.SetActive(false);
    }
    public void OnClickPausePopUp_ExitGameBtn() {
        //* きれつダンジョンの時、出るボタンをリワード得ることにする
        if(GM._.Stage == Config.Stage.STG_INFINITE_DUNGEON) {
            // SM._.SfxPlay(SM.SFX.ClickSFX);
            // GameoverPopUp.SetActive(false);
            // GM._.Victory();
            // Ads_ClaimX2Btn.gameObject.SetActive(false);
            // VictoryTitleTxt.text = $"돌파한 층수: {GM._.WaveCnt}층";
            // Time.timeScale = 0;

            if(GM._.WaveCnt < 3) {
                GoHome();
                return;
            }

            ShowAgainAskMsg("진행중인 게임을 나가시겠습니까?\n<color=blue>(이전 단계까지 데이터가 저장됩니다.)</color>");
            OnClickAskConfirmAction = () => {
                SM._.SfxPlay(SM.SFX.ClickSFX);
                DM._.DB.InfiniteTileMapSaveDt.Reset();
                DM._.DB.InfiniteTileMapSaveDt.SaveDt(isInfiniteDungeon: true);
                GoHome();
            };
        }
        else if(GM._.Stage == Config.Stage.STG_GOBLIN_DUNGEON) {
            GoHome();
        }
        else {
            if(GM._.WaveCnt < 3) {
                GoHome();
                return;
            }

            ShowAgainAskMsg("진행중인 게임을 나가시겠습니까?\n<color=blue>(이전 단계까지 데이터가 저장됩니다.)</color>");
            OnClickAskConfirmAction = () => {
                SM._.SfxPlay(SM.SFX.ClickSFX);
                DM._.DB.StageTileMapSaveDt.Reset();
                DM._.DB.StageTileMapSaveDt.SaveDt();
                GoHome();
            };
            OnClickAskCloseAction = () => {
                SM._.SfxPlay(SM.SFX.ClickSFX);
                Play();
                AgainAskPopUp.SetActive(false);
                PausePopUp.SetActive(false);
            };
        }
    }

    public void OnClickPlaySpeedBtn() {
        Debug.Log($"OnClickPlaySpeedBtn()::");
        SM._.SfxPlay(SM.SFX.ClickSFX);
        const int OFF = 0, ON = 1;

        var time = Time.timeScale;
        //* タイム速度
        if(time == 1) {
            Time.timeScale = 2;
            SetPlaySpeedBtnUI(playSpeedBtnSprs[ON], Time.timeScale);
        }
        else if(time == 2) {
            if(GM._.IsActiveSpeedUp) {
                Time.timeScale = 3;
                SetPlaySpeedBtnUI(playSpeedBtnSprs[ON], Time.timeScale);
            }
            else {
                //* 以前の状態とタイムスケール保存
                previousTimeScale = Time.timeScale;
                previousState = GM._.State;

                //* 停止
                Time.timeScale = 0;
                GM._.State = GameState.Pause;
                ShowAgainAskMsg("광고시청 후 게임배속 3배를\n추가하시겠습니까?");

                OnClickAskConfirmAction = () => {
                    AgainAskPopUp.SetActive(false);
                    AdmobManager._.ProcessRewardAd(() => {
                        SM._.SfxPlay(SM.SFX.CompleteSFX);
                        Time.timeScale = 3;
                        GM._.State = GameState.Play;
                        SetPlaySpeedBtnUI(playSpeedBtnSprs[ON], Time.timeScale);
                        GM._.IsActiveSpeedUp = true;
                    });
                };
                OnClickAskCloseAction = () => {
                    AgainAskPopUp.SetActive(false);
                    SM._.SfxPlay(SM.SFX.ClickSFX);
                    Time.timeScale = 1;
                    GM._.State = previousState;
                    SetPlaySpeedBtnUI(playSpeedBtnSprs[OFF], Time.timeScale);
                };
            }
        }
        else {
            Time.timeScale = 1;
            SetPlaySpeedBtnUI(playSpeedBtnSprs[OFF], Time.timeScale);
        }
    }

    public void OnClickResetWallBtn() {
        Debug.Log("OnClickResetRockBtn()");
        SM._.SfxPlay(SM.SFX.InvEquipSFX);
        if(DM._.DB.TutorialDB.IsActiveEnemyInfo) {
            ShowMsgError("튜토리얼 중에는 불가능합니다.");
            return;
        }
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

    //* PUBLIC
    public void OnClickReTryBtn() => Retry();

    //* GAME OVER
    public void OnClickAdsReviveBtn() {
        const int OFF = 0;
        //* リワード広告
        AdmobManager._.ProcessRewardAd(() => {
            Debug.Log("AD: 부활");
            GM._.IsRevived = true;
            SM._.SfxPlay(SM.SFX.InvStoneSFX);
            ShowMsgNotice("부활!!!");
            ReviveSpawnUIEF.SetActive(true);

            //* 復活したら、以前に残っているモンスターを削除（ボースはしない）
            Transform enemyGroup = GM._.em.enemyObjGroup;
            Enemy firstEnemy = enemyGroup.GetChild(0).GetComponent<Enemy>();
            if(firstEnemy.Type != EnemyType.Boss && enemyGroup.childCount > 0) {
                for(int i = 0; i < enemyGroup.childCount; i++) {
                    Enemy enemy = enemyGroup.GetChild(i).GetComponent<Enemy>();
                    enemy.DecreaseHp(999999);
                }
            }

            //* 初期化
            GameoverPopUp.SetActive(false);
            GM._.State = GameState.Ready;
            Time.timeScale = 1;
            GM._.Life = Config.DEFAULT_LIFE;
            HeartFillImg.fillAmount = 1;
            playSpeedBtnImg.sprite = playSpeedBtnSprs[OFF];
            playSpeedBtnTxt.text = $"X1";
            GM._.gef.ShowIconTxtEF(HeartFillImg.transform.position, GM._.MaxLife, "Heart");
        });
    }

    //* VICTORY
    public void OnClickVictoryPopUpConfirmBtn() => GoHome();
    #endregion

    #region UPGRADE TOWER CARDS
    public void OnClickUpgradeTowerCard(int kindIdx) {
        //* 型変換
        TowerKind kind = kindIdx == 0? TowerKind.Warrior
            : kindIdx == 1? TowerKind.Archer
            : kindIdx == 2? TowerKind.Magician
            : TowerKind.None;

        int resultPrice = TowerManager.CARD_UPG_PRICE_START + GM._.tm.TowerCardUgrLvs[kindIdx] * TowerManager.CARD_UPG_PRICE_UNIT;
        int discount = GetUpgradeCostDiscountPrice(kindIdx, resultPrice);
        resultPrice -= discount;

        //* 費用チェック
        if(!GM._.CheckMoney(resultPrice)) return;

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
        //* 現在の状態がPAUSEなら、しない。（戻してもPAUSEになるバグがあるので）
        if(GM._.State != GameState.Pause)
            previousState = GM._.State;
        //* 現在のTimeScaleが０なら、しない。（戻してもTimeScaleが０になるバグがあるので）
        if(Time.timeScale != 0)
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
    private void GoHome() {
        Time.timeScale = 1;
        DM._.Save(); //* Victoryでもらったリワードを保存 (ホームに戻ったら、データをロードするから、この前にリワードと変わったデータを保存する必要がある)
        SceneManager.LoadScene(Enum.Scene.Home.ToString());
    }
    private void Retry() {
        if(!DM._.DB.IsRemoveAd) {

        }
        Time.timeScale = 1;
        SceneManager.LoadScene(Enum.Scene.Game.ToString());
    }

    private int GetUpgradeCostDiscountPrice(int kindIdx, int resultPrice) {
        int disCountPrice = 0;
        //* 割引き能力チェック
        float disCountPer = (kindIdx == (int)TowerKind.Warrior)? DM._.DB.EquipDB.WarriorUpgCostPer
            : (kindIdx == (int)TowerKind.Archer)? DM._.DB.EquipDB.ArcherUpgCostPer
            : (kindIdx == (int)TowerKind.Magician)? DM._.DB.EquipDB.MagicianUpgCostPer
            : 0;
        //* アップデートコスト減る能力があるのに、値段を整数化したら、０なら最小１を割引きする
        if(disCountPer > 0.0f) {
            disCountPrice = disCountPer > 0? (int)(resultPrice * disCountPer) : 1;
        }
        return disCountPrice;
    }

    private void SetPlaySpeedBtnUI(Sprite spr, float  timeScaleStr) {
        playSpeedBtnImg.sprite = spr;
        playSpeedBtnTxt.text = $"X{timeScaleStr}";
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
        SM._.SfxPlay(SM.SFX.ErrorSFX);
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
        yield return Time.timeScale == 1.5f? Util.Time1_5
            : Time.timeScale == 2? Util.Time2
            : Time.timeScale == 4? Util.Time4
            : Util.Time1;
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

        SM._.SfxPlay(SM.SFX.UpgradeSFX);

        for(int i = 0; i < TowerCardUI.Buttons.Length; i++) {
            TowerCardUI.LvTxts[i].text = $"LV {cardLvs[i]}";

            int resultPrice = PRICE + (cardLvs[i] > 0 ? cardLvs[i] * UNIT : 0);
            int discount = GetUpgradeCostDiscountPrice(i, resultPrice);
            resultPrice -= discount;
            TowerCardUI.PriceTxts[i].text = $"{resultPrice}";

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
