using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Game : MonoBehaviour
{
    public List<WorldChunk> chunks = new List<WorldChunk>();
    public float chunkLoadDistance = 30;

    public List<Enemy> enemies = new List<Enemy>();
    public float minEnemyDistance = 30;
    public float maxEnemyDistance = 40;
    float lastEnemyDistance = 0;
    Enemy nextEnemy = null;

    List<WorldChunk> chunkInstances = new List<WorldChunk>();
    float loadedChunkDistance = 0;

    public Player player = null;
    public Transform gameOver = null;

    public enum State
    {
        Idle,
        Moving,
        Fighting,
        Dead
    }
    public State state = State.Idle;
    public float movementSpeed = 0;

	// Use this for initialization
	void Start ()
    {
        loadedChunkDistance = -chunkLoadDistance;
        SpawnNextEnemy();
        gameOver.gameObject.SetActive(false);
        SetState(State.Moving);
    }
	
	// Update is called once per frame
	void Update ()
    {
        UpdateChunks();

        switch (state)
        {
            case State.Idle:
                break;
            case State.Moving:
                player.transform.position += new Vector3(movementSpeed * Time.deltaTime, 0, 0);
                if(nextEnemy && player.transform.position.x >= nextEnemy.transform.position.x)
                {
                    SetState(State.Fighting);
                }
                break;
            case State.Fighting:
                player.enemy = nextEnemy;
                nextEnemy.player = player;
                if (player.health.health <= 0)
                {
                    SetState(State.Dead);
                    gameOver.gameObject.SetActive(true);
                }
                else if (nextEnemy == null || nextEnemy.health.health <= 0)
                {
                    SetState(State.Moving);
                    SpawnNextEnemy();
                }
                break;
            case State.Dead:
                if (Input.GetMouseButtonDown(0))
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
                break;
        }
        
	}

    void UpdateChunks()
    {
        //generate new chunks
        while(loadedChunkDistance < player.transform.position.x + chunkLoadDistance)
        {
            WorldChunk newChunk = CreateChunk(new Vector3(loadedChunkDistance, 0, 0));
            chunkInstances.Add(newChunk);
            loadedChunkDistance += newChunk.width;
        }

        //cleanup old chunks
        for (int i = chunkInstances.Count - 1; i >= 0; i--)
        {
            WorldChunk chunk = chunkInstances[i];
            if (chunk.transform.position.x < player.transform.position.x - chunkLoadDistance)
            {
                Destroy(chunk.gameObject);
                chunkInstances.RemoveAt(i);
            }
        }
    }

    WorldChunk CreateChunk(Vector3 pos)
    {
        WorldChunk prefab = chunks[Random.Range(0, chunks.Count)];
        GameObject gobj = GameObject.Instantiate(prefab.gameObject);
        WorldChunk instance = gobj.GetComponent<WorldChunk>();
        instance.transform.position = pos;
        return instance;
    }

    void SpawnNextEnemy()
    {
        Enemy prefab = enemies[Random.Range(0, enemies.Count)];
        GameObject gobj = GameObject.Instantiate(prefab.gameObject);
        Enemy instance = gobj.GetComponent<Enemy>();

        float distanceFromLast = Random.Range(minEnemyDistance, maxEnemyDistance);
        float distance = lastEnemyDistance + distanceFromLast;
        instance.transform.position = new Vector3(distance, 0, 0);

        lastEnemyDistance = distance;
        nextEnemy = instance;
    }

    void SetState(State newState)
    {
        if(newState == state)
        {
            return;
        }

        switch (newState)
        {
            case State.Idle:
                player.SetState(Player.State.Idle);
                break;
            case State.Moving:
                player.SetState(Player.State.Walking);
                break;
            case State.Fighting:
                player.SetState(Player.State.Fighting);
                break;
            case State.Dead:
                player.SetState(Player.State.Dead);
                break;
        }

        state = newState;
    }
}
