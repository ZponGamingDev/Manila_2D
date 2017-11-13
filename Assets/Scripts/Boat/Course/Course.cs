using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Course : MonoBehaviour
{
    public bool path = true;
    public bool direction = true;

    public int step = 10;

    [HideInInspector]
    public int degree = 2;

    [HideInInspector]
    public List<Vector2> points = new List<Vector2>();

    private Matrix4x4 oriTransformPointMatrix;
    private Vector2 oriPosition;

    public void SetCourseDataD1(Vector2 p0, Vector2 p1)
    {
        degree = 1;
        points.Add(transform.InverseTransformPoint(p0));
        points.Add(transform.InverseTransformPoint(p1));

        oriTransformPointMatrix = transform.localToWorldMatrix;
        oriPosition = transform.position;
    }

    public void SetCourseDataD2(Vector2 p0, Vector2 p1, Vector2 p2)
    {
        degree = 2;
        points.Add(transform.InverseTransformPoint(p0));
        points.Add(transform.InverseTransformPoint(p1));
        points.Add(transform.InverseTransformPoint(p2));

        oriTransformPointMatrix = transform.localToWorldMatrix;
        oriPosition = transform.position;
    }

    public void SetCourseDataD3(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
    {
        degree = 3;

        points.Add(transform.InverseTransformPoint(p0));
        points.Add(transform.InverseTransformPoint(p1));
        points.Add(transform.InverseTransformPoint(p2));
        points.Add(transform.InverseTransformPoint(p3));

        oriTransformPointMatrix = transform.localToWorldMatrix;
        oriPosition = transform.position;
    }

    public void Reset()
    {
        points.Clear();
    }

    public Vector2 GetPoint(float t)
    {
        Vector2 pt = Vector2.zero;
        switch (degree)
        {
            case 1:
                pt = Bezier.GetBezierPointD1(points, t);
                break;
            case 2:
                pt = Bezier.GetBezierPointD2(points, t);
                break;
            case 3:
                pt = Bezier.GetBezierPointD3(points, t);
                break;
        }

        //if(o!=null)
        Vector4 ptVec4 = oriTransformPointMatrix * new Vector4(pt.x, pt.y, 0.0f, 1.0f);
        return new Vector2(ptVec4.x, ptVec4.y);

        //return transform.TransformPoint(pt);
    }

    public Vector2 GetVelocity(float t)
    {
        Vector2 dt = Vector2.zero;
        switch (degree)
        {
            case 1:
                dt = Bezier.GetBezierFirstDerivativeD1(points, t);
                break;
            case 2:
                dt = Bezier.GetBezierFirstDerivativeD2(points, t);
                break;
            case 3:
                dt = Bezier.GetBezierFirstDerivativeD3(points, t);
                break;
        }

        Vector4 posVec4 = oriTransformPointMatrix * new Vector4(dt.x, dt.y, 0.0f, 1.0f);

        return new Vector2(posVec4.x, posVec4.y) - oriPosition;
    }

    public Vector2 GetDirection(float t)
    {
        return GetVelocity(t).normalized;
    }
}