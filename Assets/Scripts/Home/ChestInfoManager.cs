using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChestInfoManager : MonoBehaviour {
    [field:SerializeField] public GameObject WindowObj {get; set;}
    [field:SerializeField] public TMP_Text NameTxt {get; set;}
    [field:SerializeField] public TMP_Text InfoTxt {get; set;}
    [field:SerializeField] public TMP_Text PriceBtnTxt {get; set;}

    [field:Header("CHEST DATA SO")]



    #region EVENT
        public void OnClickPurchaseBtn() {

        }
        public void OnClickCloseBtn() {
            SM._.SfxPlay(SM.SFX.ClickSFX);
            WindowObj.SetActive(false);
        }
    #endregion
    #region FUNC
        /// <summary>
        /// 宝箱情報POPUP 表示
        /// </summary>
        /// <param name="chestIdx">0 : COMMON, 1: DIAMOND, 2 : EQUIP, 3 : GOLD, 4 : PREMIUM</param>
        public void ShowChestInfoPopUp(int chestIdx) {
            SM._.SfxPlay(SM.SFX.ClickSFX);
            WindowObj.SetActive(true);

            //* 名前
            var enumChest = (chestIdx == 0)? Etc.ConsumableItem.ChestCommon
                : (chestIdx == 1)? Etc.ConsumableItem.ChestDiamond
                : (chestIdx == 2)? Etc.ConsumableItem.ChestEquipment
                : (chestIdx == 3)? Etc.ConsumableItem.ChestGold
                : Etc.ConsumableItem.ChestPremium;
            NameTxt.text = Etc.GetChestName(enumChest);

            //* Info
            var rwdDt = HM._.rwlm.RwdItemDt;
            string infoTemp = "";
            switch(chestIdx) {
                case 0:
                case 1: { // COMMON CHEST
                    RewardContentSO chInfo = rwdDt.Rwd_ChestCommon;
                    infoTemp += $"COIN {chInfo.CoinMin} ~ {chInfo.CoinMax}";
                    infoTemp += $"\n? X 1";
                    break;
                }
                // case 1: { // DIAMOND CHEST
                //     RewardContentSO chInfo = rwdDt.Rwd_ChestDiamond;
                //     infoTemp += $"DIAMOND {chInfo.DiamondMin} ~ {chInfo.DiamondMax}";
                //     break;
                // }
                // case 2: { // EQUIP CHEST
                //     break;
                // }
                // case 3: { // GOLD CHEST
                //     break;
                // }
                // case 4: { // PREMIUM CHEST
                //     break;
                // }
            }
            
            InfoTxt.text = infoTemp;

            //* Price
        }
    #endregion
}
