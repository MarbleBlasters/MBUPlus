//-----------------------------------------------------------------------------
// Torque Game Engine
// 
// Copyright (c) 2001 GarageGames.Com
// Portions Copyright (c) 2001 by Sierra Online, Inc.
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Misc. server commands avialable to clients
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------

function serverCmdToggleCamera(%client)
{
   %control = %client.getControlObject();
   if (%control == %client.player)
   {
      %control = %client.camera;
      %control.mode = toggleCameraFly;
   }
   else
   {
      %control = %client.player;
      %control.mode = observerFly;
   }
   %client.setControlObject(%control);
}

function serverCmdDropPlayerAtCamera(%client)
{
   if ($Server::TestCheats)
   {
      %client.player.setTransform(%client.camera.getTransform());
      %client.player.setVelocity("0 0 0");
      %client.setControlObject(%client.player);
   }
}

function serverCmdDropCameraAtPlayer(%client)
{
   if ($Server::TestCheats)
   {
      %client.camera.setTransform(%client.player.getEyeTransform());
      %client.camera.setVelocity("0 0 0");
      %client.setControlObject(%client.camera);
   }
}

function serverCmdSetMarble( %client, %marbleIdx )
{
   %client.marbleIndex = %marbleIdx;
}


//-----------------------------------------------------------------------------

function serverCmdSuicide(%client)
{
   %client.player.kill("Suicide");
}   

function serverCmdPlayCel(%client,%anim)
{
   if (isObject(%client.player))
      %client.player.playCelAnimation(%anim);
}

function serverCmdPlayDeath(%client)
{
   if (isObject(%client.player))
      %client.player.playDeathAnimation();
}

function serverCmdSelectObject(%client, %mouseVec, %cameraPoint)
{
   //Determine how far should the picking ray extend into the world?
   %selectRange = 200;
   // scale mouseVec to the range the player is able to select with mouse
   %mouseScaled = VectorScale(%mouseVec, %selectRange);
   // cameraPoint = the world position of the camera
   // rangeEnd = camera point + length of selectable range
   %rangeEnd = VectorAdd(%cameraPoint, %mouseScaled);

   // Search for anything that is selectable � below are some examples
   %searchMasks = $TypeMasks::PlayerObjectType | $TypeMasks::CorpseObjectType |
      				$TypeMasks::ItemObjectType | $TypeMasks::TriggerObjectType;

   // Search for objects within the range that fit the masks above
   // If we are in first person mode, we make sure player is not selectable by setting fourth parameter (exempt
   // from collisions) when calling ContainerRayCast
   %player = %client.player;
   if ($firstPerson)
   {
	  %scanTarg = ContainerRayCast (%cameraPoint, %rangeEnd, %searchMasks, %player);
   }
   else //3rd person - player is selectable in this case
   {
	  %scanTarg = ContainerRayCast (%cameraPoint, %rangeEnd, %searchMasks);
   }

   // a target in range was found so select it
   if (%scanTarg)
   {
      %targetObject = firstWord(%scanTarg);

      %client.setSelectedObj(%targetObject);
   }
}

function serverCmdStartTalking(%client)
{
   
}

function serverCmdStopTalking(%client)
{
   
}

//----------------------------------------------------------------------------------
// Spectator Mode - Work in Progress
// Worked on by Connie, Yoshicraft224, and A-Game
//----------------------------------------------------------------------------------
// TODO list:
// Figure out how to make the timer not stop when you spectate (only visual bug)
// General code optimizations where possible & testing for bugs
//----------------------------------------------------------------------------------
// Code related to Spectator Mode is also present in:
// default.bind.cs
// game.cs (client/scripts)
// lobbyGui.gui
// lobbyPopupDlg.gui
// GamePauseGui.gui
// playGui.cs
//----------------------------------------------------------------------------------
// $Client::isspectating is the value stored locally, while %client.isspectating is the 
// value stored by the Server. Use %client.isspectating for server commands/server sided
// stuff, and $Client::isspectating for local commands.
//----------------------------------------------------------------------------------
// To check if a player is orbiting, use $Client::isorbiting locally, 
// and %client.isorbiting on the server.
//----------------------------------------------------------------------------------
// Keybinds:
// alt+c - toggle spectator mode on and off
// c - toggle between free fly and player orbiting modes
// a - toggle previous player in orbiting
// d - toggle next player in orbiting
//----------------------------------------------------------------------------------

// Set the value of isspectating locally - when in need to do stuff locally only
function clientCmdSpectateStatus(%state, %orbit)
{
   $Client::isspectating = %state;
   $Client::isorbiting = %orbit;
}

// This handles the way the HUD is modified when you spectate or when you stop spectating
// There's probably a better way of handling this, but this gets the joj done ~Connie
function SpectatorHudTog(%state)
{
   HUD_ShowPowerUp.setVisible(!%state);
   BoostBar.setVisible(!%state);   
}

// Called when the Game State goes to "Start". Will check for the client's "isspectating" value, and then act accordingly.
function StartStateSetSpectate()
{
   if ($Client::isspectating)
   {
      commandtoServer('ToggleSpecMode');
   }
}

// // Used by the Popup when you ready up for setting the isspectating value.
// function serverCmdSetSpectateStatus(%client, %state)
// {
//    %client.isspectating = %state;
//    commandtoClient(%client, 'SpectateStatus', %client.isspectating, %client.isorbiting);
// }

//Spectating Logic is here. ~Connie
function serverCmdToggleSpecMode(%client)
{
   if (!isObject(%client.speccamera))
   {
      CreateSpecCam(%client);
   }

   if (%client.getControlObject() == %client.player)
   {
      //Delete the Player Marble if they start spectating
      %client.isspectating = true;
      %client.player.delete();
      %client.player = "";

      //And use the camera instead
      %client.setControlObject(%client.speccamera);
   }
   else
   {
      //Respawn the Player Marble if the player decides to stop spectating
      %client.isspectating = false;
      %client.isorbiting = false;
      %client.spawnPlayer();
   }

   commandtoClient(%client, 'SpectateStatus', %client.isspectating, %client.isorbiting);
}

function serverCmdToggleOrbitMode(%client)
{
   if (%client.playerspectating $= "")
   {
      serverCmdNextSpecPlayer(%client);
   }
   else
   {
      serverCmdStopSpecPlayer(%client);
   }
}

//Choose previous person to orbit
function serverCmdPrevSpecPlayer(%client)
{
   if (!%client.isspectating)
      return;

   %client.playerspectating = "";
   %wrap = false;
   %top = ClientGroup.getCount() - 1;

   while (!isObject(%client.playerspectating.player))
   {
       %client.orbitspecindex --;

      //Wrap around if index is too low
      if (%client.orbitspecindex < 0)
      {
         %client.orbitspecindex = %top;
         if (%wrap)
         {
            //Didn't find one
            break;
         }
         %wrap = true;
      }

      %client.playerspectating = ClientGroup.getObject(%client.orbitspecindex);
   }
   if (%client.orbitspecindex !$= "" && %client.playerspectating.player !$= "") 
   {
      %client.speccamera.setOrbitMode(%client.playerspectating.player, %client.playerspectating.player.getTransform(), 0.5, 4.5, 4.5);
      %client.isorbiting = true;
      commandtoClient(%client, 'SpectateStatus', %client.isspectating, %client.isorbiting);
   } 
   else 
      serverCmdStopSpecPlayer(%client);
}

//Choose next person to orbit
function serverCmdNextSpecPlayer(%client)
{
   if (!%client.isspectating)
      return;

   %client.playerspectating = "";
   %wrap = false;

   while (!isObject(%client.playerspectating.player))
   {
       %client.orbitspecindex ++;

      //Wrap around if index is too low
      if (%client.orbitspecindex > ClientGroup.getCount())
      {
         %client.orbitspecindex = 0;
         if (%wrap)
         {
            //Didn't find one
            break;
         }
         %wrap = true;
      }

      %client.playerspectating = ClientGroup.getObject(%client.orbitspecindex);
   }
   if (%client.orbitspecindex !$= "" && %client.playerspectating.player !$= "") 
   {
      %client.speccamera.setOrbitMode(%client.playerspectating.player, %client.playerspectating.player.getTransform(), 0.5, 4.5, 4.5);
      %client.isorbiting = true;
      commandtoClient(%client, 'SpectateStatus', %client.isspectating, %client.isorbiting);
   }
   else 
      serverCmdStopSpecPlayer(%client);
}

//The function to get out of orbiting a player when spectating.
function serverCmdStopSpecPlayer(%client)
{
   if (%client.isspectating)
   {
      //Reset values so when we start spectating other players in orbit mode again, we can rerun everything.
      %client.playerspectating = "";
      %client.orbitspecindex = "";
      %client.isorbiting = false;

      //Set 'em free.
      %client.speccamera.setFlyMode();
   }
}

//This will be the camera where we get to spectate someone
function CreateSpecCam(%client)
{
   %client.speccamera = new Camera() {
      dataBlock = Observer;
   };
}