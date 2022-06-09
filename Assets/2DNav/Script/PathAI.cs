using System.Collections.Generic;
using UnityEngine;

public class PathAI : MonoBehaviour
{
    public PathFinder Path;
    public Transform FollowObj;       // 목표 오브젝트

    public List<Node> FinalNodeList;        //길찾기 노드 리스트
    public int NodeInx;    // 리스트 인덱스

    Node startNode, targetNode, curNode;     // 시작, 목표, 현재 노드
    List<Node> openList, closedList;
    Node[,] nodeArr;
    public bool IsFinding { get; private set; }     //길찾기 확인

    private void Start()
    {
        PathFinding();
    }

    public bool PathFinding()
    {
        IsFinding = true;
        NodeInx = 0;
        nodeArr = Path.GetNodeArr();

        //리스트 초기화
        if (openList == null)
            openList = new List<Node>();
        else
            openList.Clear();

        if (closedList == null)
            closedList = new List<Node>();
        else
            closedList.Clear();

        if (FinalNodeList == null)
            FinalNodeList = new List<Node>();
        else
            FinalNodeList.Clear();


        startNode = Path.GetObjectNode(this.gameObject.transform.position);
        targetNode = Path.GetObjectNode(FollowObj.position);

        if (targetNode == null || startNode == null)
            return false;

        openList.Add(startNode);        // 시작 노드 오픈리스트에 추가


        while (true)
        {
            curNode = MinF(openList);   // 가장 작은 F값 찾기

            openList.Remove(curNode);   // 현재 노드 오픈리스트에서 삭제
            closedList.Add(curNode);     // 닫힌 리스트에 추가

            // 주변 노드 확인
            int[,] nodeFindArr =
            {
                { 0, 1 },
                { 0, -1 },
                { 1, 0 },
                { -1, 0 },
                { 1, 1 },
                { 1, -1 },
                { -1, 1 },
                { -1, -1 },
            };
            for(int i = 0; i < nodeFindArr.GetLength(0); i++)
            {
                AddOpenList(nodeFindArr[i ,0], nodeFindArr[i, 1]);
            }

            // 닫힌 리스트에 목표 노드가 있거나
            // 오픈 리스트에 노드가 없으면 반복 종료
            if (closedList.Contains(targetNode))
                break;
            if (openList.Count <= 0)
                break;

        }

        // 노드의 부모를 따라가서 길찾기 리스트 생성
        Node finalnode = targetNode;
        while (true)
        {
            FinalNodeList.Add(finalnode);

            if (finalnode == startNode)
                break;

            if (finalnode.Parent == null)
            {
                IsFinding = false;
                break;
            }

            finalnode = finalnode.Parent;
        }

        FinalNodeList.Reverse();

        return IsFinding;
    }

    //배열 체크
    bool AddOpenList(int inx_y, int inx_x)
    {
        // 배열 넘는지 확인
        if (nodeArr.isOverArr(curNode.Y + inx_y, curNode.X + inx_x))
        {
            // 주변 노드 벽 체크
            if (nodeArr[curNode.Y + inx_y, curNode.X].IsColl
                && nodeArr[curNode.Y, curNode.X + inx_x].IsColl)
                return false;

            Node addNode = nodeArr[curNode.Y + inx_y, curNode.X + inx_x];

            // 벽
            if (addNode.IsColl)
                return false;

            // 이미 닫힌 리스트에 있음
            if (closedList.Contains(addNode))
                return false;

            // 오픈 리스트에 있는지 확인
            if (openList.Contains(addNode))
            {
                // 현재 노드를 이용한 경로
                Node tempNode = new Node(addNode);
                tempNode.SetNode(curNode);

                //더 작은 G값 으로 추가
                if (tempNode.G < addNode.G)
                {
                    // 현재 노드를 부모로 지정, g값 계산
                    addNode.SetNode(curNode);
                }
            }
            else
            {
                //없으면
                openList.Add(addNode);     //추가
                addNode.SetNode(curNode, targetNode);
            }

            return true;
        }
        return false;
    }

    //가장 작은 F찾기
    Node MinF(List<Node> nodeList)
    {
        Node minNode = nodeList[0];
        foreach (Node item in nodeList)
        {
            if (minNode.F > item.F)
            {
                minNode = item;
            }
        }

        return minNode;
    }
    void OnDrawGizmos()
    {
        if (IsFinding)
        {
            if (FinalNodeList != null && FinalNodeList.Count != 0)
            {
                for (int i = 0; i < FinalNodeList.Count - 1; i++)
                {
                    Gizmos.DrawLine(FinalNodeList[i].Pos, FinalNodeList[i + 1].Pos);
                }
            }
        }
    }
}
