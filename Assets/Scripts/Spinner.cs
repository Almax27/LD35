using UnityEngine;
using System.Collections.Generic;

public class Spinner : MonoBehaviour {

    public Transform needle = null;
    public float needleSpeed = 0;
    public float difficulty = 1;
    float needleAngle = 0;

    [System.Serializable]
    public class Segment
    {
        public float startAngle = 0;
        public float endAngle = 0;
        public bool wasHit = false;
        public Transform hitEffect = null;
    }
    public List<Segment> segments = new List<Segment>();
    public Transform missEffect = null;
    public bool isRunning = false;
    public int hitCount = 0;
    public bool missed = false;
    float missedTick = 0;

    public Animator animator = null;
    public Transform root = null;

    public AudioSource soundOnHit = null;
    public AudioSource soundOnMiss = null;

	// Use this for initialization
	void Start ()
    {
        Reset();
    }
	
	// Update is called once per frame
	void Update () {
        if(missed)
        {
            missedTick += Time.deltaTime;
            float s = Mathf.Min(1.0f, missedTick / 0.2f);
            missEffect.localScale = new Vector3(s, s, s);
            if (missedTick > 0.5f)
            {
                Reset();
            }
        }
        else if (isRunning)
        {
            needleAngle += needleSpeed * Time.deltaTime * difficulty;
            if (needleAngle >= 360)
            {
                needleAngle = 360;
                Reset();
            }
            needle.localRotation = Quaternion.AngleAxis(-needleAngle, Vector3.forward);
        }
    }

    public bool Trigger()
    {
        if(missed)
        {
            return false;
        }

        if(!isRunning)
        {
            isRunning = true;
            root.gameObject.SetActive(true);
            return false;
        }

        foreach (Segment seg in segments)
        {
            if (seg.wasHit == false && needleAngle > seg.startAngle && needleAngle < seg.endAngle)
            {
                seg.wasHit = true;
                if (seg.hitEffect)
                {
                    seg.hitEffect.gameObject.SetActive(true);
                }
                print("Hit!");
                hitCount += 1;

                if (soundOnHit)
                {
                    Instantiate<GameObject>(soundOnHit.gameObject);
                }

                return hitCount == segments.Count;
            }
        }

        print("Miss!");
        missed = true;
        missEffect.localScale = Vector3.zero;
        missEffect.gameObject.SetActive(true);

        if (soundOnMiss)
        {
            Instantiate<GameObject>(soundOnMiss.gameObject);
        }

        return false;
    }

    public void Reset()
    {
        foreach (Segment seg in segments)
        {
            seg.wasHit = false;
            if (seg.hitEffect)
            {
                seg.hitEffect.gameObject.SetActive(false);
            }
        }
        needleAngle = 0;
        hitCount = 0;
        isRunning = false;
        missed = false;
        missedTick = 0;
        missEffect.gameObject.SetActive(false);
        root.gameObject.SetActive(false);
    }
}
