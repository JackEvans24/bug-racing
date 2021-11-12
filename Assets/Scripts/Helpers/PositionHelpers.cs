using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PositionHelpers
{
    public static string GetPositionText(int position) => $"{position}{GetPositionOrdinal(position)}";

    private static string GetPositionOrdinal(int position)
    {
        switch (position)
        {
            case 1:
                return "st";
            case 2:
                return "nd";
            case 3:
                return "rd";
            default:
                return "th";
        }
    }
}
