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

    //���Ӿ� ����� obj_CameraArm zoom out Timeline���� ���� ��� �ӽ� �ڵ�
    //[SerializeField] bool EnterGame = true;
    //[SerializeField] Vector3 orgPos;

    //���Ӿ� ����� obj_CameraArm zoom out Timeline���� ���� ��� �ӽ� �ڵ�
    private void Awake()
    {
        //orgPos = new Vector3(obj_CameraArm.localPosition.x , obj_CameraArm.localPosition.y, obj_CameraArm.localPosition.z - 15);
        CameraArm_Y = 20f;
    }

    private void LateUpdate()
    {
        //rotation ����, default = player.y + 3.5f;
        //if (CameraArm_Y < 3.5f) CameraArm_Y = 3.5f;
        //if (CameraArm_Y > 20f) CameraArm_Y = 20f;

        /* //���Ӿ� ����� obj_CameraArm zoom out Timeline���� ���� ��� �ӽ� �ڵ�
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

            //mouse�� x���� �߽����� ���� �޾ƿͼ� rotation.y ����
            obj_CameraArm.Rotate(0f, Input.GetAxis("Mouse X") * dragSpeed, 0f);

            //mouse�� y���� �߽����� ���� �޾ƿͼ� position.y ���� (ī�޶��� x���� �ٲٴ°� �ƴ϶� CameraArm�� y���� ����)
            //obj_CameraArm.Translate(new Vector3(0f, Input.GetAxis("Mouse Y") * dragSpeed, 0f));

            CameraArm_Y = obj_CameraArm.position.y - obj_player.position.y; //CameraArm.position.y�� �÷��̾��� Position.y�� + �ؾ���
        }        
    }

    public void EndDrag()
    {
        Debug.Log("RightScreen_EndDrag");
    }
}
