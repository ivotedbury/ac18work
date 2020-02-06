using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tote : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnMouseOver()
    {
        GetComponent<Renderer>().material.color = Constants.TOTE_SELECTED;
    }

    void OnMouseExit()
    {
        GetComponent<Renderer>().material.color = Constants.TOTE_NORMAL;
    }
}
