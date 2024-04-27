using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ChestInfoManager : MonoBehaviour {
    [field:SerializeField] public GameObject WindowObj {get; set;}
    [field:SerializeField] public GameObject EquipGachaInfoGroup {get; set;}
    [field:SerializeField] public Image ChestImg {get; set;}
    [field:SerializeField] public TMP_Text NameTxt {get; set;}
    [field:SerializeField] public TMP_Text InfoTxt {get; set;}
    [field:SerializeField] public TMP_Text PriceBtnTxt {get; set;}

    public Action OnClickOpenChest = () => {};

    #region EVENT
        public void OnClickPurchaseBtn() {
            OnClickOpenChest?.Invoke();
        }
        public void OnClickCloseBtn() {
            SM._.SfxPlay(SM.SFX.ClickSFX);
            WindowObj.SetActive(false);
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

            return infoTemp;
        }

        /// <summary>
        /// 宝箱情報POPUP 表示
        /// </summary>
        /// <param name="chestIdx">0 : COMMON, 1: DIAMOND, 2 : EQUIP, 3 : GOLD, 4 : PREMIUM</param>
        public void ShowChestInfoPopUp(int chestIdx) {
            const int FREE_COMMON_CHEST = 0, COMMON_CHEST = 1, GOLD_CHEST = 2,
                PREMIUM_CHEST = 3, DIAMOND_CHEST = 4, 
                EQUIP_CHEST_X1 = 5, EQUIP_CHEST_X6 = 6, EQUIP_CHEST_X12 = 7;

            SM._.SfxPlay(SM.SFX.ClickSFX);
            WindowObj.SetActive(true);
            EquipGachaInfoGroup.SetActive(chestIdx >= 5);

            //* Icon And Price
            PriceBtnTxt.text = HM._.shopMg.GetChestPriceTxtFormet(chestIdx);

            //* Info
            RewardItemSO rwdDt = HM._.rwlm.RwdItemDt;
            string infoTemp = "";
            switch(chestIdx) {
                case FREE_COMMON_CHEST: {
                    HM._.shopMg.FreeCommonChestDim.SetActive(true);
                    infoTemp = SetCommonChest(rwdDt);
                    //* 次の購入イベント登録
                    OnClickOpenChest = () => {
                        rwdDt.OpenRewardContent(rwdDt.Rwd_ChestCommon);
                        WindowObj.SetActive(false);
                    };
                    break;
                }
                case COMMON_CHEST: {
                    infoTemp = SetCommonChest(rwdDt);
                    //* 次の購入イベント登録
                    OnClickOpenChest = () => rwdDt.OpenRewardContent(rwdDt.Rwd_ChestCommon);
                    break;
                }
                case GOLD_CHEST: {
                    NameTxt.text = Etc.GetChestName(Etc.ConsumableItem.ChestGold);
                    ChestImg.sprite = rwdDt.EtcConsumableDatas[(int)Etc.ConsumableItem.ChestGold].ItemImg;
                    RewardContentSO chInfo = rwdDt.Rwd_ChestGold;
                    infoTemp += $"<sprite name=Coin> {chInfo.CoinMin} ~ {chInfo.CoinMax}";
                    infoTemp += $"\n<sprite name=Ore> x 1";
                    infoTemp += $"\n<sprite name=Random> x 2";

                    //* 次の購入イベント登録
                    OnClickOpenChest = () => rwdDt.OpenRewardContent(chInfo);
                    break;
                }
                case PREMIUM_CHEST: {
                    NameTxt.text = Etc.GetChestName(Etc.ConsumableItem.ChestPremium);
                    ChestImg.sprite = rwdDt.EtcConsumableDatas[(int)Etc.ConsumableItem.ChestPremium].ItemImg;
                    RewardContentSO chInfo = rwdDt.Rwd_ChestPremium;
                    infoTemp += $"<sprite name=Coin> {chInfo.CoinMin} ~ {chInfo.CoinMax}";
                    infoTemp += $"\t<sprite name=Diamond> {chInfo.DiamondMin} ~ {chInfo.DiamondMax}";
                    infoTemp += $"\n<sprite name=Equip> x 1";
                    infoTemp += $"\t<sprite name=Random> x 5";

                    //* 次の購入イベント登録
                    OnClickOpenChest = () => rwdDt.OpenRewardContent(chInfo);
                    break;
                }
                case DIAMOND_CHEST: {
                    HM._.shopMg.DiamondChestDim.SetActive(true);
                    NameTxt.text = Etc.GetChestName(Etc.ConsumableItem.ChestDiamond);
                    ChestImg.sprite = rwdDt.EtcConsumableDatas[(int)Etc.ConsumableItem.ChestDiamond].ItemImg;
                    RewardContentSO chInfo = rwdDt.Rwd_ChestDiamond;
                    infoTemp += $"<sprite name=Diamond> {chInfo.DiamondMin} ~ {chInfo.DiamondMax}";

                    //* 次の購入イベント登録
                    OnClickOpenChest = () => {
                        rwdDt.OpenRewardContent(chInfo);
                        WindowObj.SetActive(false);
                    };
                    break;
                }
                case EQUIP_CHEST_X1 : {
                    const int cnt = 1;
                    infoTemp = SetEquipChest(rwdDt, cnt);
                    //* 次の購入イベント登録
                    OnClickOpenChest = () => rwdDt.OpenRewardContent(rwdDt.Rwd_ChestEquipment, specifiedCnt: cnt);
                    break;
                }
                case EQUIP_CHEST_X6 : {
                    const int cnt = 1;
                    infoTemp = SetEquipChest(rwdDt, cnt);
                    //* 次の購入イベント登録
                    OnClickOpenChest = () => rwdDt.OpenRewardContent(rwdDt.Rwd_ChestEquipment, specifiedCnt: cnt);
                    break;
                }
                case EQUIP_CHEST_X12: {
                    const int cnt = 1;
                    infoTemp = SetEquipChest(rwdDt, cnt);
                    //* 次の購入イベント登録
                    OnClickOpenChest = () => rwdDt.OpenRewardContent(rwdDt.Rwd_ChestEquipment, specifiedCnt: cnt);
                    break;
                }
            }
            
            InfoTxt.text = infoTemp;

            //* Price
        }
    #endregion
}
