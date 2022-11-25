using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static EnemyAI;

public class EnemyAI : MonoBehaviour
{
    public MonsterState monsterState = MonsterState.idle; // 몬스터 상태
    public enum MonsterState { idle, trace, attack }; // 기본, 추적, 공격

    private Transform monsterTr;
    private Transform playerTr;
    private NavMeshAgent nvAgent;
    private Animator animator;

    public float traceDist = 0.0f; // 추적 거리
    public float attackDist = 0.0f; // 공격 사거리
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

    IEnumerator CheckMonsterState() // 몬스터 상태
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

    IEnumerator MosterAction() // 몬스터 행동
    {
        while (!isHide)
        {
            switch (monsterState)
            {
                case MonsterState.idle:
                    nvAgent.isStopped = true;
                    animator.SetBool("", false); // 추적
                    break;
                case MonsterState.trace:
                    nvAgent.destination = playerTr.position;
                    nvAgent.isStopped = false;
                    animator.SetBool("", false); // 공격
                    animator.SetBool("", true); // 추적
                    break;
                case MonsterState.attack:
                    nvAgent.isStopped = true;
                    animator.SetBool("", true); // 공격
                    break;
            }
            yield return null;
        }
    }

    void OnPlayerDie() // 플레이어 사망 시
    {
        StopAllCoroutines();
        nvAgent.isStopped = true;
        animator.SetTrigger(""); // 빈칸, 알맞은 행동
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}