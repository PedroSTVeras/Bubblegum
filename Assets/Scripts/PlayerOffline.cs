﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports;

public class PlayerOffline : MonoBehaviour
{
    [HideInInspector] public GameObject wall;


    [HideInInspector] public bool thrusted;

    public float speed;
    public float movementLimit;
    public int score = 20;
    public Text scoreText;
    int thrustForce = 3000;

    public int playerNumber;

    public List<GumBallOffline> closeGumBalls = new List<GumBallOffline>();



    public string h;
   // PlayerControls[] playerControls;

    //SerialPort serial = new SerialPort("COM3", 9600);//definicao de porta
    private void Awake()
    {


    }




    void Start()
    {
        //serial.Open();//abrir porta

        scoreText.text = score.ToString();
        if (wall != null)
        {
            wall.gameObject.SetActive(false);
        }
        //playerControls = new PlayerControls[4];
        //playerControls[0].downKey = KeyCode.D;
    }



    void Update()
    {
        scoreText.text = score.ToString();


        //h = serial.ReadLine();



        MoveControls();

        if (Input.GetKeyDown(KeyCode.Space))
        {

            Thrust();
        }

        
        if (gameObject.transform.position.x > movementLimit)
        {
            gameObject.transform.position = new Vector3(movementLimit, gameObject.transform.position.y, gameObject.transform.position.z);
        }
        if (gameObject.transform.position.x < -movementLimit)
        {
            gameObject.transform.position = new Vector3(-movementLimit, gameObject.transform.position.y, gameObject.transform.position.z);
        }


    }


    public void Thrust()
    {
        for (int i = 0; i < closeGumBalls.Count; i++)
        {

            Vector3 dir = closeGumBalls[i].transform.position - transform.position;
            dir = dir.normalized;

            closeGumBalls[i].rb.velocity = closeGumBalls[i].rb.velocity.normalized * 0;
            closeGumBalls[i].rb.AddForceAtPosition(dir * thrustForce, transform.position);

        }

    }

   


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "GumBall")
        {

            GumBallOffline gumBall;
            gumBall = other.GetComponent<GumBallOffline>();

            if (closeGumBalls.Contains(gumBall) == false)
            {
                closeGumBalls.Add(gumBall);
            }



        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "GumBall")
        {
            GumBallOffline gumBall;
            gumBall = other.GetComponent<GumBallOffline>();
            if (closeGumBalls.Contains(gumBall) == true)
            {
                closeGumBalls.Remove(gumBall);
            }
        }
    }

   
    //interface PlayerControls
    //{
    //    KeyCode rightKey{ get; set; }
    //    KeyCode leftKey { get; set; }
    //    KeyCode upKey { get; set; }
    //    KeyCode downKey { get; set; }
    //}


    public void Move(Vector3 position, float speed)
    {
        transform.position = Vector3.MoveTowards(transform.position, position, speed * Time.deltaTime);
    }

    public void MoveControls()
    {
        if(playerNumber == 1)
        {
            if (Input.GetKey(KeyCode.D) )
            {

                transform.Translate(Time.deltaTime * speed, 0, 0);
            }
            if (Input.GetKey(KeyCode.A))
            {
                transform.Translate(Time.deltaTime * -speed, 0, 0);
            }
        }
        else if (playerNumber == 2)
        {
            if (h == "RIGHT")
            {

                transform.Translate(Time.deltaTime * speed, 0, 0);
            }
            if (h == "LEFT")
            {
                transform.Translate(Time.deltaTime * -speed, 0, 0);
            }
        }


    }

    
}
