using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public Structure structure;
    public Settings settings;

    public Robot robot;

    List<Robot> allRobots = new List<Robot>();

    void Start()
    {
        LoadSettings();

        for (int i = 0; i < 5; i++)
        {
            allRobots.Add(Instantiate(robot, structure.nodesArray[i,0].transform.position, Quaternion.identity));
            allRobots[i].currentNode = structure.nodesArray[i, 0];
        }
    }

    void Update()
    {

    }

    void LoadSettings()
    {
        string _settingsImportString = System.IO.File.ReadAllText("Assets/Settings/" + "settingsImport.txt");

        settings = JsonUtility.FromJson<Settings>(_settingsImportString);

        print(settings.testItem);

    }

    //void Load
}

