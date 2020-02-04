using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shell : MonoBehaviour
{
	public GameObject hittingGround;

	bool fade = false;

    void Update()
    {
		if (fade == true && GetComponent<Renderer>().material.color.a > 0) {
			GetComponent<Renderer>().material.color -= new Color(0, 0, 0, 0.001f);
		}
		else if (GetComponent<Renderer>().material.color.a <= 0) {
			Destroy(gameObject);
		}
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		fade = true;

		if(GetComponent<Rigidbody2D>().velocity.magnitude > 1) {
			Instantiate(hittingGround, transform);
		}
	}
}
