using System;
using System.Collections;
using System.Collections.Generic;
using Inventory.Model;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour {
    [field: SerializeField] public GameObject WindowObj {get; private set;}
    [field: SerializeField] public TMP_Text VersionTxt {get; private set;}
    [field: SerializeField] public GameObject LanguageWindowObj {get; private set;}
    [field: SerializeField] public GameObject ShowTutorialWindowObj {get; private set;}
    [field: SerializeField] public Slider BgmVolumeSlider {get; private set;}
    [field: SerializeField] public Slider SfxVolumeSlider {get; private set;}

    [field: SerializeField] public GameObject CouponWindowObj {get; private set;}
    [field: SerializeField] public TMP_InputField CouponInputField {get; private set;}

    void Start() {
        //* バージョン
        VersionTxt.text = $"Ver{Version._.Major}.{Version._.Minor}.{Version._.Revision}";
        //* ボリュームUI
        BgmVolumeSlider.value = DM._.DB.SettingDB.BgmVolume;
        SfxVolumeSlider.value = DM._.DB.SettingDB.SfxVolume;
        //* ボリュームデータ
        SM._.SetVolumeBGM(DM._.DB.SettingDB.BgmVolume);
        SM._.SetVolumeSFX(DM._.DB.SettingDB.SfxVolume);
    }

#region EVENT
    public void OnClickSettingIconBtn() {
        HM._.hui.IsActivePopUp = true;
        SM._.SfxPlay(SM.SFX.ClickSFX);
        WindowObj.SetActive(true);
    }
    public void OnClickCloseBtn() {
        HM._.hui.IsActivePopUp = false;
        SM._.SfxPlay(SM.SFX.ClickSFX);
        WindowObj.SetActive(false);
    }
    public void OnChangeBgmSliderHandle() {
        DM._.DB.SettingDB.BgmVolume = BgmVolumeSlider.value;
        SM._.SetVolumeBGM(DM._.DB.SettingDB.BgmVolume);
    }
    public void OnChangeSfxSliderHandle() {
        DM._.DB.SettingDB.SfxVolume = SfxVolumeSlider.value;
        SM._.SetVolumeSFX(DM._.DB.SettingDB.SfxVolume);
    }
    public void OnClickGoogleLoginBtn() {

    }
#region SETTING LANGUAGE
    public void OnClickLanguageBtn() {
        SM._.SfxPlay(SM.SFX.ClickSFX);
        LanguageWindowObj.SetActive(true);
    }
    public void OnClickLanguageWindowCloseBtn() {
        SM._.SfxPlay(SM.SFX.ClickSFX);
        LanguageWindowObj.SetActive(false);
    }
    //TODO CHANGE LANGUAGE
#endregion
#region SHOW TUTORIAL
    public void OnClickReplayTutorialBtn() {
        SM._.SfxPlay(SM.SFX.ClickSFX);
        ShowTutorialWindowObj.SetActive(true);
    }
    public void OnClickTutorialWindowCloseBtn() {
        SM._.SfxPlay(SM.SFX.ClickSFX);
        ShowTutorialWindowObj.SetActive(false);
    }
    public void OnClickTutorialPlayBtn(int tutoIdx) {
        SM._.SfxPlay(SM.SFX.ClickSFX);
        TutoM._.ShowTutoPopUp(tutoIdx, pageIdx: 0);
    }
#endregion
    public void OnClickResetBtn() {
        HM._.hui.ShowAgainAskMsg("정말로 데이터를 리셋하시겠습니까?\n<size=80%><color=red>(주의!)모든 데이터가 사라집니다.</color></size>");
        HM._.hui.OnClickAskConfirmAction = () => {
            SM._.SfxPlay(SM.SFX.CompleteSFX);
            DM._.Reset();
            SceneManager.LoadScene($"{Enum.Scene.Home}");
        };
    }
    public void OnClickAppClose() {
        HM._.hui.ShowAgainAskMsg("게임을 종료하시겠습니까?");
        HM._.hui.OnClickAskConfirmAction = () => {
            #if UNITY_EDITOR
                EditorApplication.isPlaying = false;
                DM._.DB.LastDateTicks = DateTime.UtcNow.Ticks; //* 終了した日にち時間データをTicks(longタイプ)で保存
                DM._.Save();
            #else
                Application.Quit();
                DM._.DB.LastDateTicks = DateTime.UtcNow.Ticks; //* 終了した日にち時間データをTicks(longタイプ)で保存
                DM._.Save();
            #endif
        };
    }
#region COUPON
    public void OnClickCouponBtn() {
        SM._.SfxPlay(SM.SFX.ClickSFX);
        CouponWindowObj.SetActive(true);
    }

    public void OnClickCouponWindowConfirmBtn() {
        string inputTxt = CouponInputField.text;
        SM._.SfxPlay(SM.SFX.ClickSFX);
        if(inputTxt == DM._.DB.COUPON_A) {
            if(!DM._.DB.IsAccept_CouponA) {
                DM._.DB.IsAccept_CouponA = true;

                WindowObj.SetActive(false);
                CouponWindowObj.SetActive(false);

                //* リワード
                var rewardList = new List<RewardItem> {
                    new (HM._.rwlm.RwdItemDt.EtcConsumableDatas[(int)Etc.ConsumableItem.Clover], 50),
                    new (HM._.rwlm.RwdItemDt.EtcConsumableDatas[(int)Etc.ConsumableItem.GoldClover], 50),
                    new (HM._.rwlm.RwdItemDt.EtcConsumableDatas[(int)Etc.ConsumableItem.MagicStone], 30),
                    new (HM._.rwlm.RwdItemDt.EtcConsumableDatas[(int)Etc.ConsumableItem.SoulStone], 30),
                    new (HM._.rwlm.RwdItemDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Coin], 100000),
                    new (HM._.rwlm.RwdItemDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Diamond], 3000),
                    new (HM._.rwlm.RwdItemDt.WeaponDatas[4]),
                    new (HM._.rwlm.RwdItemDt.ShoesDatas[5]),
                    new (HM._.rwlm.RwdItemDt.RingDatas[4]),
                };

                //* Relic リワード
                ItemSO relicDt = HM._.rwlm.RwdItemDt.RelicDatas[2];
                AbilityType[] relicAbilities = HM._.ivCtrl.InventoryData.CheckRelicAbilitiesData(relicDt);
                rewardList.Add(new (relicDt, quantity: 1, relicAbilities));

                HM._.rwlm.ShowReward(rewardList);
            }
            else {
                HM._.hui.ShowMsgError("이미 사용한 쿠폰입니다.");
            }
        }
        else {
            HM._.hui.ShowMsgError("존재하지 않는 쿠폰입니다.");
        }
    }
#endregion
#endregion
}
