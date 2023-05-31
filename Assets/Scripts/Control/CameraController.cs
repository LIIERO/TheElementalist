using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject camera_object;
    public GameObject player_object;

    [SerializeField] float smooth_camera_time;
    [SerializeField] uint overworld_cam_left_edge_offset;
    [SerializeField] uint overworld_cam_right_edge_offset;
    [SerializeField] uint overworld_cam_up_edge_offset;
    [SerializeField] uint overworld_cam_down_edge_offset;

    int cam_offset_y, cam_offset_x;

    (int left, int right, int up, int down) camera_boundaries;


    // camera velocity
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        Camera camera = camera_object.GetComponent<Camera>();
        
        float f_cam_offset_y = camera.orthographicSize;
        float f_cam_offset_x = f_cam_offset_y * camera.aspect;
        cam_offset_y = (int)f_cam_offset_y; // 10
        cam_offset_x = (int)f_cam_offset_x; // 6

        // Set camera to proper checkpoint      
        if (Global.CurrentCheckpoint != null)
        {
            camera_object.transform.position = new Vector3(Global.CurrentCheckpoint.transform.position.x, Global.CurrentCheckpoint.transform.position.y, camera_object.transform.position.z);
        }
        else
        {
            camera_object.transform.position = new Vector3(player_object.transform.position.x, player_object.transform.position.y, camera_object.transform.position.z);
        }

        // Get boundaries of the camera
        if (Global.CurrentCheckpoint != null)
        {
            (uint left, uint right, uint up, uint down) raw_boundaries = Global.CurrentCheckpoint.GetComponent<Checkpoint>().CamEdgeOffset;

            int cp_pos_x = (int)Global.CurrentCheckpoint.transform.position.x;
            int cp_pos_y = (int)Global.CurrentCheckpoint.transform.position.y;

            camera_boundaries.left = cp_pos_x - (int)raw_boundaries.left + cam_offset_x;
            camera_boundaries.right = cp_pos_x + (int)raw_boundaries.right - cam_offset_x;
            camera_boundaries.up = cp_pos_y + (int)raw_boundaries.up - cam_offset_y;
            camera_boundaries.down = cp_pos_y - (int)raw_boundaries.down + cam_offset_y;
        }
        else
        {
            camera_boundaries.left = - (int)overworld_cam_left_edge_offset + cam_offset_x;
            camera_boundaries.right = (int)overworld_cam_right_edge_offset - cam_offset_x;
            camera_boundaries.up = (int)overworld_cam_up_edge_offset - cam_offset_y;
            camera_boundaries.down = - (int)overworld_cam_down_edge_offset + cam_offset_y;
        }

        camera_object.transform.position = ApplyCameraBounds(camera_object.transform.position);
    }

    Vector3 ApplyCameraBounds(Vector3 position)
    {
        if (position.x < camera_boundaries.left)
        {
            position.x = camera_boundaries.left;
        }
        else if (position.x > camera_boundaries.right)
        {
            position.x = camera_boundaries.right;
        }
        if (position.y < camera_boundaries.down)
        {
            position.y = camera_boundaries.down;
        }
        else if (position.y > camera_boundaries.up)
        {
            position.y = camera_boundaries.up;
        }

        return position;
    }

    void LateUpdate()
    {

        //Vector3 camera_position = level_camera.transform.position;
        Vector3 desired_position = player_object.transform.position;
        desired_position = ApplyCameraBounds(desired_position);
        Vector3 smoothed_position = Vector3.SmoothDamp(camera_object.transform.position, desired_position, ref velocity, smooth_camera_time);
        camera_object.transform.position = new Vector3(smoothed_position.x, smoothed_position.y, camera_object.transform.position.z) ; // Camera following player

        //level_camera_object.transform.position = new Vector3(player_x, player_y, level_camera_object.transform.position.z);
    }
}
