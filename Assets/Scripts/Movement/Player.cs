﻿using UnityEngine;

[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour {

	public LayerMask attackingLayer;
	
	public float maxJumpHeight = 4;
	public float minJumpHeight = 1;
	public float timeToJumpApex = .4f;
	float accelerationTimeAirborne = .2f;
	float accelerationTimeGrounded = .1f;
	public float moveSpeed = 6;
	public float climbSpeed = 50;
	
	public Vector2 wallJumpClimb;
	public Vector2 wallJumpOff;
	public Vector2 wallLeap;
	
	public float wallSlideSpeedMax = 3;
	public float wallStickTime = .25f;
	
	[HideInInspector]
	public bool isAttacking;
	public bool isJumping;
	public bool isClimbable;
	public bool climbing;

    private float climbingUpPosition;
    private bool climbingUp;
	
	private PlayerAnimationController playerAnimationController;
	
	float timeToWallUnstick;
	
	float gravity;
	float maxJumpVelocity;
	float minJumpVelocity;
	Vector3 velocity;
	float velocityXSmoothing;
	
	Controller2D controller;
	
	void Start() {
		controller = GetComponent<Controller2D> ();
		playerAnimationController = GetComponent<PlayerAnimationController>();
		
		gravity = -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt (2 * Mathf.Abs (gravity) * minJumpHeight);
		isAttacking = false;
		isJumping = false;
	}
	
	void Update() {
		if (GameControl.gameControl.AnyOpenMenus() == false) {
			Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
			int wallDirX = (controller.collisions.left) ? -1 : 1;
			
			//cant move if attacking.
			if (isAttacking) {
				input = Vector2.zero;
			}

            //Animation Call Section
            if (climbingUp) {
                playerAnimationController.animator.enabled = true;
                playerAnimationController.PlayAnimation("ClimbingUp", controller.collisions.faceDir);
            }
			if (Input.GetButtonDown("Attack") && !climbingUp) {
                playerAnimationController.animator.enabled = true;
                playerAnimationController.PlayAnimation("Attack", controller.collisions.faceDir);
            }

            if (climbing && (velocity.y != 0 || velocity.x != 0) && !isAttacking && !climbingUp)
            {
                playerAnimationController.animator.enabled = true;
                playerAnimationController.PlayAnimation("Climbing", controller.collisions.faceDir);
            }
            if (climbing && (velocity.y == 0 && velocity.x == 0) && !isAttacking && !climbingUp)
            {
                playerAnimationController.animator.enabled = false;
            }

            if (velocity.y != 0 && controller.collisions.below == false && isAttacking == false && !climbing) {
                playerAnimationController.animator.enabled = true;
                playerAnimationController.PlayAnimation("Jumping", controller.collisions.faceDir);
			}
			
			if (input.x != 0 && controller.collisions.below == true && !climbing && !climbingUp) {
				playerAnimationController.PlayAnimation("Running", controller.collisions.faceDir);
			}
			
			if (input.x == 0 && isAttacking == false && controller.collisions.below == true && !climbing && !climbingUp) {
				playerAnimationController.PlayAnimation("Idle", controller.collisions.faceDir);
			}
			
						
			float targetVelocityX = input.x * moveSpeed;
			velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below)?accelerationTimeGrounded:accelerationTimeAirborne);
			
			bool wallSliding = false;
			if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0 && controller.isWallJumpable == true) {
				wallSliding = true;
				
				if (velocity.y < -wallSlideSpeedMax) {
					velocity.y = -wallSlideSpeedMax;
				}
				
				if (timeToWallUnstick > 0) {
					velocityXSmoothing = 0;
					velocity.x = 0;
					
					if (input.x != wallDirX && input.x != 0) {
						timeToWallUnstick -= Time.deltaTime;
					}
					else {
						timeToWallUnstick = wallStickTime;
					}
				}
				else {
					timeToWallUnstick = wallStickTime;
				}
			}
			
			//cant jump if attacking.
			if (!isAttacking) {
				if (Input.GetButtonDown ("Jump")) {
					if (climbing) {
						velocity.x = controller.collisions.faceDir * wallJumpClimb.x;
						velocity.y = wallJumpOff.y;
						climbing = false;
					}
					
					if (wallSliding) {
						if (wallDirX == input.x) {
							velocity.x = -wallDirX * wallJumpClimb.x;
							velocity.y = wallJumpClimb.y;
						}
						else if (input.x == 0) {
							velocity.x = -wallDirX * wallJumpOff.x;
							velocity.y = wallJumpOff.y;
						}
						else {
							velocity.x = -wallDirX * wallLeap.x;
							velocity.y = wallLeap.y;
						}
					}
					if (controller.collisions.below) {
						velocity.y = maxJumpVelocity;
					}
				}
				if (Input.GetButtonUp ("Jump")) {
					if (velocity.y > minJumpVelocity) {
						velocity.y = minJumpVelocity;
					}
				}
			}
			
			//climbing stuff
			if (isClimbable) {
				if (Input.GetButtonDown("Interact")) {
					climbing = true;
					velocity.y = 0;
				}
			}
			
			if (climbing) {
				gravity = 0;
				velocity.y = input.y * climbSpeed;
				velocity.x = input.x * climbSpeed;

                if (climbingUp) {
                    velocity = Vector3.zero;
                    Invoke("MovePlayerWhenClimbingUp", 0.125f);
                }
			} else {
				gravity = -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2);
				velocity.y += gravity * Time.deltaTime;
			}
			
			
			//possible that if climbing works i wont need this anymore.
//			velocity.y += gravity * Time.deltaTime;
			controller.Move (velocity * Time.deltaTime, input);
			
			if (controller.collisions.above || controller.collisions.below) {
				velocity.y = 0;
			}
		}
	}
	
	//this will be used to gauge interactions...I might need to do these things in the climbable script.
	public void OnTriggerEnter2D (Collider2D collider) {
		if (collider.gameObject.GetComponent<IsClimbable>()) {
			isClimbable = true;
		}
        //climbing up action
        if (collider.gameObject.layer == 15 && climbing) {
            ClimbingTransition(collider);
        }
	}
	
	//this will be used to gauge interactions...I might need to do these things in the climbable script.
	public void OnTriggerExit2D (Collider2D collider) {
		if (collider.gameObject.GetComponent<IsClimbable>()) {
			isClimbable = false;
			climbing = false;
		}
    }

    public void ClimbingTransition(Collider2D collider) {
        climbingUpPosition = collider.bounds.max.y;
        climbingUp = true;
    }
	
	
	//called from attacking animation at the begining and end.
	public void IsAttacking () {
		isAttacking = !isAttacking;
	}

    //called by climbing up animation to stop animating.
    public void IsClimbingUp() {
        climbingUp = false;
    }

    //used as an invoke to move the player
    public void MovePlayerWhenClimbingUp() {
        this.gameObject.transform.position = new Vector3(transform.position.x, climbingUpPosition);
    }
	
	//called from the animations for attacking.
	public void Attack () {
		float directionX = controller.collisions.faceDir;
		float rayLength = 12f; //make each weapon have a length component?
		float rayOriginX = 0; //define as the edge of the collider?
		float rayOriginY = 0; //define as the center of the collider?
		Vector2 rayOrigin = new Vector2 (rayOriginX, rayOriginY);
		
		RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, attackingLayer);
		if (hit) {
			CombatEngine.combatEngine.Attacking();
		}
	}
}