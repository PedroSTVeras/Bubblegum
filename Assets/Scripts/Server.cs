using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class ServerClient : MonoBehaviour
{
    public int connectionID, playerNumber;
    public bool ready;
    //public string profileName;

}

public class Server : MonoBehaviour
{
    GumBallSpawner gumBallSpawner;
    MatchManager matchManager;
    Lobby lobby;

    [HideInInspector] public List<ServerClient> clients = new List<ServerClient>();
    int connectionID;
    int maxConnections = 6;
    [HideInInspector] public int reliableChannelID, unreliableChannelID, allCostDelivery;
    int hostID;
    int socketPort = 6666;

    byte error;
    public bool isServerScene;

    public int readyCount;
    public int clientCount;

    string[] auxPlayerPosition = new string[4];
    string msgAux = "PLAYER_POS|";
    int contPlayers = 0;


    public int testspeed;
    private void Start()
    {
        gumBallSpawner = GameObject.Find("GumBallSpawner").GetComponent<GumBallSpawner>();
        matchManager = GameObject.Find("MatchManager").GetComponent<MatchManager>();
        lobby = GameObject.Find("Lobby").GetComponent<Lobby>();



        if (isServerScene == true)
        {

            NetworkTransport.Init();


            ConnectionConfig config = new ConnectionConfig();
            reliableChannelID = config.AddChannel(QosType.ReliableSequenced);
            unreliableChannelID = config.AddChannel(QosType.Unreliable);
            allCostDelivery = config.AddChannel(QosType.AllCostDelivery);
            HostTopology topology = new HostTopology(config, maxConnections);
            hostID = NetworkTransport.AddHost(topology, socketPort, null);
        }
    }

    private void Update()
    {
        if (isServerScene == false)
        {
            return;
        }

        clientCount = clients.Count;
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
                Debug.Log("A PLAYER HAS CONNECTED");


                //SendID(recConnectionID);
                ServerClient sc = new ServerClient();
                sc.connectionID = recConnectionID;
                sc.playerNumber = 0;
                clients.Add(sc);


                break;
            case NetworkEventType.DataEvent:

                string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);



                string[] splitData = msg.Split('|');
                //if (splitData[0] != "GUMBALL_POS" && splitData[0] != "PLAYER_POS")
                {
                    Debug.Log("RECEIVING: " + msg);
                }

                switch (splitData[0])
                {

                    case "PLAYER_READY": //PLAYER_READY|ready|playerNumber
                        
                        if (clients.Find(x => x.connectionID == recConnectionID).ready == true && int.Parse(splitData[1]) == 0)
                        {
                            clients.Find(x => x.connectionID == recConnectionID).ready = false;
                            readyCount--;
                        }
                        else if (int.Parse(splitData[1]) == 1)
                        {
                            clients.Find(x => x.connectionID == recConnectionID).ready = true;

                            readyCount++;
                        }
                        lobby.SendReadyButton(int.Parse(splitData[1]), int.Parse(splitData[2]));
                        break;

                    case "PLAYER_POS": //PLAYER_POS|x|z|playerNumber
                        
                        contPlayers++;
                        msgAux += splitData[1] + "|" + splitData[2] + "|" + splitData[3] + "|";
                        
                        //Debug.Log("msgAux: " + msgAux);
                        
                        string[] msgAuxSplit = msgAux.Split('|');

                        matchManager.SetPlayerTargetPosition(msgAuxSplit);


                        if (contPlayers == matchManager.players.Count)
                        {

                            //string test = "";
                            //for (int i = 0; i < msgAuxSplit.Length; i++)
                            //{
                            //    test += msgAuxSplit[i] + "|";
                            //}
                            //Debug.Log("test: " + test);

                            msgAux = "PLAYER_POS|";
                            contPlayers = 0;
                        }
                        break;

                    case "THRUST": //THRUST|playerNumber

                        matchManager.players[int.Parse(splitData[1])].Thrust();
                        break;
                    case "PLAYER_JOINED":

                        if (int.Parse(splitData[1]) == 1)
                        {
                            clients.Find(x => x.connectionID == recConnectionID).playerNumber = int.Parse(splitData[3]);


                        }
                        lobby.SendJoinButton(int.Parse(splitData[1]), splitData[2], int.Parse(splitData[3]));
                        break;
                        
                }

                break;
            case NetworkEventType.DisconnectEvent:
                Debug.Log("DISCONNECTED PLAYER ");
                break;
        }


        if (readyCount > 0 && readyCount == clients.Count)
        {
            matchManager.SendSpawn();
            matchManager.started = true;
            gumBallSpawner.started = true;

            readyCount = 0;
        }



    }




    public void SendOnlyOne(string message, int playerNumber, int channelID)
    {

        byte[] buffer = Encoding.Unicode.GetBytes(message);


        for (int i = 0; i < clients.Count; i++)
        {
            if (clients[i].playerNumber == playerNumber)
            {
                NetworkTransport.Send(hostID, clients[i].connectionID, channelID, buffer, message.Length * sizeof(char), out error);
                //Debug.Log("SENDING TO PLAYER" + clients[i].playerNumber + ": MESSAGE: " + message);
            }
        }

    }


    public void SendAll(string message, int channelID)
    {

        byte[] buffer = Encoding.Unicode.GetBytes(message);

        for (int i = 0; i < clients.Count; i++)
        {

            NetworkTransport.Send(hostID, clients[i].connectionID, channelID, buffer, message.Length * sizeof(char), out error);
            //Debug.Log("SENDING TO PLAYER CNNID " + clients[i].connectionID + ": " + message);
        }


    }

    public void SendAllLessOne(string message, int playerNumber, int channelID)
    {

        byte[] buffer = Encoding.Unicode.GetBytes(message);


        for (int i = 0; i < clients.Count; i++)
        {
            if (clients[i].playerNumber != playerNumber)
            {

                NetworkTransport.Send(hostID, clients[i].connectionID, channelID, buffer, message.Length * sizeof(char), out error);
                //Debug.Log("SENDING TO PLAYER" + clients[i].playerNumber + ": " + message);
            }
        }


    }


}
