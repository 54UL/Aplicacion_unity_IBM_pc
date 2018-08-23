using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject[] PuertosDelPC;
    public GameObject[] PlacedItems;

	

	public Transform CameraPos;
    public Transform NextCameraPos;
    public float TransitionTime = 3.4f;
    public Button ResetandStartButton;
    public Transform resetbuttonplaceholder;
    bool ready;
	bool isFirstPlay = true;

    //variables privadas
    private float Elapsed=0;
	[SerializeField]
	private GameObject m_buttonModel;

	//metodos de unity
	private void Start()
    {
        ready = false;
        ResetSimulation();
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
    public void StartSimulation()
    {
		ready = true;
        StartCoroutine(ChangeCamerapos());
        //ResetandStartButton.transform.position = resetbuttonplaceholder.position; PARA CAMBIAR LA POSICION DEL BOTON
        ResetandStartButton.GetComponentInChildren<Text>().text = "restaurar";
		ResetSimulation();
    }

	/// <summary>
	/// Performs a detach-all cables
	/// </summary>
    public void ResetSimulation()
    {
        foreach (GameObject Iterador in PuertosDelPC)
        {
			var port = Iterador.GetComponent<Port>();
			port.DetachCable();
		}
    }

	public void FillCanvas(GameObject layout)
	{
		if (!isFirstPlay)
			return;
		var cablesManager = Camera.main.GetComponent<PlaceCablesManager>();
		foreach (var connectorType in Enum.GetValues(typeof(ConnectorType)))
		{
			if((ConnectorType)connectorType == ConnectorType.None)
			{
				continue;
			}
			var button = GameObject.Instantiate(m_buttonModel);
			button.GetComponent<Transform>().SetParent(layout.GetComponent<Transform>());
			button.GetComponentInChildren<Text>().text = connectorType.ToString();

			button.GetComponent<Button>().onClick.AddListener(() =>
			{
				cablesManager.currentSelectedCable = (ConnectorType)connectorType;
			});
		}
		isFirstPlay = false;
	}
}
