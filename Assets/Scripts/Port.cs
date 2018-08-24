using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PC connector types
/// </summary>
[System.Serializable]
public enum ConnectorType
{
	None,
	PowerSupply,
	RJ45,
	VGA,
	USB,
	KeyboardPS2
};

/// <summary>
/// Represents a port
/// </summary>
public class Port : MonoBehaviour
{
	public ConnectorType inputConnectorType;
	public GameObject model;
	public bool isConnected;
	GameManager m_manager;

	private void Start()
	{
		m_manager = FindObjectOfType<GameManager>();
		model.SetActive(false);
		isConnected = false;
	}

	/// <summary>
	/// Connect the cable to a slot
	/// </summary>
	/// <returns>true if the cable was succesfuly connected, false if it was already connected</returns>
	public bool Place()
	{
		if (isConnected)
			return false;
		model.SetActive(true);
		isConnected = true;
		m_manager.ClickedPort(inputConnectorType);
		return true;
	}

	/// <summary>
	/// Disconnect the cable
	/// </summary>
	public void DetachCable()
	{
		if (!isConnected)
			return;
		model.SetActive(false);
		isConnected = false;
	}
}
