using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class LuckySpinManager : MonoBehaviour {
    //* Values
    private const int DivideAngle = 45; // 360° ÷ 8個
    private const int SpinPower = 700;
    private const int DecreaseSpinVal = 2;

    public Action OnClickCloseRewardScreen = () => {};
    [field: SerializeField] public float CurSpeed {get; private set;}
    [field: SerializeField] public bool IsStopSpin {get; private set;}

    //* Elements
    [field: SerializeField] public GameObject WindowObj {get; private set;}
    [field: SerializeField] public Transform SpinBodyTf {get; private set;}
    [field: SerializeField] public TMP_Text StopBtnTxt {get; private set;}
    [field: SerializeField] public TMP_Text GoldkeyTxt {get; private set;}
    [field: SerializeField] public TMP_Text FreeAdBtnCntTxt {get; private set;}

    void Start() {
        ResetUI();
    }

    void Update() {
        //* 回転のストップをかける
        if(IsStopSpin && CurSpeed > 0) {
            CurSpeed -= DecreaseSpinVal;

            //* 回転が完全にストップしたら
            if(CurSpeed <= 0) {
                //* 角度を検討して、角度に関したリワードIndexを取る
                IsStopSpin = false;
                CurSpeed = 0;
                float angleZ = SpinBodyTf.transform.rotation.eulerAngles.z;

                // Euler -180 ~ 180° -> 0 ~ 360°に変換
                if (angleZ < 0f)
                    angleZ += 360f;
                
                int angleRewardIndex = Mathf.RoundToInt(angleZ / DivideAngle);
                Debug.Log($"Stop Result: rotation.z= {angleZ}, 何番目 : {angleRewardIndex}");

                //* リワード受ける
                AcceptReward(angleRewardIndex);
            }
        }
        //* 回転
        SpinBodyTf.Rotate(new Vector3(0, 0, CurSpeed * Time.deltaTime));
    }

#region EVENT
    public void OnClickLuckySpinIconBtnAtHome() {
        SM._.SfxPlay(SM.SFX.ClickSFX);
        WindowObj.SetActive(true);
    }
    public void OnClickBackBtn() {
        SM._.SfxPlay(SM.SFX.ClickSFX);
        WindowObj.SetActive(false);
    }
    public void OnClickStopSpinBtn() {
        Debug.Log($"OnClickStopSpinBtn():: IsStopSpin= {IsStopSpin}");
        if(HM._.GoldKey < 2) {
            HM._.hui.ShowMsgError("황금열쇠가 부족합니다.");
            return;
        }

        HM._.GoldKey -= 2;
        StopSpin();
    }
    public void OnClickFreeAdStopSpinBtn() {
        //TODO AD

        if(DM._.DB.LuckySpinFreeAdCnt <= 0) {
            HM._.hui.ShowMsgError("일일룰렛광고 무료횟수를 전부 사용하였습니다.");
            return;
        }
        //* 無料AD数を減る
        DM._.DB.LuckySpinFreeAdCnt--;
        FreeAdBtnCntTxt.text = $"{DM._.DB.LuckySpinFreeAdCnt} / {Config.LUCKYSPIN_FREEAD_CNT}";

        StopSpin();
    }
#endregion

#region FUNC
    private void ResetUI() {
        //* 最初から回転させる
        CurSpeed = SpinPower;

        IsStopSpin = false;
        StopBtnTxt.color = Color.white;
        GoldkeyTxt.text = $"{HM._.GoldKey}/{Config.MAX_GOBLINKEY}";
        FreeAdBtnCntTxt.text = $"{DM._.DB.LuckySpinFreeAdCnt} / {Config.LUCKYSPIN_FREEAD_CNT}";
    }
    private void StopSpin() {
        //* STOPしたら、止まるまで操作できないように
        if(IsStopSpin) {
            HM._.hui.ShowMsgError("이미 진행중인 룰렛이 끝난뒤에 다시 클릭해주세요!");
            return;
        }
        //* ストップ
        SM._.SfxPlay(SM.SFX.ItemPickSFX);
        IsStopSpin = true;
        StopBtnTxt.color = Color.red;
    }
    private void AcceptReward(int angleRewardIndex) {
var rwDt = HM._.rwlm.RwdItemDt;
        var rewardList = new List<RewardItem>();
        switch (angleRewardIndex) {
            case 8: //* 真ん中からスタートするから、345° ~ 360° と
            case 0: //* 0° ~ 22.5° が０番目リワード
                rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Coin], 500));
                break;
            case 1:
                rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Diamond], 5));
                break;
            case 2:
                rewardList.Add(new (rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.SoulStone], 1));
                break;
            case 3:
                rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Coin], 1500));
                break;
            case 4: {
                int rand = Random.Range(0, 100);
                //* プレゼントの確立
                int presentIdx = (rand < 60)? (int)Etc.ConsumableItem.Present0
                    : (rand < 95)? (int)Etc.ConsumableItem.Present1
                    : (int)Etc.ConsumableItem.Present2;
                rewardList.Add(new (rwDt.EtcConsumableDatas[presentIdx], 1));
                break;
            }
            case 5:
                rewardList.Add(new (rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.MagicStone], 1));
                break;
            case 6:
                rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Diamond], 50));
                break;
            case 7: {
                int rand = Random.Range(0, 100);
                //* 宝箱の確立
                int chestIdx = (rand < 55)? (int)Etc.ConsumableItem.ChestCommon
                    : (rand < 85)? (int)Etc.ConsumableItem.ChestEquipment
                    : (rand < 95)? (int)Etc.ConsumableItem.GoldClover
                    : (rand < 98)? (int)Etc.ConsumableItem.ChestDiamond
                    : (int)Etc.ConsumableItem.ChestPremium;
                rewardList.Add(new (rwDt.EtcConsumableDatas[chestIdx], 1));
                break;
            }
        }
        HM._.rwlm.ShowReward(rewardList);
        HM._.rwm.UpdateInventory(rewardList);

        //* リワードPopUpを閉じると実行するイベント購読
        OnClickCloseRewardScreen = ResetUI;
    }
#endregion
}
