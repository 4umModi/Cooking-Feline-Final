using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using UnityEngine;
//the coroutines are currently not working and im too tired to work on it

public class CommandManager : MonoBehaviour {
    // private static CommandManager _instance;

    // public static CommandManager Instance { get { return _instance; } }

    // private List<ICommand> _commandBuffer = new List<ICommand>();

    // //Make sure there is only one commandmanager before run
    // private void Awake() {
    //     if (_instance != null && _instance != this) {
    //         Destroy(this.gameObject);
    //     } else {
    //         _instance = this;
    //     }
    // }

    // //adds commands to the Command Buffer
    // public void AddCommand (ICommand command) {
    //     _commandBuffer.Add(command);
    // }
    // //plays back all the commands with a 1-second delay
    // public void Play(){
    //     StartCoroutine(PlayRoutine());

    // }
    // //Coroutine for the Play function
    // IEnumerator PlayRoutine() {
    //     Debug.Log("Running PlayRoutine.... ");

    //     foreach(var command in _commandBuffer){
    //         command.Execute();
    //         yield return new WaitforSeconds(1.0f);
    //     }

    //     Debug.Log("Finished PlayRoutine");

    // }
    // //rewinds the comands back to the initial state 
    // public void Rewind () {
    //     StartCoroutine(RewindRoutine());
    // }
    // //Coroutine for the Rewind function
    // IEnumerator RewindRoutine() {
    //     foreach(var command in Enumerable.Reverse(_commandBuffer)){
    //         command.Undo();
    //         yield return new WaitforSeconds(1.0f);
    //     }        
    // }
    // public void Done() {
    //     //test to see if done
    //     Debug.Log("Done");
    // }

    // //empty command buffer
    // public void Reset(){
    //     _commandBuffer.Clear();
    // }
}
