using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;



public class GM : MonoBehaviour {
    public enum State {Ready, Play, Pause};
    public static GM _; //* Global
    public State state;

    //* Outside
    public GameUIManager gui;
    public PathFindManager pfm;
    public EnemyManager em;
    public TileMapController tmc;
    public ActionBarUIManager actBar;
    public TowerManager tm;
    public TowerStateUIManager tsm;
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
        tsm = GameObject.Find("TowerStateUIManager").GetComponent<TowerStateUIManager>();
        mm = GameObject.Find("MissileManager").GetComponent<MissileManager>();
    }

    void Start() {
        state = State.Ready;
    }

#region EVENT
    public void OnClickStartBtn() {
        state = State.Play;
        pfm.PathFinding();
        StartCoroutine(em.CoCreateEnemy());
    }
#endregion
}
