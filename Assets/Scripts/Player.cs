using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports;

public class Player : MonoBehaviour
{

    Client client;
    Server server;
    [HideInInspector] public GameObject wall;

    public bool isServerScene;
    [HideInInspector] public bool thrusted;

    [HideInInspector] public float speed = 15;
    public int score = 20;
    int thrustForce = 2000;
    public bool isLocalPlayer = true;
    //public string name = "none";
    //// public int connectionID;
    [HideInInspector] public int playerNumber;
    [HideInInspector] public Vector3 targetPosition;
    public List<GumBall> closeGumBalls = new List<GumBall>();
    //GumBall gumBall;


    float lastMovementUpdate = 0;
    public string h;

    //arduino girico
    SerialPort serial = new SerialPort("COM3", 9600);//definicao de porta
    private void Awake()
    {


    }




    void Start()
    {
        serial.Open();//abrir porta
        //serial.ReadTimeout = 3;
        if (isServerScene == true)
        {
            //server = GameObject.Find("Server").GetComponent<Server>();
        }
        else
        {
            //client = GameObject.Find("Client").GetComponent<Client>();
        }
        if (wall != null)
        {
            wall.gameObject.SetActive(false);
        }
    }



    void Update()
    {



        h = serial.ReadLine();

        //if (isLocalPlayer == false)
        //{
        //    Move(targetPosition, speed);

        //    return;
        //}

        //if (isServerScene == true)
        //{
        //    return;
        //}

        MoveControls();
        //string a = serial.ReadLine();
        ///tring[] test = serial.ReadLine().Split(',');
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(serial.ReadLine());
            SendThrust();
            //Thrust();
        }

        //SendPosition(0.08f);
        if (gameObject.transform.position.x > 4.5f)
        {
            gameObject.transform.position = new Vector3(4.5f, gameObject.transform.position.y, gameObject.transform.position.z);
        }
        if (gameObject.transform.position.x < -4.5f)
        {
            gameObject.transform.position = new Vector3(-4.5f, gameObject.transform.position.y, gameObject.transform.position.z);
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

    public void SendThrust()
    {
        //string msg = "THRUST|" + playerNumber;
        //client.SendMsg(msg, client.allCostDelivery);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "GumBall")
        {

            GumBall gumBall;
            gumBall = other.GetComponent<GumBall>();

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
            GumBall gumBall;
            gumBall = other.GetComponent<GumBall>();
            if (closeGumBalls.Contains(gumBall) == true)
            {
                closeGumBalls.Remove(gumBall);
            }
        }
    }

    public void SendPosition(float sendRate)
    {
        if (Time.time - lastMovementUpdate > sendRate)
        {
            string posX = transform.position.x.ToString("F1");
            string posZ = transform.position.z.ToString("F1");

            string msg = "PLAYER_POS|" + posX + "|" + posZ + "|" + playerNumber + "|";
            client.SendMsg(msg, client.unreliableChannelID);

            lastMovementUpdate = Time.time;
        }
    }

    //public void SendPosition(string[] splitdata)
    //{
    //    string msg = "3|" + splitdata[1] + "|" + splitdata[2] + "|" + splitdata[3];

    //    server.SendAllLessOne(msg, int.Parse(splitdata[3]), server.unreliableChannelID);
    //}

    public void Move(Vector3 position, float speed)
    {
        transform.position = Vector3.MoveTowards(transform.position, position, speed * Time.deltaTime);
    }

    public void MoveControls()
    {
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.A))
        {
            lastMovementUpdate = Time.time;
        }
        if (Input.GetKey(KeyCode.D) || h == "RIGHT")
        {

            transform.Translate(Time.deltaTime * speed, 0, 0);
        }
        if (Input.GetKey(KeyCode.A) || h == "LEFT")
        {
            transform.Translate(Time.deltaTime * -speed, 0, 0);
        }
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    if (isServerScene == false && other.tag == "GumBall")
    //    {

    //        GetComponent<MeshRenderer>().material.color = Color.red;

    //        SendPosition(0.1f);



    //    }

    //}

}
