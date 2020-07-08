IMyRadioAntenna antenna;
List<Vector3D> antennaLocations;
public Program(){
	Runtime.UpdateFrequency = UpdateFrequency.Update10;
	antenna = GridTerminalSystem.GetBlockWithName("Drone Antenna") as IMyRadioAntenna;
	
	antennaLocations = new List<Vector3D>();
	antennaLocations.Add(new Vector3D(1043375.43675035, 77373.5080055236, 1601189.71342357));
	antennaLocations.Add(new Vector3D(1069102.49, 81562.40, 1625408.16));
	
}
Vector3D pos;
public void Main(){
	
	pos = antenna.GetPosition();
	Echo("Position: " + pos.X + "," + pos.Y + "," + pos.Z);
	Vector3D closest = FindClosest();
	Echo("Base: " + closest.X + "," + closest.Y + "," + closest.Z);
	
	float dis = Vector3.Distance(closest, pos);
	Echo("Distance: " + dis);
	
	float range = dis+50;
	if(range > 50000) range = 50000f;
	
	antenna.Radius = range;
	
}


Vector3D FindClosest(){
	Vector3D close = new Vector3D(0,0,0);
	float dis = 60000;
	foreach(Vector3D coord in antennaLocations){
		float temp = Vector3.Distance(pos, coord);
		if(temp < dis){
			dis = temp;
			close = coord;
		}
	}
	return close;
}