using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

[System.Serializable]
public class MergableItem {
    //* Value
    [field:SerializeField] public int Lv {get; set;}
    [field:SerializeField] public TowerKind Kind {get; set;}

    //* UI Element
    [field:SerializeField] public GameObject Obj {get; private set;}
    [field:SerializeField] public Image BgBtnImg {get; private set;}
    [field:SerializeField] public Image PlusImg {get; private set;}
    [field:SerializeField] public Image TowerImg {get; private set;}
    [field:SerializeField] public TextMeshProUGUI StarTxt {get; private set;}

    public void SetUI(MergableUIManager mgb, TowerKind kind, int lv) {
        //* Data
        Lv = lv + 1;
        Kind = kind;

        //* UI
        TowerImg.sprite = (kind == TowerKind.Warrior)? mgb.WarriorSprs[lv]
            : (kind == TowerKind.Archer)? mgb.ArcherSprs[lv]
            : mgb.MagicianSprs[lv];
        BgBtnImg.sprite = mgb.BtnSprs[(int)kind];
        PlusImg.sprite = mgb.PlusIconSprs[(int)kind];
        // ★つける
        string starType = (kind == TowerKind.Warrior)? "Red" : (kind == TowerKind.Archer)? "Blue" : "Yellow";
        for(int j = 0; j <= lv; j++)
            StarTxt.text += $"<sprite name={starType}Star>";
    }
}

public class MergableUIManager : MonoBehaviour {
    //* Resource
    [field:SerializeField] public Sprite[] BtnSprs {get; private set;}
    [field:SerializeField] public Sprite[] PlusIconSprs {get; private set;}
    [field:SerializeField] public Sprite[] WarriorSprs {get; private set;}
    [field:SerializeField] public Sprite[] ArcherSprs {get; private set;}
    [field:SerializeField] public Sprite[] MagicianSprs {get; private set;}

    //* Value
    [field:SerializeField] public MergableItem[] MergableItems {get; set;}
    [field:SerializeField] public List<Tower> MergableTowerList = new List<Tower>();
    [field:SerializeField] public int CurIdx = -1;
    [field:SerializeField] public int mergableIdx = 0;

    void Start() {
        foreach(var item in MergableItems) //* 初期化
            item.Obj.SetActive(false);
    }

#region EVENT
    public void OnClickMergableItem(int idx) {
        if(CurIdx == idx) {
            Select(idx);
        }
        else {
            ResetData(idx);
            Select(idx);
        }
    }
#endregion

#region FUNC
    public void Init() {
        MergableTowerList.Clear();
        CurIdx = -1;
    }

    /// <summary>
    /// マージできるタワー順番で選択(クリック)
    /// </summary>
    /// <param name="idx">マージアイコンのインデックス</param>
    private void Select(int idx) {
        int len = MergableTowerList.Count;
        Tower tower = MergableTowerList[mergableIdx % len];

        Collider2D col = tower.GetComponentInParent<Board>().GetComponent<Collider2D>();
        int x = Mathf.RoundToInt(tower.transform.position.x);
        int y = Mathf.RoundToInt(tower.transform.position.y);
        Debug.Log($"OnClickMergableItem({idx}):: Select():: tower.name= {tower.name} ,x= {x}, y= {y}, col= {col}");

        //* マージできるタワー選択
        GM._.tmc.OnClickTile(x, y, col);

        mergableIdx++;
    }
    /// <summary>
    /// 他のマージアイコンを選択したら、Mergableに関した情報を初期化
    /// </summary>
    private void ResetData(int idx) {
            Debug.Log($"OnClickMergableItem({idx}):: Reset()::");
            CurIdx = idx;
            mergableIdx = 0;
            MergableTowerList.Clear();
            GM._.tmc.Reset(true);
            int lv = MergableItems[idx].Lv;
            TowerKind kind = MergableItems[idx].Kind;

            switch(kind) {
                case TowerKind.Warrior:
                    var warriorList = GM._.tm.WarriorGroup.GetComponentsInChildren<Tower>().ToList();
                    MergableTowerList = warriorList.FindAll(wr => wr.Lv == lv);
                    break;
                case TowerKind.Archer:
                    var archerList = GM._.tm.ArcherGroup.GetComponentsInChildren<Tower>().ToList();
                    MergableTowerList = archerList.FindAll(ac => ac.Lv == lv);
                    break;
                case TowerKind.Magician:
                    var magicianList = GM._.tm.MagicianGroup.GetComponentsInChildren<Tower>().ToList();
                    MergableTowerList = magicianList.FindAll(mg => mg.Lv == lv);
                    break;
            }
    }

    private int[] SeperateLvToArr(List<Tower> towerList) {
        int [] tempLvCntArr = new int[5];
        towerList.ForEach(wr => {
            if(wr.Lv == 1) {tempLvCntArr[wr.LvIdx]++;}
            else if(wr.Lv == 2) {tempLvCntArr[wr.LvIdx]++;}
            else if(wr.Lv == 3) {tempLvCntArr[wr.LvIdx]++;}
            else if(wr.Lv == 4) {tempLvCntArr[wr.LvIdx]++;}
            else if(wr.Lv == 5) {tempLvCntArr[wr.LvIdx]++;}
        });
        return tempLvCntArr;
    }

    

    public void Mergable() {
        //* Warrior
        var warriorList = GM._.tm.WarriorGroup.GetComponentsInChildren<Tower>().ToList();
        int[] wLvCntArr = SeperateLvToArr(warriorList); // レベル別に分ける
        //* Archer
        var archerList = GM._.tm.ArcherGroup.GetComponentsInChildren<Tower>().ToList();
        int[] aLvCntArr = SeperateLvToArr(archerList); // レベル別に分ける
        //* Magician
        var magicianList = GM._.tm.MagicianGroup.GetComponentsInChildren<Tower>().ToList();
        int[] mLvCntArr = SeperateLvToArr(magicianList); // レベル別に分ける

        //* マージできるタワーのみ抽出
        List<string> mergableList = new List<string>();
        for(int i = 0; i < wLvCntArr.Length; i++)
            if(wLvCntArr[i] > 1) mergableList.Add($"w_{i}");

        for(int i = 0; i < aLvCntArr.Length; i++)
            if(aLvCntArr[i] > 1) mergableList.Add($"a_{i}");

        for(int i = 0; i < mLvCntArr.Length; i++)
            if(mLvCntArr[i] > 1) mergableList.Add($"m_{i}");

        mergableList.ForEach(list => Debug.Log("mergableList= " + list));

        if(mergableList.Count < 0)
            return;

        //* 初期化
        for(int i = 0; i < MergableItems.Length; i++) {
            MergableItems[i].Obj.SetActive(false);
            MergableItems[i].StarTxt.text = "";
        }

        //* マージできるアイコン 表示
        for(int i = 0; i < mergableList.Count; i++) {
            if(i > MergableItems.Length) return;

            string[] splits = mergableList[i].Split("_");
            string kind = splits[0];
            TowerKind kindIdx = kind == "w"? TowerKind.Warrior : kind == "a"? TowerKind.Archer : TowerKind.Magician;
            int lv = int.Parse(splits[1]);

            MergableItems[i].Obj.SetActive(true);
            MergableItems[i].SetUI(this, kindIdx, lv);
        }
    }
#endregion
}
