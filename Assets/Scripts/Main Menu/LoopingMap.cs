using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class LoopingMapAuto : MonoBehaviour
{
    public Transform player;
    public Transform[] maps;

    [Header("Jarak setelah map tertinggal dari player")]
    public float triggerDistance = 2f;

    [Header("Tempelkan sedikit agar tidak ada celah")]
    public float overlap = 0.05f;

    void Start()
    {
        foreach (Transform map in maps)
        {
            Tilemap tilemap = GetMainTilemap(map);

            if (tilemap != null)
            {
                tilemap.CompressBounds();
            }
        }

        ArrangeMapsWithoutGap();
    }

    void Update()
    {
        foreach (Transform map in maps)
        {
            Bounds bounds = GetTilemapWorldBounds(map);

            if (player.position.x > bounds.max.x + triggerDistance)
            {
                MoveMapToFront(map);
            }
        }
    }

    void ArrangeMapsWithoutGap()
    {
        maps = maps.OrderBy(m => GetTilemapWorldBounds(m).min.x).ToArray();

        for (int i = 1; i < maps.Length; i++)
        {
            Bounds previousBounds = GetTilemapWorldBounds(maps[i - 1]);
            Bounds currentBounds = GetTilemapWorldBounds(maps[i]);

            float targetX = previousBounds.max.x - overlap;
            float difference = targetX - currentBounds.min.x;

            maps[i].position += new Vector3(difference, 0f, 0f);
        }
    }

    void MoveMapToFront(Transform map)
    {
        Transform frontMap = GetFrontMap(map);

        Bounds frontBounds = GetTilemapWorldBounds(frontMap);
        Bounds currentBounds = GetTilemapWorldBounds(map);

        float targetX = frontBounds.max.x - overlap;
        float difference = targetX - currentBounds.min.x;

        map.position += new Vector3(difference, 0f, 0f);
    }

    Transform GetFrontMap(Transform ignoredMap)
    {
        Transform frontMap = null;
        float farthestRight = float.MinValue;

        foreach (Transform map in maps)
        {
            if (map == ignoredMap)
                continue;

            Bounds bounds = GetTilemapWorldBounds(map);

            if (bounds.max.x > farthestRight)
            {
                farthestRight = bounds.max.x;
                frontMap = map;
            }
        }

        return frontMap;
    }

    Bounds GetTilemapWorldBounds(Transform map)
    {
        Tilemap tilemap = GetMainTilemap(map);

        if (tilemap == null)
        {
            Debug.LogWarning("Tilemap tidak ditemukan di dalam " + map.name);
            return new Bounds(map.position, Vector3.zero);
        }

        BoundsInt cellBounds = tilemap.cellBounds;

        Vector3 min = tilemap.CellToWorld(cellBounds.min);
        Vector3 max = tilemap.CellToWorld(cellBounds.max);

        Bounds bounds = new Bounds();
        bounds.SetMinMax(min, max);

        return bounds;
    }

    Tilemap GetMainTilemap(Transform map)
    {
        Tilemap[] tilemaps = map.GetComponentsInChildren<Tilemap>();

        foreach (Tilemap tilemap in tilemaps)
        {
            if (tilemap.gameObject.name.ToLower().Contains("tilemap"))
            {
                return tilemap;
            }
        }

        return tilemaps.Length > 0 ? tilemaps[0] : null;
    }
}