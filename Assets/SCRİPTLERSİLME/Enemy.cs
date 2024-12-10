using System.Collections;
using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
    public enum BehaviorType { Korkak, Saldaggan };
    public BehaviorType behavior;

    public float playerDistance = 3f;
    public float attackRange = 1f;
    public float speed = 5f;
    public float pushForce;

    private Animator animator;
    private bool isRunning = false;
    private bool isAttacking = false;
    public float movespeed,saldiriMiktar;
    public GameObject player;
    public KarakterDurum KarakterDurum_;

    void Start()
    {
        animator = GetComponent<Animator>();
        movespeed = speed;
      
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer < playerDistance)
        {
            switch (behavior)
            {
                case BehaviorType.Korkak:
                    HandleCowardBehavior(distanceToPlayer);
                    break;

                case BehaviorType.Saldaggan:
                    HandleAggressiveBehavior(distanceToPlayer);
                    break;
            }
        }
        else
        {
            ResetState();
        }

        UpdateAnimations();
    }

    private void HandleCowardBehavior(float distanceToPlayer)
    {
        isRunning = true;
        Vector3 awayFromPlayer = transform.position - player.transform.position;
        transform.rotation = Quaternion.LookRotation(awayFromPlayer);
        transform.position += transform.forward * movespeed * Time.deltaTime;
    }

    private void HandleAggressiveBehavior(float distanceToPlayer)
    {
        if (distanceToPlayer < attackRange && !isAttacking)
        {
            StartAttack();
            InvokeRepeating("saldir",.3f,1);
        }
        else if (distanceToPlayer >= attackRange)
        {
            ChasePlayer();
            CancelInvoke("saldir");  // Burada durduruyoruz
        }
    }

    public void saldir()
    {
        KarakterDurum_.DegistirCan(saldiriMiktar);
    }

    private void StartAttack()
    {
        isAttacking = true;
        isRunning = false;
        movespeed = 0;
        animator.SetBool("isAttacking", true);
    }

    private void ChasePlayer()
    {
        isAttacking = false;
        isRunning = true;
        movespeed = speed;
        Vector3 toPlayer = player.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(toPlayer);
        transform.position += transform.forward * movespeed * Time.deltaTime;
    }

    private void ResetState()
    {
        isRunning = false;
        isAttacking = false;
        movespeed = speed;
        CancelInvoke("saldir");  // Burada durduruyoruz
    }

    private void UpdateAnimations()
    {
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isAttacking", isAttacking);
    }
}