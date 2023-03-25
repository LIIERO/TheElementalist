using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoeMovement : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float jump_power;
    [SerializeField] float cling_drag;
    [SerializeField] float coyote_time;
    [SerializeField] float jump_buffer_time;
    [SerializeField] LayerMask ground;

    Rigidbody2D body;
    Animator anim;
    BoxCollider2D coll;

    // inputs
    float input_direction; // -1 -> left, 0 -> none, 1 -> right
    bool press_jump, jump_released;

    // state
    bool is_facing_right = true;
    bool is_grounded = false;
    (bool left, bool right) hugging_wall = (false, false);
    float coyote_time_counter; float jump_buffer_time_counter;

    enum AnimationState {idle, run, jump, fall, cling};

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // get inputs
        input_direction = Input.GetAxisRaw("Horizontal");
        press_jump = Input.GetButtonDown("Jump");
        jump_released = Input.GetButtonUp("Jump");

        // needs to be done in update
        Jump();
    }

    void FixedUpdate()
    {
        // get states
        is_grounded = IsGrounded();
        hugging_wall = HuggingWall(); //TODO: Whole body should be clinging for it to work

        // doesn't need to be done in update
        Movement(input_direction);
        UpdateAnimationState(input_direction);
    }

    void Movement(float direction)
    {
        // left right movement
        body.velocity = new Vector2(direction * 10f * speed * Time.deltaTime, body.velocity.y);

        // tortion when clinging to a wall
        if ((hugging_wall.left || hugging_wall.right) && body.velocity.y < 0f)
        { body.drag = cling_drag; }
        else { body.drag = 0f; }

    }

    void UpdateAnimationState(float direction)
    {
        // change direction facing
        if (is_facing_right && direction < 0f)
        {
            is_facing_right = false;
            transform.localScale = new Vector3(-1, 1, 1);
        }
        if (!is_facing_right && direction > 0f)
        {
            is_facing_right = true;
            transform.localScale = new Vector3(1, 1, 1);
        }

        // animations
        AnimationState anim_state;

        if (is_grounded)
        {
            if (direction == 0f || hugging_wall.left || hugging_wall.right)
            {
                anim_state = AnimationState.idle;
            }
            else
            {
                anim_state = AnimationState.run;
            }
        }
        else
        {
            if (body.velocity.y > 0.1f)
            {
                anim_state = AnimationState.jump;
            }
            else if (hugging_wall.left || hugging_wall.right)
            {
                anim_state = AnimationState.cling;
            }
            else
            {
                anim_state = AnimationState.fall;
            }
        }

        anim.SetInteger("state", (int)anim_state);
    }

    void Jump()
    {
        if (is_grounded)
        {
            coyote_time_counter = coyote_time;
        }
        else
        {
            coyote_time_counter -= Time.deltaTime;
        }

        if (press_jump)
        {
            jump_buffer_time_counter = jump_buffer_time;
        }
        else
        {
            jump_buffer_time_counter -= Time.deltaTime;
        }

        if (jump_buffer_time_counter > 0f && coyote_time_counter > 0f) // jump
        {
            body.velocity = new Vector2(body.velocity.x, body.velocity.y + jump_power);
            jump_buffer_time_counter = 0f;
        }
        if (body.velocity.y > 0 && jump_released)
        {
            body.velocity = new Vector2(body.velocity.x, 0f);
            coyote_time_counter = 0f;
        }
    }

    bool IsGrounded() // Checks if down edge of hitbox moved by 0.1 overlaps ground
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, 0.1f, ground);
    }

    (bool, bool) HuggingWall() // Checks if down edge of hitbox moved by 0.1 overlaps ground
    {
        return (Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.left, 0.1f, ground), Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.right, 0.1f, ground));
    }
}
