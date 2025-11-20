using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChasePlayerAI : MonoBehaviour
{
    public Transform player;                                                //유저 위치
    public float chaseRange = 50.0f;
    public float attackRange = 2.0f;

    private NavMeshAgent agent;                                             //길찾기 알고리즘을 지원 해주는 AI Agent
    private float distanceToPlayer;                                         //플레이어와의 거리 

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.position);               //플레이어와 거리를 측정한다.

        if (distanceToPlayer <= chaseRange )                        //추적 범위에 들어오면 추적한다.
        {
            ChasePlayer();
        }
        else
        {
            StopChasing();
        }

        if (distanceToPlayer <= attackRange )                       //공격 범위에 들어오면 공격한다.
        {
            Attack();
        }
    }

    void StopChasing()
    {
        agent.isStopped = true;
    }

    void ChasePlayer()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);                              //플레이어의 위치를 목적지로 설정한다.
    }

    void Attack()
    {
        agent.isStopped = true;
        transform.LookAt(player);
        Debug.Log("Attacking player!");
    }

    void OnDrawGizmosSelected()                        //오브젝트를 선택 했을 때 범위 표시 한다.         
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);              //추적 범위를 노란색 구체로 표시

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);             //공격 범위를 빨강색 구체로 표시
    }
}
