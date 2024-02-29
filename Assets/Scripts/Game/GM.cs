using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;


public enum GameState {Ready, Play, Pause};

public class GM : MonoBehaviour {
    
    public static GM _; //* Global
    [SerializeField] GameState state;   public GameState State {get => state; set => state = value;}
    [field: SerializeField] public int Stage {get; set;}
    [field: SerializeField] public int Money {get; set;}

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
        Stage = 1;
        Money = 0;
        gui.SwitchGameStateUI(state);
    }

#region EVENT
    public void OnClickStartBtn() {
        state = GameState.Play;
        gui.SwitchGameStateUI(state);
        gui.EnemyCntTxt.text = $"{EnemyManager.CREATE_CNT} / {EnemyManager.CREATE_CNT}";
        pfm.PathFinding();
        StartCoroutine(em.CoCreateEnemy());
    }
#endregion

#region
/// <summary>
/// 現在のステージが終わったので、ゲーム状態をReadyに戻す
/// </summary>
    public void FinishRaid() {
        state = GameState.Ready;
        gui.SwitchGameStateUI(state);
    }
#endregion
}
