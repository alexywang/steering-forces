using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Advertiser : SteeringAgent
{



	public override List<GameObject> obstacles
	{
		get
		{
			List<GameObject> all = new List<GameObject>();
			all.AddRange(_obstacles);
			all.AddRange(AdvertiserManager.Instance.advertisers);
		
			foreach (GameObject a in AdvertiserManager.Instance.advertisers)
			{
				_superAvoid.Add(a);
			}

			all.Remove(gameObject);
			return all;
		}
	}

	// Increase speed

	public override float maxVelocity => base.maxVelocity * 1.1f;
	public override float maxAvoidForce => base.maxAvoidForce * 1.5f;
	public override float maxSeekForce => base.maxSeekForce * 1.1f;



	// Dropping Ads
	public GameObject flyerPrefab;
	public GameObject seekPost;
	float timeSinceAd = 0f;


	// Pursuing customer
	bool pursuing = false;
	public float timeNearShopper = 0f;
	float timeToSell = 4f;
	public int successfulPitches = 0;
	public int requiredPitches = 3;
	bool done = false;

	public Material pursuitMaterial;
	public Material regularMaterial;
	public TextMesh scoreText;

	//Parameters
	float timeBetweenAds
	{
		get => AdvertiserManager.Instance.timeBetweenAds;
	}

	float dropProbability { get { return AdvertiserManager.Instance.dropProbability; } }
	float pitchDistance { get { return AdvertiserManager.Instance.pitchDistance; }}



	// Start is called before the first frame update
	void Start()
    {
		base.Start();
    }

    // Update is called once per frame
    void Update()
    {
		base.Update();




		// Time to despawn
		if (done && Vector2.Distance(seekTarget.transform.position, transform.position) < 1f)
		{
			AdvertiserManager.Instance.advertisers.Remove(gameObject);
			Destroy(gameObject);
		}
	
		// While not pursuing a customer
		if (!pursuing && !done)
		{
			// Countdown to next drop
			timeSinceAd += Time.deltaTime;
			if (timeSinceAd > timeBetweenAds)
			{
				DropAd();
			}

			GetComponentInChildren<Renderer>().material = regularMaterial;
			if (seekTarget == null)
			{
				seekTarget = GenerateTarget();
			}
			else
			{
				if (Vector2.Distance(seekTarget.transform.position, transform.position) < 0.2f)
				{
					if (!seekTarget.GetComponent<Shopper>())
					{
						Destroy(seekTarget);
					}
					seekTarget = null;
				}
			}
		}
		else if(!done && pursuing) // While pursuing a customer
		{
			//Reset countdown
			timeSinceAd = 0;
			GetComponentInChildren<Renderer>().material = pursuitMaterial;
			// Lost customer
			if(seekTarget == null)
			{
				GetComponentInChildren<Renderer>().material = regularMaterial;
				pursuing = false;
			}
			else
			{
				if (Vector2.Distance(seekTarget.transform.position, transform.position) < pitchDistance)
				{
					timeNearShopper += Time.deltaTime;
					if (timeNearShopper > timeToSell)
					{
						seekTarget.GetComponentInChildren<Renderer>().material = seekTarget.GetComponent<Shopper>().regularMaterial;
						timeNearShopper = 0;
						successfulPitches++;
						scoreText.text = "" + successfulPitches;
						seekTarget = null;
						pursuing = false;

						if (successfulPitches >= requiredPitches)
						{
							seekTarget = WorldManager.Instance.GetRandomDespawnPoint();
							done = true;
						}

					}
				}
				else
				{
					timeNearShopper = 0;
				}
			}
		}
			

    }

	public void TargetShopper(GameObject shopper)
	{
		if (seekTarget)
		{
			if (!seekTarget.GetComponent<Shopper>())
			{
				Destroy(seekTarget);
			}
			seekTarget = null;
		}
		pursuing = true;
		seekTarget = shopper;
	}




	// Generate a random seek target away from obstacles except shopps
	GameObject GenerateTarget()
	{
		float xMax = 6;
		float xMin = 3.5f;

		float yMax = 3.36f;
		float yMin = -3.23f;

		int[] side = { -1, 1 };

		Vector2 location = new Vector2(side[Random.Range(0, side.Length)]*Random.Range(xMin, xMax), Random.Range(yMin, yMax));
		int iterations = 0;
		while (WorldManager.Instance.GetMinDistance(location) < 1.5f)
		{
			location = new Vector2(Random.Range(xMin, xMax), Random.Range(yMin, yMax));
			iterations++;
			if(iterations > 500)
			{
			}
		}

		GameObject newTarget = Instantiate(seekPost, location, Quaternion.identity);
		newTarget.GetComponent<Renderer>().enabled = false;
		return newTarget;
	}



	void DropAd()
	{
		float prob = Random.Range(0, 1f);
		if (prob > 1-dropProbability) {
			GameObject newFlyer = Instantiate(flyerPrefab, transform.position, Quaternion.identity);
			newFlyer.GetComponent<Flyer>().owner = this;
			Physics2D.IgnoreCollision(GetComponentInChildren<Collider2D>(), newFlyer.GetComponentInChildren<Collider2D>());
		}
		timeSinceAd = 0f;

	}

	// Override so that it still flees the shopper even when seeking it
	protected override Vector2 GetTotalFleeForce()
	{
		Vector2 flee = Vector2.zero;
		foreach (GameObject obstacle in fleeObstacles)
		{
			if (seekTarget == null || obstacle != gameObject)
			{
				flee += _superAvoid.Contains(obstacle) ? GetFleeForce(obstacle, true) : GetFleeForce(obstacle, false);
			}
		}

		return Vector2.ClampMagnitude(flee, maxFleeForce);
	}

}
