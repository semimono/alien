using UnityEngine;
using System.Collections;

public abstract class Controlable : MonoB {

	public virtual void setMovement(Vector2 movementAxis) { }

	public virtual void jump() { }

}
