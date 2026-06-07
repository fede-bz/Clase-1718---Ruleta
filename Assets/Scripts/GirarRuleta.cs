using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using TMPro;
using System.Collections;

public class GirarRuleta : MonoBehaviour
{
    [System.Serializable]
    public class Sector
    {
        public string nombre;
        public float anguloInicio;
        public float anguloFin;
    }
    [Header("Referencias")]
    public MonedaManager monedaManager;
    public TextMeshProUGUI textoResultado;
    [Header("Efectos")]
    [SerializeField] GameObject victoriaPS;
    [SerializeField] GameObject derrotaPS;
    [SerializeField] PostProcessVolume postProcessVolume;
    [Header("Sectores")]
    public Sector[] sectores;
    [Header("Animacion")]
    public AnimationCurve[] curvas;
    public float duracionGiro = 5f;
    public float vueltasMinimas = 3f;
    public float vueltasExtra = 3f;
    private bool estaGirando = false;
    private Vignette vignette;

    void Start()
    {
        if (postProcessVolume != null)
            postProcessVolume.profile.TryGetSettings(out vignette);
    }

    public void Girar()
    {
        if (estaGirando) return;
        StartCoroutine(CorrutinaGiro());
    }
    private IEnumerator CorrutinaGiro()
    {
        estaGirando = true;
        textoResultado.text = "";
        AnimationCurve curva = curvas[Random.Range(0, curvas.Length)];
        float gradosPorSector = 360f / sectores.Length;
        float rotacionFinal = vueltasMinimas * 360f + vueltasExtra * Random.value * 360f;
        float rotacionInicial = transform.eulerAngles.y;
        float tiempoTranscurrido = 0f;
        while (tiempoTranscurrido < duracionGiro)
        {
            tiempoTranscurrido += Time.deltaTime;
            float t = Mathf.Clamp01(tiempoTranscurrido / duracionGiro);
            float valorCurva = curva.Evaluate(t);
            float rotacionActual = rotacionInicial + rotacionFinal * valorCurva;
            transform.eulerAngles = new Vector3(
                transform.eulerAngles.x,
                rotacionActual,
                transform.eulerAngles.z
            );
            int sectorActual = ObtenerSector(rotacionActual);
            if (sectorActual >= 0)
                textoResultado.text = sectores[sectorActual].nombre;
            yield return null;
        }
        int sectorFinal = ObtenerSector(transform.eulerAngles.y);
        if (sectorFinal >= 0)
            AplicarResultado(sectorFinal);
        estaGirando = false;
    }
    private int ObtenerSector(float rotacion)
    {
        float angulo = (rotacion + 180f) % 360f;
        if (angulo < 0) angulo += 360f;
        for (int i = 0; i < sectores.Length; i++)
        {
            float ini = sectores[i].anguloInicio;
            float fin = sectores[i].anguloFin;
            if (ini > fin)
            {
                if (angulo >= ini || angulo < fin)
                    return i;
            }
            else
            {
                if (angulo >= ini && angulo < fin)
                    return i;
            }
        }
        return -1;
    }
    private void AplicarResultado(int indice)
    {
        string mensaje = "";
        bool esVictoria = false;
        switch (indice)
        {
            case 0: monedaManager.MultiplicarMonedas(3f); mensaje = "JACKPOT! x3"; esVictoria = true; break;
            case 1: monedaManager.ModificarMonedas(10); mensaje = "+10 monedas"; esVictoria = true; break;
            case 2: monedaManager.ModificarMonedas(-10); mensaje = "-10 monedas"; break;
            case 3: monedaManager.ModificarMonedas(25); mensaje = "+25 monedas"; esVictoria = true; break;
            case 4: monedaManager.ModificarMonedas(-25); mensaje = "-25 monedas"; break;
            case 5: monedaManager.MultiplicarMonedas(2f); mensaje = "x2 monedas!"; esVictoria = true; break;
            case 6: monedaManager.MultiplicarMonedas(0.5f); mensaje = "÷2 monedas"; break;
            case 7: monedaManager.SetMonedas(0); mensaje = "PERDISTE"; break;
        }
        textoResultado.text = mensaje;
        Vector3 posicion = transform.position + Vector3.up * 1f;
        Quaternion rotacion = Quaternion.identity;
        if (esVictoria && victoriaPS != null)
            Instantiate(victoriaPS, posicion, rotacion);
        else if (!esVictoria && derrotaPS != null)
        {
            Instantiate(derrotaPS, posicion, rotacion);
            if (vignette != null)
                StartCoroutine(EfectoVignette());
        }
    }
    private IEnumerator EfectoVignette()
    {
        float intensidadOriginal = 0.3f;
        float intensidadDerrota = 0.8f;
        float duracion = 0.5f;

        // Subir intensidad
        float t = 0f;
        while (t < duracion)
        {
            t += Time.deltaTime;
            vignette.intensity.value = Mathf.Lerp(intensidadOriginal, intensidadDerrota, t / duracion);
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        // Volver a la normalidad
        t = 0f;
        while (t < duracion)
        {
            t += Time.deltaTime;
            vignette.intensity.value = Mathf.Lerp(intensidadDerrota, intensidadOriginal, t / duracion);
            yield return null;
        }

        vignette.intensity.value = intensidadOriginal;
    }
    public void Salir()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}