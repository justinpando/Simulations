using UnityEngine;
using System.Collections;

public class KeepAwayBehavior : MonoBehaviour {
	public GameObject objectToAvoid = null;
	public float triggerDistance = 5.0f;
	public float avoidanceStrength = 1.0f;
	private Vector3 originalPos = Vector3.zero;

	// Use this for initialization
	void Start () {
		originalPos = transform.position;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(objectToAvoid) {
			KeepAway();
		} else {
			transform.position = originalPos;
		}	
	}
	
	void KeepAway() {
		if(Vector3.Distance(originalPos, objectToAvoid.transform.position) < triggerDistance) {
			transform.position = originalPos + (originalPos - objectToAvoid.transform.position) * avoidanceStrength;
		}
	}
}
