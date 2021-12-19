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

    // TODO probably bug somewhere here because sometimes enemy is stuck
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals(Constants.PlatformTag))
        {
            playerFollower.SetIsFlying(false);
        }
        if (collision.tag.Equals(Constants.PlatformTriggerTag))
        {
            playerFollower.SetIsFlying(false);
            playerFollower.TryToJump(false); 
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Equals(Constants.PlatformTag))
        {
            playerFollower.SetIsFlying(true);
        }
    }
}
