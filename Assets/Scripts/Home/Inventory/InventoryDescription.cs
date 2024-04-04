using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Inventory.Model;
using Unity.VisualScripting;

namespace Inventory.UI {
    public class InventoryDescription : MonoBehaviour {
        [field:SerializeField] private bool IsUpgradeValToogleActive {get; set;}

        [field:Header("CONSUMABLE POPUP")]
        [field:SerializeField] private Image EtcItemBg {get; set;}
        [field:SerializeField] private TMP_Text EtcNameTxt {get; set;}
        [field:SerializeField] private TMP_Text EtcCountTxt {get; set;}
        [field:SerializeField] private TMP_Text EtcDescription {get; set;}

        [field:Header("EQUIPMENT POPUP")]
        [field:SerializeField] private Image TopBg {get; set;}
        [field:SerializeField] private Image ItemBg {get; set;}
        [field:SerializeField] private Image ItemImg {get; set;}
        [field:SerializeField] private Image TypeImg {get; set;}
        [field:SerializeField] private TMP_Text TypeTxt {get; set;}
        [field:SerializeField] private TMP_Text NameTxt {get; set;}
        [field:SerializeField] private TMP_Text GradeTxt {get; set;}
        [field:SerializeField] private TMP_Text LvTxt {get; set;}
        [field:SerializeField] private TMP_Text StarTxt {get; set;}
        [field:SerializeField] private TMP_Text Description {get; set;}
        [field:SerializeField] private TMP_Text UpgradePriceTxt {get; set;}
        [field:SerializeField] private TMP_Text UpgradeSuccessPerTxt {get; set;}
        [field:SerializeField] private RectTransform UpgValToogleHandleTf {get; set;}
        [field:SerializeField] private TMP_Text UpgValToogleHandleTxt {get; set;}

        void Awake() => ResetDescription();

        void Start() {
            IsUpgradeValToogleActive = false;
        }

#region EVENT
        public void OnClickEquipBtn() {
            
        }

        public void OnClickUpgradeBtn() {
            if(HM._.ivm.CurInvItem.Data.Type == Enum.ItemType.Etc) return;
            HM._.ivCtrl.InventoryData.UpgradeEquipItem(HM._.ivm.CurInvItem.Data, ++HM._.ivm.CurInvItem.Val);
        }
        public void OnClickUpgradeValueNoticeToggle() {
            Debug.Log($"OnClickUpgradeValueNoticeToggle():: IsUpgradeValToogleActive= {IsUpgradeValToogleActive}");
            UpgValToogleHandleTf.anchoredPosition = new Vector2(IsUpgradeValToogleActive? -50 : 50, UpgValToogleHandleTf.anchoredPosition.y);
            UpgValToogleHandleTxt.text = IsUpgradeValToogleActive? "OFF" : "ON";
            IsUpgradeValToogleActive = !IsUpgradeValToogleActive;
        }
        public void OnClickDeleteIconBtn() {
            Debug.Log("DELETE ITEM");
        }
        public void OnClickCloseBtn() {
            //TODO
        }
#endregion

#region FUNC
        public void ResetDescription() {
            EtcItemBg.gameObject.SetActive(false);
            EtcNameTxt.text = "";
            EtcCountTxt.text = "";
            EtcDescription.text = "";

            ItemImg.gameObject.SetActive(false);
            TypeTxt.text = "";
            NameTxt.text = "";
            GradeTxt.text = "";
            Description.text = "";
            UpgradePriceTxt.text = "";
            UpgradeSuccessPerTxt.text = "";
        }

        public void SetDescription(ItemSO item, int val, int itemIdx) {
            Debug.Log($"SetDescription():: item= {item.name}, val= {val}");
            if(item.Type == Enum.ItemType.Etc) {
                EtcItemBg.gameObject.SetActive(true);
                EtcItemBg.sprite = item.ItemImg;
                EtcNameTxt.text = item.Name;
                EtcCountTxt.text = $"{val}";
                EtcDescription.text = item.Description;
            }
            else {
                ItemImg.gameObject.SetActive(true);
                //* スタイル
                TopBg.color = HM._.ivm.GradeClrs[(int)item.Grade];
                ItemBg.sprite = HM._.ivm.GradeBgSprs[(int)item.Grade];
                ItemImg.sprite = item.ItemImg;
                TypeImg.sprite = HM._.ivm.TypeSprs[(int)item.Type];
                TypeTxt.text = (item.Type == Enum.ItemType.Weapon)? "무기"
                    : (item.Type == Enum.ItemType.Shoes)? "신발"
                    : (item.Type == Enum.ItemType.Accessories)? "악세서리"
                    : (item.Type == Enum.ItemType.Relic)? "유물"
                    : "기타";

                NameTxt.text = item.Name;

                bool isRelic = item.Type == Enum.ItemType.Relic;
                int max = isRelic? Config.RELIC_UPGRADE_MAX : Config.EQUIP_UPGRADE_MAX;
                bool isLvMax = val >= max;

                LvTxt.text = $"Lv.{(isLvMax? "MAX" : val)}";
                StarTxt.text = Util.DrawEquipItemStarTxt(lv: val);
                GradeTxt.text = item.Grade.ToString();
                GradeTxt.color = HM._.ivm.GradeClrs[(int)item.Grade];
                Description.text = item.Description;
                
                int[] rPrices = Config.H_PRICE.RELIC_UPG.PRICES;
                int[] ePrices = Config.H_PRICE.EQUIP_UPG.PRICES;

                int lvIdx = val - 1;
                UpgradePriceTxt.text = $"강화\n{(isLvMax? "MAX" : $"<sprite name=Coin>{(isRelic? rPrices[lvIdx] : ePrices[lvIdx])}")}";

                int successPer = isRelic? Config.H_PRICE.RELIC_UPG.PERS[lvIdx] : Config.H_PRICE.EQUIP_UPG.PERS[lvIdx];
                UpgradeSuccessPerTxt.text = isLvMax? "" : $"확률 {successPer}%";
            }
        }
    #endregion
    }
}