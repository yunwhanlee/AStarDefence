using System.Collections;
using System.Collections.Generic;
// using Unity.VisualScripting;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

/// <summary>
/// ステージロック状態のデータ
/// </summary>
[Serializable]
public class StageLockedData {
    [field:SerializeField] public string Name {get; private set;}
    [field:SerializeField] public bool IsLockEasy {get; set;}
    [field:SerializeField] public bool IsLockNormal {get; set;}
    [field:SerializeField] public bool IsLockHard {get; set;}
    
    public StageLockedData(string name, bool isLockEasy) {
        Name = name;
        IsLockEasy = isLockEasy;
        IsLockNormal = false;
        IsLockHard = false;
    }
}

/// <summary>
/// 採掘システムの保存データ
/// </summary>
[Serializable]
public class MiningData {
    [field:SerializeField] public int[] GoblinCardCnts {get; set;} = new int[7];
    [field:SerializeField] public int[] OreCardCnts {get; set;} = new int[9];
    [field:SerializeField] public WorkSpace[] WorkSpaces {get; set;} = new WorkSpace[5];
}

/// <summary>
/// 保存・読込のデータベース ★データはPlayerPrefsに保存するので、String、Float、Intのみ！！！★
/// </summary>
[Serializable]
public class DB {
    [field:SerializeField] public StageLockedData[] StageLockedDatas {get; set;}
    [field:SerializeField] public MiningData MiningData {get; set;}
}

/// <summary>
/// データマネジャー
/// </summary>
public class DM : MonoBehaviour {
    public static DM _ {get; private set;}
    const string DB_KEY = "DB";
    const string PASSEDTIME_KEY = "PASSED_TIME";
    //* ★データベース
    [field: SerializeField] public bool IsReset {get; set;}
    [field: SerializeField] public DB DB {get; private set;}
    [field: SerializeField] public int PassedSec {get; set;}

    //* Global Values
    [field: SerializeField] public int SelectedStage {get; set;}
    [field: SerializeField] public Enum.Difficulty SelectedDiff {get; set;}

    void Awake() {
        //* SINGLETON
        if(_ == null) {
            _ = this;
            DontDestroyOnLoad(this);
        }
        else {
            Destroy(this);
            return;
        }

        //* アプリを初めてスタートした場合のみ、一回
        if(Load() == null) {
            DB = new DB();
            HM._.hui.ShowMsgNotice("AStar디펜스에 오신걸 환영합니다!");
            Reset();
        }
        else
            DB = Load();
    }

/// -----------------------------------------------------------------------------------------------------------------
#region QUIT APP EVENT
/// -----------------------------------------------------------------------------------------------------------------
#if UNITY_EDITOR
    void OnApplicationQuit() {
        Debug.Log("<color=yellow>QUIT APP(PC)::OnApplicationQuit():: SAVE</color>");
        Save();
    }
#elif UNITY_ANDROID
    void OnApplicationPause(bool paused){
        //* ゲームが開くとき（paused == true）にも起動されるので注意が必要。
        if(paused == true) {
            Debug.Log("<color=yellow>QUIT APP(Mobile)::OnApplicationPause( "+paused+" ):: Scene= " + SceneManager.GetActiveScene().name);
            Save();
        }
    }
#endif
#endregion
/// -----------------------------------------------------------------------------------------------------------------
#region SAVE
/// -----------------------------------------------------------------------------------------------------------------
    public void Save() {
        //* 経過時間 保存
        // 현재 시간을 UTC 기준으로 가져와서 1970년 1월 1일 0시 0분 0초와의 시간 차이를 구합니다.
        TimeSpan timestamp = DateTime.UtcNow - new DateTime(1970,1,1,0,0,0);
        // 시간 차이를 정수형으로 변환하여 PlayerPrefs에 저장합니다.
        PlayerPrefs.SetInt(PASSEDTIME_KEY, (int)timestamp.TotalSeconds);

        //* Serialize To Json
        PlayerPrefs.SetString(DB_KEY, JsonUtility.ToJson(DB, true)); 
        //* Print
        string json = PlayerPrefs.GetString(DB_KEY);
        Debug.Log($"★SAVE:: The Key: {DB_KEY} Exists? {PlayerPrefs.HasKey(DB_KEY)}, Data ={json}");
    }
#endregion
/// -----------------------------------------------------------------------------------------------------------------
#region LOAD
/// -----------------------------------------------------------------------------------------------------------------
    public DB Load() {
        //* 経過時間 ロード
        // 현재 시간을 UTC 기준으로 가져와서 1970년 1월 1일 0시 0분 0초와의 시간 차이를 구합니다.
        TimeSpan timestamp = DateTime.UtcNow - new DateTime(1970,1,1,0,0,0);
        // 앱을 최초로 시작했을 때와의 시간 차이를 계산하여 PassedSec에 저장합니다.
        int past = PlayerPrefs.GetInt(PASSEDTIME_KEY, defaultValue: (int)timestamp.TotalSeconds);
        PassedSec = (int)timestamp.TotalSeconds - past;

        //* (BUG)最初の実行だったら、ロードデータがないから、リセットして初期化。
        if(!PlayerPrefs.HasKey(DB_KEY)){
            return null;
        }
        //* Json 読み込み
        string json = PlayerPrefs.GetString(DB_KEY);
        Debug.Log($"★LOAD:: (json == null)? {json == null}, \ndata= {json}");
        //* Json クラス化
        DB db = JsonUtility.FromJson<DB>(json);
        return db;
    }
#endregion
/// -----------------------------------------------------------------------------------------------------------------
#region RESET
/// -----------------------------------------------------------------------------------------------------------------
    public void OnClickResetBtn() {
        Reset();
        SceneManager.LoadScene($"{Enum.Scene.Home}");
    }
    private void Reset() {
        Debug.Log($"★RESET:: The Key: {DB_KEY} Exists? {PlayerPrefs.HasKey(DB_KEY)}");
        PlayerPrefs.DeleteAll();
        Init();
    }
#endregion
/// -----------------------------------------------------------------------------------------------------------------
#region FUNC
/// -----------------------------------------------------------------------------------------------------------------
    public void Init() {
        DB = new DB();

        //* Stages
        DB.StageLockedDatas = new StageLockedData[5] {
            new ("스테이지1. 초원", false),
            new ("스테이지2. 황량한 사막", true),
            new ("스테이지3. 침묵의 바다", true),
            new ("스테이지4. 죽음의 던젼", true),
            new ("스테이지5. 불타는 지옥", true),
        };

        //* Mining
        DB.MiningData = new MiningData();
        DB.MiningData.GoblinCardCnts = new int[7] {
            5, 4, 3, 2, 1, 0, 0
        };

        DB.MiningData.OreCardCnts = new int[9] {
            5, 4, 3, 2, 4, 2, 0, 0, 0
        };

        DB.MiningData.WorkSpaces = new WorkSpace[5];
        for(int i = 0; i < DB.MiningData.WorkSpaces.Length; i++) {
            DB.MiningData.WorkSpaces[i] = new WorkSpace(i, i > 0, -1); // WorkSpace의 각 요소를 새로운 인스턴스로 만듭니다.
        }
    }
#endregion
}
