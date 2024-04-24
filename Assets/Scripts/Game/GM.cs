using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using Inventory.Model;
using Random = UnityEngine.Random;
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
    [field: SerializeField] public SettingEnemyData EnemyData {get; set;}
}

public enum GameState {Ready, Play, Pause, Gameover};

public class GM : MonoBehaviour {
    Coroutine CorReadyWaveID;
    public static GM _; //* Global

    [SerializeField] public InventorySO InventoryData;
    [SerializeField] GameState state;   public GameState State {get => state; set => state = value;}
    [field: SerializeField] public bool IsReady;
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
    [field: SerializeField] public Material BlinkMt;
    [field: SerializeField] public Material DefaultMt;

    //* Outside
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
        Stage = DM._ == null? 0 : DM._.SelectedStage;
        if(Stage == Config.GOBLIN_DUNGEON_STAGE) {
            Stage += (int)DM._.SelectedDiff;
        }
        Array.ForEach(StageDts, stageDt => stageDt.TileMapObj.SetActive(false)); //* 非表示 初期化
        MaxWave = StageDts[Stage].EnemyData.Waves.Length;
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
        string difficulty = (DM._ == null)? "TEST"
            : (DM._.SelectedDiff == Enum.StageNum.Stage1_1)? "1-1"
            : (DM._.SelectedDiff == Enum.StageNum.Stage1_2)? "1-2"
            : "1-3";
        string stageInfoTxt = $"{StageDts[Stage].Name}\n<size=70%>- {difficulty} -</size>";
        gef.ActiveStageTitleAnim(stageInfoTxt);
        gui.StageInfoTxt.text = stageInfoTxt; // Pauseのステージ情報テキストにも代入
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
    #endregion

    public void OnClickStartBtn() {
        if(!IsReady) {
            //* WAVE準備
            IsReady = true;
            gui.SetStartBtnUI(IsReady);
            pfm.PathFinding(true);
            CorReadyWaveID = StartCoroutine(CoReadyWave());
        }
        else {
            //* WAVE開始
            SM._.SfxPlay(SM.SFX.WaveStartSFX);
            IsReady = false;
            gui.SetStartBtnUI(IsReady);
            StartWave();
            StopCoroutine(CorReadyWaveID);
        }
    }
#endregion

#region FUNC
    public EnemyData GetCurEnemyData() => StageDts[Stage].EnemyData.Waves[WaveCnt - 1];
    public EnemyData GetNextEnemyData() => StageDts[Stage].EnemyData.Waves[WaveCnt];

    IEnumerator CoReadyWave() {
        yield return Util.RealTime1;
        IsReady = false;
        CorReadyWaveID = null;
        gui.SetStartBtnUI(IsReady);
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
        RewardItemSO rwDt = HM._.rwlm.RwdItemDt;
        Enum.StageNum diff = DM._.SelectedDiff;
        var rewardList = new List<RewardItem>();
        int nextStage = DM._.SelectedStage + 1;
        int selectStage = DM._.SelectedStage;

    #region UNLOCK & GOBLIN REWARD
        switch(selectStage) {
            case 0: // FOREST
                if(diff == Enum.StageNum.Stage1_1) {
                    DM._.DB.StageLockedDBs[selectStage].IsLockStage1_2 = false;
                }
                else if(diff == Enum.StageNum.Stage1_2) {
                    DM._.DB.StageLockedDBs[selectStage].IsLockStage1_3 = false;
                }
                else if(diff == Enum.StageNum.Stage1_3) {
                    DM._.DB.StageLockedDBs[nextStage].IsUnlockAlert = true;
                    DM._.DB.StageLockedDBs[nextStage].IsLockStage1_1 = false;
                }
                break;
            case 1: // DESERT
                if(diff == Enum.StageNum.Stage1_1) {
                    DM._.DB.StageLockedDBs[selectStage].IsLockStage1_2 = false;
                }
                else if(diff == Enum.StageNum.Stage1_2) {
                    DM._.DB.StageLockedDBs[selectStage].IsLockStage1_3 = false;
                }
                else if(diff == Enum.StageNum.Stage1_3) {
                    DM._.DB.StageLockedDBs[nextStage].IsUnlockAlert = true;
                    DM._.DB.StageLockedDBs[nextStage].IsLockStage1_1 = false;
                }
                break;
            case 2: // SEA
                if(diff == Enum.StageNum.Stage1_1) {
                    DM._.DB.StageLockedDBs[selectStage].IsLockStage1_2 = false;
                }
                else if(diff == Enum.StageNum.Stage1_2) {
                    DM._.DB.StageLockedDBs[selectStage].IsLockStage1_3 = false;
                }
                else if(diff == Enum.StageNum.Stage1_3) {
                    DM._.DB.StageLockedDBs[nextStage].IsUnlockAlert = true;
                    DM._.DB.StageLockedDBs[nextStage].IsLockStage1_1 = false;
                }
                break;
            case 3: // UNDEAD
                if(diff == Enum.StageNum.Stage1_1) {
                    DM._.DB.StageLockedDBs[selectStage].IsLockStage1_2 = false;
                }
                else if(diff == Enum.StageNum.Stage1_2) {
                    DM._.DB.StageLockedDBs[selectStage].IsLockStage1_3 = false;
                }
                else if(diff == Enum.StageNum.Stage1_3) {
                    DM._.DB.StageLockedDBs[nextStage].IsUnlockAlert = true;
                    DM._.DB.StageLockedDBs[nextStage].IsLockStage1_1 = false;
                }
                break;
            case 4: // HELL
                if(diff == Enum.StageNum.Stage1_1) {
                    DM._.DB.StageLockedDBs[selectStage].IsLockStage1_2 = false;
                }
                else if(diff == Enum.StageNum.Stage1_2) {
                    DM._.DB.StageLockedDBs[selectStage].IsLockStage1_3 = false;
                }
                else if(diff == Enum.StageNum.Stage1_3) {
                    gui.ShowMsgNotice("모든 스테이지 클리어!!");
                }
                break;
        }
    #endregion
    #region REWARD LIST
        ItemSO RWD_EXP = rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Exp];
        ItemSO RWD_COIN = rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Coin];
        
        const int OFFSET = 1;
        int oreRand = Random.Range(0, 100);
        int oreCnt = Random.Range(1, 5 + OFFSET);

        switch(DM._.SelectedStage) {
            case 0: // FOREST
                if(diff == Enum.StageNum.Stage1_1) {
                    rewardList.Add(new (RWD_EXP, 20));
                    rewardList.Add(new (RWD_COIN, 1000));
                    var rwdOreIdx = oreRand < 50? Etc.NoshowInvItem.Ore0
                        : Etc.NoshowInvItem.Ore1;
                    rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)rwdOreIdx], oreCnt));
                }
                else if(diff == Enum.StageNum.Stage1_2) {
                    rewardList.Add(new (RWD_EXP, 30));
                    rewardList.Add(new (RWD_COIN, 1400));
                    var rwdOreIdx = oreRand < 60? Etc.NoshowInvItem.Ore1
                        : oreRand < 90? Etc.NoshowInvItem.Ore2
                        : Etc.NoshowInvItem.Ore3;
                    rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)rwdOreIdx], oreCnt));
                }
                else if(diff == Enum.StageNum.Stage1_3) {
                    rewardList.Add(new (RWD_EXP, 40));
                    rewardList.Add(new (RWD_COIN, 1800));
                    var rwdOreIdx = oreRand < 20? Etc.NoshowInvItem.Ore1
                        : oreRand < 70? Etc.NoshowInvItem.Ore2
                        : Etc.NoshowInvItem.Ore3;
                    rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)rwdOreIdx], oreCnt));
                }
                break;
            case 1: // DESERT
                if(diff == Enum.StageNum.Stage1_1) {
                    rewardList.Add(new (RWD_EXP, 50));
                    rewardList.Add(new (RWD_COIN, 2200));
                    var rwdOreIdx = oreRand < 50? Etc.NoshowInvItem.Ore2
                        : oreRand < 85? Etc.NoshowInvItem.Ore3
                        : Etc.NoshowInvItem.Ore4;
                    rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)rwdOreIdx], oreCnt));
                }
                else if(diff == Enum.StageNum.Stage1_2) {
                    rewardList.Add(new (RWD_EXP, 60));
                    rewardList.Add(new (RWD_COIN, 2700));
                    var rwdOreIdx = oreRand < 25? Etc.NoshowInvItem.Ore2
                        : oreRand < 70? Etc.NoshowInvItem.Ore3
                        : Etc.NoshowInvItem.Ore4;
                    rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)rwdOreIdx], oreCnt));
                }
                else if(diff == Enum.StageNum.Stage1_3) {
                    rewardList.Add(new (RWD_EXP, 70));
                    rewardList.Add(new (RWD_COIN, 3100));
                    var rwdOreIdx = oreRand < 55? Etc.NoshowInvItem.Ore3
                        : Etc.NoshowInvItem.Ore4;
                    rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)rwdOreIdx], oreCnt));
                }
                break;
            case 2: // SEA
                if(diff == Enum.StageNum.Stage1_1) {
                    rewardList.Add(new (RWD_EXP, 85));
                    rewardList.Add(new (RWD_COIN, 3600));
                    var rwdOreIdx = oreRand < 50? Etc.NoshowInvItem.Ore3
                        : oreRand < 90? Etc.NoshowInvItem.Ore4
                        : Etc.NoshowInvItem.Ore5;
                    rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)rwdOreIdx], oreCnt));
                }
                else if(diff == Enum.StageNum.Stage1_2) {
                    rewardList.Add(new (RWD_EXP, 100));
                    rewardList.Add(new (RWD_COIN, 4200));
                    var rwdOreIdx = oreRand < 40? Etc.NoshowInvItem.Ore3
                        : oreRand < 80? Etc.NoshowInvItem.Ore4
                        : Etc.NoshowInvItem.Ore5;
                    rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)rwdOreIdx], oreCnt));
                }
                else if(diff == Enum.StageNum.Stage1_3) {
                    rewardList.Add(new (RWD_EXP, 125));
                    rewardList.Add(new (RWD_COIN, 4800));
                    var rwdOreIdx = oreRand < 20? Etc.NoshowInvItem.Ore3
                        : oreRand < 60? Etc.NoshowInvItem.Ore4
                        : oreRand < 95? Etc.NoshowInvItem.Ore5
                        : Etc.NoshowInvItem.Ore6;
                    rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)rwdOreIdx], oreCnt));
                }
                break;
            case 3: // UNDEAD
                if(diff == Enum.StageNum.Stage1_1) {
                    rewardList.Add(new (RWD_EXP, 150));
                    rewardList.Add(new (RWD_COIN, 5500));
                    var rwdOreIdx = oreRand < 40? Etc.NoshowInvItem.Ore4
                        : oreRand < 85? Etc.NoshowInvItem.Ore5
                        : Etc.NoshowInvItem.Ore6;
                }
                else if(diff == Enum.StageNum.Stage1_2) {
                    rewardList.Add(new (RWD_EXP, 175));
                    rewardList.Add(new (RWD_COIN, 6200));
                    var rwdOreIdx = oreRand < 30? Etc.NoshowInvItem.Ore4
                        : oreRand < 65? Etc.NoshowInvItem.Ore5
                        : oreRand < 90? Etc.NoshowInvItem.Ore6
                        : Etc.NoshowInvItem.Ore7;
                }
                else if(diff == Enum.StageNum.Stage1_3) {
                    rewardList.Add(new (RWD_EXP, 210));
                    rewardList.Add(new (RWD_COIN, 7000));
                    var rwdOreIdx = oreRand < 20? Etc.NoshowInvItem.Ore4
                        : oreRand < 50? Etc.NoshowInvItem.Ore5
                        : oreRand < 80? Etc.NoshowInvItem.Ore6
                        : Etc.NoshowInvItem.Ore7;
                }
                break;
            case 4: // HELL
                if(diff == Enum.StageNum.Stage1_1) {
                    rewardList.Add(new (RWD_EXP, 250));
                    rewardList.Add(new (RWD_COIN, 7800));
                    var rwdOreIdx = oreRand < 40? Etc.NoshowInvItem.Ore5
                        : oreRand < 70? Etc.NoshowInvItem.Ore6
                        : oreRand < 90? Etc.NoshowInvItem.Ore7
                        : Etc.NoshowInvItem.Ore8;
                }
                else if(diff == Enum.StageNum.Stage1_2) {
                    rewardList.Add(new (RWD_EXP, 300));
                    rewardList.Add(new (RWD_COIN, 8600));
                    var rwdOreIdx = oreRand < 20? Etc.NoshowInvItem.Ore5
                        : oreRand < 50? Etc.NoshowInvItem.Ore6
                        : oreRand < 80? Etc.NoshowInvItem.Ore7
                        : Etc.NoshowInvItem.Ore8;
                }
                else if(diff == Enum.StageNum.Stage1_3) {
                    rewardList.Add(new (RWD_EXP, 350));
                    rewardList.Add(new (RWD_COIN, 9200));
                    var rwdOreIdx = oreRand < 30? Etc.NoshowInvItem.Ore6
                        : oreRand < 70? Etc.NoshowInvItem.Ore7
                        : Etc.NoshowInvItem.Ore8;
                }
                break;
            case Config.GOBLIN_DUNGEON_STAGE:
            case Config.GOBLIN_DUNGEON_STAGE + 1:
            case Config.GOBLIN_DUNGEON_STAGE + 2: //* 唯一にStageSelectedStageが＋して分けている（ゴブリン敵イメージを異なるため）
            {
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
                break;
            }
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

        rwlm.ShowReward(rewardList);
        HM._.rwm.UpdateInventory(rewardList);
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
