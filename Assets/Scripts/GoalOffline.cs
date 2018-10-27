using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalOffline : MonoBehaviour {

    public PlayerOffline player;
    public GumBallSpawnerOffline gumBallSpawnerOffline;
   

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "GumBall")
        {
            GumBallOffline gb = other.GetComponent<GumBallOffline>();
            gumBallSpawnerOffline.gumBalls.Remove(gb.gumBallNumber);
            Destroy(gb.gameObject);
            player.score--;
        }

    }
}
