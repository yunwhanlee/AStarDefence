using System.Collections;
using System.Collections.Generic;
using Inventory.Model;
using UnityEngine;
using UnityEngine.Networking;
using static Enum;

[System.Serializable]
public class GoogleData {
    public string couponId, isExist, isAccept, message;
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

        Debug.Log ($"RESPONSE: id={GD.couponId}, isExist={GD.isExist}, isAccept={GD.isAccept}, message={GD.message}");

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
            SetCouponReward(GD.message); // 쿠폰 보상
        }
    }

    /// <summary>
    /// 쿠폰 보상 수령 (엑셀 Msg 3열에 쓰인 보상리스트 문자열 분석하여 제공)
    /// </summary>
    /// <param name="rewardMsg">보상문자열 분석 : {아이템이름}_{수량} 예시) Weapon1_6</param>
    private void SetCouponReward(string rewardMsg) {
        var invDt = HM._.ivCtrl.InventoryData;
        var rwdItemDt = HM._.rwlm.RwdItemDt;

        //* リワード
        var rwdList = new List<RewardItem>();

        // 보상메세지 분석 => 배열화
        string[] rwdCodeArr = rewardMsg.Split('/');

        // 보상리스트 확인 및 추가
        for(int i = 0; i < rwdCodeArr.Length; i++)
        {
            var split = rwdCodeArr[i].Split('_');
            string rwdName = split[0];
            int cnt = int.Parse(split[1]);

            switch(rwdName)
            {
                // 재화
                case "Coin": rwdList.Add(new (rwdItemDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Coin], cnt)); break;
                case "Diamond": rwdList.Add(new (rwdItemDt.EtcNoShowInvDatas[(int)Etc.NoshowInvItem.Diamond], cnt)); break;

                // 무기
                case "Weapon1": rwdList.Add(new (rwdItemDt.WeaponDatas[(int)Grade.Common], cnt)); break;
                case "Weapon2": rwdList.Add(new (rwdItemDt.WeaponDatas[(int)Grade.Rare], cnt)); break;
                case "Weapon3": rwdList.Add(new (rwdItemDt.WeaponDatas[(int)Grade.Epic], cnt)); break;
                case "Weapon4": rwdList.Add(new (rwdItemDt.WeaponDatas[(int)Grade.Unique], cnt)); break;
                case "Weapon5": rwdList.Add(new (rwdItemDt.WeaponDatas[(int)Grade.Legend], cnt)); break;
                case "Weapon6": rwdList.Add(new (rwdItemDt.WeaponDatas[(int)Grade.Myth], cnt)); break;
                case "Weapon7": rwdList.Add(new (rwdItemDt.WeaponDatas[(int)Grade.Prime], cnt)); break;

                // 신발
                case "Shoes1": rwdList.Add(new (rwdItemDt.ShoesDatas[(int)Grade.Common], cnt)); break;
                case "Shoes2": rwdList.Add(new (rwdItemDt.ShoesDatas[(int)Grade.Rare], cnt)); break;
                case "Shoes3": rwdList.Add(new (rwdItemDt.ShoesDatas[(int)Grade.Epic], cnt)); break;
                case "Shoes4": rwdList.Add(new (rwdItemDt.ShoesDatas[(int)Grade.Unique], cnt)); break;
                case "Shoes5": rwdList.Add(new (rwdItemDt.ShoesDatas[(int)Grade.Legend], cnt)); break;
                case "Shoes6": rwdList.Add(new (rwdItemDt.ShoesDatas[(int)Grade.Myth], cnt)); break;
                case "Shoes7": rwdList.Add(new (rwdItemDt.ShoesDatas[(int)Grade.Prime], cnt)); break;

                // 반지
                case "Ring1": rwdList.Add(new (rwdItemDt.RingDatas[(int)Grade.Common], cnt)); break;
                case "Ring2": rwdList.Add(new (rwdItemDt.RingDatas[(int)Grade.Rare], cnt)); break;
                case "Ring3": rwdList.Add(new (rwdItemDt.RingDatas[(int)Grade.Epic], cnt)); break;
                case "Ring4": rwdList.Add(new (rwdItemDt.RingDatas[(int)Grade.Unique], cnt)); break;
                case "Ring5": rwdList.Add(new (rwdItemDt.RingDatas[(int)Grade.Legend], cnt)); break;
                case "Ring6": rwdList.Add(new (rwdItemDt.RingDatas[(int)Grade.Myth], cnt)); break;
                case "Ring7": rwdList.Add(new (rwdItemDt.RingDatas[(int)Grade.Prime], cnt)); break;

                // 유물
                case "Relic1": rwdList.Add(new (rwdItemDt.RelicDatas[0], cnt, invDt.CheckRelicAbilitiesData(rwdItemDt.RelicDatas[0]))); break; // Epic
                case "Relic2": rwdList.Add(new (rwdItemDt.RelicDatas[1], cnt, invDt.CheckRelicAbilitiesData(rwdItemDt.RelicDatas[1]))); break; // Unique
                case "Relic3": rwdList.Add(new (rwdItemDt.RelicDatas[2], cnt, invDt.CheckRelicAbilitiesData(rwdItemDt.RelicDatas[2]))); break; // Legend
                case "Relic4": rwdList.Add(new (rwdItemDt.RelicDatas[3], cnt, invDt.CheckRelicAbilitiesData(rwdItemDt.RelicDatas[3]))); break; // Myth
                case "Relic5": rwdList.Add(new (rwdItemDt.RelicDatas[4], cnt, invDt.CheckRelicAbilitiesData(rwdItemDt.RelicDatas[4]))); break; // Prime

                // (기타) 보물상자
                case "ChestCommon": rwdList.Add(new (rwdItemDt.EtcConsumableDatas[(int)Etc.ConsumableItem.ChestCommon], cnt)); break;
                case "ChestDiamond": rwdList.Add(new (rwdItemDt.EtcConsumableDatas[(int)Etc.ConsumableItem.ChestDiamond], cnt)); break;
                case "ChestEquipment": rwdList.Add(new (rwdItemDt.EtcConsumableDatas[(int)Etc.ConsumableItem.ChestEquipment], cnt)); break;
                case "ChestGold": rwdList.Add(new (rwdItemDt.EtcConsumableDatas[(int)Etc.ConsumableItem.ChestGold], cnt)); break;
                case "ChestPremium": rwdList.Add(new (rwdItemDt.EtcConsumableDatas[(int)Etc.ConsumableItem.ChestPremium], cnt)); break;
                case "Present0": rwdList.Add(new (rwdItemDt.EtcConsumableDatas[(int)Etc.ConsumableItem.Present0], cnt)); break;
                case "Present1": rwdList.Add(new (rwdItemDt.EtcConsumableDatas[(int)Etc.ConsumableItem.Present1], cnt)); break;
                case "Present2": rwdList.Add(new (rwdItemDt.EtcConsumableDatas[(int)Etc.ConsumableItem.Present2], cnt)); break;

                // (기타) 아이템
                case "Clover": rwdList.Add(new (rwdItemDt.EtcConsumableDatas[(int)Etc.ConsumableItem.Clover], cnt)); break;
                case "GoldClover": rwdList.Add(new (rwdItemDt.EtcConsumableDatas[(int)Etc.ConsumableItem.GoldClover], cnt)); break;
                case "SoulStone": rwdList.Add(new (rwdItemDt.EtcConsumableDatas[(int)Etc.ConsumableItem.SoulStone], cnt)); break;
                case "MagicStone": rwdList.Add(new (rwdItemDt.EtcConsumableDatas[(int)Etc.ConsumableItem.MagicStone], cnt)); break;
                case "SteamPack0": rwdList.Add(new (rwdItemDt.EtcConsumableDatas[(int)Etc.ConsumableItem.SteamPack0], cnt)); break;
                case "SteamPack1": rwdList.Add(new (rwdItemDt.EtcConsumableDatas[(int)Etc.ConsumableItem.SteamPack1], cnt)); break;
                case "BizzardScroll": rwdList.Add(new (rwdItemDt.EtcConsumableDatas[(int)Etc.ConsumableItem.BizzardScroll], cnt)); break;
                case "LightningScroll": rwdList.Add(new (rwdItemDt.EtcConsumableDatas[(int)Etc.ConsumableItem.LightningScroll], cnt)); break;
            }
        }

        //* リワード
        var rewardList = new List<RewardItem> {
            // 태초신발
            new (rwdItemDt.ShoesDatas[6]), 
            // 전설무기
            new (HM._.rwlm.RwdItemDt.WeaponDatas[5]), 
            // 전설반지
            new (HM._.rwlm.RwdItemDt.RingDatas[5]), 
            // 전설유물
            new (HM._.rwlm.RwdItemDt.RelicDatas[2], 12, invDt.CheckRelicAbilitiesData(rwdItemDt.RelicDatas[2])),
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

        HM._.rwlm.ShowReward(rwdList);
    }
}