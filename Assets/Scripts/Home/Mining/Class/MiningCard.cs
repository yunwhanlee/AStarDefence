using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

[Serializable]
public class MiningCard {
    public string Name;
    public int Cnt;
    public bool IsChecked;
    public Button Button;
    public TextMeshProUGUI CntTxt;
    public TextMeshProUGUI ValTxt;
    public Image Outline;
    public Image CheckMark;
    public GameObject Dim;

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
        CntTxt.text = $"보유: {Cnt} / 5";
        CntTxt.color = Cnt > 5? Color.green : Color.white;
    }
}
