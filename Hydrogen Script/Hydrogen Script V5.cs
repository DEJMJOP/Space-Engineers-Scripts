//===========================================================================
//Config
//===========================================================================
string COCKPIT_NAME = "Control Seat";
int surface = 1;

//===========================================================================
//End Of Config
//===========================================================================

public Program(){
	Runtime.UpdateFrequency = UpdateFrequency.Update10;
	FirstTime();
}

public void Main(){
	Step();
	switch(currentStep){
		case 0:
		Step0();
		break;
		case 1:
		Step1();
		break;
		case 2:
		Step2();
		break;
		case 3:
		Step3();
		break;
		case 4:
		Step4();
		break;
		case 5:
		Step5();
		break;
	}
	
}

double HYDROGEN_PERCENT = 0; double HYDROGEN_THIS_TICK = 0; double HYDROGEN_PREVIOUS_TICK;
double OXYGEN_PERCENT = 0; double OXYGEN_THIS_TICK = 0; double OXYGEN_PREVIOUS_TICK;
double HYDROGEN_LITRES = 0; double OXYGEN_LITRES = 0; double REQUIRED_ICE = 0;
void Step0(){
	double totalFilled = 0;
	HYDROGEN_LITRES = 0; OXYGEN_LITRES = 0; REQUIRED_ICE = 0;
	if(HYDROGEN_TANKS.Count > 0){
		foreach(IMyGasTank t in HYDROGEN_TANKS){
			totalFilled += t.FilledRatio*100;
			HYDROGEN_LITRES += t.Capacity;
		}
		HYDROGEN_PERCENT = totalFilled / HYDROGEN_TANKS.Count;
	} else{
		HYDROGEN_PERCENT = 0;
	}
	totalFilled = 0;
	if(OXYGEN_TANKS.Count > 0){
		foreach(IMyGasTank t in OXYGEN_TANKS){
			totalFilled += t.FilledRatio*100;
			OXYGEN_LITRES += t.Capacity;
		}
		OXYGEN_PERCENT = totalFilled / OXYGEN_TANKS.Count;
	} else{
		OXYGEN_PERCENT = 0;
	}
	
	REQUIRED_ICE = (HYDROGEN_LITRES > OXYGEN_LITRES) ? (HYDROGEN_LITRES/5):(OXYGEN_LITRES/5);
	
	HYDROGEN_THIS_TICK = HYDROGEN_PERCENT - HYDROGEN_PREVIOUS_TICK;
	HYDROGEN_PREVIOUS_TICK = HYDROGEN_PERCENT;
	OXYGEN_THIS_TICK = OXYGEN_PERCENT - OXYGEN_PREVIOUS_TICK;
	OXYGEN_PREVIOUS_TICK = OXYGEN_PERCENT;
}
double HYDROGEN_TICKS_LEFT; double HYDROGEN_SECONDS_LEFT; double HYDROGEN_MINUTES_LEFT;
double OXYGEN_TICKS_LEFT; double OXYGEN_SECONDS_LEFT; double OXYGEN_MINUTES_LEFT;
void Step1(){
	if(HYDROGEN_THIS_TICK != 0){
		if(HYDROGEN_THIS_TICK > 0) HYDROGEN_TICKS_LEFT = ((100-HYDROGEN_PERCENT)/HYDROGEN_THIS_TICK);
		else HYDROGEN_TICKS_LEFT = ((HYDROGEN_PERCENT)/HYDROGEN_THIS_TICK) * -1;
		HYDROGEN_SECONDS_LEFT = (HYDROGEN_TICKS_LEFT);
	}
	if(OXYGEN_THIS_TICK != 0){
		if(OXYGEN_THIS_TICK > 0) OXYGEN_TICKS_LEFT = ((100-OXYGEN_PERCENT)/OXYGEN_THIS_TICK);
		else OXYGEN_TICKS_LEFT = ((OXYGEN_PERCENT)/OXYGEN_THIS_TICK) * -1;
		OXYGEN_SECONDS_LEFT = (OXYGEN_TICKS_LEFT);
	}
	HYDROGEN_MINUTES_LEFT = 0; OXYGEN_MINUTES_LEFT = 0;
	if(HYDROGEN_SECONDS_LEFT < 7200){
		while(HYDROGEN_SECONDS_LEFT >= 60){
			HYDROGEN_MINUTES_LEFT++;
			HYDROGEN_SECONDS_LEFT -= 60;
		}
	}
	else{
		HYDROGEN_SECONDS_LEFT = 0;
		HYDROGEN_MINUTES_LEFT = 240;
	}
	if(OXYGEN_SECONDS_LEFT < 7200){
		while(OXYGEN_SECONDS_LEFT >= 60){
			OXYGEN_MINUTES_LEFT++;
			OXYGEN_SECONDS_LEFT -= 60;
		}
	}
	else{
		OXYGEN_SECONDS_LEFT = 0;
		OXYGEN_MINUTES_LEFT = 240;
	}
}
string HYDROGEN_STATUS;
string OXYGEN_STATUS;
void Step2(){
	HYDROGEN_STATUS = "Idle"; OXYGEN_STATUS = "Idle";
	if(HYDROGEN_THIS_TICK > 0) HYDROGEN_STATUS = "Filling";
	else if(HYDROGEN_THIS_TICK < 0) HYDROGEN_STATUS = "EMPTYING";
	else if(HYDROGEN_PERCENT == 100) HYDROGEN_STATUS = "Full";
	else if(HYDROGEN_PERCENT == 0) HYDROGEN_STATUS = "Empty";
	if(OXYGEN_THIS_TICK > 0) OXYGEN_STATUS = "Filling";
	else if(OXYGEN_THIS_TICK < 0) OXYGEN_STATUS = "EMPTYING";
	else if(OXYGEN_PERCENT == 100) OXYGEN_STATUS = "Full";
	else if(OXYGEN_PERCENT == 0) OXYGEN_STATUS = "Empty";
}

double PREVIOUS_ICE = 0;
double CURRENT_ICE = 0;

void Step3(){
	CURRENT_ICE = 0;
	foreach(IMyCargoContainer cargo in CARGO_CONTAINERS){
		for(int i = 0 ; i < cargo.GetInventory(0).ItemCount; i ++){
			if(cargo.GetInventory(0).GetItemAt(i).Value.Type.SubtypeId == "Ice"){
				string temp = "" + cargo.GetInventory(0).GetItemAt(i).Value.Amount;
				CURRENT_ICE += Double.Parse(temp);
			}
		}
	}
	foreach(IMyGasGenerator cargo in OXYGEN_GENERATORS){
		for(int i = 0 ; i < cargo.GetInventory(0).ItemCount ; i ++){
			if(cargo.GetInventory(0).GetItemAt(i).Value.Type.SubtypeId == "Ice") {
				String temp = "" + cargo.GetInventory(0).GetItemAt(i).Value.Amount;
				CURRENT_ICE += Double.Parse(temp);
			}
		}
	}
}
double ICE_SECONDS_LEFT; double ICE_MINUTES_LEFT;
void Step4(){
	if(PREVIOUS_ICE != 0){
		double thisTick = CURRENT_ICE - PREVIOUS_ICE;
		double ticksLeft = 0;
		ICE_SECONDS_LEFT = 0;
		ICE_MINUTES_LEFT = 0;
		if(thisTick != 0){
			if(thisTick > 0) ticksLeft = ((PREVIOUS_ICE-CURRENT_ICE)/thisTick);
			else ticksLeft = (CURRENT_ICE/thisTick) * -1;
		}
		ICE_SECONDS_LEFT = ticksLeft;
		if(ICE_SECONDS_LEFT < 7200){
			while(ICE_SECONDS_LEFT >= 60){
				ICE_MINUTES_LEFT ++;
				ICE_SECONDS_LEFT -= 60;
			}
		}
		else{
			ICE_SECONDS_LEFT = 0;
			ICE_MINUTES_LEFT = 240;
		}
	}
	PREVIOUS_ICE = CURRENT_ICE;
}

string PANEL_NAME = "[H2O2 LCD]";
void Step5(){
	List<IMyTerminalBlock> panels = new List<IMyTerminalBlock>();
	GridTerminalSystem.SearchBlocksOfName(PANEL_NAME, panels, block => block.IsSameConstructAs(Me));
	
	bool hydrogen = false; bool oxygen = false; bool ice = false;
	
	foreach(IMyTerminalBlock panel in panels){
		if(panel is IMyTextPanel){
			hydrogen = false;
			oxygen = false;
			ice = false;
			if(panel.CustomData.Contains("Hydrogen")) hydrogen = true;
			if(panel.CustomData.Contains("Oxygen")) oxygen = true;
			if(panel.CustomData.Contains("Ice")) ice = true;
			WriteToLCD(panel as IMyTextSurface,hydrogen,oxygen,ice);
		}
	}
	hydrogen = false; oxygen = false; ice = false;
	if(COCKPIT != null){
		if(COCKPIT.CustomData.Contains("Hydrogen")) hydrogen = true;
		if(COCKPIT.CustomData.Contains("Oxygen")) oxygen = true;
		if(COCKPIT.CustomData.Contains("Ice")) ice = true;
		WriteToLCD(COCKPIT_SCREEN,hydrogen,oxygen,ice);
	}
	
}


void WriteToLCD(IMyTextSurface surface, bool hydrogen, bool oxygen, bool ice){
	surface.ContentType = ContentType.TEXT_AND_IMAGE;
	surface.FontSize = .75f;
	surface.Font = "Monospace";
	surface.Alignment = VRage.Game.GUI.TextPanel.TextAlignment.LEFT;
	surface.TextPadding = 0f;
	string s = "===================================";
	if(hydrogen){
		s += NewLineString("Hydrogen Tanks: " + HYDROGEN_TANKS.Count);
		s += NewLineString("Hydrogen Filled: " + HYDROGEN_PERCENT.ToString("##0.#0") + "%");
		s += NewLineString("Time Left: " + HYDROGEN_MINUTES_LEFT + "m " + HYDROGEN_SECONDS_LEFT.ToString("0#") + "s");
		s += NewLineString("Status: " + HYDROGEN_STATUS);
		s += NewLineString("-----------------------------------");
	}
	if(oxygen){
		s += NewLineString("Oxygen Tanks: " + OXYGEN_TANKS.Count);
		s += NewLineString("Oxygen Filled: " + OXYGEN_PERCENT.ToString("##0.#0") + "%");
		s += NewLineString("Time Left: " + OXYGEN_MINUTES_LEFT + "m " + OXYGEN_SECONDS_LEFT.ToString("0#") + "s");
		s += NewLineString("Status: " + OXYGEN_STATUS);
		s += NewLineString("-----------------------------------");
	}
	if(ice){
		s += NewLineString("Ice: " + CURRENT_ICE.ToString("#,###0"));
		s += NewLineString("Ice Time: " + ICE_MINUTES_LEFT + "m " + ICE_SECONDS_LEFT.ToString("0#") + "s");
		s += NewLineString("Required Ice: " + REQUIRED_ICE.ToString("#,###0"));
		
		double icePercent = CURRENT_ICE / REQUIRED_ICE * 100;
		
		string reqIcePercent = "[";
		for(int i = 1 ; i < 11; i++){
			if(icePercent/10 > i) reqIcePercent += "|";
			else reqIcePercent += "-";
		}
		reqIcePercent += "]";
		
		s += NewLineString(reqIcePercent);
		s += NewLineString("Ice Percent: " + icePercent.ToString("##0.#0"));
	}
	
	s += NewLineString("===================================");
	
	
	surface.WriteText(s);
}

string NewLineString(string s){
	return "\n"+s;
}
string NewString(string s){
	return s;
}


List<IMyGasTank> HYDROGEN_TANKS;
List<IMyGasTank> OXYGEN_TANKS;
List<IMyGasGenerator> OXYGEN_GENERATORS;
List<IMyCargoContainer> CARGO_CONTAINERS;
IMyTextSurface COCKPIT_SCREEN;
IMyTerminalBlock COCKPIT;
void FirstTime(){
	Echo("Performing First Time Setup!");
	HYDROGEN_TANKS = new List<IMyGasTank>();
	OXYGEN_TANKS = new List<IMyGasTank>();
	OXYGEN_GENERATORS = new List<IMyGasGenerator>();
	CARGO_CONTAINERS = new List<IMyCargoContainer>();
	
	List<IMyGasTank> allTanks = new List<IMyGasTank>();
	GridTerminalSystem.GetBlocksOfType<IMyGasTank>(allTanks, block => block.IsSameConstructAs(Me));
	
	Echo("All Tanks: " + allTanks.Count);
	
	foreach(IMyGasTank tank in allTanks){
		if(tank.DetailedInfo.Contains("Oxygen Tank")) OXYGEN_TANKS.Add(tank);
		else if (tank.DetailedInfo.Contains("Hydrogen Tank")) HYDROGEN_TANKS.Add(tank);
	}
	GridTerminalSystem.GetBlocksOfType<IMyGasGenerator>(OXYGEN_GENERATORS, block => block.IsSameConstructAs(Me));
	GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(CARGO_CONTAINERS, block => block.IsSameConstructAs(Me));
	
	Echo("Hydrogen Tanks: " + HYDROGEN_TANKS.Count);
	Echo("Oxygen Tanks: " + OXYGEN_TANKS.Count);
	COCKPIT = GridTerminalSystem.GetBlockWithName(COCKPIT_NAME);
	IMyTextSurfaceProvider c = COCKPIT as IMyTextSurfaceProvider;
	COCKPIT_SCREEN = c.GetSurface(surface); 
	
}

int currentStep = -1;
void Step(){
	currentStep++;
	if(currentStep == 6) currentStep = 0;
}