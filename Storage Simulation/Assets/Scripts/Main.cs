using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public Structure structure;
    public Settings settings;

    void Start()
    {
        LoadSettings();
    }
    // Update is called once per frame
    void Update()
    {

    }

    void LoadSettings()
    {
        string _settingsImportString = System.IO.File.ReadAllText("Assets/Settings/" + "settingsImport.txt");

        settings = JsonUtility.FromJson<Settings>(_settingsImportString);

        print(settings.testItem);

    }
}

