using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{

	public bool IsGrabbing;
	public bool IsWielding;

	public Sprite HandOpen;
	public Sprite HandClosed;

    void Update()
    {
		if (IsWielding) {
			Destroy(GetComponent<WheelJoint2D>());
		}

		if (!IsWielding) {
			Grabbing();
		}

		if (IsGrabbing || IsWielding) {
			GetComponent<SpriteRenderer>().sprite = HandClosed;
		}
		else {
			GetComponent<SpriteRenderer>().sprite = HandOpen;
		}
	}

	void Grabbing() {
		foreach (Collider2D collider in Physics2D.OverlapCircleAll(transform.position, 0.2f)) {
			if (collider.gameObject.transform.parent != transform.parent && IsGrabbing == false && Input.GetKey("space")) {
				IsGrabbing = true;
				gameObject.AddComponent<WheelJoint2D>();
				gameObject.GetComponent<WheelJoint2D>().connectedBody = collider.GetComponent<Rigidbody2D>();
				gameObject.GetComponent<WheelJoint2D>().connectedAnchor = collider.transform.InverseTransformPoint(transform.position);
				JointSuspension2D newWheel = gameObject.GetComponent<WheelJoint2D>().suspension;
				newWheel.dampingRatio = 1;
				newWheel.frequency = 10000;
				gameObject.GetComponent<WheelJoint2D>().suspension = newWheel;
				break;
			}
		}
		if (!Input.GetKey("space")) {
			Destroy(GetComponent<WheelJoint2D>());
			IsGrabbing = false;
		}
	}
}
