using UnityEngine;
using TMPro;
using System.Collections;

public class DayNightCycle : MonoBehaviour
{
    [SerializeField] private Light sun;                    // A luz direcional que representa o Sol
    [SerializeField, Range(0, 24)] private float timeOfDay = 15f;    
    
    [SerializeField] private float sunRotationSpeed = 1f;                   
    [SerializeField] private Gradient sunColor;
    [SerializeField] private Gradient skyColor;
    [SerializeField] private Gradient equatorColor;   
    [SerializeField] private AnimationCurve fogDensity;

    [SerializeField] private float updateInterval = 0.1f; // Intervalo de atualização em segundos
    private float updateTimer = 0f;

    private void Start()
    {
        StartCoroutine(UpdateCycle());
    }

    private IEnumerator UpdateCycle()
    {
        while (true)
        {
            timeOfDay += Time.deltaTime * sunRotationSpeed;
            if (timeOfDay > 24) 
            { 
                timeOfDay = 0; 
            }
            
            UpdateSunRotation();
            UpdateLighting();

            yield return new WaitForSeconds(updateInterval); // Espera pelo intervalo antes da próxima atualização
        }
    }

    private void UpdateSunRotation()
    {
        float currentTime = timeOfDay / 24f;
        float sunRotation = Mathf.Lerp(-90f, 270f, currentTime);
        sun.transform.rotation = Quaternion.Euler(sunRotation, 0f, 0f);
    }

    private void UpdateLighting()
    {
        float currentTime = timeOfDay / 24f;
        
        // Atualiza as propriedades de iluminação com base no ciclo do dia
        sun.color = sunColor.Evaluate(currentTime);
        RenderSettings.ambientEquatorColor = equatorColor.Evaluate(currentTime);
        RenderSettings.ambientSkyColor = skyColor.Evaluate(currentTime);
        RenderSettings.fogDensity = fogDensity.Evaluate(currentTime);
    }

    // Método para validar as configurações de iluminação durante a edição
    private void OnValidate()
    {
        UpdateSunRotation();
        UpdateLighting();
    }
}
