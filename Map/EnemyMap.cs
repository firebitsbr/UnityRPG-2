using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMap : MonoBehaviour
{
    public BattleEncounter encounter;
    public float speed;
    public float moveDelayMin;
    public float moveDelayMax;
    float currentMoveDelay;

    public float viewArea;
    GameObject player;
    Rigidbody2D body;

    void Start()
    {
        player = GameObject.Find("Player");
        body = GetComponent<Rigidbody2D>();
        currentMoveDelay = moveDelayMax;
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        if (distanceToPlayer <= viewArea)
        {
            var direction = (player.transform.position - transform.position).normalized;
            transform.Translate(direction * Time.deltaTime * speed);
        }
        else if (currentMoveDelay <= 0)
        {
            var randomDirection = new Vector2(Random.Range(0, 200), Random.Range(0, 200));
            body.AddForce(randomDirection * speed);
            currentMoveDelay = Random.Range(moveDelayMin, moveDelayMax);
        }
        else
        {
            currentMoveDelay -= Time.deltaTime;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewArea);
    }
}
