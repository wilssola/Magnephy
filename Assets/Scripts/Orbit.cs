using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{

	public Transform OrbitTarget;
	public Vector3 OrbitObject;
	public float OrbitSpeed;

	void Start ()
	{
		
	}
	
	void Update ()
	{

		transform.RotateAround (OrbitTarget.position, OrbitObject, OrbitSpeed * Time.deltaTime);
	
	}
}
