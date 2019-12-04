using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgentManager : MonoBehaviour
{
	public GameObject shopperPrefab;

	public Slider slider;
	public Text numAgentsText;

	public static AgentManager Instance = null;
	public int maxShoppers = 7;
	public List<GameObject> shoppers = new List<GameObject>();

	public Material flyeredMaterial;

	float spawnRate = 0.5f;
	float timeSinceLastSpawn = 0f;

	public Material buyerMaterial;
	public Material leaverMaterial;
	
	
    // Start is called before the first frame update
    void Start()
    {
		if (Instance == null) Instance = this;
		else Destroy(this);
    }

    // Update is called once per frame
    void Update()
    {
		timeSinceLastSpawn += Time.deltaTime;
		if(shoppers.Count < maxShoppers && timeSinceLastSpawn > spawnRate)
		{
			SpawnShopper();
			timeSinceLastSpawn = 0;
		}

		maxShoppers = (int) slider.value;
		numAgentsText.text = "Max Shoppers: "+maxShoppers;
    }



	void SpawnShopper()
	{
		float xSpawn = -6.25f;
		float yMin = -2.75f;
		float yMax = 2.75f;

		Vector3 location = new Vector3(xSpawn, Random.Range(yMin, yMax), 0);
		GameObject newShopper = Instantiate(shopperPrefab, location, Quaternion.identity);
		float assignment = Random.Range(0, 1f);
		if (assignment > 0.5f)
		{
			newShopper.AddComponent<Leaver>();
			newShopper.GetComponent<Shopper>().regularMaterial = leaverMaterial;

		}
		else
		{
			newShopper.AddComponent<Buyer>();
			newShopper.GetComponent<Shopper>().regularMaterial = buyerMaterial;

		}
		newShopper.GetComponentInChildren<Renderer>().material = newShopper.GetComponent<Shopper>().regularMaterial;
		newShopper.GetComponent<Shopper>().flyeredMaterial = flyeredMaterial;

		shoppers.Add(newShopper);

		//	// Ignore collisions with other shoppers and advertisers
		//	foreach (GameObject advertiser in AdvertiserManager.Instance.advertisers)
		//	{
		//		if(advertiser)
		//			Physics2D.IgnoreCollision(newShopper.GetComponentInChildren<Collider2D>(), advertiser.GetComponentInChildren<Collider2D>());

		//	}
		//	foreach(GameObject shopper in shoppers)
		//	{
		//		if (shopper)
		//		{
		//			Physics2D.IgnoreCollision(shopper.GetComponentInChildren<Collider2D>(), newShopper.GetComponentInChildren<Collider2D>());
		//		}
		//	}
		//
	}


}
