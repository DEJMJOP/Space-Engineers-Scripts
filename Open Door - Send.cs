IMyRadioAntenna antenna;
const string BroadcastTag = "[DRONE BROADCAST]";
public Program()
{
	antenna = GridTerminalSystem.GetBlockWithName("Antenna") as IMyRadioAntenna;
}

public void Main(string args)
{
	Vector3 pos = antenna.GetPosition();
	string s = pos.X + ":" + pos.Y + ":" + pos.Z;
	IGC.SendBroadcastMessage<string>(BroadcastTag,s, TransmissionDistance.AntennaRelay);
}


