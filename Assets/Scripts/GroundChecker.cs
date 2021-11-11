using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    private PlayerFollower playerFollower;

    // Start is called before the first frame update
    void Start()
    {
        playerFollower = GetComponentInParent<PlayerFollower>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag.Equals(Constants.PlatformTag))
        {
            playerFollower.SetIsJumping(false);
        }
    }
}
