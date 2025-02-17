using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Util : MonoBehaviour {
    public static Util _;
    public static WaitForSeconds Time0_025 = new WaitForSeconds(0.025f);
    public static WaitForSeconds Time0_05 = new WaitForSeconds(0.05f);
    public static WaitForSeconds Time0_075 = new WaitForSeconds(0.075f);
    public static WaitForSeconds Time0_1 = new WaitForSeconds(0.1f);
    public static WaitForSeconds Time0_15 = new WaitForSeconds(0.15f);
    public static WaitForSeconds Time0_2 = new WaitForSeconds(0.2f);
    public static WaitForSeconds Time0_3 = new WaitForSeconds(0.3f);
    public static WaitForSeconds Time0_4 = new WaitForSeconds(0.4f);
    public static WaitForSeconds Time0_5 = new WaitForSeconds(0.5f);
    public static WaitForSeconds Time0_6 = new WaitForSeconds(0.6f);
    public static WaitForSeconds Time0_65 = new WaitForSeconds(0.65f);
    public static WaitForSeconds Time0_7 = new WaitForSeconds(0.7f);
    public static WaitForSeconds Time0_75 = new WaitForSeconds(0.75f);
    public static WaitForSeconds Time0_8 = new WaitForSeconds(0.8f);
    public static WaitForSeconds Time0_85 = new WaitForSeconds(0.85f);
    public static WaitForSeconds Time0_9 = new WaitForSeconds(0.9f);
    public static WaitForSeconds Time0_95 = new WaitForSeconds(0.95f);
    public static WaitForSeconds Time1 = new WaitForSeconds(1);
    public static WaitForSeconds Time1_5 = new WaitForSeconds(1.5f);
    public static WaitForSeconds Time2 = new WaitForSeconds(2);
    public static WaitForSeconds Time2_5 = new WaitForSeconds(2.5f);
    public static WaitForSeconds Time3 = new WaitForSeconds(3);
    public static WaitForSeconds Time3_5 = new WaitForSeconds(3.5f);
    public static WaitForSeconds Time4 = new WaitForSeconds(4);
    public static WaitForSeconds Time4_5 = new WaitForSeconds(4.5f);
    public static WaitForSeconds Time5 = new WaitForSeconds(5);
    public static WaitForSeconds Time10 = new WaitForSeconds(10);

    public static WaitForSecondsRealtime RealTime0_1 = new WaitForSecondsRealtime(0.1f);
    public static WaitForSecondsRealtime RealTime0_5 = new WaitForSecondsRealtime(0.5f);
    public static WaitForSecondsRealtime RealTime1 = new WaitForSecondsRealtime(1);
    public static WaitForSecondsRealtime RealTime2 = new WaitForSecondsRealtime(2);
    public static WaitForSecondsRealtime RealTime3 = new WaitForSecondsRealtime(3);

    [field:SerializeField] public Material BlinkMt {get; private set;}
    [field:SerializeField] public Material DefaultMt {get; private set;}
    [field:SerializeField] public Material RedMt {get; private set;}
    public static Dictionary<string, Etc.NoshowInvItem> enumNoShowStrDic;

    void Awake() {
        _ = this;

        //* FindEnumValメソッドへ使うCaching変数
        enumNoShowStrDic = new Dictionary<string, Etc.NoshowInvItem>();
        Etc.NoshowInvItem[] enumValArr = (Etc.NoshowInvItem[])System.Enum.GetValues(typeof(Etc.NoshowInvItem));
        for(int i = 0; i < enumValArr.Length; i++) {
            var enumVal = enumValArr[i];
            enumNoShowStrDic.Add(enumVal.ToString(), enumVal);
        }
    }

    /// <summary>
    /// 過ぎた時間を習得
    /// </summary>
    /// <returns>過ぎた時間SEC</returns>
    public static void SetPassedSecData() {
        // 현재 시간을 UTC 기준으로 가져와서 1970년 1월 1일 0시 0분 0초와의 시간 차이를 구합니다.
        TimeSpan curTimeStamp = DateTime.UtcNow - new DateTime(1970,1,1,0,0,0);

        // 이전 Save에서 저장된 시간
        int savedPastTime = PlayerPrefs.GetInt(DM._.PASSEDTIME_KEY, defaultValue: (int)curTimeStamp.TotalSeconds);

        // 현재시간과 이전 저장된 시간차를 통해, 경과한 시간 획득
        DM._.PassedSec = (int)curTimeStamp.TotalSeconds - savedPastTime;
    }

    public static string ConvertTimeFormat(int timeSec) {
        int sec = timeSec % 60;
        int min = timeSec / 60;
        int hour = min / 60;
        string hourStr = (hour == 0)? "" : $"{hour:00} : ";

        return $"{hourStr} {min:00} : {sec:00}";
    }

    public void SetDefMt(SpriteRenderer sprRdr) => sprRdr.material = DefaultMt;
    public void SetRedMt(SpriteRenderer sprRdr) => sprRdr.material = RedMt;
    public bool CheckCriticalDmg(Tower myTower) {
        bool isCritical = false;
        int randPer = Random.Range(0, 100);
        if(randPer < myTower.CritPer * 100) {
            Debug.Log("Critical HIT");
            isCritical = true;
        }
        return isCritical;
    }

    /// <summary>
    /// 小数点四捨五入し、何桁まで表示
    /// </summary>
    /// <param name="val">数値</param>
    /// <param name="decimalCount">小数点何桁まで</param>
    public static float RoundDecimal(float val, int decimalCount = 2) {
        float multiplier = Mathf.Pow(10, decimalCount);
        return Mathf.Round(val * multiplier) / multiplier;
    }

    public void ComboAttack(Enemy enemy, int dmg, int hitCnt, WaitForSeconds delay)
        => StartCoroutine(CoComboAttack(enemy, dmg, hitCnt, delay));
    IEnumerator CoComboAttack(Enemy enemy, int dmg, int hitCnt, WaitForSeconds delay) {
        for(int i = 0; i < hitCnt; i ++) {
            enemy.DecreaseHp(dmg);

            //* 途中で死んだら、コルーチン終了
            if(enemy.Hp < 0 && !enemy.gameObject.activeSelf)
                yield break;

            yield return delay;
        }
    }

    public void Blink(SpriteRenderer sprRdr) => StartCoroutine(CoBlink(sprRdr));
    public void Blink(Image img) => StartCoroutine(CoBlink(img));
    IEnumerator CoBlink(SpriteRenderer sprRdr) {
        sprRdr.material = BlinkMt;
        yield return new WaitForSeconds(0.1f);
        sprRdr.material = DefaultMt;
    }
    private IEnumerator CoBlink(Image img) {
        img.material = BlinkMt;
        yield return new WaitForSeconds(0.1f);
        img.material = DefaultMt;
    }

    public static string DrawEquipItemStarTxt(int lv) {
        string[] starStrArr = new string[5];
        for(int i = 0; i < lv; i++) 
            starStrArr[i % 5] = $"<sprite name={(i < 5? "YellowStar" : "CristalStar")}>";
        return string.Join("", starStrArr);
    }

    

    public static Etc.NoshowInvItem FindEnumVal(string name) {
        // Etc.NoshowInvItem[] enumValArr = (Etc.NoshowInvItem[])System.Enum.GetValues(typeof(Etc.NoshowInvItem));
        if(enumNoShowStrDic.ContainsKey(name)) {
            Debug.Log($"FindEnumVal():: enumNoShowStrDic[{name}]= {enumNoShowStrDic[name]}");
            return enumNoShowStrDic[name];
        }
        return Etc.NoshowInvItem.NULL;
        // Etc.NoshowInvItem enumVal = System.Array.Find(enumValArr, @enum => $"{@enum}" == name);
        // return enumVal;
    }

    public static int GetSize_AbilityType() 
        => System.Enum.GetValues(typeof(AbilityType)).Length;

    public static AbilityType[] GetEnumArray_AbilityType()
        => (AbilityType[])System.Enum.GetValues(typeof(AbilityType));

    public static AbilityType PickRandomAbilityType() {
        AbilityType[] abilities = (AbilityType[])System.Enum.GetValues(typeof(AbilityType));
        //* Potential専用能力と以前の能力を除外したリストを作成
        List<AbilityType> filterList = new List<AbilityType>();
        foreach (AbilityType abt in abilities) {
            //* 除外
            if(abt == AbilityType.Attack) continue;
            if(abt == AbilityType.Speed) continue;
            if(abt == AbilityType.Range) continue;

            filterList.Add(abt);
        }
        filterList.ForEach(filterAbt => {
            Debug.Log($"PickRandomAbilityType():: FirstTime <color=yellow>filterList={filterAbt}</color>");
        });
        // ランダムで選択
        int rand = Random.Range(0, filterList.Count);
        int findIndex = Array.FindIndex(abilities, enumAbt => enumAbt == filterList[rand]);
        return abilities[findIndex];
    }
    public static AbilityType PickRandomAbilityType(AbilityType curType) {
        AbilityType[] abilities = (AbilityType[])System.Enum.GetValues(typeof(AbilityType));
        //* Potential専用能力と以前の能力を除外したリストを作成
        List<AbilityType> filterList = new List<AbilityType>();
        foreach (var abt in abilities) {
            //* 除外
            if(abt == AbilityType.Attack) continue;
            if(abt == AbilityType.Speed) continue;
            if(abt == AbilityType.Range) continue;
            if(abt == curType) continue;

            filterList.Add(abt);
        }
        filterList.ForEach(filterAbt => {
            Debug.Log($"PickRandomAbilityType(curType({curType})):: <color=red>filterList={filterAbt}</color>");
        });
        
        // ランダムで選択
        int rand = Random.Range(0, filterList.Count);
        int findIndex = Array.FindIndex(abilities, enumAbt => enumAbt == filterList[rand]);
        return abilities[findIndex];
    }

    public WaitForSeconds Get1SecByTimeScale() {
        return (Time.timeScale == Config.GAMESPEED_NORMAL)? Time1
            :(Time.timeScale == Config.GAMESPEED_FAST)? Time2
            :(Time.timeScale == Config.GAMESPEED_ULTRA)? Time3
            : Time1;
    }
}
