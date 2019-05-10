/* Name: Larry Y.
 * Date: April 30, 2019
 * Desc: The primary script for circles. */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleScript : MonoBehaviour
{
	public bool isPlayer, debugLogging;
	public float speed, absorptionThreshold;

	private Rigidbody2D rb;
	private Transform myTransform;
	private CircleCollider2D myCollider;
	private Camera myCamera;
	private float cameraToCircleRatio;

	// Start is called before the first frame update
	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		myTransform = GetComponent<Transform>();
		myCollider = GetComponent<CircleCollider2D>();
		if (isPlayer)
		{
			myCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
			cameraToCircleRatio = myTransform.localScale.magnitude / myCamera.orthographicSize;
			//Debug.Log(cameraToCircleRatio);
		}
		if (!isPlayer) // Players are always green (maybe I'll make a colour picker at some point);
		{
			GetComponent<SpriteRenderer>().color = Random.ColorHSV();
		}
		
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		Transform otherTransform = collision.gameObject.GetComponent<Transform>();
		//Debug.Log(collision.collider.gameObject.name);
		// This will increase the size of the circle based on the mass added from the one being consumed. 
		if (collision.gameObject.tag == "Circle" && myTransform.localScale.magnitude - otherTransform.localScale.magnitude >= absorptionThreshold)
		{
			Debug.Log("Another circle was absorbed.");
			float targetMass = rb.mass + collision.gameObject.GetComponent<Rigidbody2D>().mass;
			// This should calculate how much bigger this circle needs to be in order to reach targetMass.
			float newRadius = Mathf.Sqrt(targetMass / Mathf.PI);
			// (myCollider.radius * myTransform.localScale.x) should get the true radius, rather than it always being 1.925 or whatever it is set to at the beginning.
			float scale = newRadius / (myCollider.radius * myTransform.localScale.x);
			myTransform.localScale = myTransform.localScale * scale;
			Destroy(collision.gameObject);
			StartCoroutine(OnConsume(targetMass, 0.05f)); // Log mass after 50ms
			myCamera.orthographicSize = myTransform.localScale.magnitude / cameraToCircleRatio;
		}
		else if (myTransform.localScale.magnitude - otherTransform.localScale.magnitude < absorptionThreshold)
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
		if (isPlayer) // Allow only the circle controlled by the player to move based on input. Everything else should run off of an AI.
		{
			rb.velocity += new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * speed;
		}
	}
}
