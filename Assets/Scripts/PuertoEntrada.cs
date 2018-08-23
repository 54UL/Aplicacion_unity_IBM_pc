using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum TiposDeConector {NONE,PowerSupply, RJ45, VGA, USB, TECLADO, MOUSE };

public class PuertoEntrada :MonoBehaviour  {

    public TiposDeConector conectorEntrada;
    public  GameObject Modelo;
    public bool ActualmenteColocado;



    private void Start()
    {
        Modelo.SetActive(false);
        ActualmenteColocado = false;
    }

    public void Place()
    {
        Modelo.SetActive(true);
        ActualmenteColocado = true;
    }

    public void unPlace()
    {
        Modelo.SetActive(false);
        ActualmenteColocado = false;
    }
}
