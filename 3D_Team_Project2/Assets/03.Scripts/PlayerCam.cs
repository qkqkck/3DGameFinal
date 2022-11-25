using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public Transform targetTr; //플레이어 게임오브젝트의 Transform 변수
    public float dist = 10.0f; // 카메라와의 거리
    public float height = 3.0f;
    public float dampTrace = 20.0f;
    private Transform tr; // 카메라의 Transform 변수
    void Start()
    {
        tr = GetComponent<Transform>();
    }

    void LateUpdate()
    {
        tr.position = Vector3.Lerp(tr.position, //시작 위치
                                    targetTr.position - (targetTr.forward * dist) + (Vector3.up * height), // 종료위치
                                    Time.deltaTime * dampTrace); // 보간 시간
        tr.LookAt(targetTr.position);
    }
}
