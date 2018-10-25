using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lobby : MonoBehaviour
{
    public InputField nameField;
    public GameObject[] playerSlot;
    Text[] playerName = new Text[4];
    Button[] readyButton = new Button[4];
    Button[] joinButton = new Button[4];

    Client client;
    Server server;
    bool ready = false, joined = false;
    bool[] alreadyJoined = new bool[4];
    public bool isServerScene;

    private void Start()
    {
        if (isServerScene == false)
        {
            client = GameObject.Find("Client").GetComponent<Client>();
            for (int i = 0; i < 4; i++)
            {
                playerName[i] = playerSlot[i].gameObject.transform.GetChild(0).GetChild(0).GetComponent<Text>();
                readyButton[i] = playerSlot[i].gameObject.transform.GetChild(1).GetComponent<Button>();
                joinButton[i] = playerSlot[i].gameObject.transform.GetChild(2).GetComponent<Button>();
            }
        }
        else
        {
            server = GameObject.Find("Server").GetComponent<Server>();

        }
        
        

    }
    private void Update()
    {

    }


    public void Join(int index)
    {

        if (alreadyJoined[index - 1] == true)
        {
            return;
        }


        string msg;
        joined = !joined;
        joinButton[index - 1].gameObject.SetActive(false);
        readyButton[index - 1].gameObject.SetActive(true);
        playerName[index - 1].text = nameField.text;

        if (client.matchManager.playerNumber > 0)
        {
            //reseta o ultimo join
            readyButton[client.matchManager.playerNumber - 1].GetComponent<Image>().color = Color.red;
            readyButton[client.matchManager.playerNumber - 1].gameObject.SetActive(false);
            joinButton[client.matchManager.playerNumber - 1].gameObject.SetActive(true);
            playerName[client.matchManager.playerNumber - 1].text = "Player " + client.matchManager.playerNumber;
            ready = false;
            joined = true;
            msg = "PLAYER_JOINED|0|" + nameField.text + "|" + client.matchManager.playerNumber;
            client.SendMsg(msg, client.allCostDelivery);
            msg = "PLAYER_READY|0|" + client.matchManager.playerNumber;
            client.SendMsg(msg, client.allCostDelivery);

        }


        client.matchManager.playerNumber = index;

        if (joined == true)
        {
            joinButton[index - 1].gameObject.SetActive(false);


            msg = "PLAYER_JOINED|1|" + nameField.text + "|" + index;
        }
        else
        {
            joinButton[index - 1].gameObject.SetActive(true);
            //client.matchManager.playerNumber = 0;
            msg = "PLAYER_JOINED|0|" + nameField.text + "|" + index;
        }



        client.SendMsg(msg, client.allCostDelivery);

    }




    public void SetJoinButton(int joined, string name, int playerNumber)
    {
        //readyButton[playerNumber - 1].gameObject.SetActive(true);
        if (joined == 1)
        {
            joinButton[playerNumber - 1].gameObject.SetActive(false);
            readyButton[playerNumber - 1].gameObject.SetActive(true);
            playerName[playerNumber - 1].text = name;
            alreadyJoined[playerNumber - 1] = true;
        }
        else if (joined == 0)
        {
            joinButton[playerNumber - 1].gameObject.SetActive(true);
            readyButton[playerNumber - 1].gameObject.SetActive(false);
            playerName[playerNumber - 1].text = "Player " + playerNumber;
            alreadyJoined[playerNumber - 1] = false;
        }


    }

    public void SendJoinButton(int joined, string name, int playerNumber)
    {
        string msg = "PLAYER_JOINED|" + joined + "|" + name + "|" + playerNumber;
        server.SendAllLessOne(msg, playerNumber, server.allCostDelivery);
    }

    public void Ready(int index)
    {
        if (alreadyJoined[index - 1] == true)
        {
            return;
        }

        ready = !ready;
        string msg;
        if (ready == true)
        {
            readyButton[index - 1].GetComponent<Image>().color = Color.green;

            msg = "PLAYER_READY|1|" + index;
        }
        else
        {
            readyButton[index - 1].GetComponent<Image>().color = Color.red;

            msg = "PLAYER_READY|0|" + index;
        }



        client.SendMsg(msg, client.allCostDelivery);
    }

    public void SetReadyButton(int ready, int playerNumber)
    {
        readyButton[playerNumber - 1].gameObject.SetActive(true);
        if (ready == 1)
        {
            readyButton[playerNumber - 1].GetComponent<Image>().color = Color.green;
        }
        else if (ready == 0)
        {
            readyButton[playerNumber - 1].GetComponent<Image>().color = Color.red;
        }
    }


    public void SendReadyButton(int ready, int playerNumber)
    {

        string msg = "PLAYER_READY|" + ready + "|" + playerNumber;
        server.SendAllLessOne(msg, playerNumber, server.allCostDelivery);


    }


}
