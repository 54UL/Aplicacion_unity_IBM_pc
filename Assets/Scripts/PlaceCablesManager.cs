using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles all the cables that need to be connected
/// </summary>
public class PlaceCablesManager : MonoBehaviour
{
   public ConnectorType currentSelectedCable;
   public int pieceCount = 0;
   public bool WrongSlot;

	/// <summary>
	/// Logic that needs to be updated every frame
	/// </summary>
	private void Update()
	{
		// we do not need to process anything if the user doesn't click the mouse
		if (!Input.GetKeyDown(KeyCode.Mouse0))
			return;

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hittedObject;

		// does the user clicked a port?
		if (!Physics.Raycast(ray, out hittedObject))
			return;

		Port portHitted = hittedObject.transform.GetComponent<Port>();
        if (portHitted != null && portHitted.inputConnectorType == currentSelectedCable)
        {
			if (!portHitted.Place())
				return;
            pieceCount++;
            WrongSlot = false;
        }
        else if(portHitted != null && portHitted.inputConnectorType != currentSelectedCable && currentSelectedCable!=ConnectorType.None)
            WrongSlot = true;	
	}
}
