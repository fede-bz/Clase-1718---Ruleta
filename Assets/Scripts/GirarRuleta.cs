using UnityEngine;
using TMPro;

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

    [Header("Sectores")]
    public Sector[] sectores;

    [Header("Animacion")]
    public AnimationCurve[] curvas;
    public float duracionGiro = 5f;
    public float vueltasMinimas = 3f;
    public float vueltasExtra = 3f;

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

        switch (indice)
        {
            case 0: monedaManager.MultiplicarMonedas(3f); mensaje = "JACKPOT! x3"; break; // Verde
            case 1: monedaManager.ModificarMonedas(10); mensaje = "+10 monedas"; break; // Rojo1
            case 2: monedaManager.ModificarMonedas(-10); mensaje = "-10 monedas"; break; // Negro1
            case 3: monedaManager.ModificarMonedas(25); mensaje = "+25 monedas"; break; // Rojo2
            case 4: monedaManager.ModificarMonedas(-25); mensaje = "-25 monedas"; break; // Negro2
            case 5: monedaManager.MultiplicarMonedas(2f); mensaje = "x2 monedas!"; break; // Rojo3
            case 6: monedaManager.MultiplicarMonedas(0.5f); mensaje = "÷2 monedas"; break; // Negro3
            case 7: monedaManager.SetMonedas(0); mensaje = "PERDISTE"; break; // Amarillo
        }

        textoResultado.text = mensaje;
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