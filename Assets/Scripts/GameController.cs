using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class GameController : MonoBehaviour
{
    public UnityEvent Beat;
    public float startBpm;
    public bool beatAction;

    private float wait;

    private float lastTick;

    private Coroutine beatCoroutine;
    [SerializeField] private float leniency;

    [SerializeField] private int WIDTH;
    [SerializeField] private int HEIGHT;
    [SerializeField] private Sprite tileSprite;
    private Tile[,] tiles;

    private void Start()
    {
        beatCoroutine = StartCoroutine(BeatCoroutine(startBpm));
        tiles = new Tile[WIDTH,HEIGHT];
        //initializePositions
        GameObject tileParent = new GameObject("TileParent");
        //tileParent.transform.position = new Vector3(0f, 0f, .1f);
        float xScale = 88f / WIDTH;
        float yScale = 50f / HEIGHT;
        for (int x = 0; x < WIDTH; ++x)
        {
            for (int y = 0; y < HEIGHT; ++y)
            {
                GameObject newTile = new GameObject("Tile" + x + "," + y);
                tiles[x, y] = newTile.AddComponent<Tile>();
                newTile.transform.parent = tileParent.transform;
                newTile.transform.localPosition = new Vector3(-22f + ((2f * x) / (WIDTH * 2f)) * 44f, -12.5f + ((2f * y) / (HEIGHT * 2f)) * 25f);
                newTile.transform.localScale = new Vector3(xScale, yScale);
                newTile.AddComponent<SpriteRenderer>().color = ((x + y) % 2 == 0) ? new Color(.8f, .7f, .7f) : new Color(.6f, .3f, 0f);
                newTile.GetComponent<SpriteRenderer>().sprite = tileSprite;
                newTile.GetComponent<SpriteRenderer>().sortingLayerID = SortingLayer.NameToID("Tiles");
            }
        }
        foreach (Entity entity in Resources.FindObjectsOfTypeAll(typeof(Entity)))
        {
            entity.SetPositionInit(tiles[entity.xPosition, entity.yPosition]);
        }
        foreach (Item item in Resources.FindObjectsOfTypeAll(typeof(Item))) {
            item.SetPositionInit(tiles[item.xPosition, item.yPosition]);
        }
    }

    public void UpdateBPM(float bpm)
    {
        if (beatCoroutine != null)
        {
            StopCoroutine(beatCoroutine);
        }
        beatCoroutine = StartCoroutine(BeatCoroutine(bpm));
    }

    public IEnumerator BeatCoroutine(float bpm)
    {
        wait = 60f / bpm;
        FindObjectOfType<MovingMetronome>().StartMetronome(wait);

        while (true)
        {
            yield return new WaitForSeconds(wait);
            lastTick = Time.time;
            Beat.Invoke();
            beatAction = false;
        }
    }

    public bool CheckInput()
    {
        if (!beatAction && (Mathf.Abs(lastTick - Time.time) < leniency || Mathf.Abs(lastTick + wait - Time.time) < leniency))
        {
            beatAction = true;
            return true;
        }

        return false;
    }

    public List<Tile> CheckCircle(int startX, int startY, int max)
    {
        List<Tile> returnList = new List<Tile>();
        CheckQuarterRecursive(startX, startY, 1, 1, 0, max, returnList);
        CheckQuarterRecursive(startX, startY, -1, 1, 0, max, returnList);
        CheckQuarterRecursive(startX, startY, 1, -1, 0, max, returnList);
        CheckQuarterRecursive(startX, startY, -1, -1, 0, max, returnList);
        return returnList;
    }
    
    void CheckQuarterRecursive(int startX, int startY, int x, int y, int t, int m, List<Tile> r)
    {
        if (t > m || startX >= WIDTH || startY >= HEIGHT || startX < 0 || startY < 0)
        {
            return;
        }
        if (!r.Contains(tiles[startX,startY]))
        {
            r.Add(tiles[startX, startY]);
        }
        CheckQuarterRecursive(startX + x, startY, x, y, t + 1, m, r);
        CheckQuarterRecursive(startX, startY + y, x, y, t + 1, m, r);
    }

    public Tile GetTile(int x, int y)
    {
        if (x >= WIDTH || y >= HEIGHT || x < 0 || y < 0)
        {
            return null;
        }
        return tiles[x,y];
    }
}
