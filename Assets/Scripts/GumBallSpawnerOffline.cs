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

    public Dictionary<int, GumBall> gumBalls = new Dictionary<int, GumBall>();


    //public bool isServerScene;
    //float lastMovementUpdate = 0;
    int maxGumBall = 5;

    private void Start()
    {
        //if (isServerScene == true)
        //{
        //    server = GameObject.Find("Server").GetComponent<Server>();
        //}

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
        //if (isServerScene == true)
        //{
        //    //SendAllGumballPositions(0.06f);
        //}

        //if (isServerScene == true && gumBalls.Count < maxGumBall)
        //{

        //    Spawner();
        //}

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
                    GumBall gum = Instantiate(gumBallPrefab, spawnPosition[spawnPositionNumber - 1].position, Quaternion.identity).GetComponent<GumBall>();
                    gum.isServerScene = true;
                    gum.gumBallNumber = i;


                    string msg = "SPAWN_GUMBALL|" + spawnPosition[spawnPositionNumber - 1].position.x + "|" + spawnPosition[spawnPositionNumber - 1].position.z + "|" + i;
                    //server.SendAll(msg, server.reliableChannelID);

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
    public void SpawnGumBall(float posX, float posZ, int gn)
    {
        if (gumBalls.ContainsKey(gn) == false)
        {
            GumBall gumBall = Instantiate(gumBallPrefab, new Vector3(posX, 2, posZ), Quaternion.identity).GetComponent<GumBall>();
            gumBall.GetComponent<GumBall>().gumBallNumber = gn;

            gumBalls.Add(gn, gumBall);
        }
    }

    //public void SetGumBallTargetPosition(string[] splitData)
    //{

    //    for (int i = 0; i < splitData.Length - 5; i += 5)
    //    {
    //        if (gumBalls.Count > 0)
    //        {

    //            if (gumBalls.ContainsKey(int.Parse(splitData[4 + i])) == true)
    //            {
    //                gumBalls[int.Parse(splitData[4 + i])].targetPosition = new Vector3(float.Parse(splitData[0 + i]), float.Parse(splitData[1 + i]), float.Parse(splitData[2 + i]));
    //                gumBalls[int.Parse(splitData[4 + i])].targetSpeed = float.Parse(splitData[3 + i]);
    //                gumBalls[int.Parse(splitData[4 + i])].gumBallNumber = int.Parse(splitData[4 + i]);


    //            }
    //        }
    //    }
    //}



    

    //public string GetAllGumBallPositions()
    //{
    //    string msg = "";
  
    //    {



    //        for (int i = 1; i <= maxGumBall; i++)
    //        {
    //            if (gumBalls.ContainsKey(i))
    //            {
    //                msg += gumBalls[i].transform.position.x.ToString("F1") + "|"
    //                    + gumBalls[i].transform.position.y.ToString("F1") + "|"
    //                    + gumBalls[i].transform.position.z.ToString("F1") + "|"
    //                    + gumBalls[i].currentSpeed.ToString("F1") + "|"
    //                    + gumBalls[i].gumBallNumber + "|";

    //            }

    //        }
            

    //    }
    //    return msg;
    //}

}