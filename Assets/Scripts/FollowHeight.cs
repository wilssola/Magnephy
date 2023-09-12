using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowHeight : MonoBehaviour {

    public GameObject Target;

    public float Height = 2.5f;
	
	// Update is called once per frame.
	void Update () {

        transform.position = new Vector3(transform.position.x, Target.transform.position.y + Height, transform.position.z);

	}
}
