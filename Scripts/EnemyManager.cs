using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{
    public GameObject perguntaPrefab; // O prefab que contém o TextMeshPro
    private PerguntasManager perguntasManager;
    public GameObject enemyPrefab;
    public Transform player;
    public float spawnDistance = 30f;
    public int enemyLimit = 10;
    public float spawnDelay = 5f;
    private List<GameObject> spawnedEnemies = new List<GameObject>();

    private void Start()
    {
        perguntasManager = FindObjectOfType<PerguntasManager>();
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            if (spawnedEnemies.Count < enemyLimit)
            {
                SpawnEnemy();
            }
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    void SpawnEnemy()
    {
        Vector3 spawnPosition;
        if (TryGetRandomNavMeshPosition(out spawnPosition))
        {
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();

            if (enemyAI != null)
            {
                spawnedEnemies.Add(enemy);
                Pergunta pergunta = perguntasManager.GetRandomPergunta();

                if (pergunta != null)
                    InicializarPergunta(enemyAI, enemy, pergunta);
            }
        }
    }

    bool TryGetRandomNavMeshPosition(out Vector3 result)
    {
        Vector3 randomDirection = Random.insideUnitSphere * spawnDistance;
        randomDirection += player.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, spawnDistance, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }
        result = Vector3.zero;
        return false;
    }

    public void RemoverInimigo(GameObject enemy)
    {
        if (spawnedEnemies.Contains(enemy))
        {
            spawnedEnemies.Remove(enemy); // Remove da lista
            Destroy(enemy);               // Destroi o GameObject
        }
    }

    public void InicializarPergunta(EnemyAI enemyAI, GameObject enemy, Pergunta pergunta)
    {

        float yValue = 4.5f, yImagem = 50, xImagem = 100;

        if (pergunta != null)
        {

            switch (pergunta.status)
            {
                case "pitagoras":
                    yValue = 5.8f;
                    yImagem = 140;
                    xImagem = 150;
                    break;
                

                default:
                    break;
            }

            


            GameObject perguntaObject = Instantiate(perguntaPrefab, enemy.transform.position + new Vector3(0, yValue, 0), Quaternion.identity);

            // Definir a Imagem.
            Image imagemPai = perguntaObject.GetComponentInChildren<Image>();
            imagemPai.rectTransform.sizeDelta = new Vector2(xImagem, yImagem); // Define o tamanho da imagem

            // Definir a pergunta.
            enemyAI.perguntaAtual = pergunta;
            TextMeshProUGUI textoPergunta = perguntaObject.GetComponentInChildren<TextMeshProUGUI>();
            textoPergunta.text = pergunta.pergunta; // Define o texto da pergunta
            perguntaObject.transform.SetParent(enemy.transform);
        }
        else
        {
            UnityEngine.Debug.LogError("Erro: Pergunta é nula.");
        }
    }
}




// if (jsonFile == null) 
// {
//     Debug.LogError("Erro: O arquivo JSON 'perguntas' não foi encontrado em Assets/Resources.");
//     return;
// }
// else
//     Debug.Log("JSON 'perguntas' carregado com sucesso.");


// if (perguntasList == null || perguntasList.perguntas == null)
//         {
//             Debug.LogError("Erro: Falha ao desserializar o JSON para PerguntasList.");
//             return;
//         }