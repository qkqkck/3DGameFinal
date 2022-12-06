using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerCtrl2 : MonoBehaviour
{
    private float h = 0.0f;
    private float v = 0.0f;

    private Transform tr;
    public float moveSpeed = 10.0f; //�÷��̾��� �̵��ӵ� ����
    public float rotSpeed = 100.0f; //ȭ�� ȸ�� ����
    public float jumpPower = 10.0f; //�÷��̾��� ù��° ���� ����
    public float doublejumpPower = 10.0f; //�÷��̾��� �ι�° ���� ����
    public float dashSpeed = 20.0f; //�뽬 �ӵ�
    private int jumpCount = 2; // ���� Ƚ���� �� ����

    Rigidbody myRigidbody;

    public Animator anim;
    private int jumping = 0;

    public bool onladder = false; //��ٸ� ���� ��
    private bool inAir = false;
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        tr = GetComponent<Transform>(); //transform �Ҵ�
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

    void Move() //�÷��̾� �̵� & ȭ�� ȸ��
    {
        if (onladder == false)
        {
            v = Input.GetAxis("Vertical");
            h = Input.GetAxis("Horizontal");


            Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);
            tr.Translate(moveDir.normalized * Time.deltaTime * moveSpeed, Space.Self);

            if (inAir = false && (v != 0 || h != 0)) AnimCtrl("Move");
            else AnimCtrl("MoveStop");

            tr.Rotate(Vector3.up * Time.deltaTime * rotSpeed * Input.GetAxis("Mouse X")); // ȭ�� ȸ��
        }
        else if (onladder == true)
        {
            OnLadder();
        }
        //AnimCtrl("Idle");
    }

    void Jump() // �÷��̾� ����
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
                else if (jumpCount == 1) //2�� ����
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

    void Dash() //�÷��̾� �뽬
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
        if (collision.gameObject.CompareTag("Ground"))//Ground �±װ� ���� ������Ʈ�� ����� ��
        {
            jumpCount = 2; //����ī��Ʈ�� 2�� �ʱ�ȭ
            //Debug.Log("Ground!!!!");

            //���� ������ ��ٸ����� �����°����� ����
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
            //Debug.Log("Ʈ����");
            anim.SetBool("IsJumping", false);
            jumping = 0;
        }
    }
    



    void AnimCtrl(string acting) //�ִϸ��̼� ����
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
            if (Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d")) //�޸���, �뽬
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

    //��ٸ� Ż �� �߷� ������
    public void isGravity(bool gravity)
    {
        myRigidbody.useGravity = gravity;
    }

    //��ٸ� Ż �� �����̵��� ����
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

    //�÷��̾� ���� startpos ���ӿ�����Ʈ ��ġ�� ���� �̵�
    public void IsDie()
    {
        Debug.Log("player die");
        this.transform.position = GameObject.Find("StartPos").transform.position;
        this.transform.rotation = GameObject.Find("StartPos").transform.rotation;
    }
}
