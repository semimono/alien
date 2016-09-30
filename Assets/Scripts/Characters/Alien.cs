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
		movement.targetSpeed = movementAxis.x;

		// TODO: handle up and down controls
	}

	public override void jump() {
		movement.jump(rigidbody);
	}

	[System.Serializable]
	public struct Movement {

		public float walkSpeed;
		public float runSpeed;
		public float acceleration;
		public float airControl;
		public float jumpSpeed;
		public float maxSlope;
		public float floorDetectionRadius;
		public float floorDetectionDistance;

		[System.NonSerialized]
		public float targetSpeed;
		[System.NonSerialized]
		public FloorInfo floor;

		public void move(Rigidbody2D rb) {
			updateFloor(rb);

			// determine max acceleration
			float actualAccel = acceleration;
			if (!floor.floored)
				actualAccel *= airControl;

			// update velocity
			Vector2 vel = rb.velocity;
			float desiredVel = targetSpeed * walkSpeed;
			float diff = desiredVel -vel.x;
			if (Mathf.Abs(diff) < acceleration) {
				vel.x = desiredVel;
			} else {
				vel.x += actualAccel * Mathf.Sign(diff);
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

			if (floor.normal.y < 1 - maxSlope)
				floor.clear();
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
	}
	
}
