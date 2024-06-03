using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SelectedTileMapAnim : MonoBehaviour {
    public Tilemap tilemap;
    public Vector3Int tilePosition;
    public Vector3 targetScale;
    public float duration;

    public int top, bottom;
    public int left, right;

    void Start() {
        targetScale = new Vector3(1.15f, 1.15f, 1f);
        duration = 0.75f;

        tilemap = GetComponent<Tilemap>();
        Debug.Log("SelectedTileMapAnim:: GM._.Stage=" + GM._.Stage);
        if(GM._.Stage != Config.Stage.STG_INFINITE_DUNGEON) {
            top = 2;
            bottom = -2;
            right = 6;
            left = -6;
        }
        else {
            top = 2;
            bottom = -3;
            right = 7;
            left = -7;
        }

        if (tilemap == null)
        {
            Debug.LogError("Tilemap이 설정되지 않았습니다.");
            return;
        }

        // 모든 타일의 위치를 순회하며 초기 스케일을 설정합니다.
        foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
        {
            if(pos.x >= bottom && pos.x <= top && pos.y >= left && pos.y <= right) {
                // if(pos.x == bottom && pos.y == left) continue;
                // if(pos.x == top && pos.y == right) continue;
            // if (tilemap.HasTile(pos)) {
                Debug.Log("tilemap.HasTile= " + tilemap.HasTile(pos) + ", pos= " + pos.x + ", " + pos.y);
                // 초기 스케일 설정
                Matrix4x4 initialMatrix = tilemap.GetTransformMatrix(pos);
                Vector3 initialScale = initialMatrix.lossyScale;

                // DOTween을 사용하여 타일의 스케일 애니메이션을 설정합니다.
                Vector3 fromScale = initialScale;
                Vector3 toScale = targetScale;

                DOTween.To(() => fromScale, x =>
                {
                    fromScale = x;
                    Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, fromScale);
                    tilemap.SetTransformMatrix(pos, matrix);
                }, toScale, duration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutQuad);
            }
        }
    }
}
