using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class PathFinder : MonoBehaviour
{
    [Header("��� �����ֱ�")]
    public bool isGizmo = true;

    [Header("�� ������")]
    public int width;
    public int height;
    [Header("��� ������")]
    public float size;
    [Header("�ݶ��̴� ���̾�"), SerializeField]
    LayerMask layer;

    [SerializeField]
    List<Node> NodeList;        //��帮��Ʈ
    Node[,] NodeArr;              //��� �迭

    /// <summary>
    /// ���迭 ���
    /// </summary>
    /// <returns></returns>
    public Node[,] GetNodeArr()
    {
        if (NodeArr == null)
        {
            NodeArr = new Node[height, width];
            int inx = 0;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    NodeArr[i, j] = NodeList[inx];
                    inx++;
                }
            }
        }

        return NodeArr;
    }


    /// <summary>
    /// ������ ��� ���
    /// </summary>
    /// <param name="_objpos"></param>
    /// <returns></returns>
    public Node GetObjectNode(Vector2 _objpos)
    {
        //��� �迭�� ���
        Vector2 tempvec = _objpos;
        tempvec = tempvec - NodeArr[0, 0].Pos;
        tempvec /= size;
        tempvec.x = Mathf.Round(tempvec.x);
        tempvec.y = Mathf.Round(tempvec.y);

        //�迭 ������� Ȯ��
        if (!NodeArr.isOverArr((int)tempvec.y, (int)tempvec.x))
            return null;

        return NodeArr[(int)tempvec.y, (int)tempvec.x];
    }

    /// <summary>
    /// ������ ��� ���
    /// </summary>
    /// <param name="_objpos"></param>
    /// <returns></returns>
    public Node GetObjectNodeMax(Vector2 _objpos)
    {
        //��� �迭�� ���
        Vector2 tempvec = _objpos;
        tempvec = tempvec - NodeArr[0, 0].Pos;
        tempvec /= size;
        tempvec.x = Mathf.Round(tempvec.x);
        tempvec.y = Mathf.Round(tempvec.y);

        //�迭 ������� Ȯ��
        if (tempvec.y >= NodeArr.GetLength(0))
        {
            tempvec.y = NodeArr.GetLength(0) - 1;
        }
        else if (tempvec.y < 0)
        {
            tempvec.y = 0;
        }

        if (tempvec.x >= NodeArr.GetLength(1))
        {
            tempvec.x = NodeArr.GetLength(1) - 1;

        }
        else if (tempvec.x < 0)
        {
            tempvec.x = 0;
        }

        return NodeArr[(int)tempvec.y, (int)tempvec.x];
    }


    //�� ��ĵ
    public void Scan()
    {
        if (NodeList == null)
            NodeList = new List<Node>();
        else
            NodeList.Clear();

        List<Vector2> temp_vec = GetAllVector();

        int inx = 0;
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                NodeList.Add(new Node(temp_vec[inx], i, j));
                inx++;
            }
        }
        NodeArr = null;
        SetNodeCollider();
        SceneView.RepaintAll();
    }

    //�ݶ��̴� ����
    void SetNodeCollider()
    {
        GetNodeArr();
        List<Collider2D> collider_list = new List<Collider2D>(FindObjectsOfType<Collider2D>());     //�ݶ��̴� ����Ʈ
        collider_list = collider_list.FindAll((x) => (layer & 1 << x.gameObject.layer) == 1 << x.gameObject.layer); //�ش� ���̾� �ɷ���

        int[,] DirArr =
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

        float tempSize = size * 0.5f;

        foreach (Collider2D coll in collider_list)
        {
            Vector2 startvec = coll.bounds.min;     //�ݶ��̴� ���� ��ġ
            Vector2 endvec = coll.bounds.max;       //�ݶ��̴� �� ��ġ

            Node startnode = GetObjectNodeMax(startvec);   //�ݶ��̴� ���� ���
            Node endnode = GetObjectNodeMax(endvec);       //�ݶ��̴� �� ���

            //�ش� ��尡 �ִ��� Ȯ��
            if (startnode != null
                && endnode != null)
            {
                //�� üũ
                for (int i = startnode.Y; i <= endnode.Y; i++)
                {
                    for (int j = startnode.X; j <= endnode.X; j++)
                    {
                        Node tempNode = NodeList[(i * width) + j];

                        for (int k = 0; k < DirArr.GetLength(0); k++)
                        {
                            Vector2 tempVec = tempNode.Pos;
                            tempVec.x = tempVec.x + (tempSize * DirArr[k, 0]);
                            tempVec.y = tempVec.y + (tempSize * DirArr[k, 1]);

                            if (coll.OverlapPoint(tempVec))
                            {
                                NodeList[(i * width) + j].IsColl = true;
                            }

                            /*if (coll.OverlapPoint(NodeList[(i * width) + j].Pos))
                            {
                                NodeList[(i * width) + j].IsColl = true;
                            }*/
                        }
                    }
                }

            }
        }


    }

    //�� ���� ���
    List<Vector2> GetAllVector()
    {
        List<Vector2> temp_vec = new List<Vector2>();

        Vector2 totalsize = new Vector2(width, height);     //�� ������
        totalsize *= size;      //��� ������ ��
        totalsize *= 0.5f;      //������ ����
        Vector2 startpos = new Vector2(-totalsize.x, -totalsize.y);     //���� ��ġ
        Vector2 cutpos = new Vector2(startpos.x, startpos.y);           //���� ��ġ
        float possize = size * 0.5f;        //��� ������ ��

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                temp_vec.Add(new Vector2(cutpos.x + possize, cutpos.y + possize) + (Vector2)this.transform.position);   //���� ��ġ + ��� ������ �� + �� ������Ʈ ��ġ
                cutpos.Set(cutpos.x + size, cutpos.y);  //x + ��� ������
            }
            cutpos.Set(startpos.x, cutpos.y + size);    //y + ��� ������
        }

        return temp_vec;
    }


    #region DrawNode
    void DrawSize()
    {
        List<Vector2> temp = GetAllVector();
        foreach (Vector2 item in temp)
        {
            DrawGizmos.DrawGizmosBox(item,
                    new Vector2(size, size), Color.red);
        }
    }

    void DrawNode()
    {
        if (NodeList == null)
            Scan();

        foreach (Node item in NodeList)
        {
            if (item.IsColl)
                Gizmos.color = new Color(0.0f, 1.0f, 0.0f, 0.2f);   //���
            else
                Gizmos.color = new Color(0.0f, 0.0f, 1.0f, 0.2f);   //�Ķ���
            Gizmos.DrawCube(item.Pos, new Vector2(size, size));
        }
    }
    #endregion


    void OnDrawGizmos()
    {
        if (isGizmo)
        {
            DrawSize();
            DrawNode();
        }
    }

}
