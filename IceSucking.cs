public Program() 
{ 
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
} 

public void Main() 
{ 
    running();
    IMyTerminalBlock collector = GridTerminalSystem.GetBlockWithName("Ice Sucking Collector");
    if (CountIce() < 5000)
    {
        
        Echo("Yeet");
		collector.ApplyAction("OnOff_On");
    }
    else
    {
        collector.ApplyAction("OnOff_Off");
    }
}


double CountIce()
{
    List<IMyGasGenerator> OXYGEN_GENERATORS = new List<IMyGasGenerator>();
    List<IMyCargoContainer> CARGO_CONTAINERS = new List<IMyCargoContainer>();
    GridTerminalSystem.GetBlocksOfType<IMyGasGenerator>(OXYGEN_GENERATORS, block => block.IsSameConstructAs(Me));
    GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(CARGO_CONTAINERS, block => block.IsSameConstructAs(Me));
    double CURRENT_ICE = 0;
    foreach(IMyCargoContainer cargo in CARGO_CONTAINERS){
        for(int i = 0 ; i < cargo.GetInventory(0).ItemCount; i ++){
            if(cargo.GetInventory(0).GetItemAt(i).Value.Type.SubtypeId == "Ice"){
                string temp = "" + cargo.GetInventory(0).GetItemAt(i).Value.Amount;
                CURRENT_ICE += Double.Parse(temp);
            }
        }
    }
    foreach(IMyGasGenerator cargo in OXYGEN_GENERATORS){
        for(int i = 0 ; i < cargo.GetInventory(0).ItemCount ; i ++){
            if(cargo.GetInventory(0).GetItemAt(i).Value.Type.SubtypeId == "Ice") {
                String temp = "" + cargo.GetInventory(0).GetItemAt(i).Value.Amount;
                CURRENT_ICE += Double.Parse(temp);
            }
        }
    }

    return CURRENT_ICE;
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