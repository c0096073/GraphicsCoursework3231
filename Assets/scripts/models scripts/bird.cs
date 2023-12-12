using System.Collections;
using UnityEngine;

public class Bird : MonoBehaviour
{
    //variables
    public float radius = 5f;//the area the bird will be flying in.
    public float speed = 2f;
    private float angle = 0f;
    private Vector3 initialPosition;
    //animator
    public Animator birdanim;

    void Start()
    {
        //load and start the animations
        birdanim = GetComponent<Animator>();
        birdanim.SetBool("flying", true);

      
        initialPosition = transform.position;  //store the initial position
    }

    void Update()
    {
        MoveBird();
    }

/*
Function to move the bird whilst its animating to simulate the bird flying around.
It'll fly in a figure of 8 motion 
*/
    public void MoveBird()
    {
        angle += speed * Time.deltaTime;

        //calculate x and z components for figure-eight motion by combining sin functions and the angle variable
        float x = Mathf.Sin(angle) * radius * 2f; //frequency of 1
        float z = Mathf.Sin(angle * 2f) * radius; //frequency of 2

   
        transform.position = initialPosition + new Vector3(x, 0f, z);//set the position of the bird along the figure of 8 path 

       
        Vector3 tangent = new Vector3(Mathf.Cos(angle), 0f, Mathf.Cos(angle * 2f)).normalized;//calculate tangent 
        if (tangent != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(tangent, Vector3.up);//calculate the rotation based on the tangent of the figure-eight path
        }
    }
}
