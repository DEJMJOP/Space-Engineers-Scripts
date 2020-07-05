public Program(){
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
}

public void Main(){
	
	float current = 0.0f;
	float max = 0.0f;
	
	List<IMyTerminalBlock> cargo = new List<IMyTerminalBlock>();
	GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(cargo, block => block.IsSameConstructAs(Me));
	List<IMyTerminalBlock> drills = new List<IMyTerminalBlock>();
	GridTerminalSystem.GetBlocksOfType<IMyShipDrill>(drills, block => block.IsSameConstructAs(Me));
    
	
	List<IMyTerminalBlock> connectors = new List<IMyTerminalBlock>();
	GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(connectors, block => block.IsSameConstructAs(Me));
	
	foreach(IMyTerminalBlock c in cargo){
		current += (float)c.GetInventory(0).CurrentVolume;
		max += (float)c.GetInventory(0).MaxVolume;
	}
	foreach(IMyTerminalBlock c in drills){
		current += (float)c.GetInventory(0).CurrentVolume;
		max += (float)c.GetInventory(0).MaxVolume;
	}
	
	foreach(IMyTerminalBlock c in connectors){
		current += (float)c.GetInventory(0).CurrentVolume;
		max += (float)c.GetInventory(0).MaxVolume;
	}
	
	float percent = (current/max) * 100;
	
	List<IMyTerminalBlock> screens = new List<IMyTerminalBlock>();
	GridTerminalSystem.SearchBlocksOfName("[Cargo LCD]", screens);
	
	foreach(IMyTerminalBlock panel in screens){
		((IMyTextPanel)panel).WritePublicText("Cargo Filled: " + percent.ToString("0.##") + "%", false);
	}
	
}