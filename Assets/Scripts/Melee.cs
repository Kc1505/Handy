﻿using cakeslice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : MonoBehaviour
{
	GameObject hand;

	bool grabbed = false;               //Used to store if the Sword has been grabbed or not.

	void Update()
    {
		//Debug.Log(GetComponent<Rigidbody2D>());

		Equipping();

		if (grabbed == true && Input.GetMouseButtonDown(0)) {
			hand.transform.parent.GetComponent<Main>().SuperMode();
		}

		if (grabbed == true && Input.GetMouseButtonUp(0)) {
			hand.transform.parent.GetComponent<Main>().StopSuper();
		}

		Hitting();
	}

	void Equipping() {
		var handNear = false;
		gameObject.GetComponent<Outline>().eraseRenderer = true;

		if (Physics2D.OverlapBoxAll(transform.position, new Vector2(2, 1), transform.rotation.z).Length > 1) {
			foreach (Collider2D colliders in Physics2D.OverlapBoxAll(transform.position - transform.right * 0.3f, new Vector2(2.5f, 1f), transform.eulerAngles.z)) {
				if (colliders.transform.name == "Hand") {
					handNear = true;

					if (Input.GetKeyDown(KeyCode.E) && grabbed == false && colliders.GetComponent<Hand>().IsWielding == false) {
						hand = colliders.gameObject;
						grabbed = true;
						colliders.GetComponent<Hand>().IsWielding = true;
						CreateJoint(colliders);
					}
					else if (Input.GetKeyDown(KeyCode.E) && grabbed == true) {
						grabbed = false;
						hand.transform.parent.GetComponent<Main>().StopSuper();
						colliders.GetComponent<Hand>().IsWielding = false;
						Destroy(gameObject.GetComponent<HingeJoint2D>());
					}
				}
			}
		}

		if (handNear == true && grabbed == false) {
			GetComponentInChildren<Outline>().eraseRenderer = false;
			gameObject.GetComponent<Outline>().eraseRenderer = false;
			handNear = false;
		}
	}

	void CreateJoint(Collider2D colliders) {
		gameObject.AddComponent<HingeJoint2D>();
		gameObject.GetComponent<HingeJoint2D>().connectedBody = colliders.GetComponent<Rigidbody2D>();
		gameObject.GetComponent<HingeJoint2D>().autoConfigureConnectedAnchor = false;
		gameObject.GetComponent<HingeJoint2D>().connectedAnchor = new Vector2(0, 0);
		gameObject.GetComponent<HingeJoint2D>().useLimits = true;
		JointAngleLimits2D limits = new JointAngleLimits2D {
			max = Mathf.Round(gameObject.GetComponent<HingeJoint2D>().jointAngle / 360) * 360,
			min = Mathf.Round(gameObject.GetComponent<HingeJoint2D>().jointAngle / 360) * 360
		};
		gameObject.GetComponent<HingeJoint2D>().limits = limits;
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		if (collision.relativeVelocity.magnitude >= 10 /*&& GetComponent<Rigidbody2D>().attachedColliderCount < 1*/) {
			Vector2 v = GetComponent<Rigidbody2D>().velocity;
			float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
			

			gameObject.AddComponent<SliderJoint2D>();
			gameObject.GetComponent<SliderJoint2D>().connectedBody = collision.gameObject.GetComponent<Rigidbody2D>();
			gameObject.GetComponent<SliderJoint2D>().connectedAnchor = transform.InverseTransformPoint(collision.contacts[0].point);
			gameObject.GetComponent<SliderJoint2D>().autoConfigureAngle = false;
			gameObject.GetComponent<SliderJoint2D>().angle = angle;

			JointTranslationLimits2D limits = new JointTranslationLimits2D { min = -2, max = 2 };

			gameObject.GetComponent<SliderJoint2D>().limits = limits;
		}
	}

	void Hitting() {
		
	}
}