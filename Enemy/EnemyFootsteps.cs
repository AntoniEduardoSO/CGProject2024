using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(AudioSource), typeof(NavMeshAgent))]
public class EnemyFootsteps : MonoBehaviour
{
    public AudioClip[] footstepSounds;    // Array de sons de passos
    public AudioClip laughSound;          // Som de risada
    public float footstepInterval = 0.5f; // Intervalo entre os passos
    public float laughChance = 0.1f;      // Chance de risada (10%)

    private AudioSource audioSource;
    private NavMeshAgent agent;
    private Transform player;
    private float stepTimer;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        agent = GetComponent<NavMeshAgent>();

        // Configura o áudio para que seja espacial
        audioSource.spatialBlend = 1f; // 1 = som 3D

        // Procurar o jogador usando a tag "Player" ou o nome "FirstPersonController"
        GameObject playerObject = GameObject.FindWithTag("Player") ?? GameObject.Find("FirstPersonController");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player não encontrado. Verifique se a tag 'Player' ou o nome 'FirstPersonController' está corretamente configurado.");
        }
    }

    void Update()
    {
        if (player == null) return;

        // Somente tocar passos enquanto o inimigo está se movendo
        if (agent.velocity.magnitude > 0.1f && agent.remainingDistance > agent.stoppingDistance)
        {
            stepTimer += Time.deltaTime;

            if (stepTimer >= footstepInterval)
            {
                PlayFootstep();
                stepTimer = 0;
            }

            // Verificar a distância para o jogador e, se estiver entre 50 e 100, tentar rir
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer >= 50f && distanceToPlayer <= 100f)
            {
                TryLaugh();
            }
        }
        else
        {
            // Reinicia o timer de passos se o inimigo parar
            stepTimer = 0;
        }
    }

    private void PlayFootstep()
    {
        if (footstepSounds.Length > 0)
        {
            // Seleciona um som aleatório de passos
            AudioClip clip = footstepSounds[Random.Range(0, footstepSounds.Length)];
            audioSource.PlayOneShot(clip);
        }
    }

    private void TryLaugh()
    {
        // Verifica a chance de 10% de rir
        if (!audioSource.isPlaying && Random.value < laughChance)
        {
            audioSource.volume = 1;
            audioSource.PlayOneShot(laughSound);
            
        }
    }
}
