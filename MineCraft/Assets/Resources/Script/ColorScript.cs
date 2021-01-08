using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ColorScript : MonoBehaviour
{
    public Material mMaterial;

    // Start is called before the first frame update
    void Start()
    {
        InitializeRenderer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void InitializeRenderer()
    {
        GetComponent<MeshRenderer>().material = mMaterial;

        Mesh mMesh = GetComponent<MeshFilter>().mesh;

        mMesh.Clear();

        mMesh.vertices = new Vector3[] {
            //front
            new Vector3( 0.5f, -0.5f, -0.5f),
            new Vector3( 0.5f, -0.5f,  0.5f),
            new Vector3( 0.5f,  0.5f,  0.5f),
            new Vector3( 0.5f,  0.5f, -0.5f),

            //back
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3(-0.5f,  0.5f, -0.5f),
            new Vector3(-0.5f,  0.5f,  0.5f),
            new Vector3(-0.5f, -0.5f,  0.5f),

            //top
            new Vector3(-0.5f,  0.5f, -0.5f),
            new Vector3( 0.5f,  0.5f, -0.5f),
            new Vector3( 0.5f,  0.5f,  0.5f),
            new Vector3(-0.5f,  0.5f,  0.5f),

            //bottom
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3(-0.5f, -0.5f,  0.5f),
            new Vector3( 0.5f, -0.5f,  0.5f),
            new Vector3( 0.5f, -0.5f, -0.5f),

            //left
            new Vector3(-0.5f, -0.5f,  0.5f),
            new Vector3(-0.5f,  0.5f,  0.5f),
            new Vector3( 0.5f,  0.5f,  0.5f),
            new Vector3( 0.5f, -0.5f,  0.5f),

            //right
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3( 0.5f, -0.5f, -0.5f),
            new Vector3( 0.5f,  0.5f, -0.5f),
            new Vector3(-0.5f,  0.5f, -0.5f)
        };

        /*
        mMesh.triangles = new int[] {
            //front
            0, 1, 2,
            2, 3, 0,
            //back
            7, 4, 5,
            5, 6, 7,
            //top
            11, 8, 9,
            9, 10, 11,
            //bottom
            12, 13, 14,
            14, 15, 12,
            //left
            16, 17, 18,
            18, 19, 16,
            //right
            23, 20, 21,
            21, 22, 23
        };*/
        
        mMesh.triangles = new int[] {
            //front
            0, 2, 1,
            2, 0, 3,
            //back
            7, 5, 4,
            5, 7, 6,
            //top
            11, 9, 8,
            9, 11, 10,
            //bottom
            12, 14, 13,
            14, 12, 15,
            //left
            16, 18, 17,
            18, 16, 19,
            //right
            23, 21, 20,
            21, 23, 22
        };

        mMesh.uv = SetUV();

        mMesh.RecalculateNormals();
    }

    Vector2[] SetUV()
    {
        float t = 1/3f;

        Vector2[] tempUV = new Vector2[]
        {
            //front
            new Vector2(  0,  0),  //( 0.5f, -0.5f, -0.5f),
            new Vector2(  t,  0), //( 0.5f, -0.5f,  0.5f),
            new Vector2(  t,  t), //( 0.5f,  0.5f,  0.5f),
            new Vector2(  0,  t), //( 0.5f,  0.5f, -0.5f),

            //back
            new Vector2(2*t,  0), //(-0.5f, -0.5f, -0.5f),
            new Vector2(2*t,  t), //(-0.5f,  0.5f, -0.5f),
            new Vector2(  t,  t), //(-0.5f,  0.5f,  0.5f),
            new Vector2(  t,  0), //(-0.5f, -0.5f,  0.5f),

            //top
            new Vector2(2*t,  t), //(-0.5f,  0.5f, -0.5f),
            new Vector2(2*t,  0), //( 0.5f,  0.5f, -0.5f),
            new Vector2(3*t,  0), //( 0.5f,  0.5f,  0.5f),
            new Vector2(3*t,  t), //(-0.5f,  0.5f,  0.5f),

            //bottom
            new Vector2(  0,  t), //(-0.5f, -0.5f, -0.5f),
            new Vector2(  t,  t), //(-0.5f, -0.5f,  0.5f),
            new Vector2(  t,2*t), //( 0.5f, -0.5f,  0.5f),
            new Vector2(  0,2*t), //( 0.5f, -0.5f, -0.5f),

            //left
            new Vector2(2*t,  t), //(-0.5f, -0.5f,  0.5f),
            new Vector2(2*t,2*t), //(-0.5f,  0.5f,  0.5f),
            new Vector2(  t,2*t), //( 0.5f,  0.5f,  0.5f),
            new Vector2(  t,  t), //( 0.5f, -0.5f,  0.5f),

            //right
            new Vector2(2*t,  t), //(-0.5f, -0.5f, -0.5f),
            new Vector2(3*t,  t), //( 0.5f, -0.5f, -0.5f),
            new Vector2(3*t,2*t), //( 0.5f,  0.5f, -0.5f),
            new Vector2(2*t,2*t)  //(-0.5f,  0.5f, -0.5f)

        };

        return tempUV;
    }
}
