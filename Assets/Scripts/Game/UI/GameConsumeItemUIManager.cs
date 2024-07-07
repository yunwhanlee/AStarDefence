using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AssetKits.ParticleImage;
using DG.Tweening;
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
            if(WaitTurnNum < 0) {
                WaitTurnNum = 0;
                WaitTurnTxt.text = "";
            } 
            WaitTurnTxt.text = (WaitTurnNum == 0)? "" : $"{value}턴";
            DimImg.fillAmount = (WaitTurnNum == 0)? 0 : 1;
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
    [field: SerializeField] public TMP_Text BagActiveTxt {get; set;}
    [field: SerializeField] public GameObject ConsumeItemBtnGroup {get; set;}
    [field: SerializeField] public GameObject G_TutoConsumeBagMsgBubble {get; set;}
    private DOTweenAnimation ConsumeItemBtnGroupDOTAnim {get; set;}
    [field: SerializeField] public ConsumableItemBtn[] ConsumableItemBtns {get; set;}
    [field: SerializeField] public ParticleImage StreamPack0AuraUIEF {get; set;}
    [field: SerializeField] public ParticleImage StreamPack1AuraUIEF {get; set;}

    [field: SerializeField] public bool IsBagActive {get; set;}

    void Start() {
        G_TutoConsumeBagMsgBubble.SetActive(DM._.DB.TutorialDB.IsActiveConsumeBag);

        ConsumeItemBtnGroupDOTAnim = ConsumeItemBtnGroup.GetComponent<DOTweenAnimation>();

        //* 初期化
        foreach(var item in ConsumableItemBtns) {
            item.WaitTurnNum = 0;
            item.IsActive = false;
            item.QuantityTxt.text = "0";
        }

        //* Bag
        IsBagActive = false;
        ActiveBagUI();

        //* インベントリーからの消費アイテムの数 表示
        UpdateBtnQuantityTxt();
    }

#region EVENT
    public void OnClickToogleBagIconBtn() {
        if(!IsBagActive)
            SM._.SfxPlay(SM.SFX.StageSelectSFX);
        else
            SM._.SfxPlay(SM.SFX.ClickSFX);

        if(DM._.DB.TutorialDB.IsActiveConsumeBag) {
            DM._.DB.TutorialDB.IsActiveConsumeBag = false;
            G_TutoConsumeBagMsgBubble.SetActive(false);
        }

        IsBagActive = !IsBagActive;
        ActiveBagUI();
    }
    /// <summary>
    /// 消費アイテムをクリックして使うイベント
    /// </summary>
    /// <param name="idx">0: Steampack0, 1: Steampack1, 2: BlizzardScroll, 3: LighteningScroll</param>
    public void OnClickConsumeItemBtn(int idx) {
        Etc.ConsumableItem itemEnumIdx = Etc.GetConsumableItem(idx);
        //* アイテム利用が出来るのかチェック
        if(CheckAvailable(itemEnumIdx) == false) return;

        //* インベントリデータから、残る量を減る
        int invItemIdx = Array.FindIndex(GM._.InventoryData.InvArr, itemDt
            => !itemDt.IsEmpty && itemDt.Data.name == itemEnumIdx.ToString());
        var invItemDt = GM._.InventoryData.InvArr[invItemIdx];        
        GM._.InventoryData.InvArr[invItemIdx] = invItemDt.ChangeQuantity(invItemDt.Quantity - 1);

        //* 能力 反映
        switch(itemEnumIdx) {
            case Etc.ConsumableItem.SteamPack0:
                SteamPackActive(itemEnumIdx);
                break;
            case Etc.ConsumableItem.SteamPack1:
                SteamPackActive(itemEnumIdx);
                break;
            case Etc.ConsumableItem.BizzardScroll:
                ScrollActive(itemEnumIdx);
                break;
            case Etc.ConsumableItem.LightningScroll:
                ScrollActive(itemEnumIdx);
                break;
        }
    }
#endregion

#region FUNC
    private void ActiveBagUI() {
        BagIconImg.sprite = BagIconSprs[IsBagActive? BAG_ACTIVE : BAG_INACTIVE];
        BagActiveTxt.text = IsBagActive? "ON" : "OFF";
        ConsumeItemBtnGroup.SetActive(IsBagActive);
        if(IsBagActive) {
            ConsumeItemBtnGroupDOTAnim.DORestart();
        }
    }

    private void UpdateBtnQuantityTxt() {
        foreach(var itemDt in GM._.InventoryData.InvArr) {
            if(itemDt.IsEmpty)
                continue;
            if(itemDt.Data.name == $"{Etc.ConsumableItem.SteamPack0}")
                ConsumableItemBtns[0].QuantityTxt.text = $"{itemDt.Quantity}";
            if(itemDt.Data.name == $"{Etc.ConsumableItem.SteamPack1}")
                ConsumableItemBtns[1].QuantityTxt.text = $"{itemDt.Quantity}";
            if(itemDt.Data.name == $"{Etc.ConsumableItem.BizzardScroll}")
                ConsumableItemBtns[2].QuantityTxt.text = $"{itemDt.Quantity}";
            if(itemDt.Data.name == $"{Etc.ConsumableItem.LightningScroll}")
                ConsumableItemBtns[3].QuantityTxt.text = $"{itemDt.Quantity}";
        }
    }

    private bool CheckAvailable(Etc.ConsumableItem itemEnumIdx) {
        ConsumableItemBtn iconBtn = ConsumableItemBtns[(int)itemEnumIdx];
        InventoryItem findInvItem = Array.Find(GM._.InventoryData.InvArr, itemDt
            => !itemDt.IsEmpty && itemDt.Data.name == itemEnumIdx.ToString());

        if(findInvItem.Quantity <= 0) {
            GM._.gui.ShowMsgError("사용할 아이템이 없습니다.");
            return false;
        }
        else if(iconBtn.IsActive) {
            GM._.gui.ShowMsgError("현재 적용 중입니다!");
            return false;
        }
        else if(iconBtn.WaitTurnNum > 0) {
            GM._.gui.ShowMsgError($"재사용까지 {iconBtn.WaitTurnNum}턴 남았습니다!");
            return false;
        }
        return true;
    }

    /// <summary>
    /// 消費アイテムの待機ターンがなるものを確認し、ターンを減る
    /// </summary>
    public void DecreaseConsumeItemWaitTurn() {
        Debug.Log("DecreaseConsumeItemWaitTurn()::");
        for(int i = 0; i < ConsumableItemBtns.Length; i++) {
            --ConsumableItemBtns[i].WaitTurnNum;
        }
        UpdateBtnQuantityTxt();
    }

    private void ScrollActive(Etc.ConsumableItem itemEnumIdx) {
        int itemIdx = (int)itemEnumIdx;
        ItemSO[] consumableItem = GM._.rwlm.RwdItemDt.EtcConsumableDatas;
        
        Transform enemyGroup = GM._.em.enemyObjGroup;

        //* Set 待機時間
        ConsumableItemBtns[itemIdx].DimImg.fillAmount = 1;
        ConsumableItemBtns[itemIdx].WaitTurnNum = Config.CONSUMEITEM_WAIT_TURN;

        //* 能力 メッセージ
        GM._.gui.ShowMsgNotice(consumableItem[itemIdx].Description);
        ConsumableItemBtns[itemIdx].ParticleEF.Play();

        switch(itemEnumIdx) {
            case Etc.ConsumableItem.BizzardScroll:
                SM._.SfxPlay(SM.SFX.FrostNovaSFX);
                GM._.gef.ActiveObjEF(GameEF_ActiveObj.BlizzardScrollNovaEF);
                //* 全ての敵を凍らせる
                for(int i = 0; i < enemyGroup.childCount; i++) {
                    Enemy enemy = enemyGroup.GetChild(i).GetComponent<Enemy>();
                    if(enemy != null && enemy.gameObject.activeSelf)
                        enemy.Slow(sec: Config.BLIZZARDSCROLL_SLOW_SEC);
                }
                break;
            case Etc.ConsumableItem.LightningScroll:
                SM._.SfxPlay(SM.SFX.LightningNovaSFX);
                GM._.gef.ActiveObjEF(GameEF_ActiveObj.LightningScrollNovaEF);
                //* 全ての敵を凍らせる
                for(int i = 0; i < enemyGroup.childCount; i++) {
                    Enemy enemy = enemyGroup.GetChild(i).GetComponent<Enemy>();
                    if(enemy != null && enemy.gameObject.activeSelf)
                        enemy.Stun(sec: Config.LIGHTNINGSCROLL_STUN_SEC);
                }
                break;
        }
    }

    #region STEAMPACK 
    private void SteamPackActive(Etc.ConsumableItem itemEnumIdx)
        => StartCoroutine(CoSteamPackActive(itemEnumIdx));

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

        //* Set 待機時間 （TxtUIもset処理で行うため、Buffが終わってからやる）
        ConsumableItemBtns[itemIdx].WaitTurnNum = Config.CONSUMEITEM_WAIT_TURN;

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
#endregion
}
