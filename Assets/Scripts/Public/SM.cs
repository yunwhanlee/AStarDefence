using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

[System.Serializable]
/// <summary>
/// SFXサウンドを分けてチャンネル管理
/// </summary>
public class SfxAudio {
    [field: SerializeField] public GameObject Obj {get; set;}
    [field: SerializeField] public AudioSource[] Players {get; set;}
    [field: SerializeField] public float Volume {get; set;}
    [field: SerializeField] public int Channels {get; set;}
    [field: SerializeField] public int ChannelIdx {get; set;}

    public SfxAudio(string objName, int channelCnt, float volume) {
        Channels = channelCnt;
        Volume = volume;
        ChannelIdx = 0;

        Obj = new GameObject(objName);
        Obj.transform.parent = SM._.transform;
        Players = new AudioSource[Channels];

        for(int i = 0; i < Players.Length; i++) {
            Players[i] = Obj.AddComponent<AudioSource>();
            Players[i].playOnAwake = false;
            Players[i].volume = Volume;
        }
    }

    public void PlayClip(AudioClip clip) {
        for(int i = 0; i < Players.Length; i++) {
            int loopIdx = (i + ChannelIdx) % Players.Length;

            //* 現在のAudioPlayerが既に再生中なら、次に進む
            if(Players[loopIdx].isPlaying)
                continue;

            //* 空のAutioPlayerへClip再生
            ChannelIdx = loopIdx;
            Players[loopIdx].clip = clip;
            Players[loopIdx].Play();
            Debug.Log($"{Obj.name}= SfxClips[{Players[loopIdx].clip.name}]");
            break; // ループ終了
        }
    }
}

public class SM : MonoBehaviour {
    static public SM _;

    public enum BGM {
        //* HOME
        HomeBGM,
        //* GAME
        ForestBGM,
        DesertBGM,
        SeaBGM,
        UndeadBGM,
        HellBGM,
        GoblinDungeonBGM,
    }

    public enum SFX {
        //* GAME
        GameStartSFX,
        WaveStartSFX,
        GameoverSFX,
        CreateTowerSFX,
        DeleteTowerSFX,
        
        BuildSFX,
        BreakSFX,
        Merge1SFX,
        Merge2SFX,
        Merge3SFX,
        Merge4SFX,
        Merge5SFX,
        DecreaseLife,
        // Attack
        SwordSFX,
        ArrowSFX,
        Magic1SFX,
        Magic2SFX,
        Magic3SFX,
        Magic4SFX,
        Magic5SFX,
        Magic6SFX,
        CCFrostSFX,
        CCLightningSFX,
        FrostNovaSFX,
        LightningNovaSFX,
        // Hit
        HitSFX,
        HitSFX2,
        HitSFX3,
        MetalHitSFX,
        MetalHitSFX2,
        EnemyDeadSFX,
        EnemyDeadSFX2,
        BossKilledSFX,
        // Skill
        RageSFX,
        CheerUpSFX,
        WheelWindSFX,
        RoarASFX,
        RoarBSFX,
        FireExplosionSFX,
        MagicCircle1SFX,
        MagicCircle2SFX,
        MagicCircle3SFX,
        LaserSFX,
        BigbangSFX,
        PassArrowSFX,
        ArrowRainSFX,

        //* UI
        RewardSFX,
        ClickSFX,
        UpgradeSFX,
        CompleteSFX,
        ErrorSFX,
        StageSelectSFX,
        ItemPickSFX,
        InvEquipSFX,
        InvUnEquipSFX,
        InvStoneSFX,
        LevelUpSFX,
        BossSpawnSFX,
        CountdownSFX,
    }

    //* BGM
    [field: Header("BGM")]
    [field: SerializeField] public AudioSource BgmPlayer {get; set;}
    [field: SerializeField] AudioClip HomeBGM {get; set;}
    [field: SerializeField] AudioClip ForestBGM {get; set;}
    [field: SerializeField] AudioClip DesertBGM {get; set;}
    [field: SerializeField] AudioClip SeaBGM {get; set;}
    [field: SerializeField] AudioClip UndeadBGM {get; set;}
    [field: SerializeField] AudioClip HellBGM {get; set;}
    [field: SerializeField] AudioClip GoblinDungeonBGM {get; set;}

    [field: Header("SFX")]
    [field: SerializeField] AudioClip[] SfxClips {get; set;}
    [field: SerializeField] public SfxAudio NormalSfx {get; set;}
    [field: SerializeField] public SfxAudio HitSfx {get; set;}
    [field: SerializeField] public SfxAudio AtkSfx {get; set;}

    [field: Header("HIT SFX")]
    [field: SerializeField] public GameObject HitSfxObj {get; set;}
    [field: SerializeField] public AudioSource[] HitSfxPlayers {get; set;}
    [field: SerializeField] AudioClip[] HitSfxClips {get; set;}
    [field: SerializeField] public int HitChannels {get; set;}
    [field: SerializeField] public int HitChannelIdx {get; set;}


    void Awake() {
        Singleton();
        InitSfx();
    }

    private void Singleton(){
        if(_ == null) {
            _ = this;
            DontDestroyOnLoad(_);
        }
        else
            Destroy(gameObject);
    }

#region EVENT
        public void OnClickCloseBtnSFX() => SfxPlay(SFX.ClickSFX);
#endregion

#region FUNC
    private void InitSfx() {
        NormalSfx = new SfxAudio("SfxPlayer", 40, 0.5f);
        AtkSfx = new SfxAudio("AtkSfxPlayer", 40, 0.5f);
        HitSfx = new SfxAudio("HitSfxPlayer", 40, 0.5f);
    }

    public void SetVolumeBGM(float volume) {
        BgmPlayer.volume = volume;
        BgmPlayer.gameObject.SetActive(volume > 0);
        BgmPlay(BGM.HomeBGM);
    }
    public void SetVolumeSFX(float volume) {
        DM._.DB.SettingDB.SfxVolume = volume;
        for(int i = 0; i < NormalSfx.Players.Length; i++) NormalSfx.Players[i].volume = volume;
        for(int i = 0; i < HitSfx.Players.Length; i++) HitSfx.Players[i].volume = volume;
        for(int i = 0; i < AtkSfx.Players.Length; i++) AtkSfx.Players[i].volume = volume;
        NormalSfx.Obj.SetActive(volume > 0);
        HitSfx.Obj.SetActive(volume > 0);
        AtkSfx.Obj.SetActive(volume > 0);
    }

    private void SetBGM(AudioClip bgm) {
        BgmPlayer.clip = bgm;
    }

    public void BgmPlay(BGM bgm) {
        //* ON・OFF チェック
        if(!BgmPlayer.gameObject.activeSelf)
            return;

        //* BGM 設定
        if(bgm == BGM.HomeBGM)
            SetBGM(HomeBGM);
        else if(bgm == BGM.ForestBGM)
            SetBGM(ForestBGM);
        else if(bgm == BGM.DesertBGM)
            SetBGM(DesertBGM);
        else if(bgm == BGM.SeaBGM)
            SetBGM(SeaBGM);
        else if(bgm == BGM.UndeadBGM)
            SetBGM(UndeadBGM);
        else if(bgm == BGM.HellBGM)
            SetBGM(HellBGM);
        else if(bgm == BGM.GoblinDungeonBGM)
            SetBGM(GoblinDungeonBGM);

        //* プレイ
        BgmPlayer.Play();
    }

    public void SfxPlay(SFX sfx, float delay = 0) => StartCoroutine(CoSfxPlay(sfx, delay));

    private IEnumerator CoSfxPlay(SFX sfx, float delay) {
        // //* ON・OFF チェック
        if(!NormalSfx.Obj.activeSelf)
            yield break;

        //* Delay
        yield return new WaitForSeconds(delay);

        switch(sfx) {
            case SFX.HitSFX: case SFX.HitSFX2: case SFX.HitSFX3:
            case SFX.MetalHitSFX: case SFX.MetalHitSFX2:
            case SFX.EnemyDeadSFX: case SFX.EnemyDeadSFX2:
            case SFX.BossKilledSFX:
                AudioClip clip = (sfx == SFX.HitSFX || sfx == SFX.HitSFX2 || sfx == SFX.HitSFX)? SfxClips[Random.Range((int)SFX.HitSFX, (int)SFX.HitSFX3 + 1)]
                    : (sfx == SFX.MetalHitSFX || sfx == SFX.MetalHitSFX2)? SfxClips[Random.Range((int)SFX.MetalHitSFX, (int)SFX.MetalHitSFX2 + 1)]
                    : (sfx == SFX.EnemyDeadSFX || sfx == SFX.EnemyDeadSFX2)? SfxClips[Random.Range((int)SFX.EnemyDeadSFX, (int)SFX.EnemyDeadSFX2 + 1)]
                    : null;
                HitSfx.PlayClip(clip);
                break;
            case SFX.SwordSFX: case SFX.ArrowSFX: 
            case SFX.Magic1SFX: case SFX.Magic2SFX: case SFX.Magic3SFX: 
            case SFX.Magic4SFX: case SFX.Magic5SFX: case SFX.Magic6SFX:
            case SFX.CCFrostSFX: case SFX.CCLightningSFX:
            case SFX.FrostNovaSFX: case SFX.LightningNovaSFX: 
                AtkSfx.PlayClip(SfxClips[(int)sfx]);
                break;
            default:
                NormalSfx.PlayClip(SfxClips[(int)sfx]);
                break;
        }
    }
#endregion
}