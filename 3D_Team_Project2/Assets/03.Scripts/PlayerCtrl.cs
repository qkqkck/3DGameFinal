using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    private float h = 0.0f;
    private float v = 0.0f;

    private Transform tr;
    public float idleSpeed = 10.0f; //�÷��̾��� �⺻ �̵��ӵ� ����
    public float moveSpeed = 10.0f; // ������ �÷��̾ �����̰� �� �ӵ�
    public float rotSpeed = 100.0f; //ȭ�� ȸ�� ����
    public float dashSpeed = 20.0f; //�뽬 �ӵ�
    public float slideSpeed = 25.0f; //�����̵� �ӵ�
    public float slideTime = 0.5f; //�����̵��� ���ӵǴ� �ð�
    public float jumpPower = 10.0f; //�÷��̾��� ù��° ���� ����
    public float doublejumpPower = 10.0f; //�÷��̾��� �ι�° ���� ����
    private int jumpCount = 2; // ���� Ƚ���� �� ����

    private bool isGround = false; //�÷��̾ ���� ����ִ��� Ȯ��
    private bool canSlide = true;

    Rigidbody myRigidbody;

    public Animator anim;
    private int jumping = 0;

    public bool onladder = false; //��ٸ� ���� ��
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        tr = GetComponent<Transform>(); //transform �Ҵ�
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

    void Move() //�÷��̾� �̵� & ȭ�� ȸ��
    {
        if (onladder == false)
        {
            v = Input.GetAxis("Vertical");
            h = Input.GetAxis("Horizontal");

            Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);
            tr.Translate(moveDir.normalized * Time.deltaTime * moveSpeed, Space.Self);

            tr.Rotate(Vector3.up * Time.deltaTime * rotSpeed * Input.GetAxis("Mouse X")); // ȭ�� ȸ��
        }
        else if (onladder == true)
        {
            OnLadder();
        }
    }

    void Jump() // �÷��̾� ����
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
        else if (jumpCount == 1) //2�� ����
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                myRigidbody.AddForce(Vector3.up * doublejumpPower, ForceMode.Impulse);
                jumpCount--;
                isGround = false;
            }
        }
    }

    void Dash() //�÷��̾� �뽬
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
    
    void Sliding() //�÷��̾� �����̵�
    {
        //�����̵��� �ٴڿ� �������� �����ϵ���
        if (isGround == true && Input.GetKeyDown(KeyCode.LeftControl) && canSlide == true)
        {
            canSlide = false;
            moveSpeed = slideSpeed;
            Invoke("slidingFalse", slideTime);
            Invoke("TFslide", 1f);
        }
    }

    void slidingFalse() //�����̵��� ������ ��
    {
        moveSpeed = idleSpeed;
    }

    void TFslide() //�����̵��� �������� ������� ���ϰ� �ϱ� ����
    {
        if (canSlide == false)
        {
            canSlide = true;
        }
    }

    private void OnCollisionEnter(Collision other)// �÷��̾ ���� �ִ��� Ȯ��
    {
        if (other.gameObject.CompareTag("Ground"))//Ground �±װ� ���� ������Ʈ�� ����� ��
        {
            jumpCount = 2; //����ī��Ʈ�� 2�� �ʱ�ȭ
            isGround = true;
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



    void AnimCtrl() //�ִϸ��̼� ����
    {
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
    }

    //��ٸ� Ż �� �߷� ������
    public void isGravity(bool gravity)
    {
        myRigidbody.useGravity = gravity;
    }

    //��ٸ� Ż �� �����̵��� ����
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

    //�÷��̾� ���� startpos ���ӿ�����Ʈ ��ġ�� ���� �̵�
    public void IsDie()
    {
        Debug.Log("player die");
        this.transform.position = GameObject.Find("StartPos").transform.position;
        this.transform.rotation = GameObject.Find("StartPos").transform.rotation;
    }
}
