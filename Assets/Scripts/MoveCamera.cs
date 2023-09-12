using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public bool Shaking;

    public GameObject Target;

    public float Height = 2.5f;
    public float SmoothSpeed = 0.125f;

    public float MaxShake = 0.5f;
    public float MinShake = -0.5f;
	
	// Update is called once per frame.
	private void Update () {

        if (!Shaking)
        {
            Vector3 DesiredPosition = new Vector3(transform.position.x, Target.transform.position.y + Height, transform.position.z);
            Vector3 SmoothedPosition = Vector3.Lerp(transform.position, DesiredPosition, SmoothSpeed);

            transform.position = SmoothedPosition;
        }

	}

    public IEnumerator Shake (float Duration, float Magnitude)
    {
        Vector3 BackupPosition = transform.localPosition;

        float Elapsed = 0f;

        while (Elapsed < Duration)
        {
            float x = Random.Range(MinShake, MaxShake) * Magnitude;
            float y = Random.Range(MinShake, MaxShake) * Magnitude;

            Vector3 ShakePosition = new Vector3(x, y, BackupPosition.z);

            transform.localPosition = Vector3.Lerp(transform.localPosition, ShakePosition, SmoothSpeed / 500);

            Elapsed += Time.deltaTime;

            Shaking = true;

            yield return null;
        }

        transform.localPosition = BackupPosition;

        Shaking = false;
    }
}
