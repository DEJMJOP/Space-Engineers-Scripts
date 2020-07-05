bool pitOn = false;
public void Main(){

    List<IMyBlockGroup> groups = new List<IMyBlockGroup>();
    GridTerminalSystem.GetBlockGroups(groups);
	pitOn = !pitOn;
	foreach(IMyBlockGroup group in groups){
		List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
		
		if(group.Name.Contains("Grinder Pit Grinders")){
			group.GetBlocks(blocks);
			foreach(IMyTerminalBlock block in blocks){
				if(pitOn){
					block.ApplyAction("OnOff_On");
				}
				else{
					block.ApplyAction("OnOff_Off");
				}
			}
		}
		else if(group.Name.Contains("Grinder Pit Spotlights")){
			group.GetBlocks(blocks);
			foreach(IMyTerminalBlock block in blocks){
				if(pitOn){
					block.SetValue("Color", new Color(255,0,0));
				}
				else{
					block.SetValue("Color", new Color(255,255,255));
				}
			}
		}
	}
    
}

