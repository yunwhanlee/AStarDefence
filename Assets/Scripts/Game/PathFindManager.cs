using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Node
{
    public Node(bool _isWall, int _x, int _y) { isWall = _isWall; x = _x; y = _y; }

    public bool isWall;
    public Node ParentNode;

    // G : 시작으로부터 이동했던 거리, H : |가로|+|세로| 장애물 무시하여 목표까지의 거리, F : G + H
    public int x, y, G, H;
    public int F { get { return G + H; } }
}

public class PathFindManager : MonoBehaviour
{
    const int PATH_ICON_CREATE_CNT = 60;

    Coroutine corShowPathIconID;
    public Transform pathIconObjGroup;
    public GameObject pathCntUI;
    public GameObject startPosObj;
    public GameObject targetPosObj;
    public GameObject pathIconPf;
    public Vector2Int bottomLeft, topRight, startPos, targetPos;
    [field: SerializeField] public List<Node> FinalNodeList {get; private set;}

    int sizeX, sizeY;
    Node[,] NodeArray;
    Node StartNode, TargetNode, CurNode;
    List<Node> OpenList, ClosedList;

    void Start() {
        pathCntUI.SetActive(false);

        //* Set Position
        Vector2Int start = new Vector2Int((int)Mathf.Round(startPosObj.transform.position.x), (int)Mathf.Round(startPosObj.transform.position.y));
        Vector2Int goal = new Vector2Int((int)Mathf.Round(targetPosObj.transform.position.x), (int)Mathf.Round(targetPosObj.transform.position.y));
        bottomLeft = start;
        startPos = start;
        topRight = goal;
        targetPos = goal;

        //* 以前にルート表示のアイコン 生成
        for(int i = 0; i < PATH_ICON_CREATE_CNT; i ++)
            Instantiate(pathIconPf, pathIconObjGroup);

        //* 以前にルート表示のアイコン 初期化
        initPathIconsPos();
    }

    #region EVENT
        public void OnClickPathFindBtn() {
            PathFinding(isShowPath: true);
        }
    #endregion

    #region A* ACLGOLISM
    public bool PathFinding(bool isShowPath = false) {
        // NodeArray의 크기 정해주고, isWall, x, y 대입
        sizeX = topRight.x - bottomLeft.x + 1;
        sizeY = topRight.y - bottomLeft.y + 1;
        Debug.Log("sizeX= " + sizeX + ", sizeY= " + sizeY);
        NodeArray = new Node[sizeX, sizeY];

        //* 壁検索
        for (int i = 0; i < sizeX; i++) {
            for (int j = 0; j < sizeY; j++) {
                bool isWall = false;
                foreach (Collider2D col in Physics2D.OverlapCircleAll(new Vector2(i + bottomLeft.x, j + bottomLeft.y), 0.4f))
                    //* 壁タイプのタイル登録
                    if (col.gameObject.layer == Enum.Layer.Wall
                    ||  col.gameObject.layer == Enum.Layer.Board
                    ||  col.gameObject.layer == Enum.Layer.CCTower)
                        isWall = true;

                NodeArray[i, j] = new Node(isWall, i + bottomLeft.x, j + bottomLeft.y);
            }
        }

        // 시작과 끝 노드, 열린리스트와 닫힌리스트, 마지막리스트 초기화
        StartNode = NodeArray[startPos.x - bottomLeft.x, startPos.y - bottomLeft.y];
        TargetNode = NodeArray[targetPos.x - bottomLeft.x, targetPos.y - bottomLeft.y];
        Debug.Log("StartNode= " + StartNode.x + "," + StartNode.y + ", TargetNode= " + TargetNode.x + "," + TargetNode.y);

        OpenList = new List<Node>() { StartNode };
        ClosedList = new List<Node>();
        FinalNodeList = new List<Node>();

        while (OpenList.Count > 0) {
            // 열린리스트 중 가장 F가 작고 F가 같다면 H가 작은 걸 현재노드로 하고 열린리스트에서 닫힌리스트로 옮기기
            CurNode = OpenList[0];
            for (int i = 1; i < OpenList.Count; i++)
                if (OpenList[i].F <= CurNode.F && OpenList[i].H < CurNode.H) CurNode = OpenList[i];

            OpenList.Remove(CurNode);
            ClosedList.Add(CurNode);

            // 마지막
            if (CurNode == TargetNode) {
                Node TargetCurNode = TargetNode;
                var goal = new Node(false, Config.GOAL_POS.x, Config.GOAL_POS.y);
                FinalNodeList.Add(goal);
                while (TargetCurNode != StartNode)
                {
                    FinalNodeList.Add(TargetCurNode);
                    TargetCurNode = TargetCurNode.ParentNode;
                }
                FinalNodeList.Add(StartNode);
                FinalNodeList.Reverse();

                for (int i = 0; i < FinalNodeList.Count; i++) print(i + "번째는 " + FinalNodeList[i].x + ", " + FinalNodeList[i].y);
                if(isShowPath) showPathIcons();
                return true;
            }

            // ↑ → ↓ ←
            OpenListAdd(CurNode.x, CurNode.y + 1);
            OpenListAdd(CurNode.x + 1, CurNode.y);
            OpenListAdd(CurNode.x, CurNode.y - 1);
            OpenListAdd(CurNode.x - 1, CurNode.y);
        }

        //* 道が塞がる(ふさがる)確認
        if(FinalNodeList.Count == 0) {
            Debug.Log("FinalNodeList.Count= " + FinalNodeList.Count);
            return false;
        }

        return true;
    }
    void OpenListAdd(int checkX, int checkY) {
        // 인덱스가 배열 범위를 벗어나는지 확인
        if (checkX >= bottomLeft.x && checkX < topRight.x + 1 && checkY >= bottomLeft.y && checkY < topRight.y + 1 && !NodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y].isWall && !ClosedList.Contains(NodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y]))
        {
            // 이웃노드에 넣고, 직선은 10, 대각선은 14비용
            Node NeighborNode = NodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y];
            int MoveCost = CurNode.G + (CurNode.x - checkX == 0 || CurNode.y - checkY == 0 ? 10 : 14);


            // 이동비용이 이웃노드G보다 작거나 또는 열린리스트에 이웃노드가 없다면 G, H, ParentNode를 설정 후 열린리스트에 추가
            if (MoveCost < NeighborNode.G || !OpenList.Contains(NeighborNode))
            {
                NeighborNode.G = MoveCost;
                NeighborNode.H = (Mathf.Abs(NeighborNode.x - TargetNode.x) + Mathf.Abs(NeighborNode.y - TargetNode.y)) * 10;
                NeighborNode.ParentNode = CurNode;

                OpenList.Add(NeighborNode);
            }
        }
    }
    #endregion

    #region SHOW PATH ICONS
    private void initPathIconsPos() {
        foreach(Transform iconTf in pathIconObjGroup) {
            iconTf.gameObject.SetActive(false);
            iconTf.localPosition = new Vector2(-999, -999);
        }
    }
    private void activePathIconPos(int idx, Vector2 pos) {
        var iconTf = pathIconObjGroup.GetChild(idx);
        iconTf.transform.position = pos;
        iconTf.gameObject.SetActive(true);
    }
    private IEnumerator coShowPathIconsPos() {
        pathCntUI.SetActive(true);
        pathCntUI.GetComponentInChildren<TextMeshProUGUI>().text = FinalNodeList.Count.ToString();
        for (int i = 1; i < FinalNodeList.Count; i++) { // i를 1부터 시작하도록 수정
            // i가 0보다 큰지 검사하여 유효한 인덱스인지 확인
            if (i > 0 && i < FinalNodeList.Count) {
                //* A*アルゴリズムのルート 表示
                Vector2 pos = new Vector2(FinalNodeList[i - 1].x, FinalNodeList[i - 1].y);
                activePathIconPos(i, pos);
            }
        }

        yield return new WaitForSeconds(1);
        pathCntUI.SetActive(false);
        initPathIconsPos();
    }

    private void showPathIcons() {
        Debug.Log("showPathIcons()::");
        if (FinalNodeList.Count >= 2) { // FinalNodeList에 최소 2개 이상의 요소가 있어야 함
            if(corShowPathIconID != null) StopCoroutine(corShowPathIconID);
            corShowPathIconID = StartCoroutine(coShowPathIconsPos());
        }
    }
    #endregion
}
