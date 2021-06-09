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

    public GameObject player;

    private static JoystickMovement instance;
    Vector3 TouchPosition;
    Vector3 firstposition;
    float circleradius;
    private Animator player_anim;
    Rigidbody player_rb;
    public float _speed;

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
        if (joystickVec.y > 0 | joystickVec.y < 0 | joystickVec.x > 0 | joystickVec.x < 0)
        {
            player_rb.velocity = new Vector3(joystickVec.x * _speed, player_rb.velocity.y, joystickVec.y * _speed);
            player_rb.rotation = Quaternion.LookRotation(new Vector3(joystickVec.x * _speed, 0, joystickVec.y * _speed));
            player_anim.SetFloat("Move", Mathf.Abs(joystickVec.x) + Mathf.Abs(joystickVec.y)); //애니메이션 SetTrigger -> Float으로 변경
        }
    }

    public void PointDown(BaseEventData baseEventData) //조이스틱 터치시 event
    {
        Debug.Log("Joystick_Point down");

        PointerEventData pointerEventData = baseEventData as PointerEventData;

        Vector3 inputPos = pointerEventData.position;
        _smallcircle.position = inputPos;

        joystickVec = (inputPos - firstposition).normalized;
    }

    public void Drag(BaseEventData baseEventData)
    {
        Debug.Log("Joystick_Drag");

        PointerEventData pointerEventData = baseEventData as PointerEventData;
        Vector3 DragPosition = pointerEventData.position; // 드레그 중인 포인터 위치.

        joystickVec = (DragPosition - firstposition).normalized;//드레그 방향벡터. (이동코드에 사용할 예정.)

        float joystickDistance = Vector3.Distance(DragPosition, firstposition);

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
        Debug.Log("Joystick_Joystick_Drop");

        //스틱 위치 방향 벡터 초기화
        _smallcircle.anchoredPosition = Vector2.zero;
        joystickVec = Vector3.zero;
    }
}
