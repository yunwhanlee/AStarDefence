using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Util : MonoBehaviour {
    public static Util _;

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
    public static WaitForSeconds Time1 = new WaitForSeconds(1);
    public static WaitForSeconds Time1_5 = new WaitForSeconds(1.5f);
    public static WaitForSeconds Time2 = new WaitForSeconds(2);
    public static WaitForSeconds Time2_5 = new WaitForSeconds(2.5f);
    public static WaitForSeconds Time3 = new WaitForSeconds(3);
    public static WaitForSeconds Time3_5 = new WaitForSeconds(3.5f);
    public static WaitForSeconds Time4 = new WaitForSeconds(4);
    public static WaitForSeconds Time5 = new WaitForSeconds(5);

    public static WaitForSecondsRealtime RealTime1 = new WaitForSecondsRealtime(1);

    [field:SerializeField] public Material BlinkMt {get; private set;}
    [field:SerializeField] public Material DefaultMt {get; private set;}
    [field:SerializeField] public Material RedMt {get; private set;}

    void Awake() => _ = this;

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
}
