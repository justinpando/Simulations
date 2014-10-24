using UnityEngine;
using System.Collections;

public class BillBoardBehavior : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//transform.LookAt(Camera.main.transform);
		this.transform.forward =
			-(Camera.main.transform.position - transform.position).normalized;
	}
}
