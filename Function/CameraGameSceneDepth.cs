using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraGameSceneDepth : MonoBehaviour
{
    [SerializeField] Camera UICamera;
    void Start()
    {
        var screenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 100.0f);
        transform.position = UICamera.ScreenToWorldPoint(screenPoint);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
