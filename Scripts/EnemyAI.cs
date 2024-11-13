using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform player;
    public float repulsionRadius = 5f;    // Distância mínima entre inimigos
    public float repulsionForce = 3f;     // Força de repulsão
    public float attackRange = 5f;        // Distância mínima para "matar" o player

    public bool life = true;

    public Pergunta perguntaAtual;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        if (player != null && life)
        {
            // Faz o inimigo olhar e se mover em direção ao player
            Vector3 directionToPlayer = player.position - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            agent.SetDestination(player.position);

            ApplyRepulsion();
            CheckPlayerDistance();
        }
    }

    void ApplyRepulsion()
    {
        Collider[] nearbyEnemies = Physics.OverlapSphere(transform.position, repulsionRadius);

        foreach (var collider in nearbyEnemies)
        {
            if (collider.gameObject != gameObject && collider.CompareTag("Enemy"))
            {
                Vector3 directionAway = transform.position - collider.transform.position;
                agent.velocity += directionAway.normalized * repulsionForce * Time.deltaTime;
            }
        }
    }

    void CheckPlayerDistance()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        Debug.Log("distancia: " + distanceToPlayer);

        if (distanceToPlayer <= attackRange)
        {
            Debug.Log("ESTOU AQUI.");
            // Se o inimigo estiver a 1 metro ou menos do player, desativa o movimento e a rotação do player
            FirstPersonController playerController = player.GetComponent<FirstPersonController>();
            if (playerController != null)
            {
                Debug.Log("A VIDA DO OMI FOI RETIRADA!");
                playerController.life = false;
            }

            // Aqui você também pode adicionar uma lógica para "matar" o jogador ou reiniciar o jogo
            Debug.Log("Player foi alcançado e está incapacitado!");
        }
    }
}
