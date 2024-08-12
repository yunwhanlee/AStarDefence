using System.Collections;
using System.Collections.Generic;
using Inventory.Model;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class GoogleData {
    public string couponId, isExist, isAccept;
}

/// <summary>
/// 1회성 쿠폰 발급 (구글시트 서버통신) Google Sheet -> Google Apps Scripts -> Game
/// </summary>
public class GoogleSheetManager : MonoBehaviour
{
    //! mong유저님 복구 쿠폰코드 : Mong12485d
    const string URL = "https://script.google.com/macros/s/AKfycbxKMft_UQv-lwXeRU-nfidh_vjIHQ_texELIsQd2Afep42VZpqgNM5R0JHcdlLNNQWT/exec"; 

    /// <summary>
    /// 입력된 쿠폰ID 서버에 전송 및 보상확인 및 처리
    /// </summary>
    /// <param name="InputCouponID">입력한 쿠폰ID</param>
    public void CorRequestInputCouponID(string InputCouponID) {
        StartCoroutine(CoRequestInputCouponID(InputCouponID));
    }

    IEnumerator CoRequestInputCouponID(string InputCouponID) {
        const string quaryKey = "id"; // 쿼리 KEY
        
        // 서버에 전송할 포멧 준비
        WWWForm form = new WWWForm();
        form.AddField(quaryKey, InputCouponID);

        // 구글 시트 서버에 전송
        UnityWebRequest www = UnityWebRequest.Post(URL, form);
        yield return www.SendWebRequest(); // 응답까지 대기

        // 서버로부터 결과
        string res = www.downloadHandler.text;
        Debug.Log("GoogleSheetManager:: res= \n" + res);

        if(string.IsNullOrEmpty(res))
            yield break;

        GoogleData GD = JsonUtility.FromJson<GoogleData>(res);

        // 예외 처리
        if(GD.isExist == "false") {
            if(HM._) HM._.hui.ShowMsgError("존재하지 않는 쿠폰ID입니다.");
            yield break;
        }
        else if(GD.isAccept == "true") {
            if(HM._) HM._.hui.ShowMsgError("이미 수령된 쿠폰입니다.");
            yield break;
        }

        //* 쿠폰ID수령
        if(GD.couponId == InputCouponID) {
            Coupon1_Reward(); // 쿠폰1 보상
        }
    }

    private void Coupon1_Reward() {
        //* リワード
        var rewardList = new List<RewardItem> {
            // 태초신발
            new (HM._.rwlm.RwdItemDt.ShoesDatas[6]), 
            // 전설무기
            new (HM._.rwlm.RwdItemDt.WeaponDatas[5]), 
            // 전설반지
            new (HM._.rwlm.RwdItemDt.RingDatas[5]), 
            // 전설유물
            new (HM._.rwlm.RwdItemDt.RelicDatas[2], 12, HM._.ivCtrl.InventoryData.CheckRelicAbilitiesData(HM._.rwlm.RwdItemDt.RelicDatas[2])),
            // 최고급 선물상자
            new (HM._.rwlm.RwdItemDt.EtcConsumableDatas[(int)Etc.ConsumableItem.Present2], 50),
            // 마법의돌
            new (HM._.rwlm.RwdItemDt.EtcConsumableDatas[(int)Etc.ConsumableItem.MagicStone], 70),
            // 영혼의돌
            new (HM._.rwlm.RwdItemDt.EtcConsumableDatas[(int)Etc.ConsumableItem.SoulStone], 70),
            // 프리미엄상자
            new (HM._.rwlm.RwdItemDt.EtcConsumableDatas[(int)Etc.ConsumableItem.ChestPremium], 12),
            // 소비아이템
            new (HM._.rwlm.RwdItemDt.EtcConsumableDatas[(int)Etc.ConsumableItem.Clover], 40),
            new (HM._.rwlm.RwdItemDt.EtcConsumableDatas[(int)Etc.ConsumableItem.GoldClover], 40),
            new (HM._.rwlm.RwdItemDt.EtcConsumableDatas[(int)Etc.ConsumableItem.SteamPack0], 50),
            new (HM._.rwlm.RwdItemDt.EtcConsumableDatas[(int)Etc.ConsumableItem.SteamPack1], 50),
            new (HM._.rwlm.RwdItemDt.EtcConsumableDatas[(int)Etc.ConsumableItem.BizzardScroll], 50),
            new (HM._.rwlm.RwdItemDt.EtcConsumableDatas[(int)Etc.ConsumableItem.LightningScroll], 50),
            // 재화
            new (HM._.rwlm.RwdItemDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Coin], 200000),
            new (HM._.rwlm.RwdItemDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Diamond], 5000),
        };

        HM._.rwlm.ShowReward(rewardList);
    }
}