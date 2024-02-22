using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class TowerManager : MonoBehaviour {
    [SerializeField] GameObject[] warriors;
    [field: SerializeField] public List<GameObject> WarriorList {get; private set;} = new List<GameObject>();
    [SerializeField] GameObject[] archors;
    [field: SerializeField] public List<GameObject> ArchorsList {get; private set;} = new List<GameObject>();
    [SerializeField] GameObject[] magicians;
    [field: SerializeField] public List<GameObject> MagicianList {get; private set;} = new List<GameObject>();
    [SerializeField] GameObject iceTower;
    [SerializeField] GameObject stunTower;
    [field: SerializeField] public List<GameObject> CCTowerList {get; private set;} = new List<GameObject>();

    SettingTowerData settingTowerData;

    void Start() {
        settingTowerData = new SettingTowerData();
    }

#region FUNC
    public void CreateTower(TowerType type) {
        Vector2Int pos = GM._.tmc.CurSelectPos;
        GameObject obj = null;
        switch(type) {
            case TowerType.Random:
                //* 種類の選択
                int randKind = Random.Range(0, 1); //TODO max = 3
                switch(randKind) {
                    case 0: 
                        obj = Instantiate(warriors[0], new Vector2(pos.x, pos.y), quaternion.identity);
                        obj.GetComponent<WarriorTower>().StateUpdate(settingTowerData);
                        WarriorList.Add(obj);
                        break;
                    case 1:
                        //TODO ArchorTower
                        break;
                    case 2:
                        //TODO MagicianTower
                        break;
                }
                break;
            case TowerType.CC_IceTower:
                obj = Instantiate(iceTower, new Vector2(pos.x, pos.y), quaternion.identity);
                obj.GetComponent<IceTower>().StateUpdate(settingTowerData);
                CCTowerList.Add(obj);
                break;
            case TowerType.CC_StunTower:
                obj = Instantiate(stunTower, new Vector2(pos.x, pos.y), quaternion.identity);
                obj.GetComponent<StunTower>().StateUpdate(settingTowerData);
                CCTowerList.Add(obj);
                break;
        }
    }
#endregion
}
