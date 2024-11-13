using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerShooting : MonoBehaviour
{
    public Camera playerCamera;        // Referência à câmera do jogador

    public float shootingRange = 100f; // Alcance do tiro
    public GameObject weapon;          // Referência à pistola (GameObject da arma)
    public float recoilAmount = 5f;    // Ângulo do recoil
    public float recoilSpeed = 10f;    // Velocidade do recoil de volta
    public LineRenderer tracerEffect;  // LineRenderer para o efeito de feixe de luz

    public TextMeshProUGUI bulletText; // Referência ao TextMeshPro UI para exibir o valor do bullet
    private int bullet = 0;            // Inicializa o contador de bullets
    private int bulletLimit = 10;      // Limite superior e inferior do contador de bullets

    private Quaternion originalRotation;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip gunShot;

    public float lightDuration = 0.05f;
    public GameObject muzzleFlare;
    public Light muzzleLight;

    private ParticleSystem muzzleParticles;


    void Start()
    {
        muzzleParticles = muzzleFlare.GetComponent<ParticleSystem>();
        muzzleLight.enabled = false; // Deixe a luz desativada por padrã
        if (weapon != null)
        {
            originalRotation = weapon.transform.localRotation; // Guarda a rotação inicial da arma
        }
        UpdateBulletText(); // Atualiza o texto do contador inicialmente

        if (tracerEffect != null)
        {
            tracerEffect.enabled = false; // Desativa o feixe inicialmente
        }
    }

    void Update()
    {
        HandleScrollInput(); // Verifica o input do scroll para alterar o contador

        if (Input.GetMouseButtonDown(0) ) // Verifica se o botão esquerdo do mouse foi pressionado
        {
            muzzleParticles.Play();
            StartCoroutine(FlashMuzzleLight());
            audioSource.PlayOneShot(gunShot);
            Shoot();
            ApplyRecoil();
        }

        // Volta a arma para a posição original suavemente
        if (weapon != null)
        {
            weapon.transform.localRotation = Quaternion.Lerp(weapon.transform.localRotation, originalRotation, Time.deltaTime * recoilSpeed);
        }
    }

    void Shoot()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0)); // Ray a partir do centro da câmera
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * shootingRange, Color.red, 1.25f);

        if (Physics.Raycast(ray, out hit, shootingRange))
        {
            GameObject enemyRoot = hit.collider.transform.root.gameObject;
            if (enemyRoot.CompareTag("Enemy"))
            {
                EnemyAI enemyAI = enemyRoot.GetComponent<EnemyAI>();
                if (enemyAI != null && enemyAI.perguntaAtual != null)
                {
                    Debug.Log("Resposta: " + enemyAI.perguntaAtual.respostas[0]);
                }

                EnemyManager manager = FindObjectOfType<EnemyManager>();
                if (manager != null)
                {
                    if(enemyAI.perguntaAtual.respostas[0] == bullet)
                        manager.RemoverInimigo(enemyRoot);
                }
            }

            // Ativa o efeito de feixe de luz (tracer) do tiro
            if (tracerEffect != null)
            {
                tracerEffect.SetPosition(0, weapon.transform.position); // Origem do feixe na arma
                tracerEffect.SetPosition(1, hit.point);                // Destino do feixe no ponto de impacto
                StartCoroutine(ShowTracer());
            }
        }
    }

    void ApplyRecoil()
    {
        if (weapon != null)
        {
            // Aplica uma rotação temporária para simular o recoil
            weapon.transform.localRotation = Quaternion.Euler(originalRotation.eulerAngles + new Vector3(-recoilAmount, 0, 0));
        }
    }

    void HandleScrollInput()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f && bullet < bulletLimit) // Scroll para cima, aumenta o contador até o limite
        {
            bullet++;
            UpdateBulletText();
        }
        else if (scroll < 0f && bullet > -bulletLimit) // Scroll para baixo, diminui o contador até o limite
        {
            bullet--;
            UpdateBulletText();
        }
    }

    void UpdateBulletText()
    {
        if (bulletText != null)
        {
            bulletText.text = bullet.ToString(); // Atualiza o texto do UI com o valor atual de bullet
        }
    }

    // Coroutine para exibir o feixe de luz por um curto período
    private System.Collections.IEnumerator ShowTracer()
    {
        tracerEffect.enabled = true;
        yield return new WaitForSeconds(0.05f); // Duração do feixe em segundos
        tracerEffect.enabled = false;
    }

    private IEnumerator FlashMuzzleLight()
    {
        muzzleLight.enabled = true;
        yield return new WaitForSeconds(lightDuration);
        muzzleLight.enabled = false;
    }
}
