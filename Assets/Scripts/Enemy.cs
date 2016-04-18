using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

    public Player player = null;
    public Health health;
    Animator animator;

    public float minAttackRate = 2.0f;
    public float maxAttackRate = 3.0f;
    float attackTick = 0;
    bool isDead = false;

    public float difficulty = 1;

	// Use this for initialization
	void Start () {
        health = GetComponent<Health>();
        animator = GetComponent<Animator>();
        attackTick = Random.Range(minAttackRate, maxAttackRate);
    }
	
	// Update is called once per frame
	void Update () {
        if(isDead)
        {
            return;
        }

        if (!isDead && health.health <= 0)
        {
            animator.SetTrigger("Death");
            isDead = true;
            return;
        }
        if (player)
        {
            attackTick -= Time.deltaTime * difficulty;
            if (attackTick <= 0)
            {
                attackTick = Random.Range(minAttackRate, maxAttackRate);
                animator.SetTrigger("Attack");
            }
        }
        
    }

    public void DamagePlayer()
    {
        Health playerHealth = player.GetComponent<Health>();
        playerHealth.hurt(1);
    }

    public void Destory()
    {
        Destroy(this.gameObject);
    }
}
