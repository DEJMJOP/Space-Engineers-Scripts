//DEJMJOP's Hangar Door Script - Of epic proportions
string HANGAR_RELATED_BLOCKS = "Hangar Door";


List<IMyPistonBase> pistons;
List<IMyAirVent> depressurizeVents;
List<IMyAirVent> mainVents;
List<IMyGasTank> oxygenTanks;
List<IMyDoor> hangarRoomDoors;
List<IMyShipMergeBlock> mergeBlocks;
IMyGravityGenerator gravGen;

public Program(){
	Runtime.UpdateFrequency = UpdateFrequency.Update10;
	SetupScreen();
	Write("Performing First Time Setup!");
	
	
	pistons = new List<IMyPistonBase>();
	GridTerminalSystem.GetBlocksOfType<IMyPistonBase>(pistons);
	for(int i = (pistons.Count-1); i>=0; i--){
		if(!(pistons.ElementAt(i).CustomData.Contains(HANGAR_RELATED_BLOCKS))) pistons.RemoveAt(i);
	}
	
	depressurizeVents = new List<IMyAirVent>();
	mainVents = new List<IMyAirVent>();
	List<IMyAirVent> vents = new List<IMyAirVent>();
	GridTerminalSystem.GetBlocksOfType<IMyAirVent>(vents);
	foreach(IMyAirVent vent in vents){
		if(vent.CustomData.Contains(HANGAR_RELATED_BLOCKS)){
			if(vent.CustomData.Contains("Depressurize")){
				depressurizeVents.Add(vent);
			}
			else{
				mainVents.Add(vent);
			}
		}
	}
	
	oxygenTanks = new List<IMyGasTank>();
	GridTerminalSystem.GetBlocksOfType<IMyGasTank>(oxygenTanks);
	for(int i = oxygenTanks.Count-1; i >= 0; i--){
		if(!(oxygenTanks.ElementAt(i).CustomData.Contains(HANGAR_RELATED_BLOCKS))) oxygenTanks.RemoveAt(i);
	}
	Echo("Tanks: " + oxygenTanks.Count);
	
	hangarRoomDoors = new List<IMyDoor>();
	GridTerminalSystem.GetBlocksOfType<IMyDoor>(hangarRoomDoors);
	for(int i = hangarRoomDoors.Count-1; i >= 0 ; i --){
		if(!(hangarRoomDoors.ElementAt(i).CustomData.Contains(HANGAR_RELATED_BLOCKS))) hangarRoomDoors.RemoveAt(i);
	}
	Echo("Hangar Room Doors: " + hangarRoomDoors.Count);
	
	mergeBlocks = new List<IMyShipMergeBlock>();
	GridTerminalSystem.GetBlocksOfType<IMyShipMergeBlock>(mergeBlocks);
	for(int i = mergeBlocks.Count-1; i >=0; i--){
		if(!(mergeBlocks.ElementAt(i).CustomData.Contains(HANGAR_RELATED_BLOCKS))) mergeBlocks.RemoveAt(i);
	}
	Echo("Merge Blocks: " + mergeBlocks.Count);
	
	gravGen = GridTerminalSystem.GetBlockWithName("Gravity Generator - Hangar Door") as IMyGravityGenerator;
	
	currentState = "";
	lastState = "close";
	Write("Completed First Time Setup!");
}

string lastState;
string currentState;
public void Main(string args){
	Running();
	Echo("State: " + currentState);
	WriteState();
	
	if(currentState.Equals("")){
		if(args.ToLower().Equals("open") ){
			currentState = "OS1";
		}
		else if(args.ToLower().Equals("close")){
			currentState = "CS1";
		}
		else if(args.ToLower().Equals("toggle")){
			if(lastState.Equals("open")) currentState = "CS1";
			else if(lastState.Equals("close")) currentState = "OS1";
		}
	}
	
	switch(currentState){
		case "OS1":
		OpenStepOne();
		break;
		case "OS2":
		OpenStepTwo();
		break;
		case "CS1":
		CloseStepOne();
		break;
		case "CS2":
		CloseStepTwo();
		break;
		case "":
		IdleState();
		break;
	}
	
}

void IdleState(){
	if(lastState.Equals("open")){
		foreach(IMyDoor door in hangarRoomDoors){
			if(door.Enabled){
				if(!(door.Status == DoorStatus.Closed)){
					door.CloseDoor();
				}
				else{
					door.Enabled = false;
				}
			}
		}
	}
}


void OpenStepOne(){
	
	if(AreMainVentsOff()){
		if(AreDoorsClosed()){
			foreach(IMyDoor door in hangarRoomDoors){
				door.ApplyAction("OnOff_Off");
			}
			foreach(IMyAirVent vent in depressurizeVents){
				vent.Depressurize = true;
			}
			if(AreVentsDepressurized()) currentState = "OS2";
		}
		else{
			foreach(IMyDoor door in hangarRoomDoors){
				door.ApplyAction("OnOff_On");
				door.CloseDoor();
			}
		}
	}
	else{
		foreach(IMyAirVent vent in mainVents) vent.Enabled = false;
	}
	
}

bool AreDoorsClosed(){
	foreach(IMyDoor door in hangarRoomDoors){
		if(!(door.Status == DoorStatus.Closed)) return false;
	}
	return true;
}
bool AreMainVentsOff(){
	foreach(IMyAirVent vent in mainVents){
		if(vent.Enabled) return false;
	}
	return true;
}
bool AreVentsDepressurized(){
	foreach(IMyAirVent vent in depressurizeVents){
		Echo("O2 Level: " + vent.GetOxygenLevel());
		if(vent.GetOxygenLevel() >= 0.1f) return false;
	}
	return true;
}

void OpenStepTwo(){
	if(AreMergeBlocksOff()){
		foreach(IMyPistonBase piston in pistons){
			piston.Extend();
		}
		gravGen.Enabled = true;
		if(PistonsFinishedExtending()){
			currentState = "";
			lastState = "open";
		}
	}
	else{
		foreach(IMyShipMergeBlock mb in mergeBlocks){
			mb.Enabled = false;
		}
	}
}

bool AreMergeBlocksOff(){
	foreach(IMyShipMergeBlock mb in mergeBlocks){
		if(mb.Enabled) return false;
	}
	return true;
}
bool PistonsFinishedExtending(){
	foreach(IMyPistonBase piston in pistons){
		if(piston.CurrentPosition != piston.MaxLimit) return false;
	}
	return true;
}


void CloseStepOne(){
	if(!PistonsFinishedRetracting()){
		foreach(IMyPistonBase piston in pistons){
			piston.Retract();
		}
	}
	else{
		foreach(IMyShipMergeBlock mb in mergeBlocks){
			mb.Enabled = true;
		}
		if(AreMergeBlocksOn()){
			gravGen.Enabled = false;
			currentState = "CS2";
		}
	}
}

bool PistonsFinishedRetracting(){
	foreach(IMyPistonBase piston in pistons){
		if(piston.CurrentPosition != piston.MinLimit) return false;
	}
	return true;
}
bool AreMergeBlocksOn(){
	foreach(IMyShipMergeBlock mb in mergeBlocks){
		if(!mb.Enabled) return false;
	}
	return true;
}

void CloseStepTwo(){
	if(!AreOxygenTanksEmpty()){
		foreach(IMyAirVent vent in depressurizeVents){
			vent.Depressurize = false;
		}
	}
	else{
		foreach(IMyAirVent vent in mainVents){
			vent.Enabled = true;
		}
		foreach(IMyDoor door in hangarRoomDoors){
			door.Enabled = true;
		}
		currentState = "";
		lastState = "close";
	}
}
bool AreOxygenTanksEmpty(){
	foreach(IMyGasTank oxygenTank in oxygenTanks){
		if(oxygenTank.FilledRatio != 0f) return false;
	}
	return true;
}


void WriteState(){
	switch(currentState){
		case "":
		Write("State: IDLE");
		break;
		case "OS1":
		Write("State: Depressurizing Room");
		break;
		case "OS2":
		Write("State: Extending Hangar Door");
		break;
		case "CS1":
		Write("State: Retracting Hangar Door");
		break;
		case "CS2":
		Write("State: Repressurizing Room");
		break;
	}
}

IMyTextSurface pbSurface;
void SetupScreen(){
	pbSurface = Me.GetSurface(0);
	pbSurface.ContentType = ContentType.TEXT_AND_IMAGE;
	pbSurface.FontSize = 1f;
	pbSurface.Font = "Monospace";
	pbSurface.Alignment = VRage.Game.GUI.TextPanel.TextAlignment.CENTER;
	pbSurface.TextPadding = 25f;
}

void Write(string s){
	pbSurface.WriteText(s);
}

int i = 0; 
public void Running(){
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