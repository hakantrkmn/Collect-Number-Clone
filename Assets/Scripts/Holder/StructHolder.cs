using UnityEngine;

[System.Serializable]
public struct FixedValue
{
    public int row;
    public int col;
    public int value;

    public FixedValue(int r, int c, int v)
    {
        row = r;
        col = c;
        value = v;
    }
}

[System.Serializable]
public struct Number
{
    public int number;
    public Color color;
}

[System.Serializable]
public class Target
{
    public int number;
    public int amount;
}

