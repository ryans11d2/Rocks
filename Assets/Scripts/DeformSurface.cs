using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeformSurface : MonoBehaviour
{
    
    Mesh mesh;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;

        Vector3[] scaleVerts = new Vector3[mesh.vertexCount];
        for (int i = 0; i < mesh.vertexCount; i++) {
            scaleVerts[i].x = mesh.vertices[i].x * transform.localScale.x;
            scaleVerts[i].y = mesh.vertices[i].y * transform.localScale.y;
            scaleVerts[i].z = mesh.vertices[i].z * transform.localScale.z;
        }
        mesh.SetVertices(scaleVerts);
        transform.localScale = Vector3.one;

        for (int i = 0; i < mesh.triangles.Length; i += 3) {
            //Debug.Log("Triangle " + (i / 3));
            //Debug.Log(Vector3.Distance(mesh.vertices[mesh.triangles[i]], mesh.vertices[mesh.triangles[i + 1]]));
            //Debug.Log(Vector3.Distance(mesh.vertices[mesh.triangles[i]], mesh.vertices[mesh.triangles[i + 2]]));
            //Debug.Log(Vector3.Distance(mesh.vertices[mesh.triangles[i + 1]], mesh.vertices[mesh.triangles[i + 2]]));
            //If the distance between to vertices of a triangle is too big, create a new vertex at the half way point and have the triangles use that instead
        }

        for (int i = 0; i < mesh.normals.Length; i++) {
            //Debug.Log(mesh.normals[i]);
        }

        SetNormals();
        //SetTriangles();

        for (int i = 0; i < mesh.vertices.Length - 1; i++) {
            Debug.Log(Vector3.Distance(mesh.vertices[i], mesh.vertices[i + 1]));
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Deform(Vector3 pos, Vector3 dir, float power)
    {

        Debug.Log("Deform " + dir + " " + power);
        
        pos.x /= transform.localScale.x;
        pos.y /= transform.localScale.y;
        pos.z /= transform.localScale.z;

        Vector3 localPos = pos - transform.position;
        

        Vector3[] newVerts = new Vector3[mesh.vertexCount];
        Vector3[] newNorm = new Vector3[mesh.vertexCount];

        for (int i = 0; i < mesh.vertexCount; i++) {
            Vector3 newPos = mesh.vertices[i];
            
            //power /= transform.localScale.x;
            float push = Mathf.Clamp(Mathf.Abs(power) - Vector3.Distance(mesh.vertices[i], localPos), 0, 1);
            

            if (power > 0) newPos += dir * (push * push * (3.0f - 2.0f * push));
            else newPos -= dir * (push * push * (3.0f - 2.0f * push));

            if (Vector3.Distance(mesh.vertices[i], pos) <= power)
                newNorm[i] = mesh.normals[i] + new Vector3(0, 0.1f, 0);

            newVerts[i] = newPos;

        } 

        mesh.SetVertices(newVerts);
        //mesh.SetNormals(newNorm);

        SetNormals();
        //SetTriangles();

        for (int i = 0; i < mesh.vertices.Length - 1; i++) {
            Debug.Log(Vector3.Distance(mesh.vertices[i], mesh.vertices[i + 1]));
        }


    }

    private void SetNormals() {

        Vector3[] newNorm = new Vector3[mesh.vertexCount];
        
        for (int i = 0; i < newNorm.Length; i++) newNorm[i] = Vector3.zero;

        for (int i = 0; i < mesh.vertexCount; i++) {
            Vector3[] tri = new Vector3[3];
            tri[0] = mesh.vertices[mesh.triangles[i * 3]];
            tri[1] = mesh.vertices[mesh.triangles[i * 3 + 1]];
            tri[2] = mesh.vertices[mesh.triangles[i * 3 + 2]];
            
            //Debug.Log(tri[0] + ", " + tri[1] + ", " + tri[2]);

            Vector3 aer;

            aer = Vector3.Cross(tri[0], tri[1]);
            newNorm[mesh.triangles[i * 3]] += aer;
            newNorm[mesh.triangles[i * 3 + 1]] += aer;
            newNorm[mesh.triangles[i * 3 + 2]] += aer;

            aer = Vector3.Cross(tri[1], tri[2]);
            newNorm[mesh.triangles[i * 3]] += aer;
            newNorm[mesh.triangles[i * 3 + 1]] += aer;
            newNorm[mesh.triangles[i * 3 + 2]] += aer;

            aer = Vector3.Cross(tri[2], tri[0]);
            newNorm[mesh.triangles[i * 3]] += aer;
            newNorm[mesh.triangles[i * 3 + 1]] += aer;
            newNorm[mesh.triangles[i * 3 + 2]] += aer;

            //newNorm[i] = Vector3.Normalize(Vector3.Cross(tri[1] - tri[0], tri[2] - tri[0]));

        } 

        for (int i = 0; i < newNorm.Length; i++) {
            newNorm[i] = Vector3.Normalize(newNorm[i]);
            //Debug.Log(newNorm[i]);
        }

        mesh.SetNormals(newNorm);

    }

    private void SetTriangles() {

        for (int i = 0; i < mesh.vertexCount; i++) {
            Vector3[] tri = new Vector3[3];
            tri[0] = mesh.vertices[mesh.triangles[i * 3]];
            tri[1] = mesh.vertices[mesh.triangles[i * 3 + 1]];
            tri[2] = mesh.vertices[mesh.triangles[i * 3 + 2]];
            
            if (Vector3.Distance(mesh.vertices[mesh.triangles[i]], mesh.vertices[mesh.triangles[i + 1]]) > 1) SplitTriangle(mesh.triangles[i], mesh.triangles[i + 1]);
            if (Vector3.Distance(mesh.vertices[mesh.triangles[i + 1]], mesh.vertices[mesh.triangles[i + 2]]) > 1) SplitTriangle(mesh.triangles[i + 1], mesh.triangles[i + 2]);
            if (Vector3.Distance(mesh.vertices[mesh.triangles[i + 2]], mesh.vertices[mesh.triangles[i]]) > 1) SplitTriangle(mesh.triangles[i + 2], mesh.triangles[i]);

        }
        
    }

    private void SplitTriangle(int pointA, int pointB) {

        Vector3 newPoint = Vector3.Lerp(mesh.vertices[pointA], mesh.vertices[pointB], 0.5f);

        int newIdx = mesh.vertexCount;
        Vector3[] newVerts = new Vector3[newIdx + 1];

        for (int i = 0; i < mesh.vertexCount; i++) {
            newVerts[i] = mesh.vertices[i];
        }
        newVerts[newIdx] = newPoint;

        int[] newTri = mesh.triangles;

        for (int i = 0; i < mesh.vertexCount; i++) {
            int[] tri = new int[3];
            tri[0] = mesh.triangles[i * 3];
            tri[1] = mesh.triangles[i * 3 + 1];
            tri[2] = mesh.triangles[i * 3 + 2];
            
            if (tri[0] == pointA && tri[1] == pointB) tri[1] = newIdx;
            else if (tri[1] == pointA && tri[2] == pointB) tri[2] = newIdx;
            else if (tri[2] == pointA && tri[0] == pointB) tri[0] = newIdx;

            newTri[i * 3] = tri[0];
            newTri[i * 3 + 1] = tri[1];
            newTri[i * 3 + 2] = tri[2];

        }

        mesh.SetVertices(newVerts);
        //mesh.SetTriangles(newTri, 0);

    }


}
