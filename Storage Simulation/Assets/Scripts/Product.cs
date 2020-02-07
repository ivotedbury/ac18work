using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Product : MonoBehaviour
{

    public int productId;
    public string productName;
    public string barcode;
    public string description;
    public string productImageUrl;
    public int unitOfMeasureId;
    public float defaultBuyingPrice = 0.0f;
    public float defaultSellingPrice = 0.0f;

    public void SetColour()
    {
        Color productColour;

        if (productId == Constants.PRODUCT_ID_RED)
        {
            productColour = Constants.COLOUR_RED;
        }
        else if (productId == Constants.PRODUCT_ID_RED_ORANGE)
        {
            productColour = Constants.COLOUR_RED_ORANGE;
        }
        else if (productId == Constants.PRODUCT_ID_ORANGE)
        {
            productColour = Constants.COLOUR_ORANGE;
        }
        else if (productId == Constants.PRODUCT_ID_ORANGE_YELLOW)
        {
            productColour = Constants.COLOUR_ORANGE_YELLOW;
        }
        else if (productId == Constants.PRODUCT_ID_YELLOW)
        {
            productColour = Constants.COLOUR_YELLOW;
        }
        else if (productId == Constants.PRODUCT_ID_YELLOW_GREEN)
        {
            productColour = Constants.COLOUR_YELLOW_GREEN;
        }
        else if (productId == Constants.PRODUCT_ID_GREEN)
        {
            productColour = Constants.COLOUR_GREEN;
        }
        else if (productId == Constants.PRODUCT_ID_GREEN_BLUE)
        {
            productColour = Constants.COLOUR_GREEN_BLUE;
        }
        else if (productId == Constants.PRODUCT_ID_BLUE)
        {
            productColour = Constants.COLOUR_BLUE;
        }
        else if (productId == Constants.PRODUCT_ID_BLUE_VIOLET)
        {
            productColour = Constants.COLOUR_BLUE_VIOLET;
        }
        else if (productId == Constants.PRODUCT_ID_VIOLET)
        {
            productColour = Constants.COLOUR_VIOLET;
        }
        else if (productId == Constants.PRODUCT_ID_VIOLET_RED)
        {
            productColour = Constants.COLOUR_VIOLET_RED;
        }
        else
        {
            productColour = Color.black;
        }

        GetComponent<Renderer>().material.color = productColour;
    }

}

