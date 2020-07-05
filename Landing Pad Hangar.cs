public Program()

{

    Runtime.UpdateFrequency = UpdateFrequency.Update10;
}

int n = 0;
Color RED = new Color(255,0,0);
public void Main(string argument, UpdateType updateSource) 
{ 
    runningIndicator();

    hangar();
    hangarDisplay();
} 

private void hangar(){
    //Create lists of each hangar door
    List<IMyTerminalBlock> rearhangar = new List<IMyTerminalBlock>();
    GridTerminalSystem.SearchBlocksOfName("Landing Pad - Outer Hangar Door", rearhangar);

    List<IMyTerminalBlock> innerhangar = new List<IMyTerminalBlock>();
    GridTerminalSystem.SearchBlocksOfName("Landing Pad - Inner Hangar Door", innerhangar);
    
    hangarCheck(rearhangar, innerhangar);
    hangarCheck(innerhangar, rearhangar);
}

private void hangarCheck(List<IMyTerminalBlock> a, List<IMyTerminalBlock> b){
    bool openDoor = true;
    foreach(IMyTerminalBlock h in a){
        if( ((IMyDoor)h).Status != DoorStatus.Closed){
            openDoor = false;
        }
    }

    if(!openDoor){
        foreach(IMyTerminalBlock h in b){
            ((IMyDoor)h).ApplyAction("OnOff_Off");
        }
    } else{
        foreach(IMyTerminalBlock h in b){
            ((IMyDoor)h).ApplyAction("OnOff_On");
        }
    }
}

private void hangarDisplay(){
    List<IMyTerminalBlock> lcds = new List<IMyTerminalBlock>();
    GridTerminalSystem.SearchBlocksOfName("[Landing Pad LCD]", lcds);
    
    IMyDoor inner = (IMyDoor)GridTerminalSystem.GetBlockWithName("Landing Pad - Inner Hangar Door");
    IMyDoor rear = (IMyDoor)GridTerminalSystem.GetBlockWithName("Landing Pad - Outer Hangar Door");

    foreach(IMyTerminalBlock l in lcds){
        ((IMyTextPanel)l).WritePublicText(hangarDisplayText(inner, "Inner"),false);
        ((IMyTextPanel)l).WritePublicText(hangarDisplayText(rear, "Outer"),true);
    }

    List<IMyTerminalBlock> innerLights = new List<IMyTerminalBlock>();
    GridTerminalSystem.SearchBlocksOfName("Landing Pad Hangar Inner Light", innerLights);
    Color c = new Color(0,255,0);
    if(rear.Status == DoorStatus.Closing){
        c = new Color(255,255,0);
    }
    else if(rear.Status == DoorStatus.Open  || rear.Status == DoorStatus.Opening){
		Echo("Red");
        c = new Color(255,0,0);
    }

    foreach(IMyTerminalBlock l in innerLights){
        ((IMyInteriorLight)l).SetValue("Color", c);
    }
	
	List<IMyTerminalBlock> outerLights = new List<IMyTerminalBlock>();
    GridTerminalSystem.SearchBlocksOfName("Landing Pad Hangar Outer Light", outerLights);
    c = new Color(0,255,0);
    if(inner.Status == DoorStatus.Closing){
        c = new Color(255,255,0);
    }
    else if(inner.Status == DoorStatus.Open  || inner.Status == DoorStatus.Opening){
		Echo("Red");
        c = new Color(255,0,0);
    }

    foreach(IMyTerminalBlock l in outerLights){
        ((IMyInteriorLight)l).SetValue("Color", c);
    }

}

private string hangarDisplayText(IMyDoor door, string t){
    string text = t + " Door: Closed";
    
    if(!door.Enabled) text = t + " Door: Locked";
    else if(door.Status == DoorStatus.Open) text = t + " Door: Open";    
    else if(door.Status == DoorStatus.Opening) text= t +" Door: Opening";
    else if(door.Status == DoorStatus.Closing) text=t + " Door: Closing";

    return text + "\n";
}

private void innerHangarLights(){
    

}

private void runningIndicator(){
    if(n == 0){
        Echo("Running.");
        n++;
    }
    else if(n == 1){
        Echo("Running..");
        n++;
    }
    else if(n == 2){
        Echo("Running...");
        n=0;
    }
}
