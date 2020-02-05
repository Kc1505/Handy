using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
	public GameObject Body;
	public GameObject Arm1;
	public GameObject Arm2;
	public GameObject Equipped;
	public GameObject Jetpack;

	private void OnDrawGizmos() {
		UnityEditor.Handles.color = Color.yellow - new Color(0, 0, 0, 0.5f);
		UnityEditor.Handles.DrawWireDisc(Camera.main.ScreenToWorldPoint(Input.mousePosition) - Vector3.back*10, Vector3.forward, 0.2f);
	}

	bool slow = false;
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.LeftAlt)) {
			if (slow == false) {
				slow = true;
				Time.timeScale = 0.5f;
			}
			else {
				slow = false;
				Time.timeScale = 1f;
			}
		}

		UpRight();



		if (Input.GetKey(KeyCode.W)) {
			Body.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 110);
			Destroy(Instantiate(Jetpack, Body.transform.position, Quaternion.Euler(Body.transform.rotation.z + 90, 90, 0)),2.5f);
		}
		if(Mathf.Abs(Body.GetComponent<Rigidbody2D>().velocity.x) < 10) {
			if (Input.GetKey(KeyCode.A)) {
				Body.GetComponent<Rigidbody2D>().AddForce(-Vector2.right * 100);
			}
			if (Input.GetKey(KeyCode.D)) {
				Body.GetComponent<Rigidbody2D>().AddForce(Vector2.right * 100);
			}
		}
	}

	void UpRight() {

		Body.GetComponent<Rigidbody2D>().angularVelocity *= 0.9f;

		float preForce = Mathf.Abs(UnityEditor.TransformUtils.GetInspectorRotation(Body.transform).z);

		if (UnityEditor.TransformUtils.GetInspectorRotation(Body.transform).z < 0) {
			Body.GetComponent<Rigidbody2D>().AddTorque(preForce);
		}
		else if (UnityEditor.TransformUtils.GetInspectorRotation(Body.transform).z > 0) {
			Body.GetComponent<Rigidbody2D>().AddTorque(-preForce);
		}

		
	}

	public void SuperMode() {
		Time.timeScale = 0.5f;

		Arm1.GetComponent<ArmMove>().Force = Arm1.GetComponent<ArmMove>().DefaultForce * 2;
		Arm2.GetComponent<ArmMove>().Force = Arm2.GetComponent<ArmMove>().DefaultForce * 2;
	}

	public void StopSuper() {
		Time.timeScale = 1f;

		Arm1.GetComponent<ArmMove>().Force = Arm1.GetComponent<ArmMove>().DefaultForce;
		Arm2.GetComponent<ArmMove>().Force = Arm2.GetComponent<ArmMove>().DefaultForce;
	}
}
