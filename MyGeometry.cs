using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MyGeometry {
    /// <summary>
    /// 0 미만 시계방향 0 이상 반시계방향
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    /// <returns></returns>
    public static float CCW(Vector2 p1, Vector2 p2, Vector2 p3) {
        return Vector3.Cross(p2 - p1, p3 - p1).z;
    }

    public static Vector2 GetCircumCircleCenter(Vector2 pt1, Vector2 pt2, Vector2 pt3) {
        float A = pt2.x - pt1.x;
        float B = pt2.y - pt1.y;
        float C = pt3.x - pt1.x;
        float D = pt3.y - pt1.y;
        float E = A * (pt1.x + pt2.x) + B * (pt1.y + pt2.y);
        float F = C * (pt1.x + pt3.x) + D * (pt1.y + pt3.y);
        float G = 2.0f * (A * (pt3.y - pt2.y) - B * (pt3.x - pt2.x));
        if (G == 0)
            return pt1;

        Vector2 ptCenter;
        ptCenter.x = (D * E - B * F) / G;
        ptCenter.y = (A * F - C * E) / G;

        return ptCenter;
    }

    public static Vector2 GetCenter(List<Vector2> points) {
        Vector2 centroid = Vector2.zero;
        float signedArea = 0.0f;
        float x0; // Current vertex X
        float y0; // Current vertex Y
        float x1;// Next vertex X
        float y1; // Next vertex Y
        float a;  // Partial signed area

        int vertexCount = points.Count;
        List<Vector2> vertices = points;

        int i = 0;
        for (i = 0; i < vertexCount - 1; ++i) {
            x0 = vertices[i].x;
            y0 = vertices[i].y;
            x1 = vertices[i + 1].x;
            y1 = vertices[i + 1].y;
            a = x0 * y1 - x1 * y0;
            signedArea += a;
            centroid.x += (x0 + x1) * a;
            centroid.y += (y0 + y1) * a;
        }

        // Do last vertex separately to avoid performing an expensive
        // modulus operation in each iteration.
        x0 = vertices[i].x;
        y0 = vertices[i].y;
        x1 = vertices[0].x;
        y1 = vertices[0].y;
        a = x0 * y1 - x1 * y0;
        signedArea += a;
        centroid.x += (x0 + x1) * a;
        centroid.y += (y0 + y1) * a;

        signedArea *= 0.5f;
        centroid.x /= (6.0f * signedArea);
        centroid.y /= (6.0f * signedArea);
        // centroid += (Vector2)transform.position;
        return centroid;
    }

    // ConvexHull 만들기
    public static List<Vector2> ConvexHull(List<Vector2> points) {
        if (points.Count <= 2)
            return new List<Vector2>();

        Vector2 under = new Vector2(0, float.MaxValue);
        int index = 0;
        for (int i = 0; i < points.Count; i++) {
            if (points[i].y < under.y) {
                under = points[i];
                index = i;
            }
        }
        List<Vector2> lastPoints = new List<Vector2>(points);
        lastPoints.RemoveAt(index);

        List<Vector2> sortPoint = new List<Vector2>();
        sortPoint.Add(under);

        index = 0;
        while (lastPoints.Count > 0) {
            float minAngle = 1000;
            for (int i = 0; i < lastPoints.Count; i++) {
                float angle = Vector2.Angle(Vector2.right, lastPoints[i] - under);
                if (minAngle >= angle) {
                    minAngle = angle;
                    index = i;
                }
            }
            sortPoint.Add(lastPoints[index]);
            lastPoints.RemoveAt(index);
        }

        List<int> convexHull = new List<int>();
        convexHull.Add(0);
        convexHull.Add(1);
        int p1Index;
        int p2Index;
        int p3Index = 2;
        while (true) {
            p1Index = convexHull[convexHull.Count - 2];
            p2Index = convexHull[convexHull.Count - 1];
            Vector2 p1 = sortPoint[p1Index];
            Vector2 p2 = sortPoint[p2Index];
            Vector2 p3 = sortPoint[p3Index];

            float ccw = CCW(p1, p2, p3);
            // 왼쪽
            if (ccw >= 0) {
                convexHull.Add(p3Index);
                p3Index++;
            }
            else {
                convexHull.RemoveAt(convexHull.Count - 1);
            }
            if (p3Index >= sortPoint.Count)
                break;
        }
        List<Vector2> convexHullPoints = new List<Vector2>();
        for (int i = 0; i < convexHull.Count; i++) {
            convexHullPoints.Add(sortPoint[convexHull[i]]);
        }
        return convexHullPoints;
    }
    public static List<Vector2> GetVector2List(List<Transform> transforms) {
        List<Vector2> list = new List<Vector2>();
        for (int i = 0; i < transforms.Count; i++) {
            list.Add(transforms[i].position);
        }
        return list;
    }
    public static List<Vector2> GetVector2List(List<GameObject> gameObejcts) {
        List<Vector2> list = new List<Vector2>();
        for (int i = 0; i < gameObejcts.Count; i++) {
            list.Add(gameObejcts[i].transform.position);
        }
        return list;
    }
    public static List<Vector3> GetVector3List(List<Transform> transforms) {
        List<Vector3> list = new List<Vector3>();
        for(int i =0; i < transforms.Count; i++) {
            list.Add(transforms[i].position);
        }
        return list;
    }
    public static List<Vector3> GetVector3List(List<GameObject> gameObejcts) {
        List<Vector3> list = new List<Vector3>();
        for (int i = 0; i < gameObejcts.Count; i++) {
            list.Add(gameObejcts[i].transform.position);
        }
        return list;
    }
    public static List<Vector2> ClockwiseSort(List<Vector2> points) {
        if (points.Count <= 2)
            return new List<Vector2>();

        Vector2 under = new Vector2(0, float.MaxValue);
        int index = 0;
        for (int i = 0; i < points.Count; i++) {
            if (points[i].y < under.y) {
                under = points[i];
                index = i;
            }
        }
        List<Vector2> lastPoints = new List<Vector2>(points);
        lastPoints.RemoveAt(index);

        List<Vector2> sortPoint = new List<Vector2>();
        sortPoint.Add(under);

        index = 0;
        while (lastPoints.Count > 0) {
            float minAngle = 1000;
            for (int i = 0; i < lastPoints.Count; i++) {
                float angle = Vector2.Angle(Vector2.right, lastPoints[i] - under);
                if (minAngle >= angle) {
                    minAngle = angle;
                    index = i;
                }
            }
            sortPoint.Add(lastPoints[index]);
            lastPoints.RemoveAt(index);
        }
        return sortPoint;
    }
    //https://lazymankook.tistory.com/88#:~:text=%EC%A0%95%EC%A0%90%EC%9D%B4%203D%20Mesh%20%EC%95%88%EC%97%90,%EC%82%AC%EC%9A%A9%ED%95%98%EB%A9%B4%20%EB%90%98%EA%B8%B0%20%EB%95%8C%EB%AC%B8%EC%9D%B4%EB%8B%A4.
    public static bool VertexInPolygon(List<Vector2> polygon, Vector2 point) {
        int crossCount = 0;
        for (int i = 0; i < polygon.Count; i++) {
            int nextIndex = i + 1 < polygon.Count ? i + 1 : 0;
            Vector2 p1 = polygon[i];
            Vector2 p2 = polygon[nextIndex];
            if ((p1.y <= point.y && point.y < p2.y) || (p2.y <= point.y && point.y < p1.y)) {
                Vector2 downP = p1.y < p2.y ? p1 : p2;
                Vector2 upP = p1.y > p2.y ? p1 : p2;

                float v2 = point.y - downP.y;
                float v3 = upP.x - downP.x;
                float v4 = Mathf.Abs(upP.y - downP.y);
                // point위치에서 오른쪽방향으로 무한한길이의 레이저를 쐈을때
                // p1 p2를 잇는 선분과 레이저의 충돌 했을때 생기는 지점과 downP를 빗변으로 하는
                // 직각삼각형의 밑변의 길이
                float v1 = v2 * v3 / v4;
                if (point.x <= downP.x + v1) {
                    crossCount++;
                }
            }
        }
        return crossCount % 2 == 0 ? false : true;
    }

    public static Vector2 Vec3CoordToVec2Coord(Vector3 vec3p, Vector3 planeCenter, Vector3 planeNormal) {
        planeNormal.Normalize();
        float vec3pToPlaneDst = Vector3.Dot(vec3p - planeCenter, planeNormal);
        Vector3 vec3pOnPlane = vec3p + planeNormal * vec3pToPlaneDst;

        float xAngle = Vector3.Angle(Vector3.forward, new Vector3(planeNormal.x, 0, planeNormal.z));
        float yAngle = Vector3.Angle(Vector3.up, new Vector3(planeNormal.x, planeNormal.y, planeNormal.z));
        float zAngle = Vector3.Angle(Vector3.right, new Vector3(planeNormal.x, 0, planeNormal.z));

        float xPos = vec3pOnPlane.x * Mathf.Cos(xAngle * Mathf.Deg2Rad);
        float yPos = vec3pOnPlane.y * Mathf.Sin(yAngle * Mathf.Deg2Rad);
        float zPos = vec3pOnPlane.z * Mathf.Cos(zAngle * Mathf.Deg2Rad);
        Vector2 projectionPos = new Vector2(zPos - xPos, yPos);
        return projectionPos;
    }
    public static List<Vector2> Vec3CoordToVec2Coord(List<Vector3> vec3ps, Vector3 planeCenter, Vector3 planeNormal) {
        List<Vector2> vec2Coord = new List<Vector2>();
        for(int i=0;i < vec3ps.Count; i++) {
            Vector2 vec2 = Vec3CoordToVec2Coord(vec3ps[i], planeCenter, planeNormal);
            vec2Coord.Add(vec2);
        }
        return vec2Coord;
    }
    public static List<IndexTriangle> EarClippingIndexTriangle(List<Vector2> polygon, bool cwSort) {
        int count = polygon.Count;
        List<IndexVector2> indexPolygon = new List<IndexVector2>();
        for(int i =0;i < polygon.Count; i++) {
            indexPolygon.Add(new IndexVector2(i, polygon[i]));
        }
        List<IndexTriangle> triangles = new List<IndexTriangle>();
        List<int> convex = new List<int>();
        int safe = 0;
        while (indexPolygon.Count >= 3) {
            if (safe > count + 10) {
                Debug.LogWarning("세이프아웃");
                break;
            }
            safe++;
            for (int i = 0; i < indexPolygon.Count; i++) {
                int prev = i - 1 < 0 ? indexPolygon.Count - 1 : i - 1;
                int next = i + 1 > indexPolygon.Count - 1 ? 0 : i + 1;
                Vector2 prevPos = indexPolygon[prev].p;
                Vector2 nowPos = indexPolygon[i].p;
                Vector2 nextPos = indexPolygon[next].p;

                if (!cwSort) {
                    if (Vector3.Cross(prevPos - nowPos, nextPos - nowPos).z < 0) {
                        convex.Add(i);
                    }
                    else if (indexPolygon.Count > 3) {
                        continue;
                    }
                }
                else {
                    if (Vector3.Cross(prevPos - nowPos, nextPos - nowPos).z >= 0) {
                        convex.Add(i);
                    }
                    else if (indexPolygon.Count > 3) {
                        continue;
                    }
                }

                List<Vector2> triangleVertexs = new List<Vector2>();
                triangleVertexs.Add(prevPos);
                triangleVertexs.Add(nowPos);
                triangleVertexs.Add(nextPos);

                bool containOtherVertex = false;
                for (int k = 0; k < indexPolygon.Count; k++) {
                    if (k == prev || k == i || k == next)
                        continue;
                    Vector2 point = indexPolygon[k].p;
                    if (MyGeometry.VertexInPolygon(triangleVertexs, point)) {
                        containOtherVertex = true;
                        break;
                    }
                }

                if (!containOtherVertex) {
                    IndexTriangle newTriangle = new IndexTriangle(indexPolygon[prev], indexPolygon[i], indexPolygon[next]);
                    triangles.Add(newTriangle);
                    indexPolygon.RemoveAt(i);
                    i--;
                    // break;
                }
            }
        }
        return triangles;
    }

    public static List<Triangle> EarClippingAlgorithm(List<Vector2> polygonVertexs, bool cwSort) {
        List<Vector2> polygon = new List<Vector2>(polygonVertexs);
        int count = polygon.Count;
        List<Triangle> triangles = new List<Triangle>();
        List<int> convex = new List<int>();
        int safe = 0;
        while (polygon.Count >= 3) {
            if(safe > count + 10) {
                Debug.LogWarning("세이프아웃");
                break;
            }
            safe++;
            for (int i = 0; i < polygon.Count; i++) {
                int prev = i - 1 < 0 ? polygon.Count - 1 : i - 1;
                int next = i + 1 > polygon.Count - 1 ? 0 : i + 1;
                Vector2 prevPos = polygon[prev];
                Vector2 nowPos = polygon[i];
                Vector2 nextPos = polygon[next];

                if (!cwSort) {
                    if (Vector3.Cross(prevPos - nowPos, nextPos - nowPos).z < 0) {
                        convex.Add(i);
                    }
                    else if (polygon.Count > 3) {
                        continue;
                    }
                }
                else {
                    if (Vector3.Cross(prevPos - nowPos, nextPos - nowPos).z >= 0) {
                        convex.Add(i);
                    }
                    else if (polygon.Count > 3) {
                        continue;
                    }
                }

                List<Vector2> triangleVertexs = new List<Vector2>();
                triangleVertexs.Add(prevPos);
                triangleVertexs.Add(nowPos);
                triangleVertexs.Add(nextPos);

                bool containOtherVertex = false;
                for (int k = 0; k < polygon.Count; k++) {
                    if (k == prev || k == i || k == next)
                        continue;
                    Vector2 point = polygon[k];
                    if (MyGeometry.VertexInPolygon(triangleVertexs, point)) {
                        containOtherVertex = true;
                        break;
                    }
                }

                if (!containOtherVertex) {
                    Triangle newTriangle = new Triangle(prevPos, nowPos, nextPos);
                    triangles.Add(newTriangle);
                    polygon.RemoveAt(i);
                    i--;
                   // break;
                }
            }
        }
        return triangles;
    }
    public static Vector2 RandomPointInPolygon(List<Triangle> triangles) {
        if (triangles.Count <= 0)
            return Vector2.zero;

        Triangle randomTri = triangles[Random.Range(0, triangles.Count)];
        float value1 = Random.Range(0f, 1f);
        float value2 = Random.Range(0f, 1f - value1);
        Vector2 p1Top2 = randomTri.p2 - randomTri.p1;
        Vector2 p1Top3 = randomTri.p3 - randomTri.p1;
        Vector2 randomPos = p1Top2 * value1 + p1Top3 * value2 + randomTri.p1;
        return randomPos;
    }
    public static Vector2 RandomPointInPolygon(List<Vector2> polygon, bool cwSort) {
        List<Triangle> triangles = EarClippingAlgorithm(polygon,cwSort);
        return RandomPointInPolygon(triangles);
    }

    /// <summary>
    /// 폴리곤의 넓이를 반환
    /// </summary>
    /// <returns></returns>
    public static float PolygonArea(List<Triangle> triangles) {
        float area = 0;
        for(int i =0; i < triangles.Count; i++) {
            area += triangles[i].Area;
        }
        return area;
    }

    /// <summary>
    /// 폴리곤의 넓이를 반환
    /// </summary>
    /// <returns></returns>
    public static float PolygonArea(List<Vector2> polygon, bool cwSort) {
        List<Triangle> triangles = EarClippingAlgorithm(polygon, cwSort);
        return PolygonArea(triangles);
    }
}
//public static List<Triangle> EarClippingAlgorithm(List<Vector2> polygon) {
//    List<Triangle> triangles = new List<Triangle>();
//    List<PolygonVertex> polygonVertexs = new List<PolygonVertex>();
//    List<PolygonVertex> convex = new List<PolygonVertex>();
//    List<PolygonVertex> reflect = new List<PolygonVertex>();
//    List<PolygonVertex> earTip = new List<PolygonVertex>(); // 
//    Vector2 center = GetCenter(polygon);
//    MyGizmos.DrawWireCicle(center, 0.3f, 30);

//    for (int i = 0; i < polygon.Count; i++) {
//        PolygonVertex vertex = new PolygonVertex(polygon[i]);
//        polygonVertexs.Add(vertex);
//    }
//    for (int i = 0; i < polygon.Count; i++) {
//        int prev = i - 1 < 0 ? polygon.Count - 1 : i - 1;
//        int next = i + 1 > polygon.Count - 1 ? 0 : i + 1;
//        PolygonVertex vertex = polygonVertexs[i];
//        vertex.prev = polygonVertexs[prev];
//        vertex.next = polygonVertexs[next];
//    }
//    int safe = 0;
//    while (polygon.Count >= 3) {
//        if (safe > 100) {
//            Debug.LogWarning("세이프아웃");
//            break;
//        }
//        safe++;
//        for (int i = 0; i < polygonVertexs.Count; i++) {
//            int prev = i - 1 < 0 ? polygonVertexs.Count - 1 : i - 1;
//            int next = i + 1 > polygonVertexs.Count - 1 ? 0 : i + 1;
//            Vector2 prevPos = polygonVertexs[prev].pos;
//            Vector2 nowPos = polygonVertexs[i].pos;
//            Vector2 nextPos = polygonVertexs[next].pos;

//            int pprev = prev - 1 < 0 ? polygonVertexs.Count - 1 : prev - 1;
//            Vector2 pprevPos = polygonVertexs[pprev].pos;
//            float prevCCW = CCW(pprevPos, prevPos, nowPos);
//            float ccw = CCW(prevPos, nowPos, nextPos);

//            if ((ccw <= 0 && prevCCW <= 0) || (ccw >= 0 && prevCCW >= 0)) {
//                convex.Add(polygonVertexs[i]);
//            }
//            else {
//                reflect.Add(polygonVertexs[i]);
//                continue;
//            }

//            List<Vector2> triangleVertexs = new List<Vector2>();
//            triangleVertexs.Add(prevPos);
//            triangleVertexs.Add(nowPos);
//            triangleVertexs.Add(nextPos);

//            bool containOtherVertex = false;
//            for (int k = 0; k < polygon.Count; k++) {
//                if (k == prev || k == i || k == next)
//                    continue;
//                Vector2 point = polygon[k];
//                if (MyGeometry.VertexInPolygon(triangleVertexs, point)) {
//                    containOtherVertex = true;
//                    break;
//                }
//            }

//            if (!containOtherVertex) {
//                earTip.Add(polygonVertexs[i]);
//            }
//        }

//        while (earTip.Count > 0) {
//            PolygonVertex vertex = earTip[0];
//            Vector2 prevPos = vertex.prev.pos;
//            Vector2 nowPos = vertex.pos;
//            Vector2 nextPos = vertex.next.pos;

//            Triangle newTriangle = new Triangle(prevPos, nowPos, nextPos);
//            triangles.Add(newTriangle);
//            vertex.prev.next = vertex.next;
//            vertex.next.prev = vertex.prev;
//        }
//    }
//    return triangles;
//}

//}
//public class PolygonVertex {
//    public PolygonVertex prev;
//    public PolygonVertex next;
//    public Vector2 pos;
//    public PolygonVertex(Vector2 pos) {
//        this.pos = pos;
//    }
//}

public struct IndexVector2 {
    public Vector2 p;
    public int index;
    public IndexVector2(int index, Vector2 p) {
        this.index = index;
        this.p = p;
    }
}
public class IndexTriangle {
    public IndexVector2 p1;
    public IndexVector2 p2;
    public IndexVector2 p3;

    public float Area {
        get
        {
            Vector2 p1Top3 = p3.p - p1.p;
            Vector2 p1Top2 = p2.p - p1.p;
            float lineLength1 = p1Top3.magnitude;
            float lineLength2 = p1Top2.magnitude;
            float angle = Vector2.Angle(p1Top3, p1Top2) * Mathf.Deg2Rad;
            float area = lineLength1 * lineLength2 * Mathf.Sin(angle) / 2;
            return area;
        }
    }

    public IndexTriangle(IndexVector2 p1, IndexVector2 p2, IndexVector2 p3) {
        this.p1 = p1;
        this.p2 = p2;
        this.p3 = p3;
    }
}

public class Triangle {
    public Vector2 p1;
    public Vector2 p2;
    public Vector2 p3;

    public float Area {
        get
        {
            Vector2 p1Top3 = p3 - p1;
            Vector2 p1Top2 = p2 - p1;
            float lineLength1 = p1Top3.magnitude;
            float lineLength2 = p1Top2.magnitude;
            float angle = Vector2.Angle(p1Top3, p1Top2) * Mathf.Deg2Rad;
            float area = lineLength1 * lineLength2 * Mathf.Sin(angle) / 2;
            return area;
        }
    }
    public Triangle(Vector2 p1, Vector2 p2, Vector2 p3) {
        this.p1 = p1;
        this.p2 = p2;
        this.p3 = p3;
    }
}
