using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;



public class GameManager : MonoBehaviour
{

	internal class ConnectorsPlacedCounter
	{
		public ConnectorsPlacedCounter(int count)
		{
			this.count = count;
		}

		public int count;
		public GameObject button;
	}

	public GameObject[] PuertosDelPC;
    public GameObject[] PlacedItems;

	public Transform CameraPos;
    public Transform NextCameraPos;
    public float TransitionTime = 3.4f;
    public Button ResetandStartButton;
    public Transform resetbuttonplaceholder;
    bool ready;

    //variables privadas
    private float Elapsed=0;
	[SerializeField]
	private GameObject m_buttonModel;

	private Dictionary<ConnectorType, ConnectorsPlacedCounter> m_socketsLeft;

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
		if(m_socketsLeft != null)
		{
			foreach (var item in m_socketsLeft)
			{
				if(item.Value.button != null)
					Destroy(item.Value.button);
			}
		}
		m_socketsLeft = new Dictionary<ConnectorType, ConnectorsPlacedCounter>
		{
			{ ConnectorType.KeyboardPS2, new ConnectorsPlacedCounter(1) },
			{ ConnectorType.PowerSupply, new ConnectorsPlacedCounter(2) },
			{ ConnectorType.USB, new ConnectorsPlacedCounter(4) },
			{ ConnectorType.VGA, new ConnectorsPlacedCounter(1) },
			{ ConnectorType.RJ45, new ConnectorsPlacedCounter(2) }
		};
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
		foreach (Transform child in layout.GetComponent<Transform>())
		{

		}

		var cablesManager = Camera.main.GetComponent<PlaceCablesManager>();
		foreach (var connectorType in Enum.GetValues(typeof(ConnectorType)))
		{
			var connector = (ConnectorType)connectorType;
			if (connector == ConnectorType.None)
			{
				continue;
			}
			var button = GameObject.Instantiate(m_buttonModel);
			button.GetComponent<Transform>().SetParent(layout.GetComponent<Transform>());
			button.GetComponentInChildren<Text>().text = connectorType.ToString();

			ConnectorsPlacedCounter connectorCounter;
			if(m_socketsLeft.TryGetValue(connector, out connectorCounter))
			{
				connectorCounter.button = button;
			}

			button.GetComponent<Button>().onClick.AddListener(() =>
			{
				cablesManager.currentSelectedCable = (ConnectorType)connectorType;
			});
		}
	}

	public void ClickedPort(ConnectorType type)
	{
		ConnectorsPlacedCounter connectorCounter;
		if (!m_socketsLeft.TryGetValue(type, out connectorCounter))
			return;
		connectorCounter.count--;
		if(connectorCounter.count <= 0)
		{
			Destroy(connectorCounter.button);
		}
	}
}
