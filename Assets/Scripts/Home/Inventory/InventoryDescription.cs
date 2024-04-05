using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Inventory.Model;

namespace Inventory.UI {
    public class InventoryDescription : MonoBehaviour {
        [field:SerializeField] private bool IsUpgradeValToogle {get; set;}

        [field:Header("CONSUMABLE POPUP")]
        [field:SerializeField] private Image EtcItemBg {get; set;}
        [field:SerializeField] private TMP_Text EtcNameTxt {get; set;}
        [field:SerializeField] private TMP_Text EtcQuantityTxt {get; set;}
        [field:SerializeField] private TMP_Text EtcDescription {get; set;}

        [field:Header("EQUIPMENT POPUP")]
        [field:SerializeField] private Image TopBg {get; set;}
        [field:SerializeField] private Image ItemBg {get; set;}
        [field:SerializeField] private Image ItemImg {get; set;}
        [field:SerializeField] private Image TypeImg {get; set;}
        [field:SerializeField] private TMP_Text TypeTxt {get; set;}
        [field:SerializeField] private TMP_Text NameTxt {get; set;}
        [field:SerializeField] private TMP_Text GradeTxt {get; set;}
        [field:SerializeField] private TMP_Text QuantityTxt {get; set;}
        [field:SerializeField] private TMP_Text LvTxt {get; set;}
        [field:SerializeField] private TMP_Text StarTxt {get; set;}
        [field:SerializeField] private TMP_Text Description {get; set;}
        [field:SerializeField] private TMP_Text UpgradePriceTxt {get; set;}
        [field:SerializeField] private TMP_Text UpgradeSuccessPerTxt {get; set;}
        [field:SerializeField] private RectTransform UpgValToogleHandleTf {get; set;}
        [field:SerializeField] private TMP_Text UpgValToogleHandleTxt {get; set;}

        InventoryUIManager ivm;

        void Awake() => ResetDescription();

        void Start() {
            ivm = HM._.ivm;
            IsUpgradeValToogle = false;
        }

#region EVENT
        public void OnClickEquipBtn() {
            
        }

        public void OnClickUpgradeBtn() {
            var type = ivm.CurInvItem.Data.Type;
            var lv = ivm.CurInvItem.Lv;
            if(type == Enum.ItemType.Etc) return;
            if(type == Enum.ItemType.Relic && lv == Config.RELIC_UPGRADE_MAX) {
                HM._.hui.ShowMsgError("업그레이드 최대치로 더 이상 할 수 없습니다.");
                return;
            }
            if(type != Enum.ItemType.Relic && lv == Config.EQUIP_UPGRADE_MAX) {
                HM._.hui.ShowMsgError("업그레이드 최대치로 더 이상 할 수 없습니다.");
                return;
            }
            HM._.ivCtrl.InventoryData.UpgradeEquipItem(ivm.CurInvItem.Data, ivm.CurInvItem.Quantity,  ++ivm.CurInvItem.Lv, ivm.CurInvItem.RelicAbilities);
        }
        public void OnClickUpgradeValueNoticeToggle() {
            Debug.Log($"OnClickUpgradeValueNoticeToggle():: IsUpgradeValToogleActive= {IsUpgradeValToogle}");
            IsUpgradeValToogle = !IsUpgradeValToogle;
            UpgValToogleHandleTf.anchoredPosition = new Vector2(IsUpgradeValToogle? 50 : -50, UpgValToogleHandleTf.anchoredPosition.y);
            UpgValToogleHandleTxt.text = IsUpgradeValToogle? "ON" : "OFF";
            SetDescription(ivm.CurInvItem.Data, ivm.CurInvItem.Quantity, -1);
        }
        public void OnClickCloseBtn() {
            //TODO
        }
#endregion

#region FUNC
        public void ResetDescription() {
            EtcItemBg.gameObject.SetActive(false);
            EtcNameTxt.text = "";
            EtcQuantityTxt.text = "";
            EtcDescription.text = "";

            ItemImg.gameObject.SetActive(false);
            TypeTxt.text = "";
            NameTxt.text = "";
            GradeTxt.text = "";
            Description.text = "";
            UpgradePriceTxt.text = "";
            UpgradeSuccessPerTxt.text = "";
        }

        public void SetDescription(ItemSO item, int quantity, int lv) {
            Debug.Log($"SetDescription():: item= {item.name}, val= {quantity}");
            int lvIdx = lv - 1;

            if(item.Type == Enum.ItemType.Etc) {
                EtcItemBg.gameObject.SetActive(true);
                EtcItemBg.sprite = item.ItemImg;
                EtcNameTxt.text = item.Name;
                EtcQuantityTxt.text = $"{quantity}";
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
                    : (item.Type == Enum.ItemType.Ring)? "반지"
                    : (item.Type == Enum.ItemType.Relic)? "유물"
                    : "기타";

                NameTxt.text = item.Name;

                bool isRelic = item.Type == Enum.ItemType.Relic;
                int max = isRelic? Config.RELIC_UPGRADE_MAX : Config.EQUIP_UPGRADE_MAX;
                bool isLvMax = lv >= max;

                LvTxt.text = $"Lv.{(isLvMax? "MAX" : lv)}";
                QuantityTxt.text = quantity.ToString();
                StarTxt.text = Util.DrawEquipItemStarTxt(lv: quantity);
                GradeTxt.text = item.Grade.ToString();
                GradeTxt.color = HM._.ivm.GradeClrs[(int)item.Grade];

                //* 能力木テスト 表示
                string resMsg = "";
                string[] sentences = item.Description.Split('\n');
                Debug.Log($"Description Ability Sentences.Length= {sentences.Length}");
                for(int i = 0; i < item.Abilities.Length; i++) {
                    var ability = item.Abilities[i];
                    //* V{N} → 能力数値変換(実際のアイテムデータ)
                    float resItemVal = ability.Val + (lvIdx * ability.UpgradeVal);
                    string abilityMsg = sentences[i].Replace($"V{i}", $"{resItemVal * 100}");
                    //* 強化数値表示 トーグル(登録したアップグレードデータ)
                    string upgradeMsg = (ability.UpgradeVal == 0)? "<color=grey>( 고정 )</color>" : $"<color=green>( {$"+{ability.UpgradeVal * 100}%"} )</color>";
                    string upgradeToogleMsg = IsUpgradeValToogle? upgradeMsg : "";
                    resMsg += $"{abilityMsg} {upgradeToogleMsg}\n";
                }
                Description.text = resMsg;

                //* アップグレードボタン UI
                int[] rPrices = Config.H_PRICE.RELIC_UPG.PRICES;
                int[] ePrices = Config.H_PRICE.EQUIP_UPG.PRICES;
                int[] rPers = Config.H_PRICE.RELIC_UPG.PERS;
                int[] ePers = Config.H_PRICE.EQUIP_UPG.PERS;

                UpgradePriceTxt.text = $"강화\n{(isLvMax? "MAX" : $"<sprite name=Coin>{(isRelic? rPrices[lvIdx] : ePrices[lvIdx])}")}";
                UpgradeSuccessPerTxt.text = isLvMax? "" : $"확률 {(isRelic? rPers[lvIdx] : ePers[lvIdx])}%";
            }
        }
    #endregion
    }
}