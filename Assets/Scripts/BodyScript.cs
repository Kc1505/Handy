using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyScript : MonoBehaviour
{
	public GameObject jetpack;
	public GameObject sparks;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		UpRight();



		if (Input.GetKey(KeyCode.W)) {
			GetComponent<Rigidbody2D>().AddForce(transform.up * 110);
			Destroy(Instantiate(jetpack, transform.position - transform.up * 0.5f, Quaternion.Euler(Quaternion.Euler(transform.rotation.eulerAngles).z + 90, -90, -90),transform), Random.Range(0.5f,2f));
		}
		if (Mathf.Abs(GetComponent<Rigidbody2D>().velocity.x) < 10) {
			if (Input.GetKey(KeyCode.A)) {
				GetComponent<Rigidbody2D>().AddForce(-transform.right * 100);
			}
			if (Input.GetKey(KeyCode.D)) {
				GetComponent<Rigidbody2D>().AddForce(transform.right * 100);
			}
		}
	}

	void UpRight() {

		GetComponent<Rigidbody2D>().angularVelocity *= 0.9f;

		float preForce = Mathf.Abs(UnityEditor.TransformUtils.GetInspectorRotation(transform).z);

		if (UnityEditor.TransformUtils.GetInspectorRotation(transform).z < 0) {
			GetComponent<Rigidbody2D>().AddTorque(preForce);
		}
		else if (UnityEditor.TransformUtils.GetInspectorRotation(transform).z > 0) {
			GetComponent<Rigidbody2D>().AddTorque(-preForce);
		}
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		Destroy(Instantiate(sparks, collision.contacts[0].point, Quaternion.Euler(0, 90, 0)), 1.5f);
	}
}
