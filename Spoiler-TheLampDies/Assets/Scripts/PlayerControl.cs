using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour
{

	public LayerMask groundLayerMask;
	public float jumpForce = 8f;
	public float targetJumpSpeed = 6f;

	[Range(0f,1f)]
	public float wallJumpHeightMultiplier = 1f;
	public float jumpDuration;
	public float maxSpeed = 10f;
	public float groundAcceleration = 35f;
	public float airAcceleration = 15f;
	public float wallStickTime;
	public float frictionSlideTargetSpeed = 1f;
	public float frictionSlideMultiplier = 0.85f;
	public float groundFriction = 1f;
	public float airFriction = 1f;
	public float visualsRotAngle;


	private Player player;
	private CapsuleCollider2D playerCollider;
	private Rigidbody2D rb;


	private Vector2 spawnPosition;
	private Vector2 inputModifier;
	private Vector2 velocity;
	private bool disableInputs;
	private bool jumpInput = false;
	private bool prevJumpInput = false;


	private bool canJump;
	private bool canFloat = false;
	private bool isGrounded;
	private bool isWalled;
	private bool isRoofed;
	private bool wallLeft;
	private bool wallRight;



	#region Timers

	private float delta;
	private float jumpTimer;
	private float wallStickTimer = 0;

	#endregion

	private void Awake()
	{
		player = GetComponent<Player>();
		playerCollider = GetComponent<CapsuleCollider2D>();
		rb = GetComponent<Rigidbody2D>();
	}

	private bool IsGrounded()
	{
		float offset = 0.05f; // Adds to the y position so that circle check is not completely inside player collider
		float threshold = 0.01f; // makes the circle size a bit smaller
		Vector2 colSize = playerCollider.size; // easy access size
		Vector2 pos = transform.position; // easy access position

		Vector2 circlePos = new Vector2 (pos.x, pos.y - (colSize.y/4f) - offset);
		Collider2D col = Physics2D.OverlapCircle(circlePos, (colSize.x*0.5f)-threshold, groundLayerMask);
		if (col != null)
		{
			//Collider hit something in ground layers
			Debug.Log("IsGrounded");
			isGrounded = true;
			return isGrounded;
		}

		isGrounded = false;
		return isGrounded;
	}

	private bool IsRoofed()
	{
		float offset = 0.05f; // Adds to the y position so that circle check is not completely inside player collider
		float threshold = 0.01f; // makes the circle size a bit smaller
		Vector2 colSize = playerCollider.size; // easy access size
		Vector2 pos = transform.position; // easy access position

		Vector2 circlePos = new Vector2 (pos.x, pos.y + ((colSize.y/4f)+offset));
		Collider2D col = Physics2D.OverlapCircle(circlePos, (colSize.x*0.5f)-threshold, groundLayerMask);
		if (col != null)
		{
			Debug.Log("IsRoofed");
			isRoofed = true;
			return isRoofed;
		}

		isRoofed = false;
		return false;
	}

	private bool IsWalled()
	{
		isWalled = false;
		wallLeft = false;
		wallRight = false;

		float offset = 0.05f;
		Vector2 colSize = playerCollider.size;
		colSize.y -= colSize.x*0.5f;
		Vector2 pos = transform.position;
		ContactFilter2D filter = new ContactFilter2D();
		filter.SetLayerMask(groundLayerMask);

		Collider2D leftHit = Physics2D.OverlapCapsule(new Vector2(pos.x-offset, pos.y), colSize,CapsuleDirection2D.Vertical, 0 ,groundLayerMask);
		if (leftHit != null && leftHit.gameObject != null)
		{
			Debug.Log("Wall is left:");
			wallLeft = true;
			isWalled = true;
		}
		Collider2D rightHit = Physics2D.OverlapCapsule(new Vector2(pos.x+offset, pos.y), colSize,CapsuleDirection2D.Vertical, 0 ,groundLayerMask);
		if (rightHit != null && rightHit.gameObject != null)
		{
			Debug.Log("Wall is right");
			wallRight = true;
			isWalled = true;
		}

		return isWalled;
	}

	public void SetSpawnpoint(Vector3 position)
	{
		spawnPosition = position;
	}

	public void TakeDamage(int damage)
	{
		StartCoroutine(Respawn());
	}

	private IEnumerator Respawn()
	{
		velocity = Vector2.zero;
		rb.isKinematic = true;
		disableInputs = true;
		MeshRenderer renderer = GetComponent<MeshRenderer>();
		renderer.enabled = false;
		transform.position = spawnPosition;

		yield return new WaitForSeconds(0.5f);

		renderer.enabled = true;
		disableInputs = false;
		rb.isKinematic = false;

		yield break;
	}


	public void PlayerUpdate()
	{
		//Store deltaTime for easy access
		delta = Time.deltaTime;

		//Make all physics checks before applying new transforms.
		IsWalled();
		IsGrounded();
		IsRoofed();

		if (!disableInputs)
		{
			HandleKeyInput();
		}
		HandleMoving();
		RotateVisuals();
		JumpAndFloat();
	}

	private void HandleKeyInput()
	{
		inputModifier.x = Input.GetAxis("Horizontal");
		inputModifier.y = Input.GetAxis("Vertical");
		jumpInput = Input.GetButton("Jump");
	}

	private void HandleMoving()
	{
		float acceleration = isGrounded ? groundAcceleration : airAcceleration;
		Vector2 absModifier = new Vector2(Mathf.Abs(inputModifier.x), Mathf.Abs(inputModifier.y));
		velocity = rb.velocity;

		//Wallsticktimer keeps player stuck to wall for a fraction of time before letting go, makes walljumping feel better & easier.
		if ((wallLeft && inputModifier.x > 0.05f) || (wallRight && inputModifier.x < -0.05f))
			wallStickTimer += delta;
		else
			wallStickTimer = 0;

		if ((isWalled && wallStickTimer >= wallStickTime && !isGrounded) || !isWalled || isGrounded)
		{
			velocity = velocity + new Vector2(acceleration * inputModifier.x * Time.deltaTime, 0.0f);
		}

		//If no input or input is opposite of current velocity direction
		if (absModifier.x < 0.05f || (inputModifier.x > 0.05f && velocity.x < -0.05f) || (inputModifier.x < -0.05f && velocity.x > 0.05f))
		{
			// float oldXVel = velocity.x;

			//Applies friction values to velocity according to state.

			if (isGrounded)
				velocity = velocity + new Vector2(-velocity.x * groundFriction * Time.deltaTime, 0);

			if (!isGrounded)
				velocity = velocity + new Vector2(-velocity.x * airFriction * Time.deltaTime, 0);

			// if ((oldXVel < 0 && velocity.x > 0) && (oldXVel > 0 && velocity.x < 0))
			// 	velocity = new Vector2(0, velocity.y);

		}

		if ((inputModifier.x < -0.05f && wallLeft) || (inputModifier.x > 0.05f && wallRight))
		{
			Debug.Log("Going against wall");
			if (velocity.y < -frictionSlideTargetSpeed)
				velocity = new Vector2(velocity.x, velocity.y + (-velocity.y * frictionSlideMultiplier * Time.deltaTime));
		}

		//Limit velocity if over max
		if (Mathf.Abs(velocity.x) > maxSpeed)
			velocity.x = Mathf.Clamp(velocity.x,-1f,1f) * maxSpeed;
	}

	private void FixedUpdate()
	{
		rb.velocity = velocity;
	}


	private void RotateVisuals()
	{
		if (Mathf.Approximately(velocity.x, 0))
			//Apply rotation with slerp to smooth it out
			player.visuals.transform.localRotation = Quaternion.Slerp(player.visuals.transform.localRotation, Quaternion.Euler(0,0,0), Time.deltaTime*5f);
		else
		{
			//Rotation amount according to current speed.
			float rot = (Mathf.Abs(velocity.x)/maxSpeed) * visualsRotAngle;
			//Rotation direction from velocity.
			rot *= velocity.x > 0 ? -1 : 1;

			//Apply rotation with slerp to smooth it out
			player.visuals.transform.localRotation = Quaternion.Slerp(player.visuals.transform.localRotation, Quaternion.Euler(0,0,rot), Time.deltaTime*5f);

		}

	}

	private void JumpAndFloat()
	{
		if (!jumpInput)
		{
			canFloat = false;
			if (isWalled || isGrounded)
				canJump = true;
			if (velocity.y > 0)
			{
				//Slows down upwards movement
				velocity = velocity + new Vector2(0, -velocity.y * airFriction * Time.deltaTime);
			}
			return;
		}

		if (isGrounded && canJump)
		{
			velocity = velocity + Vector2.up * jumpForce;
			jumpTimer = 0;
			canJump = false;
			canFloat = true;
			prevJumpInput = true;
		}
		else if (isWalled && canJump)
		{
			Vector2 force;
			force.x = (wallLeft ? 1f : 0f) + (wallRight ? -1f : 0f);
			force.x *= jumpForce;
			force.y = jumpForce * wallJumpHeightMultiplier;

			velocity = force;
			jumpTimer = 0;
			canJump = false;
			canFloat = true;
			prevJumpInput = true;
			Debug.Log(velocity);
		}

		//Canfloat allows player to make higher jumps by holding jump button.
		if (canFloat && jumpInput)
		{
			jumpTimer += delta;
			if (jumpTimer < jumpDuration)
			{
				if (velocity.y < targetJumpSpeed)
					velocity = new Vector2(velocity.x, velocity.y + jumpForce);
				if (velocity.y > targetJumpSpeed)
					velocity = new Vector2(velocity.x, targetJumpSpeed);
			}
			else
			{
				canFloat = false;
			}

		}
	}
}
