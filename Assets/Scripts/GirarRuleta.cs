using UnityEngine;
using TMPro;

public class GirarRuleta : MonoBehaviour
{
    [Header("Referencias")]
    public MonedaManager monedaManager;
    public TextMeshProUGUI textoResultado;

    [Header("Animacion")]
    public AnimationCurve[] curvas;
    public float duracionGiro = 3f;
    public float vueltasMinimas = 3f;
    public float vueltasExtra = 5f;

    private bool estaGirando = false;

    public void Girar()
    {
        if (estaGirando) return;
        StartCoroutine(CorrutinaGiro());
    }

    private System.Collections.IEnumerator CorrutinaGiro()
    {
        estaGirando = true;
        textoResultado.text = "";

        AnimationCurve curva = curvas[Random.Range(0, curvas.Length)];
        int resultado = Random.Range(0, 8);

        float gradosPorSector = 360f / 8f;
        float rotacionExtra = vueltasMinimas * 360f + vueltasExtra * Random.value * 360f;
        float rotacionFinal = rotacionExtra + (resultado * gradosPorSector);

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

            float angulo = rotacionActual % 360f;
            if (angulo < 0) angulo += 360f;
            int sectorActual = Mathf.FloorToInt(angulo / gradosPorSector) % 8;
            textoResultado.text = NombreSector(sectorActual);

            yield return null;
        }

        AplicarResultado(resultado);
        estaGirando = false;
    }

    private void AplicarResultado(int resultado)
    {
        string mensaje = "";
        switch (resultado)
        {
            case 0:
                monedaManager.MultiplicarMonedas(3f);
                mensaje = "JACKPOT! x3";
                break;
            case 1:
                monedaManager.ModificarMonedas(10);
                mensaje = "+10 monedas";
                break;
            case 2:
                monedaManager.ModificarMonedas(-10);
                mensaje = "-10 monedas";
                break;
            case 3:
                monedaManager.ModificarMonedas(25);
                mensaje = "+25 monedas";
                break;
            case 4:
                monedaManager.ModificarMonedas(-25);
                mensaje = "-25 monedas";
                break;
            case 5:
                monedaManager.MultiplicarMonedas(2f);
                mensaje = "x2 monedas!";
                break;
            case 6:
                monedaManager.MultiplicarMonedas(0.5f);
                mensaje = "÷2 monedas";
                break;
            case 7:
                monedaManager.SetMonedas(0);
                mensaje = "PERDISTE";
                break;
        }
        textoResultado.text = mensaje;
    }

    private string NombreSector(int sector)
    {
        switch (sector)
        {
            case 0: return "JACKPOT x3";
            case 1: return "+10";
            case 2: return "-10";
            case 3: return "+25";
            case 4: return "-25";
            case 5: return "x2";
            case 6: return "/2";
            case 7: return "PERDISTE";
            default: return "";
        }
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