using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GM : MonoBehaviour {
    public static GM _; //* Global

    //* Outside
    public GameUIManager gui;
    public PathFindManager pfm;
    public EnemyManager emm;
    public TileMapController tmc;
    public ActionBarUIManager actBar;
    public TowerManager tm;

    void Awake() {
        //* Global化 値 代入
        _ = this; 
        //* 外部のスクリプト 初期化
        gui = GameObject.Find("GameUIManager").GetComponent<GameUIManager>();
        pfm = GameObject.Find("PathFindManager").GetComponent<PathFindManager>();
        emm = GameObject.Find("EnemyManager").GetComponent<EnemyManager>();
        tmc = GameObject.Find("TileMapController").GetComponent<TileMapController>();
        actBar = GameObject.Find("ActionBarUIManager").GetComponent<ActionBarUIManager>();
        tm = GameObject.Find("TowerManager").GetComponent<TowerManager>();
    }
}
