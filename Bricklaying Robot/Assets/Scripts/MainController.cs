using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{

    public TextAsset brickData;

    BrickArrangment _brickArrangment =  new BrickArrangment(20,20,41);

    void Start()
    {
           BrickImportItem [] brickImportArray = _brickArrangment.ImportBrickData(brickData);


       

        print(brickImportArray.Length);

        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
