using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Transform player; //punto en el espacio --- **antes llamada destination

    private NavMeshAgent _agent; //guardar referencia

    private float visionRange = 10f; //radios de vision
    private float attackRange = 5f;

    private bool playerInVisionRange;
    private bool playerInAttackRange;

    [SerializeField] private LayerMask playerLayer;

    //PATRULLA
    [SerializeField] private Transform[] waypoints;
    private int totalWaypoints;
    private int nextPoint;

    //Ataque
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform spawnPoint; //punto de instancia bullet
    private float timeBetweenAttacks = 2f;
    private bool canAttack;
    private float upAttackForce = 15f;
    private float forwardAttackForce = 18f;

    private void Start()
    {
        totalWaypoints = waypoints.Length; //cuantos puntos hay y que los recorra y vuelva a empezar
        nextPoint = 1;
        canAttack = false; //así llama a la corrutina
    }
    private void Awake() 
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        Vector3 pos = transform.position;
        playerInVisionRange = Physics.CheckSphere(pos, visionRange, playerLayer); //si esta en rango de visión
        playerInAttackRange = Physics.CheckSphere(pos, attackRange, playerLayer); //rango mas pequeño

        if(!playerInVisionRange && !playerInAttackRange) //NO y NO
        {
            Patrol();
        }
        if(playerInVisionRange && !playerInAttackRange) //SI y NO
        {
            Chase();
        }
        if(playerInVisionRange && playerInAttackRange) //SI y SI
        {
            Attack();
        }
           
        //_agent.SetDestination(player.position); // su destino sera la posicion de ese punto
    }

    private void Patrol() //array de puntos, recorrido 
    {
        if(Vector3.Distance(transform.position, waypoints[nextPoint].position) < 2.5f) //al minimo para que cambie de dirección/punto
        {
            nextPoint++;
            if (nextPoint == totalWaypoints) //reinicio del array
            {
                nextPoint = 0;
            }
            transform.LookAt(waypoints[nextPoint].position); //que mire siempre de frente al punto
        }
        _agent.SetDestination(waypoints[nextPoint].position);
    }

    private void Chase()
    {
       _agent.SetDestination(player.position); //persigue al player cuando entra dentro de su radio
        transform.LookAt(player);

    }
    private void Attack()
    {
        _agent.SetDestination(transform.position);//que se quede quieto hasta que le digan lo contrario
        if (canAttack)
        {
            Rigidbody rigidbody = Instantiate(bullet, spawnPoint.position, Quaternion.identity).GetComponent<Rigidbody>(); //Me quedo con el Rig del BULLET
            rigidbody.AddForce(transform.forward * forwardAttackForce, ForceMode.Impulse); //la trayectoria
            rigidbody.AddForce(transform.up *upAttackForce, ForceMode.Impulse);

            canAttack = false;
            StartCoroutine(AttackCoolDown()); //despues de atacar, esperas 2 segundos para volver hacerlo
        }
    }

    private IEnumerator AttackCoolDown()
    {
        yield return new WaitForSeconds(timeBetweenAttacks);
        canAttack = true;
    }

    private void OnDrawGizmos()
    {
        //Esfera visión
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        //Esfera ataque
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
