using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScreenTouch : MonoBehaviour
{
    public Transform obj_player;
    public Transform obj_CameraArm;    

    public float dragSpeed = 1f;
    float CameraArm_Y = 3.5f; //default

    //게임씬 입장시 obj_CameraArm zoom out Timeline쓰지 않을 경우 임시 코드
    //[SerializeField] bool EnterGame = true;
    //[SerializeField] Vector3 orgPos;

    //게임씬 입장시 obj_CameraArm zoom out Timeline쓰지 않을 경우 임시 코드
    private void Awake()
    {
        //orgPos = new Vector3(obj_CameraArm.localPosition.x , obj_CameraArm.localPosition.y, obj_CameraArm.localPosition.z - 15);
        CameraArm_Y = 20f;
    }

    private void LateUpdate()
    {
        //rotation 범위, default = player.y + 3.5f;
        //if (CameraArm_Y < 3.5f) CameraArm_Y = 3.5f;
        //if (CameraArm_Y > 20f) CameraArm_Y = 20f;

        /* //게임씬 입장시 obj_CameraArm zoom out Timeline쓰지 않을 경우 임시 코드
        if (EnterGame)
        {
            Vector3 cameraArmPosition = obj_CameraArm.localPosition;
            if ((orgPos - obj_CameraArm.position).sqrMagnitude <= 1f) obj_CameraArm.localPosition = orgPos;
            else obj_CameraArm.localPosition = Vector3.Lerp(cameraArmPosition, orgPos, 0.1f);

            if (cameraArmPosition == orgPos) EnterGame = false;
        }
        else
        {
            obj_CameraArm.position = new Vector3(obj_player.position.x, obj_player.position.y + CameraArm_Y, obj_player.position.z);
        }
        */
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

            //mouse의 x축을 중심으로 값을 받아와서 rotation.y 변경
            obj_CameraArm.Rotate(0f, Input.GetAxis("Mouse X") * dragSpeed, 0f);

            //mouse의 y축을 중심으로 값을 받아와서 position.y 변경 (카메라의 x축을 바꾸는게 아니라 CameraArm의 y축을 변경)
            //obj_CameraArm.Translate(new Vector3(0f, Input.GetAxis("Mouse Y") * dragSpeed, 0f));

            CameraArm_Y = obj_CameraArm.position.y - obj_player.position.y; //CameraArm.position.y는 플레이어의 Position.y에 + 해야함
        }        
    }

    public void EndDrag()
    {
        Debug.Log("RightScreen_EndDrag");
    }
}
