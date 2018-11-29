using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports;

public class PlayerOffline : MonoBehaviour
{
    [HideInInspector] public GameObject wall;


    [HideInInspector] public bool thrusted;
	public int deadZone;
    public float speed;
    public float movementLimit;
    public int score = 20;
    public Text scoreText;
	public int direction = 1;
    int thrustForce = 3000;

    public int playerNumber;
	public float input;

    public List<GumBallOffline> closeGumBalls = new List<GumBallOffline>();

    public ParticleSystem particle;
    public GameObject panel;


    public string h;
   // PlayerControls[] playerControls;

	SerialPort serial;
    private void Awake()
    {

		if (playerNumber == 2) {
			serial = new SerialPort("COM4", 9600);//definicao de porta
		}
    }




    void Start()
    {
		if (playerNumber == 2) {
        	serial.Open();//abrir porta
			serial.ReadTimeout = 1;
		}

        scoreText.text = score.ToString();
        if (wall)
        {
            wall.gameObject.SetActive(false);
        }
        //playerControls = new PlayerControls[4];
        //playerControls[0].downKey = KeyCode.D;

    }



    void Update()
    {
        scoreText.text = score.ToString();

		if (playerNumber == 2 && serial.IsOpen) {
			
			h = serial.ReadLine ();
		}


        MoveControls();

        if(score <= 0)
        {
            Time.timeScale = 0;
            panel.SetActive(true);
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
        particle.Play();        

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
		if (playerNumber == 1) {
			float lx = Input.GetAxis ("Horizontal");
            
			transform.Translate (Time.deltaTime * speed * lx, 0, 0);
            

			if (Input.GetButtonDown ("Thrust")) {				
				Thrust ();
			}


		} else if (playerNumber == 2) {
			string[] split = h.Split (',');

			input = (float.Parse (split [0]) / 510) - 1;

			//Debug.Log (input);
			/*(if (input > 0.1f) {
				direction = 1;
			} else if (input < -0.1f) {
				direction = -1;
			} else {
				direction = 0;


			}*/
			if(input < -0.05f || input > 0.05f)
			transform.Translate (Time.deltaTime * speed * input, 0, 0);
		}


    }

    
}
