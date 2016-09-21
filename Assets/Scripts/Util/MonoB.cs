using UnityEngine;
using System.Collections;

public abstract class MonoB: MonoBehaviour {

	protected T getComp<T>(ref T reference) {
		if (reference == null)
			reference = GetComponent<T>();
		return reference;
	}
}
