using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    public Camera cam;
    Server server;
    GumBallSpawner gumBallSpawner;
    public GameObject playerPrefab;
    public Dictionary<int, Player> players = new Dictionary<int, Player>();

    public bool isServerScene;
    public Transform[] spawnPosition;
    public Transform[] cameraPosition;
    Quaternion[] spawnRotation;
    Quaternion[] cameraRotation;


    public GameObject[] wall;
    [HideInInspector] public int playerNumber;
    [HideInInspector] public bool started;

    public float sendRate;

    float lastMovementUpdate = 0;

    private void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            wall[i].gameObject.SetActive(true);
        }
        wall[0].gameObject.SetActive(false);
        if (isServerScene == true)
        {
            server = GameObject.Find("Server").GetComponent<Server>();
        }
        gumBallSpawner = GameObject.Find("GumBallSpawner").GetComponent<GumBallSpawner>();

        spawnRotation = new Quaternion[4];
        spawnRotation[0] = Quaternion.Euler(0, 0, 0);
        spawnRotation[1] = Quaternion.Euler(0, -90, 0);
        spawnRotation[2] = Quaternion.Euler(0, -180, 0);
        spawnRotation[3] = Quaternion.Euler(0, 90, 0);

        cameraRotation = new Quaternion[4];
        cameraRotation[0] = Quaternion.Euler(45, 0, 0);
        cameraRotation[1] = Quaternion.Euler(45, -90, 0);
        cameraRotation[2] = Quaternion.Euler(45, -180, 0);
        cameraRotation[3] = Quaternion.Euler(45, 90, 0);


    }


    private void Update()
    {
       
        if (started == false)
        {
            return;
        }

        for (int i = 1; i <= 4; i++)
        {
            if (players.ContainsKey(i) == true)
            {
                if (players[i].score <= 0)
                {


                    players[i].wall.gameObject.SetActive(true);
                    Destroy(players[i].gameObject);
                    players.Remove(i);

                }
            }

        }

        if (isServerScene == true)
        {
            //SendAllPlayerPositions(0.2f);
            SendPositions(sendRate);

        }
        

    }
    public void SpawnPlayer(string[] splitData)
    {

        for (int i = 1; i < splitData.Length - 1; i++)
        {

            int playerNumberAux = int.Parse(splitData[i]);

            Player player = Instantiate(playerPrefab, spawnPosition[playerNumberAux - 1].position, spawnRotation[playerNumberAux - 1]).GetComponent<Player>();
            player.playerNumber = playerNumberAux;
            player.wall = wall[playerNumberAux - 1];

            if (isServerScene == true)
            {
                player.isServerScene = true;
            }
            //player.GetComponent<Player>().nameText.text = splitData[i + 1];
            players.Add(playerNumberAux, player);

        }
        if (players.ContainsKey(playerNumber) == true)
        {
            players[playerNumber].isLocalPlayer = true;
        }

        if (isServerScene == false)
        {

            cam.transform.position = cameraPosition[playerNumber - 1].position;
            cam.transform.rotation = cameraRotation[playerNumber - 1];

        }


    }

    public void SendSpawn()
    {
        string msg = "SPAWN_PLAYER|";

        for (int i = 0; i < server.clients.Count; i++)
        {
            msg += server.clients[i].playerNumber + "|";
        }

        server.SendAll(msg, server.allCostDelivery);


        string[] splitData = msg.Split('|');
        SpawnPlayer(splitData);



    }

    //////public void SendAllPlayerPositions(float sendRate)
    //////{

    //////    if (Time.time - lastMovementUpdate > sendRate)
    //////    {

    //////        string msg = "PLAYER_POS|";

    //////        for (int i = 1; i <= 4; i++)
    //////        {
    //////            if (players.ContainsKey(i))
    //////            {
    //////                msg += players[i].transform.position.x.ToString("F1") + "|"
    //////                    + players[i].transform.position.z.ToString("F1") + "|"
    //////                    + players[i].playerNumber + "|";

    //////            }





    //////        }
    //////        server.SendAll(msg, server.unreliableChannelID);

    //////        lastMovementUpdate = Time.time;

    //////    }
    //////}

    public string GetAllPlayerPositions()
    {
        string msg = "";
        //if (Time.time - lastMovementUpdate > sendRate)
        {



            for (int i = 1; i <= 4; i++)
            {
                if (players.ContainsKey(i))
                {
                    msg += players[i].transform.position.x.ToString("F1") + "|"
                        + players[i].transform.position.z.ToString("F1") + "|"
                        + players[i].playerNumber + "|";

                }





            }
            //server.SendAll(msg, server.unreliableChannelID);

            //lastMovementUpdate = Time.time;

        }
        return msg;
    }



    public void SendPositions(float sendRate)
    {
        string msg = "POS|";
        if (Time.time - lastMovementUpdate > sendRate)
        {
            msg += GetAllPlayerPositions() + "%" + gumBallSpawner.GetAllGumBallPositions();

            Debug.Log("msg " + msg);




            server.SendAll(msg, server.unreliableChannelID);

            lastMovementUpdate = Time.time;
        }
    }

    public void SetPlayerTargetPosition(string[] splitData)
    {
        for (int i = 0; i < splitData.Length - 3; i += 3)
        {
            if (players.Count > 0)
            {

                if (players.ContainsKey(int.Parse(splitData[3 + i])) == true)
                {
                    players[int.Parse(splitData[3 + i])].targetPosition = new Vector3(float.Parse(splitData[1 + i]), 1, float.Parse(splitData[2 + i]));

                    players[int.Parse(splitData[3 + i])].playerNumber = int.Parse(splitData[3 + i]);


                }


            }


        }
    }
}
