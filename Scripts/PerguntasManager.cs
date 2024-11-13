using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pergunta
{
    public int id;
    public string pergunta;
    public string status;
    public List<float> respostas;
}

[System.Serializable]
public class PerguntasList
{
    public List<Pergunta> perguntas;
}

public class PerguntasManager : MonoBehaviour
{
    public PerguntasList perguntasList;

    private void Awake()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("perguntas");
        if (jsonFile == null)
        {
            Debug.LogError("Erro: O arquivo JSON 'perguntas' não foi encontrado em Assets/Resources.");
            return;
        }

        perguntasList = JsonUtility.FromJson<PerguntasList>(jsonFile.text);
        if (perguntasList == null || perguntasList.perguntas == null)
        {
            Debug.LogError("Erro: Falha ao desserializar o JSON para PerguntasList.");
            return;
        }

        Debug.Log("JSON 'perguntas' carregado com sucesso.");
    }

    public Pergunta GetRandomPergunta()
    {
        Debug.Log(perguntasList.perguntas.Count);
        if (perguntasList.perguntas.Count > 0)
        {
            int randomIndex = Random.Range(0, perguntasList.perguntas.Count);
            return perguntasList.perguntas[randomIndex];
        }
        return null;
    }
}
