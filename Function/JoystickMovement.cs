using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickMovement : MonoBehaviour
{
    public static JoystickMovement Instance //sigleton
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<JoystickMovement>();
                if (Instance == null)
                {
                    var instanceContainer = new GameObject("JoystickMovement");
                    instance = instanceContainer.AddComponent<JoystickMovement>();
                }
            }
            return instance;
        }
    }

    public RectTransform _smallcircle;
    private RectTransform _bigcircle;
    [SerializeField] Vector3 joystickVec;
    float joystickDistance;

    public GameObject player;
    public Transform obj_CameraArm;

    private static JoystickMovement instance;    
    Vector3 firstposition;
    float circleradius;
    private Animator player_anim;
    Rigidbody player_rb;

    static public float _speed;

    void Awake()
    {
        _bigcircle = GetComponent<RectTransform>();
    }

    void Start()
    {
        circleradius = _bigcircle.sizeDelta.y / 5; //조이스틱 이동반경원 size 초기화.
        firstposition = _smallcircle.position;
        player_rb = player.GetComponent<Rigidbody>();
        player_anim = player.GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        Debug.DrawRay(obj_CameraArm.position, new Vector3(obj_CameraArm.forward.x, 0f, obj_CameraArm.forward.z).normalized, Color.red);

        //카메라의 forward 중심으로 방향벡터 설정 https://forum.unity.com/threads/what-is-transform-forward.338384/ 참고
        Vector3 CameraVecVertical = new Vector3(obj_CameraArm.forward.x, 0f, obj_CameraArm.forward.z).normalized;
        Vector3 CameraVecHorizontal= new Vector3(obj_CameraArm.right.x, 0f, obj_CameraArm.right.z).normalized;

        //Editor에서 테스트하기 편하도록 - 애니메이션, rotation은 x position 변경만(테스트용 - 코드 낭비 x)
        if (Application.platform == RuntimePlatform.WindowsEditor && joystickVec.magnitude == 0) 
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            Vector3 moveDIr = CameraVecVertical * moveVertical + CameraVecHorizontal * moveHorizontal;            
            player.transform.position += moveDIr * _speed * Time.deltaTime;
        }

        if (joystickVec.magnitude != 0 && joystickDistance > 5f) //https://docs.unity3d.com/ScriptReference/Vector3-magnitude.html
        {
            Vector3 moveDIr = CameraVecVertical * joystickVec.y + CameraVecHorizontal * joystickVec.x;

            player.transform.forward = moveDIr; //rotation(방향)조절, 계속 앞을 봐야한다면 CameraVecForward;
            player.transform.position += moveDIr * _speed * Time.deltaTime;                    
        }        
    }

    #region EventTrigger
    public void PointDown(BaseEventData baseEventData) //조이스틱 터치시 event
    {
        //Debug.Log("Joystick_Point down");

        PointerEventData pointerEventData = baseEventData as PointerEventData;

        Vector3 inputPos = pointerEventData.position;
        _smallcircle.position = inputPos;

        joystickVec = (inputPos - firstposition).normalized;

        joystickDistance = Vector3.Distance(inputPos, firstposition);
        if (joystickDistance > 5f)
        {
            player_anim.ResetTrigger("Idle");
            player_anim.SetTrigger("Run");
        }        
    }

    public void Drag(BaseEventData baseEventData)
    {
        //Debug.Log("Joystick_Drag");

        PointerEventData pointerEventData = baseEventData as PointerEventData;
        Vector3 DragPosition = pointerEventData.position; // 드레그 중인 포인터 위치.

        joystickVec = (DragPosition - firstposition).normalized;//드레그 방향벡터.

        joystickDistance = Vector3.Distance(DragPosition, firstposition);

        if (joystickDistance > 5f)
        {
            player_anim.ResetTrigger("Idle");
            player_anim.SetTrigger("Run");
        }
        else
        {
            player_anim.ResetTrigger("Run");
            player_anim.SetTrigger("Idle");
        }

        if (joystickDistance < circleradius) //조이스틱의 작은 원이 범위를 벗어나지 않게.
        {
            _smallcircle.transform.position = firstposition + joystickVec * joystickDistance;
        }
        else
        {
            _smallcircle.transform.position = firstposition + joystickVec * circleradius;
        }
    }

    public void Drop()
    {
        //Debug.Log("Joystick_Joystick_Drop");

        //스틱 위치 방향 벡터 초기화
        _smallcircle.anchoredPosition = Vector2.zero;
        joystickVec = Vector3.zero;

        player_anim.ResetTrigger("Run");
        player_anim.SetTrigger("Idle");
    }
    #endregion
}
