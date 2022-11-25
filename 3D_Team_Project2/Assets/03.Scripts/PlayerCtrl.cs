using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    private float h = 0.0f;
    private float v = 0.0f;

    private Transform tr;
    public float moveSpeed = 10.0f; //플레이어의 이동속도 변수
    public float rotSpeed = 100.0f; //화면 회전 변수
    public float jumpPower = 10.0f; //플레이어의 첫번째 점프 정도
    public float doublejumpPower = 10.0f; //플레이어의 두번째 점프 정도
    public float dashSpeed = 30.0f; //대쉬 속도
    private int jumpCount = 2; // 점프 횟수를 셀 변수



    Rigidbody myRigidbody;
    void Start()
    {
        tr = GetComponent<Transform>(); //transform 할당
        myRigidbody = GetComponent<Rigidbody>();

    }

    void Update()
    {
        Move();
        Jump();
    }

    void Move() //플레이어 이동 & 화면 회전
    {
        v = Input.GetAxis("Vertical");
        h = Input.GetAxis("Horizontal");

        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);
        tr.Translate(moveDir.normalized * Time.deltaTime * moveSpeed, Space.Self);

        tr.Rotate(Vector3.up * Time.deltaTime * rotSpeed * Input.GetAxis("Mouse X")); // 화면 회전
    }
    void Jump() // 플레이어 점프
    {
        if(jumpCount == 2)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                myRigidbody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
                jumpCount--;
                Debug.Log("JUMP!!!!");
            }
        }
        else if(jumpCount == 1) //2단 점프
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                myRigidbody.AddForce(Vector3.up * doublejumpPower, ForceMode.Impulse);
                jumpCount--;
                Debug.Log("DOUBLE JUMP!!!!");
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))//Ground 태그가 붙은 오브젝트에 닿았을 때
        {
            jumpCount = 2; //점프카운트를 2로 초기화
            Debug.Log("Ground!!!!");
        }
    }
}
