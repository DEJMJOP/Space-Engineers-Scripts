public Program(){
	Runtime.UpdateFrequency = UpdateFrequency.Update10;
}

public void Main(){
	
	List<IMyBlockGroup> groups = new List<IMyBlockGroup>();
	GridTerminalSystem.GetBlockGroups(groups);
	
	List<int> depressurizedRooms = new List<int>();
	
	for(int i = 0 ; i < groups.Count; i ++){
		if(groups.ElementAt(i).Name.Contains("#Room")){
			
			OpenDoors(groups.ElementAt(i));
			
			if(CheckRoom(groups.ElementAt(i))){
				depressurizedRooms.Add(i);
			}
		}
	}
	foreach(int i in depressurizedRooms){
		Echo(""+i);
		SealRoom(groups.ElementAt(i));
	}
}
private void OpenDoors(IMyBlockGroup group){
	List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
	group.GetBlocks(blocks);
	
	foreach(IMyTerminalBlock block in blocks){
		if(block is IMyDoor){
			IMyDoor d = (IMyDoor)block;
			d.ApplyAction("OnOff_On");
		}
	}

}

private void SealRoom(IMyBlockGroup group){
	List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
	group.GetBlocks(blocks);
	Echo(group.Name);
	foreach(IMyTerminalBlock block in blocks){
		if(block is IMyDoor){
			IMyDoor d = (IMyDoor)block;
			d.CloseDoor();
			if(d.Status == DoorStatus.Closed){
				d.ApplyAction("OnOff_Off");
			}
		}
	}
	
}

private bool CheckRoom(IMyBlockGroup group){
	
	List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
	group.GetBlocks(blocks);
	
	
	
	foreach(IMyTerminalBlock block in blocks){
		if(block is IMyAirVent){
			if(!((IMyAirVent)block).CanPressurize){
				return true;
			}
		}
	}
	return false;
}

/*
	
*/