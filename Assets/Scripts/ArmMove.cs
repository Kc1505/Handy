using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmMove : MonoBehaviour
{
	public float Force;
	public float DefaultForce;
	private Vector3 ArmJoint;

	private void OnDrawGizmos() {
		Vector3 ArmPivot = transform.TransformPoint(GetComponent<HingeJoint2D>().anchor);
		ArmJoint = transform.TransformPoint(GetComponent<HingeJoint2D>().anchor);

		UnityEditor.Handles.color = Color.yellow - new Color(0, 0, 0, 0.5f);
		UnityEditor.Handles.DrawWireDisc(ArmPivot, Vector3.forward, transform.localScale.x);

		UnityEditor.Handles.DrawLine(ArmJoint, transform.right * 10 + ArmJoint);
	}

	private Vector3 mouse_pos;
	public Transform target;
	private Vector3 object_pos;
	private float angle;

	private void Start() {
		DefaultForce = Force;
	}

	void FixedUpdate() {

		mouse_pos = Input.mousePosition;
		mouse_pos.z = 10; //The distance between the camera and object
		object_pos = Camera.main.WorldToScreenPoint(target.position);
		mouse_pos.x = mouse_pos.x - object_pos.x;
		mouse_pos.y = mouse_pos.y - object_pos.y;
		angle = 180 - Mathf.Atan2(mouse_pos.y, mouse_pos.x) * Mathf.Rad2Deg;
		Vector3 dir = Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.right;

		var inspectorRot = UnityEditor.TransformUtils.GetInspectorRotation(gameObject.transform).z;

		var newAngle = 360 - angle;

		if (newAngle >= 180) {
			newAngle = -angle;
		}

		if (newAngle >= 180) {
			newAngle = newAngle - 180 - newAngle;
		}

		newAngle = newAngle - inspectorRot;

		GetComponent<Rigidbody2D>().angularVelocity = 0;

		RealDifference(newAngle);

		ArmLocking();

		if (GetComponent<HingeJoint2D>().useMotor == false) {
			ArmToMouse(RealDifference(newAngle));
		}
	}

	void ArmLocking() {
		GetComponent<HingeJoint2D>().useMotor = false;

		if (Input.GetKey(KeyCode.LeftShift) && gameObject.name == "Arm1") {
			GetComponent<HingeJoint2D>().useMotor = true;
		}
		if (Input.GetKey(KeyCode.LeftControl) && gameObject.name == "Arm2") {
			GetComponent<HingeJoint2D>().useMotor = true;
		}
	}

	void ArmToMouse(float angle) {
		GetComponent<Rigidbody2D>().angularVelocity = 0;
		float ratio = 1;

		if (angle <= 20 && angle >= -20) {
			ratio = Mathf.Abs(angle)/20;
		}

		if (transform.parent.GetComponentInChildren<Hand>().IsGrabbing) {
			//ratio *= 0.5f;
		}

		if (angle < 0) {
			Clockwise(ratio);
		}
		else if (angle > 0) {
			AntiClockwise(ratio);
		}
	}

	void Clockwise(float ratio) {
		GetComponent<Rigidbody2D>().AddTorque(-Force * ratio);
		//GetComponent<Rigidbody2D>().angularVelocity += -Force * ratio;
	}

	void AntiClockwise(float ratio) {
		GetComponent<Rigidbody2D>().AddTorque(Force * ratio);
		//GetComponent<Rigidbody2D>().angularVelocity += Force * ratio;
	}

	float RealDifference(float newAngle) {
		float diff = newAngle;
		if (newAngle > 180) {
			diff = -360 + newAngle;
		}
		else if (newAngle < -180) {
			diff = 360 + newAngle;
		}

		return diff;
	}
}
