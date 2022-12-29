using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerInfo : MonoBehaviour, IHittable
{
    private NavMeshAgent agent;
    private Animator anim;

    [Header ("Player Info")]
    [SerializeField] private float damage;      
    [SerializeField] private float curShield;   //현재 보호막 수치
    [SerializeField] private float maxShield;   //스킬 사용시 충전되는 보호막 수치
    [SerializeField] private float maxHp;       
    [SerializeField] private float curHp;
    [SerializeField] private int myTurn;        //공격 횟수
    [SerializeField] private string skillName;
    private bool isMeleeAttack;
    public float HP
    {
        get { return curHp; }
        set 
        {
            curHp = value;
            if(curHp > maxHp)
                curHp = maxHp;
            if(curHp <= 0)
            {
                //Die();
            }
        }
    }
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }
 /// <summary>
 /// 대~충 공격패턴
 /// </summary>
 /// <returns></returns>
    //IEnumerator IAttackCo()
    //{
    //    if(!isMeleeAttack)
    //    {
    //        anim.SetTrigger(skillName);
    //        //파티클
    //    }
    //    if(isMeleeAttack)
    //    {
    //        anim.SetBool("IsMove", true);
    //        //agent.SetDestination(공격목표위치);
    //        //yield return new WaitForSeconds(이동시간);
    //        anim.SetTrigger(skillName);
    //        //yield return new WaitForSeconds(원래자리로 이동시간);
    //        //agent.SetDestination(원래자리);
    //        anim.SetBool("IsMove", false);
    //    }
    //}
    public void Hit(float damamge)
    {
        if (curShield - damage >= 0)
        {
            curShield -= damage;
        }
        if (curShield - damage <= 0)
        {
            HP -= Mathf.Abs(curShield - damage);
        }
    }
}
