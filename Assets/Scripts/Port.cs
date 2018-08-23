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

    private void Start()
    {
        model.SetActive(false);
        isConnected = false;
    }

    public void Place()
    {
        model.SetActive(true);
        isConnected = true;
    }

    public void DetachCable()
    {
		model.SetActive(false);
        isConnected = false;
    }
}
