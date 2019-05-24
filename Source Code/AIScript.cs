using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIScript : MonoBehaviour
{
	public GameObject curTarget;
	public float searchFrequency, lastSearchTime, targetDistanceThreshold, startDelay;
	public bool debugLogging, targetReached, targetDead;

	private Transform targetTransform;
	private Vector3 targetPos;
	private CircleScript myCircScript;

    // Start is called before the first frame update
    void Start()
    {
		myCircScript = GetComponent<CircleScript>();
		GetTarget();
    }

	// Get the closest circle that is able to be consumed by this one.
	public void GetTarget()
	{
		GameObject[] circles = GameObject.FindGameObjectsWithTag("Circle");
		float closestValidDistance = Mathf.Infinity;
		bool foundATarget = false;
		lastSearchTime = Time.time;
		foreach (GameObject circ in circles)
		{
			if ((circ.GetComponent<Transform>().position - transform.position).magnitude <= closestValidDistance &&
				transform.localScale.magnitude - circ.GetComponent<Transform>().localScale.magnitude >= myCircScript.absorptionThreshold)
			{
				closestValidDistance = (circ.GetComponent<Transform>().position - transform.position).magnitude;
				curTarget = circ;
				targetTransform = circ.transform;
				targetPos = targetTransform.position;
				foundATarget = true;
			}
		}
		if (!foundATarget)
		{
			// Find a spawner and stay near it, preferring closer ones.
			// TODO
		}
		if (debugLogging && foundATarget)
		{
			Debug.Log(name + " is now targeting " + curTarget.name);
		}
		targetDead = false;
	}

	// Get a unit vector in the direction of the current target
	public Vector2 GetDirection()
	{
		return (targetPos - transform.position).normalized;
	}

    void FixedUpdate()
    {
		if (Time.time < startDelay) // Give it time to actually find a target.
		{
			return;
		}

        if (Time.time - lastSearchTime > searchFrequency || targetDead)
		{
			GetTarget();
		}
		if ((targetPos - transform.position).magnitude < targetDistanceThreshold)
		{
			targetReached = true;
		}
		else if (targetReached)
		{
			targetReached = false;
		}
    }
}
