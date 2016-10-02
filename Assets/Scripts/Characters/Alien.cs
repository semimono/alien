using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class Alien : Controlable {

	public Movement movement;

	private Rigidbody2D rb;
	public new Rigidbody2D rigidbody {
		get {
			return getComp(ref rb);
		}
	}

	public void FixedUpdate() {
		movement.move(rigidbody);
	}

	public override void setMovement(Vector2 movementAxis) {
		movement.targetSpeed = movementAxis;
	}

	public override void jump() {
		movement.jump(rigidbody);
	}

	public override bool dash(Vector2 direction) {
		return movement.dashing.dash(rigidbody, direction, movement.floor.floored);
	}

	[System.Serializable]
	public struct Movement {

		public float walkSpeed;
		public float runSpeed;
		public float acceleration;
		public float airControl;
		public float maxFloating;
		public float maxDropRate;
		public float jumpSpeed;
		public float maxSlope;
		public float floorDetectionRadius;
		public float floorDetectionDistance;
		public Dashing dashing;

		[System.NonSerialized]
		public Vector2 targetSpeed;
		[System.NonSerialized]
		public FloorInfo floor;

		public void move(Rigidbody2D rb) {
			updateFloor(rb);
			dashing.updateDashVelocity(rb);

			// determine max acceleration
			float actualAccel = acceleration;
			if (!floor.floored)
				actualAccel *= airControl;

			// update horizontal velocity
			Vector2 vel = rb.velocity;
			float desiredVel = targetSpeed.x * walkSpeed;
			float diff = desiredVel -vel.x;
			if (Mathf.Abs(diff) < acceleration) {
				vel.x = desiredVel;
			} else {
				vel.x += actualAccel * Mathf.Sign(diff);
			}

			// update vertical velocity
			if (floor.floored) {
				rb.gravityScale = 1;
			} else {
				if (targetSpeed.y > 0) {
					rb.gravityScale = 1 - targetSpeed.y * maxFloating;
				} else {
					rb.gravityScale = 1 - targetSpeed.y * (maxDropRate -1);
				}
			}

			rb.velocity = vel;
		}

		public bool jump(Rigidbody2D rb) {
			if (!floor.floored)
				return false;
			rb.AddForce(new Vector2(0, jumpSpeed), ForceMode2D.Impulse);
			return true;
		}

		public void updateFloor(Rigidbody2D rb) {
			floor.clear();
			foreach(RaycastHit2D hit in
				Physics2D.CircleCastAll(rb.position, floorDetectionRadius, Vector3.down, floorDetectionDistance)) {
				if (hit.normal.y > floor.normal.y) {
					floor.normal = hit.normal;
					floor.floor = hit.collider;
				}
			}

			if (floor.normal.y < 1 - maxSlope) {
				floor.clear();
			} else {
				dashing.refillDashes();
			}
		}
		
		public struct FloorInfo {
			public Vector2 normal;
			public Collider2D floor;
			public bool floored { get {
					return floor != null;
				}
			}

			public void clear() {
				normal = Vector2.down;
				floor = null;
			}
		}

		[System.Serializable]
		public struct Dashing {
			public uint maxDashes;
			public float upSpeed;
			public Vector2 horizontalSpeed;
			public float downSpeed;
			public float speedDecay;

			[System.NonSerialized]
			public uint dashes;
			[System.NonSerialized]
			private Vector2 velocity;

			public void updateDashVelocity(Rigidbody2D rb) {
				if (velocity == Vector2.zero)
					return;

				float maxSpeed = Vector2.Dot(velocity.normalized, rb.velocity);
				float decay = speedDecay *Time.fixedDeltaTime;
				float mag = velocity.magnitude;
				rb.velocity -= velocity.normalized *Mathf.Max(0, Mathf.Min(decay, decay -(mag -maxSpeed)));

				velocity = velocity.normalized * (Mathf.Max(0, Mathf.Min(maxSpeed, mag -decay)));
			}

			public bool dash(Rigidbody2D rb, Vector2 direction, bool floored) {
				if (dashes < 1)
					return false;

				--dashes;
				if (Mathf.Abs(direction.y) > Mathf.Abs(direction.x)) {
					if (direction.y > 0)
						dashUp(rb);
					else if (!floored)
						dashDown(rb);
				} else {
					dashHorizontal(rb, direction.x > 0);
				}
				return true;
			}

			public void refillDashes() {
				if (dashes < maxDashes)
					dashes = maxDashes;
			}

			private void dashHorizontal(Rigidbody2D rb, bool right) {
				Vector2 vector = horizontalSpeed;
				if (!right)
					vector.x = -vector.x;
				velocity += vector;
				rb.velocity += vector;
			}

			private void dashUp(Rigidbody2D rb) {
				Vector2 newVelocity = new Vector2(0, upSpeed);
				velocity += newVelocity;
				rb.velocity += newVelocity;
			}

			private void dashDown(Rigidbody2D rb) {
				Vector2 newVelocity = new Vector2(0, -downSpeed);
				velocity += newVelocity;
				rb.velocity += newVelocity;
			}
		}
	}
	
}
