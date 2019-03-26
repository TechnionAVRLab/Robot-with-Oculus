using UnityEngine;
using UnityEngine.UI;

public class ComPort : MonoBehaviour {
    #region Public Variables
    public InputField mainInputField;
    public Text warningMessage;
    public Button startCalibration;
    public GameObject palmDown;
    public GameObject palmUp;
    public GameObject commInput;
    public GameObject introToTurnOff;
    public GameObject ZedCamera;
    public GameObject Light;

    #region Manus-VR     
    /*
    public GameObject fistCalibration;

    //Right Hand Fingers
    public GameObject rightHandThumb;
    public GameObject rightHandIndex;
    public GameObject rightHandMid;
    public GameObject rightHandRing;
    public GameObject rightHandPinky;

    //Left Hand Fingers
    public GameObject leftHandThumb;
    public GameObject leftHandIndex;
    public GameObject leftHandMid;
    public GameObject leftHandRing;
    public GameObject leftHandPinky;
    */
    #endregion

    //Right Hand Wrist
    public GameObject rightHandWrist;
    //Left Hand Wrist
    public GameObject leftHandWrist;
    #endregion

    private bool start;

    void Start () {
        mainInputField.onEndEdit.AddListener(delegate { ChangePortNumber(mainInputField); });
        startCalibration.onClick.AddListener(delegate { StartCalibration(); });
        PlayerPrefs.SetString("ComPort", "1");
        warningMessage.enabled = false;
        start = true;
    }
	
	// Update is called once per frame
	void Update () {
        //Debug.Log("Right Wrist Local Rotation: "+ rightHandWrist.transform.localRotation.z);
        //Debug.Log("Right Wrist Rotation: " + rightHandWrist.transform.rotation.z);
        //Debug.Log("Right Wrist Local Euler: " + rightHandWrist.transform.localEulerAngles.z);
        //Debug.Log("Right Wrist Euler: " + rightHandWrist.transform.eulerAngles.z);
        //Debug.Log(Mathf.Sin((Mathf.PI * rightHandWrist.transform.eulerAngles.z)/180)*100);

        if (start && (Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.KeypadEnter) || OVRInput.GetUp(OVRInput.RawButton.LIndexTrigger) || OVRInput.GetUp(OVRInput.RawButton.RIndexTrigger)))
        {
            warningMessage.enabled = false;
            if (mainInputField.text.Length > 0)
            {
                commInput.SetActive(false);
                palmDown.SetActive(true);
                start = false;
            }
            else
            {
                warningMessage.enabled = true;
            }
        }

        else if (palmDown.activeSelf && (Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.KeypadEnter) || OVRInput.GetUp(OVRInput.RawButton.LIndexTrigger) || OVRInput.GetUp(OVRInput.RawButton.RIndexTrigger)))
        {
            //Right Hand Wrist
            PlayerPrefs.SetFloat("MaxRightWrist", Mathf.Sin((Mathf.PI * rightHandWrist.transform.eulerAngles.z) / 180));
            //Left Hand Wrist
            PlayerPrefs.SetFloat("MaxLeftWrist", Mathf.Sin((Mathf.PI * leftHandWrist.transform.eulerAngles.z) / 180));

            #region Manus-VR
            //Right Hand
            //PlayerPrefs.SetFloat("MaxRightThumb", rightHandThumb.transform.localEulerAngles.z);
            //PlayerPrefs.SetFloat("MaxRightIndex", rightHandIndex.transform.localEulerAngles.y);
            //PlayerPrefs.SetFloat("MaxRightMiddle", rightHandMid.transform.localEulerAngles.y);
            //PlayerPrefs.SetFloat("MaxRightRing", rightHandRing.transform.localEulerAngles.y);
            //PlayerPrefs.SetFloat("MaxRightPinky", rightHandPinky.transform.localEulerAngles.y);

            //Left Hand
            //PlayerPrefs.SetFloat("MaxLeftThumb", leftHandThumb.transform.localEulerAngles.z);
            //PlayerPrefs.SetFloat("MaxLeftIndex", leftHandIndex.transform.localEulerAngles.y);
            //PlayerPrefs.SetFloat("MaxLeftMiddle", leftHandMid.transform.localEulerAngles.y);
            //PlayerPrefs.SetFloat("MaxLeftRing", leftHandRing.transform.localEulerAngles.y);
            //PlayerPrefs.SetFloat("MaxLeftPinky", leftHandPinky.transform.localEulerAngles.y);
            #endregion

            palmDown.SetActive(false);
            palmUp.SetActive(true);
        }
        else if (palmUp.activeSelf && (Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.KeypadEnter) || OVRInput.GetUp(OVRInput.RawButton.LIndexTrigger) || OVRInput.GetUp(OVRInput.RawButton.RIndexTrigger)))
        {
            PlayerPrefs.SetFloat("MinRightWrist", Mathf.Sin((Mathf.PI * rightHandWrist.transform.eulerAngles.z) / 180));
            //Debug.Log("MAX Right Wrist measurement: " + rightHandWrist.transform.localRotation.y);
            PlayerPrefs.SetFloat("MinLeftWrist", Mathf.Sin((Mathf.PI * leftHandWrist.transform.eulerAngles.z) / 180));
            palmUp.SetActive(false);
            introToTurnOff.SetActive(false);
            ZedCamera.SetActive(true);
            Light.SetActive(true);
            #region Manus-VR
            //fistCalibration.SetActive(true); ***** This is for the use of Manus_VR gloves coming after this *****
            #endregion
        }
        #region Manus-VR
        //else if (fistCalibration.activeSelf && Input.GetKeyUp(KeyCode.Return))
        //{
        //    //Right Hand
        //    //PlayerPrefs.SetFloat("MinRightThumb", rightHandThumb.transform.localEulerAngles.z);
        //    //PlayerPrefs.SetFloat("MinRightIndex", rightHandIndex.transform.localEulerAngles.y);
        //    //PlayerPrefs.SetFloat("MinRightMiddle", rightHandMid.transform.localEulerAngles.y);
        //    //PlayerPrefs.SetFloat("MinRightRing", rightHandRing.transform.localEulerAngles.y);
        //    //PlayerPrefs.SetFloat("MinRightPinky", rightHandPinky.transform.localEulerAngles.y);
        //    //Left Hand
        //    //PlayerPrefs.SetFloat("MinLeftThumb", leftHandThumb.transform.localEulerAngles.z);
        //    //PlayerPrefs.SetFloat("MinLeftIndex", leftHandIndex.transform.localEulerAngles.y);
        //    //PlayerPrefs.SetFloat("MinLeftMiddle", leftHandMid.transform.localEulerAngles.y);
        //    //PlayerPrefs.SetFloat("MinLeftRing", leftHandRing.transform.localEulerAngles.y);
        //    //PlayerPrefs.SetFloat("MinLeftPinky", leftHandPinky.transform.localEulerAngles.y);

        //    introToTurnOff.SetActive(false);
        //    ZedCamera.SetActive(true);
        //    Light.SetActive(true);
        //}
        #endregion
    }

    void ChangePortNumber(InputField input)
    {
        if (mainInputField.text.Length > 0)
            warningMessage.enabled = false;
        PlayerPrefs.SetString("ComPort", input.text);
    }

    void StartCalibration()
    {
        warningMessage.enabled = false;
        if (mainInputField.text.Length > 0)
        {
            commInput.SetActive(false);
            palmDown.SetActive(true);
            start = false;
        }
        else
        {
            warningMessage.enabled = true;
        }
    }
}
