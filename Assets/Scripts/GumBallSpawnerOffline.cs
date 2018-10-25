using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GumBallSpawnerOffline : MonoBehaviour
{

    public GameObject gumBallPrefab;
    //Server server;
    public Transform[] spawnPosition;
    [HideInInspector] public bool started = true;
    public float spawnRate = 200;
    float spawnTimer;
    [HideInInspector] public int spawnPositionNumber;

    public Dictionary<int, GumBallOffline> gumBalls = new Dictionary<int, GumBallOffline>();

    public Transform[] dirPoints;
    
    int maxGumBall = 7;

    private void Start()
    {
        

    }

    private void Update()
    {


        if (Input.GetKeyDown(KeyCode.Space))
        {
            started = true;
        }
        if (started == false)
        {
            return;
        }
       

        if (gumBalls.Count < maxGumBall)
        {

            Spawner();
        }
    }

    public void Spawner()
    {
        spawnTimer++;

        if (spawnTimer == spawnRate)
        {
            spawnPositionNumber++;

            for (int i = 1; i < maxGumBall + 1; i++)
            {
                if (gumBalls.ContainsKey(i) == false)
                {
                    GumBallOffline gum = Instantiate(gumBallPrefab, spawnPosition[spawnPositionNumber - 1].position, Quaternion.identity).GetComponent<GumBallOffline>();
                    //gum.savedStartDir = SetStartDir();
                    Vector3 dir = SetStartDir() - gum.transform.position;

                    dir = dir.normalized;
                    gum.GetComponent<Rigidbody>().AddForce(dir * 500);
                    gum.gumBallNumber = i;


                    

                    gumBalls.Add(i, gum);


                    break;
                }
            }

            if (spawnPositionNumber == 4)
            {
                spawnPositionNumber = 0;
            }

            spawnTimer = 0;
        }
    }
  

    Vector3 SetStartDir()
    {
        //define uma direcao inicial aleatoria
        if (spawnPositionNumber == 1)
        {

            return dirPoints[Random.Range(0, 3)].position;


        }
        else if (spawnPositionNumber == 2)
        {
            return dirPoints[Random.Range(0, 3)].position;
        }
        else if (spawnPositionNumber == 3)
        {
            return dirPoints[Random.Range(3, 5)].position;
        }
        else
        {
            return dirPoints[Random.Range(3, 5)].position;
        }
    }

   
}