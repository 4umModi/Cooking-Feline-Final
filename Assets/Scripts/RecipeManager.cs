using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class RecipeManager : MonoBehaviour {

    //create RecipeManager instance
    private static RecipeManager _instance;

    //gets instance
    public static RecipeManager Instance { get { return _instance; } }

    //gets list of commands
    private List<ICommand> _commandBuffer = new List<ICommand>();

    //text variable (we can fill in inspector)
    [Header("The middle display text")]
    public TextMeshProUGUI centerText;

    //instruction object (we can fill in inspector when dragging this script on an object)
    [Header("Add new Instruction Steps here")]
    [Tooltip("An 'Instruction' notes if the step has been completed, has the step number, and the text to be displayed")]
    public Instruction [] InstructionSteps;


    //instruction object variables (we can fill in inspector when dragging this script on an object)
   
    //What step we are on
    private int currentStep = 0;
    //needed for playing voiceover audio
    private int stepNumAudio = 0;
    private string step = "";

    //before first frame, resets instance
    private void Awake() {
        Debug.Log("Awakening");
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    // Start is called before the first frame update
    void Start () {
        Debug.Log("Starting manager!");

        //goes through the written instructions
        for(int i = 0; i < InstructionSteps.Length; i++){
            //assigns the string to the TMP gui 
            //I added no checks to this aside from the checks of creation.

            //If an instruction has no text, do not display it in the space
            if(InstructionSteps[i].instructiontext == ""){
                //Debug.Log(InstructionSteps[i].instructiontext);
                InstructionSteps[i].toggle.gameObject.SetActive(false);
            }
            //otherwise display it as steps
            InstructionSteps[i].stepDisplay.text = InstructionSteps[i].instructiontext;
        }
        //set the center text to the first instruction
        centerText.text = InstructionSteps[0].instructiontext;
    }

    //calls next recipe step
    public void NextStep(){
        //call the execute method of the current step
        InstructionSteps[currentStep].Execute();
        //AddCommand(InstructionSteps[currentStep].Execute());
        if (currentStep < InstructionSteps.Length - 1)
        {
            currentStep++;
            centerText.text = InstructionSteps[currentStep].instructiontext;

            //play audio voiceover file for the current step
            stepNumAudio = currentStep + 1; //need increment bc step 1 is stored @ index 0
            step = "step" + stepNumAudio;
            FindObjectOfType<AudioManager>().Play(step);
        }
        //if user is on last step
        else
        {
            FindObjectOfType<AudioManager>().StopPlaying("cookingMusic"); // stop playing cooking music
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    //sets to previous recipe step
    public void PreviousStep(){
        InstructionSteps[currentStep].Undo();
        if (currentStep != 0)
        {
            currentStep--;
            centerText.text = InstructionSteps[currentStep].instructiontext;

            //play audio voiceover file for the current step
            stepNumAudio = currentStep+1; //need increment bc step 1 is stored @ index 0
            step = "step" + stepNumAudio;
            FindObjectOfType<AudioManager>().Play(step);
        }
    }

    // checks if gesture is correct and moves to next step
    public void GestureDetected(string gesture){

        if (gesture == InstructionSteps[currentStep].gestureNeeded)
        {
            this.NextStep(); // call for next step
        }

    }

    //resets buffer
    public void Reset(){
        _commandBuffer.Clear();
    }
}

