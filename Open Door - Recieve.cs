const string BroadcastTag = "[DRONE BROADCAST]";
IMyBroadcastListener _bListener;
IMyTextSurface pbSurface;

public Program()
{
	Runtime.UpdateFrequency = UpdateFrequency.Update10;
	_bListener = IGC.RegisterBroadcastListener(BroadcastTag);
	_bListener.SetMessageCallback(BroadcastTag);
	
	pbSurface = Me.GetSurface(0);
	pbSurface.ContentType = ContentType.TEXT_AND_IMAGE;
	pbSurface.FontSize = 1f;
	pbSurface.Font = "Monospace";
	pbSurface.Alignment = VRage.Game.GUI.TextPanel.TextAlignment.CENTER;
	pbSurface.TextPadding = 30f;
}
List<IMyDoor> doors;
public void Main()
{
	doors = new List<IMyDoor>();
	GridTerminalSystem.GetBlocksOfType<IMyDoor>(doors);
	if(_bListener.HasPendingMessage){
		HandleMessages();
	}
}

bool HandleMessages()
{
	do
	{
		var msg = _bListener.AcceptMessage();
		string s = ""+msg.Data;		
		string[] coords = s.Split(':');
		
		string ss = "X: " + coords[0] + "\nY: " + coords[1] + "\nZ: " + coords[2];
		pbSurface.WriteText(ss);
		
		Vector3 pos = new Vector3(float.Parse(coords[0]), float.Parse(coords[1]), float.Parse(coords[2]));
		IMyDoor closestDoor = FindClosestDoor(pos);
		foreach(IMyDoor d in doors)
		{
			if(d.CustomName.Equals(closestDoor.CustomName))
			{
				d.OpenDoor();
			}
				
		}
	} while(_bListener.HasPendingMessage);
	return true;
}

IMyDoor FindClosestDoor(Vector3 pos)
{
	IMyDoor closest = doors.ElementAt(0);
	foreach(IMyDoor d in doors)
	{
		if(Vector3.Distance(pos, d.GetPosition()) < Vector3.Distance(pos, closest.GetPosition()))
			closest = d;
	}
	return closest;
}


