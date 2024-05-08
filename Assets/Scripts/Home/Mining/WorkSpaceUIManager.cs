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
        get => DM._.DB.MiningDB.WorkSpaces;
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

        //* 採掘中の作業場タイマーを開始
        for(int i = 0; i < WorkSpaces.Length; i++) {
            var ws = WorkSpaces[i];
            bool isSuccess = ws.CheckSpotActive();
            if(isSuccess)
                HM._.mnm.CorTimerIDs[ws.Id] = StartCoroutine(ws.CoTimerStart(false, isPassedTime: true));
        }

        //* ゲーム始まると見える「作業場１」のゴブリンアニメーションとUIに最新化
        // （タイマー開始後、作業場１以外のが開始されたらアニメーションとUIが変更になり、作業場１と合わない）
        OnClickWorkSpacePageBtn(-1);
        OnClickWorkSpacePageBtn(1);
    }

#region EVENT
    public void OnClickPurchaseWorkSpaceBtn() {
        int price = Config.H_PRICE.WORKSPACE_PRICES[HM._.wsm.CurIdx];
        if(HM._.Coin < price) {
            HM._.hui.ShowMsgError("구매할 코인이 부족합니다!");
            return;
        }
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
        //* マイニング チュートリアル表示
        DM._.DB.TutorialDB.CheckShowMiningInfo();

        if(CurWorkSpace.IsFinishWork) {
            HM._.wsm.AcceptReward();
            return;
        }

        HM._.hui.IsActivePopUp = false;
        SM._.SfxPlay(SM.SFX.ClickSFX);
        HM._.mnm.WindowObj.SetActive(true);
        HM._.mnm.SetUI((int)MineCate.Goblin);
    }
    public void OnClickOreSpotBtn() {
        //* マイニング チュートリアル表示
        DM._.DB.TutorialDB.CheckShowMiningInfo();

        if(CurWorkSpace.IsFinishWork) {
            HM._.wsm.AcceptReward();
            return;
        }

        HM._.hui.IsActivePopUp = false;
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
            Debug.Log($"ACCEPT MINING REWARD!! -> OreSpotDt.LvIdx= {CurWorkSpace.OreSpotDt.LvIdx}");
            SM._.SfxPlay(SM.SFX.RewardSFX);

            //* リワード
            RewardItemSO rwDt = HM._.rwlm.RwdItemDt;
            var rewardList = new List<RewardItem>();
            switch(CurWorkSpace.OreSpotDt.LvIdx) {
                case 0:
                    rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Exp], 10));
                    rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Coin], 100));
                    rewardList.Add(new (rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.Present0], 1));
                    break;
                case 1:
                    rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Exp], 20));
                    rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Coin], 250));
                    rewardList.Add(new (rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.Present0], 2));
                    break;
                case 2:
                    rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Exp], 35));
                    rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Coin], 500));
                    rewardList.Add(new (rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.Present0], 3));
                    break;
                case 3:
                    rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Exp], 60));
                    rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Coin], 800));
                    rewardList.Add(new (rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.Present1], 1));
                    break;
                case 4:
                    rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Exp], 95));
                    rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Coin], 1250));
                    rewardList.Add(new (rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.Present1], 2));
                    break;
                case 5:
                    rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Exp], 145));
                    rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Coin], 2000));
                    rewardList.Add(new (rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.Present1], 3));
                    break;
                case 6:
                    rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Exp], 265));
                    rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Coin], 4000));
                    rewardList.Add(new (rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.Present2], 1));
                    break;
                case 7:
                    rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Exp], 340));
                    rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Coin], 7500));
                    rewardList.Add(new (rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.Present2], 2));
                    break;
                case 8:
                    rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Exp], 550));
                    rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Coin], 10000));
                    rewardList.Add(new (rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.Present2], 3));
                    break;
            }

            //* 初期化
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

            //* リワード表示
            HM._.rwlm.ShowReward(rewardList);
            // HM._.rwm.CoUpdateInventoryAsync(rewardList);
    }

#endregion
}
