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
    Coroutine CorAutoWaitTimeID;
    public bool IsActiveAutoStart;

    [Header("STATIC UI")]
    public GameObject DEBUGGroup;
    public Image playSpeedBtnImg;
    public TextMeshProUGUI playSpeedBtnTxt;
    public Sprite[] playSpeedBtnSprs;
    public Button StartBtn;
    public Image StartBtnImg;
    public Image StartBtnLightImg;
    public TextMeshProUGUI StartBtnTxt;
    public TextMeshProUGUI AutoWaitTimeTxt;
    public TextMeshProUGUI AutoBtnTxt;
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
    public TextMeshProUGUI PauseExitGameTxt;
    public TextMeshProUGUI PauseStatusInfoTxt;
    [SerializeField] public GameState previousState;
    [SerializeField] public float previousTimeScale;


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
    public Button ReplayBtn;

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
        DEBUGGroup.SetActive(DM._.IsDebugMode);

        var db = DM._.DB;
        Time.timeScale = Config.GAMESPEED_NORMAL;
        Debug.Log($"GameUIManager():: Start():: timeScale= {Time.timeScale}");

        playSpeedBtnTxt.text = $"X{Config.GAMESPEED_NORMAL}";
        CorMsgNoticeID = null;
        CorAutoWaitTimeID = null;
        IsActiveAutoStart = false;
        previousState = GameState.Ready;
        previousTimeScale = Config.GAMESPEED_NORMAL;
        TopMsgError.SetActive(false);
        ResetWallBtn.gameObject.SetActive(true);
        InitTextUI();

        //* Set Bagic StatusInfoTxt At PuasePopUp
        float extraDmgPer = db.EquipDB.AttackPer + db.StatusDB.GetUserLvExtraDmgPercent() + db.InfiniteUpgradeDB.GetExtraDmgPercent();
        float extraSpdPer = db.EquipDB.SpeedPer;
        float extraRangePer = db.EquipDB.RangePer;
        float extraCritPer = db.EquipDB.CritPer;
        float extraCritDmgPer = db.EquipDB.CritDmgPer + db.InfiniteUpgradeDB.GetExtraCritDmgPercent();
        float extraBossDmgPer = db.InfiniteUpgradeDB.GetExtraBossDmgPercent();

        float wrExtraDmgPer = db.EquipDB.WarriorAttackPer + db.SkillTreeDB.GetWarriorVal((int)SKT_WR.EXTRA_DMG_A) + db.SkillTreeDB.GetWarriorVal((int)SKT_WR.EXTRA_DMG_B);
        float wrExtraSpdPer = db.SkillTreeDB.GetWarriorVal((int)SKT_WR.EXTRA_SPD);

        float acExtraDmgPer = db.EquipDB.ArcherAttackPer + db.SkillTreeDB.GetArcherVal((int)SKT_AC.EXTRA_DMG_A) + db.SkillTreeDB.GetArcherVal((int)SKT_AC.EXTRA_DMG_B);
        float acExtraRangePer = db.SkillTreeDB.GetWarriorVal((int)SKT_WR.EXTRA_RANGE);
        float acExtraCritPer = db.SkillTreeDB.GetArcherVal((int)SKT_AC.CRIT_PER);
        float acExtraCritDmgPer = db.SkillTreeDB.GetArcherVal((int)SKT_AC.CIRT_DMG_PER_A) + db.SkillTreeDB.GetArcherVal((int)SKT_AC.CIRT_DMG_PER_B);

        float mgExtraDmgPer = db.EquipDB.MagicianAttackPer + db.SkillTreeDB.GetMagicianVal((int)SKT_MG.EXTRA_DMG_A) + db.SkillTreeDB.GetMagicianVal((int)SKT_MG.EXTRA_DMG_B);
        float mgExtraRangePer = db.SkillTreeDB.GetMagicianVal((int)SKT_MG.EXTRA_RANGE);
        float mgExtraCritPer = db.SkillTreeDB.GetMagicianVal((int)SKT_MG.CRIT_PER);
        int extraLife = (int)DM._.DB.SkillTreeDB.GetUtilityVal((int)SKT_UT.EXTRA_LIFE) + + DM._.DB.EquipDB.StartLife;

        PauseStatusInfoTxt.text = "[ 기본 상태정보 ]"
            + $"\n추가공격력: {extraDmgPer * 100}%"
            + $"\n추가공격속도: {extraSpdPer * 100}%"
            + $"\n추가사정거리: {extraRangePer * 100}%"
            + $"\n추가치명타: {extraCritPer * 100}%"
            + $"\n추가치명타데미지: {extraCritDmgPer * 100}%"
            + $"\n추가보스데미지: {extraBossDmgPer * 100}%"
            + $"\n추가시작체력: +{(extraLife > 0? extraLife : "")} (최대 +8)"

            //* 下は０なら、非表示
            + $"{(wrExtraDmgPer > 0? $"\n전사 추가공격력: {wrExtraDmgPer * 100}%" : "")}"
            + $"{(wrExtraSpdPer > 0? $"\n전사 추가공격속도: {wrExtraSpdPer * 100}%" : "")}"

            + $"{(acExtraDmgPer > 0? $"\n궁수 추가공격력: {acExtraDmgPer * 100}%" : "")}"
            + $"{(acExtraRangePer > 0? $"\n궁수 추가사정거리: {acExtraRangePer * 100}%" : "")}"
            + $"{(acExtraCritPer > 0? $"\n궁수 추가치명타: {acExtraCritPer * 100}%" : "")}"
            + $"{(acExtraCritDmgPer > 0? $"\n궁수 추가치명타데미지: {acExtraCritDmgPer * 100}%" : "")}"

            + $"{(mgExtraDmgPer > 0? $"\n법사 추가공격력: {mgExtraDmgPer * 100}%" : "")}"
            + $"{(mgExtraRangePer > 0? $"\n법사 추가사정거리: {mgExtraRangePer * 100}%" : "")}"
            + $"{(mgExtraCritPer > 0? $"\n법사 추가치명타: {mgExtraCritPer * 100}%" : "")}"


        ;

        //* ステージ保存データロード
        if(DM._.DB.TutorialDB.IsActiveEnemyInfo) {
            //なし
        }
        //* 無限ダンジョン セーブデータ 読込み
        else if(GM._.Stage == Config.Stage.STG_INFINITE_DUNGEON
        && DM._.DB.InfiniteTileMapSaveDt.IsSaved)
        {
            Debug.Log($"SpawnWall():: LOAD INFINITE_DUNGEON SAVE DATA, IsSaved= {DM._.DB.InfiniteTileMapSaveDt.IsSaved}");
            DM._.DB.InfiniteTileMapSaveDt.LoadDt();
            InitTextUI();
            return;
        }
        else if((GM._.Stage == Config.Stage.STG1_FOREST
        || GM._.Stage == Config.Stage.STG2_DESERT
        || GM._.Stage == Config.Stage.STG3_SEA
        || GM._.Stage == Config.Stage.STG4_UNDEAD
        || GM._.Stage == Config.Stage.STG5_HELL)
        && DM._.DB.StageTileMapSaveDt.IsSaved)
        {
            Debug.Log($"SpawnWall():: LOAD NORMAL STAGE SAVE DATA, IsSaved= {DM._.DB.StageTileMapSaveDt.IsSaved}");
            DM._.DB.StageTileMapSaveDt.LoadDt();
            InitTextUI();
            return;
        }
    }

#region EVENT
    #region DEBUG
    //! DEBUG トップのWAVEタイトルクリックすると、WAVE UP
    public void Debug_WaveUp() => WaveUp();
    private void WaveUp() {
        if(!DM._.IsDebugMode) return;

        GM._.WaveCnt += 5;
        if(GM._.WaveCnt > GM._.MaxWave) GM._.WaveCnt = 0;
        WaveTxt.text = $"WAVE {GM._.WaveCnt} / {GM._.MaxWave}";
    }

    public void Debug_MeatUp() {
        if(!DM._.IsDebugMode) return;

        GM._.Money += 20;
        MoneyTxt.text = $"{GM._.Money}";
    }
    #endregion

    public void OnClickAutoBtn() {
        if(GM._.tmc.IsRealTimeTutoTrigger) {
            GM._.gui.ShowMsgError("손가락이 가리키는 곳을 클릭해주세요.");
            return;
        }

        SM._.SfxPlay(SM.SFX.ClickSFX);
        int towerCnt = GM._.tm.WarriorGroup.childCount + GM._.tm.ArcherGroup.childCount + GM._.tm.MagicianGroup.childCount;
        if(towerCnt <= 0) {
            GM._.gui.ShowMsgError("타워를 1개 이상 건설해주세요!");
            return;
        }

        if(AutoBtnTxt.color == Color.white) {
            AutoBtnTxt.color = Color.green;
            IsActiveAutoStart = true;
            StartBtn.interactable = false;
        }
        else {
            AutoBtnTxt.color = Color.white;
            IsActiveAutoStart = false;
            AutoWaitTimeTxt.text = "";
            StartBtn.interactable = true;
        }

        CorStartAutoWaitTime();
    }

    public void CorStartAutoWaitTime() {
        if(IsActiveAutoStart) {
            if(CorAutoWaitTimeID != null) StopCoroutine(CorAutoWaitTimeID);
            CorAutoWaitTimeID = StartCoroutine(CoCountAutoWaitTime());
        }
        else {
            if(CorAutoWaitTimeID != null)
                StopCoroutine(CorAutoWaitTimeID);
        }
    }

    public IEnumerator CoCountAutoWaitTime() {
        //* Play途中で切り替えても、Waveが終わるまで待つ
        yield return new WaitUntil(() => GM._.State == GameState.Ready);

        AutoWaitTimeTxt.text = "3";
        SM._.SfxPlay(SM.SFX.CountdownSFX);
        yield return Util._.Get1SecByTimeScale();
        AutoWaitTimeTxt.text = "2";
        SM._.SfxPlay(SM.SFX.CountdownSFX);
        yield return Util._.Get1SecByTimeScale();
        AutoWaitTimeTxt.text = "1";
        SM._.SfxPlay(SM.SFX.CountdownSFX);
        yield return Util._.Get1SecByTimeScale();
        AutoWaitTimeTxt.text = "GO";
        GM._.OnClickStartBtn();
        GM._.OnClickStartBtn();

        yield return Util._.Get1SecByTimeScale();
        AutoWaitTimeTxt.text = "";
    }

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
    public void OnClickGameoverExitGameBtn() {
        // if(GM._.IsInfiniteDungeonGameover) {
        //     SM._.SfxPlay(SM.SFX.ClickSFX);
        //     GameoverPopUp.SetActive(false);
        //     GM._.Victory();
        //     Ads_ClaimX2Btn.gameObject.SetActive(false);
        //     VictoryTitleTxt.text = $"돌파한 층수: {GM._.WaveCnt}층";
        //     Time.timeScale = 0;
        // }
        // else {
            GoHome();
        // }
    }
    public void OnClickPausePopUp_ExitGameBtn() {
        //* きれつダンジョンの時、出るボタンをリワード得ることにする
        if(GM._.Stage == Config.Stage.STG_INFINITE_DUNGEON) {
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
                Debug.Log("OnClickAskCloseAction()::");
                SM._.SfxPlay(SM.SFX.ClickSFX);
                Play();
                AgainAskPopUp.SetActive(false);
                PausePopUp.SetActive(false);
            };
        }
    }

    public void OnClickPlaySpeedBtn() {
        Debug.Log($"OnClickPlaySpeedBtn():: timeScale= {Time.timeScale}");
        SM._.SfxPlay(SM.SFX.ClickSFX);
        const int OFF = 0, ON = 1;
        
        //* タイム速度
        if(Time.timeScale == Config.GAMESPEED_NORMAL) {
            Time.timeScale = Config.GAMESPEED_FAST;
            SetPlaySpeedBtnUI(playSpeedBtnSprs[ON], Time.timeScale);
        }
        else if(Time.timeScale == Config.GAMESPEED_FAST) {
            if(GM._.IsActiveSpeedUp) {
                Time.timeScale = Config.GAMESPEED_ULTRA;
                SetPlaySpeedBtnUI(playSpeedBtnSprs[ON], Time.timeScale);
            }
            else {
                //* 以前の状態とタイムスケール保存
                // previousTimeScale = Time.timeScale;
                // previousState = GM._.State;

                //* 停止
                Pause();
                // Time.timeScale = 0;
                // GM._.State = GameState.Pause;
                ShowAgainAskMsg($"광고시청 후 게임배속 {Config.GAMESPEED_ULTRA}배를\n추가하시겠습니까?");

                OnClickAskConfirmAction = () => {
                    AgainAskPopUp.SetActive(false);
                    AdmobManager._.ProcessRewardAd(() => {
                        Debug.Log($"OnClickPlaySpeedBtn:: Before setting timeScale to 3:: timeScale= {Time.timeScale}, State= {GM._.State}");
                        SM._.SfxPlay(SM.SFX.CompleteSFX);
                        Play();
                        Time.timeScale = Config.GAMESPEED_ULTRA;
                        // GM._.State = GameState.Play;
                        SetPlaySpeedBtnUI(playSpeedBtnSprs[ON], Time.timeScale);

                        GM._.IsActiveSpeedUp = true;
                        Debug.Log($"OnClickPlaySpeedBtn:: AdSpeed X3:: timeScale= {Time.timeScale}, State= {GM._.State}");
                    });
                };

                OnClickAskCloseAction = () => {
                    AgainAskPopUp.SetActive(false);
                    SM._.SfxPlay(SM.SFX.ClickSFX);
                    Time.timeScale = Config.GAMESPEED_NORMAL;
                    GM._.State = previousState;
                    SetPlaySpeedBtnUI(playSpeedBtnSprs[OFF], Time.timeScale);
                };
            }
        }
        else {
            Time.timeScale = Config.GAMESPEED_NORMAL;
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
            // Enemy firstEnemy = enemyGroup.GetChild(0).GetComponent<Enemy>();
            bool isBoss = GM._.WaveCnt % Config.BOSS_SPAWN_CNT == 0;

            if(!isBoss && enemyGroup.childCount > 0) {
                Debug.Log($"OnClickAdsReviveBtn():: isBoss= {isBoss}");
                for(int i = 0; i < enemyGroup.childCount; i++) {
                    Enemy enemy = enemyGroup.GetChild(i).GetComponent<Enemy>();
                    enemy.DecreaseHp(999999999);
                }
            }

            //* 初期化
            GameoverPopUp.SetActive(false);
            GM._.State = isBoss? GameState.Play : GameState.Ready;
            Time.timeScale = Config.GAMESPEED_NORMAL;
            GM._.Life = Config.DEFAULT_LIFE;
            HeartFillImg.fillAmount = 1;
            playSpeedBtnImg.sprite = playSpeedBtnSprs[OFF];
            playSpeedBtnTxt.text = $"X{Config.GAMESPEED_NORMAL}";
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
        if(!GM._.CheckMoney(resultPrice))
            return;

        //* アップグレード
        GM._.tm.UpgradeTowerCard(kind);
    }
    #endregion
#endregion

#region FUNC
    private void InitTextUI() {
        PauseExitGameTxt.text = GM._.Stage == Config.Stage.STG_GOBLIN_DUNGEON? "나가기" : "<color=blue>저장</color> 및 나가기\n<color=blue><size=60%>( 최소 3스테이지 이상 )</size></color>";
        WaveTxt.text = $"WAVE {GM._.WaveCnt} / {GM._.MaxWave}";
        ResetWallCntTxt.text = $"{GM._.ResetCnt}/{Config.DEFAULT_RESET_CNT}";
        EnemyCntTxt.text = "0 / 0";
        MoneyTxt.text = $"{GM._.Money}";
        LifeTxt.text = $"{GM._.Life}";
        AutoWaitTimeTxt.text = "";
        Array.ForEach(TowerCardUI.PriceTxts, txt => txt.text = $"{TowerManager.CARD_UPG_PRICE_START}");
        UpdateTowerCardLvUI();
    }
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
        Time.timeScale = Config.GAMESPEED_NORMAL;
        DM._.Save("GoHome"); //* Victoryでもらったリワードを保存 (ホームに戻ったら、データをロードするから、この前にリワードと変わったデータを保存する必要がある)
        SceneManager.LoadScene(Enum.Scene.Home.ToString());
    }
    private void Retry() {
        if(!DM._.DB.IsRemoveAd) {

        }
        Time.timeScale = Config.GAMESPEED_NORMAL;
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
        yield return Time.timeScale == Config.GAMESPEED_NORMAL? Util.Time1_5
            : Time.timeScale == Config.GAMESPEED_FAST? Util.Time3
            : Time.timeScale == Config.GAMESPEED_ULTRA? Util.Time4_5
            : Util.Time1; // -> NULL
        BottomMsgNotice.SetActive(false);
    }
    public bool ShowErrMsgCreateTowerAtPlayState() {
        if(GM._.State == GameState.Play) {
            ShowMsgError("웨이브 진행중에는 랜덤타워 생성 및 업그레이드만 가능합니다!");
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
