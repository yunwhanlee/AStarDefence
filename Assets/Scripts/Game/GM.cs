using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
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
    public TileMapController tmc;
    public ActionBarUIManager actBar;
    public BossRewardUIManager bossRwd;
    public TowerManager tm;
    public MissileManager mm;

    void Awake() {
        //* Global化 値 代入
        _ = this;

        //* 外部のスクリプト 初期化
        gui = GameObject.Find("GameUIManager").GetComponent<GameUIManager>();
        gef = GameObject.Find("GameEffectManager").GetComponent<GameEffectManager>();
        pfm = GameObject.Find("PathFindManager").GetComponent<PathFindManager>();
        em = GameObject.Find("EnemyManager").GetComponent<EnemyManager>();
        tmc = GameObject.Find("TileMapController").GetComponent<TileMapController>();
        actBar = GameObject.Find("ActionBarUIManager").GetComponent<ActionBarUIManager>();
        bossRwd = GameObject.Find("BossRewardUIManager").GetComponent<BossRewardUIManager>();
        tm = GameObject.Find("TowerManager").GetComponent<TowerManager>();
        mm = GameObject.Find("MissileManager").GetComponent<MissileManager>();

        state = GameState.Ready;
        CorReadyWaveID = null;
        IsReady = false;
        Stage = DM._ == null? 0 : DM._.SelectedStage;
        Array.ForEach(StageDts, stageDt => stageDt.TileMapObj.SetActive(false)); //* 非表示 初期化
        MaxWave = StageDts[Stage].EnemyData.Waves.Length;
        WaveCnt = 0;
        ResetCnt = Config.DEFAULT_RESET_CNT;
        life = Config.DEFAULT_LIFE;
        MaxLife = life;
        Money = Config.DEFAULT_MONEY;

        //* 現在選択したステージのタイルマップ 表示
        StageDts[Stage].TileMapObj.SetActive(true);

        gui.SetNextEnemyInfoFlagUI();
        gui.SwitchGameStateUI(state);

        //* ステージタイトルと難易度 表示 アニメーション
        string difficulty = (DM._ == null)? "TEST"
            : (DM._.SelectedDiff == Enum.Difficulty.Easy)? "EASY"
            : (DM._.SelectedDiff == Enum.Difficulty.Normal)? "NORMAL"
            : "HARD";
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
            gui.VictoryPopUp.SetActive(true);
            gui.Pause();
            return;
        }

        //* ボスリワード 表示
        if(WaveCnt % 10 == 0) {
            bossRwd.Active(2 + WaveCnt / 10);
        }
    }
    /// <summary>
    /// ライフ減る
    /// </summary>
    /// <param name="type">敵のタイプ（一般、空、ボス）</param>
    public void DecreaseLife(EnemyType type) {
        Util._.Blink(gui.HeartFillImg);

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
