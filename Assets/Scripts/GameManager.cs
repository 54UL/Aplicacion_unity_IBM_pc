using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {



    public GameObject[] PuertosDelPC;
    public GameObject[] PlacedItems; //perdoname alan por usar arrays, list es el mejor pero no se ven en el inspector =( xd


    public Transform CameraPos;
    public Transform NextCameraPos;
    public float TransitionTime = 3.4f;
    public Button ResetandStartButton;
    public Transform resetbuttonplaceholder;
    bool ready;

    //variables privadas
    private float Elapsed=0;



    //metodos de unity
    private void Start()
    {

        ready = false;
        resetSimulation();
    }


    //corutina del transform
    IEnumerator ChangeCamerapos()
    {
        while (Elapsed < TransitionTime)
        {
            //positional things
            CameraPos.position = Vector3.Lerp(CameraPos.position, NextCameraPos.position, Elapsed);
            CameraPos.rotation = Quaternion.Slerp(CameraPos.rotation, NextCameraPos.rotation, Elapsed);
            Elapsed +=  Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
       
    
    }


    //funcion de inicio de la simualcion
    public void startSimulation()
    {
        ready = true;
        StartCoroutine(this.ChangeCamerapos());
        //ResetandStartButton.transform.position = resetbuttonplaceholder.position; PARA CAMBIAR LA POSICION DEL BOTON
        ResetandStartButton.GetComponentInChildren<Text>().text = "restaurar";
    }

    public void resetSimulation()
    {
        //Reset things
       
        foreach (GameObject Iterador in PuertosDelPC)
        {
            Iterador.GetComponent<PuertoEntrada>().ActualmenteColocado = false;
            //aqui tambien van los objetos ya colocados
        }
       
    }


	
}
