using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using TMPro;

public enum GameEF_UI {
    StageTitleAnim,
}

public enum GameEF {
    NULL = -1,
    BuildTowerEF,
    WoodDestroyEF,
    GrassDestroyEF,
    StoneDestroyEF,
    StarlineExplosionRedEF,
    StarlineExplosionBlueEF,
    StarlineExplosionYellowEF,

    //* Text EF
    DmgTxtEF,
    CritDmgTxtEF,
    RefundTxtEF,

    //* Warrior EF
    SwordSlashWhiteEF,
    SwordSlashYellowEF,
    SwordSlashRedEF,
    SwordSlashBlueEF,
    SwordSlashPurpleBlackEF,

    //* Magician EF
    ExplosionWindEF, // Lv 3
    ExplosionFireballPurpleEF, // Lv 4
    ExplosionFireballBlueEF, // Lv 5
    ExplosionFireballRedEF, // Lv 6
}

public class GameEffectManager : MonoBehaviour {
    /* UI Active Type */
    [field:SerializeField] public GameObject StageTitleAnim;

    /* Pool Type */
    List<ObjectPool<GameObject>> pool = new List<ObjectPool<GameObject>>();
    [field:SerializeField] public GameObject BuildTowerEF;
    [field:SerializeField] public GameObject WoodDestroyEF;
    [field:SerializeField] public GameObject GrassDestroyEF;
    [field:SerializeField] public GameObject StoneDestroyEF;
    [field:SerializeField] public GameObject StarlineExplosionRedEF;
    [field:SerializeField] public GameObject StarlineExplosionBlueEF;
    [field:SerializeField] public GameObject StarlineExplosionYellowEF;
    //* Text EF
    [field:SerializeField] public GameObject DmgTxtEF;
    [field:SerializeField] public GameObject CritDmgTxtEF;
    [field: SerializeField] public GameObject RefundTxtEF;
    //* Warrior EF
    [field: SerializeField] public GameObject SwordSlashWhiteEF;
    [field: SerializeField] public GameObject SwordSlashYellowEF;
    [field: SerializeField] public GameObject SwordSlashRedEF;
    [field: SerializeField] public GameObject SwordSlashBlueEF;
    [field: SerializeField] public GameObject SwordSlashPurpleBlackEF;
    //* Magician EF
    [field: SerializeField] public GameObject ExplosionWindEF;
    [field: SerializeField] public GameObject ExplosionFireballPurpleEF;
    [field: SerializeField] public GameObject ExplosionFireballBlueEF;
    [field: SerializeField] public GameObject ExplosionFireballRedEF;

    void Awake() {
        pool.Add(InitEF(BuildTowerEF, max: 1));
        pool.Add(InitEF(WoodDestroyEF, max: 1));
        pool.Add(InitEF(GrassDestroyEF, max: 1));
        pool.Add(InitEF(StoneDestroyEF, max: 1));
        pool.Add(InitEF(StarlineExplosionRedEF, max: 1));
        pool.Add(InitEF(StarlineExplosionBlueEF, max: 1));
        pool.Add(InitEF(StarlineExplosionYellowEF, max: 1));
        pool.Add(InitEF(DmgTxtEF, max: 75));
        pool.Add(InitEF(CritDmgTxtEF, max: 30));
        pool.Add(InitEF(RefundTxtEF, max: 3));
        //* Warrior EF
        pool.Add(InitEF(SwordSlashWhiteEF, max: 20));
        pool.Add(InitEF(SwordSlashYellowEF, max: 10));
        pool.Add(InitEF(SwordSlashRedEF, max: 10));
        pool.Add(InitEF(SwordSlashBlueEF, max: 10));
        pool.Add(InitEF(SwordSlashPurpleBlackEF, max: 10));
        //* Magician EF
        pool.Add(InitEF(ExplosionWindEF, max: 3));
        pool.Add(InitEF(ExplosionFireballPurpleEF, max: 2));
        pool.Add(InitEF(ExplosionFireballBlueEF, max: 1));
        pool.Add(InitEF(ExplosionFireballRedEF, max: 1));
    }

#region POOL
    private ObjectPool<GameObject> InitEF(GameObject obj, int max) {
        return new ObjectPool<GameObject>(
            () => InstantiateEF(obj), //* 生成
            OnGetEF, //* 呼出
            OnReleaseEF, //* 戻し
            Destroy, 
            maxSize : max //* 最大生成回数
        );
    }
    private GameObject InstantiateEF(GameObject obj) => Instantiate(obj, transform);
    private void OnGetEF(GameObject obj) => obj.SetActive(true);
    private void OnReleaseEF(GameObject obj) => obj.SetActive(false);
#endregion

#region SHOW EFFECT
    public void ShowEF(GameEF efIdx, Vector2 pos, WaitForSeconds delay = null) {
        StartCoroutine(CoShowEF(efIdx, pos, delay));
    }
    IEnumerator CoShowEF(GameEF efIdx, Vector2 pos, WaitForSeconds delay) {
        if(delay == null)
            delay = Util.Time1_5;

        GameObject ef = pool[(int)efIdx].Get();
        ef.transform.position = pos;
        yield return delay;
        pool[(int)efIdx].Release(ef);
    }

    public void ShowDmgTxtEF(Vector2 pos, int dmg, bool isCritical = false) => StartCoroutine(CoShowDmgTxtEF(pos, dmg, isCritical));
    IEnumerator CoShowDmgTxtEF(Vector2 pos, int dmg, bool isCritical) {
        int idx = isCritical? (int)GameEF.CritDmgTxtEF : (int)GameEF.DmgTxtEF;
        GameObject ef = pool[idx].Get();
        ef.transform.position = pos;
        ef.GetComponentInChildren<TextMeshPro>().text = $"{dmg}";
        yield return Util.Time0_75;
        pool[idx].Release(ef);
    }

    public void ShowRefundTxtEF(Vector2 pos, int val) => StartCoroutine(CoShowRefundTxtEF(pos, val));
    IEnumerator CoShowRefundTxtEF(Vector2 pos, int val)
    {
        int idx = (int)GameEF.RefundTxtEF;
        GameObject ef = pool[idx].Get();
        ef.transform.position = pos;
        ef.GetComponentInChildren<TextMeshPro>().text = $"<sprite name=Meat>+{val}";
        yield return Util.Time0_75;
        pool[idx].Release(ef);
    }
    #endregion

#region UI EFFECT ANIM
    public void ActiveStageTitleAnim(string txt) => StartCoroutine(CoActiveStageTitleAnim(txt));
    IEnumerator CoActiveStageTitleAnim(string txt) {
        StageTitleAnim.GetComponentInChildren<TextMeshProUGUI>().text = txt;
        StageTitleAnim.SetActive(true);
        yield return Util.Time3;
        StageTitleAnim.SetActive(false);
    }
#endregion
}
