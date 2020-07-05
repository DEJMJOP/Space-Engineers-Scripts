public Program(){
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
}

public void Main(string argument, UpdateType updateSource){
	IMyPistonBase piston = GridTerminalSystem.GetBlockWithName("Piston 2") as IMyPistonBase;
	Echo("Velocity: " + piston.Velocity);
	//piston.Velocity = 1f;
	String pistonInfo = piston.DetailedInfo;
	String[] pistonInfoSplit = (pistonInfo.Split(':'));
	String[] pistonDistanceSplit = (pistonInfoSplit[1].Split('m'));
	double pistonDistance = double.Parse(pistonDistanceSplit[0]);
    if (pistonDistance == 10f) piston.Velocity = 0;
	Echo("" + pistonDistance);
}