using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public Transform targetTr; //�÷��̾� ���ӿ�����Ʈ�� Transform ����
    public float dist = 10.0f; // ī�޶���� �Ÿ�
    public float height = 3.0f;
    public float dampTrace = 20.0f;
    private Transform tr; // ī�޶��� Transform ����
    void Start()
    {
        tr = GetComponent<Transform>();
    }

    void LateUpdate()
    {
        tr.position = Vector3.Lerp(tr.position, //���� ��ġ
                                    targetTr.position - (targetTr.forward * dist) + (Vector3.up * height), // ������ġ
                                    Time.deltaTime * dampTrace); // ���� �ð�
        tr.LookAt(targetTr.position);
    }
}
