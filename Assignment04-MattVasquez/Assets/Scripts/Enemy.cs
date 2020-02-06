using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private EnemyAIStates state = EnemyAIStates.Patrolling;
    static private List<GameObject> checkpoints = null;

    #region Enemy Options
    public float walkingSpeed = 6.0f;
    public float chasingSpeed = 7.75f;
    public float attackingSpeed = 1.5f;
    public float attackingDistance = 1.0f;
    public float nextJumpTime = 3.0f;
    #endregion

    private GameObject checkpoint;
    private GameObject playerOfInterest;

    #region Standard MonoBehaviour Methods
    void Start()
    {
        if (checkpoints == null)
        {
            checkpoints = new List<GameObject>();
            foreach (GameObject go in GameObject.FindGameObjectsWithTag("Checkpoint"))
            {
                Debug.Log("Checkpoint: " + go.transform.position);
                checkpoints.Add(go);
            }
        }
        ChangeToPatrolling();
    }

    void Update()
    {
        switch (state)
        {
            case EnemyAIStates.Attacking:
                OnAttackingUpdate();
                break;
            case EnemyAIStates.Chasing:
                OnChasingUpdate();
                break;
            case EnemyAIStates.Patrolling:
                OnPatrollingUpdate();
                break;
            case EnemyAIStates.Idle:
                OnIdleUpdate();
                break;
        }
    }
    #endregion

    #region Update Methods
    void OnAttackingUpdate()
    {
        float step = attackingSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, playerOfInterest.transform.position, step);

        float distance = Vector3.Distance(transform.position, playerOfInterest.transform.position);
        if (distance > attackingDistance)
        {
            ChangeToChasing(playerOfInterest);
        }
    }

    void OnChasingUpdate()
    {
        float step = chasingSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, playerOfInterest.transform.position, step);

        float distance = Vector3.Distance(transform.position, playerOfInterest.transform.position);
        if (distance <= attackingDistance)
        {
            ChangeToAttacking(playerOfInterest);
        }
    }

    void OnPatrollingUpdate()
    {
        float step = walkingSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, checkpoint.transform.position, step);

        float distance = Vector3.Distance(transform.position, checkpoint.transform.position);
        if (distance == 0)
        {
            SelectRandomPatrolPoint();
        }
    }

    void OnIdleUpdate()
    {
        Debug.Log("Idle");
        ChangeToPatrolling();
    }
    #endregion

    #region Collider Methods
    void OnTriggerEnter(Collider collider)
    {
        ChangeToChasing(collider.gameObject);
    }

    void OnTriggerExit(Collider collider)
    {
        ChangeToPatrolling();
    }
    #endregion

    #region Changes to states
    void ChangeToPatrolling()
    {
        state = EnemyAIStates.Patrolling;
        GetComponent<Renderer>().material.color = new Color(1.0f, 0.0f, 0.0f);
        SelectRandomPatrolPoint();
        playerOfInterest = null;
    }

    void ChangeToAttacking(GameObject target)
    {
        state = EnemyAIStates.Attacking;
        GetComponent<Renderer>().material.color = new Color(0.0f, 1.0f, 0.0f);
    }

    void ChangeToChasing(GameObject target)
    {
        state = EnemyAIStates.Chasing;
        GetComponent<Renderer>().material.color = new Color(1.0f, 1.0f, 0.0f);
        playerOfInterest = target;
    }

    void ChangeToIdle(GameObject target)
    {
        state = EnemyAIStates.Idle;
        GetComponent<Renderer>().material.color = new Color(1.0f, 1.0f, 1.0f);
        playerOfInterest = target;
    }
    #endregion

    void SelectRandomPatrolPoint()
    {
        int choice = Random.Range(0, checkpoints.Count);
        checkpoint = checkpoints[choice];
        Debug.Log("Enemy going to patrol to point " + checkpoint.name);
    }
}