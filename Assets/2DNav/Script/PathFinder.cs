using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class PathFinder : MonoBehaviour
{
    [Header("노드 보여주기")]
    public bool isGizmo = true;

    [Header("맵 사이즈")]
    public int width;
    public int height;
    [Header("노드 사이즈")]
    public float size;
    [Header("콜라이더 레이어"), SerializeField]
    LayerMask layer;

    [SerializeField]
    List<Node> NodeList;        //노드리스트
    Node[,] NodeArr;              //노드 배열

    /// <summary>
    /// 노드배열 얻기
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
    /// 벡터의 노드 얻기
    /// </summary>
    /// <param name="_objpos"></param>
    /// <returns></returns>
    public Node GetObjectNode(Vector2 _objpos)
    {
        //노드 배열값 계산
        Vector2 tempvec = _objpos;
        tempvec = tempvec - NodeArr[0, 0].Pos;
        tempvec /= size;
        tempvec.x = Mathf.Round(tempvec.x);
        tempvec.y = Mathf.Round(tempvec.y);

        //배열 벗어나는지 확인
        if (!NodeArr.isOverArr((int)tempvec.y, (int)tempvec.x))
            return null;

        return NodeArr[(int)tempvec.y, (int)tempvec.x];
    }

    /// <summary>
    /// 벡터의 노드 얻기
    /// </summary>
    /// <param name="_objpos"></param>
    /// <returns></returns>
    public Node GetObjectNodeMax(Vector2 _objpos)
    {
        //노드 배열값 계산
        Vector2 tempvec = _objpos;
        tempvec = tempvec - NodeArr[0, 0].Pos;
        tempvec /= size;
        tempvec.x = Mathf.Round(tempvec.x);
        tempvec.y = Mathf.Round(tempvec.y);

        //배열 벗어나는지 확인
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


    //맵 스캔
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

    //콜라이더 세팅
    void SetNodeCollider()
    {
        GetNodeArr();
        List<Collider2D> collider_list = new List<Collider2D>(FindObjectsOfType<Collider2D>());     //콜라이더 리스트
        collider_list = collider_list.FindAll((x) => (layer & 1 << x.gameObject.layer) == 1 << x.gameObject.layer); //해당 레이어 걸러냄

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
            Vector2 startvec = coll.bounds.min;     //콜라이더 시작 위치
            Vector2 endvec = coll.bounds.max;       //콜라이더 끝 위치

            Node startnode = GetObjectNodeMax(startvec);   //콜라이더 시작 노드
            Node endnode = GetObjectNodeMax(endvec);       //콜라이더 끝 노드

            //해당 노드가 있는지 확인
            if (startnode != null
                && endnode != null)
            {
                //벽 체크
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

    //맵 벡터 얻기
    List<Vector2> GetAllVector()
    {
        List<Vector2> temp_vec = new List<Vector2>();

        Vector2 totalsize = new Vector2(width, height);     //맵 사이즈
        totalsize *= size;      //노드 사이즈 곱
        totalsize *= 0.5f;      //반으로 나눔
        Vector2 startpos = new Vector2(-totalsize.x, -totalsize.y);     //시작 위치
        Vector2 cutpos = new Vector2(startpos.x, startpos.y);           //현재 위치
        float possize = size * 0.5f;        //노드 사이즈 반

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                temp_vec.Add(new Vector2(cutpos.x + possize, cutpos.y + possize) + (Vector2)this.transform.position);   //현재 위치 + 노드 사이즈 반 + 이 오브젝트 위치
                cutpos.Set(cutpos.x + size, cutpos.y);  //x + 노드 사이즈
            }
            cutpos.Set(startpos.x, cutpos.y + size);    //y + 노드 사이즈
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
                Gizmos.color = new Color(0.0f, 1.0f, 0.0f, 0.2f);   //녹색
            else
                Gizmos.color = new Color(0.0f, 0.0f, 1.0f, 0.2f);   //파란색
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
