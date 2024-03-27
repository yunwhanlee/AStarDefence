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
    [field: SerializeField] public WorkSpace[] WorkSpaces {get; private set;} = new WorkSpace[4];

    [field: Header("HOME UI ELEM")]
    [field: SerializeField] public CharacterControls GoblinChrCtrl {get; set;}
    [field: SerializeField] public Slider WorkTimeSlider {get; set;}
    [field: SerializeField] public Transform WorkAreaTf {get; set;}
    [field: SerializeField] public TextMeshProUGUI TitleTxt {get; set;}
    [field: SerializeField] public GoblinSpot GoblinSpot;
    [field: SerializeField] public OreSpot OreSpot;

#region EVENT
    public void OnClickPurchaseWorkSpaceBtn() {
        HM._.hui.ShowAgainAskMsg($"<sprite name=Coin>{GetPrice()}을 사용하여\n작업장{HM._.wsm.CurIdx + 1} 구매하시겠습니까?");
        HM._.hui.OnClickConfirmAction = () => {
            GetCurWorkSpace().IsLock = false;
            GetCurWorkSpace().UpdateUI(HM._.wsm.WorkAreaTf);
        };
    }
    /// <summary> ワークスペース移動 </summary>
    /// <param name="dir">-1：左、1：右</param>
    public void OnClickWorkSpacePageBtn(int dir) {
        SetCurIdx(dir);
        TitleTxt.text = $"작업장 {CurIdx + 1}";
        var curWorkSpace = GetCurWorkSpace();

        //* 作業場（アンロック Or Not）
        curWorkSpace.UpdateUI(WorkAreaTf, GetPrice());

        //* 配置状態 表示
        ActiveSpot(MineCate.Goblin, curWorkSpace.GoblinSpotDt);
        ActiveSpot(MineCate.Ore, curWorkSpace.OreSpotDt);
    }
#endregion

#region FUNC
    public WorkSpace GetCurWorkSpace() => WorkSpaces[CurIdx];
    private int GetPrice() => Config.H_PRICE.WORKSPACE_PRICES[CurIdx];
    private void SetCurIdx(int val) { //* -1 とか +1
        CurIdx += val;
        //* Min & Max 繰り返す
        if(CurIdx >= WorkSpaces.Length)
            CurIdx = 0;
        else if(CurIdx < 0)
            CurIdx = WorkSpaces.Length - 1;
    }

    public void SetTimerSlider(string timeTxt, float value) {
        WorkTimeSlider.GetComponentInChildren<TextMeshProUGUI>().text = $"{timeTxt}";
        WorkTimeSlider.value = value;
    }
    public void ActiveSpot(MineCate cate, SpotData spotDt) {
        const int OFF = 0, ON = 1;
        if(cate == MineCate.Goblin) {
            GoblinSpot.PlusIconObj.SetActive(!spotDt.IsActive);
            GoblinSpot.DisplayObj.SetActive(spotDt.IsActive);

            //* ✓非表示
            Array.ForEach(HM._.mnm.GoblinCards, card => card.InitCheck());

            //* ✓と画像 アップデート
            if(spotDt.LvIdx != -1) {
                HM._.mnm.GoblinCards[spotDt.LvIdx].Check();
                GoblinSpot.BodySprLib.spriteLibraryAsset = HM._.mnm.GoblinSprLibAst[spotDt.LvIdx];
            }
        }
        else {
            OreSpot.PlusIconObj.SetActive(!spotDt.IsActive);
            OreSpot.DisplayObj.SetActive(spotDt.IsActive);
            
            //* ✓非表示
            Array.ForEach(HM._.mnm.OreCards, card => card.InitCheck());

            //* ✓と画像 アップデート
            if(spotDt.LvIdx != -1) {
                HM._.mnm.OreCards[spotDt.LvIdx].Check();
                OreSpot.OreImg.sprite = HM._.mnm.OreSprs[spotDt.LvIdx];
            }

        }
    }

#endregion
}
