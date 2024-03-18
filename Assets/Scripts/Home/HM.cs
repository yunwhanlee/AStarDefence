using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class HM : MonoBehaviour {
    public static HM _; //* Global

    //* Value
    [field: SerializeField] public int SelectedStage {get; set;}

    //* Outside
    public HomeUIManager hui;
    public StageUIManager stgm;

    void Awake() {
        //* Global化 値 代入
        _ = this;

        //* 外部のスクリプト 初期化
        hui = GameObject.Find("HomeUIManager").GetComponent<HomeUIManager>();
        stgm = GameObject.Find("StageUIManager").GetComponent<StageUIManager>();

        //* 初期化
        SelectedStage = 0;
    }
}
