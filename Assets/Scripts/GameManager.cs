using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


/// <summary>
/// Manages the game state such as port status
/// </summary>
public class GameManager : MonoBehaviour
{
	/// <summary>
	/// Represents a heap-stored object that handles how many ports of a specified type are connected
	/// </summary>
	internal class ConnectorsPlacedCounter
	{
		public ConnectorsPlacedCounter(int count)
		{
			this.count = count;
		}

		public int count;
		public GameObject button;
	}

	/// <summary>
	/// Represents the PC ports
	/// </summary>
	public GameObject[] PuertosDelPC;
	private PlaceCablesManager cablesManager;
	public Transform CameraPos;
	public Transform NextCameraPos;
	public float TransitionTime = 3.4f;

	[Header("UI CONTROLS")]
	public Button ResetandStartButton;
	public Transform resetbuttonplaceholder;

	public GameObject panel1, panel2;
	public GameObject Canvas3Dinfo;
	public Text ErrorMessage;
	public Toggle ShowInformation;
	public Text InfoText;


	private float Elapsed = 0;
	[SerializeField]
	private GameObject m_buttonModel;

	private Dictionary<ConnectorType, ConnectorsPlacedCounter> m_socketsLeft;

	//dictionary for the instructions
	private Dictionary<ConnectorType, string> instrucciones;



	/// <summary>
	/// Called when the scene starts
	/// </summary>
	private void Start()
	{
		cablesManager = Camera.main.GetComponent<PlaceCablesManager>();
		ResetSimulation();
		instrucciones = new Dictionary<ConnectorType, string> {
			{ ConnectorType.None,"agarra algun componente" },
			{ ConnectorType.PowerSupply,"conecta P1-T1-C1 Y P1-T1-C2 (power supply)" },
			{ ConnectorType.RJ45,"conecta P1-T1-E1 Y P1-T1-E2   (RJ45) " },
			{ ConnectorType.KeyboardPS2,"conecta P1-T1-PS2     (Teclado)" },
			{ ConnectorType.USB,"conecta P1-T1- DEL U1 AL U4 (USB)" },
			{ ConnectorType.VGA,"conecta P1-T1-V (VGA)" }
			};
	}

	/// <summary>
	///Corrutines
	/// </summary>
	IEnumerator ChangeCamerapos()
	{
		while (Elapsed < TransitionTime)
		{
			//positional things
			CameraPos.position = Vector3.Lerp(CameraPos.position, NextCameraPos.position, Elapsed);
			CameraPos.rotation = Quaternion.Slerp(CameraPos.rotation, NextCameraPos.rotation, Elapsed);
			Elapsed += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
	}

	/// <summary>
	/// Shows an error message when user try to connect a cable in a wrong socket and disappear the message 2 seconds later 
	/// </summary>
	/// <returns></returns>
	IEnumerator ShowErrorMessage()
	{
		ErrorMessage.color = Color.red;
		ErrorMessage.text = "¡Ahi no va ese cable!";
		ErrorMessage.gameObject.SetActive(true);
		yield return new WaitForSeconds(2);
		ErrorMessage.gameObject.SetActive(false);
		cablesManager.wrongSlot = false;
	}

	/// <summary>
	/// Initialize the simulation
	/// </summary>
	public void StartSimulation()
	{
		if (m_socketsLeft != null)
		{
			foreach (var item in m_socketsLeft)
			{
				if (item.Value.button != null)
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
		StartCoroutine(ChangeCamerapos());
		ShowInformation.gameObject.SetActive(true);
		//ResetandStartButton.transform.position = resetbuttonplaceholder.position; PARA CAMBIAR LA POSICION DEL BOTON

		ResetandStartButton.GetComponentInChildren<Text>().text = "restaurar";
		ResetSimulation();
	}

	/// <summary>
	/// Performs a detach-all cables
	/// </summary>
	public void ResetSimulation()
	{
		cablesManager.pieceCount = 0;
		ErrorMessage.gameObject.SetActive(false);
		foreach (GameObject Iterador in PuertosDelPC)
		{
			var port = Iterador.GetComponent<Port>();
			port.DetachCable();
		}
	}

	/// <summary>
	/// Fillst the canvas with all type of connector types (connectors are getted from an enum)
	/// </summary>
	/// <param name="layout">Layout that stores the buttons</param>
	public void FillCanvas(GameObject layout)
	{
		foreach (var connectorType in Enum.GetValues(typeof(ConnectorType)))
		{
			var connector = (ConnectorType)connectorType;

			// first pass, we do not need "None"
			if (connector == ConnectorType.None)
			{
				continue;
			}

			// create the gameobject and set his parent and his text
			var button = GameObject.Instantiate(m_buttonModel);
			button.GetComponent<Transform>().SetParent(layout.GetComponent<Transform>());
			button.GetComponentInChildren<Text>().text = connectorType.ToString();

			// Create heap managed counter
			ConnectorsPlacedCounter connectorCounter;
			if (m_socketsLeft.TryGetValue(connector, out connectorCounter))
			{
				connectorCounter.button = button;
			}

			button.GetComponent<Button>().onClick.AddListener(() =>
			{
				cablesManager.currentSelectedCable = (ConnectorType)connectorType;
				// change the text when something is selected
				InfoText.text = instrucciones[cablesManager.currentSelectedCable];
			});
		}
	}

	/// <summary>
	/// Called when a port is connected
	/// </summary>
	/// <param name="type">type of the port that was previously connected</param>
	public void ClickedPort(ConnectorType type)
	{
		ConnectorsPlacedCounter connectorCounter = m_socketsLeft[type];
		connectorCounter.count--;
		if (connectorCounter.count <= 0)
		{
			Destroy(connectorCounter.button);
		}
	}

	/// <summary>
	/// Show or hides the app how-to use information
	/// </summary>
	public void ToggleInfo()
	{
		panel1.SetActive(!panel1.activeSelf);
		panel2.SetActive(!panel2.activeSelf);
		Canvas3Dinfo.SetActive(!Canvas3Dinfo.activeSelf);
	}

	/// <summary>
	/// Called every frame
	/// </summary>
	public void Update()
	{
		if (cablesManager.pieceCount >= 10)
		{
			ErrorMessage.color = Color.green;
			ErrorMessage.text = "¡Has completado las conexiones!";
			ErrorMessage.gameObject.SetActive(true);
		}

		if (cablesManager.wrongSlot)
			StartCoroutine(ShowErrorMessage());
	}
}
