using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

[Serializable]
public class MiningCard {
    public MineCate Cate;
    public int Id;
    public string Name;
    [SerializeField] int cnt; public int Cnt {
        get => (Cate == MineCate.Goblin)? DM._.DB.MiningDB.GoblinCardCnts[Id]
            : DM._.DB.MiningDB.OreCardCnts[Id];
        set {
            if(Cate == MineCate.Goblin)
                DM._.DB.MiningDB.GoblinCardCnts[Id] = value;
            else
                DM._.DB.MiningDB.OreCardCnts[Id] = value;
        } 
    }
    public bool IsChecked;
    public Button Button;
    public TextMeshProUGUI CntTxt;
    public TextMeshProUGUI ValTxt;
    public Image Outline;
    public Image CheckMark;
    public GameObject Dim;

    public void InitData(MineCate cate, int id, string valStr) {
        Cate = cate;
        Id = id;
        ValTxt.text = valStr;
    }

    public void InitOutline() => Outline.color = Color.black;
    public void Select() => Outline.color = Color.red;
    public bool Selected() => Outline.color == Color.red;
    public void InitCheck() {
        IsChecked = false;
        CheckMark.gameObject.SetActive(false);
    }
    public void Check() {
        IsChecked = true;
        CheckMark.gameObject.SetActive(true);
    }
    public void Update() {
        Dim.SetActive(Cnt <= 0);
        CntTxt.text = $"{Cnt} / {Config.MINING_MERGE_CNT}";
        CntTxt.color = Cnt > Config.MINING_MERGE_CNT? Color.green : Color.white;
    }
}
