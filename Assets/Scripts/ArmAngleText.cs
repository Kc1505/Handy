using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArmAngleText : MonoBehaviour {

	private Text Angletext;
	private List<Transform> Arms;

	private void Start() {
		Arms = new List<Transform>();

		Angletext = GameObject.Find("Arm Angles").GetComponent<Text>();

		foreach (Transform bodyPart in GameObject.Find("Man").transform) {
			if (bodyPart.name.Contains("Arm")) {
				Arms.Add(bodyPart);
			}
		}
	}

	void Update() {
		UpdateText();
	}

	void UpdateText() {
		Angletext.text = "";

		foreach (Transform arm in Arms) {
			Angletext.text += arm.name + " HINGE: " + arm.GetComponent<HingeJoint2D>().jointAngle + "\n";


			Angletext.text += "\n\n";
		}
	}
}
