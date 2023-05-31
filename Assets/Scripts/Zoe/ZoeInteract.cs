using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GlobalTypes;

public class ZoeInteract : MonoBehaviour
{
    bool can_interact = false;
    GameObject latest_interacted = null;

    ZoeMovementAnimationsOverlay movement_script;

    private void Start()
    {
        movement_script = GetComponent<ZoeMovementAnimationsOverlay>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Interactable"))
        {
            can_interact = true;
            latest_interacted = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Interactable"))
        {
            can_interact = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(Global.kcUp))
        {
            if (can_interact && movement_script.IsGrounded())
            {
                can_interact = false;
                InteractableType interactable_type = latest_interacted.GetComponent<Interactable>().Type;

                if (interactable_type == InteractableType.teleport)
                {
                    Global.LevelEntryCoords = latest_interacted.transform.position;
                    Global.CurrentCheckpoint = latest_interacted.GetComponent<Teleport>().Checkpoint;
                    Global.ReloadCurrentScene();
                }
                else if (interactable_type == InteractableType.checkpoint)
                {
                    Global.SetCheckpointToOverworld();
                    Global.ReloadCurrentScene();
                }
            }
        }
    }
}
