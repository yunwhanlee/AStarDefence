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
        const int BLUE_BTN = 0, RED_BTN = 1;
        [field:SerializeField] private Sprite[] BtnBgSprs {get; set;}
        [field:SerializeField] private bool IsUpgradeValToogle {get; set;}

        [field:Header("CONSUMABLE POPUP")]
        private Action OnClickConfirmBtn = () => {};
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
        [field:SerializeField] private Image EquipBtnBg {get; set;}
        [field:SerializeField] private TMP_Text TypeTxt {get; set;}
        [field:SerializeField] private TMP_Text NameTxt {get; set;}
        [field:SerializeField] private TMP_Text GradeTxt {get; set;}
        [field:SerializeField] private TMP_Text QuantityTxt {get; set;}
        [field:SerializeField] private TMP_Text LvTxt {get; set;}
        [field:SerializeField] private TMP_Text StarTxt {get; set;}
        [field:SerializeField] private TMP_Text Description {get; set;}
        [field:SerializeField] private TMP_Text EquipBtnTxt {get; set;}
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
            SM._.SfxPlay(SM.SFX.ClickSFX);
            OnClickConfirmBtn?.Invoke();
        }

        public void OnClickEquipBtn() {
            Debug.Log($"OnClickEquipBtn():: CurInvItem= {HM._.ivm.CurInvItem}, IsEquip= {HM._.ivm.CurInvItem.IsEquip}");
            HM._.ivm.EquipPopUp.SetActive(false);

            if(HM._.ivm.CurInvItem.IsEquip)
                HM._.ivCtrl.UnEquipSlotUI();
            else
                HM._.ivCtrl.EquipItemSlotUI();

            HM._.ivEqu.UpdateAllEquipAbilityData();
        }

        public void OnClickUpgradeBtn() {
            Enum.ItemType type = ivm.CurInvItem.Data.Type;
            int lv = ivm.CurInvItem.Lv;
            if(type == Enum.ItemType.Etc) return;
            if(type == Enum.ItemType.Relic && lv == Config.RELIC_UPGRADE_MAX) {
                HM._.hui.ShowMsgError("업그레이드 최대치로 더 이상 할 수 없습니다.");
                return;
            }
            if(type != Enum.ItemType.Relic && lv == Config.EQUIP_UPGRADE_MAX) {
                HM._.hui.ShowMsgError("업그레이드 최대치로 더 이상 할 수 없습니다.");
                return;
            }
            HM._.ivCtrl.InventoryData.UpgradeEquipItem (
                ivm.CurInvItem.Data,
                ivm.CurInvItem.Quantity,
                ++ivm.CurInvItem.Lv,
                ivm.CurInvItem.RelicAbilities,
                ivm.CurInvItem.IsEquip
            );

            //* Equipスロット最新化
            SM._.SfxPlay(SM.SFX.UpgradeSFX);
            HM._.ivEqu.EquipItem(type, HM._.ivEqu.FindEquipItem(type), isEffect: false);
            HM._.ivEqu.UpdateAllEquipAbilityData();
        }
        public void OnClickUpgradeValueNoticeToggle() {
            Debug.Log($"OnClickUpgradeValueNoticeToggle():: IsUpgradeValToogleActive= {IsUpgradeValToogle}");
            SM._.SfxPlay(SM.SFX.ClickSFX);
            IsUpgradeValToogle = !IsUpgradeValToogle;
            UpgValToogleHandleTf.anchoredPosition = new Vector2(IsUpgradeValToogle? 50 : -50, UpgValToogleHandleTf.anchoredPosition.y);
            UpgValToogleHandleTxt.text = IsUpgradeValToogle? "ON" : "OFF";
            SetDescription(
                ivm.CurInvItem.Data, 
                ivm.CurInvItem.Quantity, 
                ivm.CurInvItem.Lv, 
                ivm.CurInvItem.RelicAbilities,
                ivm.CurInvItem.IsEquip
            );
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
            if(isActive)
                SM._.SfxPlay(SM.SFX.ItemPickSFX);
            icon.SetActive(isActive);
            EtcConfirmBtnTxt.text = isActive? "활성화" : "비활성화";
            EtcConfirmBtnTxt.color = isActive? ActiveColor: Color.gray;
        }

        private void SetConsumePopUpUI(string btnTxt, Etc.ConsumableItem type) {
            RewardItemSO rwdItemDt = HM._.rwlm.RwdItemDt;
            EtcConfirmBtnTxt.text = btnTxt;
            //* Actionボタン 購読
            OnClickConfirmBtn = () => {
                switch(type) {
                    case Etc.ConsumableItem.Present0:
                        rwdItemDt.OpenRewardContent(rwdItemDt.Rwd_Present0);
                        break;
                    case Etc.ConsumableItem.Present1:
                        rwdItemDt.OpenRewardContent(rwdItemDt.Rwd_Present1);
                        break;
                    case Etc.ConsumableItem.Present2:
                        rwdItemDt.OpenRewardContent(rwdItemDt.Rwd_Present2);
                        break;
                }
            };
        }

        public void SetDescription(ItemSO item, int quantity, int lv, AbilityType[] relicAbilities, bool isEquip) {
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
                    OnClickConfirmBtn = () => {
                        db.IsCloverActive = !db.IsCloverActive;
                        ActiveCloverItem(hui.CloverActiveIcon, db.IsCloverActive, Color.green);
                    };
                }
                else if(item.name == $"{Etc.ConsumableItem.GoldClover}") {
                    ActiveCloverItem(hui.GoldCloverActiveIcon, db.IsGoldCloverActive, Color.yellow);
                    //* Actionボタン 購読
                    OnClickConfirmBtn = () => {
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
                || item.name == $"{Etc.ConsumableItem.SoulStone}")
                {
                    EtcConfirmBtnTxt.text = "확인";
                    //* Actionボタン 購読
                    OnClickConfirmBtn = () => HM._.ivm.ConsumePopUp.SetActive(false);
                }

                //* PresentBox 表示
                else if(item.name == $"{Etc.ConsumableItem.Present0}")
                    SetConsumePopUpUI("열기", Etc.ConsumableItem.Present0);
                else if(item.name == $"{Etc.ConsumableItem.Present1}")
                    SetConsumePopUpUI("열기", Etc.ConsumableItem.Present1);
                else if(item.name == $"{Etc.ConsumableItem.Present2}")
                    SetConsumePopUpUI("열기", Etc.ConsumableItem.Present2);

                //* ChestBox 表示
                else if(item.name == $"{Etc.ConsumableItem.ChestCommon}")
                    rwlm.ShowChestPopUp(Etc.ConsumableItem.ChestCommon, quantity);
                else if(item.name == $"{Etc.ConsumableItem.ChestDiamond}")
                    rwlm.ShowChestPopUp(Etc.ConsumableItem.ChestDiamond, quantity);
                else if(item.name == $"{Etc.ConsumableItem.ChestEquipment}")
                    rwlm.ShowChestPopUp(Etc.ConsumableItem.ChestEquipment, quantity);
                else if(item.name == $"{Etc.ConsumableItem.ChestGold}")
                    rwlm.ShowChestPopUp(Etc.ConsumableItem.ChestGold, quantity);
                else if(item.name == $"{Etc.ConsumableItem.ChestPremium}")
                    rwlm.ShowChestPopUp(Etc.ConsumableItem.ChestPremium, quantity);

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


                //* 能力メッセージ 初期化
                string resMsg = "";
                
                //* Relic 能力) 
                // 1. ItemSO.Abilities.RelicAbilitiesへデータがあったら、Relicという意味で
                // 2. RelicSO.Abilitiesへ全て能力データが宣言されている。
                // 3. ここから、ValとUpgradeValを取って使う。
                if(isRelic) {
                    for(int i = 0; i < relicAbilities.Length; i++) {
                        AbilityType rType = relicAbilities[i];
                        //* データ取る
                        string msg = Config.ABILITY_DECS[(int)rType];
                        AbilityData[] relicAllAbilityDatas = item.Abilities;
                        AbilityData relicAbility = Array.Find(relicAllAbilityDatas, rAbility => rAbility.Type == rType);
                        Debug.Log("rType=" + rType + ", relicAbility= " + relicAbility);
                        float val = relicAbility.Val + (lvIdx * relicAbility.UpgradeVal);
                        //* V{N} → 能力数値変換(実際のアイテムデータ)
                        bool isIntType = (rType == AbilityType.StartMoney || rType == AbilityType.StartLife || rType == AbilityType.BonusCoinBy10Kill); //rType == AbilityType.SkillPoint);
                        string abilityMsg = msg.Replace($"V", $"{val * (isIntType? 1 : 100)}");
                        //* 強化数値表示 トーグル(登録したアップグレードデータ)
                        string upgradeMsg = (relicAbility.UpgradeVal == 0)? "<color=grey>( 고정 )</color>" : $"<color=green>( {$"+{relicAbility.UpgradeVal * (isIntType? 1 : 100)}%"} )</color>";
                        string upgradeToogleMsg = IsUpgradeValToogle? upgradeMsg : "";
                        resMsg += $"{abilityMsg} {upgradeToogleMsg}\n";
                    }
                }
                //* Equip 能力
                else {
                    for(int i = 0; i < item.Abilities.Length; i++) {
                        var ability = item.Abilities[i];
                        string msg = Config.ABILITY_DECS[(int)ability.Type];
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

                Debug.Log($"ePrices.Length= {ePrices.Length}, lvIdx= {lvIdx}");
                EquipBtnTxt.text = isEquip? "해제" : "장비";
                EquipBtnBg.sprite = BtnBgSprs[isEquip? RED_BTN : BLUE_BTN];
                UpgradePriceTxt.text = $"강화\n{(isLvMax? "MAX" : $"<sprite name=Coin>{(isRelic? rPrices[lvIdx] : ePrices[lvIdx])}")}";
                UpgradeSuccessPerTxt.text = isLvMax? "" : $"성공확률 {(isRelic? rPers[lvIdx] : ePers[lvIdx])}%";
            }
        }
    #endregion
    }
}