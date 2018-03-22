using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllerUI : MonoBehaviour {

	public Text speed;
	// Use this for initialization
	void Start () {
		speed.text = CarControlCS.getSpeedPlayer ().ToString() + " MPH";
	}
	
	// Update is called once per frame
	void Update () {
		speed.text = CarControlCS.getSpeedPlayer ().ToString() + " MPH";
	}
}
