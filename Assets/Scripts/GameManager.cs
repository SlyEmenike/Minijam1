using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Author: Sly Emenike
    Date: 20-06-2017
    
    This project is a personal small game jame.
    I set a time of 8 hours to design and code the basic of the game to test if I could do this.
    One of the aspects I wanted to use was coroutines, since this is a new aspect I learned a while ago.
 */

/// <summary>
/// This class is used as a center point of all the game events.
/// It handles the phase order of the players, player actions, and over actions.
/// </summary>
public class GameManager : MonoBehaviour {
    enum CurrentPhase { StartTurn, BuildWall, RemoveWall, Move, EndTurn, EndRound }

    [SerializeField]
    List<Player> players;
    Player currentPlayer;

    CurrentPhase currentPhase;

    [SerializeField]
    Wall wall;

    [SerializeField]
    public int walkOrders;

    List<Wall> wallsBeingMade;
    List<Wall> wallsBeingRemoved;

    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new GameManager();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null) _instance = this;
        else if (_instance != this) Destroy(gameObject);
    }
    
    void Start ()
    {
        StartNewRound();

        wallsBeingRemoved = new List<Wall>();
        wallsBeingMade = new List<Wall>();
	}
	
	void Update ()
    {
        switch (currentPhase)
        {
            case CurrentPhase.StartTurn:
                StartTurn();
                break;
            case CurrentPhase.BuildWall:
                BuildPhase();
                break;
            case CurrentPhase.RemoveWall:
                RemovePhase();
                break;
            case CurrentPhase.Move:
                MovePhase();
                break;
            case CurrentPhase.EndTurn:
                
                break;
            case CurrentPhase.EndRound:
                break;
        }
	}

    void StartNewRound()
    {
        currentPlayer = players[0];
        currentPhase = CurrentPhase.StartTurn;
    }

    void StartTurn()
    {
        //Wait for select
        if (Input.GetKey(KeyCode.Space))
        {
            //Go to build phase
            StartBuildPhase();
        }
    }

    void StartBuildPhase()
    {
        currentPhase = CurrentPhase.BuildWall;
        Wall w = Instantiate(wall);
        w.owner = currentPlayer.playerNumber;
        w.StartSetPosition();
        wallsBeingMade.Add(w);
    }

    void BuildPhase()
    {
        //Set wall on position
        //Check if placed
    }

    public void EndBuildPhase()
    {
        currentPhase = CurrentPhase.RemoveWall;
    }

    void RemovePhase()
    {
        //Remove wall
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        LayerMask lm = 1 << LayerMask.NameToLayer("Wall");
        if (Physics.Raycast(ray, out hit,40f, lm))
        {
            Wall w = hit.transform.GetComponent<Wall>();
            //Show destroy button
            if (Input.GetMouseButtonDown(0))
            {
                if (w.currentPhase != Wall.WallPhase.ghostPlaced)
                {
                    if (!wallsBeingRemoved.Contains(w))
                    {
                        wallsBeingRemoved.Add(w);
                    }
                    w.BeingRemoved();//Set color
                    StartMovePhase();
                }
            }
            
        }

    }

    void StartMovePhase()
    {
        currentPlayer.startWalkPhase();
        currentPhase = CurrentPhase.Move;
    }

    void MovePhase()
    {
        //For each moves
        currentPlayer.WaitForNextWalkCell();
    }
    
    public void EndMovePhase()
    {
        currentPhase = CurrentPhase.EndTurn;
        GridManager.Instance.topHL.gameObject.SetActive(false);
        GridManager.Instance.rightHL.gameObject.SetActive(false);
        GridManager.Instance.downHL.gameObject.SetActive(false);
        GridManager.Instance.leftHL.gameObject.SetActive(false);
        EndTurn();
    }

    void EndTurn()
    {
        for (int i = 0; i < wallsBeingMade.Count; i++)
        {
            wallsBeingMade[i].EndTurn();
        }
        for (int i = 0; i < wallsBeingRemoved.Count; i++)
        {
            wallsBeingRemoved[i].EndTurn();
        }
        currentPlayer.EndTurn();
        if (currentPlayer.playerNumber == Player.PlayerNumber.Player4)
        {
            //All completed;
            EndRound();
        }
        else
        {
            currentPlayer = players[players.IndexOf(currentPlayer) + 1];
            currentPhase = CurrentPhase.StartTurn;
        }
        
    }

    void EndRound()
    {
        currentPhase = CurrentPhase.EndRound;
        StartCoroutine(EndRoundPhase());
    }

    IEnumerator EndRoundPhase()
    {
        //Place walls
        yield return StartCoroutine(PlaceWalls());// After place walls
        wallsBeingMade.Clear();

        yield return StartCoroutine(RemoveWalls());
        wallsBeingRemoved.Clear();
        //Destroy walls
        //Move players
        yield return StartCoroutine(MovePlayers());

        StartNewRound();
    }

    IEnumerator PlaceWalls()
    {
        Debug.Log("Start place walls");
        for (int i = 0; i < wallsBeingMade.Count; i++)
        {
            wallsBeingMade[i].gameObject.SetActive(true);
            yield return StartCoroutine(wallsBeingMade[i].PlaceWall());
        }
        yield return null;
    }

    IEnumerator RemoveWalls()
    {
        for (int i = 0; i < wallsBeingRemoved.Count; i++)
        {
            yield return StartCoroutine(wallsBeingRemoved[i].RemoveWall());
        }
        yield return null;
    }

    IEnumerator MovePlayers()
    {
        Debug.Log("Start moveing Players");
        for (int i = 0; i < players.Count; i++)
        {
            //Move players allong path
            yield return StartCoroutine(players[i].MoveAlongPath());
            players[i].OnEndRound();//Clear path and remove indicators
        }
        yield return null;
    }
}
