//	server.cs
//-----------------------------------------------------------------------------------------------------------
// Title: PeggyFootsteps
// Author: Peggworth the Pirate
// Description:  Footsteps that change noise based on what you touch below you.
//	
//				A set of footsteps ripped from an old diablo-esque game, and from 'FreeSound.org'.
//				We shall call them peggsteps and they will be the best.
//-----------------------------------------------------------------------------------------------------------
//	Version:  2.3.4: Fixed error where certain commands would tell you to use '/help' not '/pegghelp' [2/20/2016] 
//        2.3.3: Replaced '/help' with '/pegghelp' [12/29/2015]         
//        2.3.2: Fixed bug with '/clearcustomsound' where when a single sound was deleted, the entire list was no longer played. [12/27/2015] 
//        2.3.1: Fixed 'sand' sound FX (these weren't playing when you added custom sounds). Added '/clearcustomsound' to remove a single SFX to the color you have selected. Edited command system again ('/get' is no longer a parametered command) [12/23/2015]
//        2.3.0: Revamped custom sound functions. Instead of being stored in one large string, custom souds are in an array now. Edited 'getHex' to 'rgbToHex' to fix the occasional hex error. 
//        2.2.1 : Fixed Small Bugs	[12/12/2015]
//				2.2.0 :	Edited command system, Sand footstep soundFX, Removed use of 'eval', New Pref, New Command	[12/6/2015]
//				2.1.0 :	Metal and Snow footstep soundFX, Custom soundFX, Bug fix	[10/03/2015]
//				2.0.0 :	Swimming, RTB Prefs, Released to public		[09/01/2015]
//				1.1.0 :	All sounds replaced with new ones, no new functionality	
//				1.0.0 :	Footsteps adapt to color of ground stepped on
//-----------------------------------------------------------------------------------------------------------
//  Thanks to Greek2Me for the 'rgbToHex' function, and to Hata for the framework to playing footsteps
//-----------------------------------------------------------------------------------------------------------

exec("./Sounds/sounds.cs");
exec("./PeggyFootsteps.cs");