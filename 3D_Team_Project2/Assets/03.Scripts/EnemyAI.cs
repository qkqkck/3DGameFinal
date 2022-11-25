using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static EnemyAI;

public class EnemyAI : MonoBehaviour
{
    public MonsterState monsterState = MonsterState.idle; // ���� ����
    public enum MonsterState { idle, trace, attack }; // �⺻, ����, ����

    private Transform monsterTr;
    private Transform playerTr;
    private NavMeshAgent nvAgent;
    private Animator animator;

    public float traceDist = 0.0f; // ���� �Ÿ�
    public float attackDist = 0.0f; // ���� ��Ÿ�
    private bool isHide = false;
    // Start is called before the first frame update
    void Start()
    {
        monsterTr = this.gameObject.GetComponent<Transform>();
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
        nvAgent = this.gameObject.GetComponent<NavMeshAgent>();
        animator = this.gameObject.GetComponent<Animator>();

        StartCoroutine(this.CheckMonsterState());
        StartCoroutine(this.MosterAction());
    }

    IEnumerator CheckMonsterState() // ���� ����
    {
        while (!isHide)
        {
            yield return new WaitForSeconds(0.2f);

            float dist = Vector3.Distance(playerTr.position, monsterTr.position);

            if (dist <= attackDist)
            {
                monsterState = MonsterState.attack;
            }
            else if (dist <= traceDist)
            {
                monsterState = MonsterState.trace;
            }
            else
            {
                monsterState = MonsterState.idle;
            }
        }
    }

    IEnumerator MosterAction() // ���� �ൿ
    {
        while (!isHide)
        {
            switch (monsterState)
            {
                case MonsterState.idle:
                    nvAgent.isStopped = true;
                    animator.SetBool("", false); // ����
                    break;
                case MonsterState.trace:
                    nvAgent.destination = playerTr.position;
                    nvAgent.isStopped = false;
                    animator.SetBool("", false); // ����
                    animator.SetBool("", true); // ����
                    break;
                case MonsterState.attack:
                    nvAgent.isStopped = true;
                    animator.SetBool("", true); // ����
                    break;
            }
            yield return null;
        }
    }

    void OnPlayerDie() // �÷��̾� ��� ��
    {
        StopAllCoroutines();
        nvAgent.isStopped = true;
        animator.SetTrigger(""); // ��ĭ, �˸��� �ൿ
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}