using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
	bool show = true;
	public GameObject sliders;
    // Start is called before the first frame update
    void Start()
    {
		Toggle();   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void Toggle()
	{
		show = !show;
		CanvasRenderer[] renderers = sliders.GetComponentsInChildren<CanvasRenderer>();
		Debug.Log(renderers.Length);
		foreach(CanvasRenderer r in renderers)
		{
			r.cull = show;
		}
	}
	

}
