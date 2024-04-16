using System;
using System.Collections;
using System.Collections.Generic;
using AssetKits.ParticleImage;
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
    [field: SerializeField] public bool IsActive {get; set;}
    [SerializeField] int waitTurnNum; public int WaitTurnNum {
        get => waitTurnNum;
        set {
            waitTurnNum = value;
            WaitTurnTxt.text = DimImg.fillAmount < 1? "" : $"{value}턴";
            // DimImg.gameObject.SetActive(value > 0);
        }
    }
    [field: SerializeField] public Image DimImg {get; set;}
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
    [field: SerializeField] public ParticleImage StreamPack0AuraUIEF {get; set;}
    [field: SerializeField] public ParticleImage StreamPack1AuraUIEF {get; set;}

    [field: SerializeField] public bool IsBagActive {get; set;}

    void Start() {
        foreach(var item in ConsumableItemBtns) {
            item.QuantityTxt.text = "1";
            item.WaitTurnNum = 0;
            item.IsActive = false;
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
        var consumeItemBtn = ConsumableItemBtns[(int)itemEnumIdx];
        if(consumeItemBtn.IsActive) {
            GM._.gui.ShowMsgError($"현재 적용 중입니다!");
            return;
        }
        else if(consumeItemBtn.WaitTurnNum > 0) {
            GM._.gui.ShowMsgError($"재사용까지 {consumeItemBtn.WaitTurnNum}턴 남았습니다!");
            return;
        }
        StartCoroutine(CoSteamPackActive(itemEnumIdx));
    }

    IEnumerator CoSteamPackActive(Etc.ConsumableItem itemEnumIdx) {
        int itemIdx = (int)itemEnumIdx;
        ItemSO[] consumableItem = GM._.rwlm.RwdItemDt.EtcConsumableDatas;
        string key = consumableItem[itemIdx].name;
        SM._.SfxPlay(SM.SFX.CheerUpSFX);

        //* アクティブトリガー ON
        ConsumableItemBtns[itemIdx].IsActive = true;

        //* 能力 メッセージ
        GM._.gui.ShowMsgNotice(consumableItem[itemIdx].Description);
        ConsumableItemBtns[itemIdx].ParticleEF.Play();

        //* Buff 追加
        switch(itemEnumIdx) {
            case Etc.ConsumableItem.SteamPack0:
                StreamPack0AuraUIEF.Play();
                GM._.tm.AddAllTowerExtraDmg(key, Config.STEAMPACK0_EXTRA_DMG_PER);
                break;
            case Etc.ConsumableItem.SteamPack1:
                StreamPack1AuraUIEF.Play();
                GM._.tm.AddAllTowerExtraSpd(key, Config.STEAMPACK1_EXTRA_SPD_PER);
                break;
        }

        //* ステータスUI 最新化
        if(GM._.tmc.HitObject != null) {
            Tower tower = GM._.tmc.HitObject.GetComponentInChildren<Tower>();
            GM._.gui.tsm.ShowTowerStateUI(tower.InfoState());
        }

        //* 10秒間、BUFF適用 (FillAmount UI処理)
        float elapsedTime = 0;
        while(elapsedTime < Config.STREAMPACK_DURATION) {
            elapsedTime += Time.deltaTime; // 경과 시간 업데이트
            float fillAmount = elapsedTime / Config.STREAMPACK_DURATION; // 현재 채워진 양 계산
            Debug.Log("fillAmount= " + fillAmount);
            ConsumableItemBtns[itemIdx].DimImg.fillAmount = fillAmount; // Image의 fillAmount 업데이트
            yield return null; // 한 프레임을 기다림
        }

        //* アクティブトリガー OFF
        ConsumableItemBtns[itemIdx].IsActive = false;

        //* 待機時間を登録 （TxtUIもset処理で行うため、Buffが終わってからやる）
        ConsumableItemBtns[itemIdx].WaitTurnNum = Config.STEAMPACK_WAIT_TURN;

        //* Buff 解除
        switch(itemEnumIdx) {
            case Etc.ConsumableItem.SteamPack0:
                StreamPack0AuraUIEF.Stop();
                GM._.tm.RemoveAllTowerExtraDmg(key);
                break;
            case Etc.ConsumableItem.SteamPack1:
                StreamPack1AuraUIEF.Stop();
                GM._.tm.RemoveAllTowerExtraSpd(key);
                break;
        }
        GM._.tm.RemoveAllTowerExtraDmg(key);

        //* ステータスUI 最新化
        if(GM._.tmc.HitObject != null) {
            Tower tower = GM._.tmc.HitObject.GetComponentInChildren<Tower>();
            GM._.gui.tsm.ShowTowerStateUI(tower.InfoState());
        }
    }
#endregion
}
