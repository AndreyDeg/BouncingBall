using UnityEngine;

public class GroundData
{
    public Vector3 Position;
    public int? NumberLenght;
    public int WidthX, WidthZ;

    public GroundData(float x, float y, float z, int? numberLenght, int widthX, int widthZ)
    {
        Position = new Vector3(x, y, z);
        NumberLenght = numberLenght;
        WidthX = widthX;
        WidthZ = widthZ;
    }
}