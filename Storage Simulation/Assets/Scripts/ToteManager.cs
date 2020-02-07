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
                  AddTote(allTotes, aNode, Random.Range(0,11), Random.Range(0, 71));
        }
    }
    private void AddTote(List<Tote> _toteList, Node _node, int _productId, int _quantity)
    {
        Tote newTote = Instantiate(referenceTote, _node.transform.position, Quaternion.identity, this.transform);
        int newToteNumber = _toteList.Count;
        newTote.PopulateItems(_productId, _quantity);
        newTote.name = "Tote" + newToteNumber.ToString();
        newTote.number = newToteNumber;
        newTote.properties = properties;

        allTotes.Add(newTote);
    }

}
