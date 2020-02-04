using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{

    // List<Tote> Totes = new List<Tote>();
    //GameObject[,,] toteGrid = new GameObject[10, 10, 10];
    //public GameObject tote;

    Tote[,,] toteGrid = new Tote[10, 10, 10];
    public Tote tote;

    float xFactor = 0.5f;
    float yFactor = 0.5f;
    float zFactor = 0.7f;

    //public 

    // Use this for initialization
    void Start()
    {
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                for (int z = 0; z < 10; z++)
                {
                    toteGrid[x, y, z] = Instantiate(tote, tote.transform.position + new Vector3(xFactor * x, yFactor * y, zFactor * z), tote.transform.rotation);

                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (Time.time > 2)
        //{
        //    Debug.Log("Line");

        //    int i = 0;

        //    for (int x = 0; x < 10; x++)
        //    {
        //        for (int y = 0; y < 10; y++)
        //        {
        //            for (int z = 0; z < 10; z++)
        //            {
        //                toteGrid[x, y, z].transform.position = new Vector3(i * xFactor, 0, 0);
        //                Debug.Log(i);
        //                i++;

        //            }
        //        }
        //    }
        //}
    }
}

