using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flyer : MonoBehaviour
{
	public Advertiser owner;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	// TODO: Notify the advertiser that the shopper as been flyered
	public void NotifyAdvertiser(GameObject shopper)
	{
		if (owner.successfulPitches < owner.requiredPitches)
		{
			owner.TargetShopper(shopper);
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		Shopper shopper = collision.GetComponent<Shopper>();
		if (shopper && shopper.GetComponentInChildren<Renderer>().material != shopper.flyeredMaterial)
		{
			NotifyAdvertiser(collision.gameObject);
			shopper.GetFlyered();
			Destroy(gameObject);
		}
	}
}
