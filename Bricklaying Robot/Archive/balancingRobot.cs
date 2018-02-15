using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class balancingRobot : MonoBehaviour {

    public GameObject beam;
    public GameObject column;
    public GameObject brick;
    public GameObject freeColumn;

    Transform beamCentre;
    float brickDisplacement = 0.15f;
    float freeColumnDisplacement;
    
    // Use this for initialization
    void Start () {
        beam.transform.position = new Vector3(0, 0.32f, 0);

    }
	
	// Update is called once per frame
	void Update () {

        freeColumnDisplacement = -brickDisplacement * 3;

        beamCentre = beam.transform;

        brick.transform.position = beamCentre.transform.position + beamCentre.transform.rotation * new Vector3(brickDisplacement, 0.07f, 0);
        brick.transform.rotation = beamCentre.transform.rotation;

        freeColumn.transform.position = beamCentre.transform.position + beamCentre.transform.rotation * new Vector3(freeColumnDisplacement, 0.07f, 0);
        freeColumn.transform.rotation = beamCentre.transform.rotation;

        if (Input.GetKeyDown("l"))
        {
            brickDisplacement = brickDisplacement + 0.02f;
            print("right");
        }
        if (Input.GetKeyDown("k"))
        {
            brickDisplacement = brickDisplacement - 0.02f;
            print("left");
        }
    }
}
