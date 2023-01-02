using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum SkillType
{
    Attack, Defense, Buff, Debuff
}
public class Skill : MonoBehaviour
{
    private Animator anim;
    private NavMeshAgent agent;

    [Header("Skill Info")]
    [SerializeField] private KeyCode keyCode;
    [SerializeField] private SkillType skillType;
    [SerializeField] private string skillName;
    [SerializeField] private float damage;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }
}
