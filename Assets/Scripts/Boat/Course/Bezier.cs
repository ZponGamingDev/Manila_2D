using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class Bezier
{
    static public Vector2 GetBezierPointD1(List<Vector2> pts, float t)
    {
        t = Mathf.Clamp01(t);

        return Vector2.Lerp(pts[0], pts[1], t);
    }

    static public Vector2 GetBezierFirstDerivativeD1(List<Vector2> pts, float t)
    {
        return pts[1] - pts[0];
    }

    static public Vector2 GetBezierPointD2(List<Vector2> pts, float t)
    {
		t = Mathf.Clamp01(t);
        float oneMinusT = 1.0f - t;

		return
			oneMinusT * oneMinusT * pts[0] +
			2f * oneMinusT * t * pts[1] +
			t * t * pts[2];
	}

    static public Vector2 GetBezierFirstDerivativeD2(List<Vector2> pts, float t)
    {
		t = Mathf.Clamp01(t);

		return 
            2.0f * (1.0f - t) * (pts[1] - pts[0]) +
            2.0f * t * (pts[2] - pts[1]);
    }

    static public Vector2 GetBezierPointD3(List<Vector2> pts, float t)
    {
		t = Mathf.Clamp01(t);
		float oneMinusT = 1.0f - t;

        return
            oneMinusT * oneMinusT * oneMinusT * pts[0] +
            3f * oneMinusT * oneMinusT * t * pts[1] +
            3f * oneMinusT * t * t * pts[2] +
            t * t * t * pts[3];
    }

    static public Vector2 GetBezierFirstDerivativeD3(List<Vector2> pts, float t)
    {
		t = Mathf.Clamp01(t);
		float oneMinusT = 1f - t;

		return
			3.0f * oneMinusT * oneMinusT * (pts[1] - pts[0]) +
			6.0f * oneMinusT * t * (pts[2] - pts[1]) +
			3.0f * t * t * (pts[3] - pts[2]);
    }
}
