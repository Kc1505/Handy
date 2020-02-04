using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
	public float variance;
    // Start is called before the first frame update
    void Start()
    {
		GetComponent<AudioSource>().pitch += Random.Range(-variance, variance);
		GetComponent<AudioSource>().pitch *= Time.timeScale;
		Destroy(gameObject, GetComponent<AudioSource>().clip.length);
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
