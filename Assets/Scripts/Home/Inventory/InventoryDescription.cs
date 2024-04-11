using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Inventory.Model;
using System;

namespace Inventory.UI {
    /// <summary>
    /// インベントリポップアップ
    /// </summary>
    public class InventoryDescription : MonoBehaviour {
        [field:SerializeField] private bool IsUpgradeValToogle {get; set;}

        [field:Header("CONSUMABLE POPUP")]
        private Action OnClickConsumPopUpConfirmBtn = () => {};
        [field:SerializeField] private Image EtcItemBg {get; set;}
        [field:SerializeField] private TMP_Text EtcNameTxt {get; set;}
        [field:SerializeField] private TMP_Text EtcQuantityTxt {get; set;}
        [field:SerializeField] private TMP_Text EtcDescription {get; set;}
        [field:SerializeField] private TMP_Text EtcConfirmBtnTxt {get; set;}

        [field:Header("EQUIPMENT POPUP")]
        [field:SerializeField] private Image TopBg {get; set;}
        [field:SerializeField] private Image GlowBg {get; set;}
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
        public void OnClickConsumableItemConfirm() {
            Debug.Log($"OnClickConsumableItemConfirm():: CurInvItem.Name= {HM._.ivm.CurInvItem.Data.Name}");
            OnClickConsumPopUpConfirmBtn?.Invoke();
        }

        public void OnClickEquipBtn() {
            HM._.ivCtrl.EquipItemSlot();
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
            SetDescription(
                ivm.CurInvItem.Data, 
                ivm.CurInvItem.Quantity, 
                ivm.CurInvItem.Lv, 
                ivm.CurInvItem.RelicAbilities
            );
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

        private void ActiveCloverItem(GameObject icon, bool isActive, Color ActiveColor) {
            icon.SetActive(isActive);
            EtcConfirmBtnTxt.text = isActive? "활성화" : "비활성화";
            EtcConfirmBtnTxt.color = isActive? ActiveColor: Color.gray;
        }

        public void SetDescription(ItemSO item, int quantity, int lv, AbilityType[] relicAbilities) {
            Debug.Log($"SetDescription():: item= {item.name}, val= {quantity}");
            var hui = HM._.hui;
            var db = DM._.DB;
            var rwlm = HM._.rwlm;
            int lvIdx = lv - 1;

            //* その他 アイテム
            if(item.Type == Enum.ItemType.Etc) {
                //* ボタンのテキスト色 初期化
                EtcConfirmBtnTxt.color = Color.white; 
                
                //* Active (Gold) Clover EXP Bonus
                if(item.name == $"{Etc.ConsumableItem.Clover}") {
                    ActiveCloverItem(hui.CloverActiveIcon, db.IsCloverActive, Color.green);
                    //* Actionボタン 購読
                    OnClickConsumPopUpConfirmBtn = () => {
                        db.IsCloverActive = !db.IsCloverActive;
                        ActiveCloverItem(hui.CloverActiveIcon, db.IsCloverActive, Color.green);
                    };
                }
                else if(item.name == $"{Etc.ConsumableItem.GoldClover}") {
                    ActiveCloverItem(hui.GoldCloverActiveIcon, db.IsGoldCloverActive, Color.yellow);
                    //* Actionボタン 購読
                    OnClickConsumPopUpConfirmBtn = () => {
                        db.IsGoldCloverActive = !db.IsGoldCloverActive;
                        ActiveCloverItem(hui.GoldCloverActiveIcon, db.IsGoldCloverActive, Color.yellow);
                    };
                }
                //* Ingameで使えるアイテムの情報 表示
                else if(item.name == $"{Etc.ConsumableItem.BizzardScroll}"
                || item.name == $"{Etc.ConsumableItem.LightningScroll}"
                || item.name == $"{Etc.ConsumableItem.SteamPack0}"
                || item.name == $"{Etc.ConsumableItem.SteamPack1}"
                || item.name == $"{Etc.ConsumableItem.MagicStone}"
                || item.name == $"{Etc.ConsumableItem.SoulStone}") {
                    EtcConfirmBtnTxt.text = "확인";
                    //* Actionボタン 購読
                    OnClickConsumPopUpConfirmBtn = () => {
                        HM._.ivm.ConsumePopUp.SetActive(false);
                    };
                }

                //* PresentBox 表示
                else if(item.name == $"{Etc.ConsumableItem.Present0}") {
                    EtcConfirmBtnTxt.text = "열기";
                    OnClickConsumPopUpConfirmBtn = ()
                        => rwlm.RwdItemDt.OpenRewardContent(rwlm.RwdItemDt.Rwd_Present0);
                }
                else if(item.name == $"{Etc.ConsumableItem.Present1}") {
                    EtcConfirmBtnTxt.text = "열기";
                    OnClickConsumPopUpConfirmBtn = ()
                        => rwlm.RwdItemDt.OpenRewardContent(rwlm.RwdItemDt.Rwd_Present1);
                }
                else if(item.name == $"{Etc.ConsumableItem.Present2}") {
                    EtcConfirmBtnTxt.text = "열기";
                    OnClickConsumPopUpConfirmBtn = ()
                        => rwlm.RwdItemDt.OpenRewardContent(rwlm.RwdItemDt.Rwd_Present2);
                }

                //* ChestBox 表示
                else if(item.name == $"{Etc.ConsumableItem.ChestCommon}") {
                    rwlm.SetChestPopUpUI(Etc.ConsumableItem.ChestCommon, quantity);
                    rwlm.OnClickOpenChest = () 
                        => rwlm.RwdItemDt.OpenRewardContent(rwlm.RwdItemDt.Rwd_ChestCommon);
                }
                else if(item.name == $"{Etc.ConsumableItem.ChestDiamond}") {
                    rwlm.SetChestPopUpUI(Etc.ConsumableItem.ChestDiamond, quantity);
                    rwlm.OnClickOpenChest = ()
                        => rwlm.RwdItemDt.OpenRewardContent(rwlm.RwdItemDt.Rwd_ChestDiamond);
                }
                else if(item.name == $"{Etc.ConsumableItem.ChestEquipment}") {
                    rwlm.SetChestPopUpUI(Etc.ConsumableItem.ChestEquipment, quantity);
                    rwlm.OnClickOpenChest = ()
                        => rwlm.RwdItemDt.OpenRewardContent(rwlm.RwdItemDt.Rwd_ChestEquipment);
                }
                else if(item.name == $"{Etc.ConsumableItem.ChestGold}") {
                    rwlm.SetChestPopUpUI(Etc.ConsumableItem.ChestGold, quantity);
                    rwlm.OnClickOpenChest = ()
                        => rwlm.RwdItemDt.OpenRewardContent(rwlm.RwdItemDt.Rwd_ChestGold);
                }
                else if(item.name == $"{Etc.ConsumableItem.ChestPremium}") {
                    rwlm.SetChestPopUpUI(Etc.ConsumableItem.ChestPremium, quantity);
                    rwlm.OnClickOpenChest = ()
                        => rwlm.RwdItemDt.OpenRewardContent(rwlm.RwdItemDt.Rwd_ChestPremium);
                }

                EtcItemBg.gameObject.SetActive(true);
                EtcItemBg.sprite = item.ItemImg;
                EtcNameTxt.text = item.Name;
                EtcQuantityTxt.text = $"{quantity}";
                EtcDescription.text = item.Description;
                LvTxt.text = "";
            }
            //* 装置 アイテム
            else {
                ItemImg.gameObject.SetActive(true);
                //* スタイル
                TopBg.color = HM._.ivm.TopBgClrs[(int)item.Grade];
                GlowBg.color = HM._.ivm.GlowBgClrs[(int)item.Grade];
                ItemBg.sprite = HM._.ivm.GradeBgSprs[(int)item.Grade];
                ItemImg.sprite = item.ItemImg;
                TypeImg.sprite = HM._.ivm.TypeSprs[(int)item.Type];
                TypeTxt.text = Enum.GetItemTypeName(item.Type);

                NameTxt.text = item.Name;

                bool isRelic = item.Type == Enum.ItemType.Relic;
                int max = isRelic? Config.RELIC_UPGRADE_MAX : Config.EQUIP_UPGRADE_MAX;
                bool isLvMax = lv >= max;

                LvTxt.text = $"Lv.{(isLvMax? "MAX" : lv)}";
                QuantityTxt.text = quantity.ToString();
                StarTxt.text = Util.DrawEquipItemStarTxt(lv);
                GradeTxt.text = Enum.GetGradeName(item.Grade);
                GradeTxt.color = HM._.ivm.GradeClrs[(int)item.Grade];

                //* 能力木テスト 表示
                string resMsg = "";
                
                //* 遺物能力（ItemSOのAbilityDataを等級によるValとUpgradeValとして使う）
                if(isRelic) {
                    for(int i = 0; i < relicAbilities.Length; i++) {
                        AbilityType type = relicAbilities[i];
                        string msg = Config.ABILITY_TYPE_DECS_MSGS[(int)type];
                        AbilityData relicAbility = Array.Find(item.Abilities, val => val.Type == type);
                        float val = relicAbility.Val + (lvIdx * relicAbility.UpgradeVal);
                        //* V{N} → 能力数値変換(実際のアイテムデータ)
                        bool isIntType = (type == AbilityType.StartCoin || type == AbilityType.StartLife || type == AbilityType.SkillPoint || type == AbilityType.BonusCoinBy10Kill);
                        string abilityMsg = msg.Replace($"V", $"{val * (isIntType? 1 : 100)}");
                        //* 強化数値表示 トーグル(登録したアップグレードデータ)
                        string upgradeMsg = (relicAbility.UpgradeVal == 0)? "<color=grey>( 고정 )</color>" : $"<color=green>( {$"+{relicAbility.UpgradeVal * (isIntType? 1 : 100)}%"} )</color>";
                        string upgradeToogleMsg = IsUpgradeValToogle? upgradeMsg : "";
                        resMsg += $"{abilityMsg} {upgradeToogleMsg}\n";
                    }
                }
                //* 装置能力（ItemSOのAbilityDataを自体をそのまま使う）
                else {
                    for(int i = 0; i < item.Abilities.Length; i++) {
                        var ability = item.Abilities[i];
                        string msg = Config.ABILITY_TYPE_DECS_MSGS[(int)ability.Type];
                        float val = ability.Val + (lvIdx * ability.UpgradeVal);
                        //* V{N} → 能力数値変換(実際のアイテムデータ)
                        string abilityMsg = msg.Replace($"V", $"{val * 100}");
                        //* 強化数値表示 トーグル(登録したアップグレードデータ)
                        string upgradeMsg = (ability.UpgradeVal == 0)? "<color=grey>( 고정 )</color>" : $"<color=green>( {$"+{ability.UpgradeVal * 100}%"} )</color>";
                        string upgradeToogleMsg = IsUpgradeValToogle? upgradeMsg : "";
                        resMsg += $"{abilityMsg} {upgradeToogleMsg}\n";
                    }
                }

                Description.text = resMsg;

                //* アップグレードボタン UI
                int[] rPrices = Config.H_PRICE.RELIC_UPG.PRICES;
                int[] ePrices = Config.H_PRICE.EQUIP_UPG.PRICES;
                int[] rPers = Config.H_PRICE.RELIC_UPG.PERS;
                int[] ePers = Config.H_PRICE.EQUIP_UPG.PERS;

                Debug.Log($"ePrices.Length= {ePrices.Length}");
                Debug.Log($"lvIdx= {lvIdx}");
                UpgradePriceTxt.text = $"강화\n{(isLvMax? "MAX" : $"<sprite name=Coin>{(isRelic? rPrices[lvIdx] : ePrices[lvIdx])}")}";
                UpgradeSuccessPerTxt.text = isLvMax? "" : $"성공확률 {(isRelic? rPers[lvIdx] : ePers[lvIdx])}%";
            }
        }
    #endregion
    }
}