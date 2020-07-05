public Program(){
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
}

public void Main(){
		float currentInput = 0f;
	
		List<IMyPowerProducer> power = new List<IMyPowerProducer>();
		GridTerminalSystem.GetBlocksOfType<IMyPowerProducer>(power, b => b.CubeGrid == Me.CubeGrid);
		
		foreach(IMyPowerProducer p in power){
			if(!(p is IMyBatteryBlock)){
				currentInput += p.CurrentOutput;
			}
		}
		Echo("Base Output: " + currentInput.ToString("0.00") + "MW");
}