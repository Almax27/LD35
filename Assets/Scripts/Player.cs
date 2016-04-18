using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour {

    public Sprite standing = null;
    public List<Sprite> walkCycle = new List<Sprite>();
    public Sprite attacking = null;

    public SpriteRenderer spriteRenderer = null;

    public Spinner spinner = null;

    public Animator attackAnimatorPrefab = null;

    public Transform shield = null;

    public Enemy enemy = null;

    public Health health = null;
    public TextMesh healthText = null;

    public TextMesh distanceText = null;

    public enum State
    {
        Idle,
        Walking,
        Fighting,
        Dead
    }

    Animator animator = null;

    public State state = State.Idle;
    public float walkingAnimSpeed = 0.5f;
    public float walkingAnimTick = 0;
    public int walkCycleIndex = 0;

    // Use this for initialization
    void Start () {
        shield.gameObject.SetActive(false);
        health = GetComponent<Health>();
        animator = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
        switch (state)
        {
            case State.Idle:
                break;
            case State.Walking:
                walkingAnimTick += Time.deltaTime;
                float t = walkingAnimTick / walkingAnimSpeed;
                if (t >= 1)
                {
                    walkingAnimTick = 0;
                    t = 0;
                }

                int index = Mathf.FloorToInt(t * walkCycle.Count);
                spriteRenderer.sprite = walkCycle[index];

                break;
            case State.Fighting:
                if (Input.GetMouseButtonDown(1) && !spinner.isRunning)
                {
                    spriteRenderer.sprite = attacking;
                    shield.gameObject.SetActive(true);
                    GetComponent<Health>().blockDamage = true;
                }
                else if(Input.GetMouseButtonUp(1) && !spinner.isRunning)
                {
                    spriteRenderer.sprite = standing;
                    shield.gameObject.SetActive(false);
                    GetComponent<Health>().blockDamage = false;
                }
                else if(!shield.gameObject.activeSelf)
                {
                    if (!shield.gameObject.activeSelf && Input.GetMouseButtonDown(0))
                    {
                        spriteRenderer.sprite = attacking;

                        if (spinner.Trigger())
                        {
                            //complete hit
                            GameObject gobj = Instantiate<GameObject>(attackAnimatorPrefab.gameObject);
                            Vector3 p = gobj.transform.position;
                            p.x = this.transform.position.x;
                            gobj.transform.position = p;

                            if (enemy)
                            {
                                Health enemyHealth = enemy.GetComponent<Health>();
                                enemyHealth.hurt(1, 0.8f);
                            }
                        }
                        else
                        {
                            //miss!
                        }
                    }
                    else if (!spinner.isRunning)
                    {
                        spriteRenderer.sprite = standing;
                    }
                }
                if(health.health <= 0)
                {
                    SetState(State.Dead);
                }
                break;
            case State.Dead:

                break;
        }

        if(health)
        {
            if(health.health <= 0)
            {
                //on death
                SetState(State.Dead);
            }
            healthText.text = string.Format("{0}/{1}", health.health, health.maxHealth);
        }
        if(distanceText)
        {
            distanceText.text = string.Format("{0}", (int)transform.position.x);
        }

        float difficulty = Mathf.Min(2, 1 + transform.position.x / 200);
        spinner.difficulty = difficulty;
        if(enemy)
        {
            enemy.difficulty = difficulty;
        }
    }

    public void SetState(State newState)
    {
        if(state == newState)
        {
            return;
        }

        shield.gameObject.SetActive(false);
        GetComponent<Health>().blockDamage = false;
        spinner.Reset();

        switch (newState)
        {
            case State.Idle:
                spriteRenderer.sprite = standing;
                break;
            case State.Walking:
                break;
            case State.Fighting:
                spriteRenderer.sprite = standing;
                break;
            case State.Dead:
                animator.SetTrigger("Death");
                if(enemy)
                {
                    enemy.enabled = false;
                }
                break;
        }
        state = newState;
    }
}
