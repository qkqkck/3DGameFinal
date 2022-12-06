using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerCtrl2 : MonoBehaviour
{
    private float h = 0.0f;
    private float v = 0.0f;

    private Transform tr;
    public float moveSpeed = 10.0f; //플레이어의 이동속도 변수
    public float rotSpeed = 100.0f; //화면 회전 변수
    public float jumpPower = 10.0f; //플레이어의 첫번째 점프 정도
    public float doublejumpPower = 10.0f; //플레이어의 두번째 점프 정도
    public float dashSpeed = 20.0f; //대쉬 속도
    private int jumpCount = 2; // 점프 횟수를 셀 변수

    Rigidbody myRigidbody;

    public Animator anim;
    private int jumping = 0;

    public bool onladder = false; //사다리 탔을 때
    private bool inAir = false;
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        tr = GetComponent<Transform>(); //transform 할당
        myRigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Jump();
        Dash();
        //AnimCtrl("Idle");

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

            if (inAir = false && (v != 0 || h != 0)) AnimCtrl("Move");
            else AnimCtrl("MoveStop");

            tr.Rotate(Vector3.up * Time.deltaTime * rotSpeed * Input.GetAxis("Mouse X")); // 화면 회전
        }
        else if (onladder == true)
        {
            OnLadder();
        }
        //AnimCtrl("Idle");
    }

    void Jump() // 플레이어 점프
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {

            if (onladder == false)
            {
                if (jumpCount == 2)
                {
                    myRigidbody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
                    jumpCount--;
                    AnimCtrl("Jump");
                }
                else if (jumpCount == 1) //2단 점프
                {
                    myRigidbody.AddForce(Vector3.up * doublejumpPower, ForceMode.Impulse);
                    jumpCount--;
                    AnimCtrl("Jump");
                }
            }
            else if (onladder == true)
            {
                isGravity(true);
                onladder = false;
                myRigidbody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
                jumpCount--;
                AnimCtrl("Jump");
            }
            inAir = true;
        }
    }

    void Dash() //플레이어 대쉬
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveSpeed = dashSpeed;
            //Debug.Log("DASH!!!!!!!!!!!!!");
            AnimCtrl("DashOn");
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            moveSpeed = 10.0f;
            //Debug.Log("DASH DONE!!!!!!!!!!");
            AnimCtrl("DashOff");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))//Ground 태그가 붙은 오브젝트에 닿았을 때
        {
            jumpCount = 2; //점프카운트를 2로 초기화
            //Debug.Log("Ground!!!!");

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
    



    void AnimCtrl(string acting) //애니메이션 관리
    {
        switch (acting)
        {
            case "Move":
                anim.SetBool("IsRun", true);
                break;
            case "DashOn":
                anim.SetBool("IsRun", false);
                anim.SetBool("IsDash", true);
                break;
            case "DashOff":
                anim.SetBool("IsDash", false);
                break;
            case "MoveStop":
                anim.SetBool("IsDash", false);
                anim.SetBool("IsRun", false);
                break;
            case "Jump":
                anim.SetBool("IsDash", false);
                anim.SetBool("IsRun", false);
                anim.SetBool("IsJumping", true);
                anim.SetTrigger("IsJump");
                break;
            case "Idle":
                break;
           // default:
           //     anim.SetBool("IsRun", false);
            //    anim.SetBool("IsDash", false);
           //     anim.SetBool("IsJumping", false);
            //    break;

        }
        /*
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
        */
    }

    //사다리 탈 때 중력 미적용
    public void isGravity(bool gravity)
    {
        myRigidbody.useGravity = gravity;
    }

    //사다리 탈 때 상하이동만 가능
    void OnLadder()
    {
        jumpCount = 2;
        myRigidbody.velocity = Vector3.zero;
        myRigidbody.angularVelocity = Vector3.zero;

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
