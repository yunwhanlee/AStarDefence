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
    [field: SerializeField] public WorkSpace[] WorkSpaces {get; private set;} = new WorkSpace[4];
    [field: SerializeField] public WorkSpace CurWorkSpace {get => WorkSpaces[CurIdx];}

    [field: Header("HOME UI ELEM")]
    [field: SerializeField] public CharacterControls GoblinChrCtrl {get; set;}
    [field: SerializeField] public Transform WorkAreaTf {get; set;}
    [field: SerializeField] public TextMeshProUGUI TitleTxt {get; set;}
    [field: SerializeField] public GoblinSpot GoblinSpot;
    [field: SerializeField] public OreSpot OreSpot;

#region EVENT
    public void OnClickPurchaseWorkSpaceBtn() {
        HM._.hui.ShowAgainAskMsg($"<sprite name=Coin>{GetPrice()}을 사용하여\n작업장{HM._.wsm.CurIdx + 1} 구매하시겠습니까?");
        HM._.hui.OnClickConfirmAction = () => {
            CurWorkSpace.IsLock = false;
            CurWorkSpace.UpdateUI(HM._.wsm.WorkAreaTf);
        };
    }
    /// <summary> ワークスペース移動 </summary>
    /// <param name="dir">-1：左、1：右</param>
    public void OnClickWorkSpacePageBtn(int dir) {
        SetCurIdx(dir);
        TitleTxt.text = $"작업장 {CurIdx + 1}";

        //* 配置状態 表示
        ActiveSpot(MineCate.Goblin, CurWorkSpace.GoblinSpotDt);
        ActiveSpot(MineCate.Ore, CurWorkSpace.OreSpotDt);

        //* 作業場（アンロック Or Not）
        CurWorkSpace.UpdateUI(WorkAreaTf, GetPrice());

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

    public void ActiveSpot(MineCate cate, SpotData spotDt) {
        if(cate == MineCate.Goblin) {
            GoblinSpot.Show(spotDt.IsActive);

            //* ページを移動してから退屈中なら、アニメーション再生
            // if(CurWorkSpace.GoblinSpotDt.IsActive && CurWorkSpace.OreSpotDt.IsActive) {
            //     Debug.Log($"GoblinSpot.DisplayObj.activeSelf= {GoblinSpot.DisplayObj.activeSelf}");
            //     if(GoblinSpot.DisplayObj.activeSelf) {
            //         int goblinLvIdx = HM._.wsm.CurWorkSpace.GoblinSpotDt.LvIdx;
            //         GoblinChrCtrl.MiningAnim(goblinLvIdx);
            //     }
            // }

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

#endregion
}
