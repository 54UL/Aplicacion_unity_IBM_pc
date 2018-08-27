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
   [Header("animaciones")]
    public float TransitionSpeed=0.6f;
    public Vector3 AnimationRelativeStart = new Vector3(5, 0, 0);

   
    private  Vector3 ModelStartPos;
    private Vector3 FromPosition;
    /// <summary>
    ///  starts an transition from the relative start pos from the original object's position
    /// </summary>
  

    IEnumerator StartTransition()
    {
        float Elapsed = 0.0f;

        while (Elapsed <1)
        {
            //positional things
            model.transform.localPosition = Vector3.Lerp(FromPosition, ModelStartPos, Elapsed);
            Elapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    private void Start()
    {
        FromPosition = model.transform.localPosition + AnimationRelativeStart;
        m_manager = FindObjectOfType<GameManager>();
        ModelStartPos = model.transform.localPosition;
        model.SetActive(false);
        isConnected = false;
    }

    /// <summary>
    ///  set the propierty to show the ports and starts his transition
    /// </summary>

    public void Place()
    {
		if (isConnected)
			return;
        //activamos la animacion
        model.SetActive(true);
        StartCoroutine(StartTransition());
        isConnected = true;
		m_manager.ClickedPort(inputConnectorType);
	}

    /// <summary>
    ///  put off the cable, this helps to reset the simulation
    /// </summary>
 
    public void DetachCable()
    {
		if (!isConnected)
			return;
		model.SetActive(false);
        isConnected = false;
    }
}
