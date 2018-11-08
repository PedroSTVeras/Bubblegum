using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour {

    public Text arrow;

    public int arrowPos = 0;
    public float lx;


    int characterID;
	int carID;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKey("d"))
        {
            Debug.Log(arrowPos);
            arrowPos++;
            if (arrowPos > 2)
            {
                arrowPos = 0;
            }
        }
        if (Input.GetKey("a"))
        {
            arrowPos--;
            if (arrowPos < 0)
            {
                arrowPos = 2;
            }
        }



	}
}
