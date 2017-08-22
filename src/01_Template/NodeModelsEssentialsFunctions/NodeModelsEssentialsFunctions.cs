﻿using Autodesk.DesignScript.Runtime; // needed to add flags like [IsVisibleDynamoLibrary(false)]

namespace NodeModelsEssentials.Functions
{
    [IsVisibleInDynamoLibrary(false)]

    public class NodeModelsEssentialsFunctions
    {
        public static double Multiply(double a, double b)
        {
            return a * b;
        }
    }
}
