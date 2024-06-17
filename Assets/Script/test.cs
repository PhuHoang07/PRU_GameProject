using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class test : MonoBehaviour
{
    public Tilemap[] tilemaps;
    public TileBase mushroomTile;
    public int numberOfMushrooms;

    void Start()
    {
        if (tilemaps.Length > 0)
        {
            if (CheckAvailableSpace(tilemaps[0], numberOfMushrooms))
            {
                PlaceMushroomsAtRandomPositions(tilemaps, numberOfMushrooms);
            }
            else
            {
                PlaceMushroomsAtRandomPositions(tilemaps, GetAvailableSpaceCount(tilemaps[0]));
            }
        }
        else
        {
            Debug.LogWarning("No Tilemap assigned in the array.");
        }
    }

    bool CheckAvailableSpace(Tilemap tilemap, int count)
    {
        int availableSpaceCount = GetAvailableSpaceCount(tilemap);
        return availableSpaceCount >= count;
    }

    int GetAvailableSpaceCount(Tilemap tilemap)
    {
        BoundsInt bounds = tilemap.cellBounds;
        int availableSpaceCount = 0;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);
                if (tilemap.GetTile(position) == null)
                {
                    availableSpaceCount++;
                }
            }
        }

        return availableSpaceCount;
    }

    void PlaceMushroomsAtRandomPositions(Tilemap[] tilemaps, int count)
    {
        BoundsInt bounds = tilemaps[0].cellBounds; // Use the bounds of the first Tilemap

        List<Vector3Int> availablePositions = new List<Vector3Int>();

        foreach (Tilemap tilemap in tilemaps)
        {
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                for (int y = bounds.yMin; y < bounds.yMax; y++)
                {
                    Vector3Int position = new Vector3Int(x, y, 0);
                    if (tilemap.GetTile(position) == null)
                    {
                        bool positionAvailable = true;
                        // Check if the position is available in other Tilemaps
                        foreach (Tilemap otherTilemap in tilemaps)
                        {
                            if (otherTilemap != tilemap && otherTilemap.GetTile(position) != null)
                            {
                                positionAvailable = false;
                                break;
                            }
                        }
                        if (positionAvailable)
                        {
                            availablePositions.Add(position);
                        }
                    }
                }
            }
        }

        for (int i = 0; i < count && availablePositions.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, availablePositions.Count);
            Vector3Int randomPosition = availablePositions[randomIndex];
            tilemaps[0].SetTile(randomPosition, mushroomTile);
            availablePositions.RemoveAt(randomIndex);
        }
    }
}
