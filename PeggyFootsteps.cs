//	PeggyFootsteps.cs
//--------------------------------------------------------------------------------------
// Title: PeggyFootsteps
// Author: Peggworth the Pirate
// Description:  Footsteps that change noise based on what you touch below you.
//	
//				A set of footsteps ripped from an old diablo-esque game, and from 'FreeSound.org'.
//				We shall call them peggsteps and they will be the best.
//--------------------------------------------------------------------------------------

//--------------------------------------------------------------------------------------
//		Misc. Functions:
//--------------------------------------------------------------------------------------

function rgbToHex(%rgb) //! Ripped from Greek2Me's SLAYER
{
	//use % to find remainder
	%r = getWord(%rgb,0);
	if(%r <= 1)
		%r = %r * 255;
	%g = getWord(%rgb,1);
	if(%g <= 1)
		%g = %g * 255;
	%b = getWord(%rgb,2);
	if(%b <= 1)
		%b = %b * 255;
	%a = "0123456789ABCDEF";

	%r = getSubStr(%a,(%r-(%r % 16))/16,1) @ getSubStr(%a,(%r % 16),1);
	%g = getSubStr(%a,(%g-(%g % 16))/16,1) @ getSubStr(%a,(%g % 16),1);
	%b = getSubStr(%a,(%b-(%b % 16))/16,1) @ getSubStr(%a,(%b % 16),1);

	return %r @ %g @ %b;
}

//--------------------------------------------------------------------------------------
//		Main Functions:
//--------------------------------------------------------------------------------------

//++	A function that returns the sound datablock based off the surface.
function PeggFootsteps_getSound(%surface, %speed)
{
  // there is no different sounds for running and walking while swimming, so this is retrieved regardless of speed.
	if ( %surface $= "under water" )	
	{
		return $StepSwimming[getRandom(1,6)];
	}
  //walking
	else if ( %speed $= "walking" )		
	{
		if ( %surface $= "metal" )    // there are only 3 sound FX for metal steps, in both walking and running.
		{
			return $StepMetalW[getRandom(1,3)];
		}
		else if ( %surface $= "dirt" )
		{
			return $StepDirtW[getRandom(1,4)];
		}
		else if ( %surface $= "grass" )
		{
			return $StepGrassW[getRandom(1,4)];
		}
		else if ( %surface $= "stone" )
		{
			return $StepStoneW[getRandom(1,4)];
		}
		else if ( %surface $= "water" )
		{
			return $StepWaterW[getRandom(1,4)];
		}
		else if ( %surface $= "wood" )
		{
			return $StepWoodW[getRandom(1,4)];
		}		
    else if ( %surface $= "sand" )
		{
			return $StepSandW[getRandom(1,4)];
		}
		else if ( %surface $= "snow" )		// there is only 1 snow sound global array, and it's for running, so the global variable still has the 'R', not a 'W'
		{
			return $StepSnowR[getRandom(1,3)];
		}
	}
  // running
	else		
	{
		if ( %surface $= "metal" )    // there are only 3 sound FX for metal steps, in both walking and running.
		{
			return $StepMetalR[getRandom(1,3)];
		}
		else if ( %surface $= "dirt" )
		{
			return $StepDirtR[getRandom(1,6)];
		}		
    else if ( %surface $= "sand" )
		{
			return $StepSandR[getRandom(1,6)];
		}
		else if ( %surface $= "grass" )
		{
			return $StepGrassR[getRandom(1,6)];
		}
		else if ( %surface $= "stone" )
		{
			return $StepStoneR[getRandom(1,6)];
		}
		else if ( %surface $= "water" )
		{
			return $StepWaterR[getRandom(1,6)];
		}
		else if ( %surface $= "wood" )
		{
			return $StepWoodR[getRandom(1,6)];
		}
		else if ( %surface $= "snow" )
		{
			return $StepSnowR[getRandom(1,3)];
		}
	}
}

//++ 	A function that calculates the noice that will need to be produced.
function colorcheck_playback(%obj)
{	
	%surface = %obj.touchcolor;
	
	if ( %surface $= "under water" )
	{	
		return PeggFootsteps_getSound(%surface);
	}
	if(%obj.isSlow == 0)
	{	
		%speed = "running";
	}
	else
	{	
		%speed = "walking";
	}	
	if(%surface $= "metal")
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


//--------------------------------------------------------------------------------------
//		Package:
//			The actual functions where sounds are produced.		
//--------------------------------------------------------------------------------------

deactivatepackage(peggsteps);
package peggsteps
{	
	//++ When any creature spawns, start peggstepping.
	function Armor::onNewDatablock(%this, %obj)
	{
		if($Pref::Server::PF::footstepsEnabled == 0) 
		{
			return parent::onNewDatablock(%this, %obj);
		}
		if (%this.rideable) 
		{
			return parent::onNewDatablock(%this, %obj);
		}
		%obj.touchcolor = "0.5 0.5 0.5";
		%obj.isSlow = 0;
		%obj.peggstep = schedule(50,0,PeggFootsteps,%obj);
		return parent::onNewDatablock(%this, %obj);
	}
	
	//++ When any creature crouches, pay attention and take note!
	function Armor::onTrigger(%data,%obj,%slot,%val)
	{
		if(%slot == 3)
		{
			%obj.isProning = %val;
		}
		return Parent::onTrigger(%data,%obj,%slot,%val);
	}
	
	//++ (next two) When a creature enters or exits water, take note!
	function Armor::onEnterLiquid(%data,%obj, %cov, %type)
	{
		%obj.isSwimming=1;
		return Parent::onEnterLiquid(%data,%obj, %cov, %type);
	}
	function Armor::onLeaveLiquid(%data,%obj, %cov, %type)
	{
		%obj.isSwimming=0;
		return Parent::onLeaveLiquid(%data,%obj, %cov, %type);
	}
	//++ Drop some rad peggstep noise in here!
	function PeggFootsteps(%obj)
	{	
		if($Pref::Server::PF::footstepsEnabled == 1 && isObject(%obj))
		{	
			cancel(%obj.peggstep);	
			%pos = %obj.getPosition();
			%xyP = getWords(%pos,0,1);
			%vel = %obj.getVelocity();
			//! Ripped from Hata's support_footstep.cs
			%posA = %obj.getPosition();
			initContainerBoxSearch(%posA, "1.25 1.25 0.1", $TypeMasks::fxBrickObjectType|$Typemasks::TerrainObjectType);
			%colA = containerSearchNext();
			%posB = vectorAdd(%posA,"0 0 0.5");
			initContainerBoxSearch(%posB, "1.25 1.25 0.1", $TypeMasks::fxBrickObjectType|$Typemasks::TerrainObjectType);
			%colB = containerSearchNext();
			if(isObject(%colA) && %colA != %colB == true)
			{	
				if (%colA.getClassName() $= "fxDTSbrick")
				{
					%obj.touchcolor = getColorIDTable(%colA.getColorId()); 
					if( $Pref::Server::PF::specialMetalSFX && (%colA.getColorFxID() == 1 || %colA.getColorFxID() == 2) )	// if the surface has a chrome or pearl special fx applied, then play the metal sound fx.
					{  
						%obj.touchcolor = "metal";	
					}
				}
				else
				{
					%obj.touchcolor = "0.5 0.5 0.5";
				}
				%isGround = true;
			}
			else
			{	
				%isGround = false;
			}
			if(mAbs(getWord(%vel,0)) < $Pref::Server::PF::runningMinSpeed && mAbs(getWord(%vel,1)) < $Pref::Server::PF::runningMinSpeed)
			{	
				%obj.isSlow = 1;
			}
			else
			{
				%obj.isSlow = 0;
			}
			if(%obj.lastXY $= %xyP || %obj.isProning || !%isGround)
			{
				if(%obj.swimskip != 1)
				{ 
					%obj.swimskip = 1;
				}
				else
				{
					%obj.swimskip = 0;
				}
				if(%obj.isSwimming == 1 && $Pref::Server::PF::waterSFX == 1 && %obj.lastXY !$= %xyP)
				{	
					%obj.lastXY = %xyP;
					%obj.touchcolor = "water";
					if(%obj.swimskip == 0)
					{ 
						serverplay3d(colorcheck_playback(%obj),%obj.getHackPosition());
					}
					cancel(%obj.peggstep);
					%obj.peggstep = schedule(500,0,PeggFootsteps,%obj);
					return;
				}
				%obj.lastXY = %xyP;
				cancel(%obj.peggstep);
				%obj.peggstep = schedule(50,0,PeggFootsteps,%obj);
				return;
			}
			serverplay3d(colorcheck_playback(%obj),%obj.getHackPosition());
			%obj.lastXY = %xyP;
			%obj.peggstep = schedule(320,0,PeggFootsteps,%obj);
		}
	}
};
activatepackage(peggsteps);


//--------------------------------------------------------------------------------------
//		Commands:
//			Helpful ingame controls to start and stop peggstepping!
//--------------------------------------------------------------------------------------

//++-- Toggle Prefs:
function servercmdToggle(%client, %toggle)
{
	if(!%client.isAdmin && !%client.isSuperAdmin)
	{	
		messageClient(%client,'',"<color:ff0000>You dingus!<color:ffff00> Narry a man, save admin or super admin, hath the divine power to do hither command!");
	}
	else if ( %toggle $= "SwimmingFX" )
	{	
		if($Pref::Server::PF::waterSFX == 0)
		{
			messageClient(%client,'',"<color:00ff00>You have activated the swimming sound fx package.");
			chatMessageAll('',"<color:ffffff>Swimming sound effects are now enabled.");
			$Pref::Server::PF::waterSFX = 1;
		}
		else
		{
			messageClient(%client,'',"<color:ff0000>You have de-activated the swimming sound fx package.");
			chatMessageAll('',"<color:ff0000>Swimming sound effects are now disabled.");
			$Pref::Server::PF::waterSFX = 0;
		}
	}
	else if ( %toggle $= "footsteps" )
	{	
		if($Pref::Server::PF::footstepsEnabled == 0)
		{
			messageClient(%client,'',"<color:00ff00>You have activated the footstep sound fx package.");
			chatMessageAll('',"<color:ffffff>Footstep sound effects are now enabled.");
			$Pref::Server::PF::footstepsEnabled = 1;
		}
		else
		{
			messageClient(%client,'',"<color:ff0000>You have de-activated the footstep sound fx package.");
			chatMessageAll('',"<color:ff0000>Footstep sound effects are now disabled.");
			$Pref::Server::PF::footstepsEnabled = 0;
		}
	}
	else if ( %toggle $= "MetalSpecialFX" )
	{
		if($Pref::Server::PF::specialMetalSFX == 0)
		{
			messageClient(%client,'',"<color:00ff00>You have activated the special metal sound fx package.");
			chatMessageAll('',"<color:ffffff>Special metal sound effects are now enabled.");
			$Pref::Server::PF::specialMetalSFX = 1;
		}
		else
		{
			messageClient(%client,'',"<color:ff0000>You have de-activated the special metal sound fx package.");
			chatMessageAll('',"<color:ff0000>Special metal sound effects are now disabled.");
			$Pref::Server::PF::specialMetalSFX = 0;
		}
	}
	else
	{
		if ( %toggle $= "" )
		{
			messageClient(%client,'',"<color:ff0000>You must enter a parameter to use this command. Use '/pegghelp' to learn more.");
		}
		else
		{
			messageClient(%client,'',"<color:ff0000>" @ %toggle @ " is not a valid parameter. Use '/pegghelp' to learn more.");
		}
	}
}

//++-- Running Speed:
function servercmdSetMinRunSpeed(%client, %value)
{
	if(!%client.isAdmin && !%client.isSuperAdmin)
	{	
		messageClient(%client,'',"<color:ff0000>You dingus!<color:ffff00> Narry a man, save admin or super admin, hath the divine power to do hither command!");
	}
	else
	{	
		if(%value > 50)
		{
			messageClient(%client,'',"<color:ffffff>That value is too high to use as the minimum running speed.");
			return;
		}
		if(%value <= 0)
		{
			messageClient(%client,'',"<color:ffffff>That value is too low to use as the minimum runnings speed.");
			return;
		}
		$Pref::Server::PF::runningMinSpeed = %value;
		messageClient(%client,'',"<color:ffffff>You have now set the minimum running value to " @ %value @ ". Gowing below this speed will play a walking noise instead of running.");
	}
}

//++-- Basic help command
function servercmdpegghelp(%client)
{
	messageClient(%client,'',"\c6The following is a list of admin commands for the \c3PeggyFootsteps\c6 add-on:");
	messageClient(%client,'',"\c6/toggle Footsteps");
	messageClient(%client,'',"\c6/toggle MetalSpecialFX");
	messageClient(%client,'',"\c6/toggle SwimmingFX");
	messageClient(%client,'',"\c6/getPeggstepPrefs");
	messageClient(%client,'',"\c6/getCustomSounds");
	messageClient(%client,'',"\c6/setMinRunSpeed \c0<\c3decimal value\c0>\c6");
	messageClient(%client,'',"\c6/setColorToSound \c0<\c3sound\c0>\c6 (This command will set the current color you have selected on your paint selector to play a sound you select.)");
	messageClient(%client,'',"\c6/clearCustomSounds (This command will remove all custom SFX)");
	messageClient(%client,'',"\c6/clearCustomSound (This command will remove the SFX for the color currently selected on your paint selector.)");
}

//--------------------------------------------------------------------------------------
//		 Assigning custom sounds to colors.
//--------------------------------------------------------------------------------------

//++ Clear the custom list.
function servercmdClearCustomSounds(%client)
{
	if(!%client.isAdmin && !%client.isSuperAdmin)
	{	
		messageClient(%client,'',"<color:ff0000>You dingus!<color:ffff00> Narry a man, save admin or super admin, hath the divine power to do hither command!");
		return;
	}
	messageClient(%client,'',"\c6You have cleared all the custom foostep SFX for each color of brick.");
  for ( %i = 0; %i < $Pref::Server::PF::customsounds; %i++ )
  {
    $Pref::Server::PF::colorPlaysFX[%i] = "";
  }
  $Pref::Server::PF::customsounds = 0;
}

//++ Clear a single entry on the custom list
function servercmdClearCustomSound(%client)
{
	if(!%client.isAdmin && !%client.isSuperAdmin)
	{	
		messageClient(%client,'',"<color:ff0000>You dingus!<color:ffff00> Narry a man, save admin or super admin, hath the divine power to do hither command!");
		return;
	}
  %color = getColorIDTable(%client.currentColor);
  	%r = getWord(%color, 0) * 255;
		%g = getWord(%color, 1) * 255;
		%b = getWord(%color, 2) * 255;
	%hex = rgbToHex(%color);
  for ( %i = 0; %i < $Pref::Server::PF::customsounds; %i++ )
  {
    %hit = $Pref::Server::PF::colorPlaysFX[%i];
    %hitcolor = getWords(%hit, 0, 3);
      %hr = getWord(%hitcolor, 0) * 255;
      %hg = getWord(%hitcolor, 1) * 255;
      %hb = getWord(%hitcolor, 2) * 255;
    // if there's a match, delete it.
    if ( %hr == %r && %hg == %g && %hb == %b)
    {
      $Pref::Server::PF::colorPlaysFX[%i] = "";
      // if the match isn't the last one in the list, update the list to fill the hole made by removing the hit.
      if ( %i < $Pref::Server::PF::customsounds-1 )
      {
        for ( %j = %i; %j < $Pref::Server::PF::customsounds; %j++ )
        {
          if ( %j < $Pref::Server::PF::customsounds-1 )
          {
            $Pref::Server::PF::colorPlaysFX[%j] = $Pref::Server::PF::colorPlaysFX[%j+1];
            $Pref::Server::PF::colorPlaysFX[%j+1] = "";
          }
          else
          {
            $Pref::Server::PF::colorPlaysFX[%j] = "";
          }
        }
      }
      $Pref::Server::PF::customsounds--;
      messageClient(%client,'',"\c6You have cleared the custom sound for <color:" @ %hex @ ">THIS COLOR\c6.");
      break;
    }          
    else if ( %i == $Pref::Server::PF::customsounds-1 )
    {
      messageClient(%client,'',"\c0Error.\c6 There was no sound effect found for <color:" @ %hex @ ">THIS COLOR\c6.");
    }
  }
}

//++ Get a list of custom sounds.
function servercmdGetCustomSounds(%client)
{
	if(!%client.isAdmin && !%client.isSuperAdmin)
	{	
		messageClient(%client,'',"<color:ff0000>You dingus!<color:ffff00> Narry a man, save admin or super admin, hath the divine power to do hither command!");
		return; 
	}
  messageClient(%client,'',"\c5List of Custom Sounds for Each Color:");
  for ( %i = 0; %i < $Pref::Server::PF::customsounds; %i++ )
  {
    %hit = $Pref::Server::PF::colorPlaysFX[%i];
    %hitcolor = getWords(%hit, 0, 3);
    %hitsound = getWord(%hit, 4);
    %hex =  rgbToHex(%hitcolor); 
    messageClient(%client,'',"<color:" @ %hex @ ">THIS COLOR\c6 is assigned to the sound \c4" @ %hitsound @ "\c6.");	
  }
}

function servercmdGetPeggstepPrefs(%client)
{
	if(!%client.isAdmin && !%client.isSuperAdmin)
	{	
		messageClient(%client,'',"<color:ff0000>You dingus!<color:ffff00> Narry a man, save admin or super admin, hath the divine power to do hither command!");
		return; 
	}
  messageClient(%client,'',"<color:ffffff>Footsteps Enabled: " @ $Pref::Server::PF::footstepsEnabled ? "<color:00ff00>enabled" : "<color:ff0000>disabled");
  messageClient(%client,'',"<color:ffffff>Swimming SoundFX: " @ $Pref::Server::PF::waterSFX ? "<color:00ff00>enabled" : "<color:ff0000>disabled");
  messageClient(%client,'',"<color:ffffff>Special Metal SoundFX" @ $Pref::Server::PF::specialMetalSFX ? "<color:00ff00>enabled" : "<color:ff0000>disabled");
  messageClient(%client,'',"<color:ffffff>Running Minimum Speed: <color:ffff00>" @ $Pref::Server::PF::runningMinSpeed);
}

//++ Set a color to a new footstep.
function servercmdSetColorToSound(%client, %sound)
{	
	if(!%client.isAdmin && !%client.isSuperAdmin)
	{	
    messageClient(%client,'',"<color:ff0000>You dingus!<color:ffff00> Narry a man, save admin or super admin, hath the divine power to do hither command!");
		return; 
  }
	%color = getColorIDTable(%client.currentColor);
	%hex = rgbToHex(%color);
  %i = 0;
	%sounds[%i]   = "water";
	%sounds[%i++] = "stone";
	%sounds[%i++] = "grass";
	%sounds[%i++] = "snow";
	%sounds[%i++] = "wood";
	%sounds[%i++] = "metal";
	%sounds[%i++] = "dirt";
	%sounds[%i++] = "sand";
	%match = false;
	for (%a=0;%a<=%i;%a++)
	{
		if(trim(%sound)$=%sounds[%a])
		{	
			%match = true;
			break;
		}
	}
	if(!%match)
	{
		messageClient(%client,'',"\c6There is no sound with the name \c0" @ trim(%sound) @ "\c6." NL "The sounds you can choose are:");
		for (%a=0;%a<=%i;%a++)
		{
			messageClient(%client,'',"\c5 - " @ %sounds[%a]);
		}
		return;
	}
  %overwrite = false;
	for ( %i = 0; %i < $Pref::Server::PF::customsounds; %i++ )
  {
    %hit = $Pref::Server::PF::colorPlaysFX[%i];
    %hitcolor = getWords(%hit, 0, 3);
    %hr = getWord(%hitcolor, 0);
    %hg = getWord(%hitcolor, 1);
    %hb = getWord(%hitcolor, 2);
    %r = getWord(%color, 0);
    %g = getWord(%color, 1);
    %b = getWord(%color, 2);
    if ( %hr == %r && %hg == %g && %hb == %b)
    {
      %hitsound = getWord(%hit, 4);
      if ( %sound $= %hitsound ) 
      {  
        messageClient(%client,'',"\c6The sound, \c4" @ trim(%sound) @ "\c6, is already playing for <color:" @ %hex @ ">THIS COLOR\c6.");
        return;
      }
      else
      {
        $Pref::Server::PF::colorPlaysFX[%i] = %color SPC %sound;
        %overwrite = true;
        break;
      }
    }
  }
  if ( ! %overwrite )
  {
    $Pref::Server::PF::colorPlaysFX[$Pref::Server::PF::customsounds] = %color SPC %sound;
    $Pref::Server::PF::customsounds++;
  }
	messageClient(%client,'',"\c6The sound, \c4" @ trim(%sound) @ "\c6, now plays for <color:" @ %hex @ ">THIS COLOR\c6.");
}


//--------------------------------------------------------------------------------------
// 		Server Preference Configuration:
//--------------------------------------------------------------------------------------

//++-- If RTB is enabled:

//++ Register preferences:
if(isFile("Add-Ons/SystemR_SoundeturnToBlockland/server.cs"))
{
	if(!$RTB::RTBR_ServerControl_Hook)
	{
		exec("Add-Ons/SystemR_SoundeturnToBlockland/RTBR_ServerControl_Hook.cs");
	}
	RTB_RegisterPref("Enable Footstep SoundFX", "Peggy Footsteps", "$Pref::Server::PF::footstepsEnabled", "bool", "Script_PeggFootsteps", 1, 0, 0);
	RTB_RegisterPref("Enable Special Metal SoundFX", "Peggy Footsteps", "$Pref::Server::PF::specialMetalSFX", "bool", "Script_PeggFootsteps", 1, 0, 0);
	RTB_RegisterPref("Enable Swimming SoundFX", "Peggy Footsteps", "$Pref::Server::PF::waterSFX", "bool", "Script_PeggFootsteps", 1, 0, 0);
	RTB_RegisterPref("Running Threshold", "Peggy Footsteps", "$Pref::Server::PF::runningMinSpeed", "float 0.1 50.0", "Script_PeggFootsteps", 2.8, 0, 0);
}
//++-- If RTB is disabled:
else
{
	if ( $Pref::Server::PF::footstepsEnabled $= "" ) $Pref::Server::PF::footstepsEnabled = 1;
	if ( $Pref::Server::PF::waterSFX $= "" ) $Pref::Server::PF::waterSFX = 1;
	if ( $Pref::Server::PF::runningMinSpeed $= "" ) $Pref::Server::PF::runningMinSpeed = 2.8;
	if ( $Pref::Server::PF::specialMetalSFX $= "" ) $Pref::Server::PF::specialMetalSFX = 1;
}

if ( $Pref::Server::PF::customsounds $= "" )
{
  $Pref::Server::PF::customsounds = 0;
}