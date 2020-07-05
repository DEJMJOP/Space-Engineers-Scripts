public Program() 
{ 
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
} 

public void Main() 
{ 
    running();
    
    List<IMyBlockGroup> groups = new List<IMyBlockGroup>();
    GridTerminalSystem.GetBlockGroups(groups);
    foreach(IMyBlockGroup group in groups){
        if(group.Name.Contains("Airlock")){
            CheckGroup(group);
        }
    }
} 

public void CheckGroup(IMyBlockGroup group){
    List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
    group.GetBlocks(blocks);
    
    bool c = true;
    foreach(IMyTerminalBlock block in blocks){
        if(!(block is IMyDoor)) c = false;
        
    }
    if(c) airlock(blocks);
    else{
            Echo("--------------------------------------------------");
            Echo("Error in Group " + group.Name);
            Echo("Non-Door Block Found");
            Echo("Please Check Group");
            Echo("--------------------------------------------------\n");
        }
}

public void airlock(List<IMyTerminalBlock> blocks){
    bool c = false;
    foreach(IMyDoor door in blocks){
        if(door.Status == DoorStatus.Open || door.Status == DoorStatus.Closing || door.Status == DoorStatus.Opening){
            c = true;
        }
    }
    if(c){
        foreach(IMyDoor door in blocks){
            if(door.Status != DoorStatus.Open && door.Status != DoorStatus.Closing && door.Status != DoorStatus.Opening){
                door.ApplyAction("OnOff_Off");
            }
        }
    }
    else{
        foreach(IMyDoor door in blocks){
            door.ApplyAction("OnOff_On");
        }
    }
}

int i = 0; 
public void running(){
    switch(i){
        case 0:
        Echo("Running.");
        i++;
        break;
        case 1:
        Echo("Running..");
        i++;
        break;
        case 2:
        Echo("Running...");
        i=0;
        break;
    }
}