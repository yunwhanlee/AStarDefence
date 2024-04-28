using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;
using TMPro;
using Inventory.Model;

public class ShopManager : MonoBehaviour {
    const int TAPBTN_PACKAGE = 0, TAPBTN_CHEST = 1, TAPBTN_RSC = 2;

    [field:SerializeField] public int EquipPackageCnt {get; set;}
    public Action OnClickEquipPackage;

    [SerializeField] Color TapBtnActiveClr;
    [SerializeField] Color TapBtnInActiveClr;
    [field:SerializeField] public bool[] IsActiveTapBtnCates {get; private set;}
    [field:SerializeField] public GameObject WindowObj {get; private set;}
    [field:SerializeField] public GameObject[] CateTitleLineObjs {get; private set;}
    [field:SerializeField] public GameObject[] CateGroupObjs {get; private set;}
    [field:SerializeField] public GameObject[] PackageDimObjs {get; private set;}

    //* 一日制限がある アイテム
    [field:SerializeField] public GameObject FreeCommonChestDim {get; private set;}
    [field:SerializeField] public GameObject DiamondChestDim {get; private set;}
    [field:SerializeField] public GameObject FreeTinyDiamondDim {get; private set;}

    [field:SerializeField] public Button[] TapBtns {get; private set;}

    [field:SerializeField] public TMP_Text[] ChestPriceTxts {get; set;}

    void Start() {
        InitUI();

        //* Daily アイテム結果時間 チェック
        var shopDB = DM._.DB.ShopDB;
        FreeCommonChestDim.SetActive(shopDB.TogglePassedDay(ShopDB.FREE_COMMON));
        DiamondChestDim.SetActive(shopDB.TogglePassedDay(ShopDB.DIAMOND_CHEST));
        FreeTinyDiamondDim.SetActive(shopDB.TogglePassedDay(ShopDB.FREE_TINY));
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
    /// <summary>
    /// SHOPでパッケージクリック
    /// </summary>
    /// 
    /// </summary>
    /// <param name="idx"></param>
    public void OnClickPackageBtn(int idx) {
        const int AllInOne = 0, LevelUpSupport = 1, RandomEquip = 2,
            RandomRelic = 3, EquipUpgradeSupport = 4, MiningSupport = 5;

        RewardItemSO rwDt = HM._.rwlm.RwdItemDt;

        //TODO IAP PURCHASE

        //* 購入完了DIM 表示
        DM._.DB.ShopDB.IsPruchasedPackages[idx] = true;
        PackageDimObjs[idx].SetActive(true);

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
                rewardList.Add(new (rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.GoldClover], 10));
                rewardList.Add(new (rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.SteamPack0], 7));
                rewardList.Add(new (rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.SteamPack1], 7));
                rewardList.Add(new (rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.BizzardScroll], 7));
                rewardList.Add(new (rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.LightningScroll], 7));
                break;
            case RandomEquip: {
                //* リワードでもらえるアイテム数（最大表示数を超えたら、閉じると続けて再実行するため）
                if(EquipPackageCnt == 0)
                    EquipPackageCnt = 24;

                List<int> gradePerList = rwDt.Rwd_ChestEquipment.RwdGradeTb.EquipPerList;

                //* 等級パーセントリスト 準備
                int COMMON = gradePerList[0];
                int RARE = COMMON + gradePerList[1];
                int EPIC = RARE + gradePerList[2];
                int UNIQUE = EPIC + gradePerList[3];
                int LEGEND = UNIQUE + gradePerList[4];
                int MYTH = LEGEND + gradePerList[5];

                //* ランダム等級 適用
                int cnt = Mathf.Min(Config.MAX_REWARD_SLOT, EquipPackageCnt);
                int lastIdx = cnt - 1;

                CheckEquipPackageAction(cnt, RandomEquip);

                for(int i = 0; i < cnt; i++) {
                    int rdPer = Random.Range(0, 1000);
                    int grade = (rdPer < COMMON)? 0
                        : (rdPer < RARE)? 1
                        : (rdPer < EPIC)? 2
                        : (rdPer < UNIQUE)? 3
                        : (rdPer < LEGEND)? 4
                        : (rdPer < MYTH)? 5
                        : 6; // PRIME
                    
                    int rdItemKind = Random.Range(0, 3);
                    var randItemDts = (rdItemKind == 0)? rwDt.WeaponDatas
                        : (rdItemKind == 1)? rwDt.ShoesDatas
                        : rwDt.RingDatas;
                    
                    //* リワード追加
                    rewardList.Add(new (randItemDts[(i == lastIdx && EquipPackageCnt == 0)? (int)Enum.Grade.Myth : grade]));
                }
                break;
            }
            case RandomRelic: {
                //* リワードでもらえるアイテム数（最大表示数を超えたら、閉じると続けて再実行するため）
                if(EquipPackageCnt == 0)
                    EquipPackageCnt = 18;

                List<int> gradePerList = rwDt.Rwd_ChestEquipment.RwdGradeTb.RelicPerList;

                //* 等級パーセントリスト 準備
                int EPIC = gradePerList[0];
                int UNIQUE = EPIC + gradePerList[1];
                int LEGEND = UNIQUE + gradePerList[2];
                int MYTH = LEGEND + gradePerList[3];

                //* ランダム等級 適用
                int cnt = Mathf.Min(Config.MAX_REWARD_SLOT, EquipPackageCnt);
                int lastIdx = cnt - 1;

                CheckEquipPackageAction(cnt, RandomRelic);

                for(int i = 0; i < cnt; i++) {
                    int rdPer = Random.Range(0, 1000);
                    int grade = (rdPer < EPIC)? 0
                        : (rdPer < UNIQUE)? 1
                        : (rdPer < LEGEND)? 2
                        : (rdPer < MYTH)? 3
                        : 4; // PRIME

                    //* リワード追加
                    const int offset = 2; //* RelicはEpicから始まるため
                    int relicLegendIdx = (int)Enum.Grade.Legend - offset;
                    int gradeIdx = (i == lastIdx && EquipPackageCnt == 0)? relicLegendIdx : grade;
                    ItemSO relicDt = rwDt.RelicDatas[gradeIdx];
                    AbilityType[] relicAbilities = HM._.ivCtrl.InventoryData.CheckRelicAbilitiesData(relicDt);
                    rewardList.Add(new (relicDt, quantity: 1, relicAbilities));
                }
                break;
            }
            case EquipUpgradeSupport:
                rewardList.Add(new (rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.SoulStone], 15));
                rewardList.Add(new (rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.MagicStone], 25));
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
    /// <summary>
    /// SHOPで宝箱クリック
    /// </summary>
    /// <param name="chestIdx">0 : COMMON, 1: DIAMOND, 2 : EQUIP, 3 : GOLD, 4 : PREMIUM</param>
    public void OnClickChestBtn(int chestIdx)
        => HM._.cim.ShowChestInfoPopUp(chestIdx);
    /// <summary>
    /// ダイアモンド購入
    /// </summary> <summary>
    public void OnClickDiamondBtn(int diamondIdx) {
        const int FREE_DIAMOND = 0,
            DIAMOND_TINY = 1,
            DIAMOND_SMALL = 2,
            DIAMOND_MEDIUM = 3,
            DIAMOND_BIG = 4,
            DIAMOND_HUGE = 5;

        RewardItemSO rwDt = HM._.rwlm.RwdItemDt;
        ItemSO DIAMOND = rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Diamond];

        var rewardList = new List<RewardItem>();
        switch(diamondIdx) {
            case FREE_DIAMOND:
                //* Daily Item
                if(DM._.DB.ShopDB.DailyItems[ShopDB.FREE_TINY].IsAccept)
                    return;

                //TODO AD

                DM._.DB.ShopDB.SetAcceptData(ShopDB.FREE_TINY);
                FreeTinyDiamondDim.SetActive(true);

                rewardList.Add(new (DIAMOND, 10));
                break;
            case DIAMOND_TINY:
                rewardList.Add(new (DIAMOND, 180));
                break;
            case DIAMOND_SMALL:
                rewardList.Add(new (DIAMOND, 500));
                break;
            case DIAMOND_MEDIUM:
                rewardList.Add(new (DIAMOND, 1200));
                break;
            case DIAMOND_BIG:
                rewardList.Add(new (DIAMOND, 6500));
                break;
            case DIAMOND_HUGE:
                rewardList.Add(new (DIAMOND, 15000));
                break;
        }

        HM._.rwlm.ShowReward(rewardList);
        HM._.rwm.UpdateInventory(rewardList);
    }
    /// <summary>
    /// コイン購入
    /// </summary> <summary>
    public void OnClickCoinBtn(int coinIdx) {
        //* Try Purchase
        bool isSuccess = Config.H_PRICE.SHOP.TryPurchaseCoinPack(coinIdx);
        if(!isSuccess) return;

        RewardItemSO rwDt = HM._.rwlm.RwdItemDt;
        ItemSO COIN = rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Coin];

        var rewardList = new List<RewardItem>();
        switch(coinIdx) {
            case Config.H_PRICE.SHOP.COIN_TINY: {
                rewardList.Add(new (COIN, 600));
                break;
            }
            case Config.H_PRICE.SHOP.COIN_MEDIUM: {
                rewardList.Add(new (COIN, 12000));
                break;
            }
            case Config.H_PRICE.SHOP.COIN_HUGE: {
                rewardList.Add(new (COIN, 48000));
                break;
            }
        }

        HM._.rwlm.ShowReward(rewardList);
        HM._.rwm.UpdateInventory(rewardList);
    }
#endregion
#region FUNC
    private void CheckEquipPackageAction(int cnt, int packageIdx) {
        //* 表示したアイテム数を引く
        EquipPackageCnt -= cnt;
        //* もし０以下だったら、０に正しく初期化
        if(EquipPackageCnt < 0) EquipPackageCnt = 0;

        //* カウントが０以上なら、リワードアイテム数が残っているので、閉じるイベントがしたらActionで再実行
        if(EquipPackageCnt > 0)
            OnClickEquipPackage = () => OnClickPackageBtn(packageIdx);
        else
            OnClickEquipPackage = null; //ではなければ、NULLにして終了
    }

    private void ShowShopPanel(bool isShow) {
        SM._.SfxPlay(SM.SFX.ClickSFX);
        WindowObj.SetActive(isShow);
        HM._.hui.TopNavCanvas.sortingOrder = isShow? 101 : 99;
        HM._.dailyMs.OnUpdateUI.Invoke();
    }
    private void InitUI() {
        Array.ForEach(IsActiveTapBtnCates, isActive => isActive = false);
        Array.ForEach(TapBtns, btn => btn.GetComponent<Image>().color = TapBtnInActiveClr);
        Array.ForEach(CateTitleLineObjs, titleLine => titleLine.SetActive(true));
        Array.ForEach(CateGroupObjs, groupObj => groupObj.SetActive(true));

        int i = 0;
        //* Package購入結果 Dim表示
        Array.ForEach(PackageDimObjs, dim => dim.SetActive(DM._.DB.ShopDB.IsPruchasedPackages[i++]));

        //* Chest Icon And Price
        i = 0;
        Array.ForEach(ChestPriceTxts, priceTxt => 
            priceTxt.text = GetChestPriceTxtFormet(i++)
        );
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
    /// <summary>
    /// SHOPの宝箱の値段と財貨アイコン情報をテキストFORMATで返す
    /// </summary>
    public string GetChestPriceTxtFormet(int chestIdx) {
        string keyword = Config.H_PRICE.SHOP.CHEST_PRICES[chestIdx];
        var splitDt = keyword.Split("_");
        string spriteTag = splitDt[0];
        string priceStr = splitDt[1];
        return $"<size=70%><sprite name={spriteTag}></size> {priceStr}";
    }
#endregion
}
