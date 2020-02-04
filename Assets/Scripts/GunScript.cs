using cakeslice;
using UnityEngine;

public class GunScript : MonoBehaviour
{
	public bool canShoot;               //If the gun can or cannot fire bullets.
	public bool hasMag;                 //If the gun has a reloadable Magazine.
	public bool atOnce;                 //If the gun reloads all shells at once (eg. Grenade Launcher).
	public GameObject Mag;              //The Magazine that the gun is using (includes ammo type in mag).
	public GameObject oldMag;           //used to swap out old mags for new mags.
	public GameObject Barrel;           //Where the bullet is fired from.
	public GameObject MagSlot;          //The slot in which the magazine goes.
	public GameObject Chamber;          //Where the empty shell is ejected from.
	public GameObject FiredRounds;      //Empty GameObject to organise fired bullets.
	public GameObject EmptyShells;      //Empty GameObject to organise spent shells.
	public GameObject EmptyMags;        //Empty GameObject to organise spent shells.
	public GameObject AudioSources;
	public GameObject gunshotSound;
	public GameObject emptyClick;
	public float bulletForce;           //Force at which the bullet is fired with.
	public float recoil;                //Recoil torque applied to gun.
	public float RPS;                   //Rounds per second of gun.
	public float ejectForce;            //Force of spent shells being ejected.

	GameObject handUsing;
	float lastShotSec;                  //Time since last shot fired from gun.
	float lastShotTime;                 //The time of the last shot fired from the gun.
	int gunAmmoCount;                   //The maximum ammo count of the mag/gun.

	bool grabbed = false;               //Used to store if the gun has been grabbed or not.


	//Set any default variables
	private void Start() {
		lastShotSec = RPS;
		lastShotTime = Time.time;
		gunAmmoCount = Mag.GetComponent<Magazine>().AmmoCount;
	}

	//Check if the player is equipping, shooting, or reloading
	private void Update() {
		Equipping();

		lastShotSec = Time.time - lastShotTime;
		if (canShoot == true && grabbed == true && Input.GetMouseButton(0)) {
			if (lastShotSec >= 1 / RPS) {
				Shooting();
			}
		}
		if (Input.GetKeyDown(KeyCode.R) && grabbed == true) {
			reloading();
		}
	}

	//Check if the player can equip, if they are equipping, or are unequipping
	void Equipping() {
		var handNear = false;
		oldMag.GetComponent<Outline>().eraseRenderer = true;
		gameObject.GetComponent<Outline>().eraseRenderer = true;

		if (Input.GetKeyDown(KeyCode.E) && grabbed == true) {
			handUsing.GetComponent<PolygonCollider2D>().enabled = true; // This is maing throwing guns kinda wierrddd...
		}

		if (Physics2D.OverlapBoxAll(transform.position, new Vector2(2, 1), transform.rotation.z).Length > 1) {
			foreach (Collider2D colliders in Physics2D.OverlapBoxAll(transform.position - transform.right * 0.3f, new Vector2(2.5f, 1f), transform.eulerAngles.z)) {
				if (colliders.transform.name == "Hand") {
					handNear = true;

					if (Input.GetKeyDown(KeyCode.E) && grabbed == false && colliders.GetComponent<Hand>().IsWielding == false) {
						grabbed = true;
						handUsing = colliders.gameObject;
						colliders.GetComponent<PolygonCollider2D>().enabled = false;
						colliders.GetComponent<Hand>().IsWielding = true;
						CreateJoint(colliders);
					}
					else if (Input.GetKeyDown(KeyCode.E) && grabbed == true) {
						grabbed = false;
						colliders.GetComponent<Hand>().IsWielding = false;
						Destroy(gameObject.GetComponent<HingeJoint2D>());
					}
				}
			}
		}

		//Highlights the objects near the hand
		if (handNear == true && grabbed == false) {
			oldMag.GetComponent<Outline>().eraseRenderer = false;
			gameObject.GetComponent<Outline>().eraseRenderer = false;
		}
	}

	//Handles ammo count, shooting the bullets, playing sounds, ejecting shells
	void Shooting() {
		if (oldMag.GetComponent<Magazine>().AmmoCount > 0) {
			oldMag.GetComponent<Magazine>().AmmoCount -= 1;

			GameObject sound = Instantiate(gunshotSound, Barrel.transform.position, Barrel.transform.rotation);
			sound.transform.parent = AudioSources.transform;

			GameObject round = Instantiate(Mag.GetComponent<Magazine>().AmmoType.GetComponent<Ammunition>().Round, Barrel.transform.position, Barrel.transform.rotation);
			round.transform.parent = FiredRounds.transform;
			round.GetComponent<Rigidbody2D>().AddForce(-Barrel.transform.right * bulletForce + Barrel.transform.up * Random.Range(-0.2f, 0.2f));

			if (atOnce == false) {
				GameObject shell = Instantiate(Mag.GetComponent<Magazine>().AmmoType.GetComponent<Ammunition>().Shell, Chamber.transform.position, Chamber.transform.rotation);
				shell.transform.parent = EmptyShells.transform;
				shell.GetComponent<Rigidbody2D>().AddForce(Chamber.transform.up * ejectForce + Chamber.transform.right * Random.Range(3, 5f));
			}

			GetComponent<Rigidbody2D>().AddTorque(-recoil);

			lastShotTime = Time.time;
		}
		else {
			GameObject emptySound = Instantiate(emptyClick, Chamber.transform.position, Chamber.transform.rotation);

			lastShotTime = Time.time + 0.5f;
		}
	}

	//Handles reloading, ejecting old ones, resetting gun ammo etc
	void reloading() {
		if (hasMag == true) {
			try {
				Destroy(oldMag.GetComponent<HingeJoint2D>());
				//Collision with Hand
				oldMag.layer = 8;
				oldMag.transform.parent = EmptyMags.transform;
				oldMag.transform.position += -transform.up * 0.25f;
				oldMag.GetComponent<Rigidbody2D>().AddForce(-transform.up * 20);
			}
			catch {
				Debug.Log("No oldMag, reloading anyway");
			}

			GameObject newMag = Instantiate(Mag, MagSlot.transform.position, transform.rotation);
			newMag.transform.parent = MagSlot.transform;

			//No collision with Hand
			oldMag.layer = 15;

			newMag.GetComponent<HingeJoint2D>().connectedBody = GetComponent<Rigidbody2D>();
			newMag.GetComponent<HingeJoint2D>().connectedAnchor = new Vector2(-0.75f, 0);
			JointAngleLimits2D limits = new JointAngleLimits2D {
				max = Mathf.Round(newMag.GetComponent<HingeJoint2D>().jointAngle / 360) * 360,
				min = Mathf.Round(newMag.GetComponent<HingeJoint2D>().jointAngle / 360) * 360
			};
			newMag.gameObject.GetComponent<HingeJoint2D>().limits = limits;

			oldMag = newMag;
		}
		else {
			for (int i = gunAmmoCount - Mag.GetComponent<Magazine>().AmmoCount; i > 0; i--) {
				GameObject shell = Instantiate(Mag.GetComponent<Magazine>().AmmoType.GetComponent<Ammunition>().Shell, Chamber.transform.position, Chamber.transform.rotation);
				shell.transform.parent = EmptyShells.transform;
				shell.GetComponent<Rigidbody2D>().AddForce(Chamber.transform.right * Random.Range(-0.1f, 0.1f));
			}
			for (int i = Mag.GetComponent<Magazine>().AmmoCount; i > 0; i--) {
				GameObject shell = Instantiate(Mag.GetComponent<Magazine>().AmmoType, Chamber.transform.position, Chamber.transform.rotation);
				shell.transform.parent = EmptyShells.transform;
				//shell.GetComponent<Rigidbody2D>().AddForce(Chamber.transform.right * Random.Range(-0.1f, 0.1f));
			}

			Mag.GetComponent<Magazine>().AmmoCount = gunAmmoCount;
		}
	}

	//Creates the joint between the weapon and the hand
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
}
