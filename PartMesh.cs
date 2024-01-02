using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class PartMeshUtil {

    public static List<PartMesh> SlashMEsh(MeshRenderer originalRenderer, MeshFilter originalFilter,
        Vector3 slicePoint, Vector3 sliceNormal) {
        slicePoint = slicePoint - originalRenderer.transform.position;
        PartMesh origPartMesh = new PartMesh(originalRenderer, originalFilter);
        Mesh origMesh = originalFilter.mesh;

        // 폴리곤 개수
        int triCount = origMesh.triangles.Length / 3;
        PartMesh positiveMesh = new PartMesh(originalRenderer, originalFilter);
        PartMesh negativeMesh = new PartMesh(originalRenderer, originalFilter);

        List<MeshEdge> edges = new List<MeshEdge>();
        List<MeshEdge> sortEdges = new List<MeshEdge>();
        for (int i = 0; i < triCount; i++) {
            int i0 = i * 3;
            int i1 = i0 + 1;
            int i2 = i0 + 2;

            int tri1 = origMesh.triangles[i0];
            int tri2 = origMesh.triangles[i1];
            int tri3 = origMesh.triangles[i2];

            Vector3 vertex1 = origMesh.vertices[tri1];
            Vector3 vertex2 = origMesh.vertices[tri2];
            Vector3 vertex3 = origMesh.vertices[tri3];

            Vector3 normal1 = origMesh.normals[tri1];
            Vector3 normal2 = origMesh.normals[tri2];
            Vector3 normal3 = origMesh.normals[tri3];

            Vector2 uv1 = origMesh.uv[tri1];
            Vector2 uv2 = origMesh.uv[tri2];
            Vector2 uv3 = origMesh.uv[tri3];

            float dot1 = Vector3.Dot(sliceNormal, vertex1 - slicePoint);
            float dot2 = Vector3.Dot(sliceNormal, vertex2 - slicePoint);
            float dot3 = Vector3.Dot(sliceNormal, vertex3 - slicePoint);

            // 모든 정점이 positive 방향에 있는경우
            if (dot1 >= 0 && dot2 >= 0 && dot3 >= 0) {
                positiveMesh.vertices.Add(vertex1);
                positiveMesh.vertices.Add(vertex2);
                positiveMesh.vertices.Add(vertex3);

                positiveMesh.normals.Add(normal1);
                positiveMesh.normals.Add(normal2);
                positiveMesh.normals.Add(normal3);

                positiveMesh.uvs.Add(uv1);
                positiveMesh.uvs.Add(uv2);
                positiveMesh.uvs.Add(uv3);

                positiveMesh.triangles[0].Add(positiveMesh.triangles[0].Count);
                positiveMesh.triangles[0].Add(positiveMesh.triangles[0].Count);
                positiveMesh.triangles[0].Add(positiveMesh.triangles[0].Count);
            }
            // 모든 정점이 negative 방향에 있는경우
            else if (dot1 <= 0 && dot2 <= 0 && dot3 <= 0) {
                negativeMesh.vertices.Add(vertex1);
                negativeMesh.vertices.Add(vertex2);
                negativeMesh.vertices.Add(vertex3);

                negativeMesh.normals.Add(normal1);
                negativeMesh.normals.Add(normal2);
                negativeMesh.normals.Add(normal3);

                negativeMesh.uvs.Add(uv1);
                negativeMesh.uvs.Add(uv2);
                negativeMesh.uvs.Add(uv3);

                negativeMesh.triangles[0].Add(negativeMesh.triangles[0].Count);
                negativeMesh.triangles[0].Add(negativeMesh.triangles[0].Count);
                negativeMesh.triangles[0].Add(negativeMesh.triangles[0].Count);
            }
            // 정점들 위치가 positive negative 섞인경우
            else {
                int lonelyTri = 0, friendTri1 = 0, friendTri2 = 0;
                Vector3 lonelyV, friendV1, friendV2;
                Vector3 lonelyNormal, friendNormal1, friendNormal2;
                Vector2 lonelyUV, friendUV1, friendUV2;

                // friend순서중요
                if (!SameSign(dot1, dot2) && !SameSign(dot1, dot3)) {
                    lonelyTri = tri1;
                    friendTri1 = tri2;
                    friendTri2 = tri3;
                }
                else if (!SameSign(dot2, dot1) && !SameSign(dot2, dot3)) {
                    lonelyTri = tri2;
                    friendTri1 = tri3;
                    friendTri2 = tri1;
                }
                else if (!SameSign(dot3, dot1) && !SameSign(dot3, dot2)) {
                    lonelyTri = tri3;
                    friendTri1 = tri1;
                    friendTri2 = tri2;
                }
                lonelyV = origMesh.vertices[lonelyTri];
                friendV1 = origMesh.vertices[friendTri1];
                friendV2 = origMesh.vertices[friendTri2];
                lonelyNormal = origMesh.normals[lonelyTri];
                friendNormal1 = origMesh.normals[friendTri1];
                friendNormal2 = origMesh.normals[friendTri2];
                lonelyUV = origMesh.uv[lonelyTri];
                friendUV1 = origMesh.uv[friendTri1];
                friendUV2 = origMesh.uv[friendTri2];

                // 외로운 점이 positive 영역에 있으면 1
                int lonelyInPositive = Vector3.Dot(sliceNormal, lonelyV - slicePoint) >= 0 ? 1 : -1;
                // 90도 꺾은 벡터

                Vector3 crossVec = sliceNormal;
                // EPA 알고리즘에서 사용한 원점에서 한 선분의 최단거리 찾기랑 비슷

                // friend1 정점에서 SlicePlane쪽 방향으로 수직으로 만나는 선의 길이
                float lVToSlicePlaneLength = Mathf.Abs(Vector3.Dot(lonelyV - slicePoint, crossVec));
                float fV1ToSlicePlaneLength = Mathf.Abs(Vector3.Dot(friendV1 - slicePoint, crossVec));
                float fV2ToSlicePlaneLength = Mathf.Abs(Vector3.Dot(friendV2 - slicePoint, crossVec));
                // 각 정점에서 SlicePlane쪽 방향으로 수직으로 가는 선의 길이의 비율은
                // 수직으로 가는 선과 SlicePlane의 충돌 지점과 slicePoint와의 거리의 비율과 같다.
                float lerpValue1 = fV1ToSlicePlaneLength / (lVToSlicePlaneLength + fV1ToSlicePlaneLength);
                float lerpValue2 = fV2ToSlicePlaneLength / (lVToSlicePlaneLength + fV2ToSlicePlaneLength);

                Vector3 lVToSlicePlane = sliceNormal * lonelyInPositive * -1 * lVToSlicePlaneLength + lonelyV;
                Vector3 fV1ToSlicePlane = sliceNormal * lonelyInPositive * 1 * fV1ToSlicePlaneLength + friendV1;
                Vector3 fV2ToSlicePlane = sliceNormal * lonelyInPositive * 1 * fV2ToSlicePlaneLength + friendV2;

                Vector3 sliceP1 = Vector3.Lerp(friendV1, lonelyV, lerpValue1);
                Vector3 sliceP2 = Vector3.Lerp(friendV2, lonelyV, lerpValue2);
                Vector3 sliceP1Normal = Vector3.Lerp(friendNormal1, lonelyNormal, lerpValue1);
                Vector3 sliceP2Normal = Vector3.Lerp(friendNormal2, lonelyNormal, lerpValue2);
                Vector3 sliceP1UV = Vector3.Lerp(friendUV1, lonelyUV, lerpValue1);
                Vector3 sliceP2UV = Vector3.Lerp(friendUV2, lonelyUV, lerpValue2);

                //Vector3 sliceP1 = Vector3.Lerp(lVToSlicePlane, fV1ToSlicePlane, lerpValue1);
                //Vector3 sliceP2 = Vector3.Lerp(lVToSlicePlane, fV2ToSlicePlane, lerpValue2);
                //Vector3 sliceP1Normal = Vector3.Lerp(lonelyNormal, friendNormal1, lerpValue1);
                //Vector3 sliceP2Normal = Vector3.Lerp(lonelyNormal, friendNormal2, lerpValue2);
                //Vector3 sliceP1UV = Vector3.Lerp(lonelyUV, friendUV1, lerpValue1);
                //Vector3 sliceP2UV = Vector3.Lerp(lonelyUV, friendUV2, lerpValue2);

                int newLonelyTri = lonelyInPositive == 1 ? positiveMesh.vertices.Count : negativeMesh.vertices.Count;
                int newFriendTri = lonelyInPositive == 1 ? negativeMesh.vertices.Count : positiveMesh.vertices.Count;

                List<int> lonelyAreaTri = new List<int>();
                List<int> friendAreaTri = new List<int>();
                List<Vector3> lonelyAreaVertex = new List<Vector3>();
                List<Vector3> friendAreaVertex = new List<Vector3>();
                List<Vector3> lonelyAreaNormal = new List<Vector3>();
                List<Vector3> friendAreaNormal = new List<Vector3>();
                List<Vector2> lonelyAreaUV = new List<Vector2>();
                List<Vector2> friendAreaUV = new List<Vector2>();


                // 절단하기
                // fv1이 왼쪽 fv2가 오른쪽에 있다 가정했을때 시계방향으로 순서가 정해져 있는
                // 경우들
                //             l
                //            / ↘
                //   -------------------------- slicePlane
                //          /     ↘
                //      f1 ---------- f2
                // 라고 할려고했는데 이렇게하면 이상하게 된다 왜일까
                #region error    
                //if ((lonelyTri > friendTri1 && friendTri1 > friendTri2) // l = 3  f1 = 2  f2 = 1
                //    || (friendTri2 > lonelyTri && lonelyTri > friendTri1) // f2 = 3  l = 2  f1 = 1
                //    || (friendTri1 > friendTri2 && friendTri2 > lonelyTri)){  // f1 = 3  f2 = 2  l = 1 
                //    lonelyAreaVertex.Add(lonelyV);
                //    lonelyAreaVertex.Add(sliceP2);
                //    lonelyAreaVertex.Add(sliceP1);

                //    lonelyAreaNormal.Add(lonelyNormal);
                //    lonelyAreaNormal.Add(sliceP2Normal);
                //    lonelyAreaNormal.Add(sliceP1Normal);

                //    lonelyAreaUV.Add(lonelyUV);
                //    lonelyAreaUV.Add(sliceP2UV);
                //    lonelyAreaUV.Add(sliceP1UV);

                //    lonelyAreaTri.Add(newLonelyTri);
                //    lonelyAreaTri.Add(newLonelyTri + 1);
                //    lonelyAreaTri.Add(newLonelyTri + 2);

                //    // 친구있는 놈들
                //    // p1쪽
                //    friendAreaVertex.Add(friendV1);
                //    friendAreaVertex.Add(sliceP1);
                //    friendAreaVertex.Add(friendV2);

                //    friendAreaNormal.Add(friendNormal1);
                //    friendAreaNormal.Add(sliceP1Normal);
                //    friendAreaNormal.Add(friendNormal2);

                //    friendAreaUV.Add(friendUV1);
                //    friendAreaUV.Add(sliceP1UV);
                //    friendAreaUV.Add(friendUV2);

                //    friendAreaTri.Add(newFriendTri);
                //    friendAreaTri.Add(newFriendTri + 1);
                //    friendAreaTri.Add(newFriendTri + 2);

                //    // p2쪽
                //    friendAreaVertex.Add(friendV2);
                //    friendAreaVertex.Add(sliceP1);
                //    friendAreaVertex.Add(sliceP2);

                //    friendAreaNormal.Add(friendNormal2);
                //    friendAreaNormal.Add(sliceP1Normal);
                //    friendAreaNormal.Add(sliceP2Normal);

                //    friendAreaUV.Add(friendUV2);
                //    friendAreaUV.Add(sliceP1UV);
                //    friendAreaUV.Add(sliceP2UV);

                //    friendAreaTri.Add(newFriendTri + 3);
                //    friendAreaTri.Add(newFriendTri + 4);
                //    friendAreaTri.Add(newFriendTri + 5);
                //}
                //else {
                //    lonelyAreaVertex.Add(lonelyV);
                //    lonelyAreaVertex.Add(sliceP1);
                //    lonelyAreaVertex.Add(sliceP2);

                //    lonelyAreaNormal.Add(lonelyNormal);
                //    lonelyAreaNormal.Add(sliceP1Normal);
                //    lonelyAreaNormal.Add(sliceP2Normal);

                //    lonelyAreaUV.Add(lonelyUV);
                //    lonelyAreaUV.Add(sliceP1UV);
                //    lonelyAreaUV.Add(sliceP2UV);

                //    lonelyAreaTri.Add(newLonelyTri);
                //    lonelyAreaTri.Add(newLonelyTri + 1);
                //    lonelyAreaTri.Add(newLonelyTri + 2);

                //    // 친구있는 놈들
                //    // p1쪽
                //    friendAreaVertex.Add(friendV1);
                //    friendAreaVertex.Add(friendV2);
                //    friendAreaVertex.Add(sliceP1);

                //    friendAreaNormal.Add(friendNormal1);
                //    friendAreaNormal.Add(friendNormal2);
                //    friendAreaNormal.Add(sliceP1Normal);

                //    friendAreaUV.Add(friendUV1);
                //    friendAreaUV.Add(friendUV2);
                //    friendAreaUV.Add(sliceP1UV);

                //    friendAreaTri.Add(newFriendTri);
                //    friendAreaTri.Add(newFriendTri + 1);
                //    friendAreaTri.Add(newFriendTri + 2);

                //    // p2쪽
                //    friendAreaVertex.Add(friendV2);
                //    friendAreaVertex.Add(sliceP2);
                //    friendAreaVertex.Add(sliceP1);

                //    friendAreaNormal.Add(friendNormal2);
                //    friendAreaNormal.Add(sliceP2Normal);
                //    friendAreaNormal.Add(sliceP1Normal);

                //    friendAreaUV.Add(friendUV2);
                //    friendAreaUV.Add(sliceP2UV);
                //    friendAreaUV.Add(sliceP1UV);

                //    friendAreaTri.Add(newFriendTri + 3);
                //    friendAreaTri.Add(newFriendTri + 4);
                //    friendAreaTri.Add(newFriendTri + 5);
                //}
                #endregion

                lonelyAreaVertex.Add(lonelyV);
                lonelyAreaVertex.Add(sliceP1);
                lonelyAreaVertex.Add(sliceP2);

                lonelyAreaNormal.Add(lonelyNormal);
                lonelyAreaNormal.Add(sliceP1Normal);
                lonelyAreaNormal.Add(sliceP2Normal);

                lonelyAreaUV.Add(lonelyUV);
                lonelyAreaUV.Add(sliceP1UV);
                lonelyAreaUV.Add(sliceP2UV);

                lonelyAreaTri.Add(newLonelyTri);
                lonelyAreaTri.Add(newLonelyTri + 1);
                lonelyAreaTri.Add(newLonelyTri + 2);

                // 친구있는 놈들
                // p1쪽
                friendAreaVertex.Add(friendV1);
                friendAreaVertex.Add(friendV2);
                friendAreaVertex.Add(sliceP1);

                friendAreaNormal.Add(friendNormal1);
                friendAreaNormal.Add(friendNormal2);
                friendAreaNormal.Add(sliceP1Normal);

                friendAreaUV.Add(friendUV1);
                friendAreaUV.Add(friendUV2);
                friendAreaUV.Add(sliceP1UV);

                friendAreaTri.Add(newFriendTri);
                friendAreaTri.Add(newFriendTri + 1);
                friendAreaTri.Add(newFriendTri + 2);

                // p2쪽
                friendAreaVertex.Add(friendV2);
                friendAreaVertex.Add(sliceP2);
                friendAreaVertex.Add(sliceP1);

                friendAreaNormal.Add(friendNormal2);
                friendAreaNormal.Add(sliceP2Normal);
                friendAreaNormal.Add(sliceP1Normal);

                friendAreaUV.Add(friendUV2);
                friendAreaUV.Add(sliceP2UV);
                friendAreaUV.Add(sliceP1UV);

                friendAreaTri.Add(newFriendTri + 3);
                friendAreaTri.Add(newFriendTri + 4);
                friendAreaTri.Add(newFriendTri + 5);
                if (lonelyInPositive == 1) {
                    positiveMesh.AddTriangle(0, lonelyAreaTri, lonelyAreaVertex, lonelyAreaNormal, lonelyAreaUV);
                    negativeMesh.AddTriangle(0, friendAreaTri, friendAreaVertex, friendAreaNormal, friendAreaUV);
                }
                else {
                    negativeMesh.AddTriangle(0, lonelyAreaTri, lonelyAreaVertex, lonelyAreaNormal, lonelyAreaUV);
                    positiveMesh.AddTriangle(0, friendAreaTri, friendAreaVertex, friendAreaNormal, friendAreaUV);
                }

                edges.Add(new MeshEdge(sliceP1, sliceP2, sliceP1UV, sliceP2UV));

                // Debug.Log(sliceP1 + " " + sliceP2);
            }
        }
        // 뚜껑 덮을 차례
        sortEdges.Add(edges[3]);
        edges.RemoveAt(3);
        int safe = 0;

        while (edges.Count > 0 && safe < 100) {
            safe++;
            // Debug.Log(sortEdges.Count);
            int prevCount = edges.Count;
            for (int i = 0; i < edges.Count; i++) {
                MeshEdge insertEdge = edges[i];
                for (int k = 0; k < sortEdges.Count; k++) {
                    MeshEdge edge2 = sortEdges[k];
                    if (insertEdge == edge2)
                        continue;
                    if (Vector3.Distance(insertEdge.p1, edge2.p1) < 0.0001f) {
                        insertEdge.Reverse();
                        sortEdges.Insert(k, insertEdge);
                        edges.RemoveAt(i--);
                        break;
                    }
                    else if (Vector3.Distance(insertEdge.p1, edge2.p2) < 0.0001f) {
                        sortEdges.Insert(k + 1, insertEdge);
                        edges.RemoveAt(i--);
                        break;
                    }
                    else if (Vector3.Distance(insertEdge.p2, edge2.p1) < 0.0001f) {
                        sortEdges.Insert(k, insertEdge);
                        edges.RemoveAt(i--);
                        break;
                    }
                    else if (Vector3.Distance(insertEdge.p2, edge2.p2) < 0.0001f) {
                        insertEdge.Reverse();
                        sortEdges.Insert(k + 1, insertEdge);
                        edges.RemoveAt(i--);
                        break;
                    }
                }
            }
            if (prevCount == edges.Count)
                break;
        }
        //Debug.Log(edges.Count);
        //for (int k = 0; k < sortEdges.Count; k++) {
        //    Debug.Log(sortEdges[k].p1 + " " + sortEdges[k].p2);
        //}
        List<PartMesh> partMeshs = new List<PartMesh>();
        positiveMesh.test = sortEdges;
        List<Vector3> vec3Coord = new List<Vector3>();
        for(int i =0;i < sortEdges.Count; i++) {
            vec3Coord.Add(sortEdges[i].p1);
        }
        
        List<Vector2> vec2Coord = MyGeometry.Vec3CoordToVec2Coord(vec3Coord, slicePoint, sliceNormal);
        List<Vector2> vec2Coord2 = new List<Vector2>();
        for (int i = 0; i < vec2Coord.Count; i++) {
            Vector2 vec2 = vec2Coord[i];
            vec2Coord2.Add(vec2);
        }
        positiveMesh.vec2Coord = vec2Coord2;

        // 정점에 인자로 넣었던 vec2Coord 의 요소 번호를 더 저정해놓은 triangle 
        // vec2Coord 인자번호 == sortMeshs 번호
        List<IndexTriangle> indexTriangles1 = MyGeometry.EarClippingIndexTriangle(vec2Coord, true);
        List<IndexTriangle> indexTriangles2 = MyGeometry.EarClippingIndexTriangle(vec2Coord, false);

        MyGeometry.EarClippingAlgorithm(vec2Coord, true);
        MyGeometry.EarClippingAlgorithm(vec2Coord, false);

        List<IndexTriangle> indexTriangles = indexTriangles1.Count > indexTriangles2.Count ? indexTriangles1 : indexTriangles2;
        positiveMesh.triangle2D = indexTriangles;
        for(int i =0;i < indexTriangles.Count; i++) {
            IndexTriangle indexTriangle = indexTriangles[i];
            List<int> positiveSlicePlaneTri = new List<int>();
            List<Vector3> positiveSlicePlaneV = new List<Vector3>();
            List<Vector3> positiveSlicePlaneNormal = new List<Vector3>();
            List<Vector2> positiveSlicePlaneUV = new List<Vector2>();

            List<int> negativeSlicePlaneTri = new List<int>();
            List<Vector3> negativeSlicePlaneV = new List<Vector3>();
            List<Vector3> negativeSlicePlaneNormal = new List<Vector3>();
            List<Vector2> negativeSlicePlaneUV = new List<Vector2>();

            //반시계 방향
            if (MyGeometry.CCW(indexTriangle.p1.p, indexTriangle.p2.p, indexTriangle.p3.p) < 0) {
                int i0 = indexTriangle.p1.index;
                int i1 = indexTriangle.p2.index;
                int i2 = indexTriangle.p3.index;
                
                negativeSlicePlaneV.Add(sortEdges[i0].p1);
                negativeSlicePlaneV.Add(sortEdges[i1].p1);
                negativeSlicePlaneV.Add(sortEdges[i2].p1);
                negativeSlicePlaneNormal.Add(sliceNormal);
                negativeSlicePlaneNormal.Add(sliceNormal);
                negativeSlicePlaneNormal.Add(sliceNormal);
                negativeSlicePlaneUV.Add(sortEdges[i0].p1UV);
                negativeSlicePlaneUV.Add(sortEdges[i1].p1UV);
                negativeSlicePlaneUV.Add(sortEdges[i2].p1UV);
                negativeSlicePlaneTri.Add(negativeMesh.triangles[0].Count);
                negativeSlicePlaneTri.Add(negativeMesh.triangles[0].Count + 1);
                negativeSlicePlaneTri.Add(negativeMesh.triangles[0].Count + 2);

                i0 = indexTriangle.p1.index;
                i1 = indexTriangle.p3.index;
                i2 = indexTriangle.p2.index;
                positiveSlicePlaneV.Add(sortEdges[i0].p1);
                positiveSlicePlaneV.Add(sortEdges[i1].p1);
                positiveSlicePlaneV.Add(sortEdges[i2].p1);
                positiveSlicePlaneNormal.Add(-sliceNormal);
                positiveSlicePlaneNormal.Add(-sliceNormal);
                positiveSlicePlaneNormal.Add(-sliceNormal);
                positiveSlicePlaneUV.Add(sortEdges[i0].p1UV);
                positiveSlicePlaneUV.Add(sortEdges[i1].p1UV);
                positiveSlicePlaneUV.Add(sortEdges[i2].p1UV);
                positiveSlicePlaneTri.Add(positiveMesh.triangles[0].Count);
                positiveSlicePlaneTri.Add(positiveMesh.triangles[0].Count + 1);
                positiveSlicePlaneTri.Add(positiveMesh.triangles[0].Count + 2);

                positiveMesh.triangle3D.Add(sortEdges[i0].p1);
                positiveMesh.triangle3D.Add(sortEdges[i1].p1);
                positiveMesh.triangle3D.Add(sortEdges[i2].p1);
            }
            else {
                int i0 = indexTriangle.p1.index;
                int i1 = indexTriangle.p2.index;
                int i2 = indexTriangle.p3.index;
                positiveSlicePlaneV.Add(sortEdges[i0].p1);
                positiveSlicePlaneV.Add(sortEdges[i1].p1);
                positiveSlicePlaneV.Add(sortEdges[i2].p1);
                positiveSlicePlaneNormal.Add(-sliceNormal);
                positiveSlicePlaneNormal.Add(-sliceNormal);
                positiveSlicePlaneNormal.Add(-sliceNormal);
                positiveSlicePlaneUV.Add(sortEdges[i0].p1UV);
                positiveSlicePlaneUV.Add(sortEdges[i1].p1UV);
                positiveSlicePlaneUV.Add(sortEdges[i2].p1UV);
                positiveSlicePlaneTri.Add(positiveMesh.triangles[0].Count);
                positiveSlicePlaneTri.Add(positiveMesh.triangles[0].Count + 1);
                positiveSlicePlaneTri.Add(positiveMesh.triangles[0].Count + 2);

                                positiveMesh.triangle3D.Add(sortEdges[i0].p1);
                positiveMesh.triangle3D.Add(sortEdges[i1].p1);
                positiveMesh.triangle3D.Add(sortEdges[i2].p1);

                i0 = indexTriangle.p1.index;
                i1 = indexTriangle.p3.index;
                i2 = indexTriangle.p2.index;

                negativeSlicePlaneV.Add(sortEdges[i0].p1);
                negativeSlicePlaneV.Add(sortEdges[i1].p1);
                negativeSlicePlaneV.Add(sortEdges[i2].p1);
                negativeSlicePlaneNormal.Add(sliceNormal);
                negativeSlicePlaneNormal.Add(sliceNormal);
                negativeSlicePlaneNormal.Add(sliceNormal);
                negativeSlicePlaneUV.Add(sortEdges[i0].p1UV);
                negativeSlicePlaneUV.Add(sortEdges[i1].p1UV);
                negativeSlicePlaneUV.Add(sortEdges[i2].p1UV);
                negativeSlicePlaneTri.Add(negativeMesh.triangles[0].Count);
                negativeSlicePlaneTri.Add(negativeMesh.triangles[0].Count + 1);
                negativeSlicePlaneTri.Add(negativeMesh.triangles[0].Count + 2);
            }
            negativeMesh.AddTriangle(0, negativeSlicePlaneTri, negativeSlicePlaneV, negativeSlicePlaneNormal, negativeSlicePlaneUV);
            positiveMesh.AddTriangle(0, positiveSlicePlaneTri, positiveSlicePlaneV, positiveSlicePlaneNormal, positiveSlicePlaneUV);
        }
        partMeshs.Add(positiveMesh);
        partMeshs.Add(negativeMesh);
        return partMeshs;
    }
    static bool SameSign(float a, float b) {
        return Mathf.Sign(a) == Mathf.Sign(b);
    }
}
public class MeshEdge {
    public Vector3 p1;
    public Vector2 p1UV;

    public Vector3 p2;
    public Vector2 p2UV;

    public MeshEdge(Vector3 p1, Vector3 p2, Vector2 p1UV, Vector2 p2UV) {
        this.p1 = p1;
        this.p1UV = p1UV;
        this.p2 = p2;
        this.p2UV = p2UV;
    }

    public void Reverse() {
        Vector3 temp1 = p1;
        p1 = p2;
        p2 = temp1;

        Vector3 temp2 = p1UV;
        p1UV = p2UV;
        p2UV = temp2;
    }
}
public class PartMesh
{
    public List<Vector3> vertices = new List<Vector3>();
    public List<Vector3> normals = new List<Vector3>();
    public List<List<int>> triangles = new List<List<int>>();
    public List<Vector2> uvs = new List<Vector2>();
    public List<MeshEdge> test = new List<MeshEdge>();
    public List<Vector3> newVertexs = new List<Vector3>();
    public List<Vector2> vec2Coord = new List<Vector2>();
    public List<IndexTriangle> triangle2D = new List<IndexTriangle>();
    public List<Vector3> triangle3D = new List<Vector3>();

    Bounds bounds = new Bounds();

    Mesh origMesh;
    int[] origTriangles;
    Vector3[] origVertices;
    Vector2[] origUV;
    GameObject origObject;
    public PartMesh(MeshRenderer renderer, MeshFilter filter) {
        origMesh = filter.mesh;
        origTriangles = origMesh.triangles;
        origVertices = origMesh.vertices;
        origUV = origMesh.uv;
        origObject = renderer.gameObject;

        triangles.Add(new List<int>());
        triangles.Add(new List<int>());
    }
    public void Slice(Vector3 slicePoint,Vector3 sliceNormal) {

    }
    public void AddTriangle(int submesh,List<int> tris ,List<Vector3> newVertexs, List<Vector3> newNormals, List<Vector2> newUvs) {
        for(int i =0;i < tris.Count; i++) {
            int tri = tris[i];
            triangles[submesh].Add(tri);
        }
        for (int i = 0; i < newVertexs.Count; i++) {
            Vector3 vertex = newVertexs[i];
            vertices.Add(vertex);
            this.newVertexs.Add(vertex);
        }
        for (int i = 0; i < newNormals.Count; i++) {
            Vector3 normal = newNormals[i];
            normals.Add(normal);
        }
        for (int i = 0; i < newUvs.Count; i++) {
            Vector2 uv = newUvs[i];
            uvs.Add(uv);
        }
    }

    public void AddTriangle(int submesh, Vector3 vert1, Vector3 vert2, Vector3 vert3,
        Vector3 normal1, Vector3 normal2, Vector3 normal3, Vector2 uv1, Vector2 uv2, Vector2 uv3) {
        triangles[submesh].Add(vertices.Count);
        vertices.Add(vert1);
        triangles[submesh].Add(vertices.Count);
        vertices.Add(vert2);
        triangles[submesh].Add(vertices.Count);
        vertices.Add(vert3);

        normals.Add(normal1);
        normals.Add(normal2);
        normals.Add(normal3);

        uvs.Add(uv1);
        uvs.Add(uv2);
        uvs.Add(uv3);

        bounds.min = Vector3.Min(bounds.min, vert1);
        bounds.min = Vector3.Min(bounds.min, vert2);
        bounds.min = Vector3.Min(bounds.min, vert3);
        bounds.max = Vector3.Max(bounds.max, vert1);
        bounds.max = Vector3.Max(bounds.max, vert2);
        bounds.max = Vector3.Max(bounds.max, vert3);

    }

    public void FillArrays() {

    }

    public GameObject MakeGameObject() {
        GameObject original = origObject;
        GameObject obj = new GameObject();
        obj.transform.position = original.transform.position;
        obj.transform.rotation = original.transform.rotation;
        obj.transform.localScale = original.transform.localScale;

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uvs.ToArray();
        //mesh.SetTriangles(triangles[0], 0, true);
        mesh.triangles = triangles[0].ToArray();
        //for (int i =0;i < triangles.Count; i++) {
        //    mesh.SetTriangles(triangles[i], i, true);
        //}
        bounds = mesh.bounds;
        MeshRenderer renderer = obj.AddComponent<MeshRenderer>();
        renderer.material = original.GetComponent<MeshRenderer>().material;

        MeshFilter filter = obj.AddComponent<MeshFilter>();
        filter.mesh =mesh;
        return obj;
    }
}
