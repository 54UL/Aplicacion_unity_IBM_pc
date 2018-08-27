﻿using System;
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

    internal class SocketInfo
    {
        public SocketInfo(string infoText, Sprite referenceimg)
        {

            inftxt = infoText;
            btnimage= referenceimg;
         }

        public String inftxt;
        public Sprite btnimage;

    }

    public GameObject[] PuertosDelPC;
    private PlaceCablesManager cablesManager;
    public Transform CameraPos;
    public Transform NextCameraPos;
    public float TransitionTime = 3.4f;
    bool ready;
   
    [Header("UI CONTROLS")]
    public Button ResetandStartButton;
    public Transform resetbuttonplaceholde;
    public GameObject panel1, panel2;
    public GameObject Canvas3Dinfo;
    public Text ErrorMessage; 
    public Toggle ShowInformation;
    public Text InfoText;
    //variables privadas
    private float Elapsed=0;
	[SerializeField]
	private GameObject m_buttonModel;
	private Dictionary<ConnectorType, ConnectorsPlacedCounter> m_socketsLeft;
    //dictionary for the instructions
    private Dictionary<ConnectorType, SocketInfo> instrucciones;

    [Header("preview images")]
    public Sprite powersupplyimg;
    public Sprite USBimg;
    public Sprite PS2img;
    public Sprite RJ45img;
    public Sprite VGAimg;

    //metodos de unity
    private void Start()
    {
        ready = false;
        cablesManager = Camera.main.GetComponent<PlaceCablesManager>();
        ResetSimulation();
  
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
            Elapsed +=  Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator ShowErrorMessage()
    {
        ErrorMessage.color = Color.red;
        ErrorMessage.text = "¡Ahi no va ese cable!";
        ErrorMessage.gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        ErrorMessage.gameObject.SetActive(false);
        cablesManager.WrongSlot = false;
    }

    /// <summary>
	/// Initialize the simulation
	/// </summary>
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

        instrucciones = new Dictionary<ConnectorType, SocketInfo> {
            { ConnectorType.None, new SocketInfo( "agarra algun componente",null) },
            { ConnectorType.PowerSupply,new SocketInfo("conecta P1-T1-C1 Y P1-T1-C2 (power supply)",powersupplyimg) },
            { ConnectorType.RJ45,new SocketInfo("conecta P1-T1-E1 Y P1-T1-E2   (RJ45) ",RJ45img) },
            { ConnectorType.KeyboardPS2,new SocketInfo("conecta P1-T1-PS2     (Teclado)",PS2img) },
            { ConnectorType.USB,new SocketInfo("conecta P1-T1- DEL U1 AL U4 (USB)",USBimg) },
            { ConnectorType.VGA,new SocketInfo("conecta P1-T1-V (VGA)",VGAimg) }
            };


        ready = true;
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
        cablesManager.PiceCount = 0;
        ErrorMessage.gameObject.SetActive(false);
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
            button.GetComponentInChildren<Image>().sprite = instrucciones[connector].btnimage;

			ConnectorsPlacedCounter connectorCounter;
			if(m_socketsLeft.TryGetValue(connector, out connectorCounter))
			{
				connectorCounter.button = button;
			}

			button.GetComponent<Button>().onClick.AddListener(() =>
			{
				cablesManager.currentSelectedCable = (ConnectorType)connectorType;
                //change the text when something is selected
                InfoText.text = instrucciones[cablesManager.currentSelectedCable].inftxt;
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

    public void toggleInfo()
    {
        panel1.SetActive(!panel1.activeSelf);
        panel2.SetActive(!panel2.activeSelf);
        Canvas3Dinfo.SetActive(!Canvas3Dinfo.activeSelf);
    }

    public void Update()
    {
        if (cablesManager.PiceCount >= 10)
        {
            ErrorMessage.color = Color.green;
            ErrorMessage.text = "¡Has completado las conexiones!";
            ErrorMessage.gameObject.SetActive(true);
           
        }
           
        if(cablesManager.WrongSlot)
               StartCoroutine(ShowErrorMessage());

    }
}
