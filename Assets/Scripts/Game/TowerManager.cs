using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class TowerManager : MonoBehaviour {
    [Header("WARRIOR")]
    [SerializeField] GameObject[] warriors; public GameObject[] Warriors {get => warriors;}
    [field: SerializeField] public List<GameObject> WarriorList {get; private set;} = new List<GameObject>();
    [Header("ARCHER")]
    [SerializeField] GameObject[] archers; public GameObject[] Archers {get => archers;}
    [field: SerializeField] public List<GameObject> ArchersList {get; private set;} = new List<GameObject>();
    [Header("MAGICIAN")]
    [SerializeField] GameObject[] magicians; public GameObject[] Magicians {get => magicians;}
    [field: SerializeField] public List<GameObject> MagicianList {get; private set;} = new List<GameObject>();
    [Header("CC")]
    [SerializeField] GameObject[] iceTowers; public GameObject[] IceTowers {get => iceTowers;}
    [SerializeField] GameObject[] stunTowers; public GameObject[] StunTowers {get => stunTowers;}
    [field: SerializeField] public List<GameObject> CCTowerList {get; private set;} = new List<GameObject>();

    void Start() {
    }

#region FUNC
    private void InstantiateTower(GameObject towerObj, List<GameObject> listGroup) {
        GameObject obj = Instantiate(towerObj, GM._.tmc.HitObject.transform);
        obj.transform.localPosition = new Vector2(0, 0.15f); //* 少し上で、Board上にのせるように
        listGroup.Add(obj); //TODO Delete処理
    }

    public void CreateTower(TowerType type) {
        switch(type) {
            case TowerType.Random:
                const int WARRIOR = 0, ARCHER = 1, MAGICIAN = 2;
                //* 種類の選択
                int randKind = Random.Range(0, 3);
                switch(randKind) {
                    case WARRIOR: 
                        InstantiateTower(warriors[0], WarriorList);
                        break;
                    case ARCHER:
                        InstantiateTower(archers[0], ArchersList);
                        break;
                    case MAGICIAN:
                        InstantiateTower(magicians[0], MagicianList);
                        break;
                }
                break;
            case TowerType.CC_IceTower:
                GM._.tmc.InstallIceTowerTile();
                break;
            case TowerType.CC_StunTower:
                GM._.tmc.InstallStunTowerTile();
                break;
        }
    }
#endregion
}
