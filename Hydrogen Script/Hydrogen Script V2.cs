//===========================================================================
//Config
//---------------------------------------------------------------------------
	//The text you want the LCD's to contain
	string PANEL_NAME = "[Hydrogen LCD]";
	//The text you want hydrogen tanks to contain
	string HYDROGEN_TANK_NAME = "Hydrogen Tank";
	
	int notches = 25;
//---------------------------------------------------------------------------
//End Of Config
//===========================================================================

public Program(){
	Runtime.UpdateFrequency = UpdateFrequency.Update10;
}

public void Main(){
	if(firstTime) FirstTime();
	step();
	switch(currentStep){
		case 0:
		//Calculate Amount of Hydrogen/Ticks left
			step0();
			break;
		case 1:
		//Time till Hydrogen is empty
			step1();
			break;
		case 2:
		//Hydrogen Status
			step2();
			break;
		case 3:
		//Amount of Ice
			step3();
			break;
		case 4:
		//Ice ticks left and time
			step4();
			break;
		case 5:
		//Display on LCDs
			step5();
			break;
		
	}
}
int currentStep = -1;
public void step(){
	currentStep++;
	if(currentStep == 7) currentStep = 0;
}


//Calculate Amount of Hydrogen/Ticks left
double HYDROGEN_PERCENT = 0;
double HYDROGEN_TICKS_LEFT = 0;
double HYDROGEN_PREVIOUS_TICK = 0;
double HYDROGEN_THIS_TICK = 0;
void step0(){
	double totalFilled = 0;
	
	for(int i = 0 ; i < HYDROGEN_TANKS.Count ; i ++){
		totalFilled += ((IMyGasTank)HYDROGEN_TANKS.ElementAt(i)).FilledRatio*100;
	}
	HYDROGEN_PERCENT = totalFilled / HYDROGEN_TANKS.Count;
	
	HYDROGEN_THIS_TICK = HYDROGEN_PERCENT - HYDROGEN_PREVIOUS_TICK;
	
	HYDROGEN_PREVIOUS_TICK = HYDROGEN_PERCENT;
}
//Time till Hydrogen is empty
double HYDROGEN_SECONDS_LEFT;
double HYDROGEN_MINUTES_LEFT;
void step1(){
	if(HYDROGEN_THIS_TICK != 0){
		if(HYDROGEN_THIS_TICK > 0) HYDROGEN_TICKS_LEFT = ((100-HYDROGEN_PERCENT)/HYDROGEN_THIS_TICK);
		else HYDROGEN_TICKS_LEFT = ((HYDROGEN_PERCENT)/HYDROGEN_THIS_TICK) * -1;
		HYDROGEN_SECONDS_LEFT = (HYDROGEN_TICKS_LEFT);
	}
	HYDROGEN_MINUTES_LEFT = 0;
	if(HYDROGEN_SECONDS_LEFT < 3600){
		while(HYDROGEN_SECONDS_LEFT > 60){
			HYDROGEN_MINUTES_LEFT++;
			HYDROGEN_SECONDS_LEFT -= 60;
		}
	}
	else{
		HYDROGEN_SECONDS_LEFT = 0;
		HYDROGEN_MINUTES_LEFT = 120;
	}
}
//Hydrogen Status
string HYDROGEN_STATUS;
void step2(){
	HYDROGEN_STATUS = "Idle";
	if(HYDROGEN_THIS_TICK > 0) HYDROGEN_STATUS = "Filling";
	else if(HYDROGEN_THIS_TICK < 0) HYDROGEN_STATUS = "EMPTYING";
	else if(HYDROGEN_PERCENT == 100) HYDROGEN_STATUS = "Full";
	else if(HYDROGEN_PERCENT == 0) HYDROGEN_STATUS = "Empty";
}
//Amount of Ice
List<IMyInventoryItem> items;
double PREVIOUS_ICE = 0;
double CURRENT_ICE = 0;
void step3(){
	CURRENT_ICE = 0;
	
	foreach(IMyCargoContainer cargo in CARGO_CONTAINERS){
		items = new List<IMyInventoryItem>();
		items = cargo.GetInventory(0).GetItems();
		for(int i = 0 ; i < items.Count ; i ++){
			string[] split = items[i].ToString().Split('/');
			if(split[1] != "Ice") continue;
			CURRENT_ICE += Double.Parse(""+items[i].Amount);
		}
	}
	foreach(IMyGasGenerator cargo in OXYGEN_GENERATORS){
		items = new List<IMyInventoryItem>();
		items = cargo.GetInventory(0).GetItems();
		for(int i = 0 ; i < items.Count ; i ++){
			string[] split = items[i].ToString().Split('/');
			if(split[1] != "Ice") continue;
			CURRENT_ICE += Double.Parse(""+items[i].Amount);
		}
	}
}
//Ice ticks left and time
double ICE_SECONDS_LEFT;
double ICE_MINUTES_LEFT;
void step4(){
	if(PREVIOUS_ICE != 0){
		double thisTick = CURRENT_ICE - PREVIOUS_ICE;
		Echo("This Tick: " + thisTick);
		double ticksLeft = 0;
		ICE_SECONDS_LEFT = 0;
		ICE_MINUTES_LEFT = 0;
		
		if(thisTick != 0){
			if(thisTick > 0) ticksLeft = ((PREVIOUS_ICE-CURRENT_ICE)/thisTick);
			else ticksLeft = (CURRENT_ICE/thisTick) * -1;
		}
		
		ICE_SECONDS_LEFT = ticksLeft;
		
		if(ICE_SECONDS_LEFT < 3600){
			while(ICE_SECONDS_LEFT > 60){
				ICE_MINUTES_LEFT ++;
				ICE_SECONDS_LEFT -= 60;
			}
		}
		else{
			ICE_SECONDS_LEFT = 0;
			ICE_MINUTES_LEFT = 120;
		}
	}
	PREVIOUS_ICE = CURRENT_ICE;
}
//DISPLAY
void step5(){
	List<IMyTerminalBlock> panels = new List<IMyTerminalBlock>();
	GridTerminalSystem.SearchBlocksOfName(PANEL_NAME, panels, block => block.IsSameConstructAs(Me));
	foreach(IMyTerminalBlock panel in panels){
		if(panel is IMyTextPanel){
			WriteToPanel("", panel, false);
			//Hydrogen
			WriteToPanelLn("Hydrogen Tanks: " + HYDROGEN_TANKS.Count, panel, true);
			WriteToPanelLn("Hydrogen Filled: " + HYDROGEN_PERCENT.ToString("###.##") + "%", panel, true);
			WriteToPanel("[", panel, true);
			for(int i = 0 ; i < notches ; i ++){
				if((HYDROGEN_PERCENT/(100/notches)) >= i) WriteToPanel("|", panel, true);
				else WriteToPanel("-", panel, true);
			}
			WriteToPanelLn("]", panel, true);
			WriteToPanelLn("Status: " + HYDROGEN_STATUS, panel, true);
			WriteToPanelLn("Time Left: " + HYDROGEN_MINUTES_LEFT + "m " + HYDROGEN_SECONDS_LEFT.ToString("0#") + "s", panel, true);
			
			WriteToPanelLn("", panel, true);
			//Ice
			WriteToPanelLn("Ice: " + CURRENT_ICE.ToString("#,##0"), panel, true);
			WriteToPanelLn("Ice Time Left: " + ICE_MINUTES_LEFT + "m " + ICE_SECONDS_LEFT.ToString("0#") + "s", panel, true);
		}
	}
}

//First Time Setup
bool firstTime = true;
List<IMyTerminalBlock> HYDROGEN_TANKS;
List<IMyTerminalBlock> CARGO_CONTAINERS;
List<IMyTerminalBlock> OXYGEN_GENERATORS;
void FirstTime(){
	HYDROGEN_TANKS = new List<IMyTerminalBlock>();
	CARGO_CONTAINERS = new List<IMyTerminalBlock>();
	OXYGEN_GENERATORS = new List<IMyTerminalBlock>();
	
	GridTerminalSystem.SearchBlocksOfName(HYDROGEN_TANK_NAME, HYDROGEN_TANKS, block => block.IsSameConstructAs(Me));
	GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(CARGO_CONTAINERS, block => block.IsSameConstructAs(Me));
	GridTerminalSystem.GetBlocksOfType<IMyGasGenerator>(OXYGEN_GENERATORS, block => block.IsSameConstructAs(Me));
	firstTime = false;
}

void WriteToPanelLn(string s, IMyTerminalBlock panel, bool b){
	WriteToPanel(s + "\n",panel,b);
}
void WriteToPanel(string s, IMyTerminalBlock panel, bool b){
	((IMyTextPanel)panel).WritePublicText(s, b);
}