using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecuteAction
{
  private string extractActionParameters(string actionInstruction)
  {
    return actionInstruction.Split(",")[1].Substring(0,actionInstruction.Length-2);
  }

  private string getMoveToId(string moveToInstruction)
  {
    return moveToInstruction.Split("_")[1];
  }

  private bool classifyActionsAndExecute(string actionInstruction)
  {
    if (actionInstruction.Contains("moveto"))
    {
      string parameter = extractActionParameters(actionInstruction);
      string moveToId = getMoveToId(parameter);
      Vector3 moveToPosition = GameObject.Find(moveToId).transform.position;
      // TODO: Move to given position
      return true;
    }
    else if (actionInstruction.Contains("do"))
    {
      string parameter = extractActionParameters(actionInstruction);
      if (parameter.Equals("capture_image"))
      {
        // TODO:capture image
        return true;
      }
      else if (parameter.Equals("take_off"))
      {
        // TODO: take_off
        return true;
      }
    }

    return false;
  }
}
