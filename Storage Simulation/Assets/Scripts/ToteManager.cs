using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToteManager : MonoBehaviour
{

    public Tote referenceTote;

    Structure structure;

    public List<Tote> allTotes = new List<Tote>();

    public Properties properties;

    public int numberOfTotes;

    public void LoadTotes(Structure _structure)
    {
        structure = _structure;

        foreach (Node aNode in structure.nodesArray)
        {
            AddTote(allTotes, aNode, Random.Range(0, 11), Random.Range(0, 71));
        }
    }

    private void AddTote(List<Tote> _toteList, Node _node, int _productId, int _quantity)
    {
        Tote newTote = Instantiate(referenceTote, _node.transform.position + Constants.TOTE_RAISE, Quaternion.identity, this.transform);
        int newToteNumber = _toteList.Count;
        newTote.PopulateItems(_productId, _quantity);
        newTote.name = "Tote" + newToteNumber.ToString();
        newTote.number = newToteNumber;
        newTote.properties = properties;

        allTotes.Add(newTote);
    }

    private void Update()
    {
        if (Input.GetKeyDown("o"))
        {
            LoadSavedTotes();
        }

        if (Input.GetKeyDown(","))
        {
            SaveTotes();
        }
    }

    void LoadSavedTotes()
    {
        List<Tote> totesToLoad = new List<Tote>();

        string _settingsImportString = System.IO.File.ReadAllText("Assets/Data/ToteData/" + "toteDataImporta.txt");

        totesToLoad = JsonUtility.FromJson<List<Tote>>(_settingsImportString);
    }

    void SaveTotes()
    {
        string toteDataToExportPath = "Assets/Data/ToteData/" + "toteDataImporta.txt";

        ToteDataItem totesState = new ToteDataItem();

        for (int i = 0; i < allTotes.Count; i++)
        {
            totesState.allTotes.Add(allTotes[i]);
            //totesState.allTotes[i].number = allTotes[i].number;
            //totesState.allTotes[i].position = allTotes[i].transform.position;
            //totesState.allTotes[i].rotation = allTotes[i].transform.rotation;
        }

        Debug.Log(totesState.allTotes.Count);

        string dataToExport = JsonUtility.ToJson(totesState);

        Debug.Log(dataToExport);

        System.IO.File.WriteAllText(toteDataToExportPath, dataToExport);
    }

}
