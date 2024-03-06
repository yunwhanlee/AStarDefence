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
    public static WaitForSeconds Time2 = new WaitForSeconds(2);

    [field:SerializeField] public Material BlinkMt {get; private set;}
    [field:SerializeField] public Material DefaultMt {get; private set;}

    void Awake() => _ = this;

    public void SetDefMt(SpriteRenderer sprRdr) => sprRdr.material = DefaultMt;

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
