using UnityEngine;
using TMPro;

public class MonedaManager : MonoBehaviour
{
    public int monedas = 100;
    public TextMeshProUGUI textoMonedas;

    void Start()
    {
        ActualizarUI();
    }

    public void ModificarMonedas(int cantidad)
    {
        monedas += cantidad;
        monedas = Mathf.Clamp(monedas, 0, 9999);
        ActualizarUI();
    }

    public void MultiplicarMonedas(float factor)
    {
        monedas = Mathf.RoundToInt(monedas * factor);
        monedas = Mathf.Clamp(monedas, 0, 9999);
        ActualizarUI();
    }

    public void SetMonedas(int valor)
    {
        monedas = valor;
        monedas = Mathf.Clamp(monedas, 0, 9999);
        ActualizarUI();
    }

    void ActualizarUI()
    {
        if (textoMonedas != null)
            textoMonedas.text = "Monedas: " + monedas;
    }
}