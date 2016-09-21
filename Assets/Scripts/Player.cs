using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class Player : MonoB {

	public Controlable entity;

	public Controls controls;

	public ViewTracking viewTracking;

	private Camera cam;
	public new Camera camera { get {
			return getComp(ref cam);
		}
	}

	public void Start () {
		viewTracking.size = camera.orthographicSize;
	}
	
	public void FixedUpdate () {
		viewTracking.trackView(camera, entity.transform.position);
		controls.handleControls(entity);
	}

	[System.Serializable]
	public struct ViewTracking {
		public float speed;
		public float tiltRate;
		public float sizeExpandRate;

		[System.NonSerialized]
		public float size;

		public void trackView(Camera cam, Vector2 target) {
			Vector2 position = cam.transform.position;
			Vector2 diff = target - position;
			float distance = diff.magnitude;

			// tilt camera
			cam.transform.rotation = Quaternion.Euler(0, 0, -diff.x * tiltRate);
			//camera.transform.rotation = Quaternion.Euler(-diff.y * viewTracking.tiltRate, diff.x *viewTracking.tiltRate, 0);

			// resize camera
			cam.orthographicSize = size * (1 + distance * sizeExpandRate);

			// move camera
			position = Vector2.Lerp(position, target, speed);
			cam.transform.position = new Vector3(position.x, position.y, cam.transform.position.z);
		}
	}

	[System.Serializable]
	public struct Controls {
		
		public void handleControls(Controlable target) {

			target.setMovement(new Vector2(
				Input.GetAxis("Horizontal"),
				Input.GetAxis("Vertical")
			));

		}

	}
}
