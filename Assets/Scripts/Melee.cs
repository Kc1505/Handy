﻿using cakeslice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : MonoBehaviour
{
	public GameObject sparks;
	public GameObject Stab;

	GameObject hand;

	bool grabbed = false;               //Used to store if the Sword has been grabbed or not.
	bool reachedMax;

	float jointTime;

	void Update()
    {
		//if(gameObject.GetComponent<SliderJoint2D>())
		//Debug.Log(gameObject.GetComponent<SliderJoint2D>().jointTranslation);

		Equipping();

		if (grabbed == true && Input.GetMouseButtonDown(0)) {
			hand.transform.parent.GetComponent<Main>().SuperMode();
		}

		if (grabbed == true && Input.GetMouseButtonUp(0)) {
			hand.transform.parent.GetComponent<Main>().StopSuper();
		}

		if (GetComponent<SliderJoint2D>()) {
			SlideOut();
		}
	}

	private void FixedUpdate() {
		
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
		if (!GetComponent<SliderJoint2D>() && collision.gameObject.name != "Hand") {
			Vector2 v = collision.relativeVelocity;
			float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;

			Instantiate(sparks, collision.contacts[0].point, Quaternion.Euler(0,90,angle));

			Debug.Log("Hit");

			//Make all of this code usable for different sized weapons etc (can it stab? where does the blade start? how long is the blade?)

			//Debug.Log(transform.InverseTransformDirection(collision.relativeVelocity).x);

			if (transform.InverseTransformDirection(collision.relativeVelocity).x > 0 && transform.InverseTransformPoint(collision.contacts[0].point).x < -4) {
				Debug.Log("MadeJoint");

				reachedMax = false;
				jointTime = Time.time;

				GameObject stabSound = Instantiate(Stab, collision.contacts[0].point, transform.rotation);
				stabSound.GetComponent<AudioSource>().pitch *= Mathf.Clamp(20/collision.relativeVelocity.magnitude,0.75f,5);
				
				gameObject.AddComponent<SliderJoint2D>();
				gameObject.GetComponent<SliderJoint2D>().connectedBody = collision.gameObject.GetComponent<Rigidbody2D>();
				gameObject.GetComponent<SliderJoint2D>().anchor = transform.InverseTransformPoint(collision.contacts[0].point);
				gameObject.GetComponent<SliderJoint2D>().connectedAnchor = collision.transform.InverseTransformPoint(collision.contacts[0].point);
				gameObject.GetComponent<SliderJoint2D>().autoConfigureAngle = false;
				gameObject.GetComponent<SliderJoint2D>().angle = 0;

				gameObject.GetComponent<SliderJoint2D>().breakForce = Mathf.Infinity;
				gameObject.GetComponent<SliderJoint2D>().breakTorque = Mathf.Infinity;

				//Maybe make the motor stop working once it gets as deep as it can, to simulate momentum?

				gameObject.GetComponent<SliderJoint2D>().useMotor = true;
				gameObject.GetComponent<SliderJoint2D>().motor = new JointMotor2D {motorSpeed = 1000, maxMotorTorque = 1000 };

				//Debug.Log(collision.otherRigidbody.velocity.x);

				gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2 {x = collision.otherRigidbody.velocity.x };

				//make it so the faster it goes the deeper the stab is (Watch out for weird collisions I guess)

				gameObject.GetComponent<SliderJoint2D>().limits = new JointTranslationLimits2D { min = 0.1f, max = Mathf.Clamp(transform.InverseTransformDirection(collision.relativeVelocity).x / 50, 0.2f, 1.5f) };
			}
		}
	}

	void SlideOut() {

		if (reachedMax) {
			gameObject.GetComponent<SliderJoint2D>().breakForce = Mathf.Clamp(1 / (1 - Mathf.Clamp(gameObject.GetComponent<SliderJoint2D>().jointTranslation, 0f, gameObject.GetComponent<SliderJoint2D>().limits.max) / gameObject.GetComponent<SliderJoint2D>().limits.max), 500, Mathf.Infinity);
			gameObject.GetComponent<SliderJoint2D>().breakTorque = Mathf.Clamp(1 / (1 - Mathf.Clamp(gameObject.GetComponent<SliderJoint2D>().jointTranslation, 0f, gameObject.GetComponent<SliderJoint2D>().limits.max) / gameObject.GetComponent<SliderJoint2D>().limits.max), 500, Mathf.Infinity);
		}
		
		if (reachedMax == false && (gameObject.GetComponent<SliderJoint2D>().jointTranslation >= gameObject.GetComponent<SliderJoint2D>().limits.max || (Time.time - jointTime) > 3 )) {
			reachedMax = true;
			gameObject.GetComponent<SliderJoint2D>().motor = new JointMotor2D { motorSpeed = 1000, maxMotorTorque = 20 };
		}
	}
}