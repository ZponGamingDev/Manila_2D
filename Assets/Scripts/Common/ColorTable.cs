using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class ColorTable
{

    #region Player color
    static Color c_DARK_RED = new Color(139.0f / 255.0f, 0.0f, 0.0f, 1.0f);
    static Color c_STEEL_BLUE = new Color(70.0f / 255.0f, 130.0f / 255.0f, 180.0f / 255.0f, 1.0f);
    static Color c_DARK_OLIVE_GREEN = new Color(85.0f / 255.0f, 107.0f / 255.0f, 47.0f / 255.0f, 1.0f);
    static Color c_DARK_GOLDEN_ROD = new Color(184.0f / 255.0f, 134.0f / 255.0f, 11.0f / 255.0f, 1.0f);

    public static Color GeneratePlayerColor(int iColor)
    {
        switch (iColor)
        {
            case 0:
                return c_DARK_RED;
            //break;
            case 1:
                return c_STEEL_BLUE;
            //break;
            case 2:
                return c_DARK_OLIVE_GREEN;
            //break;
            case 3:
                return c_DARK_GOLDEN_ROD;
                //break;
        }

        return Color.black;
    }
    #endregion

    #region Boat table & Stock transition highlight color

    public static Color c_ORANGE_RED = new Color(1.0f, 69.0f / 255.0f, 0.0f, 1.0f);

    #endregion

    #region Good color
    static public Color c_TOMATO = new Color(1.0f, 99.0f / 255.0f, 71.0f / 255.0f, 1.0f);
    static public Color c_SILK = new Color(0.0f, 199.0f / 255.0f, 1.0f, 1.0f);
    static public Color c_PADDY = new Color(246.0f / 255.0f, 242.0f / 255.0f, 197.0f / 255.0f, 1.0f);
    static public Color c_JADE = new Color(0.0f, 168.0f / 255.0f, 107.0f / 255.0f, 1.0f);
    #endregion
}
