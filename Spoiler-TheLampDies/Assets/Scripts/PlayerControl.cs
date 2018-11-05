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
	private float forceMoveTimer = 0;
	private float inputOverrideTimer = 0;
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
		float threshold = 0.01f; // Adds to the y position so that circle check is not completely inside player collider
		Vector2 colSize = playerCollider.size; // easy access size
		Vector2 pos = transform.position; // easy access position

		Vector2 circlePos = new Vector2 (pos.x, pos.y- ((colSize.y/4f)-threshold));
		Collider2D col = Physics2D.OverlapCircle(circlePos, Mathf.Max(colSize.x/2f,colSize.y/2f), groundLayerMask);
		if (col != null)
		{
			//Collider hit something in ground layers
			isGrounded = true;
			return isGrounded;
		}

		isGrounded = false;
		return isGrounded;
	}

	private bool IsRoofed()
	{
		float threshold = 0.01f; // Adds to the y position so that circle check is not completely inside player collider
		Vector2 colSize = playerCollider.size; // easy access size
		Vector2 pos = transform.position; // easy access position

		Vector2 circlePos = new Vector2 (pos.x, pos.y + ((colSize.y/4f)+threshold));
		Collider2D col = Physics2D.OverlapCircle(circlePos, Mathf.Max(colSize.x/2f,colSize.y/2f), groundLayerMask);
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

		float threshold = 0.01f;
		Vector2 colSize = playerCollider.size;
		Vector2 pos = transform.position;

		Collider2D leftHit = Physics2D.OverlapCapsule(new Vector2(pos.x-threshold, pos.y), colSize,CapsuleDirection2D.Vertical, groundLayerMask);
		if (leftHit)
		{
			Debug.Log("Wall is left");
			wallLeft = true;
			isWalled = true;
		}
		Collider2D rightHit = Physics2D.OverlapCapsule(new Vector2(pos.x+threshold, pos.y), colSize,CapsuleDirection2D.Vertical, groundLayerMask);
		if (rightHit)
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
		rb.velocity = Vector2.zero;
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


	private void Update()
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

		if (isWalled && absModifier.x > 0.05f)
		{
			if (wallStickTimer < wallStickTime)
			{
				wallStickTimer += delta;
			}
		}
		if (!isWalled)
		{
			wallStickTimer = 0;
			// if (!(((!_wallLeft && inputModifier.x < 0.05f) || (!_wallRight && inputModifier.x > -0.05f) ) && wallStickTimer < wallStickTime) ||_isGrounded)
			{
				if (Mathf.Abs(rb.velocity.x) < maxSpeed)
					rb.velocity = rb.velocity + new Vector2(acceleration * inputModifier.x * Time.deltaTime, 0.0f);
				else
					rb.velocity = rb.velocity + new Vector2(maxSpeed * inputModifier.x * Time.deltaTime, 0.0f);

				if (rb.velocity.x > maxSpeed)
					rb.velocity = new Vector2(maxSpeed, rb.velocity.y);
			}

		}


		if (absModifier.x < 0.05f)
		{
			float oldXVel = rb.velocity.x;

			if (isGrounded)
				rb.velocity = rb.velocity + new Vector2(-rb.velocity.x * groundFriction * Time.deltaTime, 0);

			if (!isGrounded)
				rb.velocity = rb.velocity + new Vector2(-rb.velocity.x * airFriction * Time.deltaTime, 0);

			if ((oldXVel < 0 && rb.velocity.x > 0) && (oldXVel > 0 && rb.velocity.x < 0))
				rb.velocity = new Vector2(0, rb.velocity.y);

		}

		if ((inputModifier.x < -0.05f && wallLeft) || (inputModifier.x > 0.05f && wallRight))
		{
			Debug.Log("Detecting walls");
			if (rb.velocity.y < -frictionSlideTargetSpeed)
				rb.velocity = new Vector2(0, rb.velocity.y + (-rb.velocity.y * frictionSlideMultiplier * Time.deltaTime));
		}


		//In case character seems to be stuck for a while
		if (absModifier.x > 0.05f && rb.velocity.magnitude <0.01f)
		{
			Debug.Log("might need to force move");
			forceMoveTimer += delta;
			if (forceMoveTimer > 0.25f)
				ForceMove(); //Applies transform movement instead of rigidbody
		}
		else
			forceMoveTimer = 0;
	}


	private void RotateVisuals()
	{
		if (Mathf.Approximately(rb.velocity.x, 0))
			//Apply rotation with slerp to smooth it out
			player.visuals.transform.localRotation = Quaternion.Slerp(player.visuals.transform.localRotation, Quaternion.Euler(0,0,0), Time.deltaTime*5f);
		else
		{
			//Rotation amount according to current speed.
			float rot = (Mathf.Abs(rb.velocity.x)/maxSpeed) * visualsRotAngle;
			//Rotation direction from velocity.
			rot *= rb.velocity.x > 0 ? -1 : 1;

			//Apply rotation with slerp to smooth it out
			player.visuals.transform.localRotation = Quaternion.Slerp(player.visuals.transform.localRotation, Quaternion.Euler(0,0,rot), Time.deltaTime*5f);


		}

	}

	private void ForceMove()		// Called if player seems to be stuck
	{
		//Ignore force move in cases where player tries to get inside walls.
		if (((inputModifier.x < -0.05f && wallLeft) || (inputModifier.x > 0.05f && wallRight) || (jumpInput && isRoofed)) && !(isWalled && isRoofed && isGrounded))
			return;

		if ((isWalled && isGrounded) || (isRoofed && isGrounded))
		{
			Debug.LogWarning("Force Move");
			Vector3 dir = new Vector3(inputModifier.normalized.x, jumpInput? 0.1f : 0.0f, 0);
			transform.position = Vector2.Lerp(transform.position, transform.position+dir, delta);
		}
	}

	private void JumpAndFloat()
	{
		if (!jumpInput)
		{
			canFloat = false;
			if (isWalled || isGrounded)
				canJump = true;
			if (rb.velocity.y > 0)
			{
				//Slows down upwards movement
				rb.velocity = rb.velocity + new Vector2(0, -rb.velocity.y * airFriction * Time.deltaTime);
			}
			return;
		}

			Debug.Log("jump input yo");
		if (isGrounded && canJump)
		{
			Debug.Log("JUMP");
			rb.velocity = rb.velocity + Vector2.up * jumpForce;
			jumpTimer = 0;
			canJump = false;
			canFloat = true;
			prevJumpInput = true;
		}
		else if (isWalled && canJump)
		{
			int wallHitDirection = wallLeft ? 1 : -1;
			Vector2 force;
			force.x = (wallLeft ? 1f : 0f) + (wallRight ? -1f : 0f);
			force.x *= jumpForce;
			force.y = jumpForce * wallJumpHeightMultiplier;

			rb.velocity = force;
			jumpTimer = 0;
			canJump = false;
			canFloat = true;
			prevJumpInput = true;
		}


		if (canFloat && jumpInput)
		{
			jumpTimer += delta;
			if (jumpTimer < jumpDuration /1000)
			{
				Debug.Log("Floaty mc floater");
				if (rb.velocity.y < targetJumpSpeed)
					rb.velocity = rb.velocity + Vector2.up * jumpForce;

				// rb.velocity = new Vector2 (rb.velocity.x, _canFloatUp ? jumpForce : 0);
			}
			else
				canFloat = false;

		}
	}
}
