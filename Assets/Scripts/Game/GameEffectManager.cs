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
    UpgradeCylinderRedEF,
    UpgradeCylinderBlueEF,
    UpgradeCylinderYellowEF,

    //* Text EF
    DmgTxtEF,
    CritDmgTxtEF,
    RefundTxtEF,

    //* CCTower EF
    NovaFrostLv1EF, NovaFrostLv2EF, NovaFrostLv3EF,
    NovaLightningLv1EF, NovaLightningLv2EF, NovaLightningLv3EF,
    FrostExplosionEF,
    LightningExplosionEF,

    //* Warrior EF
    SwordSlashWhiteEF,
    SwordSlashYellowEF,
    SwordSlashRedEF,
    SwordSlashBlueEF,
    SwordSlashPurpleBlackEF,

    //* Archer EF
    ArrowExplosionFireEF,
    ArrowCritExplosionEF,

    //* Magician EF
    // Hit
    MiniMagicExplosionBlueEF,
    MagicExplosionYellowEF,
    MagicExplosionWhiteEF,
    MagicExplosionPurpleEF,
    MagicExplosionBlueEF,
    MagicShadowExplosionEF,

    // Skill
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
    [field:SerializeField] public GameObject UpgradeCylinderRedEF;
    [field:SerializeField] public GameObject UpgradeCylinderBlueEF;
    [field:SerializeField] public GameObject UpgradeCylinderYellowEF;
    //* Text EF
    [field:SerializeField] public GameObject DmgTxtEF;
    [field:SerializeField] public GameObject CritDmgTxtEF;
    [field: SerializeField] public GameObject RefundTxtEF;
    //* CCTower EF
    [field: SerializeField] public GameObject NovaFrostLv1EF;
    [field: SerializeField] public GameObject NovaFrostLv2EF;
    [field: SerializeField] public GameObject NovaFrostLv3EF;
    [field: SerializeField] public GameObject NovaLightningLv1EF;
    [field: SerializeField] public GameObject NovaLightningLv2EF;
    [field: SerializeField] public GameObject NovaLightningLv3EF;
    [field: SerializeField] public GameObject FrostExplosionEF;
    [field: SerializeField] public GameObject LightningExplosionEF;
    //* Warrior EF
    [field: SerializeField] public GameObject SwordSlashWhiteEF;
    [field: SerializeField] public GameObject SwordSlashYellowEF;
    [field: SerializeField] public GameObject SwordSlashRedEF;
    [field: SerializeField] public GameObject SwordSlashBlueEF;
    [field: SerializeField] public GameObject SwordSlashPurpleBlackEF;
    //* Archer EF
    [field: SerializeField] public GameObject ArrowExplosionFireEF;
    [field: SerializeField] public GameObject ArrowCritExplosionEF;
    //* Magician EF
    [field: SerializeField] public GameObject MiniMagicExplosionBlueEF;
    [field: SerializeField] public GameObject MagicExplosionYellowEF;
    [field: SerializeField] public GameObject MagicExplosionWhiteEF;
    [field: SerializeField] public GameObject MagicExplosionPurpleEF;
    [field: SerializeField] public GameObject MagicExplosionBlueEF;
    [field: SerializeField] public GameObject MagicShadowExplosionEF;

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
        pool.Add(InitEF(UpgradeCylinderRedEF, max: 1));
        pool.Add(InitEF(UpgradeCylinderBlueEF, max: 1));
        pool.Add(InitEF(UpgradeCylinderYellowEF, max: 1));

        pool.Add(InitEF(DmgTxtEF, max: 75));
        pool.Add(InitEF(CritDmgTxtEF, max: 30));
        pool.Add(InitEF(RefundTxtEF, max: 3));
        //* CCTower EF
        pool.Add(InitEF(NovaFrostLv1EF, max: 2));
        pool.Add(InitEF(NovaFrostLv2EF, max: 2));
        pool.Add(InitEF(NovaFrostLv3EF, max: 2));
        pool.Add(InitEF(NovaLightningLv1EF, max: 2));
        pool.Add(InitEF(NovaLightningLv2EF, max: 2));
        pool.Add(InitEF(NovaLightningLv3EF, max: 2));
        
        pool.Add(InitEF(FrostExplosionEF, max: 10));
        pool.Add(InitEF(LightningExplosionEF, max: 10));
        //* Warrior EF
        pool.Add(InitEF(SwordSlashWhiteEF, max: 20));
        pool.Add(InitEF(SwordSlashYellowEF, max: 10));
        pool.Add(InitEF(SwordSlashRedEF, max: 10));
        pool.Add(InitEF(SwordSlashBlueEF, max: 10));
        pool.Add(InitEF(SwordSlashPurpleBlackEF, max: 10));
        //* Archer EF
        pool.Add(InitEF(ArrowExplosionFireEF, max: 50));
        pool.Add(InitEF(ArrowCritExplosionEF, max: 25));
        //* Magician EF
        pool.Add(InitEF(MiniMagicExplosionBlueEF, max: 20));
        pool.Add(InitEF(MagicExplosionYellowEF, max: 20));
        pool.Add(InitEF(MagicExplosionWhiteEF, max: 20));
        pool.Add(InitEF(MagicExplosionPurpleEF, max: 10));
        pool.Add(InitEF(MagicExplosionBlueEF, max: 10));
        pool.Add(InitEF(MagicShadowExplosionEF, max: 10));
        
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
    public void ShowEF(GameEF efIdx, Vector2 pos, WaitForSeconds delay = null)
        => StartCoroutine(CoShowEF(efIdx, pos, delay));

    IEnumerator CoShowEF(GameEF efIdx, Vector2 pos, WaitForSeconds delay) {
        if(delay == null)
            delay = Util.Time1_5;

        GameObject ef = pool[(int)efIdx].Get();
        ef.transform.position = pos;
        yield return delay;
        pool[(int)efIdx].Release(ef);
    }

    public void ShowDmgTxtEF(Vector2 pos, int dmg, bool isCritical = false)
        => StartCoroutine(CoShowDmgTxtEF(pos, dmg, isCritical));

    IEnumerator CoShowDmgTxtEF(Vector2 pos, int dmg, bool isCritical) {
        int idx = isCritical? (int)GameEF.CritDmgTxtEF : (int)GameEF.DmgTxtEF;
        GameObject ef = pool[idx].Get();
        ef.transform.position = pos;
        ef.GetComponentInChildren<TextMeshPro>().text = $"{dmg}";
        yield return Util.Time0_75;
        pool[idx].Release(ef);
    }

    public void ShowRefundTxtEF(Vector2 pos, int val)
        => StartCoroutine(CoShowRefundTxtEF(pos, val));

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
        Debug.Log("StageTitleAnim= " + StageTitleAnim);
        Debug.Log("StageTitleAnim.GetComponentInChildren<TextMeshProUGUI>()= " + StageTitleAnim.GetComponentInChildren<TextMeshProUGUI>());
        StageTitleAnim.GetComponentInChildren<TextMeshProUGUI>().text = txt;
        StageTitleAnim.SetActive(true);
        yield return Util.Time3;
        StageTitleAnim.SetActive(false);
    }
#endregion
}
