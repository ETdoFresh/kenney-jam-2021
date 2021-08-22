using UnityEngine;
using UnityEngine.Tilemaps;

public class SlopeGenerator : MonoBehaviour
{
    public GameObject slopePrefab;
    public string keyword = "slope";
    public bool enableSpriteRenderer;
}

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(SlopeGenerator))]
class SlopeGeneratorEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var slopeGenerator = (SlopeGenerator) target;
        var buttonText = slopeGenerator.transform.childCount == 0 ? "Generate Slopes" : "Clear and Generator Slopes";

        if (GUILayout.Button(buttonText))
        {
            for (var i = slopeGenerator.transform.childCount - 1; i >= 0; i--)
                DestroyImmediate(slopeGenerator.transform.GetChild(i).gameObject);

            foreach (var tilemap in FindObjectsOfType<Tilemap>())
            {
                for (var x = tilemap.cellBounds.xMin; x <= tilemap.cellBounds.xMax; x++)
                for (var y = tilemap.cellBounds.yMin; y <= tilemap.cellBounds.yMax; y++)
                for (var z = tilemap.cellBounds.zMin; z <= tilemap.cellBounds.zMax; z++)
                {
                    var tilePosition = new Vector3Int(x, y, z);
                    var tile = tilemap.GetTile<Tile>(tilePosition);
                    if (tile)
                        if (tile.name.Contains(slopeGenerator.keyword))
                        {
                            var slopeGameObject = Instantiate(slopeGenerator.slopePrefab);
                            slopeGameObject.name = $"{tile.name}_{x}_{y}_{z}";

                            var slopeTransform = slopeGameObject.transform;
                            slopeTransform.SetParent(slopeGenerator.transform);

                            var slopeTile = slopeGameObject.GetComponent<SlopeTile>();
                            slopeTile.SetRotation(tile);
                            slopeTile.SetPosition(tilePosition, tilemap);

                            var slopeRenderer = slopeGameObject.GetComponentInChildren<SpriteRenderer>();
                            slopeRenderer.color = new Color(0, 0.5f, 0, 1);
                            slopeRenderer.enabled = slopeGenerator.enableSpriteRenderer;
                        }
                }
            }
        }
    }
}

#endif