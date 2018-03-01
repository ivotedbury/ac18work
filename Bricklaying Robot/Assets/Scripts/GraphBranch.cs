using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphBranch
{

   public Cell start;
   public Cell end;

   public int type;

    public GraphBranch(Cell _start, Cell _end, int _type)
    {
        start = _start;
        end = _end;
        type = _type;
    }

    public void SetGraphBranchType()
    {
    }
}
