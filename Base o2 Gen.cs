public Program(){
	Runtime.UpdateFrequency = UpdateFrequency.Update10;
}

List<IMyGasGenerator> oxygenGens;
List<IMyGasTank> tanks;
public void Main(){
	oxygenGens = new List<IMyGasGenerator>();
	GridTerminalSystem.GetBlocksOfType<IMyGasGenerator>(oxygenGens);
	tanks = new List<IMyGasTank>();
	GridTerminalSystem.GetBlocksOfType<IMyGasTank>(tanks);
	
	if(ShouldEnableForHydrogen() || ShouldEnableForOxygen() ){
		EnableOxygenGens();
	}
	else{
		DisableOxygenGens();
	}
	
}

void EnableOxygenGens(){
	foreach(IMyGasGenerator oxygenGen in oxygenGens){
		if(oxygenGen.IsSameConstructAs(Me))
			oxygenGen.Enabled = true;
	}
}
void DisableOxygenGens(){
	foreach(IMyGasGenerator oxygenGen in oxygenGens){
		oxygenGen.Enabled = false;
	}
}


bool ShouldEnableForHydrogen(){
	foreach(IMyGasTank tank in tanks){
		if(!(tank.CustomData.Contains("Exclude"))){
			if(tank.DetailedInfo.Contains("Hydrogen Tank")){
				if(tank.FilledRatio != 1f) return true;
			}
		}
	}
	return false;
}

bool ShouldEnableForOxygen(){
	float currentoxygen = 0f;
	int oxygentanks = 0;
	foreach(IMyGasTank tank in tanks){
		if(!(tank.CustomData.Contains("Exclude"))){
			if(tank.DetailedInfo.Contains("Oxygen Tank") && tank.IsSameConstructAs(Me)){
				oxygentanks ++;
				currentoxygen += (float)tank.FilledRatio*100;
			}
		}
	}
	float percent = currentoxygen / oxygentanks;
	Echo("percent: " + percent);
	if(percent < 10) return true;
	return false;
}