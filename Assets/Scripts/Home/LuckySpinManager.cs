using System;
using System.Collections;
using System.Collections.Generic;
using AssetKits.ParticleImage;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class LuckySpinManager : MonoBehaviour {
    //* Values
    private const int DivideAngle = 45; // 360° ÷ 8個
    private const int SpinPower = 700;
    private const int DecreaseSpinVal = 500;
    private const int BONUS_MULTI_CHANGE_CNT = 30; //★

    public Action OnClickCloseRewardScreen = () => {};
    [field: SerializeField] public int BonusMultiNum {get; private set;} //★
    [field: SerializeField] public float CurSpeed {get; private set;}
    [field: SerializeField] public bool IsStopSpin {get; private set;}

    //* Elements
    [field: SerializeField] public GameObject WindowObj {get; private set;}
    [field: SerializeField] public GameObject FreeIconObj {get; private set;}
    [field: SerializeField] public GameObject AdIconObj {get; private set;}
    [field: SerializeField] public DOTweenAnimation FocusGlowDOTAnim {get; private set;}
    [field: SerializeField] public DOTweenAnimation BonusMultiNumTxtDOTAnim {get; private set;}
    [field: SerializeField] public Transform SpinBodyTf {get; private set;}
    [field: SerializeField] public TMP_Text StopBtnTxt {get; private set;}
    [field: SerializeField] public TMP_Text GoldkeyTxt {get; private set;}
    [field: SerializeField] public TMP_Text AdFreeSpinTxt {get; private set;}
    [field: SerializeField] public TMP_Text FreeAdBtnCntTxt {get; private set;}
    [field: SerializeField] public TMP_Text BonusMultiNumTxt {get; private set;} //★
    [field: SerializeField] public ParticleImage GoldKeyAttractionUIEF {get; private set;}
    

    void Start() {
		    BonusMultiNum = 1; //★
		    BonusMultiNumTxt.text = $"x{BonusMultiNum}"; //★
        ResetUI();
    }

    void Update() {
        //* 回転のストップをかける
        if(IsStopSpin && CurSpeed > 0) {
            CurSpeed -= DecreaseSpinVal * Time.deltaTime;

            //* 回転が完全にストップしたら
            if(CurSpeed <= 0) {
                //* 角度を検討して、角度に関したリワードIndexを取る
                CurSpeed = 0;
                float angleZ = SpinBodyTf.transform.rotation.eulerAngles.z;

                // Euler -180 ~ 180° -> 0 ~ 360°に変換
                if (angleZ < 0f)
                    angleZ += 360f;
                
                int angleRewardIndex = Mathf.RoundToInt(angleZ / DivideAngle);
                Debug.Log($"Stop Result: rotation.z= {angleZ}, 何番目 : {angleRewardIndex}");

                //* リワード受ける
                StartCoroutine(CoAcceptReward(angleRewardIndex)); //★変更
            }
        }
        //* 回転
        SpinBodyTf.Rotate(new Vector3(0, 0, CurSpeed * Time.deltaTime));
    }

#region EVENT
    public void OnClickLuckySpinIconBtnAtHome() {
        HM._.hui.IsActivePopUp = true;
        SM._.SfxPlay(SM.SFX.ClickSFX);
        WindowObj.SetActive(true);
    }
    public void OnClickBackBtn() {
        if(IsStopSpin) return;

        HM._.hui.IsActivePopUp = false;
        SM._.SfxPlay(SM.SFX.ClickSFX);
        WindowObj.SetActive(false);
    }
    public void OnClickStopSpinBtn() {
        Debug.Log($"OnClickStopSpinBtn():: IsStopSpin= {IsStopSpin}");
        if(HM._.GoldKey < 2) {
            HM._.hui.ShowMsgError("황금열쇠가 부족합니다.");
            return;
        }
        if(IsStopSpin) {
            // HM._.hui.ShowMsgError("이미 룰렛을 멈추고 있습니다. 끝난뒤에 다시 클릭해주세요!");
            return;
        }
        StopSpin();
        HM._.GoldKey -= 2;
    }
    private void SetFreeLuckySpin() {
        try {
            //* 無料AD数を減る
            DM._.DB.LuckySpinFreeAdCnt--;
            AdFreeSpinTxt.text = "광고 무료스핀!";
            AdIconObj.SetActive(true);
            FreeIconObj.SetActive(false);
            FreeAdBtnCntTxt.text = $"{DM._.DB.LuckySpinFreeAdCnt} / {Config.LUCKYSPIN_FREE_AD_CNT}";
            StopSpin();
        }
        catch(Exception ex) {
            Debug.LogError("SetFreeLuckySpin Exception: " + ex.Message);
        }
    }

    public void OnClickFreeAdStopSpinBtn() {
        if(DM._.DB.LuckySpinFreeAdCnt == Config.LUCKYSPIN_FREE_AD_CNT) {
            SetFreeLuckySpin(); 
            return;
        }

        if(DM._.DB.LuckySpinFreeAdCnt <= 0) {
            HM._.hui.ShowMsgError("일일룰렛광고 무료횟수를 전부 사용하였습니다.");
            return;
        }
        if(IsStopSpin) // HM._.hui.ShowMsgError("이미 룰렛을 멈추고 있습니다. 끝난뒤에 다시 클릭해주세요!");
            return; 

        //* リワード広告
        AdmobManager._.ProcessRewardAd(() => SetFreeLuckySpin());
    }
#endregion

#region FUNC
    private void ResetUI() {
        //* 最初から回転させる
        CurSpeed = SpinPower;

        IsStopSpin = false;
        StopBtnTxt.color = Color.white;
        GoldkeyTxt.text = $"{HM._.GoldKey}";

        if(DM._.DB.LuckySpinFreeAdCnt == Config.LUCKYSPIN_FREE_AD_CNT) {
            AdIconObj.SetActive(false);
            FreeIconObj.SetActive(true);
            AdFreeSpinTxt.text = "무료 스핀!";
        }
        else {
            AdIconObj.SetActive(true);
            FreeIconObj.SetActive(false);
            AdFreeSpinTxt.text = "광고 무료스핀!";
        }
        FreeAdBtnCntTxt.text = $"{DM._.DB.LuckySpinFreeAdCnt} / {Config.LUCKYSPIN_FREE_AD_CNT}";
    }
    private void StopSpin() {
        //* ストップ
        SM._.SfxPlay(SM.SFX.WaveStartSFX);
        GoldKeyAttractionUIEF.Play();
        IsStopSpin = true;
        StopBtnTxt.color = Color.red;
    }
    private IEnumerator CoAcceptReward(int angleRewardIndex) { //★変更
		var rwDt = HM._.rwlm.RwdItemDt;
        var rewardList = new List<RewardItem>();
        
        //★ BONUS 掛け算 確率 設定
				int rd = Random.Range(0, 1000);
				const float SGL_PER = 650, // SINGLE
                    DBL_PER = 250, // DOUBLE
                    TRP_PER = 70, // TRIPLE
                    QTR_PER = 25, // QUATER
                    PNT_PER = 5; // PENTA

        //★ BONUS 掛け算 ランダム 選択	
				int multiVal = rd < SGL_PER ? 1
                    : rd < SGL_PER + DBL_PER ? 2
                    : rd < SGL_PER + DBL_PER + TRP_PER ? 3
                    : rd < SGL_PER + DBL_PER + TRP_PER + QTR_PER ? 4
                    : 5;
        
        // スピン結果 + 掛け算適用
        switch (angleRewardIndex) {
            case 8: //* 真ん中からスタートするから、345° ~ 360° と
            case 0: //* 0° ~ 22.5° が０番目リワード
                rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Fame], Random.Range(1, 5 + 1) * multiVal)); //★
                break;
            case 1:
                rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Diamond], 5 * multiVal)); //★
                break;
            case 2:
                rewardList.Add(new (rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.SoulStone], 1 * multiVal)); //★
                break;
            case 3:
                rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Coin], 1500 * multiVal)); //★
                break;
            case 4: {
                int rand = Random.Range(0, 100);
                //* プレゼントの確立
                int presentIdx = (rand < 60)? (int)Etc.ConsumableItem.Present0
                    : (rand < 95)? (int)Etc.ConsumableItem.Present1
                    : (int)Etc.ConsumableItem.Present2;
                rewardList.Add(new (rwDt.EtcConsumableDatas[presentIdx], 1 * multiVal)); //★
                break;
            }
            case 5:
                rewardList.Add(new (rwDt.EtcConsumableDatas[(int)Etc.ConsumableItem.MagicStone], 1 * multiVal)); //★
                break;
            case 6:
                rewardList.Add(new (rwDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Diamond], 50 * multiVal)); //★
                break;
            case 7: {
                int rand = Random.Range(0, 100);
                //* 宝箱の確立
                int chestIdx = (rand < 55)? (int)Etc.ConsumableItem.ChestCommon
                    : (rand < 85)? (int)Etc.ConsumableItem.ChestEquipment
                    : (rand < 95)? (int)Etc.ConsumableItem.GoldClover
                    : (rand < 98)? (int)Etc.ConsumableItem.ChestDiamond
                    : (int)Etc.ConsumableItem.ChestPremium;
                rewardList.Add(new (rwDt.EtcConsumableDatas[chestIdx], 1 * multiVal)); //★
                break;
            }
        }
        
        //★ BONUS 掛け算 X1 ~ X5
        int previousRandNum = -1; // 이전에 생성된 숫자를 저장할 변수 초기화

        FocusGlowDOTAnim.DORestart();

        for(int i = 0; i < BONUS_MULTI_CHANGE_CNT ; i++) {
            yield return Util.Time0_1;
            SM._.SfxPlay(SM.SFX.CountingSFX);
            int randNum = Random.Range(1, 5 + 1);
            do
            {
                randNum = Random.Range(1, 6); // 1부터 5까지 랜덤 숫자 생성
            } while (randNum == previousRandNum); // 이전 숫자와 중복되면 다시 생성

            previousRandNum = randNum; // 현재 숫자를 이전 숫자로 저장
            BonusMultiNumTxt.text = $"x{randNum}";
        }

        FocusGlowDOTAnim.DOPause();
        BonusMultiNumTxtDOTAnim.DORestart();

        //★ 結果
        BonusMultiNum = multiVal;
        BonusMultiNumTxt.text = $"x{BonusMultiNum}";
        
        //★ リワード表示
        HM._.rwlm.ShowReward(rewardList);
        // HM._.rwm.CoUpdateInventoryAsync(rewardList);

        //* リワードPopUpを閉じると実行するイベント購読
        OnClickCloseRewardScreen = ResetUI;
    }
#endregion
}