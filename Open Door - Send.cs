IMyTerminalBlock position;
const string BroadcastTag = "[DRONE BROADCAST]";
public Program()
{
	position = GridTerminalSystem.GetBlockWithName("Antenna");
}

public void Main(string args)
{
	Vector3 pos = position.GetPosition();
	string s = pos.X + ":" + pos.Y + ":" + pos.Z;
	IGC.SendBroadcastMessage<string>(BroadcastTag,s, TransmissionDistance.AntennaRelay);
}