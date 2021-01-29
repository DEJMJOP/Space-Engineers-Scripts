char[] delims = new[]{'\r','\n'};
IMyRadioAntenna antenna;
List<Vector3D> antennaLocations;
List<float> antennaRanges;
List<string> baseNames;


List<IMyShipConnector> connectors;
public Program(){
	Runtime.UpdateFrequency = UpdateFrequency.Update10;
	
	List<IMyRadioAntenna> tempAntennaList = new List<IMyRadioAntenna>();
	GridTerminalSystem.GetBlocksOfType<IMyRadioAntenna>(tempAntennaList, block => block.IsSameConstructAs(Me));
	
	antenna = tempAntennaList[0];
	connectors = new List<IMyShipConnector>();
	GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(connectors, block => block.IsSameConstructAs(Me));
	
	antennaLocations = new List<Vector3D>();
	antennaRanges = new List<float>();
	baseNames = new List<string>();
}
Vector3D pos;
bool shouldLearn = true;
public void Main()
{
	if(IsConnected()){
		if(shouldLearn){
			LoadData();
			LearnAntenna();
			SaveData();
			shouldLearn = false;
		}
	}
	else{
		shouldLearn = true;
	}
	Echo("Antenna Owner: " + antenna.OwnerId);
	Echo("Programmable Block Owner: " + Me.OwnerId);
	float range = 50000f;
	Echo("("+antennaLocations.Count+","+antennaRanges.Count+","+baseNames.Count+")");
	if(antennaLocations.Count > 0){
		pos = antenna.GetPosition();
		//Echo("Position: " + pos.X + "," + pos.Y + "," + pos.Z);
		Vector3D closest = FindClosest();
		//Echo("Base: " + closest.X + "," + closest.Y + "," + closest.Z);
		
		float dis = Vector3.Distance(closest, pos);
		//Echo("Distance: " + dis);
		
		range = dis+50;
		if(range > 50000) range = 50000f;
	}
	antenna.Radius = range;
}

void LoadData(){
	string[] lines = Me.CustomData.Split(delims, StringSplitOptions.RemoveEmptyEntries);
	antennaLocations.Clear();
	antennaRanges.Clear();
	baseNames.Clear();
	
	foreach(string line in lines){
		string[] param = line.Split(':');
		if(param.Length == 5){
			try{
				Vector3D p = new Vector3D(float.Parse(param[0]),float.Parse(param[1]),float.Parse(param[2]));
				float range = float.Parse(param[3]);
				string name = param[4];
				antennaLocations.Add(p);
				antennaRanges.Add(range);
				baseNames.Add(name);
			} catch(Exception e){}
		}
	}
}
void SaveData(){
	string save = "";
	for(int i = 0; i < antennaLocations.Count; i++){
		save += antennaLocations[i].X + ":";
		save += antennaLocations[i].Y + ":";
		save += antennaLocations[i].Z + ":";
		save += antennaRanges[i] + ":";
		save += baseNames[i];
		save += "\n";
	}
	Me.CustomData = save;
}

void LearnAntenna(){
	foreach(IMyShipConnector con in connectors){
		Echo("Status: " + con.Status);
		if(con.Status == MyShipConnectorStatus.Connected){
			//If connected to a station!
			if(con.OtherConnector.CubeGrid.IsStatic){
				List<IMyRadioAntenna> ants = new List<IMyRadioAntenna>();
				GridTerminalSystem.GetBlocksOfType<IMyRadioAntenna>(ants, block => block.IsSameConstructAs(con.OtherConnector));
			
				foreach(IMyRadioAntenna ant in ants){
					Echo("Antenna: " + ant.DisplayNameText);
					Vector3D position = ant.GetPosition();
					float distance = ant.Radius;
					string baseName = ant.CubeGrid.CustomName;
					if(!antennaLocations.Contains(position) && ValidAntenna(ant)){
						antennaLocations.Add(position);
						antennaRanges.Add(distance);
						baseNames.Add(baseName);
					}
				}
				
				for(int i = antennaLocations.Count-1; i >= 0; i--){
					if(baseNames[i].Equals(con.OtherConnector.CubeGrid.CustomName)){
						bool exists = false;
						foreach(IMyRadioAntenna ra in ants){
							if(ra.GetPosition() == antennaLocations[i]) exists = true;
						}
						
						if(!exists){
							antennaLocations.RemoveAt(i);
							antennaRanges.RemoveAt(i);
							baseNames.RemoveAt(i);
						}
						else{
							float r = 0;
							foreach(IMyRadioAntenna ra in ants){
								if(ra.GetPosition() == antennaLocations[i]) r = ra.Radius;
							}
							antennaRanges[i] = r;
						}
					}
				}
				
			}
		}
	}
}

bool ValidAntenna(IMyRadioAntenna ant){
	if(!ant.IsFunctional) return false;
	if(ant.OwnerId == Me.OwnerId) return true;
	return false;
}

bool IsConnected(){
	foreach(IMyShipConnector con in connectors){
		if(con.Status == MyShipConnectorStatus.Connected)
			if(con.OtherConnector.CubeGrid.IsStatic)
				return true;
	}
	return false;
}

Vector3D FindClosest(){
	Vector3D close = new Vector3D(0,0,0);
	int closeIndex = -1;
	float dis = 60000;
	for(int i = 0 ; i < antennaLocations.Count; i ++){
		float temp = Vector3.Distance(pos, antennaLocations[i]);
		if(temp < dis && temp < antennaRanges[i]){
			dis = temp;
			close = antennaLocations[i];
			closeIndex = i;
		}
	}
	if(closeIndex > -1 && closeIndex < baseNames.Count) Echo("Connected To: " + baseNames[closeIndex]);
	return close;
}