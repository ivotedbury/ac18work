using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureElement
{
    int elementType; // staircaseUp, staircaseDown, corridor, slot, accessPoint
    int orientation;
    bool clockwise;
    int depth;

    public StructureElement(int _elementType, Node[,,] _nodeArray, Node _origin, int _orientation, bool _clockwise, int _depth)
    {
        elementType = _elementType;
        orientation = _orientation;
        clockwise = _clockwise;
        depth = depth;

        if (elementType == Constants.STRUCTURE_CORRIDOR)
        {
            _origin.corridor = true;
        }
    }
}



