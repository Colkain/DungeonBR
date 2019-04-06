using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameboardControl : MonoBehaviour {
    //Grid
    public Character[] characters;
    public Tile[, ] tiles;
    public static ArrayList doors;
    //Creation of Tiles and Characters
    [SerializeField] int maxColumns = 10;
    [SerializeField] int maxRows = 10;
    [SerializeField] float size;
    private int idt;
    private int idc;
    private int cMax;
    UIController uiC;
    CameraController cam;
    [SerializeField] private Tile currentTile;
    [SerializeField] private Tile previousTile = null;
    int mouvement;

    // Start is called before the first frame update
    public void Start () {
        cam = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<CameraController> ();
        uiC = GameObject.Find ("Canvas").GetComponent<UIController> ();
    }
    public void StartingGame (int c) {
        idt = 0; //tile id
        tiles = new Tile[maxColumns, maxRows];
        doors = new ArrayList ();
        size = 4f;
        idc = 1; //character id
        cMax = c; // max characters
        characters = new Character[cMax]; // set character list
        Spawner s = GetComponent<Spawner> ();

        //Creating tiles
        for (int x = 0; x < maxColumns; x++) {
            for (int z = 0; z < maxRows; z++) {
                tiles[x, z] = s.AddTile (idt, x, z, size, maxRows);
                idt++;
            }
        }
        MazeAlgorithm ma = new HuntAndKillMazeAlgorithm (tiles);
        ma.CreateMaze ();
    }
    // Update is called once per frame
    void Update () {
        if (characters[idc - 1] == null) {
            CreateCharacter ();
        } else {
            cam.SetCamera (idc);
            string name = "Player" + idc;
            Character player = GameObject.FindWithTag (name).GetComponent<Character> ();
            uiC.SetCharUI (player);
            ActivateDoors (false);
            currentTile = WhatTile (player);
            if (previousTile != currentTile) {
                if (previousTile != null)
                    player.SetMouvementUI (player.GetMouvementUI () - 1);
                previousTile = currentTile;
            }
            if (player.GetMouvementUI () == 0)
                ActivateDoors (true);
        }
    }
    public void SetIdc (int i) {
        idc = i;
    }
    public void SetMouvement (int m) {
        mouvement = m;
    }
    public int GetMouvement () {
        return mouvement;
    }
    public void SetPreviousTile () {
        previousTile = null;
    }
    public void CreateCharacter () {
        int c = Random.Range (0, maxColumns);
        int r = Random.Range (0, maxRows);
        Vector3 coor = new Vector3 (tiles[c, r].transform.localPosition.x, 3f, tiles[c, r].transform.localPosition.z);
        uiC.SetCreationUI (idc, coor);
    }
    public void AddInCharacters (int i) {
        characters[i - 1] = GameObject.FindGameObjectWithTag ("Player" + i).GetComponent<Character> ();
    }
    public void AddInDoors (GameObject d) {
        doors.Add (d);
    }
    public int GetIdc () => idc;
    public void ActivateDoors (bool t) {
        foreach (GameObject d in doors) {
            d.transform.GetChild (0).gameObject.SetActive (t);
        }
    }
    public Tile WhatTile (Character c) {
        Vector3 minTile;
        Vector3 maxTile;
        foreach (Tile t in tiles) {
            minTile = t.transform.localPosition + new Vector3 (-size / 2, 0, -size / 2);;
            maxTile = t.transform.localPosition + new Vector3 (size / 2, 0, size / 2);;
            if (c.transform.localPosition.x >= minTile.x && c.transform.localPosition.z >= minTile.z && c.transform.localPosition.x <= maxTile.x && c.transform.localPosition.z <= maxTile.z)
                return t;
        }
        return null;
    }
}