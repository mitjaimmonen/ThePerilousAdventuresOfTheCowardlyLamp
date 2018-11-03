using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour
{

public LayerMask groundLayerMask;
	public CapsuleCollider2D playerCollider;
	public Rigidbody2D rb;
	public Vector3 spawnPosition;

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

	
	private bool disableInputs;
	Vector2 inputModifier;
	

	private bool jumpInput = false;
	private bool prevJumpInput = false;


	private bool _canJump;
	private bool _canFloat = false;
	private bool _isGrounded;
	private bool _isWalled;
	private bool _isRoofed;
	private bool _wallLeft;
	private bool _wallRight;



	#region Timers

	private float delta;
	private float _jumpTimer;
	private float forceMoveTimer = 0;
	private float inputOverrideTimer = 0;
	private float wallStickTimer = 0;

	#endregion


	void Awake()
	{
		playerCollider = GetComponent<CapsuleCollider2D>();
		rb = GetComponent<Rigidbody2D>();
	}

	bool IsGrounded()
	{
		float searchMagnitude = 0.1f;
		float threshold = 0.01f;
		Vector2 colSize = playerCollider.size;
		Vector2 pos = transform.position;

		Vector2 bottomLineStart = new Vector2 (pos.x, pos.y- ((colSize.y/2f)-threshold));
		Vector2 BottomLineEnd = new Vector2(pos.x, bottomLineStart.y - searchMagnitude);
		RaycastHit2D bottomHit = Physics2D.Linecast(bottomLineStart, BottomLineEnd, groundLayerMask);
		if (bottomHit)
		{
			_isGrounded = true;
			return _isGrounded;
		}

		_isGrounded = false;
		return _isGrounded;
	}

	bool IsRoofed()
	{
		float searchMagnitude = 0.1f;
		float threshold = 0.01f;
		Vector2 extents = playerCollider.bounds.extents;
		Vector2 pos = transform.position;

		Vector2 topLineStart = new Vector2 (pos.x - extents.x, pos.y+ (extents.y-threshold));
		Vector2 topLineEnd = new Vector2(topLineStart.x, topLineStart.y + searchMagnitude);
		RaycastHit2D topHit = Physics2D.Linecast(topLineStart, topLineEnd, groundLayerMask);
		if (topHit)
		{
			_isRoofed = true;
			return topHit;
		}
		else
		{
			topLineStart = new Vector2 (pos.x + extents.x, pos.y + (extents.y-threshold));
			topLineEnd = new Vector2(topLineStart.x, topLineStart.y + searchMagnitude);
			topHit = Physics2D.Linecast(topLineStart, topLineEnd, groundLayerMask);
			if (topHit)
			{
				_isRoofed = true;
				return topHit;
			}

		}
		_isRoofed = false;
		return false;
	}

	bool IsWalled()
	{
		_isWalled = false;
		_wallLeft = false;
		_wallRight = false;

		float searchMagnitude = 0.1f;
		float threshold = 0.01f;
		Vector2 extents = playerCollider.bounds.extents;
		Vector2 pos = transform.position;

		Vector2 leftLineStart = new Vector2 (pos.x - extents.x - threshold, pos.y);
		Vector2 leftLineEnd = new Vector2(leftLineStart.x - searchMagnitude, leftLineStart.y);
		RaycastHit2D leftHit = Physics2D.Linecast(leftLineStart, leftLineEnd, groundLayerMask);
		if (leftHit)
		{
			_wallLeft = true;
			_isWalled = true;
		}

		Vector2 rightLineStart = new Vector2 (pos.x + extents.x + threshold, pos.y);
		Vector2 rightLineEnd = new Vector2(rightLineStart.x + searchMagnitude, rightLineStart.y);
		RaycastHit2D rightHit = Physics2D.Linecast(rightLineStart, rightLineEnd, groundLayerMask);
		if (rightHit)
		{
			_wallRight = true;
			_isWalled = true;
		}
		
		return _isWalled;
	}

	public void SetSpawnpoint(Vector3 position)
	{
		spawnPosition = position;
	}

	public void TakeDamage(int damage)
	{
		StartCoroutine(Respawn());
	}

	IEnumerator Respawn()
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


	void Update()
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
		JumpAndFloat();
	}

	void HandleKeyInput()
	{
		inputModifier.x = Input.GetAxis("Horizontal");
		inputModifier.y = Input.GetAxis("Vertical");
		jumpInput = Input.GetButton("Jump");
	}

	void HandleMoving()
	{
		float acceleration = _isGrounded ? groundAcceleration : airAcceleration;
		Vector2 absModifier = new Vector2(Mathf.Abs(inputModifier.x), Mathf.Abs(inputModifier.y));

		if (_isWalled && absModifier.x > 0.05f)
		{
			if (wallStickTimer < wallStickTime)
			{
				wallStickTimer += delta;
			}
		}
		if (!_isWalled)
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

			if (_isGrounded)
				rb.velocity = rb.velocity + new Vector2(-rb.velocity.x * groundFriction * Time.deltaTime, 0);

			if (!_isGrounded)
				rb.velocity = rb.velocity + new Vector2(-rb.velocity.x * airFriction * Time.deltaTime, 0);

			if ((oldXVel < 0 && rb.velocity.x > 0) && (oldXVel > 0 && rb.velocity.x < 0))
				rb.velocity = new Vector2(0, rb.velocity.y);

		}

		if ((inputModifier.x < -0.05f && _wallLeft) || (inputModifier.x > 0.05f && _wallRight))
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

	void ForceMove()		// Called if player seems to be stuck
	{
		//Ignore force move in cases where player tries to get inside walls.
		if (((inputModifier.x < -0.05f && _wallLeft) || (inputModifier.x > 0.05f && _wallRight) || (jumpInput && _isRoofed)) && !(_isWalled && _isRoofed && _isGrounded))
			return;

		if ((_isWalled && _isGrounded) || (_isRoofed && _isGrounded))
		{
			Debug.LogWarning("Force Move");
			Vector3 dir = new Vector3(inputModifier.normalized.x, jumpInput? 0.1f : 0.0f, 0);
			transform.position = Vector2.Lerp(transform.position, transform.position+dir, delta);
		}
	}

	void JumpAndFloat()
	{
		if (!jumpInput)
		{
			_canFloat = false;
			if (_isWalled || _isGrounded)
				_canJump = true;
			if (rb.velocity.y > 0)
			{
				//Slows down upwards movement
				rb.velocity = rb.velocity + new Vector2(0, -rb.velocity.y * airFriction * Time.deltaTime);
			}
			return;
		}

			Debug.Log("jump input yo");
		if (_isGrounded && _canJump)
		{
			rb.velocity = rb.velocity + Vector2.up * jumpForce;
			_jumpTimer = 0;
			_canJump = false;
			_canFloat = true;
			prevJumpInput = true;
		}
		else if (_isWalled && _canJump)
		{
			int wallHitDirection = _wallLeft ? 1 : -1;
			Vector2 force;
			force.x = (_wallLeft ? 1f : 0f) + (_wallRight ? -1f : 0f);
			force.x *= jumpForce;
			force.y = jumpForce * wallJumpHeightMultiplier;

			rb.velocity = force;
			_jumpTimer = 0;
			_canJump = false;
			_canFloat = true;
			prevJumpInput = true;
		}


		if (_canFloat && jumpInput)
		{
			_jumpTimer += delta;
			if (_jumpTimer < jumpDuration /1000)
			{
				Debug.Log("Floaty mc floater");
				if (rb.velocity.y < targetJumpSpeed)
					rb.velocity = rb.velocity + Vector2.up * jumpForce;

				// rb.velocity = new Vector2 (rb.velocity.x, _canFloatUp ? jumpForce : 0);
			}
			else
				_canFloat = false;
			
		}
	}
}
