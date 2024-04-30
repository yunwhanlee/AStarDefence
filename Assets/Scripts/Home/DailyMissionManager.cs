using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum MissionType {
    COLLECT_COIN, COLLECT_DIAMOND,
    KILL_MONSTER, KILL_BOSS,
    CLEAR_GOBLINDUNGYEN, CLEAR_STAGE, CLEAR_MINING,
    WATCH_ADS, OPEN_ANYCHEST
}

/// <summary>
/// 一日ミッション項目UI
/// </summary>
[Serializable]
public class MissionUI {
    [field: SerializeField] public string Name {get; private set;}
    [field: SerializeField] public MissionType Type {get; private set;}
    [field: SerializeField] public GameObject Obj {get; private set;}
    [field: SerializeField] public GameObject AcceptableBg {get; private set;}
    [field: SerializeField] public Slider Slider {get; private set;}
    [field: SerializeField] public TMP_Text SliderTxt {get; private set;}
    [field: SerializeField] public Button AcceptBtn {get; private set;}
    [field: SerializeField] public TMP_Text AcceptBtnTxt {get; private set;}
    [field: SerializeField] public GameObject Dim {get; private set;}

    public void InitElements() {
        Name = Obj.name;
        AcceptableBg = Obj.transform.GetChild(0).gameObject;
        Slider = Obj.GetComponentInChildren<Slider>();
        SliderTxt = Slider.GetComponentInChildren<TMP_Text>();
        AcceptBtn = Obj.GetComponentInChildren<Button>();
        AcceptBtnTxt = AcceptBtn.GetComponentInChildren<TMP_Text>();
        Dim = Obj.transform.GetChild(Obj.transform.childCount - 1).gameObject;

        AcceptBtn.interactable = false;
        AcceptBtnTxt.color = Color.grey;
    }

    public void UpdateUI() {
        DailyMissionDB dmDB = DM._.DB.DailyMissionDB;
        switch(Type) {
            case MissionType.COLLECT_COIN:
                SetSlider(dmDB.CollectCoinVal, DailyMissionDB.CollectCoinMax, dmDB.IsAcceptCollectCoin);
                break;
            case MissionType.COLLECT_DIAMOND:
                SetSlider(dmDB.CollectDiamondVal, DailyMissionDB.CollectDiamondMax, dmDB.IsAcceptCollectDiamond);
                break;
            case MissionType.KILL_MONSTER:
                SetSlider(dmDB.MonsterKillVal, DailyMissionDB.MonsterKill, dmDB.IsAcceptMonsterKill);
                break;
            case MissionType.KILL_BOSS:
                SetSlider(dmDB.BossKillVal, DailyMissionDB.BossKill, dmDB.IsAcceptBossKill);
                break;
            case MissionType.CLEAR_GOBLINDUNGYEN:
                SetSlider(dmDB.ClearGoblinDungyenVal, DailyMissionDB.ClearGoblinDungyen, dmDB.IsAcceptClearGoblinDungyen);
                break;
            case MissionType.CLEAR_STAGE:
                SetSlider(dmDB.ClearStageVal, DailyMissionDB.ClearStage, dmDB.IsAcceptClearStage);
                break;
            case MissionType.CLEAR_MINING:
                SetSlider(dmDB.ClearMiningVal, DailyMissionDB.ClearMining, dmDB.IsAcceptClearMining);
                break;
            case MissionType.WATCH_ADS:
                SetSlider(dmDB.WatchAdsVal, DailyMissionDB.WatchAds, dmDB.IsAcceptWatchAds);
                break;
            case MissionType.OPEN_ANYCHEST:
                SetSlider(dmDB.OpenAnyChestVal, DailyMissionDB.OpenAnyChest, dmDB.IsAcceptOpenAnyChest);
                break;
        }
    }

    public void AcceptReward() {
        //* UI
        Dim.SetActive(true);
        AcceptableBg.SetActive(false);

        //* リワード
        var rewardList = new List<RewardItem>();
        var rwDt = HM._.rwlm.RwdItemDt;
        switch(Type) {
            case MissionType.COLLECT_COIN:
                DM._.DB.DailyMissionDB.IsAcceptCollectCoin = true;
                rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Diamond], 10));
                break;
            case MissionType.COLLECT_DIAMOND:
                DM._.DB.DailyMissionDB.IsAcceptCollectDiamond = true;
                rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Coin], 1000));
                break;
            case MissionType.KILL_MONSTER:
                DM._.DB.DailyMissionDB.IsAcceptMonsterKill = true;
                rewardList.Add(new (rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.Present0]));
                break;
            case MissionType.KILL_BOSS:
                DM._.DB.DailyMissionDB.IsAcceptBossKill = true;
                rewardList.Add(new (rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.Present1]));
                break;
            case MissionType.CLEAR_GOBLINDUNGYEN:
                DM._.DB.DailyMissionDB.IsAcceptClearGoblinDungyen = true;
                rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Coin], 750));
                break;
            case MissionType.CLEAR_STAGE:
                DM._.DB.DailyMissionDB.IsAcceptClearStage = true;
                rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.GoldKey], 1));
                break;
            case MissionType.CLEAR_MINING:
                DM._.DB.DailyMissionDB.IsAcceptClearMining = true;
                rewardList.Add(new (rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.ChestCommon]));
                break;
            case MissionType.WATCH_ADS:
                DM._.DB.DailyMissionDB.IsAcceptWatchAds = true;
                rewardList.Add(new (rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.ChestEquipment]));
                break;
            case MissionType.OPEN_ANYCHEST:
                DM._.DB.DailyMissionDB.IsAcceptOpenAnyChest = true;
                rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Diamond], 15));
                break;
        }

        HM._.rwlm.ShowReward(rewardList);
        HM._.rwm.UpdateInventory(rewardList);

        HM._.dailyMs.UpdateCompleteCntUI();
    }

    public void SetSlider(int val, int max, bool isAccepted) {
        Slider.value = (float)val / max;
        SliderTxt.text = $"{val} / {max}";

        //* 達成したが、まだ受け取っていない
        if(Slider.value == 1 && !isAccepted) {
            AcceptableBg.SetActive(true);
            AcceptBtn.interactable = true;
            AcceptBtnTxt.color = Color.white;
            Obj.transform.SetAsFirstSibling(); //* 前へ整列
        }

        //* 受け取ったら、目録を後ろへ移す
        if(isAccepted)
            Obj.transform.SetAsLastSibling(); //* 後ろへ整列

        //* 受け取ったかチェック
        Dim.SetActive(isAccepted);
    }
}

/// <summary>
/// 一日ミッションマネージャー
/// </summary>
public class DailyMissionManager : MonoBehaviour {
    public const int CompleteCntMax = 5;
    public Action OnUpdateUI = () => {};
    public Color YellowClr;
    public Color SkyBlueClr;

    [field: SerializeField] public int CompleteCnt;
    //* Home Top
    [field: SerializeField] public GameObject HomeAllClearNoticeBtnObj {get; set;}
    [field: SerializeField] public Slider HomeAllClearNoticeSlider {get; set;}
    [field: SerializeField] public TMP_Text HomeAllClearNoticeSliderTxt {get; set;}
    [field: SerializeField] public GameObject HomeAllClearNoticeAlertDot {get; set;}
    [field: SerializeField] public GameObject HomeAllClearNoticeCheckIcon {get; set;}
    //*  Home Bottom
    [field: SerializeField] public GameObject MenuIconRedDotAlert {get; set;}

    //* PopUp
    [field: SerializeField] public GameObject WindowObj {get; set;}
    [field: SerializeField] public GameObject AllClearSpiceialRewardCheckIcon {get; set;}
    [field: SerializeField] public Slider CompleteCntSlider {get; set;}
    [field: SerializeField] public TMP_Text TopCompleteCntTxt {get; set;}
    [field: SerializeField] public TMP_Text CompleteCntTxt {get; set;}
    [field: SerializeField] public Transform ContentTf {get; set;}
    [field: SerializeField] public MissionUI[] MissionUIs {get; set;}

    void Start() {
        Array.ForEach(MissionUIs, missionUI => missionUI.InitElements());

        //* Actionイベント購読
        OnUpdateUI = () => {
            Array.ForEach(MissionUIs, missionUI => missionUI.UpdateUI());
            UpdateMenuIconRedDotAlert();
            UpdateCompleteCntUI();
        };
        OnUpdateUI.Invoke(); //* コールバック
    }

    #region EVENT
        public void OnClickMissionIconAtHome() {
            HM._.hui.IsActivePopUp = true;
            SM._.SfxPlay(SM.SFX.ClickSFX);
            WindowObj.SetActive(true);
            OnUpdateUI.Invoke();
            // UpdateMenuIconRedDotAlert();
            // Array.ForEach(MissionUIs, missionUI => missionUI.UpdateUI());
        }
        public void OnClickCloseBtn() {
            HM._.hui.IsActivePopUp = false;
            SM._.SfxPlay(SM.SFX.ClickSFX);
            WindowObj.SetActive(false);
            OnUpdateUI.Invoke();
            // UpdateMenuIconRedDotAlert();
        }
        /// <summary>
        /// リワード受け取る
        /// </summary>
        /// <param name="typeIdx">リワードタイプ：MissionType</param>
        public void OnClickAcceptRewardBtn(int typeIdx) {
            MissionUIs[typeIdx].AcceptReward();
        }
        /// <summary>
        /// 5個の一日ミッションを完了すると、スペシャルリワード受け取る
        /// </summary>
        public void OnClickAllClearSpecialRewardBtn() {
            if(CompleteCnt < CompleteCntMax) {
                HM._.hui.ShowMsgError("일일미션 5개를 완료해야됩니다.");
                return;
            }
            else if(DM._.DB.DailyMissionDB.IsAcceptAllClearSpecialReward) {
                HM._.hui.ShowMsgError("이미 보상을 얻었습니다.");
                return;
            }

            //* Accept Reward
            DM._.DB.DailyMissionDB.IsAcceptAllClearSpecialReward = true;
            HomeAllClearNoticeCheckIcon.SetActive(true);
            AllClearSpiceialRewardCheckIcon.SetActive(true);

            var rewardList = new List<RewardItem>() {
                new (HM._.rwlm.RwdItemDt.EtcConsumableDatas[(int)Etc.ConsumableItem.ChestPremium])
            };

            HM._.rwlm.ShowReward(rewardList);
            HM._.rwm.UpdateInventory(rewardList);
        }
    #endregion

    #region FUNC
        public void UpdateMenuIconRedDotAlert() {
            bool isAcceptableReward = false;
            Array.ForEach(MissionUIs, missionUI => {
                Debug.Log($"UpdateMenuIconRedDotAlert():: {missionUI.Name}: AcceptableBg= {missionUI.AcceptableBg.activeSelf}");
                if(missionUI.AcceptableBg.activeSelf)
                    isAcceptableReward = true;
                return;
            });
            
            MenuIconRedDotAlert.SetActive(isAcceptableReward);
        }

        public void UpdateCompleteCntUI() {
            bool isAccepted = DM._.DB.DailyMissionDB.IsAcceptAllClearSpecialReward;

            CompleteCnt = 0;
            Array.ForEach(MissionUIs, missionUI => {
                if(missionUI.Dim.activeSelf)
                    CompleteCnt++;
            });
            bool isPossibleAccept = CompleteCnt >= CompleteCntMax;

            //* Home Notice
            HomeAllClearNoticeCheckIcon.SetActive(isAccepted);
            AllClearSpiceialRewardCheckIcon.SetActive(isAccepted);
            HomeAllClearNoticeAlertDot.SetActive(!isAccepted && isPossibleAccept);
            HomeAllClearNoticeSlider.value = (float)CompleteCnt / CompleteCntMax;
            HomeAllClearNoticeSliderTxt.text = $"{CompleteCnt} / {CompleteCntMax}";

            //* Top Nav
            AllClearSpiceialRewardCheckIcon.SetActive(isAccepted);
            TopCompleteCntTxt.text = $"{CompleteCnt} / {CompleteCntMax}";
            //* Slider UI
            CompleteCntSlider.value = (float)CompleteCnt / CompleteCntMax;
            CompleteCntTxt.text = $"{CompleteCnt} / {CompleteCntMax}";
        }
    #endregion
}
