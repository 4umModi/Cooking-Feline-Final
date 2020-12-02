using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Leap;
using Leap.Unity;

[System.Serializable] public class _UnityEventString:UnityEvent<string> {} // create a class for gesture event with string argument

public class HandGesture : MonoBehaviour
{
    //list documentation https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1?view=net-5.0
    //based off of https://developer-archive.leapmotion.com/documentation/csharp/api/Leap_Classes.html

    Controller controller; //gets the controller
    int MAX_FRAMES = 100; // max number of frame buffer objects
    int FRAME_LOOKBACK = 30; // how far we look back for gesture change
    int count; // number of objects in queue
    int buffer_flag; //flag for once buffer is full
    List<string> frame_buffer; // buffer to hold past frames
    [Header("This is an event for gesture detection!")]
     public _UnityEventString gestureDetected; // UnityEvent for gesture detection

    // Start is called before the first frame update
    void Start()
    {
        controller = new Controller();
        frame_buffer = new List<string>();
        buffer_flag = 0;
        count = 0;
    }

    // Manage adding to frames queue and maintaining length
    void AddList(string gesture)
    {
        frame_buffer.Add(gesture);
        //Debug.Log("Added " + gesture.ToString());
        if (buffer_flag == 1) frame_buffer.RemoveAt(0);
        count++;
        if (count == MAX_FRAMES)
        {
            count = 0;
            buffer_flag = 1;
        }

    }

    // Detect gestures and print log
    string HandCalc(Hand hand)
    {
        //hand attributes https://developer-archive.leapmotion.com/documentation/csharp/api/gen-csharp/class_leap_1_1_hand.html#a8b5ee4774fb086f34c3df3d8a5d67284

        //TYPE_THUMB = 0 ;
        //TYPE_INDEX = 1;
        //TYPE_MIDDLE = 2 
        //TYPE_RING = 3 
        //TYPE_PINKY = 4 
        //TYPE_UNKNOWN = -1 

        //Finger index = new Finger();
        //Finger thumb = new Finger();
        bool grab = false;
        bool pinch = false;
        bool swipe = false;
        bool scoop = false;

        Vector handSpeed = hand.PalmVelocity;
        float handSpeed_x = handSpeed[0];
        float handSpeed_y = handSpeed[1];

        float grabAngle = hand.GrabAngle; //0 radian for open hand, pi radian when tight fist
        float grabStrength = hand.GrabStrength; //0 for open hand, 1 for grabbing hang pose
        float pinchDistance = hand.PinchDistance; //distance between thumb and index tips

        List<Finger> fingers = hand.Fingers;

        //Grab detection
        if (grabStrength > 0.9 && grabAngle > 3)
        {
            grab = true;
            foreach (Finger finger in fingers)
            {
                if (finger.IsExtended == true) //checks if all fingers are not extended
                {
                    grab = false;
                    break;
                }
            }
        }


        //Pinch detection
        if (pinchDistance < 30.0) //need to check y distance is minimal
        {
            int extend_num = 0;
            foreach (Finger finger in fingers)
            {

                if (finger.IsExtended == true) extend_num++; //checks number of extended fingers

            }
            if (extend_num == 3) pinch = true; //if 3 fingers
        }

        //Swipe Detection
        if (handSpeed_x > 350)
        {
            swipe = true;
            foreach (Finger finger in fingers)
            {
                if (finger.IsExtended == false)
                {
                    swipe = false;
                    break;
                }
            }
        }

        // Scoop detection
        if (handSpeed_y < -350) // check if hand has high enough downward velocity
        {
            scoop = true;
            foreach (Finger finger in fingers) // make sure hand is flat with extended fingers
            {
                if (finger.IsExtended == false) 
                {
                    scoop = false;
                    break;
                }
            }
        }
        if (grab) return ("grab gesture");
        if (pinch && !grab) return ("pinch gesture");
        if (swipe) return ("swipe gesture");
        if (scoop) return ("scoop gesture");
        return "none";
    }

    // Check for existence of hands and detect gestures
    string CheckHands(Frame frame)
    {
        List<Hand> hands = frame.Hands;
        int num_hands = hands.Count;
        bool leftHandExist = false;
        bool rightHandExist = false;

        if (num_hands < 1) return "none";

        Hand rightHand = new Hand();
        Hand leftHand = new Hand();

        if (num_hands == 1)
        {

            if (hands[0].IsRight)
            {
                rightHand = hands[0];
                rightHandExist = true;
            }
            else
            {
                leftHand = hands[0];
                leftHandExist = true;
            }
        }

        if (num_hands == 2)
        {

            leftHandExist = true;
            rightHandExist = true;

            if (hands[0].IsRight)
            {
                rightHand = hands[0];
                leftHand = hands[1];
            }
            else
            {
                rightHand = hands[1];
                leftHand = hands[0];
            }
        }

        if (leftHandExist && !rightHandExist) return HandCalc(leftHand);
        if (rightHandExist && !leftHandExist) return HandCalc(rightHand);
        if (leftHandExist && rightHandExist)
        {
            string leftGesture = HandCalc(leftHand);
            string rightGesture = HandCalc(rightHand);

            if (leftGesture == "none") return rightGesture;
            if (rightGesture == "none") return leftGesture;
            if (rightGesture == leftGesture) return rightGesture;
            return "Two Gestures";

        }
        return "none";
    }

    // Update is called once per frame
    void Update()
    {
        Frame frame = controller.Frame();
        string gesture = CheckHands(frame);
        AddList(gesture); // add output to buffer

        //checks all previous frames if any of the previous frames are not a gesture,
        //then a gesture has been detected
        // IDEA: what if we change to see if the same gesture exists (previous scoop/swipe etc)
        if (gesture != "none" && frame_buffer.Count > 10)
        {
            for (int i=frame_buffer.Count-1; i>=0; i--)
            {
                if (frame_buffer[i] == "none") 
                {
                    Debug.Log(gesture); // print gesture to console
                    gestureDetected.Invoke(gesture); // call event
                    return;
                }
                if (i == 0) return;
            }
        }

    }
}
