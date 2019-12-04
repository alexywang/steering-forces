using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buyer : Shopper
{
	GameObject targetStore;
	GameObject targetChair;


    // Start is called before the first frame update
    void Start()
    {
		base.Start();
		// Decide a store to go to
		TargetStore();
    }

	void Update()
	{
		base.Update();
		// Check after each movement if we've reached the store yet
		if (targetStore && Vector2.Distance(targetStore.transform.position, transform.position) < 0.7f)
		{
			// Pause, then go to eat.
			StartCoroutine(PauseMovement(1f));
			targetStore = null;
			seekTarget = null;
			TargetChair();
		}



	}

	public void OnCollisionEnter2D(Collision2D c)
	{
		base.OnCollisionEnter2D(c);
		// Sit in chair when colliding with it
		if(c.gameObject == targetChair)
		{
			TakeSeat();
		}
	}


	// Head towards store
	public void TargetStore()
	{
		// Get a random shop from world manager
		targetStore = WorldManager.Instance.shops[Random.Range(0, WorldManager.Instance.shops.Count)];
		seekTarget = targetStore;
	}

	// Head towards chair
	public void TargetChair()
	{
		if(WorldManager.Instance.chairs.Count == 0)
		{
			// If no seats to eat just leave
			TargetDespawn();
		}
		else
		{
			targetChair = WorldManager.Instance.chairs[Random.Range(0, WorldManager.Instance.chairs.Count)];
			WorldManager.Instance.ReserveChair(targetChair);
			seekTarget = targetChair;
		}
		
	}

	public void TakeSeat()
	{
		GameObject seat = seekTarget;
		WorldManager.Instance.UnreserveChair(seat);
		seekTarget = null;
		Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), targetChair.gameObject.GetComponent<Collider2D>());
		transform.position = targetChair.transform.position;
		StartCoroutine(PauseMovement(Random.Range(2f,3f)));
		TargetDespawn();
	}

	// Head towards despawn
	public void TargetDespawn()
	{
		targetChair = null;
		seekTarget = WorldManager.Instance.GetRandomDespawnPoint();
	}

}
