using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    private float h = 0.0f;
    private float v = 0.0f;

    private Transform tr;
    public float idleSpeed = 10.0f; //플레이어의 기본 이동속도 변수
    public float moveSpeed = 10.0f; // 실제로 플레이어가 움직이게 될 속도
    public float rotSpeed = 100.0f; //화면 회전 변수
    public float dashSpeed = 20.0f; //대쉬 속도
    public float slideSpeed = 25.0f; //슬라이딩 속도
    public float slideTime = 0.5f; //슬라이딩이 지속되는 시간
    public float jumpPower = 10.0f; //플레이어의 첫번째 점프 정도
    public float doublejumpPower = 10.0f; //플레이어의 두번째 점프 정도
    private int jumpCount = 2; // 점프 횟수를 셀 변수

    private bool isGround = false; //플레이어가 땅에 닿아있는지 확인
    private bool canSlide = true;

    Rigidbody myRigidbody;

    public Animator anim;
    private int jumping = 0;

    public bool onladder = false; //사다리 탔을 때
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        tr = GetComponent<Transform>(); //transform 할당
        myRigidbody = GetComponent<Rigidbody>();
        moveSpeed = idleSpeed;
    }

    void Update()
    {
        Jump();
        Dash();
        Sliding();
        AnimCtrl();

    }

    void FixedUpdate()
    {
        Move();
    }

    void Move() //플레이어 이동 & 화면 회전
    {
        if (onladder == false)
        {
            v = Input.GetAxis("Vertical");
            h = Input.GetAxis("Horizontal");

            Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);
            tr.Translate(moveDir.normalized * Time.deltaTime * moveSpeed, Space.Self);

            tr.Rotate(Vector3.up * Time.deltaTime * rotSpeed * Input.GetAxis("Mouse X")); // 화면 회전
        }
        else if (onladder == true)
        {
            OnLadder();
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

    void Dash() //플레이어 대쉬
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveSpeed = dashSpeed;
            //Debug.Log("DASH!!!!!!!!!!!!!");
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            moveSpeed = idleSpeed;
            //Debug.Log("DASH DONE!!!!!!!!!!");
        }
    }
    
    void Sliding() //플레이어 슬라이딩
    {
        //슬라이딩은 바닥에 있을때만 가능하도록
        if (isGround == true && Input.GetKeyDown(KeyCode.LeftControl) && canSlide == true)
        {
            canSlide = false;
            moveSpeed = slideSpeed;
            Invoke("slidingFalse", slideTime);
            Invoke("TFslide", 1f);
        }
    }

    void slidingFalse() //슬라이딩이 끝났을 때
    {
        moveSpeed = idleSpeed;
    }

    void TFslide() //슬라이딩을 연속으로 사용하지 못하게 하기 위함
    {
        if (canSlide == false)
        {
            canSlide = true;
        }
    }

    private void OnCollisionEnter(Collision other)// 플레이어가 땅에 있는지 확인
    {
        if (other.gameObject.CompareTag("Ground"))//Ground 태그가 붙은 오브젝트에 닿았을 때
        {
            jumpCount = 2; //점프카운트를 2로 초기화
            isGround = true;
            //땅에 닿으면 사다리에서 내려온것으로 판정
            if (onladder == true)
            {
                onladder = false;
                isGravity(true);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            //Debug.Log("트리거");
            anim.SetBool("IsJumping", false);
            jumping = 0;
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
    }

    //사다리 탈 때 중력 미적용
    public void isGravity(bool gravity)
    {
        myRigidbody.useGravity = gravity;
    }

    //사다리 탈 때 상하이동만 가능
    void OnLadder()
    {
        v = Input.GetAxis("Vertical");
        Vector3 moveDir = (Vector3.up * v);
        if (v == 1)
        {
            tr.Translate(moveDir.normalized * Time.deltaTime * moveSpeed, Space.Self);
        }
        else if (v == -1)
        {
            tr.Translate(moveDir.normalized * Time.deltaTime * moveSpeed, Space.Self);

        }
    }

    //플레이어 죽음 startpos 게임오브젝트 위치로 강제 이동
    public void IsDie()
    {
        Debug.Log("player die");
        this.transform.position = GameObject.Find("StartPos").transform.position;
        this.transform.rotation = GameObject.Find("StartPos").transform.rotation;
    }
}
