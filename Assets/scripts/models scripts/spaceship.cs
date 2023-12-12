using System.Collections;
using UnityEngine;

public class Spaceship : MonoBehaviour
{

    //variables
    public Animator spaceshipAnim;
    public bool startAnimations = false;
    public float AnimDelay = 5f;
    public AudioSource ufoSound;

    private void Start()
    {
        StartCoroutine(PlayRayAnimWithDelay());
    }

    private IEnumerator PlayRayAnimWithDelay()
    {
    
        if (spaceshipAnim == null)
        {
    
            yield break;
        }

        yield return new WaitForSeconds(AnimDelay);

       
        if (ufoSound != null)
        {
            ufoSound.Play();//play ufo sound
        }

        //play the ray animation
        spaceshipAnim.SetBool("ray", true);
        spaceshipAnim.SetBool("idle", false);

        //wait for animation to finish
        yield return new WaitForSeconds(6f);//length of animation

     

        //reset animation state once animation is finished.
        spaceshipAnim.SetBool("ray", false);
        spaceshipAnim.SetBool("idle", true);


        StartCoroutine(PlayRayAnimWithDelay());//recursive
    }
}
