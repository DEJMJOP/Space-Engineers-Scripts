float StartChargeAt = 5;
float StopChargeAt = 10;
float notches = 20;


public Program(){
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
}

bool charging = false;

float previousPercent = 0f;

public void Main(){
	
	List<IMyBatteryBlock> batteries = new List<IMyBatteryBlock>();
	GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(batteries, b => b.CubeGrid == Me.CubeGrid);
	List<IMyReactor> reactors = new List<IMyReactor>();
	GridTerminalSystem.GetBlocksOfType<IMyReactor>(reactors, r => r.CubeGrid == Me.CubeGrid);
	List<IMyPowerProducer> power = new List<IMyPowerProducer>();
	GridTerminalSystem.GetBlocksOfType<IMyPowerProducer>(power, b => b.CubeGrid == Me.CubeGrid);
	
	float batteryCurrent = 0f;
	float batteryMax = 0f;
	float currentInput = 0f;
	
	foreach(IMyBatteryBlock battery in batteries){
		batteryCurrent += battery.CurrentStoredPower;
		batteryMax += battery.MaxStoredPower;
	}
	
	float percent = (batteryCurrent/batteryMax)*100;
	
	if(percent < StartChargeAt) charging = true;
	
	Echo("Charging: " + charging);
	Echo("Percent: " + percent);
	
	foreach(IMyPowerProducer p in power){
		if(!(p is IMyBatteryBlock)){
			currentInput += p.CurrentOutput;
		}
	}
	
	
	
	
	IMyTextSurface surface = GridTerminalSystem.GetBlockWithName("Transparent LCD") as IMyTextSurface;
	if(surface != null){
		surface.ContentType = ContentType.TEXT_AND_IMAGE;
		
		String s = charging ? "On":"Off";
		String p = (previousPercent < percent) ? "Charging":"Discharging";
		String bar = "[";
		for(int i = 0 ; i < notches ; i ++){
			if((percent/(100/notches)) >= i) bar += "|";
			else bar += "-";
		}
		bar += "]";
		
		
		surface.WriteText("Battery Percentage: " + percent.ToString("0.00") + "%" 
		+ "\n\n" + bar
		+ "\n\n\n\n" + "Output: " + currentInput.ToString("0.00") + "MW"
		+ "\n Status: " + p
		+ "\n"
		+ "\n Emergency Power: " + s);
		
		if(percent < StartChargeAt) surface.FontColor = new Color(1f,0f,0f);
		else if (percent < StopChargeAt) surface.FontColor = new Color(1f,1f,0f);
		else surface.FontColor = new Color(0f,1f,0f);
	}
	
	
	if(charging){
		if(percent > StopChargeAt){
			charging = false;
			foreach(IMyReactor r in reactors){
				r.ApplyAction("OnOff_Off");
			}
		}
		else{
			foreach(IMyReactor r in reactors){
				r.ApplyAction("OnOff_On");
			}
		}
	}
	previousPercent = percent;
}



