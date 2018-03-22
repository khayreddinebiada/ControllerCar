using UnityEngine;
using System.Collections;
using UnityEngine.UI;



public enum DriveType
{
	RWD,
	FWD,
	AWD
};
[System.Serializable]
public class WC
{
	public WheelCollider wheelFL;
	public WheelCollider wheelFR;
	public WheelCollider wheelRL;
	public WheelCollider wheelRR;
}

[System.Serializable]
public class WT
{
	public Transform wheelFL;
	public Transform wheelFR;
	public Transform wheelRL;
	public Transform wheelRR;
}

[RequireComponent(typeof(AudioSource))]//needed audiosource
[RequireComponent(typeof(Rigidbody))]//needed Rigid body
public class CarControlCS : MonoBehaviour {

	public WC wheels;
	public WT tires;
	public WheelCollider[] extraWheels;
	public Transform[] extraWheelObjects;

	public DriveType DriveTrain = DriveType.RWD;
	public Vector3 centerOfGravity;


	public float maxTorque = 1000f;
	public float maxReverseSpeed = 50f;
	public float handBrakeTorque = 500f;
	public float maxSteer = 25f;
	public float maxSpeed = 150f;//how fast the vehicle can go

	public bool isPlayer = false;
	public GameObject UIMobile;
	public bool mobileInput = true;
	public float[] GearRatio;

	private int throttleInput;//read only
	private int steerInput;//read only
	private bool reversing;//read only
	private int gear;//current gear
	Vector3 localCurrentSpeed;


	private static float speedPlayer = 0.0f;
	private float currentSpeed = 0.0f;//read only

	// Use this for initialization
	void Start () {
		GUIButtonControl ();

		GetComponent<Rigidbody>().centerOfMass = centerOfGravity;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		currentSpeed = GetComponent<Rigidbody>().velocity.magnitude * 2.0f;

		if (GetComponent<Rigidbody>().centerOfMass != centerOfGravity)
		GetComponent<Rigidbody>().centerOfMass = centerOfGravity;
		
		AllignWheels ();

		DriveMobile ();
		Drive ();
		EngineAudio ();

		if (isPlayer) speedPlayer = GetComponent<Rigidbody>().velocity.magnitude * 2.23693629f;//convert currentspeed into MPH

		localCurrentSpeed = transform.InverseTransformDirection (GetComponent<Rigidbody> ().velocity);

	}

	void AllignWheels()
	{
		//allign the wheel objs to their colliders

			Quaternion quat;
			Vector3 pos;
			wheels.wheelFL.GetWorldPose(out pos,out quat);
			tires.wheelFL.position = pos;
			tires.wheelFL.rotation = quat;

		wheels.wheelFR.GetWorldPose(out pos,out quat);
		tires.wheelFR.position = pos;
		tires.wheelFR.rotation = quat;

		wheels.wheelRL.GetWorldPose(out pos,out quat);
		tires.wheelRL.position = pos;
		tires.wheelRL.rotation = quat;

		wheels.wheelRR.GetWorldPose(out pos,out quat);
		tires.wheelRR.position = pos;
		tires.wheelRR.rotation = quat;

		for (int i = 0; i < extraWheels.Length; i++)
		{

			for (int k = 0; k < extraWheelObjects.Length; k++) {
			
				Quaternion quater;
				Vector3 vec3;

				extraWheels [i].GetWorldPose (out vec3, out quater);
				extraWheelObjects [k].position = vec3;
				extraWheelObjects [k].rotation = quater;
			
			}

		}
	}
		
	public static int getSpeedPlayer (){
		return (int) speedPlayer;
	}

	void GUIButtonControl()
	{
		//simple function that disables/enables GUI buttons when we need and dont need them.
		if (mobileInput)
		{
			UIMobile.SetActive (true);
		}
		else{
			UIMobile.SetActive (false);
		}
	}

	void DriveMobile()
	{
		if (!mobileInput) 
			return;
		
		//dont call this function if the mobileiput box is not checked in the editor
		float gasMultiplier = 0f;

		if (!reversing) {
			
			if (currentSpeed < maxSpeed)
				gasMultiplier = 1f;
			else
				gasMultiplier = 0f;

		} else {
			
			if (currentSpeed < maxReverseSpeed)
				gasMultiplier = 1f;
			else
				gasMultiplier = 0f;
			
		}

		if (isPlayer) {
			throttleInput = ControllerPointerUI.throttleInput;
			steerInput = ControllerPointerUI.steerInput;
		}

		wheels.wheelFL.steerAngle = maxSteer * steerInput;
		wheels.wheelFR.steerAngle = maxSteer * steerInput;


		if (DriveTrain == DriveType.RWD)
		{
			wheels.wheelRL.motorTorque = maxTorque * throttleInput * gasMultiplier;
			wheels.wheelRR.motorTorque = maxTorque * throttleInput * gasMultiplier;

			if (localCurrentSpeed.z < -0.1f && wheels.wheelRL.rpm < 10) {//in local space, if the car is travelling in the direction of the -z axis, (or in reverse), reversing will be true
				reversing = true;
			} else {
				reversing = false;
			}
		}
		if (DriveTrain == DriveType.FWD)
		{
			wheels.wheelFL.motorTorque = maxTorque * throttleInput * gasMultiplier;
			wheels.wheelFR.motorTorque = maxTorque * throttleInput * gasMultiplier;

			if (localCurrentSpeed.z < -0.1f && wheels.wheelFL.rpm < 10) {//in local space, if the car is travelling in the direction of the -z axis, (or in reverse), reversing will be true
				reversing = true;
			} else {
				reversing = false;
			}
		}
		if (DriveTrain == DriveType.AWD)
		{
			wheels.wheelFL.motorTorque = maxTorque * throttleInput * gasMultiplier;
			wheels.wheelFR.motorTorque = maxTorque * throttleInput * gasMultiplier;
			wheels.wheelRL.motorTorque = maxTorque * throttleInput * gasMultiplier;
			wheels.wheelRR.motorTorque = maxTorque * throttleInput * gasMultiplier;

			if (localCurrentSpeed.z < -0.1f && wheels.wheelRL.rpm < 10) {
				reversing = true;
			} else {
				reversing = false;
			}
		}
	}

	void Drive()
	{
		if (mobileInput)
			return;
		
		//dont call this function if mobile input is checked in the editor
		float gasMultiplier = 0f;

		if (!reversing) {
			if (currentSpeed < maxSpeed)
				gasMultiplier = 1f;
			else
				gasMultiplier = 0f;

		} else {
			if (currentSpeed < maxReverseSpeed)
				gasMultiplier = 1f;
			else
				gasMultiplier = 0f;
		}

		if (DriveTrain == DriveType.RWD)
		{
			if (isPlayer) {
				wheels.wheelRR.motorTorque = maxTorque * Input.GetAxis ("Vertical") * gasMultiplier;
				wheels.wheelRL.motorTorque = maxTorque * Input.GetAxis ("Vertical") * gasMultiplier;
			}
			if (localCurrentSpeed.z < -0.1f && wheels.wheelRL.rpm < 10) {
				
				reversing = true;
			} else {
				reversing = false;
			}
		}
		if (DriveTrain == DriveType.FWD)
		{
			if (isPlayer) {
				wheels.wheelFL.motorTorque = maxTorque * Input.GetAxis ("Vertical") * gasMultiplier;
				wheels.wheelFR.motorTorque = maxTorque * Input.GetAxis ("Vertical") * gasMultiplier;
			}
			if (localCurrentSpeed.z < -0.1f && wheels.wheelFL.rpm < 10) {
				
				reversing = true;
			} else {
				reversing = false;
			}
		}
		if (DriveTrain == DriveType.AWD)
		{

			if (isPlayer) {
				wheels.wheelFL.motorTorque = maxTorque * Input.GetAxis ("Vertical") * gasMultiplier;
				wheels.wheelFR.motorTorque = maxTorque * Input.GetAxis ("Vertical") * gasMultiplier;
				wheels.wheelRL.motorTorque = maxTorque * Input.GetAxis ("Vertical") * gasMultiplier;
				wheels.wheelRR.motorTorque = maxTorque * Input.GetAxis ("Vertical") * gasMultiplier;
			}
			if (localCurrentSpeed.z < -0.1f && wheels.wheelRL.rpm < 10) {
				
				reversing = true;
			} else {
				reversing = false;
			}
		}


		if (isPlayer) {
			wheels.wheelFL.steerAngle = maxSteer * Input.GetAxis ("Horizontal");
			wheels.wheelFR.steerAngle = maxSteer * Input.GetAxis ("Horizontal");
		}

		if (Input.GetButton("Jump"))
		{
			wheels.wheelFL.brakeTorque = handBrakeTorque;
			wheels.wheelFR.brakeTorque = handBrakeTorque;
			wheels.wheelRL.brakeTorque = handBrakeTorque;
			wheels.wheelRR.brakeTorque = handBrakeTorque;
		}
		else
		{
			wheels.wheelFL.brakeTorque = 0f;
			wheels.wheelFR.brakeTorque = 0f;
			wheels.wheelRL.brakeTorque = 0f;
			wheels.wheelRR.brakeTorque = 0f;
		}
	}

	void EngineAudio()
	{
		
		for (int i = 0; i < GearRatio.Length; i++) {
			if (GearRatio [i] > currentSpeed) {
				
				break;
			}

			float minGearValue = 0f;
			float maxGearValue = 0f;
			if (i == 0) {
				minGearValue = 0f;
			} else {
				minGearValue = GearRatio [i];
			}
			maxGearValue = GearRatio [i+1];
		
			float pitch = ((currentSpeed - minGearValue) / (maxGearValue - minGearValue)+0.3f * (gear+1));
			GetComponent<AudioSource> ().pitch = pitch;
		
			gear = i;
		}
	}
}
