using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {

    public float maxHealth = 10;
    public float health = 10;
    public Transform toRotate;
    public float rotationOnHit = 10;
    public float rotationDuration = 0.2f;
    float rotationTick = 0;

    float damageTick = 0;
    float pendingDamage = 0;

    public AudioSource soundOnHit = null;
    public AudioSource soundOnBlock = null;

    public void hurt(float amount, float delay = 0)
    {
        if (delay <= 0)
        {
            hurt_internal(amount);
        }
        else
        {
            pendingDamage = amount;
            damageTick = delay;
        }
    }

    public bool blockDamage = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (damageTick > 0)
        {
            damageTick -= Time.deltaTime;
            if(damageTick <= 0)
            {
                hurt_internal(pendingDamage);
                pendingDamage = 0;
            }
        }

	    if(rotationTick > 0)
        {
            rotationTick -= Time.deltaTime;
            if(toRotate && rotationTick <= 0)
            {
                toRotate.rotation = Quaternion.identity;
            }
        }
	}

    void hurt_internal(float amount)
    {
        if (!blockDamage)
        {
            health -= amount;
            toRotate.rotation = Quaternion.Euler(0, 0, rotationOnHit);
            rotationTick = rotationDuration;
            if (soundOnHit)
            {
                Instantiate<GameObject>(soundOnHit.gameObject);
            }
        }
        else
        {
            if (soundOnBlock)
            {
                Instantiate<GameObject>(soundOnBlock.gameObject);
            }
        }
    }
}
