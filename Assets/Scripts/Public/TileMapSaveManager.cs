using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WallDt {
    [field:SerializeField] public Vector3Int Pos {get; set;}
    public WallDt(Vector3Int pos) {
        Pos = pos;
    }
}

[Serializable]
public class TowerDt {
    [field:SerializeField] public TowerType TowerType {get; set;}
    [field:SerializeField] public TowerKind TowerKind {get; set;}
    [field:SerializeField] public int TowerLv {get; set;}
    [field:SerializeField] public Vector3Int Pos {get; set;}

    public TowerDt(TowerType type, TowerKind kind, int lv, Vector3Int pos) {
        TowerType = type;
        TowerKind = kind;
        TowerLv = lv;
        Pos = pos;
    }
}

[Serializable]
public class TileMapSaveDt {
    [field:SerializeField] public bool IsSaved {get; set;}
    [field:SerializeField] public bool IsRevived {get; set;}

    [field:SerializeField] public int Stage {get; set;}
    [field:SerializeField] public Enum.StageNum StageNum {get; set;}
    [field:SerializeField] public int Wave {get; set;}

    [field:SerializeField] public int MaxLife {get; set;}
    [field:SerializeField] public int Life {get; set;}
    [field:SerializeField] public int Money {get; set;}
    [field:SerializeField] public int[] TowerUpgrades {get; set;} = new int[3];

    [field:SerializeField] public List<WallDt> WallDtList {get; set;} = new List<WallDt>();
    [field:SerializeField] public List<TowerDt> SaveBoardList {get; set;} = new List<TowerDt>();
    [field:SerializeField] public List<TowerDt> SaveWarriorList {get; set;} = new List<TowerDt>();
    [field:SerializeField] public List<TowerDt> SaveArcherList {get; set;} = new List<TowerDt>();
    [field:SerializeField] public List<TowerDt> SaveMagicianList {get; set;} = new List<TowerDt>();
    [field:SerializeField] public List<TowerDt> SaveCCTowerList {get; set;} = new List<TowerDt>();

    public TileMapSaveDt() {
        Reset();
    }

    public void Reset() {
        IsSaved = false;
        IsRevived = false;
        Stage = 0;
        StageNum = 0;
        Wave = 1;

        MaxLife = 10;
        Life = 10;
        Money = 100;
        TowerUpgrades = new int[3];

        WallDtList = new List<WallDt>();
        SaveBoardList = new List<TowerDt>();
        SaveWarriorList = new List<TowerDt>();
        SaveArcherList = new List<TowerDt>();
        SaveMagicianList = new List<TowerDt>();
        SaveCCTowerList = new List<TowerDt>();
    }
}
