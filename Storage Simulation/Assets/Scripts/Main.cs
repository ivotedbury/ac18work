using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public Structure structure;
    public Settings settings;
    public ToteManager toteManager;
    public RobotManager robotManager;


    void Start()
    {
        LoadSettings();

        toteManager.LoadTotes(structure);
        robotManager.InitialiseRobots(structure);

        Time.timeScale = 0.1f;

    }

    void Update()
    {

    }

    void LoadSettings()
    {
        string _settingsImportString = System.IO.File.ReadAllText("Assets/Settings/" + "settingsImport.txt");

        settings = JsonUtility.FromJson<Settings>(_settingsImportString);

       // print(settings.testItem);

    }

    //void Load
}

