using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScreenTouch : MonoBehaviour
{
    public Transform obj_player;
    public Transform obj_CameraArm;    

    public float dragSpeed = 1f;
    float CameraArm_Y = 3.5f;

    private void FixedUpdate()
    {
        if (CameraArm_Y < 3.5f) CameraArm_Y = 3.5f;
        if (CameraArm_Y > 20f) CameraArm_Y = 20f;

        obj_CameraArm.position = new Vector3(obj_player.position.x, obj_player.position.y + CameraArm_Y, obj_player.position.z);
    }

    public void BeginDrag()
    {
        Debug.Log("RightScreen_BeginDrag");
    }

    public void Drag(BaseEventData baseEventData)
    {
        PointerEventData touchData = baseEventData as PointerEventData;

        if (Input.mousePosition.x > Screen.width / 2.5f)
        {
            Debug.Log("RightScreen_Drag");

            obj_CameraArm.Rotate(0f, Input.GetAxis("Mouse X") * dragSpeed, 0f);
            obj_CameraArm.Translate(new Vector3(0f, Input.GetAxis("Mouse Y") * dragSpeed, 0f));
            CameraArm_Y = obj_CameraArm.position.y - obj_player.position.y;
        }        
    }

    public void EndDrag()
    {
        Debug.Log("RightScreen_EndDrag");
    }
}
