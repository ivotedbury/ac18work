using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class NewBehaviourScript : MonoBehaviour
{

    public GameObject sphere;
    public GameObject auger;

    List<GameObject> spheres = new List<GameObject>();
    List<Vector3> spheresPos = new List<Vector3>();
    List<float> spheresDistance = new List<float>();


    float distanceArray = 6f;
    Vector3 spawnPos;
    Quaternion spawnRot;
    int rotationAngle = 0;
    int overallCount = 0;
    // Use this for initialization
    void Start()
    {
        Time.timeScale = 5;
        spawnPos = sphere.transform.position;
        spawnRot = sphere.transform.rotation;
        for (int i = 0; i < 100; i++)
        {
            //Vector3 spherePos = new Vector3(Random.Range(0, distanceArray), Random.Range(0, distanceArray), Random.Range(0, distanceArray)) + sphere.transform.position;
            //spheres.Add(Instantiate(sphere, spherePos, sphere.transform.rotation));
            //spheres[spheres.Count - 1].GetComponent<Renderer>().material.color = (new Color(Random.Range(0,1f), Random.Range(0, 1f), Random.Range(0, 1f)));
        }
    }

    // Update is called once per frame
    void Update()
    {
        Color sphereColour;

        if (spheres.Count < 600)
        {
            Vector3 spherePos = /*new Vector3(Random.Range(0, distanceArray), Random.Range(0, distanceArray), Random.Range(0, distanceArray)) + */spawnPos;
            spheres.Add(Instantiate(sphere, spherePos, spawnRot));
            spheresPos.Add(new Vector3(0, 0, 0));
            spheresDistance.Add(0);
            //if (Random.Range(0, 2) == 0)
            //{
            //    sphereColour = Color.red;
            //}
            //else
            //{
            //    sphereColour = Color.blue;
            //}
            //spheres[spheres.Count - 1].GetComponent<Renderer>().material.color = sphereColour;
        }

        for (int i = 0; i < spheres.Count; i++)
        {
            Vector3 newSpherePosition = spheres[i].transform.position;
            spheresDistance[i] = Vector3.Magnitude(newSpherePosition - spheresPos[i]);
            spheresPos[i] = newSpherePosition;
            float colourFactor = spheresDistance[i] / spheresDistance.Average() / 2;
            spheres[i].GetComponent<Renderer>().material.color = new Color(1, colourFactor, colourFactor);

        }

             auger.transform.rotation = Quaternion.Euler(0, rotationAngle, 0);

        if (overallCount < 5)
        {
            rotationAngle = rotationAngle + 4;
        }
        else
        {
            rotationAngle = rotationAngle - 2;
        }

        if (rotationAngle == 360 || rotationAngle == -360)
        {
            rotationAngle = 0;
            overallCount++;
        }
    }
}
