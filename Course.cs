using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class Course : MonoBehaviour {
    public int Speed = 1;

    public UnityEngine.UI.InputField FileField;
    public UnityEngine.UI.Button LoadFileButton;
    public UnityEngine.UI.Text FileStatusText;
    public GameObject TrackPrefab;
    public GameObject CartPrefab;
    public Sprite[] TrackSprites;
    public HUD HUD;

    bool CoroutineRunning = false;
    bool InitilizationFinished = false;
    private bool Paused = true;
    private bool FileLoaded = false;

    List<Cart> Carts;
    Dictionary<Vector2Int, Track> Tracks;

    public int Ticks { get; private set; }

    // Use this for initialization
    void Start () {
        Ticks = 0;
        Carts = new List<Cart>();
        Tracks = new Dictionary<Vector2Int, Track>();
        LoadFileButton.onClick.RemoveAllListeners();
        LoadFileButton.onClick.AddListener(LoadFile);
	}

    private void LoadFile()
    {
        List<KeyValuePair<Vector2Int, string>> turns = new List<KeyValuePair<Vector2Int, string>>();

        string filePath = System.IO.Path.GetFullPath(FileField.text);

        if (System.IO.File.Exists(filePath))
        {
            using (System.IO.StreamReader reader = new System.IO.StreamReader(filePath))
            {
                string line = null;
                int y = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    for (int x = 0; x < line.Length; x++)
                    {
                        string s = line.Substring(x, 1);
                        ProcessMapToken(s, new Vector2Int(x, y), turns);
                    }

                    y++;
                }
            }

            foreach (KeyValuePair<Vector2Int, string> turn in turns)
            {
                ProcessTurn(turn.Key, turn.Value);
            }

            FileLoaded = true;
        }
        else
        {
            FileStatusText.text = string.Format("{0} not found.", filePath);
        }
    }

    private void ProcessMapToken(string token, Vector2Int coordinate, List<KeyValuePair<Vector2Int,string>> turns)
    {
        Track track = null;

        switch (token)
        {
            case "/":
            case "\\":
                turns.Add(new KeyValuePair<Vector2Int, string>(coordinate, token));
                return;
            case "-":
                track = CreateTrack(coordinate, Track.TrackTypes.Horizontal);
                break;
            case "|":
                track = CreateTrack(coordinate, Track.TrackTypes.Vertical);
                break;
            case "^":
                track = CreateTrack(coordinate, Track.TrackTypes.Vertical);
                Carts.Add(CreateCart(track, Direction.Up));
                break;
            case "v":
                track = CreateTrack(coordinate, Track.TrackTypes.Vertical);
                Carts.Add(CreateCart(track, Direction.Down));
                break;
            case "<":
                track = CreateTrack(coordinate, Track.TrackTypes.Horizontal);
                Carts.Add(CreateCart(track, Direction.Left));
                break;
            case ">":
                track = CreateTrack(coordinate, Track.TrackTypes.Horizontal);
                Carts.Add(CreateCart(track, Direction.Right));
                break;
            case "+":
                track = CreateTrack(coordinate, Track.TrackTypes.Intersection);
                break;
            case " ":
                return;
        }

        Tracks.Add(coordinate, track);
    }

    private void ProcessTurn(Vector2Int coordinate, string token)
    {
        Track track = null;
        Track source;

        switch (token)
        {
            case "/":
                if (Tracks.TryGetValue(coordinate + Vector2Int.left, out source) && Track.IsSource(coordinate, source))
                {
                    track = CreateTrack(coordinate, Track.TrackTypes.ULTurn);
                }
                else
                {
                    track = CreateTrack(coordinate, Track.TrackTypes.RDTurn);
                }
                break;
            case "\\":
                if (Tracks.TryGetValue(coordinate + Vector2Int.right, out source) && Track.IsSource(coordinate, source))
                {
                    track = CreateTrack(coordinate, Track.TrackTypes.URTurn);
                }
                else
                {
                    track = CreateTrack(coordinate, Track.TrackTypes.DLTurn);
                }
                break;
        }

        Tracks.Add(coordinate, track);
    }
	
    private Track CreateTrack(Vector2Int coordinate, Track.TrackTypes type)
    {
        GameObject newTrack = Instantiate(TrackPrefab);
        Track track = newTrack.GetComponent<Track>();
        track.Initialize(coordinate, type, TrackSprites[(int)type]);

        return track;
    }

    private Cart CreateCart(Track position, int direction)
    {
        GameObject cart = Instantiate(CartPrefab);
        cart.GetComponent<Cart>().Initilize(position, direction, Tracks, HUD);
        return cart.GetComponent<Cart>();
    }

    // Update is called once per frame
    void Update ()
    {
        if (!InitilizationFinished)
        {
            HUD.SetRemainingCartText(Cart.GetNumberLivingCarts(Carts));
        }

        if (Input.GetKeyDown(KeyCode.KeypadPlus) || Input.GetKeyDown(KeyCode.Plus))
        {
            HUD.UpdateSpeedText(++Speed);
        }
        else if (Input.GetKeyDown(KeyCode.KeypadMinus) || Input.GetKeyDown(KeyCode.Minus))
        {
            HUD.UpdateSpeedText(--Speed);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Paused = Paused ? false : true;
        }

        

        if (!Paused && FileLoaded)
        {
            int ticksFinished = Speed;

            for (int i = 0; i < Speed; i++)
            {
                bool cartDied = false;

                foreach (Cart c in Carts.OrderBy((c) => c.Position.Coordinate.y).ThenBy((c) => c.Position.Coordinate.x))
                {
                    if (c.IsAlive)
                    {
                        c.Advance();

                        if (!c.IsAlive)
                        {
                            HUD.SetRemainingCartText(Cart.GetNumberLivingCarts(Carts));
                            cartDied = true;
                        }
                    }
                }

                if (cartDied && Cart.GetNumberLivingCarts(Carts) == 1)
                {
                    Vector2Int coordinate = Carts.First((cc) => cc.IsAlive).Position.Coordinate;
                    HUD.UpdateLastCartText(coordinate);
                    Paused = true;
                    ticksFinished = i;
                    break;
                }
            }

            Ticks += ticksFinished;
            HUD.SetTicksText(Ticks);
        }
	}

    private IEnumerator UpdateCart()
    {
        CoroutineRunning = true;

        foreach (Cart c in Carts.OrderBy((c) => c.Position.Coordinate.y).ThenBy((c) => c.Position.Coordinate.x))
        {
            if (c.IsAlive)
            {
                c.Advance();

                if (!c.IsAlive)
                {
                    Debug.Log(string.Format("{0} carts remaining.", Cart.GetNumberLivingCarts(Carts)));
                }
            }

            yield return new WaitForSeconds(Speed);
        }

        CoroutineRunning = false;
    }
}
