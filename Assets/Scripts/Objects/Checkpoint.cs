using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    //[SerializeField] string cp_name;
    //public string Name { get { return cp_name; } }

    //[SerializeField] (uint left, uint right, uint up, uint down) camera_block_offset;
    //public (uint left, uint right, uint up, uint down) CameraBlockOffset { get { return camera_block_offset; } }
    //public (uint left, uint right, uint up, uint down) camera_block_offset;

    // How many units to each direction camera can move at max
    [SerializeField] uint cam_left_edge_offset;
    [SerializeField] uint cam_right_edge_offset;
    [SerializeField] uint cam_up_edge_offset;
    [SerializeField] uint cam_down_edge_offset;

    public (uint left, uint right, uint up, uint down) CamEdgeOffset { get { return (cam_left_edge_offset, cam_right_edge_offset, cam_up_edge_offset, cam_down_edge_offset); } }

    private void Awake()
    {
        // Set current checkpoint to the same checkpoint before reload (it is reassigned after being destroyed and created)
        if (Global.CurrentCheckpointObjectName == name)
        {
            Global.CurrentCheckpoint = gameObject;
        }
    }

    /*Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        bool is_current_cp = anim.GetBool("IsCurrent");

        if (Global.current_checkpoint == id && !is_current_cp)
        {
            anim.SetBool("IsCurrent", true);
        }
        else if (Global.current_checkpoint != id && is_current_cp)
        {
            anim.SetBool("IsCurrent", false);
        }
    }*/

}
