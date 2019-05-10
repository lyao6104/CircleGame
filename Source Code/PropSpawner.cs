﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropSpawner : MonoBehaviour
{
	public GameObject propToSpawn;
	// spawnDelay starts at spawnDelayStart, then gets closer to spawnDelayEnd on each tick. It should get slower if max is greater than min, and faster if it is lesser.
	public float spawnDelayStart, spawnDelayEnd; 
	public int minSpawnSize, maxSpawnSize; // How many prop circles to spawn each tick
	public int tickAmount; // How many ticks pass before the spawner disables itself.
	public float initialMultiplier; // The first spawn will have its amount multiplied by this number.
	public float spawnRange;

	private float spawnDelay; // Time, in seconds, between each spawn
	private int ticksRemaining;
	private float lastTick;

	// Start is called before the first frame update
	private void OnEnable()
    {
		Invoke("Initialize", 1f);
    }

	// The first spawn tick, which 
	private void Initialize()
	{
		ticksRemaining = tickAmount;
		spawnDelay = spawnDelayStart;
		DoTick(initialMultiplier, true);
	}

	// Spawn circles, change spawnDelay, and decrease ticksRemaining.
	// multiplier is for the initial spawn.
	private void DoTick(float multiplier = 1.0f, bool isInitial = false)
	{
		if (!isInitial)
		{
			float delayDifference = spawnDelay - spawnDelayEnd;
			spawnDelay -= delayDifference / ticksRemaining;
			Debug.Log("spawnDelay changed by " + -(delayDifference / ticksRemaining) + ". It is now " + spawnDelay + ".");
		}
		int spawnCount = Mathf.RoundToInt(Random.Range(minSpawnSize, maxSpawnSize + 1) * multiplier);
		for (int i = 0; i < spawnCount; i++)
		{
			SpawnProp();
		}
		Debug.Log(spawnCount + " " + propToSpawn.name + "s were spawned.");
		lastTick = Time.time;
		ticksRemaining--;
		if (ticksRemaining <= 0)
		{
			Debug.Log("This spawner has been depleted.");
			gameObject.SetActive(false);
		}
	}

	// Instantiates a new prop circle
	private void SpawnProp()
	{
		Vector3 spawnPos = new Vector3(transform.position.x + Random.Range(-spawnRange, spawnRange), transform.position.y + Random.Range(-spawnRange, spawnRange));
		GameObject newSpawn = Instantiate(propToSpawn, spawnPos, transform.rotation);
		// Give the newly spawned object some velocity to get it away from other spawned objects. (Or at the very least make sure it's not partially inside anything)
		newSpawn.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-spawnRange, spawnRange), Random.Range(-spawnRange, spawnRange)));
	}

	// Update is called once per frame
	private void Update()
    {
        if (Time.time - lastTick >= spawnDelay && ticksRemaining > 0)
		{
			DoTick();
		}
    }
}