using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.U2D.Animation;
using Assets.PixelFantasy.PixelHeroes.Common.Scripts.ExampleScripts;

[Serializable]
public class Spot {
    [field: SerializeField] public GameObject PlusIconObj {get; private set;} // 1st Child
    [field: SerializeField] public GameObject DisplayObj {get; private set;} //2nd Child

    public void Show(bool isActive) {
        PlusIconObj.SetActive(!isActive);
        DisplayObj.SetActive(isActive);
    }
}
[Serializable]
public class GoblinSpot : Spot {
    [field: SerializeField] public SpriteLibrary BodySprLib {get; private set;}
}
[Serializable]
public class OreSpot : Spot {
    [field: SerializeField] public Image OreImg {get; private set;}
}

public class WorkSpaceUIManager : MonoBehaviour {
    [field: Header("VALUE")]
    [field: SerializeField] public int CurIdx {get; set;}
    [SerializeField] WorkSpace[] workSpaces; public WorkSpace[] WorkSpaces {
        get => DM._.DB.MiningData.WorkSpaces;
    }
    [SerializeField] public WorkSpace CurWorkSpace {
        get => WorkSpaces[CurIdx];
    }

    [field: Header("HOME UI ELEM")]
    [field: SerializeField] public CharacterControls GoblinChrCtrl {get; set;}
    [field: SerializeField] public ParticleSystem MetalHitEF {get; set;}
    [field: SerializeField] public Transform WorkAreaTf {get; set;}
    [field: SerializeField] public TextMeshProUGUI TitleTxt {get; set;}
    [field: SerializeField] public GoblinSpot GoblinSpot;
    [field: SerializeField] public OreSpot OreSpot;

    void Start() {
        
        //* 読みこんだデータで、退屈のWorkSpaceたちを最新化
        UpdateSpotAndUI();

        for(int i = 0; i < WorkSpaces.Length; i++) {
            var ws = WorkSpaces[i];
            bool isSuccess = ws.CheckSpotActive();
            if(isSuccess)
                HM._.mnm.CorTimerIDs[ws.Id] = StartCoroutine(ws.CoTimerStart(false, isPassedTime: true));
        }
    }

#region EVENT
    public void OnClickPurchaseWorkSpaceBtn() {
        SM._.SfxPlay(SM.SFX.ClickSFX);
        HM._.hui.ShowAgainAskMsg($"<sprite name=Coin>{GetPrice()}을 사용하여\n작업장{HM._.wsm.CurIdx + 1} 구매하시겠습니까?");
        HM._.hui.OnClickAskConfirmAction = () => {
            SM._.SfxPlay(SM.SFX.CompleteSFX);
            CurWorkSpace.IsLock = false;
            //* 最新化
            CurWorkSpace.UpdateUI(HM._.wsm.WorkAreaTf);
        };
    }

    /// <summary> ワークスペース移動 </summary>
    /// <param name="dir">-1：左、1：右</param>
    public void OnClickWorkSpacePageBtn(int dir) {
        SM._.SfxPlay(SM.SFX.ClickSFX);
        HM._.mtm.SliderBtnAnim.SetTrigger("SlideIn");

        //* ゴブリンが有ったら、以前のアニメーションを一旦停止
        if(HM._.wsm.CurWorkSpace.GoblinSpotDt.IsActive)
            HM._.wsm.GoblinChrCtrl.StopGoblinAnim();

        SetCurIdx(dir);
        TitleTxt.text = $"작업장 {CurIdx + 1}";


        UpdateSpotAndUI();
    }

    /// <summary> ホームの場所⊕ボタン </summary>
    public void OnClickGoblinLeftSpotBtn() {
        if(CurWorkSpace.IsFinishWork) {
            HM._.wsm.AcceptReward();
            return;
        }

        SM._.SfxPlay(SM.SFX.ClickSFX);
        HM._.mnm.WindowObj.SetActive(true);
        HM._.mnm.SetUI((int)MineCate.Goblin);
    }
    public void OnClickOreSpotBtn() {
        if(CurWorkSpace.IsFinishWork) {
            HM._.wsm.AcceptReward();
            return;
        }

        SM._.SfxPlay(SM.SFX.ClickSFX);
        HM._.mnm.WindowObj.SetActive(true);
        HM._.mnm.SetUI((int)MineCate.Ore);
    }
#endregion
#region FUNC
    private int GetPrice() => Config.H_PRICE.WORKSPACE_PRICES[CurIdx];
    private void SetCurIdx(int val) { //* -1 とか +1
        CurIdx += val;
        //* Min & Max 繰り返す
        if(CurIdx >= WorkSpaces.Length)
            CurIdx = 0;
        else if(CurIdx < 0)
            CurIdx = WorkSpaces.Length - 1;
    }

    private void UpdateSpotAndUI() {
        //* 配置状態 表示
        ActiveSpot(MineCate.Goblin, CurWorkSpace.GoblinSpotDt);
        ActiveSpot(MineCate.Ore, CurWorkSpace.OreSpotDt);
        //* 最新化
        CurWorkSpace.UpdateUI(WorkAreaTf, GetPrice());
    }

    public void ActiveSpot(MineCate cate, SpotData spotDt) {
        if(cate == MineCate.Goblin) {
            GoblinSpot.Show(spotDt.IsActive);

            //* ✓非表示 初期化
            Array.ForEach(HM._.mnm.GoblinCards, card => card.InitCheck());

            //* ✓と画像 アップデート
            if(spotDt.LvIdx != -1) {
                HM._.mnm.GoblinCards[spotDt.LvIdx].Check();
                GoblinSpot.BodySprLib.spriteLibraryAsset = HM._.mnm.GoblinDataSO.Datas[spotDt.LvIdx].SprLibAst;
            }
        }
        else {
            OreSpot.Show(spotDt.IsActive);
            
            //* ✓非表示 初期化
            Array.ForEach(HM._.mnm.OreCards, card => card.InitCheck());

            //* ✓と画像 アップデート
            if(spotDt.LvIdx != -1) {
                HM._.mnm.OreCards[spotDt.LvIdx].Check();
                OreSpot.OreImg.sprite = HM._.mnm.OreDataSO.Datas[spotDt.LvIdx].Sprs[(int)ORE_SPRS.DEF];
            }
        }
    }

    public void AcceptReward() {
            Debug.Log("ACCEPT MINING REWARD!!");
            SM._.SfxPlay(SM.SFX.RewardSFX);

            //* 1. 初期化
            CurWorkSpace.IsFinishWork = false;
            HM._.mtm.RewardAuraEF.SetActive(false);
            // ゴブリンアニメー
            GoblinChrCtrl.StopGoblinAnim();
            // スライダー UI
            HM._.mtm.InitSlider();
            // 鉱石スポット Off
            CurWorkSpace.OreSpotDt.Init();
            HM._.wsm.OreSpot.Show(isActive: false);
            // 鉱石カード
            Array.ForEach(HM._.mnm.OreCards, card => card.InitCheck());

            //* 2. リワードPopUp表示
            HM._.rwm.RewardListPopUp.SetActive(true);
    }

#endregion
}
