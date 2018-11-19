using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.SceneManagement;

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

	[UnityTest]
	public IEnumerator TestMoveCameraRight() {

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

	[UnityTest]
	public IEnumerator TestMoveCameraLeft() {

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

	[UnityTest]
	public IEnumerator TestMoveCameraUp() {

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

	[UnityTest]
	public IEnumerator TestMoveCameraDown() {

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


}
