IMyShipController seat;
IMyTimerBlock timer;
IMyBlockGroup turretGroup;
public Program() 
{ 
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
	seat = GridTerminalSystem.GetBlockWithName("Type-7 Gunner Seat") as IMyShipController;
	timer = GridTerminalSystem.GetBlockWithName("Left Gunner Timer Block") as IMyTimerBlock;
	
	List<IMyBlockGroup> groupList = new List<IMyBlockGroup>();
	GridTerminalSystem.GetBlockGroups(groupList);
	for(int i = 0; i < groupList.Count; i++){
		if(groupList[i].Name == "Type-7 Gatlings"){
			turretGroup = groupList[i];
			break;
		}
	}
	
}
bool wasOccupied = false;
/
public void Main() 
{
	bool occupied = false;
	occupied = seat.IsUnderControl;
	List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
	turretGroup.GetBlocks(blocks);
	foreach(IMyTerminalBlock block in blocks){
		if(block is IMyLargeGatlingTurret){
			if( ((IMyLargeGatlingTurret)block).IsUnderControl){
				occupied = true;
				break;
			}
		}
	}
	
	
	if(wasOccupied != occupied){
		timer.Trigger();
	}
    Running();
	wasOccupied = occupied;
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