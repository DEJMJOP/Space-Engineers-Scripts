//===========================================================================
//Config
//---------------------------------------------------------------------------
	//The text you want the LCD's to contain
	string PANEL_NAME = "[Hydrogen]";
	//The text you want hydrogen tanks to contain
	string HYDROGEN_TANK_NAME = "Hydrogen Tank";
	
	int notches = 30;
//---------------------------------------------------------------------------
//End Of Config
//===========================================================================


public Program(){
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
}

double previousHydrogen = 0;

public void Main(string argument, UpdateType updateSource){
	calculateIce();
	calculateHydrogen();
	
}

private void calculateHydrogen(){
	//Get all hydrogen tanks
    List<IMyTerminalBlock> tanks = new List<IMyTerminalBlock>();
    GridTerminalSystem.SearchBlocksOfName(HYDROGEN_TANK_NAME, tanks);
	//Work out amount of Hydrogen tanks and how filled they are
	double totalAmt = 0;
    double totalFilled = 0;
    foreach(IMyGasTank gas in tanks){
        totalAmt += 1;
        totalFilled += (gas.FilledRatio*100);
    }
	//Turn into a percentage out of 100
    totalFilled /= totalAmt;

	//Calculate change this tick
    double thisTick = totalFilled-previousHydrogen;
	
	//Work out ticks until complete
	double ticksLeft = 0;
    double secondsLeft = 0;
	if(thisTick != 0){
        if(thisTick > 0){
            ticksLeft = ((100-totalFilled)/thisTick);
        }
        else{
            ticksLeft = (totalFilled/thisTick) * -1;
        }
		//Convert into seconds
        secondsLeft = ticksLeft/6;
    }
	double minutesLeft = 0;
	if(secondsLeft < 3600){
		//Convert seconds to minutes and seconds
		while(secondsLeft > 60){
			minutesLeft++;
			secondsLeft -= 60;
		}
	}
	else{
		secondsLeft = 0;
		minutesLeft = 120;
	}
	
	
	//Work out status
    string status = "Idle";
    if(thisTick > 0) status = "Filling";
    else if(thisTick < 0) status = "Emptying";
    else if(totalFilled == 100) status = "Full";
	//Displaying on LCDs
	List<IMyTerminalBlock> panels = new List<IMyTerminalBlock>();
    GridTerminalSystem.SearchBlocksOfName(PANEL_NAME, panels);
    foreach(IMyTerminalBlock panel in panels){
		if(panel is IMyTextPanel){
			((IMyTextPanel)panel).WritePublicText("Hydrogen Tanks: " + totalAmt + "\n", false);
			((IMyTextPanel)panel).WritePublicText("Hydrogen Filled: " + totalFilled.ToString("###.##") + "%\n", true);
			
			//
			((IMyTextPanel)panel).WritePublicText("[", true);
			for(int i = 0 ; i < notches ; i ++){
				if((totalFilled/(100/notches)) >= i) ((IMyTextPanel)panel).WritePublicText("|", true);
				else ((IMyTextPanel)panel).WritePublicText("-", true);
			}
			((IMyTextPanel)panel).WritePublicText("]\n", true);
			//
			
			((IMyTextPanel)panel).WritePublicText("Status: " + status + "\n", true);
			((IMyTextPanel)panel).WritePublicText("Time Left Hydrogen: " + minutesLeft + "m " + secondsLeft.ToString("0#") + "s \n" , true);
			
			((IMyTextPanel)panel).WritePublicText("\nIce: " + previousIce.ToString("#,##0") + "\n", true);
			((IMyTextPanel)panel).WritePublicText("Time Left Ice: " + iceMinutes + "m " + iceSeconds.ToString("0#") + "s \n" , true);
		}
		else{
			Echo("--------------------------------------------------");
			Echo("Something contains \"" + PANEL_NAME + "\" that isn't an LCD");
			Echo(panel.CustomName);
			Echo("--------------------------------------------------");
		}
    }
    //Reset filled amount
    previousHydrogen = totalFilled;
}

double previousIce = 0;
double iceSeconds = 0;
double iceMinutes = 0;
private void calculateIce(){
	double currentIce = 0;
	
	List<IMyTerminalBlock> cargoContainers = new List<IMyTerminalBlock>();
	GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(cargoContainers);
	List<IMyTerminalBlock> oxygenGenerators = new List<IMyTerminalBlock>();
	GridTerminalSystem.GetBlocksOfType<IMyGasGenerator>(oxygenGenerators);
	
	Echo("Cargo Containers: " + cargoContainers.Count);
	Echo("O2/H2 Generators: " + oxygenGenerators.Count);
	
	List<IMyInventoryItem> items;
	
	foreach(IMyCargoContainer cargo in cargoContainers){
		items = new List<IMyInventoryItem>();
		items = cargo.GetInventory(0).GetItems();
		
		for(int i = 0 ; i < items.Count ; i ++){
			string[] split = items[i].ToString().Split('/');
			if(split[1] != "Ice") continue;
			currentIce += Double.Parse(""+items[i].Amount);
			
			Echo("Item: " + split[1] + ": " + items[i].Amount);
		}
	}
	
	foreach(IMyGasGenerator og in oxygenGenerators){
		items = new List<IMyInventoryItem>();
		items = og.GetInventory(0).GetItems();
		
		for(int i = 0 ; i < items.Count ; i ++){
			string[] split = items[i].ToString().Split('/');
			if(split[1] != "Ice") continue;		
			currentIce += Double.Parse(""+items[i].Amount);
			
			Echo("Item: " + split[1] + ": " + items[i].Amount);
		}
	}
	
	Echo("Previous Ice: " + previousIce);
	Echo("Total Ice: " + currentIce);
	
	double thisTick = currentIce - previousIce;
	double ticksLeft = 0;
	double secondsLeft = 0;
	
	if(thisTick != 0){
        if(thisTick > 0){
            ticksLeft = ((previousIce-currentIce)/thisTick);
        }
        else{
            ticksLeft = (currentIce/thisTick) * -1;
        }
        secondsLeft = ticksLeft/6;
    }
	double minutesLeft = 0;
	if(secondsLeft < 3600){
		while(secondsLeft > 60){
			minutesLeft++;
			secondsLeft -= 60;
		}
	}
	else{
		secondsLeft = 0;
		minutesLeft = 120;
	}
	
	iceSeconds = secondsLeft;
	iceMinutes = minutesLeft;
	
	previousIce = currentIce;
}


