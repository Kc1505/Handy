﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{

	public bool IsGrabbing;
	public bool IsWielding;

    void FixedUpdate()
    {
		if (IsWielding) {
			Destroy(GetComponent<WheelJoint2D>());
		}

		if (!IsWielding) {
			Grabbing();
		}
	}

	void Grabbing() {
		if (Physics2D.OverlapCircleAll(transform.position, 0.2f).Length > 1 && Input.GetKey("space") && IsGrabbing == false) {
			IsGrabbing = true;
			foreach (Collider2D collider in Physics2D.OverlapCircleAll(transform.position, 0.18f)) {
				if (collider.gameObject.transform.parent != transform.parent) {
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
		}
		else if (!Input.GetKey("space")) {
			Destroy(GetComponent<WheelJoint2D>());
			IsGrabbing = false;
		}
	}
}