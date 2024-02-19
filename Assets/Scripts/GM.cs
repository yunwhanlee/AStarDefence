using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GM : MonoBehaviour {
    public static GM _; //* Global

    //* Outside
    public PathFindManager pfm;
    public EnemyManager emm;
    public TileMapController tmc;

    void Awake() {
        _ = this; //* Global化 値 代入
    }

    void Start() {
    }
}
