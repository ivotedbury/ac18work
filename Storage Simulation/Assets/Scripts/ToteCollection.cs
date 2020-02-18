using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class ToteCollection
{
    public int structureId;
    public List<ToteDataItem> allTotes = new List<ToteDataItem>();
}

[System.Serializable]

public class ToteDataItem
{
    public int number;
    public Vector3Int gridPos;
    public List<ProductDataItem> toteProducts = new List<ProductDataItem>();


    public ToteDataItem(List<ProductDataItem> _toteProducts, int _number, Vector3Int _gridPos)
    {
        number = _number;
        gridPos = _gridPos;
        toteProducts = _toteProducts;
    }
}

[System.Serializable]

public class ProductDataItem
{
    public int productId;

    public ProductDataItem(int _productId)
    {
        productId = _productId;
    }
}

