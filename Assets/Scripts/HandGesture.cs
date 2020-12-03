using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;

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
    public RecipeManager manager; // use to call methods from RecipleManager
   
    void Awake() 
    {
        manager = GameObject.Find("Manager").GetComponent<RecipeManager>(); // define manager object
    }
   
    // Start is called before the first frame update
    void Start()
    {
        //creates controller for leap
        controller = new Controller();
        //initializes frame buffer, flag, count
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

        //initialize variables for various gestures
        bool grab = false;
        bool pinch = false;
        bool swipe = false;
        bool scoop = false;

        //finds handspeed vector from palm velocity
        Vector handSpeed = hand.PalmVelocity;

        //gets horizontal and vertical speed of hand
        float handSpeed_x = handSpeed[0];
        float handSpeed_y = handSpeed[1];

        float grabAngle = hand.GrabAngle; //0 radian for open hand, pi radian when tight fist
        float grabStrength = hand.GrabStrength; //0 for open hand, 1 for grabbing hang pose
        float pinchDistance = hand.PinchDistance; //distance between thumb and index tips

        //list of fingers
        List<Finger> fingers = hand.Fingers;

        //Grab detection - if the grab strength/angle are within a threshold and all fingers are not extended, grab is true
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


        //Pinch detection - if the pinch distance is within a threshold and 3 fingers are extended, pinch is true
        if (pinchDistance < 30.0) //need to check y distance is minimal
        {
            int extend_num = 0;
            foreach (Finger finger in fingers)
            {

                if (finger.IsExtended == true) extend_num++; //checks number of extended fingers

            }
            if (extend_num == 3) pinch = true; //if 3 fingers
        }

        //Swipe Detection - if the horizontal hand speed is more than the threshold and all fingers are extended, swipe is true
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

        // Scoop detection if the vertical hand speed is more than the threshold and all fingers are extended, scoop is true
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

        //returns current gesture as a string
        if (grab) return ("grab");
        if (pinch && !grab) return ("pinch");
        if (swipe) return ("swipe");
        if (scoop) return ("scoop");
        return "none";
    }

    // Check for existence of hands and detect gestures
    string CheckHands(Frame frame)
    {

        //gets list of hands
        List<Hand> hands = frame.Hands;
        //gets number of hands
        int num_hands = hands.Count;

        //creates bools for left/right hand existance
        bool leftHandExist = false;
        bool rightHandExist = false;

        //if no hands over leap, return 
        if (num_hands < 1) return "none";

        //creates new hand variables
        Hand rightHand = new Hand();
        Hand leftHand = new Hand();

        //if there is only one hand, set corresponding hand to true and set hand object equal to corresponding hand
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

        //if there are two hands, set both hands to true and set hand objects equal to corresponding hands
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

        //calls HandCalc for each hand over leap (returns gesture string)
        if (leftHandExist && !rightHandExist) return HandCalc(leftHand);
        if (rightHandExist && !leftHandExist) return HandCalc(rightHand);

        //if both hands are in view and are both are doing gestures, then return two gestures, else return gesture being done
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
        //gets frame from controller
        Frame frame = controller.Frame();

        //gets current gesture from checkhands(frame)
        string gesture = CheckHands(frame);

        AddList(gesture); // add output to buffer

        //checks all previous frames if any of the previous frames are not a gesture,
        //then a gesture has been detected
        if (gesture != "none" && frame_buffer.Count > 1)
        {
            for (int i=frame_buffer.Count-1; i>=0; i--)
            {
                if (frame_buffer[i] == "none") 
                {
                    Debug.Log(gesture); // print gesture to console
                    manager.GestureDetected(gesture);// send message that gesture was detected
                    resetArray(gesture); //reset array to be filled with gesture string so no duplicated
                    return;
                }
                if (i == 0)
                {
                    manager.GestureDetected("none");
                    return;
                }
            }
        }
        manager.GestureDetected("none");
        return;
    }

    //resets Array to be filled with gesture so there are no duplicates
    void resetArray(string gesture) 
    {
        for (int i=frame_buffer.Count-1; i>=0; i--) 
        {
            frame_buffer[i] = gesture;
        }
    }
}
