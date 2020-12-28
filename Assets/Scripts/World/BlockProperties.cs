using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct BlockProperties
{
    public string Name;
    public int Weight;
    public BlockType Blocktype;
    public Vector2[] TexturesUV;

    public BlockProperties(BlockType blocktype, string name, int weight, Vector2[] texturesUV)
    {
        Blocktype = blocktype;
        Name = name;
        Weight = weight;
        TexturesUV = texturesUV;
    }

    public Vector2 GetTexture(Orientation o)
    {
        switch(o)
        {
            case Orientation.Up:
                return TexturesUV[0];

            case Orientation.Down:
                return TexturesUV[1];

            case Orientation.Forward:
                return TexturesUV[2];

            case Orientation.Backward:
                return TexturesUV[3];

            case Orientation.Left:
                return TexturesUV[4];

            case Orientation.Right:
                return TexturesUV[5];
        }

        return new Vector2();
    }

    public override string ToString()
    {
        return string.Format("{0} {1} {2}", Name, Weight, TexturesUV.ToString());
    }
}

public class BlockPropertiesList
{
    public List<BlockProperties> properties = new List<BlockProperties>();

    public BlockPropertiesList(List<BlockProperties> properties)
    {
        this.properties = properties;
    }
}
