using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingManager : MonoBehaviour {
    [field: SerializeField] public GameObject WindowObj {get; private set;}
    [field: SerializeField] public GameObject BgmToogleCheckImg {get; private set;}
    [field: SerializeField] public GameObject SfxToogleCheckImg {get; private set;}

    void Start() {
        BgmToogleCheckImg.SetActive(DM._.DB.SettingDB.IsActiveBgm);
        SfxToogleCheckImg.SetActive(DM._.DB.SettingDB.IsActiveSfx);
    }

#region EVENT
    public void OnClickSettingIconBtn() {
        SM._.SfxPlay(SM.SFX.ClickSFX);
        WindowObj.SetActive(true);
    }
    public void OnClickCloseBtn() {
        SM._.SfxPlay(SM.SFX.ClickSFX);
        WindowObj.SetActive(false);
    }
    public void OnClickBgmToogleBtn() {
        DM._.DB.SettingDB.IsActiveBgm = !DM._.DB.SettingDB.IsActiveBgm;
        BgmToogleCheckImg.SetActive(DM._.DB.SettingDB.IsActiveBgm);
    }
    public void OnClickSfxToogleBtn() {
        DM._.DB.SettingDB.IsActiveSfx = !DM._.DB.SettingDB.IsActiveSfx;
        SfxToogleCheckImg.SetActive(DM._.DB.SettingDB.IsActiveSfx);
    }
    public void OnClickLanguageBtn() {

    }
    public void OnClickGoogleLoginBtn() {

    }
    public void OnClickResetBtn() {
        HM._.hui.ShowAgainAskMsg("정말로 데이터를 리셋하시겠습니까?");
        HM._.hui.OnClickAskConfirmAction = () => {
            SM._.SfxPlay(SM.SFX.CompleteSFX);
            DM._.Reset();
            SceneManager.LoadScene($"{Enum.Scene.Home}");
        };
    }
#endregion
#region FUNC

#endregion
}
