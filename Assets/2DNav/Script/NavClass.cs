using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Node
{
    public Node Parent;     // 부모
    public Vector2 Pos;     // 위치
    public bool IsColl;        // 벽 확인
    public int X, Y;           // 배열 위치

    public int G { get; private set; }           // 시작점부터 이동 값
    public int H { get; private set; }           // 목적지까지 예상 값
    public int F { get { return G + H; } }  //비용

    public Node(Node node)
    {
        this.Parent = node.Parent;
        this.Pos = node.Pos;
        this.IsColl = node.IsColl;
        this.X = node.X;
        this.Y = node.Y;
        this.G = node.G;
        this.H = node.H;
    }

    public Node(Vector2 vec, int i, int j)
    {
        this.Pos = vec;
        this.Y = i;
        this.X = j;
    }

    /// <summary>
    /// 노드 세팅, H값 계산O
    /// </summary>
    /// <param name="parent">부모</param>
    /// <param name="target">목표 노드</param>
    public void SetNode(Node parent, Node target)
    {
        this.Parent = parent;
        CalG();                // G값 계산
        CalH(target);  // H값 계산
    }

    /// <summary>
    /// 노드세팅, H값 계산X
    /// </summary>
    /// <param name="parent">부모</param>
    public void SetNode(Node parent)
    {
        this.Parent = parent;
        CalG();                // G값 계산
    }

    void CalG()
    {
        //부모노드로 부터 이동한 값 알아내기
        int disX = Mathf.Abs(Parent.X - X);
        int disY = Mathf.Abs(Parent.Y - Y);

        int value = 10; //비용
        if (disX == 0 && disY == 0)         //이동X
            value = 0;
        else if (disX == 1 && disY == 1)    //대각선
            value = 14;


        G = Parent.G + value;   //부모비용 + 비용
    }

    void CalH(Node _target)
    {
        //목적지 예상값 알아내기
        int disX = Mathf.Abs(_target.X - X);
        int disY = Mathf.Abs(_target.Y - Y);

        H = (disX + disY) * 10;
    }

}

public static class MyFunc
{
    public static bool isOverArr<T>(this T[] Arr, int i)
    {
        if (i <= 0
            || i >= Arr.Length)
            return false;

        return true;
    }

    public static bool isOverArr<T>(this T[,] Arr, int i, int j)
    {
        bool isover = true;
        if (i < 0
            || i >= Arr.GetLength(0))
            isover = false;

        if (j < 0
            || j >= Arr.GetLength(1))
            isover = false;

        return isover;
    }
}

public static class DrawGizmos
{
    public static Vector2 DrawGizmosCircleXY(Vector2 pos, float radius, Color _color, int circleStep = 20, float ratioLastPt = 1f)
    {
        Gizmos.color = _color;

        float theta, step = (2f * Mathf.PI) / (float)circleStep;
        Vector2 p0 = pos;
        Vector2 p1 = pos;
        for (int i = 0; i < circleStep; ++i)
        {
            theta = step * (float)i;
            p0.x = pos.x + radius * Mathf.Sin(theta);
            p0.y = pos.y + radius * Mathf.Cos(theta);

            theta = step * (float)(i + 1);
            p1.x = pos.x + radius * Mathf.Sin(theta);
            p1.y = pos.y + radius * Mathf.Cos(theta);
            Gizmos.DrawLine(p0, p1);
        }

        theta = step * ((float)circleStep * ratioLastPt);
        p0.x = pos.x + radius * Mathf.Sin(theta);
        p0.y = pos.y + radius * Mathf.Cos(theta);

        return p0;
    }

    public static void DrawGizmosBox(Vector2 pos, Vector2 size, Color _color)
    {
        Gizmos.color = _color;

        size *= 0.5f;

        Vector2 p0 = pos - size;
        Vector2 p1 = pos + size;

        Gizmos.DrawLine(new Vector2(p0.x, p0.y), new Vector2(p1.x, p0.y));
        Gizmos.DrawLine(new Vector2(p1.x, p0.y), new Vector2(p1.x, p1.y));
        Gizmos.DrawLine(new Vector2(p1.x, p1.y), new Vector2(p0.x, p1.y));
        Gizmos.DrawLine(new Vector2(p0.x, p0.y), new Vector2(p0.x, p1.y));
    }
}