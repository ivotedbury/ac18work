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

        LoadSavedTotes();

        //CreateNewTotes();  
    }

    private void CreateNewTotes()
    {
        int toteNumber = 0;
        for (int x = 0; x < Constants.MAIN_STRUCTURE_DIMS.x; x++)
        {
            for (int y = 0; y < Constants.MAIN_STRUCTURE_DIMS.y; y++)
            {
                for (int z = 0; z < Constants.MAIN_STRUCTURE_DIMS.z; z++)
                {
                    AddTote(structure.nodesArray[x, y, z], toteNumber, Random.Range(0, 11), Random.Range(1, 71));
                    toteNumber++;
                }
            }
        }
    }

    private void AddTote(Node _node, int _toteNumber, int _productId, int _quantity)
    {
        Tote newTote = Instantiate(referenceTote, _node.transform.position + Constants.TOTE_RAISE, Quaternion.identity, this.transform);
        int newToteNumber = _toteNumber;
        newTote.PopulateItems(_productId, _quantity);
        newTote.gridPos = _node.gridPos;
        newTote.name = "Tote" + newToteNumber.ToString();
        newTote.number = newToteNumber;
        newTote.properties = properties;

        allTotes.Add(newTote);
    }

    void Update()
    {
        if (Input.GetKeyDown("o"))
        {
            LoadSavedTotes();
        }

        if (Input.GetKeyDown(","))
        {
            SaveTotes();
        }

        //if (Input.GetButtonDown("0"))
        //{
        //     ray = Camera.main.ScreenPointToRay(Input.MousePosition);
        //    Raycasthit hit;
        //    //this if checks, a detection of hit in an GameObject with the mouse on screen
        //    if (Physics.Raycast(ray, hit))
        //    {
        //        //GameObject.Find("Nameofyourobject") search your gameobject on the hierarchy with the desired name and allows you to use it
        //        Destroy(GameObject.Find(hit.name));
        //    }
        //}
    }

    public void LoadSavedTotes()
    {
        List<Tote> totesToLoad = new List<Tote>();

        ToteCollection totesState = new ToteCollection();

        string _settingsImportString = System.IO.File.ReadAllText("Assets/Data/ToteData/" + "toteDataImport_200218_ExampleB.txt");

        totesState = JsonUtility.FromJson<ToteCollection>(_settingsImportString);
        List<ToteDataItem> toteDataItemsToLoad = new List<ToteDataItem>();
        toteDataItemsToLoad = totesState.allTotes;

        for (int i = 0; i < toteDataItemsToLoad.Count; i++)
        {
            Node nodeToLoadTo = structure.nodesArray[toteDataItemsToLoad[i].gridPos.x, toteDataItemsToLoad[i].gridPos.y, toteDataItemsToLoad[i].gridPos.z];

            List<ProductDataItem> productDataItemsToLoad = new List<ProductDataItem>();
            productDataItemsToLoad = toteDataItemsToLoad[i].toteProducts;

            Debug.Log(productDataItemsToLoad.Count);

            AddTote(nodeToLoadTo, toteDataItemsToLoad[i].number, productDataItemsToLoad[0].productId, productDataItemsToLoad.Count);
        }
    }

    void SaveTotes()
    {
        string toteDataToExportPath = "Assets/Data/ToteData/" + "toteDataImport_200218_ExampleB.txt";

        ToteCollection totesState = new ToteCollection();

        for (int i = 0; i < allTotes.Count; i++)
        {
            List<ProductDataItem> productsToAdd = new List<ProductDataItem>();
            foreach (Product product in allTotes[i].toteProducts)
            {
                productsToAdd.Add(new ProductDataItem(product.productId));
            }

            totesState.allTotes.Add(new ToteDataItem(productsToAdd, allTotes[i].number, allTotes[i].gridPos));
        }

        Debug.Log(totesState.allTotes.Count);

        string dataToExport = JsonUtility.ToJson(totesState, true);

        Debug.Log(dataToExport);

        System.IO.File.WriteAllText(toteDataToExportPath, dataToExport);
    }

}
