using UnityEngine;

[CreateAssetMenu(menuName = "Create MapRotation", fileName = "MapRotation", order = 0)]
public class MapDirection : ScriptableObject
{
    public IsoDirection direction;
}