bool captainOccupied = false;
IMyShipController captainChair;
IMyTimerBlock captainTimer;


bool leftGunnerOccupied = false;
IMyShipController leftGunnerChair;
IMyTimerBlock leftGunnerTimer;
IMyBlockGroup turretGroup;

public Program(){
	Runtime.UpdateFrequency = UpdateFrequency.Update10;
	//Finds the turret group and stores it
	List<IMyBlockGroup> groupList = new List<IMyBlockGroup>();
	GridTerminalSystem.GetBlockGroups(groupList);
	for(int i = 0; i < groupList.Count; i++){
		if(groupList[i].Name == "Type-7 Gatlings"){
			turretGroup = groupList[i];
			break;
		}
	}
	
	//Sets up captain variables
	captainChair = GridTerminalSystem.GetBlockWithName("Type 7 - Remote Control") as IMyShipController;
	captainTimer = GridTerminalSystem.GetBlockWithName("Captain Chair - Timer Block") as IMyTimerBlock;
	captainOccupied = captainChair.IsUnderControl;
	
	//Sets up left gunner variables
	leftGunnerChair = GridTerminalSystem.GetBlockWithName("Type-7 Gunner Seat") as IMyShipController;
	leftGunnerTimer = GridTerminalSystem.GetBlockWithName("Left Gunner Timer Block") as IMyTimerBlock;
	leftGunnerOccupied = leftGunnerChair.IsUnderControl;
}

public void Main(){
	
	CaptainChair();
	LeftGunner();
	
	Running();
}

private void CaptainChair(){
	if(captainOccupied != captainChair.IsUnderControl){
		captainTimer.Trigger();
	}
	captainOccupied = captainChair.IsUnderControl;
}

private void LeftGunner(){
	bool occupied = false;
	occupied = leftGunnerChair.IsUnderControl;
	
	if(leftGunnerOccupied)
		if(TurretBeingControlled()) occupied = true;
	
	if(leftGunnerOccupied != occupied){
		leftGunnerTimer.Trigger();
	}
	leftGunnerOccupied = occupied;
}


private bool TurretBeingControlled(){
	List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
	turretGroup.GetBlocks(blocks);
	foreach(IMyTerminalBlock block in blocks){
		if(block is IMyLargeGatlingTurret){
			if( ((IMyLargeGatlingTurret)block).IsUnderControl){
				return true;
			}
		}
	}
	return false;
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