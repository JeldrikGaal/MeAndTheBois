using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPositions : MonoBehaviour
{
    public List<Vector3> cameraPositions;

    public Vector3 pos1;
    public Vector3 pos2;
    public Vector3 pos3;
    public Vector3 pos4;
    public Vector3 pos5;
    public Vector3 pos6;
    public Vector3 pos7;
    public Vector3 pos8;
    public Vector3 pos9;

    // Start is called before the first frame update
    void Start()
    {
        cameraPositions.Add(pos1);
        cameraPositions.Add(pos2);
        cameraPositions.Add(pos3);
        cameraPositions.Add(pos4);
        cameraPositions.Add(pos5);
        cameraPositions.Add(pos6);
        cameraPositions.Add(pos7);
        cameraPositions.Add(pos8);
        cameraPositions.Add(pos9);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
