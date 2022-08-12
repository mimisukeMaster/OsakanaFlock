using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MediaPipe.HandPose {

public sealed class HandAnimator : MonoBehaviour
{
    #region Editable attributes

    [SerializeField] WebcamInput _webcam = null;
    [SerializeField] ResourceSet _resources = null;
    [SerializeField] bool _useAsyncReadback = true;
    [Space]
    [SerializeField] Mesh _jointMesh = null;
    [SerializeField] Mesh _boneMesh = null;
    [Space]
    [SerializeField] Material _jointMaterial = null;
    [SerializeField] Material _boneMaterial = null;
    [Space]
    [SerializeField] RawImage _monitorUI = null;

    #endregion

    #region Private members

    HandPipeline _pipeline;

    public Material BallColor;
        private Material[] MyBallColor;
    static readonly (int, int)[] BonePairs =
    {
        (0, 1), (1, 2), (1, 2), (2, 3), (3, 4),     // Thumb
        (5, 6), (6, 7), (7, 8),                     // Index finger
        (9, 10), (10, 11), (11, 12),                // Middle finger
        (13, 14), (14, 15), (15, 16),               // Ring finger
        (17, 18), (18, 19), (19, 20),               // Pinky
        (0, 17), (2, 5), (5, 9), (9, 13), (13, 17)  // Palm
    };

    Matrix4x4 CalculateJointXform(Vector3 pos)
      => Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one * 0.07f);

    Matrix4x4 CalculateBoneXform(Vector3 p1, Vector3 p2)
    {
        var length = Vector3.Distance(p1, p2) / 2;
        var radius = 0.03f;

        var center = (p1 + p2) / 2;
        var rotation = Quaternion.FromToRotation(Vector3.up, p2 - p1);
        var scale = new Vector3(radius, length, radius);

        return Matrix4x4.TRS(center, rotation, scale);
    }

        #endregion

        #region MonoBehaviour implementation

        void Start()
        {
            _pipeline = new HandPipeline(_resources);

            MyBallColor = new Material[21];
            for (int i = 0; i <  MyBallColor.Length; i++)
            {
                MyBallColor[i] = new Material(BallColor);
            }
        }

        void OnDestroy()
      => _pipeline.Dispose();

    void LateUpdate()
    {
        // Feed the input image to the Hand pose pipeline.
        _pipeline.UseAsyncReadback = _useAsyncReadback;
        _pipeline.ProcessImage(_webcam.Texture);

        var layer = gameObject.layer;

        // Joint balls
        for (var i = 0; i < HandPipeline.KeyPointCount; i++)
        {
            var xform = CalculateJointXform(_pipeline.GetKeyPoint(i));

            
            if (i == 4)
            {
                MyBallColor[i].color = new Color(1f, 0, 0);
                Graphics.DrawMesh(_jointMesh, xform, MyBallColor[i]/*_jointMaterial*/, layer);
                Debug.Log("[4]: " + _pipeline.GetKeyPoint(i));
            }
            else if (i == 8)
            {
                MyBallColor[i].color = new Color(1f, 1f, 0);
                Graphics.DrawMesh(_jointMesh, xform, MyBallColor[i]/*_jointMaterial*/, layer);
                    Debug.Log("[8]: " + _pipeline.GetKeyPoint(i));
                }
            else if (i == 12)
            {
                MyBallColor[i].color = new Color(0, 1f, 0);
                Graphics.DrawMesh(_jointMesh, xform, MyBallColor[i]/*_jointMaterial*/, layer);
                    Debug.Log("[12]: " + _pipeline.GetKeyPoint(i));
                }
            else if (i == 16)
            {
                MyBallColor[i].color = new Color(0, 1f, 1f);
                Graphics.DrawMesh(_jointMesh, xform, MyBallColor[i]/*_jointMaterial*/, layer);
                    Debug.Log("[16]: " + _pipeline.GetKeyPoint(i));
                }
            else if (i == 20)
            {
                MyBallColor[i].color = new Color(0, 0, 1f);
                Graphics.DrawMesh(_jointMesh, xform, MyBallColor[i]/*_jointMaterial*/, layer);
                    Debug.Log("[20]: " + _pipeline.GetKeyPoint(i));
                }

            else
            {
            Graphics.DrawMesh(_jointMesh, xform, _jointMaterial, layer);
            }
            
        }

        // Bones
        foreach (var pair in BonePairs)
        {
            var p1 = _pipeline.GetKeyPoint(pair.Item1);
            var p2 = _pipeline.GetKeyPoint(pair.Item2);
            var xform = CalculateBoneXform(p1, p2);
            Graphics.DrawMesh(_boneMesh, xform, _boneMaterial, layer);
        }

        // UI update
        _monitorUI.texture = _webcam.Texture;
    }

    #endregion
}

} // namespace MediaPipe.HandPose
