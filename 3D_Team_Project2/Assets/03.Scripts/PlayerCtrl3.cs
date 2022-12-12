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

    public float idleSpeed = 10.0f; //�÷��̾��� �⺻ �̵��ӵ� ����
    public float moveSpeed = 10.0f; // ������ �÷��̾ �����̰� �� �ӵ�
    public float dashSpeed = 20.0f; //�뽬 �ӵ�
    public float slideSpeed = 30.0f; //�����̵� �ӵ�
    public float slideTime = 0.25f; //�����̵��� ���ӵǴ� �ð�
    public float jumpPower = 10.0f; //�÷��̾��� ù��° ���� ����
    public float doublejumpPower = 10.0f; //�÷��̾��� �ι�° ���� ����
    private int jumpCount = 2; // ���� Ƚ���� �� ����

    public bool isGround; //�÷��̾ ���� ����ִ��� Ȯ��
    private bool canSlide = true;
    private bool isDash; //�޸��� �ִϸ��̼�

    private int jumping = 0;
    public float sensitivity = 100f; //ī�޶��� ����

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

        //lookForward�� �ϸ� ��,��,�� �̵��� �� ���� �ִϸ��̼� �߰��ؾߵ�, ������ moveDir�� ���� ��
        characterBody.forward = moveDir;
        //�̵�
        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }

    private void LookAround() //�÷��̾� ī�޶�
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime);
        Vector3 camAngle = cameraArm.rotation.eulerAngles;
        float x = camAngle.x - mouseDelta.y;

        if (x < 180f) //x�� ȸ���� ������ �ּ� ȭ���� ������ ���� ���� ����
        {
            x = Mathf.Clamp(x, -1f, 70f);
        }
        else
        {
            x = Mathf.Clamp(x, 335f, 361f);
        }

        //���콺�� �Ʒ��� ������ ķ�� �Ʒ��� ���� ���콺�� �ø��� ķ�� ���� ��
        cameraArm.rotation = Quaternion.Euler(x, camAngle.y + mouseDelta.x, camAngle.z);
    }
    void Dash() //�÷��̾� �뽬
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
    void Sliding() //�÷��̾� �����̵�
    {
        //�����̵��� �ٴڿ� �������� �����ϵ���
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
    void slidingFalse() //�����̵��� ������ ��
    {
        moveSpeed = idleSpeed;
        this.GetComponent<CapsuleCollider>().direction = 1;
        this.GetComponent<CapsuleCollider>().center = new Vector3(0f, 0.51f, 0f);
    }

    void TFslide() //�����̵��� �������� ������� ���ϰ� �ϱ� ����
    {
        if (canSlide == false)
        {
            anim.SetBool("IsSliding", false);
            canSlide = true;
        }
    }

    private void OnCollisionEnter(Collision other)// �÷��̾ ���� �ִ��� Ȯ��
    {
        if (other.gameObject.CompareTag("Ground"))//Ground �±װ� ���� ������Ʈ�� ����� ��
        {
            jumpCount = 2; //����ī��Ʈ�� 2�� �ʱ�ȭ
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
        if (other.gameObject.CompareTag("Wall"))//Ground �±װ� ���� ������Ʈ�� ����� ��
        {
            idleSpeed = 3.0f; //�÷��̾��� �⺻ �̵��ӵ� ����
            moveSpeed = 3.0f; // ������ �÷��̾ �����̰� �� �ӵ�
            dashSpeed = 3.0f; //�뽬 �ӵ�
            slideSpeed = 3.0f; //�����̵� �ӵ�
        }
    }
    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Wall"))//Ground �±װ� ���� ������Ʈ�� ����� ��
        {
            idleSpeed = 10.0f; //�÷��̾��� �⺻ �̵��ӵ� ����
            moveSpeed = 10.0f; // ������ �÷��̾ �����̰� �� �ӵ�
            dashSpeed = 20.0f; //�뽬 �ӵ�
            slideSpeed = 30.0f; //�����̵� �ӵ�
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
