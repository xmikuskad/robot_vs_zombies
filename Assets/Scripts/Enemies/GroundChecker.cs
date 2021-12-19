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
        if (collision.tag.Equals(Constants.PlatformTag))
        {
            playerFollower.SetIsFlying(false);
        }
        if (collision.tag.Equals(Constants.PlatformTriggerTag))
        {
            playerFollower.TryToJump(false);
            playerFollower.SetIsFlying(false);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag.Equals(Constants.PlatformTag))
        {
            playerFollower.SetIsFlying(false);
        }
        if (collision.tag.Equals(Constants.PlatformTriggerTag))
        {
            playerFollower.SetIsFlying(false);
        }
    }

    // TODO probably bug somewhere here because sometimes enemy is stuck
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Equals(Constants.PlatformTag))
        {
            playerFollower.SetIsFlying(true);
        }
    }
}
