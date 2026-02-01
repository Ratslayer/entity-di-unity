using System.Collections.Generic;
using UnityEngine;

public static class LineRectIntersection2D
{
    public static bool LineIntersectsRect(
        Vector2 lineStart,
        Vector2 lineEnd,
        Rect rect,
        out Vector2 intersectionPoint)
    {
        intersectionPoint = Vector2.zero;

        List<Vector2> hits = new List<Vector2>();

        Vector2 bl = new Vector2(rect.xMin, rect.yMin); // bottom-left
        Vector2 br = new Vector2(rect.xMax, rect.yMin); // bottom-right
        Vector2 tr = new Vector2(rect.xMax, rect.yMax); // top-right
        Vector2 tl = new Vector2(rect.xMin, rect.yMax); // top-left

        // Check against all 4 edges
        CheckEdge(lineStart, lineEnd, bl, br, hits);
        CheckEdge(lineStart, lineEnd, br, tr, hits);
        CheckEdge(lineStart, lineEnd, tr, tl, hits);
        CheckEdge(lineStart, lineEnd, tl, bl, hits);

        if (hits.Count == 0)
            return false;

        // Return closest intersection to lineStart
        float minDist = float.MaxValue;
        foreach (var hit in hits)
        {
            float d = Vector2.SqrMagnitude(hit - lineStart);
            if (d < minDist)
            {
                minDist = d;
                intersectionPoint = hit;
            }
        }

        return true;
    }

    private static void CheckEdge(
        Vector2 p1,
        Vector2 p2,
        Vector2 p3,
        Vector2 p4,
        List<Vector2> hits)
    {
        if (LineSegmentIntersection(p1, p2, p3, p4, out Vector2 hit))
        {
            hits.Add(hit);
        }
    }

    // Line segment vs line segment intersection
    private static bool LineSegmentIntersection(
        Vector2 p1,
        Vector2 p2,
        Vector2 p3,
        Vector2 p4,
        out Vector2 intersection)
    {
        intersection = Vector2.zero;

        Vector2 r = p2 - p1;
        Vector2 s = p4 - p3;

        float rxs = Cross(r, s);
        float qpxr = Cross(p3 - p1, r);

        // Parallel or collinear
        if (Mathf.Approximately(rxs, 0f))
            return false;

        float t = Cross(p3 - p1, s) / rxs;
        float u = qpxr / rxs;

        if (t >= 0f && t <= 1f && u >= 0f && u <= 1f)
        {
            intersection = p1 + t * r;
            return true;
        }

        return false;
    }

    private static float Cross(Vector2 a, Vector2 b)
    {
        return a.x * b.y - a.y * b.x;
    }
}
