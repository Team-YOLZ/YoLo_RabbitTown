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

    public GameObject _smallcircle;
    public GameObject _bigcircle;
    public Vector3 joystickVec;
    public GameObject player;

    private static JoystickMovement instance;
    Vector3 TouchPosition;
    Vector3 bigfirstposition;
    Vector3 smallfirstposition;
    float circleradius;
    private bool leftwidthtouch = false;
    private Animator player_anim;
    Rigidbody player_rb;
    public float _speed = 5f;

    void Start()
    {
        circleradius = _bigcircle.gameObject.GetComponent<RectTransform>().sizeDelta.y / 2; //조이스틱 이동반경원 size 초기화.
        bigfirstposition = _bigcircle.transform.position;        
        player_rb = player.GetComponent<Rigidbody>();
        player_anim = player.GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (joystickVec.x < 0 | joystickVec.y < 0 | joystickVec.x > 0 |joystickVec.x < 0)
        {
            player_rb.velocity = new Vector3(joystickVec.x *_speed, player_rb.velocity.y, joystickVec.y * _speed);
            player_rb.rotation = Quaternion.LookRotation(new Vector3(joystickVec.x * _speed, 0, joystickVec.y * _speed));
            player_anim.SetFloat("Move", Mathf.Abs(joystickVec.x) + Mathf.Abs(joystickVec.y)); //애니메이션 SetTrigger -> Float으로 변경
        }
    }
      

    public void PointDown() //조이스틱 터치시 event
    {
        if (Input.mousePosition.x > Screen.width / 2) //화면 오른쪽은 조이스틱 키작동 X
        {
            leftwidthtouch = false;
            return;
        }
        else //화면 왼쪽만 가능
        {
            leftwidthtouch = true;
            _bigcircle.transform.position = Input.mousePosition;
            _smallcircle.transform.position = Input.mousePosition;

            TouchPosition = Input.mousePosition; // 터치후 드래그시 방향벡터 잡아오기 위한 변수.
        }
    }
    public void Drag(BaseEventData baseEventData)
    {
        if (leftwidthtouch == true)
        {
            PointerEventData pointerEventData = baseEventData as PointerEventData;
            Vector3 DragPosition = pointerEventData.position; // 드레그 중인 포인터 위치.

            joystickVec = (DragPosition - TouchPosition).normalized;//드레그 방향벡터. (이동코드에 사용할 예정.)
            
            float joystickDistance = Vector3.Distance(DragPosition, TouchPosition);

            if (joystickDistance < circleradius) //조이스틱의 작은 원이 범위를 벗어나지 않게.
            {
                _smallcircle.transform.position = TouchPosition + joystickVec * joystickDistance;
            }
            else
            {
                _smallcircle.transform.position = TouchPosition + joystickVec * circleradius;
            }
        }
    }
    public void Drop()
    {
        joystickVec = Vector3.zero;
        leftwidthtouch = false;
        //조이스틱 위치초기화.
        _bigcircle.transform.position = bigfirstposition;
        _smallcircle.transform.position = bigfirstposition;
    }
}
