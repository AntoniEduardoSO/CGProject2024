using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AnimationStateController : MonoBehaviour
{
    public NavMeshAgent agent; // Referência ao NavMeshAgent do inimigo
    public Transform player;   // Referência ao transform do jogador
    public float followDistance = 1000f; // Distância em que o inimigo começa a seguir o jogador
    private int isWalkingHash;
    private int isDeathHash;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        isWalkingHash = Animator.StringToHash("isWalking");
        isDeathHash = Animator.StringToHash("isDeath");
    }

    void Update()
    {
        if (agent != null)
        {
            animator.SetBool(isWalkingHash, true);
        }

        if (Input.GetKey("n"))
        {
            animator.SetBool(isDeathHash, true);
        }
    }
}
