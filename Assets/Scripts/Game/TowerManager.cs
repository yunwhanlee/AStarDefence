using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class TowerManager : MonoBehaviour {
    public readonly static int CARD_UPG_LV_MAX = 10;

    [field: SerializeField] public int[] TowerCardUgrLvs {get; set;}
    [Header("WARRIOR")]
    [SerializeField] GameObject[] warriors; public GameObject[] Warriors {get => warriors;}
    [field: SerializeField] public Transform WarriorGroup {get; private set;}
    [Header("ARCHER")]
    [SerializeField] GameObject[] archers; public GameObject[] Archers {get => archers;}
    [field: SerializeField] public Transform ArcherGroup {get; private set;}
    [Header("MAGICIAN")]
    [SerializeField] GameObject[] magicians; public GameObject[] Magicians {get => magicians;}
    [field: SerializeField] public Transform MagicianGroup {get; private set;}

    [Header("CC")]
    [SerializeField] GameObject[] iceTowers; public GameObject[] IceTowers {get => iceTowers;}
    [SerializeField] GameObject[] stunTowers; public GameObject[] StunTowers {get => stunTowers;}
    [field: SerializeField] public Transform CCTowerGroup {get; private set;}
    [Header("BOARD")]
    [SerializeField] GameObject[] boards; public GameObject[] Boards {get => boards;}
    [field: SerializeField] public Transform BoardGroup {get; private set;}

    TowerManager tm;
    TileMapController tmc;

    void Start() {
        tm = GM._.tm;
        tmc = GM._.tmc;
        TowerCardUgrLvs = new int[3] {1, 1, 1};
    }

#region CREATE
    public void InstallBoard() {
        Debug.Log("InstallBoard()::");
        GameObject ins = Instantiate(boards[Random.Range(0, boards.Length)], tm.BoardGroup);
        ins.transform.localPosition = tmc.getCurSelectedPos();
        int boardCnt = tm.WarriorGroup.childCount + tm.ArcherGroup.childCount + tm.MagicianGroup.childCount;
        ins.name = $"Board{boardCnt}";
        tmc.HitObject = ins;
    }
    private void InstantiateTower(GameObject towerObj, Transform objGroup) {
        //* タワー → Board子に入れる
        Tower tower = Instantiate(towerObj, tmc.HitObject.transform).GetComponent<Tower>();
        tower.transform.localPosition = new Vector2(0, 0.15f); //* 少し上で、Board上にのせるように
        //* そのBoard → Group子に入れる
        tmc.HitObject.transform.SetParent(objGroup);
        //TODO Delete処理
    }
    private void InstallIceTower(int lvIdx) {
        Debug.Log("InstallIceTower()::");
        GameObject ccTower = Instantiate(iceTowers[lvIdx], CCTowerGroup);
        ccTower.transform.position = tmc.getCurSelectedPos();
        tmc.HitObject = ccTower;
    }
    private void InstallStunTower(int lvIdx) {
        Debug.Log("InstallStunTower()::");
        GameObject ccTower = Instantiate(stunTowers[lvIdx], CCTowerGroup);
        ccTower.transform.position = tmc.getCurSelectedPos();
        tmc.HitObject = ccTower;
    }

    public void CreateTower(TowerType type, int lvIdx = 0) {
        Debug.Log($"<color=white>CreateTower({type}, {lvIdx})::</color>");
        switch(type) {
            case TowerType.Random:
                //* 種類の選択
                int randKind = Random.Range(0, 3);
                switch(randKind) {
                    case (int)TowerKind.Warrior:
                        InstantiateTower(warriors[lvIdx], WarriorGroup);
                        break;
                    case (int)TowerKind.Archer:
                        InstantiateTower(archers[lvIdx], ArcherGroup);
                        break;
                    case (int)TowerKind.Magician:
                        InstantiateTower(magicians[lvIdx], MagicianGroup);
                        break;
                }
                //* タワー設置 トリガー ON
                tmc.HitObject.GetComponent<Board>().IsTowerOn = true;
                tmc.HitObject.GetComponentInChildren<Tower>().trc.SprRdr.enabled = true;
                break;
            case TowerType.CC_IceTower:
                InstallIceTower(lvIdx);
                tmc.HitObject.GetComponent<Tower>().trc.SprRdr.enabled = true;
                break;
            case TowerType.CC_StunTower:
                InstallStunTower(lvIdx);
                tmc.HitObject.GetComponent<Tower>().trc.SprRdr.enabled = true;
                break;
        }
    }
#endregion
#region TOWER RANGE 表示
    public void ClearAllTowerRanges() {
        foreach(Transform child in WarriorGroup) {
            Board board = child.GetComponent<Board>();
            if(board.IsTowerOn)
                child.GetComponentInChildren<WarriorTower>().trc.SprRdr.enabled = false;
        }
        foreach(Transform child in ArcherGroup) {
            Board board = child.GetComponent<Board>();
            if(board.IsTowerOn)
                board.GetComponentInChildren<ArcherTower>().trc.SprRdr.enabled = false;
        }
        foreach(Transform child in MagicianGroup) {
            Board board = child.GetComponent<Board>();
            if(board.IsTowerOn)
                board.GetComponentInChildren<MagicianTower>().trc.SprRdr.enabled = false;
        }
        foreach(Transform child in CCTowerGroup) {
            Tower tower = child.GetComponent<Tower>();
            switch(tower.Type) {
                case TowerType.CC_IceTower:
                    var icetower = tower as IceTower;
                    icetower.trc.SprRdr.enabled = false;
                    break;
                case TowerType.CC_StunTower:
                    var stunTower = tower as StunTower;
                    stunTower.trc.SprRdr.enabled = false;
                    break;
            }            
        }
    }
#endregion
#region UPGRADE CARD
    public void UpgradeTowerCard(TowerKind kind) {
        Debug.Log($"UpgradeTowerCard({kind}):: UPGRADE!");
        switch(kind) {
            case TowerKind.Warrior:
                TowerCardUgrLvs[(int)TowerKind.Warrior]++;
                for(int i = 0; i < WarriorGroup.childCount; i++) {
                    var warrior = WarriorGroup.GetChild(i).GetComponentInChildren<WarriorTower>();
                    warrior.Upgrade();
                }

                break;
            case TowerKind.Archer:
                GM._.tm.TowerCardUgrLvs[(int)TowerKind.Archer]++;
                for(int i = 0; i < ArcherGroup.childCount; i++) {
                    var archer = ArcherGroup.GetChild(i).GetComponentInChildren<ArcherTower>();
                    archer.Upgrade();
                }
                break;
            case TowerKind.Magician:
                GM._.tm.TowerCardUgrLvs[(int)TowerKind.Magician]++;
                for(int i = 0; i < MagicianGroup.childCount; i++) {
                    var magician = MagicianGroup.GetChild(i).GetComponentInChildren<MagicianTower>();
                    magician.Upgrade();
                }
                break;
        }
        GM._.gui.UpdateTowerCardLvUI();
    }
#endregion
}
