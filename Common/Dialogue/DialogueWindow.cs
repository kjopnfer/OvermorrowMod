namespace OvermorrowMod.Common.Dialogue
{
    public class DialogueWindow
    {
        // Choices can be optional, if none specified: they will close the dialogue window
        // Dialogue nodes should have an onClose method or something to assign quests or do other actions

        /*new DialogueNode(
              "start",
              new Text[]
              {
                  new Text("text", drawTime, holdTime),
                  new Text("text", drawTime, holdTime),
              }
          ),
          new DialogueNode(
              "second",
              new Text[]
              {
                  new Text("text", drawTime, holdTime),
                  new Text("text", drawTime, holdTime),
              }
          ),
          new DialogueNode(
              "third",
              new Text[]
              {
                  new Text("text", drawTime, holdTime),
                  new Text("text", drawTime, holdTime),
              }
          ),
        */
        // Needs to be able to take a Texture to represent the NPC

        // Text must specify the draw time
        // Needs to display icons for the dialogue options


        // Needs to be able to support "actions" to:
        // Assign quests
        // Spawn NPCs
        // etc. all done dynamically to allow the user to specify the end action
    }

    public class DialogueText
    {

    }

    public class DialogueChoice
    {

    }
}