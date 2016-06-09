//	PeggyFootsteps.cs
//--------------------------------------------------------------------------------------
// Title:			PeggyFootsteps
// Author: 			Peggworth the Pirate
// Description:		Footsteps that change noise based on what you touch below you.
//--------------------------------------------------------------------------------------

//--------------------------------------------------------------------------------------
//		Misc. Functions:
//--------------------------------------------------------------------------------------

function rgbToHex(%rgb) //! Ripped from Greek2Me's SLAYER
{
	// use % to find remainder
	%r = getWord(%rgb,0);
	%g = getWord(%rgb,1);
	%b = getWord(%rgb,2);
	// in-case the rgb value isn't on the right scale
	%r = ( %r <= 1 ) ? %r * 255 : %r;
	%g = ( %g <= 1 ) ? %g * 255 : %g;
	%b = ( %b <= 1 ) ? %b * 255 : %b;
	// the hexidecimal numbers
	%a = "0123456789ABCDEF";

	%r = getSubStr(%a,(%r-(%r % 16))/16,1) @ getSubStr(%a,(%r % 16),1);
	%g = getSubStr(%a,(%g-(%g % 16))/16,1) @ getSubStr(%a,(%g % 16),1);
	%b = getSubStr(%a,(%b-(%b % 16))/16,1) @ getSubStr(%a,(%b % 16),1);

	return %r @ %g @ %b;
}


//--------------------------------------------------------------------------------------
//		Footstep Deciders:
//--------------------------------------------------------------------------------------

//+++	Return the sound datablock based off the surface.
function PeggFootsteps_getSound(%surface, %speed)
{
		switch$ ( %surface )
		{	
			// swimmingfx only has one speed. There is no walking speed for swimmingfx.
			case "under water":   
				return $StepSwimming[getRandom(1,6)];
			
			case "metal":
				if ( %speed $= "walking" )
					return $StepMetalW[getRandom(1,3)];
				else
					return $StepMetalR[getRandom(1,3)];
			
			case "dirt":
				if ( %speed $= "walking" )
					return $StepDirtW[getRandom(1,4)];
				else
					return $StepDirtR[getRandom(1,6)];
		
			case "grass":
				if ( %speed $= "walking" )
					return $StepGrassW[getRandom(1,4)];
				else
					return $StepGrassR[getRandom(1,6)];
		
			case "stone":
				if ( %speed $= "walking" )
					return $StepStoneW[getRandom(1,4)];
				else
					return $StepStoneR[getRandom(1,6)];
		
			case "water":
				if ( %speed $= "walking" )
					return $StepWaterW[getRandom(1,4)];
				else
					return $StepWaterR[getRandom(1,6)];
		
			case "wood":
				if ( %speed $= "walking" )
					return $StepWoodW[getRandom(1,4)];
				else
					return $StepWoodR[getRandom(1,6)];
			
			case "sand":
				if ( %speed $= "walking" )
					return $StepSandW[getRandom(1,4)];
				else
					return $StepSandR[getRandom(1,6)];
			
			case "basic":
				if ( %speed $= "walking" )
					return $StepBasicW[getRandom(1,4)];
				else
					return $StepBasicR[getRandom(1,4)];
			
			// snowsteps only have one speed. There is no walking sound effect for snowsteps
			case "snow" :		
				return $StepSnowR[getRandom(1,3)];
		}
}

//+++ 	Calculate the noise based off what the player is stepping on.
function checkPlayback(%obj)
{	
	%surface = ( %obj.touchcolor $= "" ) ? %obj.surface : %obj.touchColor;
	%speed = ( %obj.isSlow == 0 ) ? "running" : "walking";
	
	if ( %obj.touchColor $= "" )
	{		
		return PeggFootsteps_getSound(%surface, %speed);
	}
	else
	{
		%r = getWord(%surface, 0) * 255;
		%g = getWord(%surface, 1) * 255;
		%b = getWord(%surface, 2) * 255;
		
		for ( %i = 0; %i < $Pref::Server::PF::customsounds; %i++ )
		{
			%hit = $Pref::Server::PF::colorPlaysFX[%i];
			%hitcolor = getWords(%hit, 0, 3);
			%hr = getWord(%hitcolor, 0) * 255;
			%hg = getWord(%hitcolor, 1) * 255;
			%hb = getWord(%hitcolor, 2) * 255;
			if ( %hr == %r && %hg == %g && %hb == %b)
			{
				%hitsound = getWord(%hit, 4);
				return PeggFootsteps_getSound(%hitsound, %speed);
			}
		}

		if (%r >= %b && %b >= %g || (%r > 180 && %g > 180))
		{
			%surface = "dirt";
		}	
		else if (%r >= %g && %g > %b)
		{
			%surface = "wood";
		}
		else if (%g >= %b && %g > %r)
		{	
			%surface = "grass";
		}
		else
		{	
			%surface = "stone";
		}
		if (%r == %b && %r == %g && %b == %g)
		{	
			%surface = "stone";
			if(%r > 230 && %g > 230 && %b > 230)
			{	
				%surface = "snow";
			}
		}
	}
	if ( %obj.isSwimming )		// splashing in water, not under the water
	{
		%surface = "water";
	}
	return PeggFootsteps_getSound(%surface, %speed);
}

//+++ Return the surface's name when given a number from a list.
function parseSoundFromNumber(%val, %obj)
{
	//Default 0 Basic 1 Dirt 2 Grass 3 Metal 4 Sand 5 Snow 6 Stone 7 Water 8 Wood 9
	switch ( %val )
	{
		case 0:
			if ( %obj == -1 )
			{
				return "by color";
			}
			else
			{
				if ( isObject(%obj.lastBrick) && getColorIDTable(%obj.lastBrick.getColorId()) !$= "" )
				{
					return "";
				}
				return parseSoundFromNumber($Pref::Server::PF::defaultStep, %obj);
			}
		case 1:
			return "basic";
		case 2:
			return "dirt";
		case 3:
			return "grass";
		case 4:
			return "metal";
		case 5:
			return "sand";
		case 6:
			return "snow";
		case 7:
			return "stone";
		case 8:
			return "water";
		case 9:
			return "wood";
	}
}

//+++ When any lifeform spawns, create a footstep loop.
deactivatepackage(peggsteps);
package peggsteps
{	
	function Armor::onNewDatablock(%this, %obj)
	{
		// creatures like horses won't make footsteps, but other AI will
		if ( %this.rideable )	return parent::onNewDatablock(%this, %obj);
		%obj.touchcolor = "";
		%obj.surface = parseSoundFromNumber($Pref::Server::PF::defaultStep, %obj);
		%obj.isSlow = 0;
		%obj.peggstep = schedule(50,0,PeggFootsteps,%obj);
		return parent::onNewDatablock(%this, %obj);
	}	
};
activatepackage(peggsteps);


//--------------------------------------------------------------------------------------
//		Footstep Playback:
//--------------------------------------------------------------------------------------

//+++ Drop some rad peggstep noise in here!
function PeggFootsteps(%obj)
{	
	cancel(%obj.peggstep);	
	if($Pref::Server::PF::footstepsEnabled == 1 && isObject(%obj))
	{	
		if ( %obj.isMounted() )
		{
			%obj.peggstep = schedule(50,0,PeggFootsteps,%obj);
			return;
		}
		//! Ripped from Hata's support_footstep.cs
		%pos = %obj.getPosition();
		%xyP = getWords(%pos,0,1);
		%vel = %obj.getVelocity();
		%posA = %obj.getPosition();
		initContainerBoxSearch(%posA, "1.25 1.25 0.1", $TypeMasks::fxBrickObjectType | $Typemasks::TerrainObjectType | $TypeMasks::VehicleObjectType);
		%colA = containerSearchNext();
		%posB = vectorAdd(%posA,"0 0 0.5");
		initContainerBoxSearch(%posB, "1.25 1.25 0.1", $TypeMasks::fxBrickObjectType | $Typemasks::TerrainObjectType | $TypeMasks::VehicleObjectType);
		%colB = containerSearchNext();
		//	echo(%type);
		if( isObject(%colA) && %colA != %colB )
		{	
			%type = %colA.getClassName();
			if (  %type $= "fxDTSbrick" && %colA.isRendering() )
			{
					%obj.lastBrick =  %colA;
					// by default, the surface isn't decided yet, and will be decided by the color
					%obj.touchColor = getColorIDTable(%colA.getColorId()); 		
					%obj.surface = "";
					// check to see if there is a custom sound based on the brick's special FX
					if ( $Pref::Server::PF::brickFXsounds::enabled )
					{
						switch ( %colA.getColorFxID() )
						{
							case 1:				
								%obj.touchColor = "";
								%obj.surface = parseSoundFromNumber($Pref::Server::PF::brickFXsounds::pearlStep, %obj);
							case 2:				
								%obj.touchColor = "";
								%obj.surface = parseSoundFromNumber($Pref::Server::PF::brickFXsounds::chromeStep, %obj);
							case 3:
								%obj.touchColor = "";
								%obj.surface = parseSoundFromNumber($Pref::Server::PF::brickFXsounds::glowStep, %obj);
							case 4:	
								%obj.touchColor = "";
								%obj.surface = parseSoundFromNumber($Pref::Server::PF::brickFXsounds::blinkStep, %obj);
							case 5:
								%obj.touchColor = "";
								%obj.surface = parseSoundFromNumber($Pref::Server::PF::brickFXsounds::swirlStep, %obj);
							case 6:
								%obj.touchColor = "";
								%obj.surface = parseSoundFromNumber($Pref::Server::PF::brickFXsounds::rainbowStep, %obj);
						}
						if ( %colA.getShapeFxID() )
						{
							%obj.touchColor = "";
							%obj.surface = parseSoundFromNumber($Pref::Server::PF::brickFXsounds::unduloStep, %obj);
						}
					}
					// check to see if the brick has an event based custom sound
					if ( %colA.customStep !$= "" ) 
					{
						%obj.touchColor = "";
						%obj.surface = %colA.customStep;
					}
			}
			else if ( %type $= "fxPlane" )
			{
				%obj.touchColor = "";
					%obj.surface = parseSoundFromNumber($Pref::Server::PF::terrainStep, %obj);						
			}
			else if ( %type $= "WheeledVehicle" || %type $= "FlyingVehicle" )
			{
					%obj.touchColor = "";
					%obj.surface = parseSoundFromNumber($Pref::Server::PF::vehicleStep, %obj);
			}	
			else
			{
					%obj.touchColor = "";
					%obj.surface = parseSoundFromNumber($Pref::Server::PF::defaultStep, %obj);
			}
			if ( %obj.getWaterCoverage() > 0 )
			{
				%obj.surface = "water";
				%obj.touchColor = "";
			}
			%isGround = true;
		}
		else
		{	
			%isGround = false;
		}

		%obj.isSlow = ( mAbs(getWord(%vel, 0)) < $Pref::Server::PF::runningMinSpeed && mAbs(getWord(%vel, 1)) < $Pref::Server::PF::runningMinSpeed || %obj.isCrouched() );
		
		if( %obj.getWaterCoverage() > 0 && $Pref::Server::PF::waterSFX == 1 && %obj.lastXY !$= %xyP && !%isGround )
		{
			%obj.touchColor = "";
			%obj.surface = "under water";
			%obj.lastXY = %xyP;
			serverplay3d(checkPlayback(%obj), %obj.getHackPosition());
			%obj.peggstep = schedule(500, 0, PeggFootsteps, %obj);
		}
		else if( %obj.lastXY $= %xyP || !%isGround )
		{	
			%obj.peggstep = schedule(50, 0, PeggFootsteps, %obj);	
		}
		else
		{
			%obj.lastXY = %xyP;
			%obj.peggstep = schedule(320, 0, PeggFootsteps, %obj);
			serverplay3d(checkPlayback(%obj), %obj.getHackPosition());
		}
		return;
	}
	%obj.peggstep = schedule(1000, 0, PeggFootsteps, %obj);
}

	
//--------------------------------------------------------------------------------------
//		 Assigning custom sounds to specific bricks with events.
//--------------------------------------------------------------------------------------

registerOutputEvent("fxDTSBrick","setFootstep","List Clear 0 Basic 1 Dirt 2 Grass 3 Metal 4 Sand 5 Snow 6 Stone 7 Water 8 Wood 9");

function fxDTSBrick::setFootstep(%brick, %val, %client)
{
	if ( !%val )
	{
		%brick.customStep = "";
	}
	else
	{
		%brick.customStep = parseSoundFromNumber(%val, %client.player);
	}
}
