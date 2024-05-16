using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using Inventory.Model;

public class ChestInfoManager : MonoBehaviour {
    [field:SerializeField] public GameObject WindowObj {get; set;}
    [field:SerializeField] public GameObject EquipGachaPercentInfoIcon {get; set;}
    [field:SerializeField] public GameObject EquipGachaPercentInfoPopUp {get; set;}
    [field:SerializeField] public TMP_Text MileageAddPointTxt {get; set;}
    [field:SerializeField] public Image ChestImg {get; set;}
    [field:SerializeField] public TMP_Text NameTxt {get; set;}
    [field:SerializeField] public TMP_Text InfoTxt {get; set;}
    [field:SerializeField] public TMP_Text PriceBtnTxt {get; set;}

    public Action OnClickOpenChest = () => {};

    void Start() {
        EquipGachaPercentInfoIcon.SetActive(false);
    }

    #region EVENT
        public void OnClickPurchaseBtn() {
            OnClickOpenChest?.Invoke();
        }
        public void OnClickCloseBtn() {
            SM._.SfxPlay(SM.SFX.ClickSFX);
            WindowObj.SetActive(false);
            EquipGachaPercentInfoIcon.SetActive(false);
        }
        public void OnClickEquipGachaPercentInfoIcon() {
            SM._.SfxPlay(SM.SFX.ClickSFX);
            EquipGachaPercentInfoPopUp.SetActive(true);
        }
        public void OnClickEquipGachaPercentInfoPopUpDim() {
            SM._.SfxPlay(SM.SFX.ClickSFX);
            EquipGachaPercentInfoPopUp.SetActive(false);
        }
    #endregion
    #region FUNC
        private string SetCommonChest(RewardItemSO rwdDt) {
            string infoTemp = "";

            NameTxt.text = Etc.GetChestName(Etc.ConsumableItem.ChestCommon);
            ChestImg.sprite = rwdDt.EtcConsumableDatas[(int)Etc.ConsumableItem.ChestCommon].ItemImg;
            RewardContentSO chInfo = rwdDt.Rwd_ChestCommon;
            infoTemp += $"<sprite name=Coin> {chInfo.CoinMin} ~ {chInfo.CoinMax}";
            infoTemp += $"\n<sprite name=Random> x 1";

            return infoTemp;
        }

        private string SetEquipChest(RewardItemSO rwdDt, int cnt) {
            string infoTemp = "";

            NameTxt.text = Etc.GetChestName(Etc.ConsumableItem.ChestEquipment) + $" {cnt}개";
            ChestImg.sprite = rwdDt.EtcConsumableDatas[(int)Etc.ConsumableItem.ChestEquipment].ItemImg;
            infoTemp += $"<sprite name=Equip> x {cnt}";
            MileageAddPointTxt.text = $"X{cnt}";

            return infoTemp;
        }

        public void SetInfoTxtUI(string name, Sprite itemImg, string description) {
            NameTxt.text = name;
            ChestImg.sprite = itemImg;
            InfoTxt.text = description;
        }

        private void DailyFreeCommonChest() {
            RewardItemSO rwDt = HM._.rwlm.RwdItemDt;
            rwDt.OpenRewardContent(rwDt.Rwd_ChestCommon);
        }

        /// <summary>
        /// 宝箱情報POPUP 表示
        /// </summary>
        /// <param name="chestIdx"> Config:: H_PRICE:: SHOPへある const int変数を参考</param>
        public void ShowChestInfoPopUp(int chestIdx) {
            SM._.SfxPlay(SM.SFX.ClickSFX);
            WindowObj.SetActive(true);
            EquipGachaPercentInfoIcon.SetActive(chestIdx >= 5);

            //* Icon And Price
            PriceBtnTxt.text = HM._.shopMg.GetChestPriceTxtFormet(chestIdx);

            //* Info
            RewardItemSO rwDt = HM._.rwlm.RwdItemDt;
            string infoTemp = "";
            switch(chestIdx) {
                case Config.H_PRICE.SHOP.FREECOMMON: {
                    //* Daily Item
                    // if(DM._.DB.ShopDB.DailyItems[ShopDB.FREE_COMMON].IsAccept)
                    //     return;

                    infoTemp = SetCommonChest(rwDt);
                    PriceBtnTxt.text = !DM._.DB.ShopDB.DailyItems[ShopDB.FREE_COMMON].IsOnetimeFree? "무료"
                        : "<sprite name=Ad>광고";

                    //* 次の購入イベント登録
                    OnClickOpenChest = () => {
                        //* 一回無料
                        if(!DM._.DB.ShopDB.DailyItems[ShopDB.FREE_COMMON].IsOnetimeFree) {
                            DM._.DB.ShopDB.SetOneTimeFreeTriggerOn(ShopDB.FREE_COMMON);
                            DailyFreeCommonChest();
                            PriceBtnTxt.text = "<sprite name=Ad>광고";
                        }
                        //* 広告見る
                        else {
                            //* Daily Item
                            DM._.DB.ShopDB.SetAcceptTriggerOn(ShopDB.FREE_COMMON);
                            HM._.shopMg.FreeCommonChestDim.SetActive(true);
                            WindowObj.SetActive(false);

                            //* リワード広告
                            AdmobManager._.ProcessRewardAd(DailyFreeCommonChest);
                        }
                    };
                    break;
                }
                case Config.H_PRICE.SHOP.DIAMONDCHEST: {
                    //* Daily Item
                    if(DM._.DB.ShopDB.DailyItems[ShopDB.DIAMOND_CHEST].IsAccept)
                        return;

                    ItemSO chestDt = rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.ChestDiamond];

                    RewardContentSO chInfo = rwDt.Rwd_ChestDiamond;
                    infoTemp += $"<sprite name=Diamond> {chInfo.DiamondMin} ~ {chInfo.DiamondMax}";

                    SetInfoTxtUI (chestDt.Name, chestDt.ItemImg, infoTemp);

                    //* 次の購入イベント登録
                    OnClickOpenChest = () => {
                        //* Try Purchase
                        bool isSuccess = Config.H_PRICE.SHOP.TryPurchaseChest(chestIdx);
                        if(!isSuccess) return;

                        //* Daily Item
                        DM._.DB.ShopDB.SetAcceptTriggerOn(ShopDB.DIAMOND_CHEST);
                        HM._.shopMg.DiamondChestDim.SetActive(true);

                        rwDt.OpenRewardContent(chInfo);
                        WindowObj.SetActive(false);
                    };
                    break;
                }
                case Config.H_PRICE.SHOP.COMMON: {
                    infoTemp = SetCommonChest(rwDt);
                    //* 次の購入イベント登録
                    OnClickOpenChest = () => {
                        //* Try Purchase
                        bool isSuccess = Config.H_PRICE.SHOP.TryPurchaseChest(chestIdx);
                        if(!isSuccess) return;
                        rwDt.OpenRewardContent(rwDt.Rwd_ChestCommon);
                    };
                    
                    break;
                }
                case Config.H_PRICE.SHOP.GOLDCHEST: {
                    ItemSO chestDt = rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.ChestGold];

                    RewardContentSO chInfo = rwDt.Rwd_ChestGold;
                    infoTemp += $"<sprite name=Coin> {chInfo.CoinMin} ~ {chInfo.CoinMax}";
                    infoTemp += $"\n<sprite name=Ore> x 1";
                    infoTemp += $"\n<sprite name=Random> x 2";

                    SetInfoTxtUI (chestDt.Name, chestDt.ItemImg, infoTemp);

                    //* 次の購入イベント登録
                    OnClickOpenChest = () => {
                        //* Try Purchase
                        bool isSuccess = Config.H_PRICE.SHOP.TryPurchaseChest(chestIdx);
                        if(!isSuccess) return;
                        rwDt.OpenRewardContent(chInfo);
                    };
                    break;
                }
                case Config.H_PRICE.SHOP.PREMIUM: {
                    ItemSO chestDt = rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.ChestPremium];

                    RewardContentSO chInfo = rwDt.Rwd_ChestPremium;
                    infoTemp += $"<sprite name=Coin> {chInfo.CoinMin} ~ {chInfo.CoinMax}";
                    infoTemp += $"\t<sprite name=Diamond> {chInfo.DiamondMin} ~ {chInfo.DiamondMax}";
                    infoTemp += $"\n<sprite name=Equip> x 1";
                    infoTemp += $"\t<sprite name=Random> x 5";

                    SetInfoTxtUI(chestDt.Name, chestDt.ItemImg, infoTemp);

                    //* 次の購入イベント登録
                    OnClickOpenChest = () => {
                        //* Try Purchase
                        bool isSuccess = Config.H_PRICE.SHOP.TryPurchaseChest(chestIdx);
                        if(!isSuccess) return;
                        rwDt.OpenRewardContent(chInfo);
                    };
                    break;
                }
                case Config.H_PRICE.SHOP.EQUIP1:
                case Config.H_PRICE.SHOP.EQUIP5:
                case Config.H_PRICE.SHOP.EQUIP10:
                case Config.H_PRICE.SHOP.EQUIP20:
                case Config.H_PRICE.SHOP.EQUIP40:
                {
                    int cnt = (chestIdx == Config.H_PRICE.SHOP.EQUIP1)? 1
                        :(chestIdx == Config.H_PRICE.SHOP.EQUIP5)? 5
                        :(chestIdx == Config.H_PRICE.SHOP.EQUIP10)? 10
                        :(chestIdx == Config.H_PRICE.SHOP.EQUIP20)? 20
                        :(chestIdx == Config.H_PRICE.SHOP.EQUIP40)? 40 : 0;
                    infoTemp = SetEquipChest(rwDt, cnt);
                    //* 次の購入イベント登録
                    OnClickOpenChest = () => {
                        //* Try Purchase
                        bool isSuccess = Config.H_PRICE.SHOP.TryPurchaseChest(chestIdx);
                        if(!isSuccess) return;
                        rwDt.OpenRewardContent(rwDt.Rwd_ChestEquipment, specifiedCnt: cnt);
                        HM._.Mileage += cnt;
                    };
                    break;
                }
            }
            
            InfoTxt.text = infoTemp;

            //* Price
        }
    #endregion
}
