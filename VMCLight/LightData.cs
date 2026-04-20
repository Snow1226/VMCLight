using UnityEngine;

namespace VMCLight;

public class LightData
{
    public LightTypeEnum LigthType;
    public Color Color;
    public Vector3 Position;
    public Quaternion Rotation;
}

public enum LightTypeEnum
{
    Directional,
    Spot,
    LeftSaber,
    RightSaber,
}