using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

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
    public readonly static int RESET_WALL_MAX = 5;

    public static GM _; //* Global

    [SerializeField] GameState state;   public GameState State {get => state; set => state = value;}
    [field: SerializeField] public StageData[] StageDts;
    [field: SerializeField] public int Stage {get; set;}
    [field: SerializeField] public int MaxWave {get; set;}
    [field: SerializeField] public int WaveCnt {get; set;}
    [field: SerializeField] public int ResetCnt {get; set;}

    [field: SerializeField] public int MaxLife {get; set;}
    [field: SerializeField] public int Life {get; set;}

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
        Stage = 0;
        Array.ForEach(StageDts, stageDt => stageDt.TileMapObj.SetActive(false)); //* 非表示 初期化
        MaxWave = StageDts[Stage].EnemyData.Waves.Length;
        WaveCnt = 0;
        ResetCnt = 5;
        Life = 10;
        MaxLife = Life;
        Money = 1000;

        //* 現在選択したステージのタイルマップ 表示
        StageDts[Stage].TileMapObj.SetActive(true);

        gui.SetNextEnemyInfoFlagUI();
        gui.SwitchGameStateUI(state);
    }

#region EVENT
    //! DEBUG
    public void OnClickNextTower() {
        if(tmc.HitObject == null) {
            gui.ShowMsgError("PLEASE SELECT TOWER");
            return;
        }

        var tower = tmc.HitObject.GetComponentInChildren<Tower>();
        if(tower.Lv > 5) {
            gui.ShowMsgError("MAX LV");
            return;
        }

        //* 次のレベルタワーランダムで生成
        tm.CreateTower(tower.Type, tower.Lv++, tower.Kind);
        //* 自分を削除
        DestroyImmediate(tower.gameObject);
        actBar.UpdateUI(Enum.Layer.Tower);
    }

    public void OnClickStartBtn() =>StartWave();
#endregion

#region FUNC
    public EnemyData GetCurEnemyData() => StageDts[Stage].EnemyData.Waves[WaveCnt - 1];
    public EnemyData GetNextEnemyData() => StageDts[Stage].EnemyData.Waves[WaveCnt];

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

        if(gui.ResetWallBtn.gameObject.activeSelf)
            gui.ResetWallBtn.gameObject.SetActive(false);
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
        gui.SetNextEnemyInfoFlagUI();

        //* ボスリワード 表示
        if(WaveCnt % 10 == 0) {
            bossRwd.Active(3);
        }
    }
    /// <summary>
    /// ライフ減る
    /// </summary>
    /// <param name="type">敵のタイプ（一般、空、ボス）</param>
    public void DecreaseLife(EnemyType type) {
        Util._.Blink(gui.HeartFillImg);
        Life -= (type == EnemyType.Boss)? Enemy.LIFE_DEC_BOSS : Enemy.LIFE_DEC_MONSTER;
        gui.HeartFillImg.fillAmount = (float)Life / MaxLife;

        //* ゲームオーバ
        if(Life <= 0) {
            State = GameState.Gameover;
            Life = 0;
            Debug.Log("GAMEOVER");
            gui.GameoverPopUp.SetActive(true);
        }

        gui.LifeTxt.text = Life.ToString();
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
