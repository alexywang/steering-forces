using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
	public static WorldManager Instance = null;

	// Procedurally generated objects
	public GameObject tablePrefab;
	public GameObject chairPrefab;
	public GameObject planterPrefab;

	// Reference to all objects
	public List<GameObject> tables;
	public List<GameObject> chairs;
	public List<GameObject> reservedChairs;
	public List<GameObject> planters;
	
	// References should be set in scene.
	public List<GameObject> shops;
	public List<GameObject> despawnPoints;

	// Size of moving agents to ensure that there is enough space to traverse around the mall.
	public GameObject agentPrefab; 

	// Random properties
	int numTables;
	int numPlanters;

	// Start is called before the first frame update
	void Start()
    {
		if(Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(this);
		}

		tables = new List<GameObject>();
		chairs = new List<GameObject>();
		planters = new List<GameObject>();
		GenerateWorld();	
    }


    // Update is called once per frame
    void Update()
    {
        
    }

	// Generate randomly spawned objects
	public void GenerateWorld()
	{
		bool worldIsValid = false;
		// Randomly initialize properties of the map
		numTables = Random.Range(3, 5);
		numPlanters = Random.Range(2, 6);

		// Spawn Tables
		int iterations = 0;
		while (!worldIsValid)
		{
			foreach(GameObject t in tables)
			{
				Destroy(t);
			}
			foreach(GameObject c in chairs)
			{
				Destroy(c);
			}
			tables = new List<GameObject>();
			chairs = new List<GameObject>();
			for (int i = 0; i < numTables; i++)
			{
				// Pick a random direction and magnitude
				Vector2 spawnPos = new Vector2(Random.Range(-3.5f, 3.5f), Random.Range(-2f, 2f));
				GameObject newTable = Instantiate(tablePrefab, spawnPos, Quaternion.identity);
				tables.Add(newTable);
				GenerateChairs(newTable, Random.Range(4,6), 0.5f);

			}

			worldIsValid = CheckValidWorld(4);

			iterations++;
			if(iterations > 1000)
			{
				return;
			}

		}

		worldIsValid = false;

		iterations = 0;
		// Spawn planters
		while (!worldIsValid)
		{
			foreach(GameObject p in planters)
			{
				Destroy(p);
			}

			planters = new List<GameObject>();
			for(int i = 0; i < numPlanters; i++)
			{
				Vector2 spawnPos = new Vector2(Random.Range(-4.5f, 4.5f), Random.Range(-2f, 2f));
				GameObject newPlanter = Instantiate(planterPrefab, spawnPos, Quaternion.identity);
				planters.Add(newPlanter);
			}

			worldIsValid = CheckValidWorld(3);

			iterations++;
			if (iterations > 5000)
			{
				numPlanters = 2;
			}

		}


	}

	// Checks if there is enough space between objects for agents to walk through
	bool CheckValidWorld(float factor)
	{
		float agentSize = agentPrefab.transform.localScale.x;
		List<GameObject> allObjects = new List<GameObject>();
		allObjects.AddRange(tables);
		allObjects.AddRange(planters);


		// Check nearest points between all pairs of objects
		for (int i = 0; i < allObjects.Count; i++)
		{
			Collider2D c1 = allObjects[i].GetComponentInChildren<Collider2D>();
			for (int j = i + 1; j < allObjects.Count; j++)
			{
				Collider2D c2 = allObjects[j].GetComponentInChildren<Collider2D>();
				if (c1.Distance(c2).distance < agentSize*factor)
				{
					return false;
				}
			}
		}

		return true;
	}

	// Gets the minimum distance between an object and the obstacles in the world
	public float GetMinDistance(Vector2 location)
	{
		float minDistance = Mathf.Infinity;
		List<GameObject> obstacles = new List<GameObject>();
		obstacles.AddRange(tables);
		obstacles.AddRange(chairs);
		obstacles.AddRange(reservedChairs);
		obstacles.AddRange(planters);
		foreach(GameObject o in obstacles)
		{
			float dist = Vector2.Distance(location, o.transform.position);
			if (dist < minDistance)
			{
				minDistance = dist;
			}
		}

		return minDistance;
	}


	// Generate chairs around a table object
	void GenerateChairs(GameObject table, int numChairs, float radius)
	{
		for(int i = 0; i < numChairs; i++)
		{
			float angle = i * Mathf.PI * 2f / numChairs;
			Vector3 spawnPos = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
			GameObject newChair = Instantiate(chairPrefab, table.transform.position + spawnPos, Quaternion.identity);
			chairs.Add(newChair);
		}
	}

	// Returns a random position off the right of the screen
	public GameObject GetRandomDespawnPoint()
	{
		int index = Random.Range(0, despawnPoints.Count);
		return despawnPoints[index];
	}

	// Get all static obstacles to avoid
	public List<GameObject> GetAllObstacles()
	{
		List<GameObject> allObjects = new List<GameObject>();
		allObjects.AddRange(tables);
		allObjects.AddRange(planters);
		allObjects.AddRange(shops);
		allObjects.AddRange(chairs);
		allObjects.AddRange(reservedChairs);
		return allObjects;
	}

	public void ReserveChair(GameObject reserved)
	{
		chairs.Remove(reserved);
		reservedChairs.Add(reserved);
	}

	public void UnreserveChair(GameObject chair)
	{
		chairs.Add(chair);
		reservedChairs.Remove(chair);
	}

}
