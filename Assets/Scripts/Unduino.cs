using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports;
using MathNet.Numerics.LinearAlgebra;


public class Unduino : MonoBehaviour
{
    #region public variables (exposed in Unity IDE)

    public float HeadTiltSpeed = 1;
    public float HeadTurnSpeed = 1;
    public float HeadUpDownSpeed = 1;
    public GameObject manusHead;
    public Button exitButton;
    public GameObject zedCam;

    #region Manus-VR calibration
    //Right Hand Fingers
    //public GameObject rightHandThumb;
    //public GameObject rightHandIndex;
    //public GameObject rightHandMid;
    //public GameObject rightHandRing;
    //public GameObject rightHandPinky;
    //Left Hand Fingers
    //public GameObject leftHandThumb;
    //public GameObject leftHandIndex;
    //public GameObject leftHandMid;
    //public GameObject leftHandRing;
    //public GameObject leftHandPinky;
    #endregion

    //Right Hand Wrist
    public GameObject rightHandWristRotation;
    //Left Hand Wrist
    public GameObject leftHandWristRotation;

    // init these in Unity, necessary for the calculations
    public GameObject neck;
    public GameObject right_elbow;
    public GameObject right_hand;
    public GameObject left_elbow;
    public GameObject left_hand;
    public GameObject spine_base;

    // integers to add to servos in order to fine tune the motion
    public int rightT1RaiseAdd = 90;
    public int rightT3TwistAdd = 110;
    public int rightT4ElbowAdd = -50;
    public int leftT1RaiseAdd = 65;
    public int leftT3TwistAdd = 110;
    public int leftT4ElbowAdd = -20;

    public int SmallServoMin = 35;
    public int SmallServoMax = 155;
    public bool rightWristDebug = false;
    public int rightWristValue = 35;
    public bool leftWristDebug = false;
    public int leftWristValue = 35;
    public bool rightArmDebug = false;
    public int rightRaise = 90;
    public int rightTwist = 90;
    public int rightTurn = 30;
    public int rightElbow = 30;
    public bool leftArmDebug = false;
    public int leftRaise = 90;
    public int leftTwist = 90;
    public int leftTurn = 30;
    public int leftElbow = 30;
    public bool headDebug = false;
    public int debugYaw = 0;
    public int debugRoll = 0;
    public int debugPitch = 0;
    #endregion

    #region private variable
    SerialPort stream;
    double counter = 0; //in Update(), this counter will enable writing data less frequently to the Arduino

    //Robot length measurments
    private readonly float L1 = 0.145f;
    private readonly float L2 = 0.05f;
    private readonly float L3 = 0.07f;
    private readonly float L4 = 0.2302f; //Upper arm length
    private readonly float L5 = 0.3058f; //Forearm length

    // The MOUTH variable should be changed later when sensor is attached to mouth...
    private int MOUTH = 150;
    private string ConstantAngle = "90";
    Vector3 headRotation;
    // we need the X for rotation of wrist
    int lh_Wrist;
    int rh_Wrist;
    float lh_Wrist_Calib_min;
    float lh_Wrist_Calib_max;
    float rh_Wrist_Calib_min;
    float rh_Wrist_Calib_max;
    float left_prev_raise = 0;
    float left_prev_twist = 0;
    float left_prev_elbow = 0;
    float right_prev_raise = 0;
    float right_prev_twist = 0;
    float right_prev_elbow = 0;
    int pitch;
    int yaw;
    int roll;
    private bool emergencyStop = false;
    private bool paused = true;

    #region Manus-VR
    //int lh_Thumb;
    //int lh_Index;
    //int lh_Mid;
    //int lh_Ring;
    //int lh_Pinky;
    //Thumb we need Z
    //int rh_Thumb;
    //int rh_Index;
    //int rh_Mid;
    //int rh_Ring;
    //int rh_Pinky;
    //float l_thumb_calib_min;
    //float l_thumb_calib_max;
    //float l_index_calib_min;
    //float l_index_calib_max;
    //float l_middle_calib_min;
    //float l_middle_calib_max;
    //float l_ring_calib_min;
    //float l_ring_calib_max;
    //float l_pinky_calib_min;
    //float l_pinky_calib_max;
    //float r_thumb_calib_min;
    //float r_thumb_calib_max;
    //float r_index_calib_min;
    //float r_index_calib_max;
    //float r_middle_calib_min;
    //float r_middle_calib_max;
    //float r_ring_calib_min;
    //float r_ring_calib_max;
    //float r_pinky_calib_min;
    //float r_pinky_calib_max;
    #endregion

    #endregion

    void Start()
    {
        stream = new SerialPort("\\\\.\\COM" + PlayerPrefs.GetString("ComPort"), 115200);
        stream.ReadTimeout = 50;
        if (!stream.IsOpen)
            stream.Open();
        exitButton.onClick.AddListener(delegate { ClosePortsAndQuitElegently(); });

        rh_Wrist_Calib_min = PlayerPrefs.GetFloat("MinRightWrist");
        rh_Wrist_Calib_max = PlayerPrefs.GetFloat("MaxRightWrist");
        lh_Wrist_Calib_min = PlayerPrefs.GetFloat("MinLeftWrist");
        lh_Wrist_Calib_max = PlayerPrefs.GetFloat("MaxLeftWrist");

        #region Manus-VR
        //r_thumb_calib_min = PlayerPrefs.GetFloat("MinRightThumb");
        //r_thumb_calib_max = PlayerPrefs.GetFloat("MaxRightThumb");
        //r_index_calib_min = PlayerPrefs.GetFloat("MinRightIndex");
        //r_index_calib_max = PlayerPrefs.GetFloat("MaxRightIndex");
        //r_middle_calib_min = PlayerPrefs.GetFloat("MinRightMiddle");
        //r_middle_calib_max = PlayerPrefs.GetFloat("MaxRightMiddle");
        //r_ring_calib_min = PlayerPrefs.GetFloat("MinRightRing");
        //r_ring_calib_max = PlayerPrefs.GetFloat("MaxRightRing");
        //r_pinky_calib_min = PlayerPrefs.GetFloat("MinRightPinky");
        //r_pinky_calib_max = PlayerPrefs.GetFloat("MaxRightPinky");

        //l_thumb_calib_min = PlayerPrefs.GetFloat("MinLeftThumb");
        //l_thumb_calib_max = PlayerPrefs.GetFloat("MaxLeftThumb");
        //l_index_calib_min = PlayerPrefs.GetFloat("MinLeftIndex");
        //l_index_calib_max = PlayerPrefs.GetFloat("MaxLeftIndex");
        //l_middle_calib_min = PlayerPrefs.GetFloat("MinLeftMiddle");
        //l_middle_calib_max = PlayerPrefs.GetFloat("MaxLeftMiddle");
        //l_ring_calib_min = PlayerPrefs.GetFloat("MinLeftRing");
        //l_ring_calib_max = PlayerPrefs.GetFloat("MaxLeftRing");
        //l_pinky_calib_min = PlayerPrefs.GetFloat("MinLeftPinky");
        //l_pinky_calib_max = PlayerPrefs.GetFloat("MaxLeftPinky");
        #endregion
    }

    void Update()
    {
        //Head
        headRotation = manusHead.transform.rotation.eulerAngles;
        Vector3 head = manusHead.transform.position;
        //Debug.Log("Head Transform: " + head);
        //Vector3 rightHand = rightHandWristRotation.transform.position;
        //Debug.Log("Yad Transform: " + rightHand);
        pitch = HeadNormalize((int)headRotation.x);
        yaw = HeadNormalize((int)headRotation.y);
        roll = HeadNormalize((int)headRotation.z);
        
        //Left and Right Wrists
        lh_Wrist = NormalizeAngle(Mathf.Sin((Mathf.PI * leftHandWristRotation.transform.eulerAngles.z) / 180), lh_Wrist_Calib_min, lh_Wrist_Calib_max);
        rh_Wrist = NormalizeAngle(Mathf.Sin((Mathf.PI * rightHandWristRotation.transform.eulerAngles.z) / 180), rh_Wrist_Calib_min, rh_Wrist_Calib_max, true);
        if (rightWristDebug)
        {
            rh_Wrist = rightWristValue;
        }
        if (leftWristDebug)
        {
            lh_Wrist = leftWristValue;
        }

        //Debug.Log("RH Wrist Normalized: " + rh_Wrist);
        //Debug.Log("Min Right Wrist measurement: " + rh_Wrist_Calib_min);
        //Debug.Log("MAX Right Wrist measurement: " + rh_Wrist_Calib_max);
        //Debug.Log("Right Hand Rotation: " + Mathf.Sin((Mathf.PI * rightHandWristRotation.transform.eulerAngles.z)/180));


        //Right Hand Fingers
        int rh_index = (int)(OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch) * 120 + 35);
        int rh_thumb = 35;
        if (OVRInput.Get(OVRInput.Touch.One, OVRInput.Controller.RTouch) || OVRInput.Get(OVRInput.Touch.Two, OVRInput.Controller.RTouch) 
            || OVRInput.Get(OVRInput.Touch.PrimaryThumbstick, OVRInput.Controller.RTouch))
        {
            //Debug.Log("THUMB");
            rh_thumb = 155;
        }
        int rh_mid_ring_pinky = (int)(OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.RTouch) * 120 + 35);

        //Left Hand Fingers
        int lh_index = (int)(OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.LTouch) * 120 + 35);
        int lh_thumb = 35;
        if (OVRInput.Get(OVRInput.Touch.One, OVRInput.Controller.LTouch) || OVRInput.Get(OVRInput.Touch.Two, OVRInput.Controller.LTouch) 
            || OVRInput.Get(OVRInput.Touch.PrimaryThumbstick, OVRInput.Controller.LTouch))
        {
            lh_thumb = 155;
        }
        int lh_mid_ring_pinky = (int)(OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.LTouch) * 120 + 35);

        //Left and Right Arms
        Vector<float> right_arm_angles = ComputeAngles(right_hand, right_elbow, true);
        Vector<float> left_arm_angles = ComputeAngles(left_hand, left_elbow, false);
        if(leftArmDebug)
        {
            left_arm_angles[3] = leftRaise;
            left_arm_angles[2] = leftTurn;
            left_arm_angles[1] = leftTwist;
            left_arm_angles[0] = leftElbow;
        }
        if (rightArmDebug)
        {
            right_arm_angles[3] = rightRaise;
            right_arm_angles[2] = rightTurn;
            right_arm_angles[1] = rightTwist;
            right_arm_angles[0] = rightElbow;
        }
        if (headDebug)
        {
            roll = debugRoll;
            yaw = debugYaw;
            pitch = debugPitch;
        }

        //Pause the robot and transmit all defaults
        if (paused)
        {
            headDebug = true;
            rightArmDebug = true;
            leftArmDebug = true;
            leftWristDebug = true;
            rightWristDebug = true;
            rh_index = 35;
            rh_thumb = 35;
            rh_mid_ring_pinky = 35;
            lh_index = 35;
            lh_thumb = 35;
            lh_mid_ring_pinky = 35;
        }

        if (OVRInput.GetUp(OVRInput.Button.One, OVRInput.Controller.RTouch) || OVRInput.GetUp(OVRInput.Button.One, OVRInput.Controller.LTouch) ||
            OVRInput.GetUp(OVRInput.Button.Two, OVRInput.Controller.RTouch) || OVRInput.GetUp(OVRInput.Button.Two, OVRInput.Controller.LTouch))
        {
            paused = !paused; 
            if (!paused)
            {
                headDebug = false;
                rightArmDebug = false;
                leftArmDebug = false;
                leftWristDebug = false;
                rightWristDebug = false;
            }
        }

        #region Electric cutoff for future implementation
        //Emergency Stop
        //if (OVRInput.GetUp(OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.RTouch) || OVRInput.GetUp(OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.LTouch))
        //{
        //    emergencyStop = !emergencyStop;
        //}
        #endregion

        //Debug.Log("Elbow: " + right_hand_angles[0]);
        //Debug.Log("Twist: " + right_hand_angles[1]);
        //Debug.Log("Turn: " + right_hand_angles[2]);
        //Debug.Log("Raise: " + right_hand_angles[3]);

        #region Manus-VR
        //Left Hand
        //Thumb we need Z
        //lh_Thumb = NormalizeAngle(leftHandThumb.transform.localEulerAngles.z, l_thumb_calib_min, l_thumb_calib_max, false);
        //lh_Index = NormalizeAngle(leftHandIndex.transform.localEulerAngles.y, l_index_calib_min, l_index_calib_max, false);
        //lh_Mid = NormalizeAngle(leftHandMid.transform.localEulerAngles.y, l_middle_calib_min, l_middle_calib_max, false);
        //lh_Ring = NormalizeAngle(leftHandRing.transform.localEulerAngles.y, l_ring_calib_min, l_ring_calib_max, false);
        //lh_Pinky = NormalizeAngle(leftHandPinky.transform.localEulerAngles.y, l_pinky_calib_min, l_pinky_calib_max, false);

        //Right Hand
        //Thumb we need Z
        //rh_Thumb = NormalizeAngle(rightHandThumb.transform.localEulerAngles.z, r_thumb_calib_min, r_thumb_calib_max, true);
        //rh_Index = NormalizeAngle(rightHandIndex.transform.localEulerAngles.y, r_index_calib_min, r_index_calib_max, true);
        //rh_Mid = NormalizeAngle(rightHandMid.transform.localEulerAngles.y, r_middle_calib_min, r_middle_calib_max, true);
        //rh_Ring = NormalizeAngle(rightHandRing.transform.localEulerAngles.y, r_ring_calib_min, r_ring_calib_max, true);
        //rh_Pinky = NormalizeAngle(rightHandPinky.transform.localEulerAngles.y, r_pinky_calib_min, r_pinky_calib_max, true);
        #endregion

        if ((counter++) % 4 == 0 && stream.IsOpen)
        {
            stream.WriteLine(
            "9022" + "," + left_arm_angles[3].ToString() + "," +
            "9023" + "," + left_arm_angles[2].ToString() + "," +
            "9024" + "," + left_arm_angles[1].ToString() + "," +
            "9025" + "," + left_arm_angles[0].ToString() + "," +
            "9026" + "," + right_arm_angles[3].ToString() + "," +
            "9027" + "," + right_arm_angles[2].ToString() + "," +
            "9028" + "," + right_arm_angles[1].ToString() + "," +
            "9029" + "," + right_arm_angles[0].ToString() + "," +
            "9030" + "," + lh_Wrist.ToString() + "," +
            "9031" + "," + lh_mid_ring_pinky.ToString() + "," +
            "9032" + "," + lh_mid_ring_pinky.ToString() + "," +
            "9033" + "," + lh_mid_ring_pinky.ToString() + "," +
            "9034" + "," + lh_index.ToString() + "," +
            "9035" + "," + lh_thumb.ToString() + "," +
            "9036" + "," + rh_Wrist.ToString() + "," +
            "9037" + "," + rh_mid_ring_pinky.ToString() + "," +
            "9038" + "," + rh_mid_ring_pinky.ToString() + "," +
            "9039" + "," + rh_mid_ring_pinky.ToString() + "," +
            "9040" + "," + rh_index.ToString() + "," +
            "9041" + "," + rh_thumb.ToString() + "," +
            "9042" + "," + (90 - HeadTiltSpeed * roll).ToString() + "," +
            "9043" + "," + (90 + HeadTiltSpeed * roll).ToString() + "," +
            "9044" + "," + (100 - HeadTurnSpeed * yaw).ToString() + "," +
            "9045" + "," + (110 - HeadUpDownSpeed * pitch).ToString());

            #region Electric Cutoff for future implementation

            //if (emergencyStop)
            //{
            //    stream.WriteLine(
            //    "9020" + "," + 
            //    "9022" + "," + left_arm_angles[3].ToString() + "," +
            //    "9023" + "," + left_arm_angles[2].ToString() + "," +
            //    "9024" + "," + left_arm_angles[1].ToString() + "," +
            //    "9025" + "," + left_arm_angles[0].ToString() + "," +
            //    "9026" + "," + right_arm_angles[3].ToString() + "," +
            //    "9027" + "," + right_arm_angles[2].ToString() + "," +
            //    "9028" + "," + right_arm_angles[1].ToString() + "," +
            //    "9029" + "," + right_arm_angles[0].ToString() + "," +
            //    "9030" + "," + lh_Wrist.ToString() + "," +
            //    "9031" + "," + lh_mid_ring_pinky.ToString() + "," +
            //    "9032" + "," + lh_mid_ring_pinky.ToString() + "," +
            //    "9033" + "," + lh_mid_ring_pinky.ToString() + "," +
            //    "9034" + "," + lh_index.ToString() + "," +
            //    "9035" + "," + lh_thumb.ToString() + "," +
            //    "9036" + "," + rh_Wrist.ToString() + "," +
            //    "9037" + "," + rh_mid_ring_pinky.ToString() + "," +
            //    "9038" + "," + rh_mid_ring_pinky.ToString() + "," +
            //    "9039" + "," + rh_mid_ring_pinky.ToString() + "," +
            //    "9040" + "," + rh_index.ToString() + "," +
            //    "9041" + "," + rh_thumb.ToString() + "," +
            //    "9042" + "," + (90 - HeadTiltSpeed * roll).ToString() + "," +
            //    "9043" + "," + (90 + HeadTiltSpeed * roll).ToString() + "," +
            //    "9044" + "," + (100 - HeadTurnSpeed * yaw).ToString() + "," +
            //    "9045" + "," + (110 - HeadUpDownSpeed * pitch).ToString());
            //}
            //else
            //{
            //    stream.WriteLine(
            //        "9021" + "," + 
            //        "9022" + "," + left_arm_angles[3].ToString() + "," +
            //        "9023" + "," + left_arm_angles[2].ToString() + "," +
            //        "9024" + "," + left_arm_angles[1].ToString() + "," +
            //        "9025" + "," + left_arm_angles[0].ToString() + "," +
            //        "9026" + "," + right_arm_angles[3].ToString() + "," +
            //        "9027" + "," + right_arm_angles[2].ToString() + "," +
            //        "9028" + "," + right_arm_angles[1].ToString() + "," +
            //        "9029" + "," + right_arm_angles[0].ToString() + "," +
            //        "9030" + "," + lh_Wrist.ToString() + "," +
            //        "9031" + "," + lh_mid_ring_pinky.ToString() + "," +
            //        "9032" + "," + lh_mid_ring_pinky.ToString() + "," +
            //        "9033" + "," + lh_mid_ring_pinky.ToString() + "," +
            //        "9034" + "," + lh_index.ToString() + "," +
            //        "9035" + "," + lh_thumb.ToString() + "," +
            //        "9036" + "," + rh_Wrist.ToString() + "," +
            //        "9037" + "," + rh_mid_ring_pinky.ToString() + "," +
            //        "9038" + "," + rh_mid_ring_pinky.ToString() + "," +
            //        "9039" + "," + rh_mid_ring_pinky.ToString() + "," +
            //        "9040" + "," + rh_index.ToString() + "," +
            //        "9041" + "," + rh_thumb.ToString() + "," +
            //        "9042" + "," + (90 - HeadTiltSpeed * roll).ToString() + "," +
            //        "9043" + "," + (90 + HeadTiltSpeed * roll).ToString() + "," +
            //        "9044" + "," + (100 - HeadTurnSpeed * yaw).ToString() + "," +
            //        "9045" + "," + (110 - HeadUpDownSpeed * pitch).ToString());
            //}
            #endregion
        }
        #region Arduino values range

        //    Base_input = [
        //90,...  % Elbow Right - input(1) - (0 - 80) | 0 = Right completely spred
        //0,...  % Twist Right - input(2) - (0 - 180)((-90) - 90 deg) | 0 = Right Baisepse completley in
        //0,...  % Turn Right - input(3) - (forward)(0 - 180)(0 - (-180) deg) | 0 = Right Hand completly down
        //0,... % Raise Right - input(4) - (wings)(90 - 165)(5 - 80 deg) | 100 = Right shoulder("Wing") completly closed
        //10,...  % Elbow left - input(5) - (0 - 85) | 0 = Left completely spred
        //90,...  % Twist left - input(6) - (0 - 180)((-90) - (90)) | 0 = Left Baisepse completley in
        //40,...  % Turn left - input(7) - (forward) - (0 - 140) | 0 = Left Hand completly down
        //90,... % Raise left - input(8) - (wings)(80 - 160)(5 - 80 deg) | 100 = Left shoulder("Wing") completly closed
        //150,... % wrist Left - input(9) - (0 - 180) | 155 = Left wrist(palm) completly out (thumb out)
        //35,... % (35 - 155) - input(10) -
        //35,... % fingers Left - input(11) -
        // 35,... % fingers Left - input(12)
        //35,... % fingers Left - input(13)
        //35,...  % fingers Left - input(14)
        //50,... % wrist Right - input(15) - (35 - 155) | 155 = Right wrist(palm) completly out (thumb out)
        //155,... % (155 - 35) - input(16)
        //155,... % fingers Right - input(17)
        //155,...  % fingers Right - input(18)
        //155,...  % fingers Right - input(19)
        //155,...  % fingers Right - input(20)
        //60 % Tilt - input(21) - (Body side - to - side) (35 - 75) | 70 = body sid - to - sde completly to the left
        //];
        //% 90,...  % neck side - input(21)
        //  % 60,...  % Head Up Down  -input(23) - (60 - 175)
        #endregion
    }

    Vector<float> ComputeAngles(GameObject hand, GameObject elbow, bool is_right_hand)
    {
        Vector3 middle_chest_position = neck.transform.position;
        middle_chest_position.y -= 0.145f;
        //Debug.Log("Middle Chest: " + middle_chest_position.ToString("F4"));
        //Debug.Log("Elbow: " + elbow.transform.position.ToString("F4"));
        //Debug.Log("Hand: " + hand.transform.position.ToString("F4"));
        var elbowToChest = elbow.transform.position - middle_chest_position;
        var handToElbow = hand.transform.position - elbow.transform.position;
        //Debug.Log("Elbow To Chest: " + elbowToChest.ToString("F4"));
        //Debug.Log("Hand To Elbow: " + handToElbow.ToString("F4"));

        //Calculating t2
        float t2 = 0f;
        var s2 = (Mathf.Min(Mathf.Abs(elbowToChest.z), L4) / L4);
        var c2 = Mathf.Sqrt(1 - Mathf.Pow(s2, 2));
        t2 = Mathf.Atan2(s2, c2);

        #region old code
        /*
        if (is_right_hand)
        {
            var s2 = (Mathf.Min(Mathf.Abs(elbowToChest.z), L4) / L4);
            var c2 = Mathf.Sqrt(1 - Mathf.Pow(s2, 2));
            t2 = Mathf.Atan2(s2, c2);

            //Debug.Log("Elbow to Chest X: " + elbowToChest.x);
            //Debug.Log("Elbow to Chest Y: " + elbowToChest.y);
            //Debug.Log("Elbow to Chest Z: " + elbowToChest.z);
        }
        else
        {
            t2 = Mathf.Asin(Mathf.Min(Mathf.Abs(elbowToChest.z), L4) / L4);
        }
        */
        #endregion  

        if (elbowToChest.y > 0)
        {
            t2 = Mathf.PI - t2;
        }
        t2 = Mathf.Max(t2, 0);

        //Calculating t1
        var element1 = L2 + L4 * Mathf.Cos(t2);
        var element2 = L3;
        if (is_right_hand) element2 = -L3;
        var element3 = -L3;
        if (is_right_hand) element3 = L3;
        var A = Matrix<float>.Build.Dense(2, 2);
        A.SetRow(0, new[] { element1, element2 });
        A.SetRow(1, new[] { element3, element1 });
        //Debug.Log("Matrix A: " + A);
        Vector<float> B = null;
        if (is_right_hand)
        {
            B = Vector<float>.Build.Dense(new[] { elbowToChest.y, elbowToChest.x - L1 });
        }
        else
        {
            B = Vector<float>.Build.Dense(new[] { -elbowToChest.y, elbowToChest.x + L1 });
        }
        var solution = A.Transpose().Solve(B);
        //Debug.Log("B Vec: " + B);
        //Debug.Log("Solution Vec: " + solution);

        var t1 = Mathf.Atan2(solution.At(1), solution.At(0));
        if (is_right_hand)
        {
            if (t1 < 0)
            {
                t1 = 0;
            }
            else
            {
                t1 = Mathf.PI - t1;
            }
        }
        else
        {
            if (t1 > 0)
            {
                t1 = 0;
            }
            else
            {
                t1 = - t1;
            }
        }
        
        //Calculating t3
        var t3 = Mathf.Atan2(handToElbow.x, handToElbow.z);
        t3 = t3 * 180 / Mathf.PI;
        //Calculating t4
        var t4 = Mathf.Atan2( ( Mathf.Sqrt(Mathf.Pow(handToElbow.x, 2) + Mathf.Pow(handToElbow.z, 2) ) / L5) , -(handToElbow.y/L5));
        t4 = t4 * 180 / Mathf.PI;

        if (is_right_hand) t1 = t1 * 180 / Mathf.PI;
        else t1 = (t1 * 180 / Mathf.PI);
        t2 = t2 * 180 / Mathf.PI;
        
        float elbow_angle, twist, turn, raise;

        turn = t2;
        if (t2 < 10) turn = 10;
        else if (t2 > 160) turn = 160;

        if (is_right_hand)
        {
            if (t1 > 10 && t1 < 75)
            {
                raise = rightT1RaiseAdd + t1;
                right_prev_raise = raise;
            }
            else if (t1 >= 75)
            {
                raise = 160;
                right_prev_raise = raise;
            }
            else raise = right_prev_raise;

            if (t3 > -75 && t3 < 75)
            {
                twist = rightT3TwistAdd + t3;
                right_prev_twist = twist;
            }
            else if (t3 >= 75)
            {
                twist = 170;
                right_prev_twist = twist;
            }
            else twist = right_prev_twist;

            if (t4 > 20 && t4 < 120)
            {
                elbow_angle = rightT4ElbowAdd + t4;
                right_prev_elbow = elbow_angle;
            }
            else if (t4 >= 120)
            {
                elbow_angle = 85;
                right_prev_elbow = elbow_angle;
            }
            else elbow_angle = right_prev_elbow;

        }
        else
        {
            t3 = -t3;
            if (t1 > 10 && t1 < 95)
            {
                raise = leftT1RaiseAdd + t1;
                left_prev_raise = raise;
            }
            else if (t1 >= 95)
            {
                raise = 160;
                left_prev_raise = raise;
            }
            else raise = left_prev_raise;
            ;

            if (t3 > -75 && t3 < 75)
            {
                twist = leftT3TwistAdd + t3;
                left_prev_twist = twist;
            }
            else if (t3 >= 75)
            {
                twist = 170;
                left_prev_twist = twist;
            }
            else twist = left_prev_twist;

            if (t4 > 20 && t4 < 110)
            {
                elbow_angle = leftT4ElbowAdd + t4;
                left_prev_elbow = elbow_angle;
            }
            else if (t4 >= 110)
            {
                elbow_angle = 90;
                left_prev_elbow = elbow_angle;
            }
            else elbow_angle = left_prev_elbow;

            //Debug.Log("T1: " + raise);
            //Debug.Log("T2: " + turn);
            //Debug.Log("T3: " + twist);
            //Debug.Log("T4: " + elbow_angle);
            
        }

        return Vector<float>.Build.Dense(new[] { elbow_angle, twist, turn, raise });
    }

    int NormalizeAngle(float angle, float calibrationMinValue, float calibrationMaxValue, bool isRightHand = false)
    {
        int normAngle = (int)(SmallServoMin + ((angle - calibrationMinValue) / (calibrationMaxValue - calibrationMinValue)) * (SmallServoMax - SmallServoMin)); ;
        if (isRightHand)
        {
            return SmallServoMax + SmallServoMin - normAngle;
        }
        return normAngle;
    }

    int HeadNormalize(int angle)
    {
        return (angle > 180) ? angle - 360 : angle;
    }

    void ClosePortsAndQuitElegently()
    {
        if (stream.IsOpen)
        {
            stream.Close();
        }
        Application.Quit();
    }
}
