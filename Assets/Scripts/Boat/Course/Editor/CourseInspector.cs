using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if DEBUG
[CustomEditor(typeof(Course))]
public class CourseInspector : Editor
{
    private List<Vector2> controlPts = new List<Vector2>();
    private Course course;
    private Transform handleTransform;
    private Quaternion handleRotation;

    private void OnSceneGUI()
    {
        course = target as Course;

        course.degree = Mathf.Clamp(course.degree, 1, 4);

        int d = course.degree + 1;

        if (course.points.Count != d)
        {
            course.points.Clear();
            for (int i = 0; i < d; ++i)
            {
                course.points.Add(Vector2.zero);
            }
        }

        handleTransform = course.transform;
        handleRotation = Tools.pivotRotation == PivotRotation.Local ?
                                handleTransform.rotation : Quaternion.identity;

        Vector2 startPoint = GetPoint(0);
        Vector2 endPoint = GetPoint(course.degree);

        Handles.color = Color.red;

        if (course.path || course.direction)
        {
            Draw();
        }
    }

    private Vector2 GetPoint(int index)
    {
        Vector2 pt = handleTransform.TransformPoint(course.points[index]);

        EditorGUI.BeginChangeCheck();

        pt = Handles.DoPositionHandle(pt, handleRotation);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(course, "Move point");
            EditorUtility.SetDirty(course);
            course.points[index] = handleTransform.InverseTransformPoint(pt);
        }

        return pt;
    }

    private void Draw()
    {
        Vector2 sp = course.GetPoint(0.0f);

        for (int i = 1; i < course.step; ++i)
        {
            Vector2 ep = course.GetPoint(i / (float)course.step);

            if (course.path)
            {
                Handles.color = Color.red;
                Handles.DrawLine(sp, ep);
            }

            if (course.direction)
            {
                Handles.color = Color.green;
                Handles.DrawLine(ep, ep + course.GetDirection(i / (float)course.step));
            }

            sp = ep;
        }
    }
}
#endif