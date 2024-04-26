using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;

public class ShopManager : MonoBehaviour {
    const int TAPBTN_PACKAGE = 0, TAPBTN_CHEST = 1, TAPBTN_RSC = 2;
    [SerializeField] Color TapBtnActiveClr;
    [SerializeField] Color TapBtnInActiveClr;
    [field:SerializeField] public bool[] IsActiveTapBtnCates {get; private set;}
    [field:SerializeField] public GameObject WindowObj {get; private set;}
    [field:SerializeField] public GameObject[] CateTitleLineObjs {get; private set;}
    [field:SerializeField] public GameObject[] CateGroupObjs {get; private set;}
    [field:SerializeField] public Button[] TapBtns {get; private set;}

    void Start() {
        InitUI();
    }

#region EVENT
    public void OnClickShopIconBtnAtHome() => ShowShopPanel(true);
    public void OnClickBackBtn() => ShowShopPanel(false);
    public void OnClickTapBtn(int btnIdx) {
        SM._.SfxPlay(SM.SFX.ClickSFX);
        //* 再クリック：全て表示して終了
        if(TapBtns[btnIdx].GetComponent<Image>().color == TapBtnActiveClr) {
            InitUI();
            return;
        }

        //* TapBtnの色 初期化
        Array.ForEach(TapBtns, btn => btn.GetComponent<Image>().color = TapBtnInActiveClr);

        //* 適用
        IsActiveTapBtnCates[btnIdx] = true;
        TapBtns[btnIdx].GetComponent<Image>().color = TapBtnActiveClr;
        ActiveCateGroup(btnIdx);
    }
    public void OnClickPackageBtn(int idx) {
        const int AllInOne = 0, LevelUpSupport = 1, RandomEquip = 2,
            RandomRelic = 3, EquipUpgradeSupport = 4, MiningSupport = 5;

        RewardItemSO rwDt = HM._.rwlm.RwdItemDt;

        //TODO IAP PURCHASE

        var rewardList = new List<RewardItem>();
        switch(idx) {
            case AllInOne:
                rewardList.Add(new (rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.ChestCommon], 10));
                rewardList.Add(new (rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.ChestDiamond], 3));
                rewardList.Add(new (rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.ChestGold], 5));
                rewardList.Add(new (rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.ChestEquipment], 16));
                rewardList.Add(new (rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.ChestPremium], 3));
                //TODO Remove Ads Item
                break;
            case LevelUpSupport:
                rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.GoldKey], 10));
                rewardList.Add(new (rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.Clover], 10));
                rewardList.Add(new (rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.Clover], 10));
                rewardList.Add(new (rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.SteamPack0], 7));
                rewardList.Add(new (rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.SteamPack1], 7));
                rewardList.Add(new (rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.BizzardScroll], 7));
                rewardList.Add(new (rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.LightningScroll], 7));
                break;
            case RandomEquip:
                List<int> gradePerList = rwDt.Rwd_ChestEquipment.RwdGradeTb.EquipPerList;

                //* 等級パーセントリスト 準備
                int COMMON = gradePerList[(int)Enum.Grade.Common];
                int RARE = COMMON + gradePerList[(int)Enum.Grade.Rare];
                int EPIC = RARE + gradePerList[(int)Enum.Grade.Epic];
                int UNIQUE = EPIC + gradePerList[(int)Enum.Grade.Unique];
                int LEGEND = UNIQUE + gradePerList[(int)Enum.Grade.Legend];
                int MYTH = LEGEND + gradePerList[(int)Enum.Grade.Myth];
                // int PRIME = MYTH + gradePerList[(int)Enum.Grade.Prime];

                //* ランダム等級 適用
                int cnt = 12;
                int lastIdx = cnt - 1;
                for(int i = 0; i < cnt; i++) {
                    int rdPer = Random.Range(0, 1000);
                    int grade = (rdPer < COMMON)? 0
                        : (rdPer < RARE)? 1
                        : (rdPer < EPIC)? 2
                        : (rdPer < UNIQUE)? 3
                        : (rdPer < LEGEND)? 4
                        : (rdPer < MYTH)? 5
                        : 6;
                    
                    int rdItemKind = Random.Range(0, 3);
                    var randItemDts = (rdItemKind == 0)? rwDt.WeaponDatas
                        : (rdItemKind == 1)? rwDt.ShoesDatas
                        : rwDt.RingDatas;
                    
                    //* リワード追加
                    rewardList.Add(new (randItemDts[i == lastIdx? (int)Enum.Grade.Myth : grade]));
                }
                break;
            case RandomRelic:
                break;
            case EquipUpgradeSupport:
                rewardList.Add(new (rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.LightningScroll], 15));
                rewardList.Add(new (rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.LightningScroll], 25));
                rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Coin], 50000));
                break;
            case MiningSupport:
                rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Goblin0], 100));
                rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Goblin1], 50));
                rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Goblin2], 25));
                rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Goblin3], 10));
                rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Goblin4], 5));
                rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Goblin5], 1));
                break;
        }

        HM._.rwlm.ShowReward(rewardList);
        HM._.rwm.UpdateInventory(rewardList);
    }

#endregion
#region FUNC
    private void ShowShopPanel(bool isShow) {
        SM._.SfxPlay(SM.SFX.ClickSFX);
        WindowObj.SetActive(isShow);
        HM._.hui.TopNavCanvas.sortingOrder = isShow? 101 : 99;
    }
    private void InitUI() {
        Array.ForEach(IsActiveTapBtnCates, isActive => isActive = false);
        Array.ForEach(TapBtns, btn => btn.GetComponent<Image>().color = TapBtnInActiveClr);
        Array.ForEach(CateTitleLineObjs, titleLine => titleLine.SetActive(true));
        Array.ForEach(CateGroupObjs, groupObj => groupObj.SetActive(true));
    }
    public void ActiveCateGroup(int btnIdx) {
        for(int i = 0; i < CateGroupObjs.Length; i++) {
            CateTitleLineObjs[i].SetActive(btnIdx == i);
            CateGroupObjs[i].SetActive(btnIdx == i);
        }

        //* TAPがリーソースであれば、DiamondとCoin両方を表示する
        if(btnIdx == TAPBTN_RSC) {
            const int COIN_GROUP = TAPBTN_RSC + 1;
            CateTitleLineObjs[COIN_GROUP].SetActive(true);
            CateGroupObjs[COIN_GROUP].SetActive(true);
        }
    }
#endregion
}
