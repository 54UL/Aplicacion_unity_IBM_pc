using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles all the cables that need to be connected
/// </summary>
public class PlaceCablesManager : MonoBehaviour
{
   public ConnectorType currentSelectedCable;

	/// <summary>
	/// Logic that needs to be updated every frame
	/// </summary>
    private void Update()
    {
		if (!Input.GetKeyDown(KeyCode.Mouse0))
			return;

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hittedObject;

		if (!Physics.Raycast(ray, out hittedObject))
			return;

		Port portHitted = hittedObject.transform.GetComponent<Port>();

		if (portHitted != null && portHitted.inputConnectorType == currentSelectedCable)
				portHitted.Place();
	}
}
