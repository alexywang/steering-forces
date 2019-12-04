using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdvertiserManager : MonoBehaviour
{
	public static AdvertiserManager Instance = null;
	public GameObject advertiserPrefab;
	public List<GameObject> advertisers;
	int maxAdvertisers = 3;

	// Parameters
	public float timeBetweenAds = 3f;

	public float dropProbability = 0.3f;
	public float pitchDistance = 0.2f;

	public Slider probSlider;
	public Text probText;

	public Slider rateSlider;
	public Text rateText;

	public Slider pitchSlider;
	public Text pitchText;

	public Slider maxSlider;
	public Text maxText;


	// Start is called before the first frame update
	void Start()
    {
		if (Instance == null) Instance = this;
		else Destroy(this);

		for(int i = 0; i < 3; i++)
		{
			SpawnAdvertiser();
		}
    }

    // Update is called once per frame
    void Update()
    {
		dropProbability = probSlider.value;
		probText.text = "Ad Probability: " + Mathf.Round(dropProbability*10)/10;
		timeBetweenAds = rateSlider.value;
		rateText.text = "Ad Rate: " + timeBetweenAds;
		pitchDistance = pitchSlider.value;
		pitchText.text = "Pitch Distance: " + Mathf.Round(pitchDistance*10)/10;
		maxAdvertisers = (int)maxSlider.value;
		maxText.text = "Max Advertisers: " + maxAdvertisers;
		if(advertisers.Count < maxAdvertisers)
		{
			SpawnAdvertiser();
		}
		while(advertisers.Count > maxAdvertisers)
		{
			Destroy(advertisers[advertisers.Count-1]);
			advertisers.RemoveAt(advertisers.Count-1);

		}
	}

	public void SpawnAdvertiser()
	{
		float xMin = -6.25f;
		float xMax = 6.25f;
		float yMin = -2.75f;
		float yMax = -2.75f;
		Vector2 location = new Vector2();
		bool valid = false;
		while (!valid)
		{
			location = new Vector2(Random.Range(xMin, xMax), Random.Range(yMin, yMax));
			if (WorldManager.Instance.GetMinDistance(location) > 0.5)
			{
				valid = true;
			}
		}

		GameObject newAdvertiser = Instantiate(advertiserPrefab, location, Quaternion.identity);
		advertisers.Add(newAdvertiser);

	}
}
