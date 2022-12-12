using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GoblinAI: MonoBehaviour
{
    public enum MonsterState { Idle01, Move01, Attack01, Die01 };
    public MonsterState monsterState = MonsterState.Idle01;
    private Transform monsterTr;
    private Transform playerTr;
    private NavMeshAgent nvAgent;
    private Animator animator;

    public float traceDist = 100.0f;
    public float attackDist = 2.0f;
    private bool isDie = false;

    //private int hp = 100;
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

    IEnumerator CheckMonsterState()
    {
        while (!isDie)
        {
            yield return new WaitForSeconds(0.2f);

            float dist = Vector3.Distance(playerTr.position, monsterTr.position);

            if (dist <= attackDist)
            {
                //monsterState = MonsterState.Attack01;
                GameObject.Find("Player").GetComponent<PlayerCtrl>().IsDie();
            }
            else if (dist <= traceDist)
            {
                monsterState = MonsterState.Move01;
            }
            else
            {
                monsterState = MonsterState.Idle01;
            }
        }
    }

    IEnumerator MosterAction()
    {
        while (!isDie)
        {
            switch (monsterState)
            {
                case MonsterState.Idle01:
                    nvAgent.isStopped = true;
                    animator.SetBool("IsTrace", false);
                    break;
                case MonsterState.Move01:
                    nvAgent.destination = playerTr.position;
                    nvAgent.isStopped = false;
                    animator.SetBool("IsAttack", false);
                    animator.SetBool("IsTrace", true);
                    break;
                case MonsterState.Attack01:
                    nvAgent.isStopped = true;
                    animator.SetBool("IsAttack", true);
                    break;
            }
            yield return null;
        }
    }
    /*
    void OnPlayerDie()
    {
        StopAllCoroutines();
        nvAgent.isStopped = true;
        animator.SetTrigger("IsPlayerDie");
    }

    private void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.tag == "BULLET")
        {
            Destroy(coll.gameObject);
            hp -= coll.gameObject.GetComponent<BulletCtrl>().damage;

            if (hp <= 0)
            {
                MonsterDie();
            }
            else
            {
                animator.SetTrigger("IsHIt");
            }
        }
    }

    void MonsterDie()
    {
        StopAllCoroutines();

        isDie = true;
        monsterState = MonsterState.die;
        nvAgent.isStopped = true;
        animator.SetTrigger("IsDie");

        gameObject.GetComponentInChildren<CapsuleCollider>().enabled = false;

        foreach (Collider coll in gameObject.GetComponentsInChildren<SphereCollider>())
        {
            coll.enabled = false;
        }
    }
    */
    // Update is called once per frame
    void Update()
    {

    }
}