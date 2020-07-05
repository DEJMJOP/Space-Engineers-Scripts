//===========================================================================
//Config
//===========================================================================

//---------------------------------------------------------------------------
//Power Config
//---------------------------------------------------------------------------
//The power percentage that enables emergency power
float StartChargeAt = 5;
//The power percentage that disables emergency power
float StopChargeAt = 10;

//---------------------------------------------------------------------------
//LCD Config
//---------------------------------------------------------------------------
//Name of the LCD that displays information
string LCD_NAME = "[Battery LCD]";
//How many notches in the percentage bar
float notches = 20;

//Colors of the different power states displayed on screen
Color LOW_POWER_COLOR  = new Color(0.50f,0.00f,0.00f);
Color MED_POWER_COLOR  = new Color(0.50f,0.50f,0.00f);
Color HIGH_POWER_COLOR = new Color(0.00f,0.50f,0.00f);

//---------------------------------------------------------------------------
//Lights Config
//---------------------------------------------------------------------------#
//Do you want the script to control your bases lights? true/false
bool CONTROL_LIGHTS = true;

//Color of the lights in different power states
Color LOW_POWER_LIGHTS = new Color(.25f,0f,0f);
Color MED_POWER_LIGHTS = new Color(.75f,.75f,.75f);
Color HIGH_POWER_LIGHTS = new Color(1f,1f,1f);

//Any lights you want not not be controlled, place this word in the custom data
string EXCLUSION_WORD = "Exclude";

//---------------------------------------------------------------------------
//End Of Config
//===========================================================================

public Program(){
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
}

bool charging = false;

float previousPercent = 0f;

public void Main(){
	if(!firstTime) FirstTimeSetup();
	Running();
	
	
	List<IMyBatteryBlock> batteries = new List<IMyBatteryBlock>();
	GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(batteries, b => b.CubeGrid == Me.CubeGrid);
	List<IMyReactor> reactors = new List<IMyReactor>();
	GridTerminalSystem.GetBlocksOfType<IMyReactor>(reactors, r => r.CubeGrid == Me.CubeGrid);
	List<IMyPowerProducer> power = new List<IMyPowerProducer>();
	GridTerminalSystem.GetBlocksOfType<IMyPowerProducer>(power);
	
	float batteryCurrent = 0f;
	float batteryMax = 0f;
	float currentInput = 0f;
	float powerOutput = 0f;
	
	foreach(IMyBatteryBlock battery in batteries){
		batteryCurrent += battery.CurrentStoredPower;
		batteryMax += battery.MaxStoredPower;
		powerOutput += battery.CurrentOutput;
	}
	
	float percent = (batteryCurrent/batteryMax)*100;
	
	if(percent < StartChargeAt) charging = true;
	
	foreach(IMyPowerProducer p in power){
		if(!(p is IMyBatteryBlock)){
			currentInput += p.CurrentOutput;
		}
	}
	
	
	
	
	IMyTextSurface surface = GridTerminalSystem.GetBlockWithName(LCD_NAME) as IMyTextSurface;
	if(surface != null){
		surface.ContentType = ContentType.TEXT_AND_IMAGE;
		
		String s = charging ? "On":"Off";
		
		s = (reactors.Count > 0) ? s : "No Reactors";
		String p = (previousPercent < percent) ? "Charging":"Discharging";
		if(percent > 99) p = "Full";
		String bar = "[";
		for(int i = 0 ; i < notches ; i ++){
			if((percent/(100/notches)) >= i) bar += "|";
			else bar += "-";
		}
		bar += "]";
		
		
		surface.WriteText("Battery Percentage: " + percent.ToString("0.00") + "%" 
		+ "\n\n" + bar
		+ "\n\n\n\n" + "Base Input: " + currentInput.ToString("0.00") + "MW"
		+ "\n Battery Output: " + powerOutput.ToString("0.00") + "MW"
		+ "\n Status: " + p
		+ "\n"
		+ "\n Emergency Power: " + s);
		
		if(percent < StartChargeAt){
			surface.FontColor = LOW_POWER_COLOR;
			if(CONTROL_LIGHTS) ChangeLights(LOW_POWER_LIGHTS);
		}
		else if(percent < StopChargeAt){
			surface.FontColor = MED_POWER_COLOR;
			if(CONTROL_LIGHTS) ChangeLights(MED_POWER_LIGHTS);
		}
		else{
			surface.FontColor = HIGH_POWER_COLOR;
			if(CONTROL_LIGHTS) ChangeLights(HIGH_POWER_LIGHTS);
		}
	}
	
	
	if(charging){
		if(percent > StopChargeAt){
			foreach(IMyReactor r in reactors){
				r.ApplyAction("OnOff_Off");
			}
			charging = false;
		}
		else{
			foreach(IMyReactor r in reactors){
				r.ApplyAction("OnOff_On");
			}
		}
	}
	previousPercent = percent;
}

bool firstTime = false;
List<IMyLightingBlock> lights;
void FirstTimeSetup(){
	Echo("Performing First Time Setup!");
	lights = new List<IMyLightingBlock>();
	GridTerminalSystem.GetBlocksOfType<IMyLightingBlock>(lights, b => b.IsSameConstructAs(Me));
	
	for(int i = lights.Count - 1 ; i >= 0 ; i --){
		if(lights.ElementAt(i).CustomData.Contains(EXCLUSION_WORD)){
			lights.RemoveAt(i);
		}
	}
	pbSurface = Me.GetSurface(0);
	Echo("First Time Setup Complete!");
	firstTime = true;
}


int r = 0;
IMyTextSurface pbSurface;
void Running(){
	string run = "";
	switch(r){
		case 0: run = "\\";
		break;
		case 1: run = "|";
		break;
		case 2: run = "/";
		break;
		case 3: run = "-";
		r = -1;
		break;
	}
	r ++;
	
	pbSurface.ContentType = ContentType.TEXT_AND_IMAGE;
	pbSurface.FontSize = 1f;
	pbSurface.Font = "Monospace";
	pbSurface.Alignment = VRage.Game.GUI.TextPanel.TextAlignment.CENTER;
	pbSurface.TextPadding = 30f;
	string s = "Running " + run; 
	s += "\n" + "Lights: " + lights.Count;
	s += "\n Controlling Lights: " + CONTROL_LIGHTS;
	
	pbSurface.WriteText(s);
}


void ChangeLights(Color c){
	foreach(IMyLightingBlock light in lights){
		light.Color = c;
	}
}