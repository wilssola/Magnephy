using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlatform : MonoBehaviour
{

    public GameObject[] Targets;
    public int StartTarget = 0;
    public float Speed = 2.5f;
    public bool StartInvert;
    public bool RestartOnEnd;
    public bool Move = true;

    private int ActualTarget = 0;

    void Start()
    {
        if (transform.tag == "Black")
        {
            foreach (GameObject Go in Targets)
            {
                Go.transform.localPosition = new Vector3(Go.transform.localPosition.x, -0.25f, Go.transform.localPosition.z);
            }
        }

        if (StartTarget < Targets.Length)
        {
            ActualTarget = StartTarget;
        }
        else
        {
            ActualTarget = 0;
        }
    }

    void Update()
    {
        if (Move && !Player.GameOver)
        {
            if (StartInvert == false)
            {
                if (Vector3.Distance(transform.position, Targets[ActualTarget].transform.position) < 0.1f)
                {
                    if (ActualTarget < Targets.Length - 1)
                    {
                        ActualTarget++;
                    }
                    else
                    {
                        if (RestartOnEnd == true)
                        {
                            ActualTarget = 0;
                        }
                        else
                        {
                            StartInvert = true;
                        }
                    }
                }
                transform.position = Vector3.MoveTowards(transform.position, Targets[ActualTarget].transform.position, Speed * Time.deltaTime);
            }
            else
            {
                if (Vector3.Distance(transform.position, Targets[ActualTarget].transform.position) < 0.1f)
                {
                    if (ActualTarget > 0)
                    {
                        ActualTarget--;
                    }
                    else
                    {
                        if (RestartOnEnd == true)
                        {
                            ActualTarget = Targets.Length - 1;
                        }
                        else
                        {
                            StartInvert = false;
                        }
                    }
                }
                transform.position = Vector3.MoveTowards(transform.position, Targets[ActualTarget].transform.position, Speed * Time.deltaTime);
            }
        }
    }
}