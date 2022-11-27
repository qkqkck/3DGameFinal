using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
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
        AnimCtrl();

    }

    void FixedUpdate()
    {
        Move();
    }

    void Move() //�÷��̾� �̵� & ȭ�� ȸ��
    {
        v = Input.GetAxis("Vertical");
        h = Input.GetAxis("Horizontal");

        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);
        tr.Translate(moveDir.normalized * Time.deltaTime * moveSpeed, Space.Self);

        tr.Rotate(Vector3.up * Time.deltaTime * rotSpeed * Input.GetAxis("Mouse X")); // ȭ�� ȸ��
    }

    void Jump() // �÷��̾� ����
    {
        if(jumpCount == 2)
        {
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                myRigidbody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
                jumpCount--;
            }
        }
        else if(jumpCount == 1) //2�� ����
        {
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                myRigidbody.AddForce(Vector3.up * doublejumpPower, ForceMode.Impulse);
                jumpCount--;
            }
        }
    }
    
    void Dash() //�÷��̾� �뽬
    {
        if(Input.GetKey(KeyCode.LeftShift))
        {
            moveSpeed = dashSpeed;
            //Debug.Log("DASH!!!!!!!!!!!!!");
        }

        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            moveSpeed = 10.0f;
            //Debug.Log("DASH DONE!!!!!!!!!!");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))//Ground �±װ� ���� ������Ʈ�� ����� ��
        {
            jumpCount = 2; //����ī��Ʈ�� 2�� �ʱ�ȭ
            //Debug.Log("Ground!!!!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Ground"))
        {
            //Debug.Log("Ʈ����");
            anim.SetBool("IsJumping", false);
            jumping = 0;
        }
    }



    void AnimCtrl() //�ִϸ��̼� ����
    {
        if(jumping == 0)
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

        if(Input.GetKeyDown(KeyCode.Space))
        {
            jumping++;

            if(jumping <= 2)
            {
                anim.SetBool("IsJumping", true);
                anim.SetTrigger("IsJump");
            }
            else if(jumping == 2)
            {
                anim.SetBool("IsJumping", false);
            }
                

        }
    }
}
