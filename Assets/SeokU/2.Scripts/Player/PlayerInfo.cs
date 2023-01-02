using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerInfo : MonoBehaviour, IHittable
{ 
    [Header ("Player Info")]
    [SerializeField] private float curShield;   //현재 보호막 수치
    [SerializeField] private float maxShield;   //스킬 사용시 충전되는 보호막 수치
    [SerializeField] private float maxHp;       
    [SerializeField] private float curHp;
    [SerializeField] private int myTurn;        //공격 횟수

    private void OnEnable()
    {
        curHp = maxHp;
    }
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
    public void Hit(float damage)
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
