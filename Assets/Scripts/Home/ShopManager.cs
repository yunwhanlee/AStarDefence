using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;
using TMPro;
using Inventory.Model;

public class ShopManager : MonoBehaviour {
    public const int TAPBTN_PACKAGE = 0, 
        TAPBTN_CHEST = 1, 
        ETC_CHEST = 2,
        TAPBTN_RSC = 3;

    [field:SerializeField] public int EquipPackageCnt {get; set;}
    public Action OnClickEquipPackage;

    [SerializeField] Color TapBtnActiveClr;
    [SerializeField] Color TapBtnInActiveClr;
    [SerializeField] Sprite GameConsumeItemsSpr;

    [field:SerializeField] public bool[] IsActiveTapBtnCates {get; private set;}
    [field:SerializeField] public GameObject WindowObj {get; private set;}
    [field:SerializeField] public GameObject[] CateTitleLineObjs {get; private set;}
    [field:SerializeField] public GameObject[] CateGroupObjs {get; private set;}
    [field:SerializeField] public GameObject[] PackageDimObjs {get; private set;}

    //* 一日制限がある アイテム
    [field:SerializeField] public GameObject FreeCommonChestDim {get; private set;}
    [field:SerializeField] public GameObject DiamondChestDim {get; private set;}
    [field:SerializeField] public GameObject FreeTinyDiamondDim {get; private set;}
    [field:SerializeField] public TMP_Text FreeTinyDiamondTxt {get; private set;}
    [field:SerializeField] public GameObject FreeTinyCoinDim {get; private set;}
    [field:SerializeField] public TMP_Text FreeTinyCoinTxt {get; private set;}

    //* 広告削除購入 DIM
    [field:SerializeField] public GameObject RemoveAdDim {get; private set;}

    [field:SerializeField] public Button[] TapBtns {get; private set;}

    [field:SerializeField] public TMP_Text[] ChestPriceTxts {get; set;}

    void Start() {
        InitUI();

        //* Daily アイテム結果時間 チェック
        var shopDB = DM._.DB.ShopDB;
        FreeCommonChestDim.SetActive(shopDB.TogglePassedDay(ShopDB.FREE_COMMON));
        DiamondChestDim.SetActive(shopDB.TogglePassedDay(ShopDB.DIAMOND_CHEST));
        FreeTinyDiamondDim.SetActive(shopDB.TogglePassedDay(ShopDB.FREE_TINYDIAMOND));
        FreeTinyCoinDim.SetActive(shopDB.TogglePassedDay(ShopDB.FREE_TINYCOIN));
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
            RandomRelic = 3, EquipUpgradeSupport = 4, MiningSupportGoblin = 5,
            GoldPackage = 6, MiningSupportOre = 7;

        RewardItemSO rwDt = HM._.rwlm.RwdItemDt;

        //TODO IAP PURCHASE

        //* 購入完了DIM 表示
        DM._.DB.ShopDB.IsPruchasedPackages[idx] = true;
        PackageDimObjs[idx].SetActive(true);

        List<RewardItem> rewardList = new List<RewardItem>();
        switch(idx) {
            case AllInOne:
                rewardList.Add(new (rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.ChestDiamond], 1));
                rewardList.Add(new (rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.ChestCommon], 10));
                rewardList.Add(new (rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.ChestPremium], 1));
                rewardList.Add(new (rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.ChestGold], 2));
                rewardList.Add(new (rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.ChestEquipment], 20));
                rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.RemoveAd]));
                HM._.Mileage += 99;
                break;
            case LevelUpSupport:
                rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.GoldKey], 15));
                rewardList.Add(new (rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.Clover], 20));
                rewardList.Add(new (rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.GoldClover], 20));
                rewardList.Add(new (rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.SteamPack0], 10));
                rewardList.Add(new (rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.SteamPack1], 10));
                rewardList.Add(new (rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.BizzardScroll], 10));
                rewardList.Add(new (rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.LightningScroll], 10));
                HM._.Mileage += 55;
                break;
            case RandomEquip: {
                //* リワードでもらえるアイテム数（最大表示数を超えたら、閉じると続けて再実行するため）
                if(EquipPackageCnt == 0)
                    EquipPackageCnt = 40;

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
                    rewardList.Add(new (randItemDts[(i == lastIdx && EquipPackageCnt == 0)? (int)Enum.Grade.Myth : grade], 1));
                }
                HM._.Mileage += 89;
                break;
            }
            case RandomRelic: {
                //* リワードでもらえるアイテム数（最大表示数を超えたら、閉じると続けて再実行するため）
                if(EquipPackageCnt == 0)
                    EquipPackageCnt = 30;

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
                HM._.Mileage += 99;
                break;
            }
            case EquipUpgradeSupport:
                rewardList.Add(new (rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.SoulStone], 10));
                rewardList.Add(new (rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.MagicStone], 10));
                rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Coin], 55000));
                HM._.Mileage += 180;
                break;
            case MiningSupportGoblin:
                rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Goblin0], 100));
                rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Goblin1], 50));
                rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Goblin2], 25));
                rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Goblin3], 10));
                rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Goblin4], 5));
                rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Goblin5], 1));
                HM._.Mileage += 79;
                break;
            case GoldPackage:
                rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.GoldKey], 25));
                rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Coin], 100000));
                HM._.Mileage += 220;
                break;
            case MiningSupportOre:
                rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Ore1], 200));
                rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Ore2], 100));
                rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Ore3], 50));
                rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Ore4], 25));
                rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Ore5], 10));
                rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Ore6], 5));
                rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Ore7], 1));
                HM._.Mileage += 79;
                break;
        }

        HM._.rwlm.ShowReward(rewardList);
    }
    /// <summary>
    /// SHOPで宝箱クリック
    /// </summary>
    /// <param name="chestIdx">0 : COMMON, 1: DIAMOND, 2 : EQUIP, 3 : GOLD, 4 : PREMIUM</param>
    public void OnClickChestBtn(int chestIdx)
        => HM._.cim.ShowChestInfoPopUp(chestIdx);

    /// <summary>
    /// その他アイテム購入（★少し間違えたが、ChestPopUpのWindowを活用）
    /// </summary>    
    public void OnClickEtcItemBtn(int etcIdx) {
        SM._.SfxPlay(SM.SFX.ClickSFX);
        ChestInfoManager cim = HM._.cim;
        RewardItemSO rwDt = HM._.rwlm.RwdItemDt;

        cim.WindowObj.SetActive(true);

        //* Icon And Price
        cim.PriceBtnTxt.text = HM._.shopMg.GetEtcPriceTxtFormet(etcIdx);

        //* Info
        switch (etcIdx) {
            case Config.H_PRICE.SHOP.GOLDKEY:
            case Config.H_PRICE.SHOP.GOLDKEY_5:
            case Config.H_PRICE.SHOP.GOLDKEY_10:
            {
                ItemSO goldKeyDt = rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.GoldKey];
                cim.SetInfoTxtUI(goldKeyDt.Name, goldKeyDt.ItemImg, goldKeyDt.Description);

                //* 次の購入イベント登録
                cim.OnClickOpenChest = () => {
                    //* Try Purchase
                    bool isSuccess = Config.H_PRICE.SHOP.TryPurchaseEtc(etcIdx);
                    if(!isSuccess) return;
                    //* 数量
                    int quantity = (etcIdx == Config.H_PRICE.SHOP.GOLDKEY)? 1
                        : (etcIdx == Config.H_PRICE.SHOP.GOLDKEY_5)? 5
                        : (etcIdx == Config.H_PRICE.SHOP.GOLDKEY_10)? 10 : 0;
                    //* リワード
                    var rewardList = new List<RewardItem>() {new (goldKeyDt, quantity) };
                    HM._.rwlm.ShowReward(rewardList);
                    // HM._.rwm.CoUpdateInventoryAsync(rewardList);
                };
                break;
            }
            case Config.H_PRICE.SHOP.SOULSTONE:
            case Config.H_PRICE.SHOP.SOULSTONE_5:
            case Config.H_PRICE.SHOP.SOULSTONE_10:
            {
                ItemSO soulStoneDt = rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.SoulStone];
                cim.SetInfoTxtUI(soulStoneDt.Name, soulStoneDt.ItemImg, soulStoneDt.Description);

                //* 次の購入イベント登録
                cim.OnClickOpenChest = () => {
                    //* Try Purchase
                    bool isSuccess = Config.H_PRICE.SHOP.TryPurchaseEtc(etcIdx);
                    if(!isSuccess) return;
                    //* 数量
                    int quantity = (etcIdx == Config.H_PRICE.SHOP.SOULSTONE)? 1
                        : (etcIdx == Config.H_PRICE.SHOP.SOULSTONE_5)? 5
                        : (etcIdx == Config.H_PRICE.SHOP.SOULSTONE_10)? 10 : 0;
                    //* リワード
                    var rewardList = new List<RewardItem>() { new (soulStoneDt, quantity) };
                    HM._.rwlm.ShowReward(rewardList);
                    // HM._.rwm.CoUpdateInventoryAsync(rewardList);
                };
                break;
            }
            case Config.H_PRICE.SHOP.MAGICSTONE:
            case Config.H_PRICE.SHOP.MAGICSTONE_5:
            case Config.H_PRICE.SHOP.MAGICSTONE_10:
            {
                ItemSO magicStoneDt = rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.MagicStone];
                cim.SetInfoTxtUI(magicStoneDt.Name, magicStoneDt.ItemImg, magicStoneDt.Description);

                //* 次の購入イベント登録
                cim.OnClickOpenChest = () => {
                    //* Try Purchase
                    bool isSuccess = Config.H_PRICE.SHOP.TryPurchaseEtc(etcIdx);
                    if(!isSuccess) return;
                    //* 数量
                    int quantity = (etcIdx == Config.H_PRICE.SHOP.MAGICSTONE)? 1
                        : (etcIdx == Config.H_PRICE.SHOP.MAGICSTONE_5)? 5
                        : (etcIdx == Config.H_PRICE.SHOP.MAGICSTONE_10)? 10 : 0;
                    //* リワード
                    var rewardList = new List<RewardItem>() { new (magicStoneDt, quantity) };
                    HM._.rwlm.ShowReward(rewardList);
                    // HM._.rwm.CoUpdateInventoryAsync(rewardList);
                };
                break;
            }
            case Config.H_PRICE.SHOP.RANDOM_CONSUMEITEM:
            case Config.H_PRICE.SHOP.RANDOM_CONSUMEITEM_5:
            case Config.H_PRICE.SHOP.RANDOM_CONSUMEITEM_10:
            {
                ItemSO[] itemDts = new ItemSO[] {
                    rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.SteamPack0],
                    rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.SteamPack1],
                    rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.BizzardScroll],
                    rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.LightningScroll]
                };

                string infoMsg = $"{itemDts[0].Name}: {itemDts[0].Description}"
                    + $"\n{itemDts[1].Name}: {itemDts[1].Description}"
                    + $"\n{itemDts[2].Name}: {itemDts[2].Description}"
                    + $"\n{itemDts[3].Name}: {itemDts[3].Description}";

                cim.SetInfoTxtUI("랜덤 소비아이템", GameConsumeItemsSpr, infoMsg);

                //* 次の購入イベント登録
                cim.OnClickOpenChest = () => {
                    //* Try Purchase
                    bool isSuccess = Config.H_PRICE.SHOP.TryPurchaseEtc(etcIdx);
                    if(!isSuccess) return;

                    //* 数量
                    int quantity = (etcIdx == Config.H_PRICE.SHOP.RANDOM_CONSUMEITEM)? 1
                        : (etcIdx == Config.H_PRICE.SHOP.RANDOM_CONSUMEITEM_5)? 5
                        : (etcIdx == Config.H_PRICE.SHOP.RANDOM_CONSUMEITEM_10)? 10 : 0;

                    //* リワード
                    var rewardList = new List<RewardItem>();
                    for(int i = 0; i < quantity; i++) 
                        rewardList.Add(new (itemDts[Random.Range(0, 4)]));
                    HM._.rwlm.ShowReward(rewardList);
                    // HM._.rwm.CoUpdateInventoryAsync(rewardList);
                };
                break;
            }
        }
    }

    public void OnclickFreeDiamondBtn() {
        RewardItemSO rwDt = HM._.rwlm.RwdItemDt;
        ItemSO DIAMOND = rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Diamond];
        var rewardList = new List<RewardItem>();

        //* Daily Item
        // if(DM._.DB.ShopDB.DailyItems[ShopDB.FREE_TINY].IsAccept)
        //     return;

        FreeTinyDiamondTxt.text = !DM._.DB.ShopDB.DailyItems[ShopDB.FREE_TINYDIAMOND].IsOnetimeFree? "무료"
            : "<sprite name=Ad>광고";

        //* 一回無料
        if(!DM._.DB.ShopDB.DailyItems[ShopDB.FREE_TINYDIAMOND].IsOnetimeFree) {
            DM._.DB.ShopDB.SetOneTimeFreeTriggerOn(ShopDB.FREE_TINYDIAMOND);
            HM._.rwlm.ShowReward(new List<RewardItem>() {new (DIAMOND, 10)});
            FreeTinyDiamondTxt.text = "<sprite name=Ad>광고";
        }
        //* 広告見る
        else {
            AdmobManager._.ProcessRewardAd(() => {
                DM._.DB.ShopDB.SetAcceptTriggerOn(ShopDB.FREE_TINYDIAMOND);
                FreeTinyDiamondDim.SetActive(true);
                HM._.rwlm.ShowReward(new List<RewardItem>() {new (DIAMOND, 10)});
            });
        }
    }

    /// <summary>
    /// ダイアモンド購入
    /// </summary> <summary>
    public void OnClickDiamondBtn(int diamondIdx) {
        const int DIAMOND_TINY = 1,
            DIAMOND_SMALL = 2,
            DIAMOND_MEDIUM = 3,
            DIAMOND_BIG = 4,
            DIAMOND_HUGE = 5;

        RewardItemSO rwDt = HM._.rwlm.RwdItemDt;
        ItemSO DIAMOND = rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Diamond];

        var rewardList = new List<RewardItem>();
        
        switch(diamondIdx) {
            case DIAMOND_TINY:
                rewardList.Add(new (DIAMOND, 180));
                break;
            case DIAMOND_SMALL:
                rewardList.Add(new (DIAMOND, 540));
                break;
            case DIAMOND_MEDIUM:
                rewardList.Add(new (DIAMOND, 1260));
                break;
            case DIAMOND_BIG:
                rewardList.Add(new (DIAMOND, 5000));
                break;
            case DIAMOND_HUGE:
                rewardList.Add(new (DIAMOND, 15000));
                break;
        }

        HM._.rwlm.ShowReward(rewardList);
        // HM._.rwm.CoUpdateInventoryAsync(rewardList);
    }

    public void OnclickFreeCoinBtn() {
        RewardItemSO rwDt = HM._.rwlm.RwdItemDt;
        ItemSO COIN = rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Coin];
        var rewardList = new List<RewardItem>();

        FreeTinyCoinTxt.text = !DM._.DB.ShopDB.DailyItems[ShopDB.FREE_TINYCOIN].IsOnetimeFree? "무료"
            : "<sprite name=Ad>광고";

        //* 一回無料
        if(!DM._.DB.ShopDB.DailyItems[ShopDB.FREE_TINYCOIN].IsOnetimeFree) {
            DM._.DB.ShopDB.SetOneTimeFreeTriggerOn(ShopDB.FREE_TINYCOIN);
            HM._.rwlm.ShowReward(new List<RewardItem>() {new (COIN, 600)});
            FreeTinyCoinTxt.text = "<sprite name=Ad>광고";
        }
        //* 広告見る
        else {
            AdmobManager._.ProcessRewardAd(() => {
                DM._.DB.ShopDB.SetAcceptTriggerOn(ShopDB.FREE_TINYCOIN);
                FreeTinyCoinDim.SetActive(true);
                HM._.rwlm.ShowReward(new List<RewardItem>() {new (COIN, 600)});
            });
        }
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
                rewardList.Add(new (COIN, 900));
                break;
            }
            case Config.H_PRICE.SHOP.COIN_MEDIUM: {
                rewardList.Add(new (COIN, 18000));
                break;
            }
            case Config.H_PRICE.SHOP.COIN_HUGE: {
                rewardList.Add(new (COIN, 72000));
                break;
            }
        }

        HM._.rwlm.ShowReward(rewardList);
        // HM._.rwm.CoUpdateInventoryAsync(rewardList);
    }

    /// <summary>
    /// 広告削除
    /// </summary>
    public void OnClickRemoveAdBtn() {
        RewardItemSO rwDt = HM._.rwlm.RwdItemDt;
        var rewardList = new List<RewardItem>() {
            new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.RemoveAd])
        };
        HM._.rwlm.ShowReward(rewardList);
        RemoveAdDim.SetActive(true);
        // HM._.rwm.CoUpdateInventoryAsync(rewardList);
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
        HM._.hui.IsActivePopUp = isShow;
        HM._.hui.SetTopNavOrderInLayer(isShow);
        HM._.dailyMs.OnUpdateUI.Invoke();
    }
    public void InitUI() {
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
    /// <summary>
    /// SHOPのETCの値段と財貨アイコン情報をテキストFORMATで返す
    /// </summary>
    public string GetEtcPriceTxtFormet(int chestIdx) {
        string keyword = Config.H_PRICE.SHOP.ETC_PRICES[chestIdx];
        var splitDt = keyword.Split("_");
        string spriteTag = splitDt[0];
        string priceStr = splitDt[1];
        return $"<size=70%><sprite name={spriteTag}></size> {priceStr}";
    }
#endregion
}
