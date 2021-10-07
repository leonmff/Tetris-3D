using UnityEngine;

[CreateAssetMenu(fileName = "ColorConfig", menuName = "Grid/Color Config")]
public class SOColorConfig : ScriptableObject
{
    // Must maintain alphabetic order.
    public enum GridColor { Aqua, Blue, Green, Orange, Purple, Red, Yellow }

    [ColorUsage(true, true)]
    public Color Aqua;
    [ColorUsage(true, true)]
    public Color Blue;
    [ColorUsage(true, true)]
    public Color Green;
    [ColorUsage(true, true)]
    public Color Orange;
    [ColorUsage(true, true)]
    public Color Purple;
    [ColorUsage(true, true)]
    public Color Red;
    [ColorUsage(true, true)]
    public Color Yellow;

    [ColorUsage(true, true), Space(15)]
    public Color White;

    public Color GetColor(SOColorConfig.GridColor pColor)
    {
        Color t_color = Color.white;

        switch (pColor)
        {
            case SOColorConfig.GridColor.Aqua:
                t_color = Aqua;
                break;
            case SOColorConfig.GridColor.Blue:
                t_color = Blue;
                break;
            case SOColorConfig.GridColor.Green:
                t_color = Green;
                break;
            case SOColorConfig.GridColor.Orange:
                t_color = Orange;
                break;
            case SOColorConfig.GridColor.Purple:
                t_color = Purple;
                break;
            case SOColorConfig.GridColor.Red:
                t_color = Red;
                break;
            case SOColorConfig.GridColor.Yellow:
                t_color = Yellow;
                break;
        }

        return t_color;
    }
}
