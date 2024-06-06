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

public enum GameState {Ready, Play, Pause, Gameover, Victory};

public class GM : MonoBehaviour {
    Coroutine CorReadyWaveID;
    public static GM _; //* Global

    [SerializeField] public InventorySO InventoryData;
    [SerializeField] GameState state;   public GameState State {get => state; set => state = value;}
    [field: SerializeField] public bool IsReady;
    [field: SerializeField] public bool IsRevived;
    [field: SerializeField] public bool IsActiveSpeedUp {get; set;}
    [field: SerializeField] public StageData[] StageDts;
    [field: SerializeField] public int Stage {get; set;}
    [field: SerializeField] public Enum.StageNum StageNum {get; set;}
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
        IsActiveSpeedUp = DM._.DB.IsRemoveAd;
        Stage = DM._.SelectedStage;
        StageNum = DM._.SelectedStageNum;

#region STAGE DATA SET
        //* Camera Projection Size
        Camera.main.orthographicSize = (Stage == Config.Stage.STG_INFINITE_DUNGEON)? 4.5f : 4.1f;

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
                SM._.BgmPlay(SM.BGM.HellBGM);
                gef.GoldKeyAttractionUIEF.Play();
                break;
        }

        //* 非表示 初期化
        Array.ForEach(StageDts, stageDt => stageDt.TileMapObj.SetActive(false));

        MaxWave = StageDts[Stage].EnemyDatas[(int)StageNum].Waves.Length;
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
            : (StageNum == Enum.StageNum.Stage1_1)? "1-1"
            : (StageNum == Enum.StageNum.Stage1_2)? "1-2"
            : "1-3";
        string stageInfoTxt = $"{StageDts[Stage].Name}\n<size=70%>- {difficulty} -</size>";
        gef.ActiveStageTitleAnim(stageInfoTxt);
        gui.StageInfoTxt.text = stageInfoTxt; // Pauseのステージ情報テキストにも代入

        //* Tutorial
        TutoM._.InitGameBubbleElements();
        TutoM._.ShowGameBubbles(false);
        if(DM._.DB.TutorialDB.IsActiveGameStart) {
            DM._.DB.TutorialDB.IsActiveGameStart = false;
            TutoM._.ShowTutoPopUp(TutoM.HOWTIPLAY_INFO, pageIdx: 0); // TutoM._.ShowHowToPlayPopUp(delay: 0.3f);
        }
    }
#endregion

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
    public void OnClickVictoryBtn() => Victory();
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
            if(CorReadyWaveID != null) {
                StopCoroutine(CorReadyWaveID);
                CorReadyWaveID = null;
            }
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
                TutoM._.G_TutoUpgradeMsgBubble.SetActive(false);
                TutoM._.G_TutoGameStartFinger.SetActive(false);
            }
        }
    }

    public void OnClickClaimX2AdBtn() {
        AdmobManager._.ProcessRewardAd(() => {
            gui.ShowMsgNotice("보상 두배 적용!");
            SM._.SfxPlay(SM.SFX.CompleteSFX);
            gui.Ads_ClaimX2Btn.gameObject.SetActive(false);
            // rwlm.ShowReward(VictoryRwdList);
            StartCoroutine(HM._.rwm.CoUpdateInventoryAsync(VictoryRwdList));

            for(int i = 0 ; i < rwlm.VictoryContent.childCount; i++) {
                InventoryUIItem rwdItemUI = rwlm.VictoryContent.GetChild(i).GetComponent<InventoryUIItem>();
                rwdItemUI.DoubleRewardLabel.SetActive(true);
            }
        });
    }
#endregion

#region FUNC
    public EnemyData GetCurEnemyData() {
        return StageDts[Stage].EnemyDatas[(int)StageNum].Waves[WaveCnt - 1];
    } 
    public EnemyData GetNextEnemyData() {
        return StageDts[Stage].EnemyDatas[(int)StageNum].Waves[WaveCnt];
    } 

    IEnumerator CoReadyWave() {
        yield return Util._.Get1SecByTimeScale();
        Debug.Log("CoReadyWave()::");
        IsReady = false;
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
        actBar.HideUI();

        //* Next Enemy Info UI
        if(WaveCnt < MaxWave)
            gui.SetNextEnemyInfoFlagUI();
        else {
            Victory();
            return;
        }

        //* ボスリワード 表示
        if(WaveCnt % Config.BOSS_SPAWN_CNT == 0) {
            int rwdSelectCnt;
            int bossNum = WaveCnt / Config.BOSS_SPAWN_CNT;
            switch(bossNum) {
                case 1: 
                    rwdSelectCnt = 1;
                    break;
                case 2:
                case 3:
                case 4:
                case 5:
                    rwdSelectCnt = 2;
                    break;
                default:
                    rwdSelectCnt = 3;
                    break;
            }
            bossRwd.Active(rwdSelectCnt);
        }

        //* 消費アイテムの待機ターンを一個 減る
        gcsm.DecreaseConsumeItemWaitTurn();

    }

    private void UnLockStageDB(bool isLastStage = false) {
        int stage = DM._.SelectedStage;
        int nextStage = stage + 1;
        StageLockedDB stageLockDt = DM._.DB.StageLockedDBs[stage];
        StageLockedDB nextStageLockDt = DM._.DB.StageLockedDBs[nextStage];

        if(StageNum == Enum.StageNum.Stage1_1) {
            stageLockDt.IsLockStage1_2 = false;
            stageLockDt.StageRewards[1].IsUnlockAlert = true;
            stageLockDt.StageRewards[0].IsActiveBonusReward = true;
        }
        else if(StageNum == Enum.StageNum.Stage1_2) {
            stageLockDt.IsLockStage1_3 = false;
            stageLockDt.StageRewards[2].IsUnlockAlert = true;
            stageLockDt.StageRewards[1].IsActiveBonusReward = true;

            //* ダンジョンのアンロック
            if(stage == Config.Stage.STG1_FOREST) {
                DM._.DB.DungeonLockedDB.IsLockGoblinNormal = true;
            }
            else if(stage == Config.Stage.STG2_DESERT) {
                DM._.DB.DungeonLockedDB.IsLockInfinite = true;
            }
            else if(stage == Config.Stage.STG3_SEA) {
                DM._.DB.DungeonLockedDB.IsLockGoblinHard = true;
            }
        }
        else if(StageNum == Enum.StageNum.Stage1_3) {
            if(!isLastStage) {
                nextStageLockDt.IsLockStage1_1 = false;
                nextStageLockDt.StageRewards[0].IsUnlockAlert = true;
                stageLockDt.StageRewards[2].IsActiveBonusReward = true;
            }
            else {
                gui.ShowMsgNotice("모든 스테이지 클리어!!");
                stageLockDt.StageRewards[2].IsActiveBonusReward = true;
            }
        }
    }

    public void Victory() {
        // gui.Pause();
        Time.timeScale = 1;
        State = GameState.Victory;

        SM._.SfxPlay(SM.SFX.CompleteSFX);
        gui.VictoryPopUp.SetActive(true);

        //* リワード
        DB db = DM._.DB;
        RewardItemSO rwDt = HM._.rwlm.RwdItemDt;
        Enum.StageNum idxNum = DM._.SelectedStageNum;
        var rewardList = new List<RewardItem>();
        int stageIdx = DM._.SelectedStage;
        int nextStage = stageIdx + 1;

    #region STAGE CLEAR UNLOCK
        //* もしIndexを超えると今のステージIndexに戻す
        if(nextStage >= db.StageLockedDBs.Count())
            nextStage = stageIdx;

        //* 一般のステージのみ
        if(stageIdx != Config.Stage.STG_GOBLIN_DUNGEON && stageIdx != Config.Stage.STG_INFINITE_DUNGEON) {
            StageLockedDB stageLockDt = db.StageLockedDBs[stageIdx];
            StageLockedDB nextStageLockDt = db.StageLockedDBs[nextStage];

            switch(stageIdx) {
                case Config.Stage.STG1_FOREST:
                case Config.Stage.STG2_DESERT:
                case Config.Stage.STG3_SEA:
                case Config.Stage.STG4_UNDEAD:
                    UnLockStageDB();
                    break;
                case Config.Stage.STG5_HELL:
                    UnLockStageDB(isLastStage: true);
                    break;
            }
        }
    #endregion
    #region REWARD LIST
        const int OFFSET = 1;
        const int FIXED_REWARD = -1;
        int rd = Random.Range(0, 100);
        int oreCnt = Random.Range(1, 5 + OFFSET);

        //* Daily Mission DB
        if(stageIdx < Config.Stage.STG_GOBLIN_DUNGEON)
            DM._.DB.DailyMissionDB.ClearStageVal++;
        else
            DM._.DB.DailyMissionDB.ClearGoblinDungyenVal++;

        //* リワード
        if(stageIdx == Config.Stage.STG1_FOREST
        || stageIdx == Config.Stage.STG2_DESERT
        || stageIdx == Config.Stage.STG3_SEA
        || stageIdx == Config.Stage.STG4_UNDEAD
        || stageIdx == Config.Stage.STG5_HELL)
        {
            RewardContentSO stgClearDt = rwDt.Rwd_StageClearDts[Config.Stage.GetCurStageDtIdx(stageIdx, (int)idxNum)];
            var rwdTbList = rwDt.PrepareItemPerTable(stgClearDt);
            var fixRwdList = rwdTbList.FindAll(list => list.percent == FIXED_REWARD);

            // * お先に固定リワード
            for (int i = 0; i < fixRwdList.Count; i++) {
                var (item, per, quantity) = fixRwdList[i];
                rewardList.Add(new RewardItem(item, quantity));
                // Debug.Log($"<color=yellow>Victory():: i({i}): fixItemTblist -> rewardList.Add( name= {item.Name}, per= {per}, quantity= {quantity})</color=yellow>");
            }

            //* ボーナスリワード
            int bonusItemCnt = stgClearDt.Cnt - Config.Stage.CLEAR_REWARD_FIX_CNT;
            var bonusRwdList = rwdTbList.FindAll(list => list.percent > 0);

            //? ログ
            for(int i = 0; i < bonusRwdList.Count; i++) {
                Debug.Log($"Victory():: BonusRwdList[{i}].Name= {bonusRwdList[i].item.Name}, per= {bonusRwdList[i].percent}, quantity= {bonusRwdList[i].quantity}");
            }

            int randMax = stgClearDt.ItemPerTb.GetTotal();

            //* ボーナスのカウントほど、ランダムでリワード追加
            for(int i = 0; i < bonusItemCnt; i++) {
                int rand = Random.Range(0, randMax);
                int startRange = 0;
                foreach (var (item, per, quantity) in bonusRwdList) {
                    int endRange = startRange + per;
                    if(rand < endRange) {
                        Debug.Log($"<color=yellow>Victory():: i= [{i}/{bonusItemCnt}]: randPer({rand}) < per({per}):: BonusRwdList.Name= {item.Name} quantity= {quantity}</color>");
                        rewardList.Add(new RewardItem(item, quantity));
                        bonusRwdList.Remove((item, per, quantity));
                        randMax -= per;
                        break;
                    }
                    startRange = endRange;
                }
            }
        }
        else if(stageIdx == Config.Stage.STG_GOBLIN_DUNGEON) { //|| stage == Config.GOBLIN_DUNGEON_STAGE + 1|| stage == Config.GOBLIN_DUNGEON_STAGE + 2 ) { //* 唯一にStageSelectedStageが＋して分けている（ゴブリン敵イメージを異なるため）
            RewardContentSO stgClearDt = rwDt.Rwd_GoblinDungeonClearDts[(int)DM._.SelectedStageNum];
            Debug.Log($"<color=yellow>Victory():: stgClearDt= {stgClearDt.name}</color>");
            var rwdTbList = rwDt.PrepareItemPerTable(stgClearDt);
            var fixRwdList = rwdTbList.FindAll(list => list.percent == FIXED_REWARD);

            // * 固定リワード
            for (int i = 0; i < fixRwdList.Count; i++) {
                var (item, per, quantity) = fixRwdList[i];
                rewardList.Add(new RewardItem(item, quantity));
                Debug.Log($"<color=yellow>Victory():: i({i}): fixItemTblist -> rewardList.Add( name= {item.Name}, per= {per}, quantity= {quantity})</color=yellow>");
            }

            /*
            // //* idxNumによる、リワードデータ
            // int exp = (idxNum == Enum.StageNum.Stage1_1)? 150 : (idxNum == Enum.StageNum.Stage1_2)? 350 : 700;
            // int coin = (idxNum == Enum.StageNum.Stage1_1)? 1000 : (idxNum == Enum.StageNum.Stage1_2)? 2500 : 5000;
            // int chestGoldQuantity = (idxNum == Enum.StageNum.Stage1_1)? 1 : (idxNum == Enum.StageNum.Stage1_2)? 2 : 3;

            // //* Difficultによる、ゴブリンリワードデータ
            // Etc.NoshowInvItem[] gblEasyRwdArr = {Etc.NoshowInvItem.Goblin0, Etc.NoshowInvItem.Goblin1, Etc.NoshowInvItem.Goblin2};
            // Etc.NoshowInvItem[] gblNormalRwdArr = {Etc.NoshowInvItem.Goblin2, Etc.NoshowInvItem.Goblin3, Etc.NoshowInvItem.Goblin4};
            // Etc.NoshowInvItem[] gblHardRwdArr = {Etc.NoshowInvItem.Goblin4, Etc.NoshowInvItem.Goblin5, Etc.NoshowInvItem.Goblin6};

            // rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Exp], exp));
            // rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Coin], coin));
            // rewardList.Add(new (rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.ChestGold], chestGoldQuantity));

            // //* Goblin Reward
            // int rand = Random.Range(0, 100);
            // var goblinRwd = rand < 50? gblEasyRwdArr[0] : rand < 85? gblNormalRwdArr[1] : gblHardRwdArr[2];
            // rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)goblinRwd], Random.Range(1, 4)));
            */
        }
        else if(stageIdx == Config.Stage.STG_INFINITE_DUNGEON) {
            //* Reward
            int exp = WaveCnt;
            int fame = Mathf.FloorToInt(WaveCnt * 0.05f);
            int crack = Mathf.FloorToInt(WaveCnt * 0.165f);

            if(exp > 0)
                rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Exp], exp));
            if(fame > 0)
                rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Fame], fame));
            if(crack > 0)
                rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Crack], crack));

            //* ベストスコア
            DM._.DB.InfiniteUpgradeDB.UpdateBestScore(WaveCnt);
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

    public void Gameover() {
        const int FIXED_REWARD = -1;
        Time.timeScale = 1;
        State = GameState.Gameover;

        SM._.SfxPlay(SM.SFX.GameoverSFX);
        gui.GameoverPopUp.SetActive(true);

        //* ゴブリンダンジョン
        if(Stage == Config.Stage.STG_GOBLIN_DUNGEON) {
            // ステージは復活できない
            gui.Ads_ReviveBtn.gameObject.SetActive(false);
            gui.RetryBtn.gameObject.SetActive(false);
        }
        //* 無限ダンジョン
        else if(Stage == Config.Stage.STG_INFINITE_DUNGEON)
            gui.GameoverExitBtnTxt.text = "보상받기";
        //* 一般ステージ
        else {
            int stageIdx = DM._.SelectedStage;
            RewardItemSO rwDt = HM._.rwlm.RwdItemDt;
            Enum.StageNum idxNum = DM._.SelectedStageNum;

            RewardContentSO stgClearDt = rwDt.Rwd_StageClearDts[Config.Stage.GetCurStageDtIdx(stageIdx, (int)idxNum)];
            var rwdTbList = rwDt.PrepareItemPerTable(stgClearDt);
            var fixRwdList = rwdTbList.FindAll(list => list.percent == FIXED_REWARD);
            var goodsList = fixRwdList.FindAll(list => list.item.name == $"{Etc.NoshowInvItem.Coin}" || list.item.name == $"{Etc.NoshowInvItem.Exp}");
            var rewardList = new List<RewardItem>();

            gui.Ads_ReviveBtn.gameObject.SetActive(!IsRevived);

            //* 進んだステージによる結果％
            float rewardPercent = (WaveCnt < 7)? 0
                : (WaveCnt < MaxWave * 0.25f)? 0.1f
                : (WaveCnt < MaxWave / 0.333f)? 0.15f
                : (WaveCnt < MaxWave / 0.5f)? 0.225f
                : (WaveCnt < MaxWave * 0.75f)? 0.3f
                : (WaveCnt < MaxWave * 0.875f)? 0.35f
                : 0.4f;

            // * 固定リワード 追加
            for (int i = 0; i < goodsList.Count; i++) {
                var (item, per, quantity) = goodsList[i];
                rewardList.Add(new RewardItem(item, Mathf.RoundToInt(quantity * rewardPercent)));
                Debug.Log($"<color=red>Gameover():: i({i}): fixItemTblist -> rewardList.Add( name= {item.Name}, per= {per}, quantity= {quantity})</color=red>");
            }

            //* リワード結果 表示
            rwlm.ShowReward(rewardList);
            StartCoroutine(HM._.rwm.CoUpdateInventoryAsync(rewardList));
        }
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
            Gameover();
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
