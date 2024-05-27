using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Inventory.Model;
using System;
using AssetKits.ParticleImage;
using System.Linq;
using Random = UnityEngine.Random;

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
        [field:SerializeField] private GameObject MagicStoneBtnObj {get; set;}
        [field:SerializeField] private GameObject SoulStoneBtnObj {get; set;}
        [field:SerializeField] private ParticleImage ItemImgScaleUIEF {get; set;}
        [field:SerializeField] private ParticleImage UpgradeSucessUIEF {get; set;}
        [field:SerializeField] private ParticleImage UpgradeFailUIEF {get; set;}
        [field:SerializeField] private ParticleImage SoulStoneSpawnUIEF {get; set;}
        [field:SerializeField] private ParticleImage MagicStoneSpawnUIEF {get; set;}
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
        [field:SerializeField] private TMP_Text MagicStoneBtnTxt {get; set;}
        [field:SerializeField] private TMP_Text SoulStoneBtnTxt {get; set;}

        InventoryUIManager ivm;

        void Awake() => ResetDescription();

        void Start() {
            ivm = HM._.ivm;
            //* 初期値：アップグレードトーグルキーをONにする
            IsUpgradeValToogle = true;
            UpgValToogleHandleTf.anchoredPosition = new Vector2(IsUpgradeValToogle? 50 : -50, UpgValToogleHandleTf.anchoredPosition.y);
            UpgValToogleHandleTxt.text = IsUpgradeValToogle? "ON" : "OFF";
        }

#region EVENT
        public void OnClickConsumableItemConfirm() {
            Debug.Log($"OnClickConsumableItemConfirm():: CurInvItem.Name= {HM._.ivm.CurInvItem.Data.Name}");
            SM._.SfxPlay(SM.SFX.ClickSFX);
            OnClickConfirmBtn?.Invoke();
        }

        public void OnClickEquipBtn() {
            Debug.Log($"OnClickEquipBtn():: CurInvItem= {ivm.CurInvItem}, IsEquip= {ivm.CurInvItem.IsEquip}");
            ivm.EquipPopUp.SetActive(false);
            //* 装置・解除
            if(ivm.CurInvItem.IsEquip)
                HM._.ivCtrl.UnEquipSlotUI();
            else
                HM._.ivCtrl.EquipItemSlotUI();

            HM._.ivEqu.UpdateAllEquipAbilityData();

            //* イベントリーUI アップデート
            HM._.ivCtrl.InventoryData.InformAboutChange();
        }

        private bool TryUpgrade() {
            Enum.ItemType type = ivm.CurInvItem.Data.Type;
            bool isRelic = type == Enum.ItemType.Relic;
            int lvIdx = ivm.CurInvItem.Lv - 1;

            //* Euqipタイプによって、アップグレードと成功％データリスト
            int upgradeMax = isRelic? Config.RELIC_UPGRADE_MAX : Config.EQUIP_UPGRADE_MAX;
            int[] priceDts = isRelic? Config.H_PRICE.RELIC_UPG.GetRelicUpgradePriceArr(ivm.CurInvItem.Data.Grade)
                : Config.H_PRICE.EQUIP_UPG.GetEquipUpgradePriceArr(ivm.CurInvItem.Data.Grade);
            int[] perDts = isRelic? Config.H_PRICE.RELIC_UPG.PERS : Config.H_PRICE.EQUIP_UPG.PERS;

            if(ivm.CurInvItem.Lv == upgradeMax) {
                HM._.hui.ShowMsgError("업그레이드 최대치로 더 이상 할 수 없습니다.");
                return false;
            }
            else if(HM._.Coin < priceDts[lvIdx]) {
                HM._.hui.ShowMsgError("코인이 부족합니다.");
                return false;
            }
            //* アップグレード試す
            int randPer = Random.Range(0, 100);
            HM._.Coin -= priceDts[lvIdx];
            if(randPer < perDts[lvIdx]) {
                HM._.hui.ShowMsgNotice("<color=blue>업그레이드 성공!</color>");
                return true;
            }
            else {
                SM._.SfxPlay(SM.SFX.EnemyDeadSFX);
                UpgradeFailUIEF.Play();
                HM._.ivm.EquipPopUpAnim.SetTrigger("DoShake");
                HM._.hui.ShowMsgNotice($"<color=red>업그레이드 실패."); //.{randPer} / {perDts[lvIdx]}</color>");
                return false;
            }
        }

        public void OnClickUpgradeBtn() {
            Enum.ItemType type = ivm.CurInvItem.Data.Type;

            if(type == Enum.ItemType.Etc)
                return;

            //* アップグレードが成功しなかったら、以下の処理しない
            if(!TryUpgrade())
                return;

            //* アップグレードデータ 最新化
            HM._.ivCtrl.InventoryData.UpgradeEquipItem (
                ivm.CurInvItem.Data,
                ivm.CurInvItem.Quantity,
                ++ivm.CurInvItem.Lv,
                ivm.CurInvItem.RelicAbilities,
                ivm.CurInvItem.IsEquip
            );

            //* Equipスロット最新化
            ItemImgScaleUIEF.sprite = ivm.CurInvItem.Data.ItemImg;
            ItemImgScaleUIEF.Play();
            UpgradeSucessUIEF.Play();
            SM._.SfxPlay(SM.SFX.UpgradeSFX);
            HM._.ivEqu.EquipItem(type, HM._.ivEqu.FindEquipItem(type), isEffect: false);
            HM._.ivEqu.UpdateAllEquipAbilityData();

            //* イベントリーUI アップデート
            HM._.ivCtrl.InventoryData.InformAboutChange();

            //* 情報表示ポップアップUI アップデート
            HM._.ivm.UpdateDescription(ivm.CurItemIdx, ivm.CurInvItem.Data, ivm.CurInvItem.Quantity, ivm.CurInvItem.Lv, ivm.CurInvItem.RelicAbilities, ivm.CurInvItem.IsEquip);
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

        private bool CheckStoneItemQuantity(Etc.ConsumableItem itemEnumIdx, string againAskMsg) {
            InventoryItem stoneItem = HM._.ivCtrl.InventoryData.invList.Find( item
                => !item.IsEmpty && item.Data.name == itemEnumIdx.ToString());
            if(stoneItem.IsEmpty) {
                HM._.hui.ShowMsgError("사용할 아이템이 없습니다.");
                return false;
            }

            //* 本当にしますか？メッセージ
            HM._.hui.ShowAgainAskMsg(againAskMsg);
            return true;
        }

        /// <summary>
        /// ストーンボタンクリック
        /// </summary>
        /// <param name="idx">0: SoulStone, 1: MagicStone</param>
        public void OnClickStoneBtn(int idx) {
            const int SOULSTONE = 0, MAGICSTONE = 1;

            SM._.SfxPlay(SM.SFX.ClickSFX);
            //* パラメーター１
            Etc.ConsumableItem[] stoneItemEnumList = {
                Etc.ConsumableItem.SoulStone, 
                Etc.ConsumableItem.MagicStone, 
            };
            //* パラメーター２
            var relicAbilities = HM._.ivm.CurInvItem.RelicAbilities;
            string potentialStatusWord = "";
            if(idx == SOULSTONE) {
                if(relicAbilities != null)
                    potentialStatusWord = relicAbilities.Length == 0? "개방" : "변경";
            }

            string[] againAskMsgs = {
                $"소울스톤을 사용하여 잠재능력을\n{potentialStatusWord}하시겠습니까?",
                "매직스톤을 사용하여 유물능력을\n변경하시겠습니까?"
            };

            //* ストーンアイテムが有るか 確認
            if(!CheckStoneItemQuantity(stoneItemEnumList[idx], againAskMsgs[idx]))
                return;

            //* AgainAskPopUpクリックボタンイベント登録
            HM._.hui.OnClickAskConfirmAction = () => {
                switch(idx) {
                    case SOULSTONE: {
                        //* インベントリデータから、残る量を減る
                        int invItemIdx = HM._.ivCtrl.InventoryData.invList.FindIndex (itemDt
                            => !itemDt.IsEmpty && itemDt.Data.name == stoneItemEnumList[idx].ToString());
                        HM._.ivCtrl.InventoryData.DecreaseItem(invItemIdx, -1);

                        //* Potential Ability 解放
                        OpenEquipPotentialAbility();
                        break;
                    }
                    case MAGICSTONE: {
                        //* インベントリデータから、残る量を減る
                        int invItemIdx = HM._.ivCtrl.InventoryData.invList.FindIndex (itemDt
                            => !itemDt.IsEmpty && itemDt.Data.name == stoneItemEnumList[idx].ToString());
                        HM._.ivCtrl.InventoryData.DecreaseItem(invItemIdx, -1);

                        //* リセット RELIC 能力
                        ResetCurrentRelicAbilities();
                        break;
                    }
                }
            };
        }

        private void OpenEquipPotentialAbility() {
            int curIdx = HM._.ivm.CurItemIdx;
            SM._.SfxPlay(SM.SFX.InvStoneSFX);
            HM._.hui.ShowMsgNotice("잠재능력 설정 완료!", y: 170);
            SoulStoneSpawnUIEF.Play();

            //* 新しいRelic能力を一つランダム選択
            InventoryItem curInvItem = HM._.ivCtrl.OpenCurrentEquipPotentialAbility();

            //* イベントリーUI アップデート
            HM._.ivCtrl.InventoryData.InformAboutChange();

            //* 情報表示ポップアップUI アップデート
            HM._.ivm.UpdateDescription(curIdx, curInvItem.Data, curInvItem.Quantity, curInvItem.Lv, curInvItem.RelicAbilities, curInvItem.IsEquip);
        }

        private void ResetCurrentRelicAbilities() {
            int curIdx = HM._.ivm.CurItemIdx;
            SM._.SfxPlay(SM.SFX.InvStoneSFX);
            HM._.hui.ShowMsgNotice("능력 재설정 완료!", y: 170);
            MagicStoneSpawnUIEF.Play();

            //* リセット
            InventoryItem curInvItem = HM._.ivCtrl.ResetCurrentRelicAbilities();

            //* イベントリーUI アップデート
            HM._.ivCtrl.InventoryData.InformAboutChange();

            //* 情報表示ポップアップUI アップデート
            HM._.ivm.UpdateDescription(curIdx, curInvItem.Data, curInvItem.Quantity, curInvItem.Lv, curInvItem.RelicAbilities, curInvItem.IsEquip);
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
            MagicStoneBtnTxt.text = "0";
            SoulStoneBtnTxt.text = "0";
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
                        rwdItemDt.OpenRewardContent(isOpenByInv: true, rwdItemDt.Rwd_Present0);
                        break;
                    case Etc.ConsumableItem.Present1:
                        rwdItemDt.OpenRewardContent(isOpenByInv: true, rwdItemDt.Rwd_Present1);
                        break;
                    case Etc.ConsumableItem.Present2:
                        rwdItemDt.OpenRewardContent(isOpenByInv: true, rwdItemDt.Rwd_Present2);
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
                else if(item.name == $"{Etc.ConsumableItem.SteamPack0}"
                || item.name == $"{Etc.ConsumableItem.SteamPack1}"
                || item.name == $"{Etc.ConsumableItem.BizzardScroll}"
                || item.name == $"{Etc.ConsumableItem.LightningScroll}"
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

                        //* 説明メッセージ
                        string msg = Config.ABILITY_DECS[(int)rType];

                        AbilityData[] relicAllAbilityDatas = item.Abilities;
                        AbilityData relicAbility = Array.Find(relicAllAbilityDatas, rAbility => rAbility.Type == rType);
                        Debug.Log("<color=red>rType=" + rType + ", relicAbility= " + relicAbility + "</color>");

                        float val = Util.RoundDecimal(relicAbility.Val + (lvIdx * relicAbility.UpgradeVal));

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
                    //* 基本アイテムの能力
                    for(int i = 0; i < item.Abilities.Length; i++) {
                        AbilityData ability = item.Abilities[i];
                        string msg = Config.ABILITY_DECS[(int)ability.Type];
                        float val = Util.RoundDecimal(ability.Val + (lvIdx * ability.UpgradeVal));
                        //* V{N} → 能力数値変換(実際のアイテムデータ)
                        string abilityMsg = msg.Replace($"V", $"{val * 100}");
                        //* 強化数値表示 トーグル(登録したアップグレードデータ)
                        string upgradeMsg = (ability.UpgradeVal == 0)? "<color=grey>( 고정 )</color>" : $"<color=green>( {$"+{ability.UpgradeVal * 100}%"} )</color>";
                        string upgradeToogleMsg = IsUpgradeValToogle? upgradeMsg : "";
                        resMsg += $"{abilityMsg} {upgradeToogleMsg}\n";
                    }

                    //* Potential Ability有れば、表示
                    if(relicAbilities != null && relicAbilities.Length == 1) {
                        AbilityType rType = relicAbilities[0];

                        //* 説明メッセージ
                        string msg = Config.ABILITY_DECS[(int)rType];

                        //* 等級 判断
                        ItemSO[] relicDts = HM._.rwlm.RwdItemDt.RelicDatas;
                        AbilityData[] relicAllAbilityDatas = Enum.GetRelicAllDts(item.Grade);

                        AbilityData relicAbility = Array.Find(relicAllAbilityDatas, rAbility => rAbility.Type == rType);
                        Debug.Log("relicAllAbilityDatas.Length= " + relicAllAbilityDatas.Length);
                        Debug.Log("Grade=" + item.Grade + ", rType=" + rType + ", relicAbility= " + relicAbility);

                        float val = Util.RoundDecimal(relicAbility.Val + (lvIdx * relicAbility.UpgradeVal));

                        //* V{N} → 能力数値変換(実際のアイテムデータ)
                        bool isIntType = (rType == AbilityType.StartMoney || rType == AbilityType.StartLife || rType == AbilityType.BonusCoinBy10Kill); //rType == AbilityType.SkillPoint);
                        string abilityMsg = msg.Replace($"V", $"{val * (isIntType? 1 : 100)}");

                        //* 強化数値表示 トーグル(登録したアップグレードデータ)
                        string upgradeMsg = (relicAbility.UpgradeVal == 0)? "<color=grey>( 고정 )</color>" : $"<color=green>( {$"+{relicAbility.UpgradeVal * (isIntType? 1 : 100)}%"} )</color>";
                        string upgradeToogleMsg = IsUpgradeValToogle? upgradeMsg : "";
                        resMsg += $"<color=red>{abilityMsg}</color> {upgradeToogleMsg}\n";
                    }
                }

                Description.text = resMsg;

                //* アップグレードボタン UI
                
                int[] rPrices = Config.H_PRICE.RELIC_UPG.GetRelicUpgradePriceArr(item.Grade);
                int[] ePrices = Config.H_PRICE.EQUIP_UPG.GetEquipUpgradePriceArr(item.Grade);
                int[] rPers = Config.H_PRICE.RELIC_UPG.PERS;
                int[] ePers = Config.H_PRICE.EQUIP_UPG.PERS;

                Debug.Log($"ePrices.Length= {ePrices.Length}, lvIdx= {lvIdx}");
                MagicStoneBtnObj.SetActive(isRelic);
                SoulStoneBtnObj.SetActive(!isRelic);
                var invItemList = HM._.ivCtrl.InventoryData.invList;
                MagicStoneBtnTxt.text = $"{invItemList.Find(item => !item.IsEmpty && item.Data.name == $"{Etc.ConsumableItem.MagicStone}").Quantity}";
                SoulStoneBtnTxt.text = $"{invItemList.Find(item => !item.IsEmpty && item.Data.name == $"{Etc.ConsumableItem.SoulStone}").Quantity}";
                EquipBtnTxt.text = isEquip? "해제" : "장비";
                EquipBtnBg.sprite = BtnBgSprs[isEquip? RED_BTN : BLUE_BTN];
                UpgradePriceTxt.text = $"강화\n{(isLvMax? "MAX" : $"<sprite name=Coin>{(isRelic? rPrices[lvIdx] : ePrices[lvIdx])}")}";
                UpgradeSuccessPerTxt.text = isLvMax? "" : $"성공확률 {(isRelic? rPers[lvIdx] : ePers[lvIdx])}%";
            }
        }
    #endregion
    }
}