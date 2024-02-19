using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public GameObject startPosObj;
    public GameObject targetPosObj;
    public GameObject pathIconPf;
    public Vector2Int topLeft, bottomRight, startPos, targetPos;
    public List<Node> FinalNodeList;

    int sizeX, sizeY;
    Node[,] NodeArray;
    Node StartNode, TargetNode, CurNode;
    List<Node> OpenList, ClosedList;


    void Start()
    {
        //* Set Position
        Vector2Int start = new Vector2Int((int)Mathf.Round(startPosObj.transform.position.x), (int)Mathf.Round(startPosObj.transform.position.y));
        Vector2Int goal = new Vector2Int((int)Mathf.Round(targetPosObj.transform.position.x), (int)Mathf.Round(targetPosObj.transform.position.y));
        topLeft = start;
        startPos = start;
        bottomRight = goal;
        targetPos = goal;

        //* 以前にルート表示のアイコン 生成
        for(int i = 0; i < PATH_ICON_CREATE_CNT; i ++)
            Instantiate(pathIconPf, pathIconObjGroup);
        //* 以前にルート表示のアイコン 初期化
        initPathIconsPos();
    }

    #region EVENT BUTTON
        public void onClickPathFindBtn() {
            PathFinding();
        }
    #endregion

    #region A* ACLGOLISM
    private void PathFinding() {
        // NodeArray의 크기 정해주고, isWall, x, y 대입
        sizeX = bottomRight.x - topLeft.x + 1;
        sizeY = topLeft.y - bottomRight.y + 1;
        Debug.Log("sizeX= " + sizeX + ", sizeY= " + sizeY);
        NodeArray = new Node[sizeX, sizeY];

        for (int i = 0; i < sizeX; i++) {
            for (int j = 0; j < sizeY; j++) {
                bool isWall = false;
                foreach (Collider2D col in Physics2D.OverlapCircleAll(new Vector2(i + topLeft.x, topLeft.y - j), 0.4f))
                    if (col.gameObject.layer == LayerMask.NameToLayer("Wall")) isWall = true;

                NodeArray[i, j] = new Node(isWall, i + topLeft.x, topLeft.y - j);
            }
        }
        

        // 시작과 끝 노드, 열린리스트와 닫힌리스트, 마지막리스트 초기화
        StartNode = NodeArray[startPos.x - topLeft.x, startPos.y - topLeft.y];
        TargetNode = NodeArray[targetPos.x - topLeft.x, topLeft.y - targetPos.y];
        Debug.Log("StartNode= " + (StartNode.x) + "," + (StartNode.y) + ", TargetNode= " + (TargetNode.x) + "," + (TargetNode.y));


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
                while (TargetCurNode != StartNode)
                {
                    FinalNodeList.Add(TargetCurNode);
                    TargetCurNode = TargetCurNode.ParentNode;
                }
                FinalNodeList.Add(StartNode);
                FinalNodeList.Reverse();

                for (int i = 0; i < FinalNodeList.Count; i++) print(i + "번째는 " + FinalNodeList[i].x + ", " + FinalNodeList[i].y);

                showPathIcons();
                return;
            }

            // ↑ → ↓ ←
            OpenListAdd(CurNode.x, CurNode.y + 1);
            OpenListAdd(CurNode.x + 1, CurNode.y);
            OpenListAdd(CurNode.x, CurNode.y - 1);
            OpenListAdd(CurNode.x - 1, CurNode.y);
        }
    }

    void OpenListAdd(int checkX, int checkY) {
        // 인덱스가 배열 범위를 벗어나는지 확인
        if (checkX >= topLeft.x && checkX <= bottomRight.x && checkY >= bottomRight.y && checkY <= topLeft.y) {
            // 배열 내의 실제 인덱스 계산
            int arrayX = checkX - topLeft.x;
            int arrayY = topLeft.y - checkY;

            // 벽이 아니고 닫힌 리스트에 포함되어 있지 않은 경우에만 처리
            if (!NodeArray[arrayX, arrayY].isWall && !ClosedList.Contains(NodeArray[arrayX, arrayY])) {
                Node NeighborNode = NodeArray[arrayX, arrayY];
                int MoveCost = CurNode.G + ((CurNode.x - checkX == 0 || checkY - CurNode.y == 0) ? 10 : 14);

                if (MoveCost < NeighborNode.G || !OpenList.Contains(NeighborNode)) {
                    NeighborNode.G = MoveCost;
                    NeighborNode.H = (Mathf.Abs(NeighborNode.x - TargetNode.x) + Mathf.Abs(TargetNode.y - NeighborNode.y)) * 10;
                    NeighborNode.ParentNode = CurNode;

                    OpenList.Add(NeighborNode);
                }
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
        for (int i = 1; i < FinalNodeList.Count; i++) { // i를 1부터 시작하도록 수정
            // i가 0보다 큰지 검사하여 유효한 인덱스인지 확인
            if (i > 0 && i < FinalNodeList.Count) {
                //* A*アルゴリズムのルート 表示
                Vector2 pos = new Vector2(FinalNodeList[i - 1].x, FinalNodeList[i - 1].y);
                activePathIconPos(i, pos);
            }
        }

        yield return new WaitForSeconds(1);
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
