using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Util : MonoBehaviour {
    public static Util _;

    public static WaitForSeconds Time0_5 = new WaitForSeconds(0.5f);
    public static WaitForSeconds Time0_75 = new WaitForSeconds(0.75f);
    public static WaitForSeconds Time0_1 = new WaitForSeconds(0.1f);
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
    public static bool CheckCriticalDmg(Tower myTower) {
        bool isCritical = false;
        int randPer = Random.Range(0, 100);
        if(randPer < myTower.CritPer * 100) {
            Debug.Log("Critical HIT");
            isCritical = true;
        }
        return isCritical;
    }

    public void Blink(SpriteRenderer sprRdr) => StartCoroutine(CoBlink(sprRdr));
    public void Blink(Image img) => StartCoroutine(CoBlink(img));
    private IEnumerator CoBlink(SpriteRenderer sprRdr) {
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
