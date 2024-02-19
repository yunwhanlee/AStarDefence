using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapController : MonoBehaviour {
    [field: SerializeField] public int WallSpawnPer {get; private set;} = 5;  // 壁の生成確率
    [field: SerializeField] public int WallSpawnMax {get; private set;} = 15; // 壁のMAX数
    [field: SerializeField] public Tilemap WallTileMap {get; set;}
    [field: SerializeField] public TileBase Wall {get; set;}
    [field: SerializeField] public int WallSpawnCnt {get; private set;}

    private void Start() {
        WallSpawnCnt = 0;
        spawnWall();
    }

#region FUNC
    public void spawnWall() {
        var sp = Config.START_POS;
        var gp = Config.GOAL_POS;
        const int ofs = 1;
        for(int y = gp.y + ofs; y < sp.y + ofs; y++) {
            for(int x = sp.x + ofs; x < gp.x; x++) {
                int rd = Random.Range(0, 100);
                if(rd < WallSpawnPer) {
                    //* 生成・カウント++
                    WallTileMap.SetTile(new Vector3Int(y, x, 0), Wall);
                    WallSpawnCnt++;

                    //* 生成数を満たしたら、終了
                    if(WallSpawnCnt >= WallSpawnMax) {
                        return;
                    }
                }
            }
        }

        //* 生成数が足りなかったら、再起呼び出す
        if(WallSpawnCnt < WallSpawnMax) {
            spawnWall();
        }
    }
#endregion
}
