using System.Collections.Generic;
using UnityEngine;

public class Cart : MonoBehaviour {

    static HashSet<Color> UsedColors = new HashSet<Color>();

    public int HighlightTime;

    private int CurrentDirection;
    
    private Turn IntersectionTurn = Turn.Left;
    private Dictionary<Vector2Int, Track> Tracks;
    private HUD HUD;

    public Track Position { get; private set; }
    public bool IsAlive { get; private set; }

    public Vector2Int Coordinate
    {
        get
        {
            return Position.Coordinate;
        }
    }

    public static int GetNumberLivingCarts(List<Cart> carts)
    {
        int sum = 0;

        foreach (Cart c in carts)
        {
            if (c.IsAlive)
            {
                sum++;
            }
        }

        return sum;
    }

    public void Advance()
    {
        List<int> directions = Position.Directions;

        Track nextTrack;
        if (directions.Count == 4)
        {
            TurnDirection();
        }
        else
        {
            foreach (int direction in directions)
            {
                if (Direction.GetOppositeDirection(CurrentDirection) != direction)
                {
                    CurrentDirection = direction;
                    break;
                }
            }
        }
        nextTrack = Tracks[Position.Coordinate + Direction.GetVector(CurrentDirection)];
        Move(nextTrack);
    }

    private void Collided()
    {
        Position.Cart = null;
        IsAlive = false;
        //code to remove sprite.
    }

    private void Place(Track position, int direction)
    {
        Position = position;
        CurrentDirection = direction;
        transform.position = new Vector3(position.Coordinate.x, -position.Coordinate.y, -1);
    }

    public void Initilize(Track position, int direction, Dictionary<Vector2Int, Track> tracks, HUD hud)
    {
        Place(position, direction);
        Tracks = tracks;
        HUD = hud;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Color color;
        do
        {
            color = new Color(Random.value, Random.value, Random.value);
        } while (UsedColors.Contains(color));

        UsedColors.Add(color);
        spriteRenderer.color = color;
    }

    private void TurnDirection()
    {
        switch (IntersectionTurn)
        {
            case Turn.Left:
                CurrentDirection = Direction.TurnLeft(CurrentDirection);
                IntersectionTurn = Turn.Straight;
                break;
            case Turn.Straight:
                IntersectionTurn = Turn.Right;
                break;
            case Turn.Right:
                CurrentDirection = Direction.TurnRight(CurrentDirection);
                IntersectionTurn = Turn.Left;
                break;
        }
    }

    private void Move(Track nextTrack)
    {
        if (nextTrack.HasCart)
        {
            HUD.UpdateFirstCollisionText(nextTrack.Cart.Position.Coordinate);
            nextTrack.Cart.Collided();
            Collided();
        }
        else
        {
            Position.Cart = null;
            Position = nextTrack;
            Position.Cart = this;
            transform.position = new Vector3(Position.Coordinate.x, -Position.Coordinate.y, -1);
            StartCoroutine(Position.Highlight(GetComponent<SpriteRenderer>().color, HighlightTime));
        }
    }

    void Start ()
    {
        IsAlive = true;
	}

    public enum Turn
    {
        Left, Straight, Right
    }
}
