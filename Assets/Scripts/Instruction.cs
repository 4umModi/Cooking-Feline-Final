using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//System.Serializable allows these to be created from the unity inspector
[System.Serializable]

public class Instruction : ICommand {
    //a toggle is a button w/ a box
    public Toggle toggle;
    //this says the Toggle is 'off' / unchecked at the start

    //What number step is this?
    public int stepNumber;
    //what is the basic instruction?
    public string instructiontext;
    public string gestureNeeded;
    public TextMeshProUGUI stepDisplay;

    //command from the ICommand Interface
    public void Execute(){
        Debug.Log("Executing!");
        toggle.isOn = true;
        stepNumber++;
    } 

    //command from the ICommand Interface
    public void Undo(){
        Debug.Log("Undoing!");
        toggle.isOn = false;
        stepNumber--;
    } 

}

