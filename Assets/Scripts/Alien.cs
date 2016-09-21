using UnityEngine;
using System.Collections;

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

	[System.Serializable]
	public struct Movement {

		public float walkSpeed;
		public float runSpeed;
		public float acceleration;
		public float airControl;

		[System.NonSerialized]
		public float targetSpeed;
		public FloorInfo floor;

		public void move(Rigidbody2D rb) {
			Vector2 vel = rb.velocity;
			float desiredVel = targetSpeed * walkSpeed;

			float actualAccel = acceleration;
			float diff = desiredVel -vel.x;
			if (Mathf.Abs(diff) < acceleration) {
				vel.x = desiredVel;
			} else {
				vel.x += actualAccel * Mathf.Sign(diff);
			}

			rb.velocity = vel;
		}

		// TODO: use this
		public struct FloorInfo {
			public bool floored;
			public Vector2 floorNormal;
			public Collider floor;
		}
	}
	
}
