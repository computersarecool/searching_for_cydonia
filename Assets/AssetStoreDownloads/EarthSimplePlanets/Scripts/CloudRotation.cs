using UnityEngine;

public class CloudRotation : MonoBehaviour {

	public float timeMultiplier;

	public void Update () {
		var rotateAmount = SettingsSingleton.Instance.Clock * timeMultiplier;
		transform.rotation = Quaternion.Euler(rotateAmount * Vector3.up);
	}
}
