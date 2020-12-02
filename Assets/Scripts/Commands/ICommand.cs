using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICommand {
    //commands must do something
    void Execute();
    //commands must be reversibe
    void Undo();
} 
