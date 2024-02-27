using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class TowerManager : MonoBehaviour {
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
    }

#region FUNC
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
                const int WARRIOR = 0, ARCHER = 1, MAGICIAN = 2;
                //* 種類の選択
                int randKind = Random.Range(0, 3);
                switch(randKind) {
                    case WARRIOR: 
                        InstantiateTower(warriors[lvIdx], WarriorGroup);
                        break;
                    case ARCHER:
                        InstantiateTower(archers[lvIdx], ArcherGroup);
                        break;
                    case MAGICIAN:
                        InstantiateTower(magicians[lvIdx], MagicianGroup);
                        break;
                }
                //* タワー設置 トリガー ON
                tmc.HitObject.GetComponent<Board>().IsTowerOn = true;
                tmc.HitObject.GetComponentInChildren<Tower>().TowerRangeSprRdr.enabled = true;
                break;
            case TowerType.CC_IceTower:
                InstallIceTower(lvIdx);
                tmc.HitObject.GetComponent<Tower>().TowerRangeSprRdr.enabled = true;
                break;
            case TowerType.CC_StunTower:
                InstallStunTower(lvIdx);
                tmc.HitObject.GetComponent<Tower>().TowerRangeSprRdr.enabled = true;
                break;
        }
    }

    public void ClearAllTowerRanges() {
        foreach(Transform child in WarriorGroup) {
            Board board = child.GetComponent<Board>();
            if(board.IsTowerOn)
                child.GetComponentInChildren<WarriorTower>().TowerRangeSprRdr.enabled = false;
        }
        foreach(Transform child in ArcherGroup) {
            Board board = child.GetComponent<Board>();
            if(board.IsTowerOn)
                board.GetComponentInChildren<ArcherTower>().TowerRangeSprRdr.enabled = false;
        }
        foreach(Transform child in MagicianGroup) {
            Board board = child.GetComponent<Board>();
            if(board.IsTowerOn)
                board.GetComponentInChildren<MagicianTower>().TowerRangeSprRdr.enabled = false;
        }
        foreach(Transform child in CCTowerGroup) {
            Tower tower = child.GetComponent<Tower>();
            switch(tower.Type) {
                case TowerType.CC_IceTower:
                    var icetower = tower as IceTower;
                    icetower.TowerRangeSprRdr.enabled = false;
                    break;
                case TowerType.CC_StunTower:
                    var stunTower = tower as StunTower;
                    stunTower.TowerRangeSprRdr.enabled = false;
                    break;
            }            
        }
    }
#endregion
}
