using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaver : Shopper
{

    // Start is called before the first frame update
    void Start()
    {
		base.Start();
		// Upon spawn choose a despawn point to target
		seekTarget = WorldManager.Instance.GetRandomDespawnPoint();
    }


}
