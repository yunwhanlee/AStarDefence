using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;


public enum GameState {Ready, Play, Pause, Gameover};

public class GM : MonoBehaviour {
    public static GM _; //* Global

    [SerializeField] GameState state;   public GameState State {get => state; set => state = value;}
    [field: SerializeField] public int Map {get; set;}
    [field: SerializeField] public int MaxStage {get; set;}
    [field: SerializeField] public int Stage {get; set;}
    [field: SerializeField] public int MaxLife {get; set;}
    [field: SerializeField] public int Life {get; set;}
    [field: SerializeField] public int Money {get; set;}
    [field: SerializeField] public Material BlinkMt;
    [field: SerializeField] public Material DefaultMt;

    //* Outside
    public GameUIManager gui;
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
        pfm = GameObject.Find("PathFindManager").GetComponent<PathFindManager>();
        em = GameObject.Find("EnemyManager").GetComponent<EnemyManager>();
        tmc = GameObject.Find("TileMapController").GetComponent<TileMapController>();
        actBar = GameObject.Find("ActionBarUIManager").GetComponent<ActionBarUIManager>();
        tm = GameObject.Find("TowerManager").GetComponent<TowerManager>();
        mm = GameObject.Find("MissileManager").GetComponent<MissileManager>();

        state = GameState.Ready;
        Map = 0;
        MaxStage = 10;
        Stage = 0;
        Life = 10;
        MaxLife = Life;
        Money = 0;
        gui.SwitchGameStateUI(state);
    }

#region EVENT
    /// <summary>
    /// レイド開始
    /// </summary>
    public void OnClickStartBtn() {
        state = GameState.Play;
        gui.StageTxt.text = $"STAGE {++Stage}";
        gui.SwitchGameStateUI(state);
        gui.EnemyCntTxt.text = $"{EnemyManager.CREATE_CNT} / {EnemyManager.CREATE_CNT}";
        pfm.PathFinding();
        StartCoroutine(em.CoCreateEnemy());
    }
#endregion

#region FUNC
    /// <summary>
    /// レイド終了
    /// </summary>
    public void FinishRaid() {
        state = GameState.Ready;
        gui.StageTxt.text = $"STAGE {Stage} / {MaxStage}";
        gui.SwitchGameStateUI(state);
    }

    public void DecreaseLife(EnemyType type) {
        Util._.Blink(gui.HeartFillImg);
        Life -= (type == EnemyType.Boss)? Enemy.LIFE_DEC_BOSS : Enemy.LIFE_DEC_MONSTER;
        gui.HeartFillImg.fillAmount = (float)Life / MaxLife;
        gui.LifeTxt.text = Life.ToString();
        //* ゲームオーバ
        if(Life <= 0) {
            State = GameState.Gameover;
            Life = 0;
            Debug.Log("GAMEOVER");
        }
    }
#endregion
}
