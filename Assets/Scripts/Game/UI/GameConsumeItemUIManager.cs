using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ゲームシーンで、消費できるアイテムの属性とUI要素
/// </summary>
[Serializable]
public class ConsumableItemBtn {
    [field: SerializeField] public string Name {get; set;}
    [field: SerializeField] public int Id {get; set;}
    [SerializeField] int waitTurnNum; public int WaitTurnNum {
        get => waitTurnNum;
        set {
            waitTurnNum = value;
            WaitTurnTxt.text = $"{value}턴";
        }
    }
    [field: SerializeField] public GameObject Dim {get; set;}
    [field: SerializeField] public ParticleSystem ParticleEF {get; set;}
    [field: SerializeField] public TMP_Text QuantityTxt {get; set;}
    [field: SerializeField] public TMP_Text WaitTurnTxt {get; set;}
}

/// <summary>
/// ゲームシーンで、消費アイテムの管理マネジャー
/// </summary>
public class GameConsumeItemUIManager : MonoBehaviour {
    const int BAG_INACTIVE = 0, BAG_ACTIVE = 1;
    [field: Header("RESOURCE")]
    [field: SerializeField] public Sprite[] BagIconSprs {get; private set;}

    [field: Header("UI Elements")]
    [field: SerializeField] public Image BagIconImg {get; set;}
    [field: SerializeField] public GameObject ConsumeItemBtnGroup {get; set;}
    [field: SerializeField] public ConsumableItemBtn[] ConsumableItemBtns {get; set;}

    [field: SerializeField] public bool IsBagActive {get; set;}

    void Start() {
        foreach(var item in ConsumableItemBtns) {
            item.QuantityTxt.text = "1";
            item.Dim.SetActive(false);
        }
    }

#region EVENT
    public void OnClickToogleBagIconBtn() {
        IsBagActive = !IsBagActive;
        BagIconImg.sprite = BagIconSprs[IsBagActive? BAG_ACTIVE : BAG_INACTIVE];
        ConsumeItemBtnGroup.SetActive(IsBagActive);
    }
    /// <summary>
    /// 消費アイテムをクリックして使うイベント
    /// </summary>
    /// <param name="idx">0: Steampack0, 1: Steampack1, 2: BlizzardScroll, 3: LighteningScroll</param>
    public void OnClickConsumeItemBtn(int idx) {
        switch(idx) {
            case (int)Etc.ConsumableItem.SteamPack0:
                SteamPack0Active();
                break;
            case (int)Etc.ConsumableItem.SteamPack1:
                break;
            case (int)Etc.ConsumableItem.BizzardScroll:
                break;
            case (int)Etc.ConsumableItem.LightningScroll:
                break;
        }
    }
#endregion

#region FUNC
    /// <summary>
    /// 消費アイテムの待機ターンがなるものを確認し、ターンを減る
    /// </summary>
    public void CheckConsumeItemWaitTurn() {
        Debug.Log("CheckConsumeItemWaitTurn()::");
        for(int i = 0; i < ConsumableItemBtns.Length; i++) {
            var cItem = ConsumableItemBtns[i];
            if(cItem.WaitTurnNum > 0)
                --ConsumableItemBtns[i].WaitTurnNum;
        }
    }

    public void SteamPack0Active() {
        if(ConsumableItemBtns[0].WaitTurnNum > 0) {
            GM._.gui.ShowMsgError($"재사용하려면 {ConsumableItemBtns[0].WaitTurnNum}이 남았습니다!");
            return;
        }
        StartCoroutine(CoSteamPack0Active());
    } 
    IEnumerator CoSteamPack0Active() {
        const string STEAMPACK0 = "STEAMPACK0";


        SM._.SfxPlay(SM.SFX.CheerUpSFX);
        // CheerUpEF.SetActive(true);
        float dmgUpPer = 1.3f;
        ConsumableItemBtns[0].ParticleEF.Play();
        ConsumableItemBtns[0].WaitTurnNum = 2;
        ConsumableItemBtns[0].Dim.SetActive(true);

        //* 全てタワーを探す
        List<Tower> allTowerList = new List<Tower>();
        for(int i = 0 ; i < GM._.tm.WarriorGroup.childCount; i++)
            allTowerList.Add(GM._.tm.WarriorGroup.GetChild(i).GetComponentInChildren<Tower>());
        for(int i = 0 ; i < GM._.tm.ArcherGroup.childCount; i++)
            allTowerList.Add(GM._.tm.ArcherGroup.GetChild(i).GetComponentInChildren<Tower>());
        for(int i = 0 ; i < GM._.tm.MagicianGroup.childCount; i++)
            allTowerList.Add(GM._.tm.MagicianGroup.GetChild(i).GetComponentInChildren<Tower>());

        //* 全てタワー
        allTowerList.ForEach(tower => {
            //* スタイル変更
            Util._.SetRedMt(tower.BodySprRdr);

            //* 追加タメージ
            int extraDmg = (int)(tower.TowerData.Dmg * dmgUpPer);
            if(tower.ExtraDmgDic.ContainsKey(STEAMPACK0))
                tower.ExtraDmgDic.Remove(STEAMPACK0);
            tower.ExtraDmgDic.Add(STEAMPACK0, extraDmg);
        });

        if(GM._.tmc.HitObject != null) {
            Tower tower = GM._.tmc.HitObject.GetComponentInChildren<Tower>();
            GM._.gui.tsm.ShowTowerStateUI(tower.InfoState());
        }

        yield return Util.Time5;
        // CheerUpEF.SetActive(false);

        //* 全てタワー
        allTowerList.ForEach(tower => {
            //* スタイル戻す
            Util._.SetDefMt(tower.BodySprRdr);
            //* ダメージと速度戻す
            tower.ExtraDmgDic.Remove(STEAMPACK0);
            tower.ExtraSpdDic.Remove(STEAMPACK0);
        });

        // if(GM._.tmc.HitObject != null)
            // GM._.gui.tsm.ShowTowerStateUI(InfoState());

        // yield return new WaitForSeconds(10);
        // IsCheerUpActive = true;
    }
#endregion
}
