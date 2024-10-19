using AdvancedEditorTools.Attributes;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UIElements;

public class Fax : MonoBehaviour
{
    [SerializeField] private SplineContainer spline;

    [SerializeField] private float width, length;
    [SerializeField, Min(2)] private int resolution;
    [SerializeField] private TMP_Text textField;

    private MeshFilter meshFilter;
    private Mesh mesh;

    private List<Vector3> vertices;
    private List<Vector3> normals;
    private List<Vector2> uvs;
    private List<int> triangles;
    private float splineLength;
    private float stepSize;
    private float trueOffset;
    private Tween tween;
    private float offset;


    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();

        vertices = new List<Vector3>();
        normals = new List<Vector3>();
        uvs = new List<Vector2>();
        triangles = new List<int>();
        BuildMesh();
        
        offset = -length;
    }

    private void OnValidate()
    {
        if (Application.isPlaying) return;

        meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();

        vertices = new List<Vector3>();
        normals = new List<Vector3>();
        uvs = new List<Vector2>();
        triangles = new List<int>();
        BuildMesh();
    }

    private void Update()
    {
        MoveMesh();
    }

    private void MoveMesh()
    {
        trueOffset = offset / splineLength;

        for (int i = 0; i < resolution + 1; i++)
        {
            MoveSection(i, transform.InverseTransformPoint(spline.EvaluatePosition(trueOffset + stepSize * i)));
        }

        mesh.SetVertices(vertices);

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }

    private void BuildMesh()
    {
        vertices.Clear(); 
        normals.Clear(); 
        uvs.Clear(); 
        triangles.Clear();

        splineLength = spline.CalculateLength();
        stepSize = (length / resolution) / splineLength;
        trueOffset = offset / splineLength;

        float w = width * 0.5f;

        Vector3 first = transform.InverseTransformPoint(spline.EvaluatePosition(trueOffset));
        vertices.Add(first + (w * Vector3.right));
        vertices.Add(first + (w * Vector3.left));

        uvs.Add(new Vector2(0, 1));
        uvs.Add(new Vector2(1, 1));

        for (int i = 1; i < resolution + 1; i++)
        {
            AddSection(i, transform.InverseTransformPoint(spline.EvaluatePosition(trueOffset + stepSize * i)));
        }

        mesh.SetVertices(vertices);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(triangles, 0);
        

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }

    private void AddSection(int index, Vector3 position)
    {
        float w = width * 0.5f;

        vertices.Add(position + (w * Vector3.right));
        vertices.Add(position + (w * Vector3.left));

        uvs.Add(new Vector2(0, 1 - ((float)index) / (float)resolution));
        uvs.Add(new Vector2(1, 1 - ((float)index) / (float)resolution));

        triangles.Add(index * 2 - 2);
        triangles.Add(index * 2 - 1);
        triangles.Add(index * 2);

        triangles.Add(index * 2 - 1);
        triangles.Add(index * 2 + 1);
        triangles.Add(index * 2);
    }

    private void MoveSection(int index, Vector3 position)
    {
        float w = width * 0.5f;

        vertices[index * 2] = position + w * Vector3.right;
        vertices[index * 2 + 1] = position + w * Vector3.left;
    }

    [Button("Print")]
    public void Print(string text)
    {
        textField.text = text;
        offset = -length;
        if (tween != null) tween.Kill();
        tween = DOTween.To(() => offset, x => offset = x, 0f, 3f).SetEase(Ease);
    }

    [Button("Detach")]
    public void Detach()
    {
        if(tween != null) tween.Kill();
        tween = DOTween.To(() => offset, x => offset = x, splineLength, 4f).SetEase(DG.Tweening.Ease.InCubic).SetSpeedBased(true);
    }

    private float Ease(float t, float duration, float amplitude, float period)
    {
        var x = t / duration;

        var r = 30;
        var h = Mathf.Floor(r * x) * 0.5f;
        var g = Mathf.Max((r * x % 1) - 0.5f, 0);

        return (h + g) / (r * 0.5f);
    }
}
