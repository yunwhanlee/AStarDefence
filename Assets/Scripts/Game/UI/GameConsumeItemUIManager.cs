using System;
using System.Collections;
using System.Collections.Generic;
using Inventory.Model;
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
            Dim.SetActive(value > 0);
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
            item.WaitTurnNum = 0;
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
                SteamPackActive(Etc.ConsumableItem.SteamPack0);
                break;
            case (int)Etc.ConsumableItem.SteamPack1:
                SteamPackActive(Etc.ConsumableItem.SteamPack1);
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

    private void SteamPackActive(Etc.ConsumableItem itemEnumIdx) {
        if(ConsumableItemBtns[(int)itemEnumIdx].WaitTurnNum > 0) {
            GM._.gui.ShowMsgError($"재사용하려면 {ConsumableItemBtns[0].WaitTurnNum}이 남았습니다!");
            return;
        }
        StartCoroutine(CoSteamPackActive(itemEnumIdx));
    }

    IEnumerator CoSteamPackActive(Etc.ConsumableItem itemEnumIdx) {
        int itemIdx = (int)itemEnumIdx;
        ItemSO[] consumableItem = GM._.rwlm.RwdItemDt.EtcConsumableDatas;
        string key = consumableItem[itemIdx].name;

        SM._.SfxPlay(SM.SFX.CheerUpSFX);

        //* 能力 メッセージ
        GM._.gui.ShowMsgNotice(consumableItem[itemIdx].Description);
        ConsumableItemBtns[itemIdx].ParticleEF.Play();
        ConsumableItemBtns[itemIdx].WaitTurnNum = Config.STEAMPACK_WAIT_TURN;

        //* Buff 追加
        switch(itemEnumIdx) {
            case Etc.ConsumableItem.SteamPack0:
                GM._.tm.AddAllTowerExtraDmg(key, Config.STEAMPACK0_EXTRA_DMG_PER);
                break;
            case Etc.ConsumableItem.SteamPack1:
                GM._.tm.AddAllTowerExtraSpd(key, Config.STEAMPACK1_EXTRA_SPD_PER);
                break;
        }

        //* ステータスUI 最新化
        if(GM._.tmc.HitObject != null) {
            Tower tower = GM._.tmc.HitObject.GetComponentInChildren<Tower>();
            GM._.gui.tsm.ShowTowerStateUI(tower.InfoState());
        }

        yield return Util.Time10; //* 10秒待機

        //* Buff 解除
        switch(itemEnumIdx) {
            case Etc.ConsumableItem.SteamPack0:
                GM._.tm.RemoveAllTowerExtraDmg(key);
                break;
            case Etc.ConsumableItem.SteamPack1:
                GM._.tm.RemoveAllTowerExtraSpd(key);
                break;
        }
        GM._.tm.RemoveAllTowerExtraDmg(key);
    }
#endregion
}
