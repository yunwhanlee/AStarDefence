using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MergableUIManager : MonoBehaviour {
    const int W = 0, A = 1, M = 2; // Warrior, Archer, Magician

    //* Resource
    [field:SerializeField] public Sprite[] BtnSprs {get; private set;}
    [field:SerializeField] public Sprite[] PlusIconSprs {get; private set;}
    [field:SerializeField] public Sprite[] WarriorSprs {get; private set;}
    [field:SerializeField] public Sprite[] ArcherSprs {get; private set;}
    [field:SerializeField] public Sprite[] MagicianSprs {get; private set;}

    //* Value
    [field:SerializeField] public GameObject[] Items {get; private set;}
    [field:SerializeField] public Image[] BgBtnImgs {get; private set;}
    [field:SerializeField] public Image[] PlusImgs {get; private set;}
    [field:SerializeField] public Image[] TowerImgs {get; private set;}
    [field:SerializeField] public TextMeshProUGUI[] StarTxts {get; private set;}

    void Start() {
        foreach(var item in Items) //* 初期化
            item.SetActive(false);
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
        for(int i = 0; i < Items.Length; i++) {
            Items[i].SetActive(false);
            StarTxts[i].text = "";
        }

        for(int i = 0; i < mergableList.Count; i++) {
            if(i > Items.Length) return;

            string[] splits = mergableList[i].Split("_");
            string type = splits[0];
            int typeIdx = type == "w"? W : type == "a"? A : M;
            int lv = int.Parse(splits[1]);

            Items[i].SetActive(true);
            
            switch(typeIdx) {
                case W: 
                    TowerImgs[i].sprite = WarriorSprs[lv];
                    BgBtnImgs[i].sprite = BtnSprs[W];
                    PlusImgs[i].sprite = PlusIconSprs[W];
                    for(int j = 0; j <= lv; j++) StarTxts[i].text += "<sprite name=RedStar>";
                    break;
                case A:
                    TowerImgs[i].sprite = ArcherSprs[lv];
                    BgBtnImgs[i].sprite = BtnSprs[A];
                    PlusImgs[i].sprite = PlusIconSprs[A];
                    for(int j = 0; j <= lv; j++) StarTxts[i].text += "<sprite name=BlueStar>";
                    break;
                case M:
                    TowerImgs[i].sprite = MagicianSprs[lv];
                    BgBtnImgs[i].sprite = BtnSprs[M];
                    PlusImgs[i].sprite = PlusIconSprs[M];
                    for(int j = 0; j <= lv; j++) StarTxts[i].text += "<sprite name=YellowStar>";
                    break;
            }
        }
    }


}
