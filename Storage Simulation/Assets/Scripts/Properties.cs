using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Properties : MonoBehaviour {

    public string type;
    public int number;

    private void Start()
    {
        ClearProperties();
        Publish();
    }

    public void Publish()
    {
        print("Type: " + type);
        print("Number: " + number.ToString());
    }

   public void ClearProperties()
    {
        type = "Null";
        number = 0;
    }

}
