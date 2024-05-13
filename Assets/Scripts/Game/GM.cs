using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using Inventory.Model;
using Random = UnityEngine.Random;
using System.Linq;
using DG.Tweening;
using Inventory.UI;
// using UnityEngine.Rendering.Universal.Internal;
// using UnityEditorInternal;
// using UnityEditor.SceneManagement;
// using UnityEngine.Events;

/// <summary>
/// ステージに関したデータ
/// </summary>
[System.Serializable]
public class StageData {
    [field: SerializeField] public string Name {get; set;}
    [field: SerializeField] public GameObject TileMapObj {get; set;}
    [field: SerializeField] public TileBase[] Walls {get; set;}
    [field: SerializeField] public SettingEnemyData[] EnemyDatas {get; set;}
}

public enum GameState {Ready, Play, Pause, Gameover};

public class GM : MonoBehaviour {
    Coroutine CorReadyWaveID;
    public static GM _; //* Global

    [SerializeField] public InventorySO InventoryData;
    [SerializeField] GameState state;   public GameState State {get => state; set => state = value;}
    [field: SerializeField] public bool IsReady;
    [field: SerializeField] public bool IsRevived;
    [field: SerializeField] public StageData[] StageDts;
    [field: SerializeField] public int Stage {get; set;}
    [field: SerializeField] public int MaxWave {get; set;}
    [field: SerializeField] public int WaveCnt {get; set;}
    [field: SerializeField] public int ResetCnt {get; set;}
    [field: SerializeField] public int MaxLife {get; set;}

    [SerializeField] int life; public int Life {
        get => life;
        set {
            life = value;
            gui.HeartFillImg.fillAmount = (float)life / MaxLife;
            gui.LifeTxt.text = life.ToString();
            bossRwd.SetCurValueTxt(Enum.BossRwd.IncreaseLife, life);
        }
    }
    [field: SerializeField] public int Money {get; set;}
    [field: SerializeField] List<RewardItem> VictoryRwdList {get; set;} = new List<RewardItem>();
    [field: SerializeField] public Material BlinkMt;
    [field: SerializeField] public Material DefaultMt;

    //* Outside
    public DOTweenAnimation CamDOTAnim;
    public GameUIManager gui;
    public GameEffectManager gef;
    public PathFindManager pfm;
    public EnemyManager em;
    public EnemyStateUIManager esm;
    public TileMapController tmc;
    public ActionBarUIManager actBar;
    public MergableUIManager mgb;
    public BossRewardUIManager bossRwd;
    public TowerManager tm;
    public MissileManager mm;
    public GameRewardUIManager rwm;
    public GameRewardListUIManager rwlm;
    public GameConsumeItemUIManager gcsm;

    void Awake() {
        //* Global化 値 代入
        _ = this;

        //* 外部のスクリプト 初期化
        CamDOTAnim = Camera.main.GetComponent<DOTweenAnimation>();
        gui = GameObject.Find("GameUIManager").GetComponent<GameUIManager>();
        gef = GameObject.Find("GameEffectManager").GetComponent<GameEffectManager>();
        pfm = GameObject.Find("PathFindManager").GetComponent<PathFindManager>();
        em = GameObject.Find("EnemyManager").GetComponent<EnemyManager>();
        esm = GameObject.Find("EnemyStateUIManager").GetComponent<EnemyStateUIManager>();
        tmc = GameObject.Find("TileMapController").GetComponent<TileMapController>();
        actBar = GameObject.Find("ActionBarUIManager").GetComponent<ActionBarUIManager>();
        mgb = GameObject.Find("MergableUIManager").GetComponent<MergableUIManager>();
        bossRwd = GameObject.Find("BossRewardUIManager").GetComponent<BossRewardUIManager>();
        tm = GameObject.Find("TowerManager").GetComponent<TowerManager>();
        mm = GameObject.Find("MissileManager").GetComponent<MissileManager>();
        rwm = GameObject.Find("MissileManager").GetComponent<GameRewardUIManager>();
        rwlm = GameObject.Find("GameRewardListUIManager").GetComponent<GameRewardListUIManager>();
        gcsm = GameObject.Find("GameConsumeItemUIManager").GetComponent<GameConsumeItemUIManager>();

        //* 初期化
        state = GameState.Ready;
        CorReadyWaveID = null;
        IsReady = false;
        Stage = DM._.SelectedStage;

        //* BGM
        switch(Stage) {
            case Config.Stage.STG1_FOREST:
                SM._.BgmPlay(SM.BGM.ForestBGM);
                break;
            case Config.Stage.STG2_DESERT:
                SM._.BgmPlay(SM.BGM.DesertBGM);
                break;
            case Config.Stage.STG3_SEA:
                SM._.BgmPlay(SM.BGM.SeaBGM);
                break;
            case Config.Stage.STG4_UNDEAD:
                SM._.BgmPlay(SM.BGM.UndeadBGM);
                break;
            case Config.Stage.STG5_HELL:
                SM._.BgmPlay(SM.BGM.HellBGM);
                break;
            case Config.Stage.STG_GOBLIN_DUNGEON:
                SM._.BgmPlay(SM.BGM.GoblinDungeonBGM);
                gef.GoldKeyAttractionUIEF.Play();
                break;
            case Config.Stage.STG_INFINITE_DUNGEON:
                SM._.BgmPlay(SM.BGM.GoblinDungeonBGM);
                gef.GoldKeyAttractionUIEF.Play();
                break;
        }

        //* 非表示 初期化
        Array.ForEach(StageDts, stageDt => stageDt.TileMapObj.SetActive(false));

        MaxWave = StageDts[Stage].EnemyDatas[(int)DM._.SelectedStageNum].Waves.Length;
        WaveCnt = 0;
        ResetCnt = Config.DEFAULT_RESET_CNT;
        life = Config.DEFAULT_LIFE
            + (int)DM._.DB.SkillTreeDB.GetUtilityVal((int)SKT_UT.EXTRA_LIFE)
            + DM._.DB.EquipDB.StartLife;
        MaxLife = life;
        Money = Config.DEFAULT_MONEY
            + (int)DM._.DB.SkillTreeDB.GetUtilityVal((int)SKT_UT.EXTRA_MONEY)
            + DM._.DB.EquipDB.StartMoney;

        SM._.SfxPlay(SM.SFX.GameStartSFX);

        //* 現在選択したステージのタイルマップ 表示
        StageDts[Stage].TileMapObj.SetActive(true);

        gui.SetNextEnemyInfoFlagUI();
        gui.SwitchGameStateUI(state);

        //* ステージタイトルと難易度 表示 アニメーション
        string difficulty = (DM._ == null)? ""
            : (DM._.SelectedStageNum == Enum.StageNum.Stage1_1)? "1-1"
            : (DM._.SelectedStageNum == Enum.StageNum.Stage1_2)? "1-2"
            : "1-3";
        string stageInfoTxt = $"{StageDts[Stage].Name}\n<size=70%>- {difficulty} -</size>";
        gef.ActiveStageTitleAnim(stageInfoTxt);
        gui.StageInfoTxt.text = stageInfoTxt; // Pauseのステージ情報テキストにも代入

        //* Tutorial
        TutoM._.InitGameBubbleElements();
        if(DM._.DB.TutorialDB.IsActiveGameStart) {
            DM._.DB.TutorialDB.IsActiveGameStart = false;
            TutoM._.ShowTutoPopUp(TutoM.HOWTIPLAY_INFO, pageIdx: 0); // TutoM._.ShowHowToPlayPopUp(delay: 0.3f);
        }
    }

#region EVENT
    #region DEBUG
    public void OnClickNextTower() {
        if(tmc.HitObject == null) {
            gui.ShowMsgError("(테스트용)레벨업할 타워를 선택해주세요.");
            return;
        }

        var tower = tmc.HitObject.GetComponentInChildren<Tower>();
        if(tower.Lv > 5) {
            gui.ShowMsgError("(테스트용)최대레벨입니다.");
            return;
        }

        //* 次のレベルタワーランダムで生成
        tm.CreateTower(tower.Type, tower.Lv++, tower.Kind);
        //* 自分を削除
        DestroyImmediate(tower.gameObject);
        actBar.UpdateUI(Enum.Layer.Tower);
    }
    public void OnClickVictoryBtn() {
        Victory();
    }
    #endregion

    public void OnClickStartBtn() {
        int towerCnt = tm.WarriorGroup.childCount + tm.ArcherGroup.childCount + tm.MagicianGroup.childCount;
        if(towerCnt <= 0) {
            gui.ShowMsgError("타워를 1개 이상 건설해주세요!");
            return;
        }

        if(!IsReady) {
            SM._.SfxPlay(SM.SFX.ClickSFX);
            //* WAVE準備
            IsReady = true;
            gui.SetStartBtnUI(IsReady);
            pfm.PathFinding(true);
            esm.ShowNextEnemyStateUI();
            CorReadyWaveID = StartCoroutine(CoReadyWave());
        }
        else {
            //* WAVE開始
            SM._.SfxPlay(SM.SFX.WaveStartSFX);
            IsReady = false;
            gui.SetStartBtnUI(IsReady);
            StartWave();
            StopCoroutine(CorReadyWaveID);
            esm.NextEnemyInfoPopUpUI.Obj.SetActive(false);

            //* Tutorial
            if(WaveCnt == 1) {
                // EnemyInfoPopUp 表示
                if(DM._.DB.TutorialDB.IsActiveEnemyInfo) {
                    DM._.DB.TutorialDB.IsActiveEnemyInfo = false; 
                    TutoM._.ShowTutoPopUp(TutoM.ENEMY_IFNO, pageIdx: 0); // TutoM._.ShowEnemyInfoPopUp(page: 0);
                }
                // Bubble 非表示
                TutoM._.G_TutoPathFindBubble.SetActive(false);
                TutoM._.G_TutoWaveStartBubble.SetActive(false);
            }
        }
    }

    public void OnClickClaimX2AdBtn() {
        AdmobManager._.ProcessRewardAd(() => {
            gui.ShowMsgNotice("보상 두배 적용!");
            SM._.SfxPlay(SM.SFX.CompleteSFX);
            gui.Ads_ClaimX2Btn.gameObject.SetActive(false);
            rwlm.ShowReward(VictoryRwdList);
            StartCoroutine(HM._.rwm.CoUpdateInventoryAsync(VictoryRwdList));
            HM._.rwm.CoUpdateInventoryAsync(VictoryRwdList);

            for(int i = 0 ; i < rwlm.Content.childCount; i++) {
                rwlm.Content.GetChild(i).GetComponent<InventoryUIItem>().DoubleRewardLabel.SetActive(true);
            }
        });
    }
#endregion

#region FUNC
    public EnemyData GetCurEnemyData() {
        return StageDts[Stage].EnemyDatas[(int)DM._.SelectedStageNum].Waves[WaveCnt - 1];
    } 
    public EnemyData GetNextEnemyData() {
        return StageDts[Stage].EnemyDatas[(int)DM._.SelectedStageNum].Waves[WaveCnt];
    } 

    IEnumerator CoReadyWave() {
        yield return Util.RealTime1;
        IsReady = false;
        CorReadyWaveID = null;
        gui.SetStartBtnUI(IsReady);
        esm.NextEnemyInfoPopUpUI.Obj.SetActive(false);
    }

    /// <summary>
    /// WAVE開始
    /// </summary>
    private void StartWave() {
        state = GameState.Play;
        gui.WaveTxt.text = $"WAVE {++WaveCnt}";
        gui.EnemyCntTxt.text = $"{em.EnemyCnt} / {em.EnemyCnt}";
        gui.SwitchGameStateUI(state);
        pfm.PathFinding();
        StartCoroutine(em.CoCreateEnemy());
        gui.InActiveResetWallBtn();
    }

    /// <summary>
    /// WAVE終了
    /// </summary>
    public void FinishWave() {
        Debug.Log($"FinishWave():: Wave= {WaveCnt}");
        state = GameState.Ready;
        gui.WaveTxt.text = $"WAVE {WaveCnt} / {MaxWave}";
        gui.SwitchGameStateUI(state);

        //* Next Enemy Info UI
        if(WaveCnt < MaxWave)
            gui.SetNextEnemyInfoFlagUI();
        else {
            Victory();
            return;
        }

        //* ボスリワード 表示
        if(WaveCnt % 10 == 0) {
            int rwdSelectCnt = 0;
            int bossNum = WaveCnt / 10;
            switch(bossNum) {
                case 1: case 2: rwdSelectCnt = 2;
                    break;
                case 3: case 4: case 5: rwdSelectCnt = 3;
                    break;
                default: rwdSelectCnt = 4;
                    break;
            }
            bossRwd.Active(rwdSelectCnt);
        }

        //* 消費アイテムの待機ターンを一個 減る
        gcsm.DecreaseConsumeItemWaitTurn();

    }

    public void Victory() {
        // gui.Pause();
        Time.timeScale = 1;
        State = GameState.Pause;

        SM._.SfxPlay(SM.SFX.CompleteSFX);
        gui.VictoryPopUp.SetActive(true);

        //* リワード
        DB db = DM._.DB;
        RewardItemSO rwDt = HM._.rwlm.RwdItemDt;
        Enum.StageNum diff = DM._.SelectedStageNum;
        var rewardList = new List<RewardItem>();
        int stage = DM._.SelectedStage;
        int nextStage = stage + 1;

    #region STAGE CLEAR UNLOCK
        //* もしIndexを超えると今のステージIndexに戻す
        if(nextStage >= db.StageLockedDBs.Count()) {
            nextStage = stage;
        }

        //* 一般のステージのみ
        if(stage != Config.Stage.STG_GOBLIN_DUNGEON && stage != Config.Stage.STG_INFINITE_DUNGEON) {
            StageLockedDB stageLockDt = db.StageLockedDBs[stage];
            StageLockedDB nextStageLockDt = db.StageLockedDBs[nextStage];

            switch(stage) {
                case Config.Stage.STG1_FOREST:
                    if(diff == Enum.StageNum.Stage1_1) {
                        stageLockDt.IsLockStage1_2 = false;
                        stageLockDt.StageRewards[1].IsUnlockAlert = true;
                        stageLockDt.StageRewards[0].IsActiveBonusReward = true;
                    }
                    else if(diff == Enum.StageNum.Stage1_2) {
                        stageLockDt.IsLockStage1_3 = false;
                        stageLockDt.StageRewards[2].IsUnlockAlert = true;
                        stageLockDt.StageRewards[1].IsActiveBonusReward = true;
                    }
                    else if(diff == Enum.StageNum.Stage1_3) {
                        nextStageLockDt.IsLockStage1_1 = false;
                        nextStageLockDt.StageRewards[0].IsUnlockAlert = true;
                        stageLockDt.StageRewards[2].IsActiveBonusReward = true;
                    }
                    break;
                case Config.Stage.STG2_DESERT: // DESERT
                    if(diff == Enum.StageNum.Stage1_1) {
                        stageLockDt.IsLockStage1_2 = false;
                        stageLockDt.StageRewards[1].IsUnlockAlert = true;
                        stageLockDt.StageRewards[0].IsActiveBonusReward = true;
                    }
                    else if(diff == Enum.StageNum.Stage1_2) {
                        stageLockDt.IsLockStage1_3 = false;
                        stageLockDt.StageRewards[2].IsUnlockAlert = true;
                        stageLockDt.StageRewards[1].IsActiveBonusReward = true;
                    }
                    else if(diff == Enum.StageNum.Stage1_3) {
                        nextStageLockDt.IsLockStage1_1 = false;
                        nextStageLockDt.StageRewards[0].IsUnlockAlert = true;
                        stageLockDt.StageRewards[2].IsActiveBonusReward = true;
                    }
                    break;
                case Config.Stage.STG3_SEA: // SEA
                    if(diff == Enum.StageNum.Stage1_1) {
                        stageLockDt.IsLockStage1_2 = false;
                        stageLockDt.StageRewards[1].IsUnlockAlert = true;
                        stageLockDt.StageRewards[0].IsActiveBonusReward = true;
                    }
                    else if(diff == Enum.StageNum.Stage1_2) {
                        stageLockDt.IsLockStage1_3 = false;
                        stageLockDt.StageRewards[2].IsUnlockAlert = true;
                        stageLockDt.StageRewards[1].IsActiveBonusReward = true;
                    }
                    else if(diff == Enum.StageNum.Stage1_3) {
                        nextStageLockDt.IsLockStage1_1 = false;
                        nextStageLockDt.StageRewards[0].IsUnlockAlert = true;
                        stageLockDt.StageRewards[2].IsActiveBonusReward = true;
                    }
                    break;
                case Config.Stage.STG4_UNDEAD: // UNDEAD
                    if(diff == Enum.StageNum.Stage1_1) {
                        stageLockDt.IsLockStage1_2 = false;
                        stageLockDt.StageRewards[1].IsUnlockAlert = true;
                        stageLockDt.StageRewards[0].IsActiveBonusReward = true;
                    }
                    else if(diff == Enum.StageNum.Stage1_2) {
                        stageLockDt.IsLockStage1_3 = false;
                        stageLockDt.StageRewards[2].IsUnlockAlert = true;
                        stageLockDt.StageRewards[1].IsActiveBonusReward = true;
                    }
                    else if(diff == Enum.StageNum.Stage1_3) {
                        nextStageLockDt.IsLockStage1_1 = false;
                        nextStageLockDt.StageRewards[0].IsUnlockAlert = true;
                        stageLockDt.StageRewards[2].IsActiveBonusReward = true;
                    }
                    break;
                case Config.Stage.STG5_HELL: // HELL
                    if(diff == Enum.StageNum.Stage1_1) {
                        stageLockDt.IsLockStage1_2 = false;
                        stageLockDt.StageRewards[1].IsUnlockAlert = true;
                        stageLockDt.StageRewards[0].IsActiveBonusReward = true;
                    }
                    else if(diff == Enum.StageNum.Stage1_2) {
                        stageLockDt.IsLockStage1_3 = false;
                        stageLockDt.StageRewards[2].IsUnlockAlert = true;
                        stageLockDt.StageRewards[1].IsActiveBonusReward = true;
                    }
                    else if(diff == Enum.StageNum.Stage1_3) {
                        gui.ShowMsgNotice("모든 스테이지 클리어!!");
                        stageLockDt.StageRewards[2].IsActiveBonusReward = true;
                    }
                    break;
        }
        }
    #endregion

    #region REWARD LIST
        var ORE0 = Etc.NoshowInvItem.Ore0;
        var ORE1 = Etc.NoshowInvItem.Ore1;
        var ORE2 = Etc.NoshowInvItem.Ore2;
        var ORE3 = Etc.NoshowInvItem.Ore3;
        var ORE4 = Etc.NoshowInvItem.Ore4;
        var ORE5 = Etc.NoshowInvItem.Ore5;
        var ORE6 = Etc.NoshowInvItem.Ore6;
        var ORE7 = Etc.NoshowInvItem.Ore7;
        var ORE8 = Etc.NoshowInvItem.Ore8;
        
        const int OFFSET = 1;
        int rd = Random.Range(0, 100);
        int oreCnt = Random.Range(1, 5 + OFFSET);

        //* Daily Mission DB
        if(stage < Config.Stage.STG_GOBLIN_DUNGEON)
            DM._.DB.DailyMissionDB.ClearStageVal++;
        else
            DM._.DB.DailyMissionDB.ClearGoblinDungyenVal++;

        //* リワード
        if(stage == Config.Stage.STG1_FOREST) {
            switch(diff) {
                case Enum.StageNum.Stage1_1: {
                    rewardList = HM._.rwm.BuildVictoryRewardList (
                        exp: 20, 
                        coin: 1000,
                        rd < 50? ORE0 : ORE1,
                        oreCnt,
                        fame: 1
                    );
                    break;
                }
                case Enum.StageNum.Stage1_2: {
                    rewardList = HM._.rwm.BuildVictoryRewardList (
                        exp: 30, 
                        coin: 1400,
                        rd < 60? ORE1 : rd < 90? ORE2 : ORE3,
                        oreCnt,
                        fame: 2
                    );
                    break;
                }
                case Enum.StageNum.Stage1_3: {
                    rewardList = HM._.rwm.BuildVictoryRewardList (
                        exp: 40, 
                        coin: 1800,
                        rd < 20? ORE1 : rd < 70? ORE2 : ORE3,
                        oreCnt,
                        fame: 3
                    );
                    break;
                }
            }
        }
        else if(stage == Config.Stage.STG2_DESERT) {
            switch(diff) {
                case Enum.StageNum.Stage1_1: {
                    rewardList = HM._.rwm.BuildVictoryRewardList (
                        exp: 50, 
                        coin: 2200,
                        rd < 50? ORE2 : rd < 85? ORE3 : ORE4,
                        oreCnt,
                        fame: 4
                    );
                    break;
                }
                case Enum.StageNum.Stage1_2: {
                    rewardList = HM._.rwm.BuildVictoryRewardList (
                        exp: 60, 
                        coin: 2700,
                        rd < 25? ORE2 : rd < 70? ORE3 : ORE4,
                        oreCnt,
                        fame: 5
                    );
                    break;
                }
                case Enum.StageNum.Stage1_3: {
                    rewardList = HM._.rwm.BuildVictoryRewardList (
                        exp: 70, 
                        coin: 3100,
                        rd < 55? ORE3 : ORE4,
                        oreCnt,
                        fame: 6
                    );
                    break;
                }
            }
        }
        else if(stage == Config.Stage.STG3_SEA) {
            switch(diff) {
                case Enum.StageNum.Stage1_1: {
                    rewardList = HM._.rwm.BuildVictoryRewardList (
                        exp: 85, 
                        coin: 3600,
                        rd < 50? ORE3 : rd < 90? ORE4 : ORE5,
                        oreCnt,
                        fame: 7
                    );
                    break;
                }
                case Enum.StageNum.Stage1_2: {
                    rewardList = HM._.rwm.BuildVictoryRewardList (
                        exp: 100, 
                        coin: 4200,
                        rd < 40? ORE3 : rd < 80? ORE4 : ORE5,
                        oreCnt,
                        fame: 8
                    );
                    break;
                }
                case Enum.StageNum.Stage1_3: {
                    rewardList = HM._.rwm.BuildVictoryRewardList (
                        exp: 125, 
                        coin: 4800,
                        rd < 20? ORE3 : rd < 60? ORE4 : rd < 95? ORE5 : ORE6,
                        oreCnt,
                        fame: 9
                    );
                    break;
                }
            }
        }
        else if(stage == Config.Stage.STG4_UNDEAD) {
            switch(diff) {
                case Enum.StageNum.Stage1_1: {
                    rewardList = HM._.rwm.BuildVictoryRewardList (
                        exp: 150, 
                        coin: 5500,
                        rd < 40? ORE4 : rd < 85? ORE5 : ORE6,
                        oreCnt,
                        fame: 10
                    );
                    break;
                }
                case Enum.StageNum.Stage1_2: {
                    rewardList = HM._.rwm.BuildVictoryRewardList (
                        exp: 175, 
                        coin: 6200,
                        rd < 30? ORE4 : rd < 65? ORE5 : rd < 90? ORE6 : ORE7,
                        oreCnt,
                        fame: 11
                    );
                    break;
                }
                case Enum.StageNum.Stage1_3: {
                    rewardList = HM._.rwm.BuildVictoryRewardList (
                        exp: 210, 
                        coin: 7000,
                        rd < 20? ORE4 : rd < 50? ORE5 : rd < 80? ORE6 : ORE7,
                        oreCnt,
                        fame: 12
                    );
                    break;
                }
            }
        }
        else if(stage == Config.Stage.STG5_HELL) {
            switch(diff) {
                case Enum.StageNum.Stage1_1: {
                    rewardList = HM._.rwm.BuildVictoryRewardList (
                        exp: 250, 
                        coin: 7800,
                        rd < 40? ORE5 : rd < 70? ORE6 : rd < 90? ORE7 : ORE8,
                        oreCnt,
                        fame: 13
                    );
                    break;
                }
                case Enum.StageNum.Stage1_2: {
                    rewardList = HM._.rwm.BuildVictoryRewardList (
                        exp: 300, 
                        coin: 8600,
                        rd < 20? ORE5 : rd < 50? ORE6 : rd < 80? ORE7 : ORE8,
                        oreCnt,
                        fame: 14
                    );
                    break;
                }
                case Enum.StageNum.Stage1_3: {
                    rewardList = HM._.rwm.BuildVictoryRewardList (
                        exp: 350, 
                        coin: 9200,
                        rd < 30? ORE6 : rd < 70? ORE7 : ORE8,
                        oreCnt,
                        fame: 15
                    );
                    break;
                }
            }
        }
        else if(stage == Config.Stage.STG_GOBLIN_DUNGEON) { //|| stage == Config.GOBLIN_DUNGEON_STAGE + 1|| stage == Config.GOBLIN_DUNGEON_STAGE + 2 ) { //* 唯一にStageSelectedStageが＋して分けている（ゴブリン敵イメージを異なるため）
            //* Difficultによる、リワードデータ
            int exp = (diff == Enum.StageNum.Stage1_1)? 150 : (diff == Enum.StageNum.Stage1_2)? 350 : 700;
            int coin = (diff == Enum.StageNum.Stage1_1)? 1000 : (diff == Enum.StageNum.Stage1_2)? 2500 : 5000;
            int chestGoldQuantity = (diff == Enum.StageNum.Stage1_1)? 1 : (diff == Enum.StageNum.Stage1_2)? 2 : 3;

            //* Difficultによる、ゴブリンリワードデータ
            Etc.NoshowInvItem[] gblEasyRwdArr = {Etc.NoshowInvItem.Goblin0, Etc.NoshowInvItem.Goblin1, Etc.NoshowInvItem.Goblin2};
            Etc.NoshowInvItem[] gblNormalRwdArr = {Etc.NoshowInvItem.Goblin2, Etc.NoshowInvItem.Goblin3, Etc.NoshowInvItem.Goblin4};
            Etc.NoshowInvItem[] gblHardRwdArr = {Etc.NoshowInvItem.Goblin4, Etc.NoshowInvItem.Goblin5, Etc.NoshowInvItem.Goblin6};

            rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Exp], exp));
            rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Coin], coin));
            rewardList.Add(new (rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.ChestGold], chestGoldQuantity));

            //* Goblin Reward
            int rand = Random.Range(0, 100);
            var goblinRwd = rand < 50? gblEasyRwdArr[0] : rand < 85? gblNormalRwdArr[1] : gblHardRwdArr[2];
            rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)goblinRwd], Random.Range(1, 4)));
        }
        else if(stage == Config.Stage.STG_INFINITE_DUNGEON) {
            //* UI

            //* Reward
            int exp = WaveCnt;
            int fame = Mathf.FloorToInt(WaveCnt * 0.05f);
            int crack = Mathf.FloorToInt(WaveCnt * 0.165f);

            rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Exp], exp));
            rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Fame], fame));
            rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Crack], crack));
        }
    #endregion

        //* 追加コイン＆EXP 適用
        rewardList.ForEach(rwdItem => {
            Etc.NoshowInvItem enumVal = Util.FindEnumVal(rwdItem.Data.name);
            if(enumVal == Etc.NoshowInvItem.Coin && DM._.DB.EquipDB.ClearCoinPer > 0)
                rwdItem.Quantity = (int)(rwdItem.Quantity * (1 + DM._.DB.EquipDB.ClearCoinPer));
            if(enumVal == Etc.NoshowInvItem.Exp) {
                float bonusExpPer = 0;
                //* ボーナスEXP％ 計算
                if(DM._.DB.EquipDB.ClearExpPer > 0)
                    bonusExpPer += DM._.DB.EquipDB.ClearExpPer;
                if(DM._.DB.IsCloverActive)
                    bonusExpPer += Config.CLOVER_BONUS_EXP_PER;
                if(DM._.DB.IsGoldCloverActive)
                    bonusExpPer += Config.GOLDCLOVER_BONUS_EXP_PER;

                //* 適用
                if(bonusExpPer > 0)
                    rwdItem.Quantity += Mathf.RoundToInt(rwdItem.Quantity * bonusExpPer);
            }
        });

        VictoryRwdList = rewardList;
        rwlm.ShowReward(rewardList);
        StartCoroutine(HM._.rwm.CoUpdateInventoryAsync(rewardList));
    }

    /// <summary>
    /// ライフ減る
    /// </summary>
    /// <param name="type">敵のタイプ（一般、空、ボス）</param>
    public void DecreaseLife(EnemyType type) {
        Util._.Blink(gui.HeartFillImg);
        SM._.SfxPlay(SM.SFX.DecreaseLife);

        //* マイナス値
        int val = (type == EnemyType.Boss)? -Enemy.LIFE_DEC_BOSS : -Enemy.LIFE_DEC_MONSTER;
        gef.ShowIconTxtEF(gui.HeartFillImg.transform.position, val, "Heart");
        Life += val; //* マイナス 計算

        CamDOTAnim.DORestart();

        //* ゲームオーバ
        if(life <= 0) {
            gui.Gameover();
        }
    }
    public void SetMoney(int value) {
        Money += value;
        if(Money < 0) 
            Money = 0;
        gui.MoneyTxt.text = Money.ToString();
    }
    /// <summary>
    /// 購入できるかお金をチェックしてから、可能ならお金適用。
    /// </summary>
    public bool CheckMoney(int price) {
        if(Money < price) {
            gui.ShowMsgError("비용이 부족합니다.");
            return false;
        }
        else {
            SetMoney(-price); //* お金減る
        }
        return true;
    }
#endregion
}
