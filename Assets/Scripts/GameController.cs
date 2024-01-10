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
    private Tile[,] tiles;

    private void Start()
    {
        beatCoroutine = StartCoroutine(BeatCoroutine(startBpm));
        tiles = new Tile[WIDTH,HEIGHT];
        //initializePositions
        for (int x = 0; x < WIDTH; ++x)
        {
            for (int y = 0; y < HEIGHT; ++y)
            {
                GameObject newTile = new GameObject("Tile" + x + "," + y);
                tiles[x, y] = newTile.AddComponent<Tile>();
                newTile.transform.position = new Vector3(-21f + ((2f * x) / (WIDTH * 2f)) * 42f, -12f + ((2f * y) / (HEIGHT * 2f)) * 24f);
            }
        }
        foreach (Entity entity in Resources.FindObjectsOfTypeAll(typeof(Entity)))
        {
            entity.SetPositionInit(tiles[entity.xPosition, entity.yPosition]);
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

    public Tile GetTile(int x, int y)
    {
        return tiles[x,y];
    }
}
