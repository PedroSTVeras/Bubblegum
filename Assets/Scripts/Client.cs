using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;



public class Client : MonoBehaviour
{
    [HideInInspector] public MatchManager matchManager;
    GumBallSpawner gumBallSpawner;
    Lobby lobby;

    [HideInInspector] public int moveSpeed = 10;
    [HideInInspector] public int connectionID;
    int maxConnections = 1;
    [HideInInspector] public int reliableChannelID, unreliableChannelID, allCostDelivery;
    int hostID;

    byte error;

    public bool connected;

    string address = "127.0.0.1";
    int socketPort = 6666;

    private void Start()
    {
        matchManager = GameObject.Find("MatchManager").GetComponent<MatchManager>();
        gumBallSpawner = GameObject.Find("GumBallSpawner").GetComponent<GumBallSpawner>();
        lobby = GameObject.Find("Lobby").GetComponent<Lobby>();
       

        NetworkTransport.Init();



        address = PlayerPrefs.GetString("ip");
        Connect();
    }


    public void Connect()
    {
        ConnectionConfig config = new ConnectionConfig();
        reliableChannelID = config.AddChannel(QosType.ReliableSequenced);
        unreliableChannelID = config.AddChannel(QosType.Unreliable);
        allCostDelivery = config.AddChannel(QosType.AllCostDelivery);
        HostTopology topology = new HostTopology(config, maxConnections);
        hostID = NetworkTransport.AddHost(topology, 0);

        connectionID = NetworkTransport.Connect(hostID, address, socketPort, 0, out error);
        connected = true;

    }

    public void Disconnect()
    {
        for (int i = 0; i < 4; i++)
        {
            Destroy(matchManager.players[i].gameObject);
        }
        matchManager.players.Clear();


        NetworkTransport.Disconnect(hostID, connectionID, out error);
        connected = false;
    }


    private void Update()
    {

        int recHostID;
        int recConnectionID;
        int recChannelID;
        byte[] recBuffer = new byte[1024];
        int bufferSize = 1024;
        int dataSize;
        NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recHostID, out recConnectionID, out recChannelID, recBuffer, bufferSize, out dataSize, out error);


        if (recNetworkEvent == NetworkEventType.Nothing)
        {
            return;
        }
        switch (recNetworkEvent)
        {
            case NetworkEventType.ConnectEvent:

                break;
            case NetworkEventType.DataEvent:
                string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);

                string[] splitData = msg.Split('|');
                string[] splitDataAux = msg.Split('%');
                if (splitData[0] != "GUMBALL_POS" && splitData[0] != "SPAWN_PLAYER" && splitData[0] != "SPAWN_GUMBALL")
                {
                    Debug.Log("PLAYER " + matchManager.playerNumber + " RECEIVING: " + msg);
                }



                switch (splitData[0])
                {

                    case "SPAWN_PLAYER": //SPAWN_PLAYER|playerNumber
                        matchManager.SpawnPlayer(splitData);
                        matchManager.started = true;
                        lobby.gameObject.SetActive(false);

                        break;

                    case "PLAYER_POS": //PLAYER_POS|x|z|playerNumber

                        //matchManager.SetPlayerTargetPosition(splitData);
                        break;

                    case "SPAWN_GUMBALL": //SPAWN_GUMBALL|x|z|gumBallNumber

                        gumBallSpawner.SpawnGumBall(float.Parse(splitData[1]), float.Parse(splitData[2]), int.Parse(splitData[3]));
                        break;

                    case "GUMBALL_POS": //GUMBALL_POS|x|z|y|currentSpeed|gumBallNumber

                        //gumBallSpawner.SetGumBallTargetPosition(splitData);
                        break;

                    case "DESTROY_GUMBALL": //DESTROY_GUMBALL|gumBallNumber

                        Destroy(gumBallSpawner.gumBalls[int.Parse(splitData[1])].gameObject);
                        gumBallSpawner.gumBalls.Remove(int.Parse(splitData[1]));
                        break;

                    case "SCORE": //SCORE|playerNumber

                        if (matchManager.players.ContainsKey(int.Parse(splitData[1])))
                        {
                            matchManager.players[int.Parse(splitData[1])].score--;
                        }
                        break;

                    case "PLAYER_JOINED": //PLAYER_JOINED|joined|name|playerNumber

                        lobby.SetJoinButton(int.Parse(splitData[1]), splitData[2], int.Parse(splitData[3]));

                        break;

                    case "PLAYER_READY": //PLAYER_READY|ready|playerNumber

                        lobby.SetReadyButton(int.Parse(splitData[1]), int.Parse(splitData[2]));

                        break;

                    case "POS":
                        string[] splitDataAux1 = splitDataAux[0].Split('|');
                        //Debug.Log("0 " + splitDataAux[0]);
                        //Debug.Log("1 " + splitDataAux[1]);
                        string[] splitDataAux2 = splitDataAux[1].Split('|');

                        matchManager.SetPlayerTargetPosition(splitDataAux1);
                        gumBallSpawner.SetGumBallTargetPosition(splitDataAux2);
                        break;


                }
                break;
            case NetworkEventType.DisconnectEvent:
                break;
        }

    }




    public void SendMsg(string message, int channelID)
    {
        byte[] buffer = Encoding.Unicode.GetBytes(message);


        NetworkTransport.Send(hostID, connectionID, channelID, buffer, message.Length * sizeof(char), out error);

        Debug.Log("SENDING: " + message);
    }

}
