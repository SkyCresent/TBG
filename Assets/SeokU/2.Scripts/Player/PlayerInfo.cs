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
    [SerializeField] private float curShield;   //���� ��ȣ�� ��ġ
    [SerializeField] private float maxShield;   //��ų ���� �����Ǵ� ��ȣ�� ��ġ
    [SerializeField] private float maxHp;       
    [SerializeField] private float curHp;
    [SerializeField] private int myTurn;        //���� Ƚ��
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
 /// ��~�� ��������
 /// </summary>
 /// <returns></returns>
    //IEnumerator IAttackCo()
    //{
    //    if(!isMeleeAttack)
    //    {
    //        anim.SetTrigger(skillName);
    //        //��ƼŬ
    //    }
    //    if(isMeleeAttack)
    //    {
    //        anim.SetBool("IsMove", true);
    //        //agent.SetDestination(���ݸ�ǥ��ġ);
    //        //yield return new WaitForSeconds(�̵��ð�);
    //        anim.SetTrigger(skillName);
    //        //yield return new WaitForSeconds(�����ڸ��� �̵��ð�);
    //        //agent.SetDestination(�����ڸ�);
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
