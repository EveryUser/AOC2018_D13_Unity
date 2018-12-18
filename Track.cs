using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Track : MonoBehaviour {
    public Vector2Int Coordinate { get; private set; }
    public List<int> Directions { get; private set; }
    public Cart Cart {get; set;}

    public bool HasCart
    {
        get
        {
            return Cart != null;
        }
    }

    public static bool IsSource(Vector2Int coordinate, Track track)
    {
        Vector2 distance = track.Coordinate - coordinate;

        if (System.Math.Abs(distance.x) + System.Math.Abs(distance.y) > 1)
        {
            return false;
        }

        foreach (int direction in track.Directions)
        {
            switch(direction)
            {
                case Direction.Up:
                    if (track.Coordinate - Vector2Int.up == coordinate)
                    {
                        return true;
                    }
                    break;
                case Direction.Right:
                    if (track.Coordinate + Vector2Int.right == coordinate)
                    {
                        return true;
                    }
                    break;
                case Direction.Down:
                    if (track.Coordinate - Vector2Int.down == coordinate)
                    {
                        return true;
                    }
                    break;
                case Direction.Left:
                    if (track.Coordinate + Vector2Int.left == coordinate)
                    {
                        return true;
                    }
                    break;
            }
        }

        return false;
    }

    public IEnumerator Highlight(Color color, int fadeSpeed)
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        Color originalColor = renderer.color;
        float rStep = (originalColor.r - color.r) / fadeSpeed;
        float bStep = (originalColor.b - color.b) / fadeSpeed;
        float gStep = (originalColor.g - color.g) / fadeSpeed;
        renderer.color = color;

        //yield return null;

        for (int i = 0; i < fadeSpeed; i++)
        {
            Color current = renderer.color;
            current = new Color(current.r + rStep, current.g + gStep, current.b + bStep);
            renderer.color = current;
            yield return null;
        }
    }

    public void Initialize(Vector2Int coordinate, TrackTypes type, Sprite sprite)
    {
        Coordinate = coordinate;
        
        switch (type)
        {
            case TrackTypes.Intersection:
                Directions = new List<int>(new int[]{ Direction.Right, Direction.Left, Direction.Up, Direction.Down });
                break;
            case TrackTypes.Horizontal:
                Directions = new List<int>(new int[] { Direction.Right, Direction.Left });
                break;
            case TrackTypes.Vertical:
                Directions = new List<int>(new int[] { Direction.Up, Direction.Down });
                break;
            case TrackTypes.URTurn:
                Directions = new List<int>(new int[] { Direction.Right, Direction.Up, });
                break;
            case TrackTypes.RDTurn:
                Directions = new List<int>(new int[] { Direction.Right, Direction.Down });
                break;
            case TrackTypes.DLTurn:
                Directions = new List<int>(new int[] { Direction.Left, Direction.Down });
                break;
            case TrackTypes.ULTurn:
                Directions = new List<int>(new int[] { Direction.Left, Direction.Up });
                break;
            default:
                throw new System.ArgumentException(string.Format("type is invalid; {0}", type));
        }

        GetComponent<SpriteRenderer>().sprite = sprite;
        transform.position = new Vector3(coordinate.x, -coordinate.y, 0);
    }

    // Use this for initialization
    void Start ()
    {
        Cart = null;
	}

    public enum TrackTypes
    {
        Intersection, Horizontal, Vertical, URTurn, RDTurn, DLTurn, ULTurn
    }
}
