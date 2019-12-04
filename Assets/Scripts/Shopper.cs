using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shopper : SteeringAgent
{
	public Material flyeredMaterial;
	public Material regularMaterial;
    // Start is called before the first frame update
    void Start()
    {
		base.Start();

		// Set seek target to a despawn point upon spawn
		//seekTarget = WorldManager.Instance.GetRandomDespawnPoint(); 
		Physics2D.IgnoreLayerCollision(9, 9);
    }

	protected virtual void OnCollisionEnter2D(Collision2D c)
	{

		if (c.gameObject.GetComponentInChildren<Despawn>())
		{
			AgentManager.Instance.shoppers.Remove(gameObject);
			Destroy(gameObject);
		}

	}

	public void GetFlyered()
	{
		StartCoroutine(PauseMovement(2f));
		GetComponentInChildren<Renderer>().material = flyeredMaterial;
	}
}
