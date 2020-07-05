//===========================================================================
//Config
//---------------------------------------------------------------------------
string PANEL_NAME = "[Battery LCD]";
string BATTERY_NAME = "[Battery]";
string SEAT_NAME = "Flight Seat";
int notches = 50;
//---------------------------------------------------------------------------
//End Of Config
//===========================================================================

public Program(){
	Runtime.UpdateFrequency = UpdateFrequency.Update10;
}
int n = 0;
private void runningIndicator(){
	//IMyTextSurfaceProvider blk = GridTerminalSystem.GetBlockWithName(SEAT_NAME) as IMyTextSurfaceProvider;
	IMyTextSurfaceProvider blk = Me as IMyTextSurfaceProvider;
	IMyTextSurface surface = blk.GetSurface(0);
	String s = "Battery Script\n";
	if(n == 0){
		s += "Running.  ";
		n++;
	}
	else if(n == 1){
		s += "Running.. ";
		n++;
	}
	else if(n == 2){
		s += "Running...";
		n=0;
	}
	if(surface != null){
		surface.ContentType = ContentType.SCRIPT;
		using(var frame = surface.DrawFrame()){
			
			MySprite text = MySprite.CreateText(s, "Monospace", new Color(1f,1f,0f), 1f, TextAlignment.CENTER);
		//	text.Position = new Vector2(250f,50f);
			frame.Add(text);
		}
	}
    
}

public void Main(){
	if(firstTime) FirstTime();
	runningIndicator();
	step();
	switch(currentStep){
		case 0:
			step0();
			break;
		case 1:
			step1();
			break;
		case 2:
			step2();
			break;
		case 3:
			step3();
			break;
		case 4:
		//	step4();
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


List<float> BATTERY_POWER;
void step0(){
	BATTERY_POWER = new List<float>();
	for(int i = 0 ; i < BATTERIES.Count ; i ++){
		float tempPower = ((IMyBatteryBlock)BATTERIES.ElementAt(i)).CurrentStoredPower;
		
		BATTERY_POWER.Add(tempPower);
		
		
	}
}
float MAXPOWER;
float CURRENTPOWER;
void step1(){
	MAXPOWER = 0;
	CURRENTPOWER = 0;
	for(int i = 0 ; i < BATTERIES.Count ; i++){
		MAXPOWER += ((IMyBatteryBlock)BATTERIES.ElementAt(i)).MaxStoredPower;
		CURRENTPOWER += BATTERY_POWER.ElementAt(i);
	}
}
void step2(){}
void step3(){}
void step4(){
	IMyTextSurfaceProvider blk = GridTerminalSystem.GetBlockWithName(SEAT_NAME) as IMyTextSurfaceProvider;
	IMyTextSurface surface = blk.GetSurface(0);
	
	if(surface != null){
		surface.ContentType = ContentType.SCRIPT;
		using(var frame = surface.DrawFrame()){
			
			MySprite text = MySprite.CreateText("Power: " + (CURRENTPOWER).ToString("0.##") + " MWh / " + MAXPOWER + " MWh", "Monospace", new Color(1f,1f,0f), 1f, TextAlignment.CENTER);
		//	text.Position = new Vector2(250f,50f);
			frame.Add(text);
		}
	}
	
}
void step5(){
	foreach(IMyTerminalBlock panel in PANELS){
		WriteToPanelLn("",panel,false);
		for(int i = 0 ; i < BATTERY_POWER.Count ; i++){
			WriteToPanel("Battery " + (i+1) + ": " + (BATTERY_POWER.ElementAt(i)).ToString("0.##") + " MWh", panel, true);
			WriteToPanelLn(" | " + ((BATTERY_POWER.ElementAt(i)/((IMyBatteryBlock)BATTERIES.ElementAt(i)).MaxStoredPower)*100).ToString("0.##") + "%", panel, true);
			
			if( ((IMyBatteryBlock)BATTERIES.ElementAt(i)).IsCharging) WriteToPanelLn("Charging",panel,true);
			else WriteToPanelLn("Discharging",panel,true);
			
			WriteToPanel("[",panel,true);
			
			for(int j = 0 ; j < notches ; j ++){
				if( (((BATTERY_POWER.ElementAt(i)/((IMyBatteryBlock)BATTERIES.ElementAt(i)).MaxStoredPower)*100)/(100/notches)) >= j) WriteToPanel("|",panel,true);
				else WriteToPanel(" ",panel,true);
			}
			
			WriteToPanelLn("]",panel,true);
		}
	}
}

bool firstTime = true;
List<IMyTerminalBlock> BATTERIES;
List<IMyTerminalBlock> PANELS;
void FirstTime(){
	BATTERIES = new List<IMyTerminalBlock>();
	GridTerminalSystem.SearchBlocksOfName(BATTERY_NAME, BATTERIES, block => block.IsSameConstructAs(Me));
	
	PANELS = new List<IMyTerminalBlock>();
	GridTerminalSystem.SearchBlocksOfName(PANEL_NAME, PANELS, block => block.IsSameConstructAs(Me));
	
	
	firstTime = false;
}

void WriteToPanelLn(string s, IMyTerminalBlock panel, bool b){
	WriteToPanel(s + "\n",panel,b);
}
void WriteToPanel(string s, IMyTerminalBlock panel, bool b){
	((IMyTextPanel)panel).WritePublicText(s,b);
}
