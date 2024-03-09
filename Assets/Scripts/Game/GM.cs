using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;


public enum GameState {Ready, Play, Pause, Gameover};

public class GM : MonoBehaviour {
    public readonly static int RESET_WALL_MAX = 5;

    public static GM _; //* Global

    [SerializeField] GameState state;   public GameState State {get => state; set => state = value;}
    [field: SerializeField] public int Map {get; set;}
    [field: SerializeField] public int MaxWave {get; set;}
    [field: SerializeField] public int Wave {get; set;}
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
    public ActionBarUIManager actBar; //TODO Move To GameUIManager
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
        tm = GameObject.Find("TowerManager").GetComponent<TowerManager>();
        mm = GameObject.Find("MissileManager").GetComponent<MissileManager>();

        state = GameState.Ready;
        Map = 0;
        MaxWave = em.StageDatas[Map].WaveCount;
        Wave = 0;
        ResetCnt = 5;
        Life = 10;
        MaxLife = Life;
        Money = 1000;
        gui.SetNextEnemyInfoFlagUI();
        gui.SwitchGameStateUI(state);
    }

#region EVENT
    /// <summary>
    /// レイド開始
    /// </summary>
    public void OnClickStartBtn() {
        state = GameState.Play;
        gui.WaveTxt.text = $"WAVE {++Wave}";
        gui.EnemyCntTxt.text = $"{em.EnemyCnt} / {em.EnemyCnt}";
        gui.SwitchGameStateUI(state);
        pfm.PathFinding();
        StartCoroutine(em.CoCreateEnemy());

        if(gui.ResetWallBtn.gameObject.activeSelf)
            gui.ResetWallBtn.gameObject.SetActive(false);
    }
#endregion

#region FUNC
    /// <summary>
    /// レイド終了
    /// </summary>
    public void FinishRaid() {
        state = GameState.Ready;
        gui.WaveTxt.text = $"WAVE {Wave} / {MaxWave}";
        gui.SwitchGameStateUI(state);

        //* Next Enemy Info UI
        gui.SetNextEnemyInfoFlagUI();
    }
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
            StartCoroutine(gui.CoShowMsgError("비용이 부족합니다."));
            return false;
        }
        else {
            SetMoney(-price); //* お金減る
        }
        return true;
    }
#endregion
}
