//===========================================================================
//Config
//---------------------------------------------------------------------------
string GROUP_NAME = "Ascent Thrusters";
int MAX_SPEED = 85;
//---------------------------------------------------------------------------
//End Of Config
//===========================================================================

public Program(){
	Runtime.UpdateFrequency = UpdateFrequency.Update10;
}

Vector3D previousPosition = new Vector3D(0,0,0);

public void Main(){
	double speed = getSpeed();
	List<IMyBlockGroup> groups = new List<IMyBlockGroup>();
	GridTerminalSystem.GetBlockGroups(groups);
	foreach(IMyBlockGroup group in groups){
		if(group.Name.Contains(GROUP_NAME)){
			List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
			group.GetBlocks(blocks);
			Echo("Yes");
			foreach(IMyTerminalBlock block in blocks){
				if(speed < MAX_SPEED)
					((IMyThrust)block).ApplyAction("IncreaseOverride");
				else
					((IMyThrust)block).ApplyAction("DecreaseOverride");
			}
			
			
		}
	}
}

double getSpeed(){
	Vector3D currentPosition = Me.GetPosition();
	double speed = ((currentPosition-previousPosition)*6).Length();
	previousPosition = currentPosition;
	
	Echo("" + speed);
	return speed;
}



