using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ManagementPlayModeTest {

	[Test]
	public void ManagementPlayModeTestSimplePasses() {
		// Use the Assert class to test conditions.
	}

	// A UnityTest behaves like a coroutine in PlayMode
	// and allows you to yield null to skip a frame in EditMode
	[UnityTest]
	public IEnumerator ManagementPlayModeTestWithEnumeratorPasses() {
		// Use the Assert class to test conditions.
		// yield to skip a frame
		yield return null;
	}

	//tests camera moving right on d key press
	[UnityTest]
	public IEnumerator TestMoveCameraRightOnDKey() {

		if (GameObject.Find ("GameInfo") != null) {
			GameObject.Destroy (GameObject.Find ("GameInfo"));
		}
			
		SceneManager.LoadScene("UnderGodScene");

		//Always give it a sec
		yield return null;

		Cam testcam = GameObject.Find("Main Camera").GetComponent<Cam>();

		Vector3 firstPos = testcam.transform.position;

		testcam.rightHeld = true;

		yield return null;
		testcam.rightHeld = false;

		Vector3 secondPos = testcam.transform.position;

		//Checks to see if we added one unit
		Assert.True(firstPos != secondPos);
	}

	// tests camera moving left on 'a' key press
	[UnityTest]
	public IEnumerator TestMoveCameraLeftOnAKey() {

		if (GameObject.Find ("GameInfo") != null) {
			GameObject.Destroy (GameObject.Find ("GameInfo"));
		}

		SceneManager.LoadScene("UnderGodScene");

		//Always give it a sec
		yield return null;

		Cam testcam = GameObject.Find("Main Camera").GetComponent<Cam>();

		Vector3 firstPos = testcam.transform.position;

		testcam.leftHeld = true;

		yield return null;
		testcam.leftHeld = false;

		Vector3 secondPos = testcam.transform.position;

		//Checks to see if we added one unit
		Assert.True(firstPos != secondPos);
	}

	// tests camera moving up o w key press
	[UnityTest]
	public IEnumerator TestMoveCameraUpOnWKey() {

		if (GameObject.Find ("GameInfo") != null) {
			GameObject.Destroy (GameObject.Find ("GameInfo"));
		}

		SceneManager.LoadScene("UnderGodScene");

		//Always give it a sec
		yield return null;

		Cam testcam = GameObject.Find("Main Camera").GetComponent<Cam>();

		Vector3 firstPos = testcam.transform.position;

		testcam.upHeld = true;

		yield return null;
		testcam.upHeld = false;

		Vector3 secondPos = testcam.transform.position;

		//Checks to see if we added one unit
		Assert.True(firstPos != secondPos);
	}

	//tests camera moving down on s key press
	[UnityTest]
	public IEnumerator TestMoveCameraDownOnSKey() {

		if (GameObject.Find ("GameInfo") != null) {
			GameObject.Destroy (GameObject.Find ("GameInfo"));
		}

		SceneManager.LoadScene("UnderGodScene");

		//Always give it a sec
		yield return null;

		Cam testcam = GameObject.Find("Main Camera").GetComponent<Cam>();

		Vector3 firstPos = testcam.transform.position;

		testcam.downHeld = true;

		yield return null;
		testcam.downHeld = false;

		Vector3 secondPos = testcam.transform.position;

		//Checks to see if we added one unit
		Assert.True(firstPos != secondPos);
	}

	// tests camera moving down on mouse in bottom boundary
	[UnityTest]
	public IEnumerator TestMoveCameraDownOnMouse() {

		if (GameObject.Find ("GameInfo") != null) {
			GameObject.Destroy (GameObject.Find ("GameInfo"));
		}

		SceneManager.LoadScene("UnderGodScene");

		//Always give it a sec
		yield return null;

		Cam testcam = GameObject.Find("Main Camera").GetComponent<Cam>();

		Vector3 firstPos = testcam.transform.position;

		testcam.transform.position += new Vector3(0.0f, 0.0f, -(Time.deltaTime * testcam.fov));

		Vector3 secondPos = testcam.transform.position;

		//Checks to see if we added one unit
		Assert.True(firstPos != secondPos);
	}

	//tests camera moving up on mouse in top boundary
	[UnityTest]
	public IEnumerator TestMoveCameraUpOnMouse() {

		if (GameObject.Find ("GameInfo") != null) {
			GameObject.Destroy (GameObject.Find ("GameInfo"));
		}

		SceneManager.LoadScene("UnderGodScene");

		//Always give it a sec
		yield return null;

		Cam testcam = GameObject.Find("Main Camera").GetComponent<Cam>();

		Vector3 firstPos = testcam.transform.position;

		testcam.transform.position += new Vector3(0.0f, 0.0f, Time.deltaTime * testcam.fov);

		Vector3 secondPos = testcam.transform.position;

		//Checks to see if we added one unit
		Assert.True(firstPos != secondPos);
	}

	// tests camera moving left on mouse in left boundary
	[UnityTest]
	public IEnumerator TestMoveCameraLeftOnMouse() {

		if (GameObject.Find ("GameInfo") != null) {
			GameObject.Destroy (GameObject.Find ("GameInfo"));
		}

		SceneManager.LoadScene("UnderGodScene");

		//Always give it a sec
		yield return null;

		Cam testcam = GameObject.Find("Main Camera").GetComponent<Cam>();

		Vector3 firstPos = testcam.transform.position;

		testcam.transform.position += new Vector3(-(Time.deltaTime * testcam.fov), 0.0f, 0.0f);

		Vector3 secondPos = testcam.transform.position;

		//Checks to see if we added one unit
		Assert.True(firstPos != secondPos);
	}


	// tests camera moving right on mouse in right boundary
	[UnityTest]
	public IEnumerator TestMoveCameraRightOnMouse() {

		if (GameObject.Find ("GameInfo") != null) {
			GameObject.Destroy (GameObject.Find ("GameInfo"));
		}

		SceneManager.LoadScene("UnderGodScene");

		//Always give it a sec
		yield return null;

		Cam testcam = GameObject.Find("Main Camera").GetComponent<Cam>();

		Vector3 firstPos = testcam.transform.position;

		testcam.transform.position += new Vector3(Time.deltaTime * testcam.fov,
			0.0f, 0.0f);

		Vector3 secondPos = testcam.transform.position;

		//Checks to see if we added one unit
		Assert.True(firstPos != secondPos);
	}

	// tests that player gains worshippers
	[UnityTest]
	public IEnumerator TestPlayerGeneratesWorshippers() {

		if (GameObject.Find ("GameInfo") != null) {
			GameObject.Destroy (GameObject.Find ("GameInfo"));
		}

		SceneManager.LoadScene("UnderGodScene");

		Faction faction = GameObject.Find ("GameManager").GetComponent<GameManager> ().PlayerFaction;

		int tempWorshippers = faction.WorshipperCount;

		//Always give it a sec
		yield return new WaitForSeconds(2);

		int tempWorshippers2 = faction.WorshipperCount;

		Assert.True (tempWorshippers != tempWorshippers2);
	}

	// tests that player gains materials
	[UnityTest]
	public IEnumerator TestPlayerGeneratesMaterials() {

		if (GameObject.Find ("GameInfo") != null) {
			GameObject.Destroy (GameObject.Find ("GameInfo"));
		}

		SceneManager.LoadScene("UnderGodScene");

		Faction faction = GameObject.Find ("GameManager").GetComponent<GameManager> ().PlayerFaction;

		int tempMats = faction.MaterialCount;

		//Always give it a sec
		yield return new WaitForSeconds(2);

		int tempMats2 = faction.MaterialCount;

		Assert.True (tempMats != tempMats2);
	}
		
	// tests that that UI shows the correct amounts of materials that the user has
	[UnityTest]
	public IEnumerator TestUIShowsCorrectMaterialAmount() {

		if (GameObject.Find ("GameInfo") != null) {
			GameObject.Destroy (GameObject.Find ("GameInfo"));
		}

		SceneManager.LoadScene("UnderGodScene");

		Faction faction = GameObject.Find ("GameManager").GetComponent<GameManager> ().PlayerFaction;

		//Always give it a sec
		yield return new WaitForSeconds(2);

		int tempMats = faction.MaterialCount;

		Text temptext = GameObject.Find ("ResourcesText").GetComponent<resourceScript> ().resourceText;

		Assert.True (temptext.text.Split (' ') [1].Trim() == tempMats.ToString());
	}

	// tests that that UI shows the correct amounts of materials that the user has
	[UnityTest]
	public IEnumerator TestUIShowsCorrectWorshippersAmount() {

		if (GameObject.Find ("GameInfo") != null) {
			GameObject.Destroy (GameObject.Find ("GameInfo"));
		}

		SceneManager.LoadScene("UnderGodScene");

		Faction faction = GameObject.Find ("GameManager").GetComponent<GameManager> ().PlayerFaction;

		//Always give it a sec
		yield return new WaitForSeconds(2);

		int tempWors = faction.WorshipperCount;

		Text temptext = GameObject.Find ("ResourcesText").GetComponent<resourceScript> ().resourceText;

		Assert.True (temptext.text.Split (' ') [4].Trim() == tempWors.ToString());
	}
}
