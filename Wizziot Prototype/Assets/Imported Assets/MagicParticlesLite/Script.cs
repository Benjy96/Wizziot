using UnityEngine;
using UnityEngine.UI;

public class Script : MonoBehaviour {

	public Transform camera;
	public Transform middle;
	public float speed;
	public GameObject sun;
	public Text effectsText;
	public Text effectName;

	[Space ( 10 )]
	public int index;
	public GameObject[] effects;

	float zoom = 0;
	bool sunOn = true;
	int maxEffects;

	void Start(){
		maxEffects = effects.Length - 1;
		effectsText.text = "1/" + (maxEffects + 1);
		effectName.text = "Name: " + effects [0].name;
	}

	void FixedUpdate(){
		//CAM MOVEMENT
		if (Input.GetKey (KeyCode.A)) {
			camera.LookAt (middle);
			camera.Translate (-Vector3.right * Time.deltaTime * speed);
		}

		if (Input.GetKey (KeyCode.D)) {
			camera.LookAt (middle);
			camera.Translate (Vector3.right * Time.deltaTime * speed);
		}

		if (zoom < 0.35f) {
			if (Input.GetKey (KeyCode.W)) {
				camera.position += camera.forward * Time.deltaTime * speed;
				camera.LookAt (middle);
				zoom += Time.deltaTime;
			}
		}

		if (zoom > -0.35f){
			if (Input.GetKey (KeyCode.S)) {
				camera.position += -camera.forward * Time.deltaTime * speed;
				camera.LookAt (middle);
				zoom -= Time.deltaTime;
			}
		}
		//END CAM MOVEMENT
	}
	
	public void EnableDisable(GameObject obj){
		if (obj.activeSelf) {
			obj.SetActive (false);
		} else {
			obj.SetActive (true);
		}
	}

	public void NextEffect(){
		effects [index].SetActive (false);
		index++;
		if (index > maxEffects)
			index = 0;
		effects [index].SetActive (true);
		effectsText.text = (index + 1) + "/" + (maxEffects + 1);
		effectName.text = "Name: " + effects [index].name;
	}

	public void PreviousEffect(){
		effects [index].SetActive (false);
		index--;
		if (index < 0)
			index = maxEffects;
		effects [index].SetActive (true);
		effectsText.text = (index + 1) + "/" + (maxEffects + 1);
		effectName.text = "Name: " + effects [index].name;
	}

	public void DayNight(){
		if (sunOn) {
			sunOn = false;
			sun.SetActive (false);
		} else {
			sunOn = true;
			sun.SetActive (true);
		}
	}

	public void Quit(){
		Application.Quit ();
	}
}
