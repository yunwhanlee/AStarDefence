using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ロードする壁データ
/// </summary>
[Serializable]
public class WallDt {
    [field:SerializeField] public Vector3Int Pos {get; set;}
    public WallDt(Vector3Int pos) {
        Pos = pos;
    }
}

/// <summary>
/// ロードするタワーデータ
/// </summary>
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

/// <summary>
/// ステージ続きタイルマップのデータ
/// </summary>
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

    [field:SerializeField] public int SuccessionTicket {get; set;}
    [field:SerializeField] public int ChangeTypeTicket {get; set;}
    [field:SerializeField] public int IncreaseCCTowerCnt {get; set;}
    [field:SerializeField] public int SwitchTowerPositionCnt {get; set;}

    [field:SerializeField] public int FreeBoardCnt {get; set;}
    [field:SerializeField] public int FreeBreakRockCnt {get; set;}

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

        SuccessionTicket = 0;
        ChangeTypeTicket = 0;
        IncreaseCCTowerCnt = 0;
        SwitchTowerPositionCnt = 0;

        FreeBoardCnt = 0;
        FreeBreakRockCnt = 0;

        WallDtList = new List<WallDt>();
        SaveBoardList = new List<TowerDt>();
        SaveWarriorList = new List<TowerDt>();
        SaveArcherList = new List<TowerDt>();
        SaveMagicianList = new List<TowerDt>();
        SaveCCTowerList = new List<TowerDt>();
    }

    public void SaveDt(bool isInfiniteDungeon = false) {
        IsSaved = true;

        //* Statue Data
        IsRevived = GM._.IsRevived;
        Stage = GM._.Stage;
        StageNum = GM._.StageNum;
        Wave = Mathf.Max(0, GM._.WaveCnt + (GM._.gui.previousState == GameState.Play? -1 : 0));
        MaxLife = GM._.MaxLife;
        Life = GM._.Life;
        Money = GM._.Money;

        //* Upgrade Card Data
        TowerUpgrades[(int)TowerKind.Warrior] = GM._.tm.TowerCardUgrLvs[(int)TowerKind.Warrior];
        TowerUpgrades[(int)TowerKind.Archer] = GM._.tm.TowerCardUgrLvs[(int)TowerKind.Archer];
        TowerUpgrades[(int)TowerKind.Magician] = GM._.tm.TowerCardUgrLvs[(int)TowerKind.Magician];

        SuccessionTicket = GM._.actBar.SuccessionTicket;
        ChangeTypeTicket = GM._.actBar.ChangeTypeTicket;
        IncreaseCCTowerCnt = GM._.tm.CCTowerMax;
        SwitchTowerPositionCnt = GM._.actBar.SwitchCnt;

        FreeBoardCnt = Mathf.Max(0, GM._.actBar.FreeBoardCnt);
        FreeBreakRockCnt = Mathf.Max(0, GM._.actBar.FreeBreakRockCnt);

        SaveWallDt(isInfiniteDungeon);
        SaveBoardDt();
        SaveCCTowerDt();
        SaveTowerDt(GM._.tm.WarriorGroup, TowerKind.Warrior);
        SaveTowerDt(GM._.tm.ArcherGroup, TowerKind.Archer);
        SaveTowerDt(GM._.tm.MagicianGroup, TowerKind.Magician);
    }

    private void SaveWallDt(bool isInfiniteDungeon) {
        //* x, y値
        int left = isInfiniteDungeon? -7 : -6;
        int right = isInfiniteDungeon? 7 : 6;
        int top = isInfiniteDungeon? 2 : 2;
        int bottom = isInfiniteDungeon? -3 : -2;

        for(int x = left; x <= right; x++) {
            for(int y = bottom; y <= top; y++) {
                //! タイルマップを作るときに、XとYを逆にした。。。
                WallDt wallDt = new WallDt( new Vector3Int(y, x, 0));
                var tileDt = GM._.tmc.WallTileMap.GetTile(new Vector3Int(y, x, 0));
                if(tileDt) {
                    WallDtList.Add(wallDt); //* Tile List 追加
                }

                //? Debug Fill Walls to Test
                // GM._.tmc.WallTileMap.SetTile(new Vector3Int(y, x, 0), GM._.StageDts[1].Walls[0]);
            }
        }
    }

    private void SaveBoardDt() {
        for(int i = 0; i < GM._.tm.BoardGroup.childCount; i++) {
            Transform tf = GM._.tm.BoardGroup.GetChild(i);
            SaveBoardList.Add( new TowerDt (
                TowerType.Board,
                TowerKind.None,
                lv: 0,
                new Vector3Int((int)tf.position.x, (int)tf.position.y, 0)
            ));
        }
    }

    private void SaveTowerDt(Transform groupTf, TowerKind kind) {
        for(int i = 0; i < groupTf.childCount; i++) {
            Transform boardTf = groupTf.GetChild(i);
            if(kind == TowerKind.Warrior)
                SaveWarriorList.Add ( new TowerDt (TowerType.Random, kind, lv: boardTf.GetComponentInChildren<Tower>().Lv, new Vector3Int((int)boardTf.position.x, (int)boardTf.position.y, 0)));
            else if(kind == TowerKind.Archer)
                SaveArcherList.Add ( new TowerDt (TowerType.Random, kind, lv: boardTf.GetComponentInChildren<Tower>().Lv, new Vector3Int((int)boardTf.position.x, (int)boardTf.position.y, 0)));
            else if(kind == TowerKind.Magician)
                SaveMagicianList.Add ( new TowerDt (TowerType.Random, kind, lv: boardTf.GetComponentInChildren<Tower>().Lv, new Vector3Int((int)boardTf.position.x, (int)boardTf.position.y, 0)));
        }
    }

    private void SaveCCTowerDt() {
        for(int i = 0; i < GM._.tm.CCTowerGroup.childCount; i++) {
            Transform childTf = GM._.tm.CCTowerGroup.GetChild(i);
            Tower ccTower = childTf.GetComponent<Tower>();
            SaveCCTowerList.Add ( new TowerDt (
                ccTower.Type,
                TowerKind.None,
                lv: ccTower.Lv,
                new Vector3Int((int)childTf.position.x, (int)childTf.position.y, 0)
            ));
        }
    }

    public void LoadDt() {
        Debug.Log("TileMapSaveManager:: LoadDt()");
        GM._.gui.ResetWallBtn.gameObject.SetActive(false);
        var walls = GM._.StageDts[GM._.Stage].Walls;

        //* Status Dt
        GM._.IsRevived = IsRevived;
        GM._.Stage = Stage;
        DM._.SelectedStageNum = StageNum;
        GM._.WaveCnt = Wave;

        GM._.MaxLife = MaxLife;
        GM._.Life = Life;
        GM._.Money = Money;

        GM._.tm.TowerCardUgrLvs[(int)TowerKind.Warrior] = TowerUpgrades[(int)TowerKind.Warrior];
        GM._.tm.TowerCardUgrLvs[(int)TowerKind.Archer] = TowerUpgrades[(int)TowerKind.Archer];
        GM._.tm.TowerCardUgrLvs[(int)TowerKind.Magician] = TowerUpgrades[(int)TowerKind.Magician];

        GM._.actBar.SuccessionTicket = SuccessionTicket;
        GM._.actBar.ChangeTypeTicket = ChangeTypeTicket;
        GM._.tm.CCTowerMax = IncreaseCCTowerCnt;
        GM._.actBar.SwitchCnt = SwitchTowerPositionCnt;

        GM._.actBar.FreeBoardCnt = FreeBoardCnt;
        if(FreeBoardCnt > 0)
            GM._.actBar.BoardPriceTxt.text = $"<color=green>무료 {FreeBoardCnt}</color>";
        else
            GM._.actBar.BoardPriceTxt.text = $"{Config.G_PRICE.BOARD}";;

        GM._.actBar.FreeBreakRockCnt = FreeBreakRockCnt;
        if(FreeBreakRockCnt > 0)
            GM._.actBar.BreakPriceTxt.text = $"<color=green>무료 {FreeBreakRockCnt}</color>";
        else
            GM._.actBar.BreakPriceTxt.text = $"{Config.G_PRICE.BREAK}";

        //* Wall 配置
        WallDtList.ForEach(wallDt => {
            GM._.tmc.WallTileMap.SetTile(new Vector3Int(wallDt.Pos.x, wallDt.Pos.y, 0), walls[0]);
        });

        //* Board 配置
        SaveBoardList.ForEach(boardDt => {
            GM._.tm.InstallBoard(boardDt.Pos);
        });

        //* Warrior 配置
        SaveWarriorList.ForEach(wrDt => {
            GM._.tm.InstallBoard(wrDt.Pos);
            GM._.tm.CreateTower(TowerType.Random, wrDt.TowerLv - 1, TowerKind.Warrior, isActiveRange: false);
        });

        //* Archer 配置
        SaveArcherList.ForEach(acDt => {
            GM._.tm.InstallBoard(acDt.Pos);
            GM._.tm.CreateTower(TowerType.Random, acDt.TowerLv - 1, TowerKind.Archer, isActiveRange: false);
        });

        //* Magician 配置
        SaveMagicianList.ForEach(mgDt => {
            GM._.tm.InstallBoard(mgDt.Pos);
            GM._.tm.CreateTower(TowerType.Random, mgDt.TowerLv - 1, TowerKind.Magician, isActiveRange: false);
        });

        //* CCTower 配置
        SaveCCTowerList.ForEach(ccDt => {
            if(ccDt.TowerType == TowerType.CC_IceTower) {
                GM._.tm.InstallIceTower(ccDt.TowerLv - 1, ccDt.Pos);
                GM._.tmc.HitObject.GetComponent<Tower>().trc.SprRdr.enabled = false;
            }
            else {
                GM._.tm.InstallStunTower(ccDt.TowerLv - 1, ccDt.Pos);
                GM._.tmc.HitObject.GetComponent<Tower>().trc.SprRdr.enabled = false;
            }
        });
    }
}
