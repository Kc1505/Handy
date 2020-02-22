using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	[Header("DAMAGE:")]
	public float hitDamage;
	public float ExplosionDamage;
	[Header("EXPLOSION:")]
	public float explosionForce;
	public float explosionRadius;
	public float explodeVelocity;
	public GameObject explosionSound;
	public GameObject Explosion;

	float startTime;
	bool fade = false;

	private void Start() {
		startTime = Time.time;
	}

	void FixedUpdate()
    {
        if (Time.time - startTime >= 30) {
			Destroy(gameObject);
		}
    }

	private void Update() {
		Vector2 v = GetComponent<Rigidbody2D>().velocity;
		float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
		
		if (GetComponent<Rigidbody2D>().velocity.magnitude >=5) {
			transform.rotation = Quaternion.AngleAxis(angle - 180, Vector3.forward);
		}

		if (fade == true && GetComponent<Renderer>().material.color.a >0) {
			GetComponent<Renderer>().material.color -= new Color(0,0,0,0.001f);
		}
		else if (GetComponent<Renderer>().material.color.a <= 0) {
			Destroy(gameObject);
		}
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		if (explosionForce > 0 && collision.relativeVelocity.magnitude > explodeVelocity) {
			Instantiate(explosionSound, transform.position, transform.rotation);
			Destroy(Instantiate(Explosion, transform.position, transform.rotation), Explosion.GetComponent<ParticleSystem>().main.duration);
			Collider2D[] hits = Physics2D.OverlapCircleAll(gameObject.GetComponent<Transform>().position, explosionRadius);
			int i = 0;
			while (i < hits.Length) {
				AddExplosionForce(hits[i].GetComponent<Rigidbody2D>(), explosionForce, transform.position, explosionRadius);
				hits[i].GetComponent<Rigidbody2D>();
				i++;
			}

			Destroy(gameObject);
		}
		else {
			fade = true;
		}
	}

	public static void AddExplosionForce(Rigidbody2D body, float expForce, Vector3 expPosition, float expRadius) {
		var dir = (body.transform.position - expPosition);
		float calc = 1 - (dir.magnitude / expRadius);
		if (calc <= 0) {
			calc = 0;
		}

		body.AddForce(dir.normalized * expForce * calc);
	}
}
