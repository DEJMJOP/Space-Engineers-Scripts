public Program(){
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
}

public void Main(){
	
	float current = 0.0f;
	float max = 0.0f;
	
	List<IMyTerminalBlock> cargo = new List<IMyTerminalBlock>();
	GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(cargo);
	List<IMyTerminalBlock> drills = new List<IMyTerminalBlock>();
	GridTerminalSystem.GetBlocksOfType<IMyShipDrill>(drills);
    
	
	List<IMyTerminalBlock> connectors = new List<IMyTerminalBlock>();
	GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(connectors);
	
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
	
	Echo("Cargo Filled: " + percent.ToString("0.##") + "%");
	
	IMyTextSurfaceProvider blk = GridTerminalSystem.GetBlockWithName("Industrial Cockpit") as IMyTextSurfaceProvider;
	
	IMyTextSurface surface = blk.GetSurface(1);
	
	//IMyTextSurface surface = GridTerminalSystem.GetBlockWithName("[Cargo LCD]") as IMyTextSurface;
	
	if(surface != null){
		surface.ContentType = ContentType.SCRIPT;
		using(var frame = surface.DrawFrame()){
			MySprite test = MySprite.CreateText("Cargo Filled: " + percent.ToString("0.##") + "%", "Debug", new Color(1f,1f,0f), 1f, TextAlignment.CENTER);
			frame.Add(test);
		}
	}
	else{
		Echo("ERROR");
	}
		
	/*
	List<IMyTerminalBlock> screens = new List<IMyTerminalBlock>();
	GridTerminalSystem.SearchBlocksOfName("[Cargo LCD]", screens);
	
	foreach(IMyTerminalBlock panel in screens){
		((IMyTextPanel)panel).WritePublicText("Cargo Filled: " + percent.ToString("0.##") + "%", false);
	}
	*/
	
}