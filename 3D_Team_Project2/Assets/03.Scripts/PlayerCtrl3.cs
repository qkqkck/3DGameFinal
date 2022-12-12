using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.SceneManagement;

public class PlayerCtrl3 : MonoBehaviour
{
    [SerializeField]
    private Transform characterBody;
    [SerializeField]
    private Transform cameraArm;

    //Animator anim;
    public Animator anim;

    Rigidbody myRigidbody;

    public float idleSpeed = 10.0f; //플레이어의 기본 이동속도 변수
    public float moveSpeed = 10.0f; // 실제로 플레이어가 움직이게 될 속도
    public float dashSpeed = 20.0f; //대쉬 속도
    public float slideSpeed = 30.0f; //슬라이딩 속도
    public float slideTime = 0.25f; //슬라이딩이 지속되는 시간
    public float jumpPower = 10.0f; //플레이어의 첫번째 점프 정도
    public float doublejumpPower = 10.0f; //플레이어의 두번째 점프 정도
    private int jumpCount = 2; // 점프 횟수를 셀 변수

    public bool isGround; //플레이어가 땅에 닿아있는지 확인
    private bool canSlide = true;
    private bool isDash; //달리기 애니메이션

    private int jumping = 0;
    public float sensitivity = 100f; //카메라의 감도

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        myRigidbody = GetComponent<Rigidbody>();
        moveSpeed = idleSpeed;
        this.transform.position = GameObject.Find("StartPos").transform.position;
        this.transform.rotation = GameObject.Find("StartPos").transform.rotation;
        myRigidbody.velocity = Vector3.zero;
        myRigidbody.angularVelocity = Vector3.zero;
        
    }

    void Update()
    {
        LookAround();
        Dash();
        Jump();
        Sliding();
        AnimCtrl();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void LateUpdate()
    {
        
    }

    private void Move()
    {
        Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector3 lookForward = new Vector3(cameraArm.forward.x, 0f, cameraArm.forward.z).normalized;
        Vector3 lookRight = new Vector3(cameraArm.right.x, 0f, cameraArm.right.z).normalized;
        Vector3 moveDir = lookForward * moveInput.y + lookRight * moveInput.x;

        //lookForward로 하면 좌,우,뒤 이동할 때 따로 애니메이션 추가해야됨, 싫으면 moveDir로 쓰면 됨
        characterBody.forward = moveDir;
        //이동
        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }

    private void LookAround() //플레이어 카메라
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime);
        Vector3 camAngle = cameraArm.rotation.eulerAngles;
        float x = camAngle.x - mouseDelta.y;

        if (x < 180f) //x축 회전에 제한을 둬서 화면이 뒤집어 지는 것을 방지
        {
            x = Mathf.Clamp(x, -1f, 70f);
        }
        else
        {
            x = Mathf.Clamp(x, 335f, 361f);
        }

        //마우스가 아래로 내리면 캠도 아래를 보고 마우스를 올리면 캠도 위를 봄
        cameraArm.rotation = Quaternion.Euler(x, camAngle.y + mouseDelta.x, camAngle.z);
    }
    void Dash() //플레이어 대쉬
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveSpeed = dashSpeed;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            moveSpeed = idleSpeed;
        }
    }

    void Jump() // 플레이어 점프
    {
        if (jumpCount == 2)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                myRigidbody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
                jumpCount--;
                isGround = false;
            }
        }
        else if (jumpCount == 1) //2단 점프
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                myRigidbody.AddForce(Vector3.up * doublejumpPower, ForceMode.Impulse);
                jumpCount--;
                isGround = false;
            }
        }
    }
    void Sliding() //플레이어 슬라이딩
    {
        //슬라이딩은 바닥에 있을때만 가능하도록
        if (isGround == true && Input.GetKeyDown("c") && canSlide == true)
        {
            Debug.Log("Sliding!!!!!!!!!!!");
            this.GetComponent<CapsuleCollider>().direction = 2;
            this.GetComponent<CapsuleCollider>().center = new Vector3(0f, 0.25f, 0f);
            canSlide = false;
            moveSpeed = slideSpeed;
            Invoke("slidingFalse", slideTime);
            Invoke("TFslide", 1f);
            anim.SetBool("IsSliding", true);
        }
    }
    void slidingFalse() //슬라이딩이 끝났을 때
    {
        moveSpeed = idleSpeed;
        this.GetComponent<CapsuleCollider>().direction = 1;
        this.GetComponent<CapsuleCollider>().center = new Vector3(0f, 0.51f, 0f);
    }

    void TFslide() //슬라이딩을 연속으로 사용하지 못하게 하기 위함
    {
        if (canSlide == false)
        {
            anim.SetBool("IsSliding", false);
            canSlide = true;
        }
    }

    private void OnCollisionEnter(Collision other)// 플레이어가 땅에 있는지 확인
    {
        if (other.gameObject.CompareTag("Ground"))//Ground 태그가 붙은 오브젝트에 닿았을 때
        {
            jumpCount = 2; //점프카운트를 2로 초기화
            isGround = true;
            anim.SetBool("IsJumping", false);
            jumping = 0;
            anim.SetBool("IsFalling", false);
            myRigidbody.velocity = Vector3.zero;
            myRigidbody.angularVelocity = Vector3.zero;
            if (isGround == false)
            {
                myRigidbody.velocity = Vector3.zero;
                myRigidbody.angularVelocity = Vector3.zero;
            }
        }
        if (other.gameObject.CompareTag("Wall"))//Ground 태그가 붙은 오브젝트에 닿았을 때
        {
            idleSpeed = 3.0f; //플레이어의 기본 이동속도 변수
            moveSpeed = 3.0f; // 실제로 플레이어가 움직이게 될 속도
            dashSpeed = 3.0f; //대쉬 속도
            slideSpeed = 3.0f; //슬라이딩 속도
        }
    }
    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Wall"))//Ground 태그가 붙은 오브젝트에 닿았을 때
        {
            idleSpeed = 10.0f; //플레이어의 기본 이동속도 변수
            moveSpeed = 10.0f; // 실제로 플레이어가 움직이게 될 속도
            dashSpeed = 20.0f; //대쉬 속도
            slideSpeed = 30.0f; //슬라이딩 속도
        }
    }

    void AnimCtrl() //애니메이션 관리
    {
        if (jumping == 0)
        {
            if (Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d")) //달리기, 대쉬
            {
                anim.SetBool("IsRun", true);

                if (Input.GetKey(KeyCode.LeftShift))
                {
                    anim.SetBool("IsRun", false);
                    anim.SetBool("IsDash", true);
                }
                else
                {
                    anim.SetBool("IsDash", false);
                }
            }
            else
            {
                anim.SetBool("IsRun", false);
                anim.SetBool("IsDash", false);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumping++;

            if (jumping <= 2)
            {
                anim.SetBool("IsJumping", true);
                anim.SetTrigger("IsJump");
            }
            else if (jumping == 2)
            {
                anim.SetBool("IsJumping", false);
            }
        }

        if(Input.GetKeyDown("c"))
        {
            anim.SetTrigger("IsSlide");
        }
    }

    public void IsDie()
    {
        Debug.Log("player die");
        
        this.transform.position = GameObject.Find("StartPos").transform.position;
        this.transform.rotation = GameObject.Find("StartPos").transform.rotation;
    }
}
