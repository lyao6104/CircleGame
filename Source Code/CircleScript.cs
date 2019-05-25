/* Name: Larry Y.
 * Date: April 30, 2019
 * Desc: The primary script for circles. */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleScript : MonoBehaviour
{
	public bool isPlayer, debugLogging;
	public float speed, absorptionThreshold, myMagnitude;
	// myMagnitude exists because calling transform.localScale.magnitude in other scripts always returns what the magnitude was at the start for some reason.

	private Rigidbody2D rb;
	private Transform myTransform;
	private CircleCollider2D myCollider;
	private Camera myCamera, deathCam;
	private float cameraToCircleRatio;
	private AIScript myAI;

	// Start is called before the first frame update
	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		myTransform = GetComponent<Transform>();
		myCollider = GetComponent<CircleCollider2D>();
		myMagnitude = myTransform.localScale.magnitude;
		if (isPlayer)
		{
			myCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
			cameraToCircleRatio = myTransform.localScale.magnitude / myCamera.orthographicSize;
			deathCam = GameObject.FindGameObjectWithTag("DeathCamera").GetComponent<Camera>();
			deathCam.gameObject.SetActive(false);
			//Debug.Log(cameraToCircleRatio);
		}
		if (!isPlayer) // Players are always green (maybe I'll make a colour picker at some point);
		{
			GetComponent<SpriteRenderer>().color = Random.ColorHSV();
			myAI = GetComponent<AIScript>();
		}
	}

	private void OnDestroy()
	{
		if (isPlayer)
		{
			myCamera.gameObject.SetActive(false);
			deathCam.gameObject.SetActive(true);
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		Transform otherTransform = collision.gameObject.GetComponent<Transform>();
		//Debug.Log(collision.collider.gameObject.name);
		// If this circle is large enough to consume the other
		if (collision.gameObject.tag == "Circle" && myTransform.localScale.magnitude - otherTransform.localScale.magnitude >= absorptionThreshold)
		{
			// Increase the size of the circle based on the mass added from the one being consumed. 
			Debug.Log("Another circle was absorbed.");
			float targetMass = rb.mass + collision.gameObject.GetComponent<Rigidbody2D>().mass;
			// This should calculate how much bigger this circle needs to be in order to reach targetMass.
			float newRadius = Mathf.Sqrt(targetMass / Mathf.PI);
			// (myCollider.radius * myTransform.localScale.x) should get the true radius, rather than it always being 1.925 or whatever it is set to at the beginning.
			float scale = newRadius / (myCollider.radius * myTransform.localScale.x);
			myTransform.localScale = myTransform.localScale * scale;
			myMagnitude = transform.localScale.magnitude;
			Destroy(collision.gameObject);
			StartCoroutine(OnConsume(targetMass, 0.05f)); // Log mass after 50ms

			if (isPlayer)
			{
				myCamera.orthographicSize = myTransform.localScale.magnitude / cameraToCircleRatio; // Zoom out the camera
			}
			if (!isPlayer)
			{
				// This tells the AI script to find a new target since the old one was just destroyed.
				// For some reason calling GetTarget() directly here doesn't work properly. 
				myAI.targetDead = true;
			}
		}
		else if (collision.gameObject.tag == "Circle" && myTransform.localScale.magnitude - otherTransform.localScale.magnitude < absorptionThreshold)
		{
			Debug.Log("This circle isn't big enough to absorb the other one.");
			if (debugLogging)
			{
				Debug.Log(myTransform.localScale.magnitude - otherTransform.localScale.magnitude);
			}
		}
	}

	// Not included in the OnCollisionEnter because it wasn't logging an accurate actualMass.
	// As you might expect, this runs after another circle is consumed by this circle.
	private IEnumerator OnConsume(float targetMass, float delayTime)
	{
		yield return new WaitForSeconds(delayTime);
		if (debugLogging)
		{
			Debug.Log("targetMass: " + targetMass + " actualMass: " + rb.mass);
		}
	}

	private void FixedUpdate()
	{
		if (!isPlayer && Time.time < myAI.startDelay) // Give it time to actually find a target.
		{
			return;
		}

		if (isPlayer) // Allow only the circle controlled by the player to move based on input. Everything else should run off of an AI.
		{
			rb.velocity += new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * speed;
		}
		else if (!myAI.targetReached)
		{
			rb.velocity += myAI.GetDirection() * speed;
			rb.rotation = myAI.GetTargetAngle();
		}
		else if (myAI.targetReached)
		{
			myAI.GetTarget();
		}
	}
}
