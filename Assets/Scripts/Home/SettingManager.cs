using System.Collections;
using System.Collections.Generic;
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
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        };
    }
#endregion
}
