using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tote : MonoBehaviour
{

    public Product referenceProduct;
    public List<Product> toteProducts = new List<Product>();

    public Properties properties;

    public int number;



    public void PopulateItems(int _productId, int _quantity)
    {
        for (int i = 0; i < _quantity; i++)
        {
            Product newProduct = Instantiate(referenceProduct, this.transform.position + GetProductPosition(i), Quaternion.identity, transform);
            newProduct.productId = _productId;
            newProduct.SetColour();
            newProduct.name = "ProductID: " + _productId.ToString();
            toteProducts.Add(newProduct);
        }
    }

    Vector3 GetProductPosition(int _productNumber)
    {
        Vector3 productPosition = new Vector3(0, 0, 0);

        Vector3[] productStackingPositions = new Vector3[Constants.PRODUCT_X * Constants.PRODUCT_Y * Constants.PRODUCT_Z];

        int i = 0;

        for (int y = 0; y < Constants.PRODUCT_Y; y++)
        {
            for (int z = 0; z < Constants.PRODUCT_Z; z++)
            {
                for (int x = 0; x < Constants.PRODUCT_X; x++)
                {
                    productStackingPositions[i] = new Vector3((x * Constants.PRODUCT_SPACING) + Constants.TOTE_OFFSET_X, (y * Constants.PRODUCT_SPACING) + Constants.TOTE_OFFSET_HEIGHT, (z * Constants.PRODUCT_SPACING) + Constants.TOTE_OFFSET_Z);
                    i++;
                }
            }
        }

        productPosition = productStackingPositions[_productNumber];

        return productPosition;
    }

    void OnMouseOver()
    {
        GetComponent<Renderer>().material.color = Constants.TOTE_SELECTED;
    }

    private void OnMouseDown()
    {
        PopulateProperties();
    }

    void OnMouseExit()
    {
        GetComponent<Renderer>().material.color = Constants.TOTE_NORMAL;
    }

    void PopulateProperties()
    {
        properties.type = "Tote";
        properties.number = number;
        properties.Publish();
    }

    void ClearProperties()
    {
        properties.ClearProperties();
    }

}
