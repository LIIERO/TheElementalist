using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;
using static GlobalTypes;

public class ZoeMovementAnimationsOverlay : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float jump_power;
    [SerializeField] float dash_power;
    [SerializeField] float dash_time;
    [SerializeField] float water_jump_momentum_preservation;
    [SerializeField] float cling_drag;
    [SerializeField] float fireball_momentum;
    [SerializeField] float max_falling_speed;
    [SerializeField] float coyote_time;
    [SerializeField] float jump_buffer_time;
    [SerializeField] float default_gravity_scale;
    [SerializeField] LayerMask ground;

    Rigidbody2D body;
    Animator anim;
    BoxCollider2D coll;
    ZoeShader shader_script;

    // inputs
    float input_direction; // -1 -> left, 0 -> none, 1 -> right
    bool press_jump, jump_released, press_ability, ability_released;

    // state

    private ElementState zoe_base_element_state;
    private Stack<ElementState> zoe_element_state_stack;

    ElementState current_ability = ElementState.normal;
    bool is_facing_right = true;
    bool is_grounded = false;
    bool is_using_ability = false;
    bool can_use_base_ability = false;
    (bool left, bool right) hugging_wall = (false, false);
    float coyote_time_counter; float jump_buffer_time_counter; float ability_buffer_time_counter;

    enum AnimationState { idle, run, jump, fall, cling, dash, fireball };

    // Fireball prefab
    public GameObject fireball;
    // Element overlay
    public GameObject element_overlay;
    // Element symbol for overlay
    public GameObject element_symbol;


    // PUBLIC

    public ElementState GetZoeEffectiveElement()
    {
        ElementState effective_elem;
        if (zoe_element_state_stack.Count == 0) { effective_elem = ElementState.normal; }
        else { effective_elem = zoe_element_state_stack.Peek(); }
        return effective_elem;
    }

    public bool IsGrounded() // Checks if down edge of hitbox moved by 0.1 overlaps ground
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, 0.1f, ground);
    }

    public (bool, bool) HuggingWall() // Checks if down edge of hitbox moved by 0.1 overlaps side walls
    {
        return (Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.left, 0.1f, ground), Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.right, 0.1f, ground));
    }


    // PRIVATE

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        shader_script = GetComponent<ZoeShader>();

        body.gravityScale = default_gravity_scale;
        zoe_base_element_state = ElementState.normal;
        zoe_element_state_stack = new Stack<ElementState>();
    }

    void Update()
    {
        if (!Global.PlayingCutscene)
        {
            //Debug.Log(1.0f / Time.deltaTime); // FPS
            // get inputs
            input_direction = Input.GetAxisRaw("Horizontal");
            press_jump = Input.GetKeyDown(Global.kcJump); // Input.GetButtonDown("Jump");
            jump_released = Input.GetKeyUp(Global.kcJump); // Input.GetButtonUp("Jump");
            press_ability = Input.GetKeyDown(Global.kcAbility);
            ability_released = Input.GetKeyUp(Global.kcAbility);

            // get states
            is_grounded = IsGrounded();
            hugging_wall = HuggingWall(); //TODO: Whole body should be clinging for it to work

            // needs to be done in update
            Jump();
            Ability();
            Movement(input_direction);
            UpdateAnimationState(input_direction);
        }

        //Debug.Log(element_overlay.transform.position);
    }

    // Refills n stuff
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Refill"))
        {
            ZoeColor ref_typ = collision.gameObject.GetComponent<Refill>().refill_type;
            
            if (ref_typ == ZoeColor.blue) AddToElementStack(ElementState.water);
            if (ref_typ == ZoeColor.red) AddToElementStack(ElementState.fire);
            if (ref_typ == ZoeColor.white) AddToElementStack(ElementState.air);
            if (ref_typ == ZoeColor.brown) AddToElementStack(ElementState.earth);

            Destroy(collision.gameObject);
        }
    }

    void AddToElementStack(ElementState element)
    {
        zoe_element_state_stack.Push(element);
        shader_script.UpdateColor(element);

        // Element Overlay
        float CENTER_OFFSET_X = -9;
        float CENTER_OFFSET_Y = 5;

        int position_offset = element_overlay.transform.childCount * 2;
        float new_x_pos = element_overlay.transform.position.x + position_offset;
        GameObject new_symbol = Instantiate(element_symbol, new Vector3(new_x_pos + CENTER_OFFSET_X, element_overlay.transform.position.y + CENTER_OFFSET_Y, transform.position.z), element_overlay.transform.rotation) as GameObject;
        new_symbol.transform.parent = element_overlay.transform;
        new_symbol.GetComponent<ElSymbol>().AssignSprite(element);
    }

    ElementState RemoveTopFromElementStack()
    {
        ElementState top_element = zoe_element_state_stack.Pop();

        int latest_child_id = element_overlay.transform.childCount - 1;
        Assert.IsTrue(latest_child_id >= 0); // There cannot be 0 children when removing one
        Destroy(element_overlay.transform.GetChild(latest_child_id).gameObject);

        return top_element;
    }

    void Movement(float direction)
    {
        if (!is_using_ability)
        {
            // left right movement
            //body.velocity = new Vector2(direction * 10f * speed * Time.deltaTime, body.velocity.y);
            body.velocity = new Vector2(direction * speed, body.velocity.y);

            // falling speed cap
            if (body.velocity.y <= -max_falling_speed) body.velocity = new Vector2(body.velocity.x, -max_falling_speed);

            // tortion when clinging to a wall
            if ((hugging_wall.left || hugging_wall.right) && body.velocity.y < 0f)
            { body.drag = cling_drag; }
            else { body.drag = 0f; }
        }     
    }

    void Jump()
    {
        if (!is_using_ability)
        {
            if (is_grounded) coyote_time_counter = coyote_time;
            else coyote_time_counter -= Time.deltaTime;

            if (press_jump) jump_buffer_time_counter = jump_buffer_time;
            else jump_buffer_time_counter -= Time.deltaTime;

            if (jump_buffer_time_counter > 0f && coyote_time_counter > 0f) // jump
            {
                body.velocity = new Vector2(body.velocity.x, jump_power);
                jump_buffer_time_counter = 0f;
            }
            if (body.velocity.y > 0 && jump_released) // cancel jump
            {
                body.velocity = new Vector2(body.velocity.x, 0f);
                coyote_time_counter = 0f;
            }
        }    
    }

    void Ability()
    {
        if (press_ability) ability_buffer_time_counter = jump_buffer_time;
        else ability_buffer_time_counter -= Time.deltaTime;

        // player uses ability
        if (!is_using_ability && ability_buffer_time_counter > 0f)
        {
            if (zoe_element_state_stack.Count > 0 || (zoe_base_element_state != ElementState.normal && can_use_base_ability))
            {
                is_using_ability = true;
                if (zoe_element_state_stack.Count == 0)
                {
                    can_use_base_ability = false; 
                    current_ability = zoe_base_element_state;
                }
                else
                {
                    // REMOVE TOP ABILITY
                    current_ability = RemoveTopFromElementStack();
                }

                //float ability_time;
                if (current_ability == ElementState.air)
                {
                    StartCoroutine(StopAbilityAfterSeconds(dash_time));
                }
                else if (current_ability == ElementState.water)
                {
                    StartCoroutine(StopAbilityAfterSeconds(dash_time / 2f));
                }
                else if (current_ability == ElementState.fire)
                {
                    SpawnFireball();
                    StartCoroutine(StopAbilityAfterSeconds(dash_time / 2f));
                }
            }
        }

        if (is_using_ability)
        {
            body.gravityScale = 0f;
            if (current_ability == ElementState.water)
            {
                body.velocity = new Vector2(0f, dash_power);
            }
            else if (current_ability == ElementState.air)
            {
                if (is_facing_right) body.velocity = new Vector2(dash_power, 0f);
                else body.velocity = new Vector2(-dash_power, 0f);
            }
            else if (current_ability == ElementState.fire)
            {
                if (is_facing_right) body.velocity = new Vector2(-dash_power/2, 0f);
                else body.velocity = new Vector2(dash_power/2, 0f);
            }
            else if (current_ability == ElementState.earth)
            {
                if (is_grounded) StopAbility();
                else body.velocity = new Vector2(0f, -dash_power);
            }
        }

        else if (is_grounded) can_use_base_ability = true;
    }

    void StopAbility()
    {
        shader_script.UpdateColor(GetZoeEffectiveElement()); // Update color

        is_using_ability = false;
        current_ability = ElementState.normal;
        body.gravityScale = default_gravity_scale;
        body.velocity = new Vector2(0f, body.velocity.y * water_jump_momentum_preservation);
        coyote_time_counter = 0f;
        jump_buffer_time_counter = 0f;
    }

    IEnumerator StopAbilityAfterSeconds(float t)
    {
        yield return new WaitForSeconds(t);
        StopAbility();
    }


    void UpdateAnimationState(float direction)
    {
        // change direction facing
        if (!is_using_ability)
        {
            if ((is_facing_right && direction < 0f) || hugging_wall.left)
            {
                is_facing_right = false;
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else if ((!is_facing_right && direction > 0f) || hugging_wall.right)
            {
                is_facing_right = true;
                transform.localScale = new Vector3(1, 1, 1);
            }
        }


        // animations
        AnimationState anim_state;

        if (current_ability == ElementState.air || current_ability == ElementState.water) anim_state = AnimationState.dash;
        else if (current_ability == ElementState.fire) anim_state = AnimationState.fireball;
        else if (is_grounded)
        {
            if (direction == 0f || hugging_wall.left || hugging_wall.right) anim_state = AnimationState.idle;
            else anim_state = AnimationState.run;
        }
        else
        {
            if (body.velocity.y > 0.1f) anim_state = AnimationState.jump;
            else if (hugging_wall.left || hugging_wall.right) anim_state = AnimationState.cling;
            else anim_state = AnimationState.fall;
        }

        anim.SetInteger("state", (int)anim_state);
    }

    void SpawnFireball()
    {
        float DOWN_OFFSET = 0.4f;
        GameObject new_fireball = Instantiate(fireball, transform.position - new Vector3(0f, DOWN_OFFSET, 0f), transform.rotation);
        if (!is_facing_right) new_fireball.transform.localScale = new Vector3(-1, 1, 1);
    }

}
