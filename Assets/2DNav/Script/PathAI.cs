using System.Collections.Generic;
using UnityEngine;

public class PathAI : MonoBehaviour
{
    [SerializeField]
    PathFinder Path;
    public Transform FollowObj;       // ��ǥ ������Ʈ

    public List<Node> FinalNodeList;        //��ã�� ��� ����Ʈ
    public int NodeInx;    // ����Ʈ �ε���

    Node startNode, targetNode, curNode;     // ����, ��ǥ, ���� ���
    List<Node> openList, closedList;
    Node[,] nodeArr;
    public bool IsFinding { get; private set; }     //��ã�� Ȯ��

    private void Start()
    {
        PathFinding();
    }

    public bool PathFinding()
    {
        IsFinding = true;
        NodeInx = 0;
        nodeArr = Path.GetNodeArr();

        //����Ʈ �ʱ�ȭ
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

        openList.Add(startNode);        // ���� ��� ���¸���Ʈ�� �߰�


        while (true)
        {
            curNode = MinF(openList);   // ���� ���� F�� ã��

            openList.Remove(curNode);   // ���� ��� ���¸���Ʈ���� ����
            closedList.Add(curNode);     // ���� ����Ʈ�� �߰�

            // �ֺ� ��� Ȯ��
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

            // ���� ����Ʈ�� ��ǥ ��尡 �ְų�
            // ���� ����Ʈ�� ��尡 ������ �ݺ� ����
            if (closedList.Contains(targetNode))
                break;
            if (openList.Count <= 0)
                break;

        }

        // ����� �θ� ���󰡼� ��ã�� ����Ʈ ����
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

    //�迭 üũ
    bool AddOpenList(int inx_y, int inx_x)
    {
        // �迭 �Ѵ��� Ȯ��
        if (nodeArr.isOverArr(curNode.Y + inx_y, curNode.X + inx_x))
        {
            // �ֺ� ��� �� üũ
            if (nodeArr[curNode.Y + inx_y, curNode.X].IsColl
                && nodeArr[curNode.Y, curNode.X + inx_x].IsColl)
                return false;

            Node addNode = nodeArr[curNode.Y + inx_y, curNode.X + inx_x];

            // ��
            if (addNode.IsColl)
                return false;

            // �̹� ���� ����Ʈ�� ����
            if (closedList.Contains(addNode))
                return false;

            // ���� ����Ʈ�� �ִ��� Ȯ��
            if (openList.Contains(addNode))
            {
                // ���� ��带 �̿��� ���
                Node tempNode = new Node(addNode);
                tempNode.SetNode(curNode);

                //�� ���� G�� ���� �߰�
                if (tempNode.G < addNode.G)
                {
                    // ���� ��带 �θ�� ����, g�� ���
                    addNode.SetNode(curNode);
                }
            }
            else
            {
                //������
                openList.Add(addNode);     //�߰�
                addNode.SetNode(curNode, targetNode);
            }

            return true;
        }
        return false;
    }

    //���� ���� Fã��
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
