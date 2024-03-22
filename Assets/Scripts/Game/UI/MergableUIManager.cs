using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class MergableItem {
    const int W = 0, A = 1, M = 2; // Warrior, Archer, Magician
    [field:SerializeField] public GameObject Obj {get; private set;}
    [field:SerializeField] public Image BgBtnImg {get; private set;}
    [field:SerializeField] public Image PlusImg {get; private set;}
    [field:SerializeField] public Image TowerImg {get; private set;}
    [field:SerializeField] public TextMeshProUGUI StarTxt {get; private set;}

    public void SetUI(MergableUIManager mgb, int typeIdx, int lv) {
        TowerImg.sprite = (typeIdx == W)? mgb.WarriorSprs[lv] : (typeIdx == A)? mgb.ArcherSprs[lv] : mgb.MagicianSprs[lv];
        BgBtnImg.sprite = mgb.BtnSprs[typeIdx];
        PlusImg.sprite = mgb.PlusIconSprs[typeIdx];
        //* ★つける
        string starType = (typeIdx == W)? "Red" : (typeIdx == A)? "Blue" : "Yellow";
        for(int j = 0; j <= lv; j++)
            StarTxt.text += $"<sprite name={starType}Star>";
    }
}

public class MergableUIManager : MonoBehaviour {
    const int W = 0, A = 1, M = 2; // Warrior, Archer, Magician

    //* Resource
    [field:SerializeField] public Sprite[] BtnSprs {get; private set;}
    [field:SerializeField] public Sprite[] PlusIconSprs {get; private set;}
    [field:SerializeField] public Sprite[] WarriorSprs {get; private set;}
    [field:SerializeField] public Sprite[] ArcherSprs {get; private set;}
    [field:SerializeField] public Sprite[] MagicianSprs {get; private set;}

    //* Value
    [field:SerializeField] public MergableItem[] MergableItems {get; set;}

    void Start() {
        foreach(var item in MergableItems) //* 初期化
            item.Obj.SetActive(false);
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
            string type = splits[0];
            int typeIdx = type == "w"? W : type == "a"? A : M;
            int lv = int.Parse(splits[1]);

            MergableItems[i].Obj.SetActive(true);
            MergableItems[i].SetUI(this, typeIdx, lv);
        }
    }


}
