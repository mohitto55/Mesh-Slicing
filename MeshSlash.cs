using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshSlash : MonoBehaviour
{
    List<MeshEdge> test1 = new List<MeshEdge>();
    List<Vector3> test = new List<Vector3>();
    List<Vector2> vec2Coord = new List<Vector2>();
    List<IndexTriangle> triangle2D = new List<IndexTriangle>();
    List<Vector3> triangle3D = new List<Vector3>();
    public Vector3 cut;
    public Vector3 normal;
    GameObject b;

    bool tb;
    private void Update() {
        if (Time.time >= 5 && !tb) {
            Generate();
            tb = true;
            Destroy(transform.gameObject);
        }
    }
    public void Generate() {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        MeshFilter filter = GetComponent<MeshFilter>();
        List<PartMesh> meshs = PartMeshUtil.SlashMEsh(renderer, filter, transform.position + cut, normal.normalized);
        for(int i =0; i < meshs.Count; i++) {
            GameObject sliceMesh = meshs[i].MakeGameObject();
            if (meshs[i].test.Count > test.Count) {
                test = meshs[i].newVertexs;
                test1 = meshs[i].test;
                vec2Coord = meshs[i].vec2Coord;
                triangle2D = meshs[i].triangle2D;
                triangle3D = meshs[i].triangle3D;
                b = sliceMesh;
            }
        }
    }
    private void OnDrawGizmos() {
        if (b) {
            //for(int i =0;i < test.Count; i++) {
            //    Gizmos.DrawSphere(test[i] + b.transform.position, 0.015f);
            //}

            for (int i = 0; i < test1.Count; i++) {
                float color = (float)i / test1.Count;
                Gizmos.color = new Color(color, color, color);
                Gizmos.DrawSphere(test1[i].p1 + transform.position, 0.005f);
                Gizmos.color = Color.green;
                int next = i + 1 < test1.Count ? i + 1 : 0;
                Gizmos.DrawLine(test1[i].p1 + transform.position, test1[i].p2 + transform.position);
            }
            for (int i = 0; i < vec2Coord.Count; i++) {
                float color = (float)i / test1.Count;
                Gizmos.color = new Color(color, color, color);
                Gizmos.DrawSphere(vec2Coord[i], 0.005f);

                int next = i + 1 < vec2Coord.Count ? i + 1 : 0;
                Gizmos.color = Color.green;
                Gizmos.DrawLine(vec2Coord[i], vec2Coord[next]);
            }
            for (int i = 0; i < triangle2D.Count; i++) {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(triangle2D[i].p1.p, triangle2D[i].p2.p);
                Gizmos.DrawLine(triangle2D[i].p2.p, triangle2D[i].p3.p);
                Gizmos.DrawLine(triangle2D[i].p3.p, triangle2D[i].p1.p);
            }
            for (int i = 0; i < triangle3D.Count - 1; i++) {
                Gizmos.DrawLine(triangle3D[i], triangle3D[i + 1]);
            }
        }
    }
}
