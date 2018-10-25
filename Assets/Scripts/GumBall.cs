using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GumBall : MonoBehaviour
{
    Server server;
    Client client;
    MatchManager matchManager;
    GumBallSpawner gumBallSpawner;
    [HideInInspector] public Rigidbody rb;
    float lifeTime;
    float minSpeed = 10f, maxSpeed = 30f;
    public float currentSpeed;
    [HideInInspector] public bool isServerScene;
    [HideInInspector] public int gumBallNumber;


    [HideInInspector] public float targetSpeed;

    //float lastMovementUpdate = 0;

    public GameObject cumBallGhostPrefab;
    //GameObject gumBallGhost;

    [HideInInspector] public Vector3 targetPosition;
    public Transform[] dirPoints;
    float startTime;
    float journeyLength;
    float journeyTotalDistance;


    //bool closeToPlayer;
    float fraction;
    float journeyFraction;
    bool closeToPlayer, scoreSent;
    float savedOldSpeed;

    int pathCount;
    Vector3 pathPositionAux;
    Vector3[] pathPosition;
    Vector3 savedStartDir, savedOldPosition;
    Vector3 dir;

    //Vector3 normalizeDirection;
    public void Start()
    {
        pathPosition = new Vector3[10];
        gumBallSpawner = GameObject.Find("GumBallSpawner").GetComponent<GumBallSpawner>();
        matchManager = GameObject.Find("MatchManager").GetComponent<MatchManager>();
        startTime = Time.time;


        journeyTotalDistance = Vector3.Distance(transform.position, targetPosition);

       

        if (isServerScene == true)
        {
            rb = GetComponent<Rigidbody>();
            server = GameObject.Find("Server").GetComponent<Server>();
            savedStartDir = SetStartDir();
            dir = savedStartDir - transform.position;

            dir = dir.normalized;
            rb.AddForce(dir * 500);
        }



    }

    public void Update()
    {
        
       // Debug.DrawLine(transform.position, transform.forward);


        lifeTime++;

        if (lifeTime % 10 == 0)
        {
            for (int i = 0; i < 6; i++)
            {
                //Debug.Log("pathPosition[" + i + "]: " + pathPosition[i] + "     aux: " + pathPositionAux);
            }
            //savedOldPosition = transform.position;
            savedOldSpeed = currentSpeed;
            //Debug.Log("SALVANDO POSICAO: " + gumBallNumber);

            if (pathCount == 6)
            {

                for (int i = 1; i < 6; i++)
                {
                    //pathPositionAux = pathPosition[i];

                    pathPosition[i - 1] = pathPosition[i];
                    pathPosition[i] = transform.position;



                    //pathPosition[i] = transform.position;

                }



            }
            else
            {
                pathPositionAux = transform.position;
                pathPosition[pathCount] = transform.position;
                pathCount++;
            }





        }


        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    //Vector3 dir = new Vector3(0, 0, 0) - transform.position;
        //    //dir = dir.normalized;
        //    //rb.AddForce(dir * 1000);
        //    rb.velocity = rb.velocity.normalized * maxSpeed;
        //}

        if (lifeTime == 60)
        {
            GetComponent<Collider>().material.bounciness = 0.95f;
            GetComponent<Collider>().material.bounceCombine = PhysicMaterialCombine.Average;
            //rb.constraints = RigidbodyConstraints.FreezePositionY;
            //gumBallGhost = Instantiate(cumBallGhostPrefab, pathPosition[0], Quaternion.identity);


        }
        //if (lifeTime == 390)
        //{
        //    //if (isServerScene == false)
        //    {
        //        gumBallSpawner.gumBalls.Remove(gumBallNumber);

        //    }
        //    Destroy(gameObject);
        //}

        if (isServerScene == true)
        {
            currentSpeed = rb.velocity.magnitude;
            //limite de velocidade
            if (rb.velocity.magnitude > maxSpeed)
            {
                rb.velocity = rb.velocity.normalized * maxSpeed;
            }
            else if (rb.velocity.magnitude < minSpeed)
            {
                rb.velocity = rb.velocity.normalized * minSpeed;
            }



            //if (gumBallGhost != null)
            {

                //gumBallGhost.transform.position = Vector3.MoveTowards(gumBallGhost.transform.position, pathPosition[1], savedOldSpeed * Time.deltaTime);

            }

            //if (gumBallGhost != null)
            {
                //SendPosition(0.2f);
            }
        }
        else
        {

            //rb.AddForce(dir * 10);

            //Vector3 dir = new Vector3(targetPosX, 1, targetPosZ) - transform.position;
            //dir = dir.normalized;
            //rb.AddForce(dir * 1000);

            //Vector3 direction = new Vector3(targetPosX, targetPosY, targetPosZ) - transform.position;
            //if (lifeTime == 50)
            //{
            //   rb.velocity = ((new Vector3(targetPosX, targetPosY, targetPosZ) - transform.position) * Time.deltaTime * targetSpeed);
            //}





            //float distCovered = (Time.time - startTime) * targetSpeed;




            //journeyLength = Vector3.Distance(startMarker, endMarker);
            //float fracJourney = (distCovered / journeyLength) * targetSpeed;
            /////////////

            // Set our position as a fraction of the distance between the markers.
            //fraction += Time.deltaTime * targetSpeed;

            //normalizeDirection = (endMarker - transform.position).normalized;



            //if(closeToPlayer == false)
            {
                //targetPosition = new Vector3(targetPosX * 1.15f, targetPosY, targetPosZ * 1.15f);
                //targetPosition = new Vector3(targetPosX, targetPosY, targetPosZ);
            }

            // Vector3 dir = targetPosition - transform.position;
            // dir = dir.normalized;
            // if(rb.velocity.magnitude <=)
            // rb.AddForce(dir * targetSpeed);


            float currentDuration = (Time.time - startTime);
            journeyFraction = currentDuration / journeyTotalDistance;
            //transform.position = Vector3.Lerp(transform.position, targetPosition, journeyFraction);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, targetSpeed * Time.deltaTime);

            //transform.position = Vector3.MoveTowards(transform.position, endMarker, targetSpeed * Time.deltaTime);
            //transform.position += normalizeDirection * targetSpeed * Time.deltaTime;
            //transform.position += dir * Time.deltaTime * targetSpeed;
            //rb.MovePosition(new Vector3(targetPosX, 1, targetPosZ));

        }




        
        //GetComponentInChildren<TextMesh>().text = targetPosition.ToString();

        //childText.transform.rotation = Quaternion.identity;




    }
   
    void SendDestroy()
    {
        string msg = "DESTROY_GUMBALL|" + gumBallNumber;
        server.SendAll(msg, server.unreliableChannelID);
    }
    

    private void OnCollisionEnter(Collision collision)
    {
        if (isServerScene == true)
        {
            if (collision.collider.tag == "GumBall")
            {

                GetComponent<MeshRenderer>().material.color = Color.red;



                //SendPosition(0);

            }
            if (collision.collider.tag == "Player")
            {
                //if (isServerScene == false)
                {
                    //SendPosition(0);
                    //closeToPlayer = true;
                }

            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "GumBall")
        {

            GetComponent<MeshRenderer>().material.color = Color.red;



            //SendPosition(0.1f);

        }
        if (other.tag == "Player")
        {
            //if (isServerScene == false)
            {
                // SendPosition(0.1f);
                //closeToPlayer = true;
            }

        }
    }

    void SendScore(int playerNumber)
    {
        //envia pontuacao
        matchManager.players[playerNumber].score--;
        string msg = "SCORE|" + playerNumber;
        server.SendAll(msg, server.allCostDelivery);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isServerScene == true)
        {
            if (other.tag == "Goal" && scoreSent == false)
            {

                GetComponent<GumBall>().GetComponent<MeshRenderer>().material.color = Color.magenta;
                if (matchManager.players.ContainsKey(other.GetComponent<Goal>().positionNumber))
                {
                    SendScore(other.GetComponent<Goal>().positionNumber);
                    scoreSent = true;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "GumBall")
        {
            if (isServerScene == true)
            {
                GetComponent<MeshRenderer>().material.color = Color.green;


            }
        }
        if (other.tag == "Goal")
        {
            //if (isServerScene == false)
            //{
            //    client = GameObject.Find("Client").GetComponent<Client>();

            if (isServerScene == true)
            {
                gumBallSpawner.gumBalls.Remove(gumBallNumber);
                SendDestroy();
                Destroy(gameObject);
            }
                //}

                
        }
            //if (other.tag == "Player")
            //{
            //    if (isServerScene == false)
            //    {
            //        closeToPlayer = false;
            //    }
            //}
        }

    Vector3 SetStartDir()
    {
        //define uma direcao inicial aleatoria
        if (gumBallSpawner.spawnPositionNumber == 1)
        {
            if (Random.Range(1, 3) == 1)
            {
                return dirPoints[Random.Range(0, 3)].position;
            }
            else
            {
                return dirPoints[Random.Range(12, 16)].position;
            }
        }
        else if (gumBallSpawner.spawnPositionNumber == 2)
        {
            return dirPoints[Random.Range(8, 15)].position;
        }
        else if (gumBallSpawner.spawnPositionNumber == 3)
        {
            return dirPoints[Random.Range(0, 7)].position;
        }
        else
        {
            return dirPoints[Random.Range(4, 11)].position;
        }
    }


}
