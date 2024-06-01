using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class TowerManager : MonoBehaviour {
    public readonly static int RANDOM_TOWER_LV_MAX = 6; // ランダムタワーMAXレベル
    public readonly static int CARD_UPG_LV_MAX = 20; // カードアップグレード MAXレベル
    public readonly static float WARRIOR_CARD_DMG_UP_PER = 0.16f; // 戦士 カードアップグレード ダメージ単位
    public readonly static float ARCHER_CARD_DMG_UP_PER = 0.1f; // アーチャー カードアップグレード ダメージ単位
    public readonly static float MAGICIAN_CARD_DMG_UP_PER = 0.13f; // マジシャン カードアップグレード ダメージ単位
    public readonly static int CARD_UPG_PRICE_START = 5; // カードアップグレード 開始値段
    public readonly static int CARD_UPG_PRICE_UNIT = 5; // カードアップグレード単位

    public readonly static int CC_TOWER_INC_LIMIT_MAX = 5; // カードアップグレード単位

    [field: SerializeField] public int[] TowerCardUgrLvs {get; set;}
    [Header("WARRIOR")]
    [SerializeField] GameObject[] warriors; public GameObject[] Warriors {get => warriors;}
    [field: SerializeField] public Transform WarriorGroup {get; private set;}
    [Header("ARCHER")]
    [SerializeField] GameObject[] archers; public GameObject[] Archers {get => archers;}
    [field: SerializeField] public Transform ArcherGroup {get; private set;}
    [Header("MAGICIAN")]
    [SerializeField] GameObject[] magicians; public GameObject[] Magicians {get => magicians;}
    [field: SerializeField] public Transform MagicianGroup {get; private set;}

    [Header("CC")]
    [SerializeField] GameObject[] iceTowers; public GameObject[] IceTowers {get => iceTowers;}
    [SerializeField] GameObject[] stunTowers; public GameObject[] StunTowers {get => stunTowers;}
    [field: SerializeField] public Transform CCTowerGroup {get; private set;}
    [SerializeField] int ccTowerMax; public int CCTowerMax {
        get => ccTowerMax;
        set {
            ccTowerMax = value;
            GM._.actBar.CCTowerCntTxt.text = $"CC : {ccTowerMax}";
            GM._.bossRwd.SetCurValueTxt(Enum.BossRwd.IncreaseCCTowerCnt, ccTowerMax);
        }
    }
    [field: SerializeField] public int CCTowerCnt {get; set;}
    [Header("BOARD")]
    [SerializeField] GameObject[] boards; public GameObject[] Boards {get => boards;}
    [field: SerializeField] public Transform BoardGroup {get; private set;}

    TowerManager tm;
    TileMapController tmc;

    void Start() {
        tm = GM._.tm;
        tmc = GM._.tmc;
        TowerCardUgrLvs = new int[3] {0, 0, 0};
        CCTowerMax = 2;
        CCTowerCnt = 0;
    }
#region FUNC
    public List<Tower> GetAllTower() {
        List<Tower> allTowerList = new List<Tower>();
        for(int i = 0 ; i < WarriorGroup.childCount; i++)
            allTowerList.Add(WarriorGroup.GetChild(i).GetComponentInChildren<Tower>());
        for(int i = 0 ; i < ArcherGroup.childCount; i++)
            allTowerList.Add(ArcherGroup.GetChild(i).GetComponentInChildren<Tower>());
        for(int i = 0 ; i < MagicianGroup.childCount; i++)
            allTowerList.Add(MagicianGroup.GetChild(i).GetComponentInChildren<Tower>());

        return allTowerList;
    }
    #region ALL TOWER : EXTRA DMG BUFF
    /// <summary>
    /// 全てのタワーへ追加ダメージBUFF 追加
    /// </summary>
    /// <param name="dicKey">追加するBUFFのキー</param>
    /// <param name="extraDmgPer">追加ダメージ％</param>
    public void AddAllTowerExtraDmg(string dicKey, float extraDmgPer) {
        //* 全てタワーを探す
        List<Tower> allTowerList = GM._.tm.GetAllTower();

        //* 全てタワー
        allTowerList.ForEach(tower => {
            //* スタイル変更
            // Util._.SetRedMt(tower.BodySprRdr);
            //* 追加タメージ
            int extraDmg = (int)(tower.TowerData.Dmg * extraDmgPer);
            if(tower.ExtraDmgDic.ContainsKey(dicKey))
                tower.ExtraDmgDic.Remove(dicKey);
            tower.ExtraDmgDic.Add(dicKey, extraDmg);
        });
    }
    /// <summary>
    /// 全てのタワーへ追加ダメージBUFF 解除
    /// </summary>
    /// <param name="key">解除するBUFFのキー</param>
    public void RemoveAllTowerExtraDmg(string key) {
        //* 全てタワー
        List<Tower> allTowerList = GM._.tm.GetAllTower();

        //* 全てタワー
        allTowerList.ForEach(tower => {
            //* スタイル戻す
            // Util._.SetDefMt(tower.BodySprRdr);
            //* ダメージと速度戻す
            tower.ExtraDmgDic.Remove(key);
        });
    }
    #endregion
    #region ALL TOWER : EXTRA SPD BUFF
    /// <summary>
    /// 全てのタワーへ追加スピードBUFF 追加
    /// </summary>
    /// <param name="dicKey">追加するBUFFのキー</param>
    /// <param name="extraDmgPer">追加ダメージ％</param>
    public void AddAllTowerExtraSpd(string dicKey, float extraSpdPer) {
        //* 全てタワーを探す
        List<Tower> allTowerList = GM._.tm.GetAllTower();

        //* 全てタワー
        allTowerList.ForEach(tower => {
            //* スタイル変更
            // Util._.SetRedMt(tower.BodySprRdr);
            //* 追加タメージ
            float extraSpd = (float)(tower.TowerData.AtkSpeed * extraSpdPer);
            if(tower.ExtraSpdDic.ContainsKey(dicKey))
                tower.ExtraSpdDic.Remove(dicKey);
            tower.ExtraSpdDic.Add(dicKey, extraSpd);
        });
    }
    /// <summary>
    /// 全てのタワーへ追加スピードBUFF 解除
    /// </summary>
    /// <param name="key">解除するBUFFのキー</param>
    public void RemoveAllTowerExtraSpd(string key) {
        //* 全てタワー
        List<Tower> allTowerList = GM._.tm.GetAllTower();

        //* 全てタワー
        allTowerList.ForEach(tower => {
            //* スタイル戻す
            // Util._.SetDefMt(tower.BodySprRdr);
            //* ダメージと速度戻す
            tower.ExtraSpdDic.Remove(key);
        });
    }
    #endregion
#endregion
#region CREATE
    public void InstallBoard() {
        Debug.Log("InstallBoard()::");
        GameObject board = Instantiate(boards[Random.Range(0, boards.Length)], tm.BoardGroup);
        board.transform.localPosition = tmc.getCurSelectedPos();
        int boardCnt = tm.WarriorGroup.childCount + tm.ArcherGroup.childCount + tm.MagicianGroup.childCount;
        board.name = $"Board{boardCnt}";
        tmc.HitObject = board;
    }
    /// <summary>
    /// ランダムタワー生成
    /// </summary>
    private void InstantiateTower(GameObject towerObj, Transform objGroup) {
        //* タワー → Board子に入れる
        Tower tower = Instantiate(towerObj, tmc.HitObject.transform).GetComponent<Tower>();
        tower.transform.localPosition = new Vector2(0, 0.15f); //* 少し上で、Board上にのせるように

        //* マージ エフェクト
        GameEF idx = (tower.Kind == TowerKind.Warrior)? GameEF.StarlineExplosionRedEF
            : (tower.Kind == TowerKind.Archer)? GameEF.StarlineExplosionBlueEF
            : (tower.Kind == TowerKind.Magician)? GameEF.StarlineExplosionYellowEF : GameEF.NULL;
        Vector2 pos = new Vector2(tower.transform.position.x, tower.transform.position.y + 0.35f);
        if(tower.Lv > 1)
            GM._.gef.ShowEF(idx, pos);

        //* カードアッグレードのデータ反映
        tower.Upgrade();

        //* そのBoard → Group子に入れる
        tmc.HitObject.transform.SetParent(objGroup);
    }
    private void InstallIceTower(int lvIdx) {
        Debug.Log("InstallIceTower()::");
        GameObject ccTower = Instantiate(iceTowers[lvIdx], CCTowerGroup);
        ccTower.transform.position = tmc.getCurSelectedPos();
        tmc.HitObject = ccTower;
    }
    private void InstallStunTower(int lvIdx) {
        Debug.Log("InstallStunTower()::");
        GameObject ccTower = Instantiate(stunTowers[lvIdx], CCTowerGroup);
        ccTower.transform.position = tmc.getCurSelectedPos();
        tmc.HitObject = ccTower;
    }

    public void CreateTower(TowerType type, int lvIdx = 0, TowerKind kind = TowerKind.None) {
        Debug.Log($"<color=white>CreateTower({type}, {lvIdx})::</color>");
        switch(type) {
            case TowerType.Random:
                //* 種類の選択 (種類がパラメータで設定されたら、そのタイプに固定して生成)
                int randKind = (kind != TowerKind.None)? (int)kind : Random.Range(0, 3);
                Debug.Log($"CreateTower():: Random Tower:: tmc.TutoSeqIdx={tmc.TutoSeqIdx} randKind= {randKind}");
                switch(randKind) {
                    case (int)TowerKind.Warrior: 
                        InstantiateTower(warriors[lvIdx], WarriorGroup);
                        break;
                    case (int)TowerKind.Archer: 
                        InstantiateTower(archers[lvIdx], ArcherGroup);
                        break;
                    case (int)TowerKind.Magician: 
                        InstantiateTower(magicians[lvIdx], MagicianGroup);
                        break;
                }
                //* タワー設置 トリガー ON
                tmc.HitObject.GetComponent<Board>().IsTowerOn = true;
                tmc.HitObject.GetComponentInChildren<Tower>().trc.SprRdr.enabled = true;
                break;
            case TowerType.CC_IceTower:
                InstallIceTower(lvIdx);
                tmc.HitObject.GetComponent<Tower>().trc.SprRdr.enabled = true;
                break;
            case TowerType.CC_StunTower:
                InstallStunTower(lvIdx);
                tmc.HitObject.GetComponent<Tower>().trc.SprRdr.enabled = true;
                break;
        }
    }
#endregion
#region TOWER RANGE 表示
    public void ClearAllTowerRanges() {
        foreach(Transform child in WarriorGroup) {
            Board board = child.GetComponent<Board>();
            if(board.IsTowerOn)
                child.GetComponentInChildren<WarriorTower>().trc.SprRdr.enabled = false;
        }
        foreach(Transform child in ArcherGroup) {
            Board board = child.GetComponent<Board>();
            if(board.IsTowerOn)
                board.GetComponentInChildren<ArcherTower>().trc.SprRdr.enabled = false;
        }
        foreach(Transform child in MagicianGroup) {
            Board board = child.GetComponent<Board>();
            if(board.IsTowerOn)
                board.GetComponentInChildren<MagicianTower>().trc.SprRdr.enabled = false;
        }
        foreach(Transform child in CCTowerGroup) {
            Tower tower = child.GetComponent<Tower>();
            switch(tower.Type) {
                case TowerType.CC_IceTower:
                    var icetower = tower as IceTower;
                    icetower.trc.SprRdr.enabled = false;
                    break;
                case TowerType.CC_StunTower:
                    var stunTower = tower as StunTower;
                    stunTower.trc.SprRdr.enabled = false;
                    break;
            }            
        }
    }
#endregion
#region UPGRADE CARD
    public void UpgradeTowerCard(TowerKind kind) {
        Debug.Log($"UpgradeTowerCard({kind}):: UPGRADE!");
        switch(kind) {
            case TowerKind.Warrior:
                TowerCardUgrLvs[(int)TowerKind.Warrior]++;
                for(int i = 0; i < WarriorGroup.childCount; i++) {
                    var warrior = WarriorGroup.GetChild(i).GetComponentInChildren<WarriorTower>();
                    GM._.gef.ShowEF(GameEF.UpgradeCylinderRedEF, warrior.transform.position);
                    warrior.Upgrade();
                }

                break;
            case TowerKind.Archer:
                GM._.tm.TowerCardUgrLvs[(int)TowerKind.Archer]++;
                for(int i = 0; i < ArcherGroup.childCount; i++) {
                    var archer = ArcherGroup.GetChild(i).GetComponentInChildren<ArcherTower>();
                    GM._.gef.ShowEF(GameEF.UpgradeCylinderBlueEF, archer.transform.position);
                    archer.Upgrade();
                }
                break;
            case TowerKind.Magician:
                GM._.tm.TowerCardUgrLvs[(int)TowerKind.Magician]++;
                for(int i = 0; i < MagicianGroup.childCount; i++) {
                    var magician = MagicianGroup.GetChild(i).GetComponentInChildren<MagicianTower>();
                    GM._.gef.ShowEF(GameEF.UpgradeCylinderYellowEF, magician.transform.position);
                    magician.Upgrade();
                }
                break;
        }
        GM._.gui.UpdateTowerCardLvUI();

        //*お知らせメッセージ表示
        string kindName = (kind == TowerKind.Warrior)? "전사" : (kind == TowerKind.Archer)? "궁수" : "마법사";
        string cardLv = GM._.tm.TowerCardUgrLvs[(int)kind].ToString();
        GM._.gui.ShowMsgNotice($"{kindName}타워 업그레이드 성공! 레벨{cardLv}");
    }
#endregion
}
