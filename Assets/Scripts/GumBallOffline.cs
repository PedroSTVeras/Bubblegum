﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GumBallOffline : MonoBehaviour
{
   
    GumBallSpawnerOffline gumBallSpawnerOffline;
    MatchManager matchManager;
    [HideInInspector] public Rigidbody rb;
    float lifeTime;
    float minSpeed = 10f, maxSpeed = 30f;
    public float currentSpeed;
    [HideInInspector] public int gumBallNumber;

    
    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        matchManager = GameObject.Find("MatchManager").GetComponent<MatchManager>();
        gumBallSpawnerOffline = GameObject.Find("GumBallSpawner").GetComponent<GumBallSpawnerOffline>();
       

        

        



    }

    public void Update()
    {
        lifeTime++;
        if (lifeTime == 60)
        {
            GetComponent<Collider>().material.bounciness = 0.95f;
            GetComponent<Collider>().material.bounceCombine = PhysicMaterialCombine.Average;



        }


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









    }



    private void OnCollisionEnter(Collision collision)
    {

        if (collision.collider.tag == "GumBall")
        {

            GetComponent<MeshRenderer>().material.color = Color.red;

        }


    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "GumBall")
        {

            GetComponent<MeshRenderer>().material.color = Color.red;


        }

    }



    private void OnTriggerEnter(Collider other)
    {

        
            if (other.tag == "Goal")
            {
                //matchManager.players[other.GetComponent<Goal>().positionNumber]
                
                gumBallSpawnerOffline.gumBalls.Remove(gumBallNumber);
                Destroy(gameObject);
            }
        
    }





}
