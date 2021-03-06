/*
Takes an input argument of a gps coordinate
GPS:Ice:1048917.7584688:79752.6323696933:1600020.79087306:
*/


public void Main(string args){
	Echo("Run");
	//Create a list to store coords
	List<float> coords = new List<float>();
	//Find the remote control block on the grid
	IMyRemoteControl remote = GridTerminalSystem.GetBlockWithName("Remote Control") as IMyRemoteControl;
	//Clear the list of coords
	coords.Clear();
	//Creates a regular expression matching a standard GPS coordinate
	string pattern = @"GPS:[a-zA-Z0-9_.-]*:(-*\d*.\d*):(-*\d*.\d*):(-*\d*.\d*):";
	//Creates an object to store a Match collection, that will store the coords if they match the above format
	System.Text.RegularExpressions.MatchCollection matches;
	//Searches for any matches using the input and the pattern
	matches = System.Text.RegularExpressions.Regex.Matches(args, pattern);
	
	//Create an empty object to store a match
	System.Text.RegularExpressions.Match match = null;
	
	//	Ensures there are actually matches for the expression.
	//	Places coordinates into the coords list by parsing strings
	if(matches.Count > 0){
		match = matches[0];
		for(int i = 0; i < match.Groups.Count; i++){
			float coord;
			if(float.TryParse(match.Groups[i].ToString(), out coord)){
				coords.Add(coord);
			}
		}
	}
	else{
		Echo("No Match");
	}
	
	
	//Adds the waypoint to the remote control block
	if(coords.Count >= 3){
		remote.ClearWaypoints();
		Vector3D Pos = new Vector3D();
		Pos.X = coords[0];
		Pos.Y = coords[1];
		Pos.Z = coords[2];
		
		Echo("X: " + Pos.X + " Y: " + Pos.Y + " Z: " + Pos.Z);
		
		remote.AddWaypoint(Pos, "Target");
	}
}


//===========================================================================

/*
A function that takes a string as an input, and returns a Vector3D gps coordinate 
*/

Vector3D StringToGPS(string gps){
	List<float> coords = new List<float>();
	string pattern = @"GPS:[a-zA-Z0-9_.-]*:(-*\d*.\d*):(-*\d*.\d*):(-*\d*.\d*):";
	System.Text.RegularExpressions.MatchCollection matches;
	matches = System.Text.RegularExpressions.Regex.Matches(gps, pattern);
	System.Text.RegularExpressions.Match match = null;
	if(matches.Count > 0){
		match = matches[0];
		for(int i = 0; i < match.Groups.Count; i++){
			float coord;
			if(float.TryParse(match.Groups[i].ToString(), out coord)){
				coords.Add(coord);
			}
		}
	}
	if(coords.Count >= 3){
		Vector3D Pos = new Vector3D();
		Pos.X = coords[0];
		Pos.Y = coords[1];
		Pos.Z = coords[2];
		return Pos;
	}
	Echo("Error! Could Not Find Coordinates!");
	return new Vector3D(0,0,0);
}

/*
Example Main Method to show usage of StringToGPS function

public void Main(){
	Vector3D test;
	test = StringToGPS("GPS:Ice:1048917.7584688:79752.6323696933:1600020.79087306:");
	Echo("X: " + test.X + " Y: " + test.Y + " Z: " + test.Z);
}
*/

