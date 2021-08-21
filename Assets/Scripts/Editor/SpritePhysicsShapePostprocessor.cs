using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpritePhysicsShapeEditor
{
    //tatic Sprite copiedSprite;
    static public List<Vector2[]> copiedShapes = new List<Vector2[]>();
    static public List<string> destinationSprites = new List<string>();

    [MenuItem("Assets/Tools/Sprites/Copy Physics Shape", true)]
    static bool CheckSpriteMenu()
    {
        if (Selection.activeObject is Texture2D)
        {
            var importer =
                AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(Selection.activeObject)) as TextureImporter;

            if (importer.textureType == TextureImporterType.Sprite &&
                importer.spriteImportMode != SpriteImportMode.Multiple)
            {
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GetAssetPath(Selection.activeObject));

                if (sprite)
                {
                    return true;
                }
            }
        }
        else if (Selection.activeObject is Sprite)
        {
            return true;
        }

        return false;
    }

    [MenuItem("Assets/Tools/Sprites/Paste Physics Shape", true)]
    static bool CheckSpritesToModifyMenu()
    {
        if (copiedShapes.Count > 0)
        {
            var sprites = GetSpritesFromSelection();

            if (sprites.Count > 0)
            {
                return true;
            }
        }

        return false;
    }

    [MenuItem("Assets/Tools/Sprites/Copy Physics Shape", false)]
    static void CopySpritePhysicsShape()
    {
        var sprite = Selection.activeObject as Sprite;

        if (!sprite && Selection.activeObject is Texture2D)
        {
            sprite = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GetAssetPath(Selection.activeObject));
        }

        copiedShapes.Clear();

        TextureImporter importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(sprite)) as TextureImporter;

        copiedShapes = GetPhysicsShape(importer, sprite.name);
        return;
    }

    [MenuItem("Assets/Tools/Sprites/Paste Physics Shape", false)]
    static void PasteSpritePhysicsShape()
    {
        List<Sprite> sprites = GetSpritesFromSelection();

        if (sprites.Count > 0)
        {
            //Undo.RecordObjects(sprites.ToArray(), "Paste sprites physics shape");

            for (int i = 0; i < sprites.Count; i++)
            {
                var path = AssetDatabase.GetAssetPath(sprites[i]);
                destinationSprites.Add(sprites[i].name);

                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                SetPhysicsShape(importer, sprites[i].name, copiedShapes);

                AssetDatabase.ForceReserializeAssets(new string[] {importer.assetPath},
                    ForceReserializeAssetsOptions.ReserializeMetadata);
                importer.SaveAndReimport();
            }
        }
    }
    
    [MenuItem("Assets/Tools/Sprites/Clear Physics Shape", false)]
    static void ClearSpritePhysicsShape()
    {
        List<Sprite> sprites = GetSpritesFromSelection();

        if (sprites.Count > 0)
        {
            //Undo.RecordObjects(sprites.ToArray(), "Paste sprites physics shape");

            for (int i = 0; i < sprites.Count; i++)
            {
                var path = AssetDatabase.GetAssetPath(sprites[i]);
                destinationSprites.Add(sprites[i].name);

                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                var physicsShapeProperty = GetPhysicsShapeProperty(importer, sprites[i].name);
                physicsShapeProperty.ClearArray();
                physicsShapeProperty.serializedObject.ApplyModifiedPropertiesWithoutUndo();

                AssetDatabase.ForceReserializeAssets(new string[] {importer.assetPath},
                    ForceReserializeAssetsOptions.ReserializeMetadata);
                importer.SaveAndReimport();
            }
        }
    }

    public static List<Sprite> GetSpritesFromSelection()
    {
        List<Sprite> sprites = new List<Sprite>(Selection.GetFiltered<Sprite>(SelectionMode.Unfiltered));
        destinationSprites.Clear();

        var textures = Selection.GetFiltered<Texture2D>(SelectionMode.Unfiltered);

        if (textures.Length > 0)
        {
            for (int i = 0; i < textures.Length; i++)
            {
                var importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(textures[i])) as TextureImporter;

                if (importer.textureType == TextureImporterType.Sprite &&
                    importer.spriteImportMode != SpriteImportMode.Multiple)
                {
                    var objects = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(textures[i]));

                    for (int k = 0; k < objects.Length; k++)
                    {
                        if (objects[k] is Sprite)
                        {
                            sprites.Add(objects[k] as Sprite);
                        }
                    }
                }
            }
        }

        return sprites;
    }

    public static void SetPhysicsShape(TextureImporter importer, string spriteName, List<Vector2[]> data)
    {
        var physicsShapeProperty = GetPhysicsShapeProperty(importer, spriteName);
        physicsShapeProperty.ClearArray();

        for (int j = 0; j < data.Count; ++j)
        {
            physicsShapeProperty.InsertArrayElementAtIndex(j);
            var o = data[j];
            SerializedProperty outlinePathSP = physicsShapeProperty.GetArrayElementAtIndex(j);
            outlinePathSP.ClearArray();

            for (int k = 0; k < o.Length; ++k)
            {
                outlinePathSP.InsertArrayElementAtIndex(k);
                outlinePathSP.GetArrayElementAtIndex(k).vector2Value = o[k];
            }
        }

        physicsShapeProperty.serializedObject.ApplyModifiedPropertiesWithoutUndo();
    }

    public static List<Vector2[]> GetPhysicsShape(TextureImporter importer, string spriteName)
    {
        var physicsShapeProperty = GetPhysicsShapeProperty(importer, spriteName);
        var physicsShape = new List<Vector2[]>();

        for (int j = 0; j < physicsShapeProperty.arraySize; ++j)
        {
            SerializedProperty physicsShapePathSP = physicsShapeProperty.GetArrayElementAtIndex(j);
            var o = new Vector2[physicsShapePathSP.arraySize];

            for (int k = 0; k < physicsShapePathSP.arraySize; ++k)
            {
                o[k] = physicsShapePathSP.GetArrayElementAtIndex(k).vector2Value;
            }

            physicsShape.Add(o);
        }

        return physicsShape;
    }

    private static SerializedProperty GetPhysicsShapeProperty(TextureImporter importer, string spriteName)
    {
        SerializedObject serializedImporter = new SerializedObject(importer);

        if (importer.spriteImportMode == SpriteImportMode.Multiple)
        {
            var spriteSheetSP = serializedImporter.FindProperty("m_SpriteSheet.m_Sprites");

            for (int i = 0; i < spriteSheetSP.arraySize; i++)
            {
                if (importer.spritesheet[i].name == spriteName)
                {
                    var element = spriteSheetSP.GetArrayElementAtIndex(i);

                    return element.FindPropertyRelative("m_PhysicsShape");
                }
            }
        }

        return serializedImporter.FindProperty("m_SpriteSheet.m_PhysicsShape");
    }
}