using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ControllerPointerUI : MonoBehaviour ,IPointerDownHandler, IPointerUpHandler {

	public byte IndexButton = 0;
	public static int throttleInput = 0;
	public static bool Retry = false;
	public static int steerInput = 0;
	public static bool shooting = false;

	public void OnPointerDown (PointerEventData data)
	{

		switch (IndexButton)
		{
		case 1:
			throttleInput = 1;
			break;
		case 2:
			throttleInput = -1;
			break;
		case 3:
			steerInput = 1;
			break;
		case 4:
			steerInput = -1;
			break;
		case 5:
			shooting = true;
			break;
		}
	}


	public void OnPointerUp (PointerEventData data)
	{
		switch (IndexButton)
		{
		case 1:
			throttleInput = 0;
			break;
		case 2:
			throttleInput = 0;
			break;
		case 3:
			steerInput = 0;
			break;
		case 4:
			steerInput = 0;
			break;
		case 5:
			shooting = false;
			break;
		}
	}


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	void FixedUpdate()
	{
		
	}
}
