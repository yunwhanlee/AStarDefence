using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEditor.PackageManager;

public class HM : MonoBehaviour {
    public static HM _; //* Global

    //* Value
    [field: SerializeField] public int SelectedStage {get; set;}

    //* Outside
    public HomeUIManager hui;
    public StageUIManager stgm;

    // Mining
    public MiningUIManager mnm;
    public MiningTimeUIManager mtm;
    public WorkSpaceUIManager wsm;

    void Awake() {
        //* Global化 値 代入
        _ = this;

        //* 外部のスクリプト 初期化
        hui = GameObject.Find("HomeUIManager").GetComponent<HomeUIManager>();
        stgm = GameObject.Find("StageUIManager").GetComponent<StageUIManager>();
        mnm = GameObject.Find("MiningUIManager").GetComponent<MiningUIManager>();
        mtm = GameObject.Find("MiningTimeUIManager").GetComponent<MiningTimeUIManager>();
        wsm = GameObject.Find("WorkSpaceUIManager").GetComponent<WorkSpaceUIManager>();

        //* 初期化
        SelectedStage = 0;
    }
}
