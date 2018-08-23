using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceCablesManager : MonoBehaviour
{
   

    //publicas
 
   public TiposDeConector currentSelectedCable;



    private void Start()
    {
        
    }

    private void Update()
    {

        Ray rashito = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit golpexd;
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (Physics.Raycast(rashito, out golpexd))
            {

                Debug.Log(golpexd.transform.name);
                PuertoEntrada componenteexterno = golpexd.transform.GetComponent<PuertoEntrada>();

                if (componenteexterno != null)
                    if (componenteexterno.conectorEntrada == currentSelectedCable)
                        componenteexterno.Place();
            }

        }
    }



}
