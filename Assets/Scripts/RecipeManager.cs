using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class RecipeManager : MonoBehaviour {


    private static RecipeManager _instance;

    public static RecipeManager Instance { get { return _instance; } }

    private List<ICommand> _commandBuffer = new List<ICommand>();
    //private List<Instruction> _instructionBuffer = new List<Instruction>();

    [Header("The middle display text")]
    public TextMeshProUGUI centerText;

    [Header("Add new Instruction Steps here")]
    [Tooltip("An 'Instruction' notes if the step has been completed, has the step number, and the text to be displayed")]
    public Instruction [] InstructionSteps;

    //What step we are on
    private int currentStep = 0;

    private void Awake() {
        Debug.Log("Awakening");
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

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

    //adds commands to the Command Buffer
    // public void AddCommand (ICommand command) {
    //     _commandBuffer.Add(command);
    //     //_instructionBuffer.Add(command);
    //     Debug.Log("ADD command - ");
    // }

    public void NextStep(){
        //call the execute method of the current step
        InstructionSteps[currentStep].Execute();
        //AddCommand(InstructionSteps[currentStep].Execute());
        if (currentStep < InstructionSteps.Length-1)
        {
            currentStep++;
            centerText.text = InstructionSteps[currentStep].instructiontext;
        }
    }

    public void PreviousStep(){
        InstructionSteps[currentStep].Undo();
        if (currentStep != 0)
        {
            currentStep--;
            centerText.text = InstructionSteps[currentStep].instructiontext;
        }
    }

    // GO FORWARD - move forward one step in the recipe
    // public void GoForward() {
    // 	Debug.Log("Going forward - start");
    //     StartCoroutine(GoForwardRoutine());
    //     Debug.Log("Going forward - end");

    // }

    // GO FORWARD ROUTINE - executes the command
    // IEnumerator GoForwardRoutine() {
    //     Debug.Log("Running GoForwardRoutine.... ");
    //     foreach(var command in _commandBuffer){ // TODO: Replace _commandBuffer with _instructionBuffer
    //     	Debug.Log("Executing " + command);
    //         command.Execute();
    //         yield return new WaitForSeconds(1.0f);
    //     }
    //     Debug.Log("Finished GoForwardRoutine");
    // }

    // // GO BACKWARD - moves bakcward one step in the recipe
    // public void GoBackward () {
    // 	Debug.Log("Going backward - start");
    //     StartCoroutine(GoBackwardRoutine());
    //     Debug.Log("Going backward - end");
    // }
    
    // // GO BACKWARD ROUTINE - executes the command
    // IEnumerator GoBackwardRoutine() {
    // 	Debug.Log("Running GoBackwardRoutine.... ");
    //     foreach(var command in Enumerable.Reverse(_commandBuffer)){ // TODO: Replace _commandBuffer with _instructionBuffer
    //     	Debug.Log("Undoing " + command);
    //         command.Undo();
    //         yield return new WaitForSeconds(1.0f);
    //     }     
    //     Debug.Log("Finished GoBackwardRoutine");   
    // }
    
    /*
    public void Done() {
        var cubes = GameObject.FindGameObjectsWithTag("cube");
        foreach(var cube in cubes){
           cube.GetComponent<MeshRenderer>().material.color = Color.white;
           Debug.Log("Done");
        }
    }
    */

    
    public void Reset(){
        _commandBuffer.Clear();
        //_instructionBuffer.Clear();
    }
}

