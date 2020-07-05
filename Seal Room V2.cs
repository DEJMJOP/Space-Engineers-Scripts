public Program(){
	Runtime.UpdateFrequency = UpdateFrequency.Update10;
}


public void Main(){
	List<IMyAirVent> vents = new List<IMyAirVent>();
	GridTerminalSystem.GetBlocksOfType<IMyAirVent>(vents, v => v.IsSameConstructAs(Me));
	List<string> depressurizedRooms = new List<string>();
	
	for(int i = vents.Count -1 ; i >= 0 ; i --){
		if(vents.ElementAt(i).CustomData.Contains("#Room")){
			if(
		}
	}
	

}
