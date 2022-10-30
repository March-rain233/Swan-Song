using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
public class Test : SerializedMonoBehaviour
{
    public List<GameObject> gameObjects = new List<GameObject>();
    public List<(float, Color)> colors = new();
    public Grid grid;
    public Sprite tile;
    [Button]
    public void TestFunc(int w, int h, float frequence, bool isRandom)
    {
        foreach (GameObject go in gameObjects)
        {
            DestroyImmediate(go, true);
        }
        gameObjects.Clear();

        Vector2 offset = Vector2.zero;
        if (isRandom)
        {
            offset = new Vector2(Random.Range(0, 255f), Random.Range(0, 255f));
        }
        var map = MapFactory.GenerateNoiseMap(w, h, frequence, 2, offset);
        string a = "";
        for(int i = 0; i < h; i++)
        {
            for(int j = 0; j < w; j++)
            {
                var obj = new GameObject("",typeof(SpriteRenderer));
                obj.transform.position = grid.CellToWorld(new Vector3Int(i, j, 0));
                obj.GetComponent<SpriteRenderer>().sprite = tile;
                gameObjects.Add(obj);
                a += $"{map[i, j]} ";

                var v = map[i, j];
                foreach(var p in colors)
                {
                    if(v <= p.Item1)
                    {
                        obj.GetComponent<SpriteRenderer>().color = p.Item2;
                        break;
                    }
                }
            }
            a += "\n";
        }
        Debug.Log(a);
    }
}
