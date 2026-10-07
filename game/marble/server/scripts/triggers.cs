//-----------------------------------------------------------------------------
// Torque Game Engine
// 
// Copyright (c) 2001 GarageGames.Com
// Portions Copyright (c) 2001 by Sierra Online, Inc.
//-----------------------------------------------------------------------------

// Normally the trigger class would be sub-classed to support different
// functionality, but we're going to use the name of the trigger instead
// as an experiment.

//-----------------------------------------------------------------------------

datablock TriggerData(InBoundsTrigger)
{
   tickPeriodMS = 100;
   marbleGravityCheck = true;
};

function InBoundsTrigger::onLeaveTrigger(%this,%trigger,%obj)
{
   // Leaving an in bounds area.
   %obj.getDatablock().onOutOfBounds(%obj);
}


//-----------------------------------------------------------------------------

datablock TriggerData(OutOfBoundsTrigger)
{
   tickPeriodMS = 100;
};

function OutOfBoundsTrigger::onEnterTrigger(%this,%trigger,%obj)
{
   // Entering an out of bounds area
   %obj.getDatablock().onOutOfBounds(%obj);
}

//-----------------------------------------------------------------------------
datablock TriggerData(HelpTrigger)
{
   tickPeriodMS = 100;
};

function HelpTrigger::onEnterTrigger(%this,%trigger,%obj)
{
   // Leaving an in bounds area.
   commandToClient(%obj.client,'setHelpLine',%trigger.text,1);
}

datablock TriggerData(CheckPointTrigger)
{
   tickPeriodMS = 100;
};

function CheckPointTrigger::onEnterTrigger(%this, %trigger, %obj)
{
   // Entering a check point
   %obj.getDatablock().setCheckPoint(%obj, %trigger);
}

datablock TriggerData(FinishTrigger)
{
   tickPeriodMS = 100;
};

function FinishTrigger::onEnterTrigger(%this, %trigger, %obj)
{
   // Entering a finish point
   %obj.getDatablock().onFinishPoint(%obj);
}

//----------------- Sky Change Trigger
datablock TriggerData(SkyChangeTrigger)
{
   tickPeriodMS = 100;
};

function SkyChangeTrigger::onEnterTrigger(%this,%trigger,%obj)
{
   // Change Sky Material To Trigger Specified One
   echo("Changing Sky To: " @ %trigger.material);
   Sky.setSkyMaterial("marble/data/skies/" @ %trigger.material @ ".dml");

}

//----------------- Physics Mod Trigger
//----------------Use In SinglePlayer Only!
datablock TriggerData(PhysModTrigger)
{
   tickPeriodMS = 100;
};

$MarbleOriginalGravity = 20;
$MarbleOriginalJump = 7.5;
$MarbleOriginalBounce = 0.5;
$MarbleOriginalStaticFriction = 1.1;
$MarbleOriginalMaxRoll = 15;
$MarbleOriginalAngular = 75;
$MarbleOriginalBraking = 30;
$MarbleOriginalKineticFriction = 0.7;
$MarbleOriginalDotSlide = 0.5;
$MarbleOriginalBounceKineticFriction = 0.2; 
$MarbleOriginalMaxRadius = 50;

function PhysModTrigger::onEnterTrigger(%this, %trigger, %obj)
{
   if (%obj.getClassName() $= "Marble")
   {

      // --- SINGLE PLAYER ONLY CHECK ---
      // Check if this specific marble instance is currently controlled by the local connection
      // %client = %obj.getControllingClient();
      
      // // If there is no controlling client, or if it doesn't match the local client host connection, ignore it
      // if (!isObject(%client) || %client != LocalClientConnection)
      // {
      //    echo("[Physmod Enter] Physmod Only Works Offline!");
      //    return;
      // }

      // 1. Get the ID of the data template the marble is using
      %dataBlock = %obj.getDataBlock();
      
      // 2. Backup the default value before overwriting it
      $MarbleOriginalGravity = %dataBlock.gravity;
      $MarbleOriginalJump = %dataBlock.jumpImpulse;
      $MarbleOriginalBounce = %dataBlock.bounceRestitution;
      $MarbleOriginalStaticFriction = %dataBlock.staticFriction;
      $MarbleOriginalMaxRoll = %dataBlock.maxRollVelocity;
      $MarbleOriginalAngular = %dataBlock.angularAcceleration;
      $MarbleOriginalBraking = %dataBlock.brakingAcceleration;
      $MarbleOriginalKineticFriction = %dataBlock.kineticFriction;
      $MarbleOriginalDotSlide = %dataBlock.maxDotSlide;
      $MarbleOriginalBounceKineticFriction = %dataBlock.bounceKineticFriction;
      $MarbleOriginalMaxRadius = %dataBlock.maxForceRadius;
      
      // 3. Shove your custom trigger field directly into the template!
      %dataBlock.gravity = %trigger.modGravity;
      %dataBlock.jumpImpulse = %trigger.modJumpImpulse;
      %dataBlock.bounceRestitution = %trigger.modBounce;
      %dataBlock.staticFriction = %trigger.modStaticFriction;
      %dataBlock.maxRollVelocity = %trigger.modRollVelocity;
      %dataBlock.angularAcceleration = %trigger.modAngularAcceleration;
      %dataBlock.brakingAcceleration = %trigger.modBrakingAcceleration;
      %dataBlock.kineticFriction = %trigger.modKineticFriction;
      %dataBlock.maxDotSlide = %trigger.modDotSlide;
      %dataBlock.bounceKineticFriction = %trigger.modbounceKineticFriction;
      %dataBlock.maxForceRadius = %trigger.modForceRadius;
      
      echo("Altered Marble Data to: " @ %dataBlock.gravity);
   }
}

function PhysModTrigger::onLeaveTrigger(%this, %trigger, %obj)
{
   if (%obj.getClassName() $= "Marble")
   {

      // // --- SINGLE PLAYER ONLY CHECK ---
      // %client = %obj.getControllingClient();
      // if (!isObject(%client) || %client != LocalClientConnection)
      // {
      //    echo("[Physmod Exit] Physmod Only Works Offline!");
      //    return;
      // }

      // Restore the template's baseline gravity
      %obj.getDataBlock().gravity = $MarbleOriginalGravity;
      %obj.getDataBlock().jumpImpulse = $MarbleOriginalJump;
      %obj.getDataBlock().bounceRestitution = $MarbleOriginalBounce;
      %obj.getDataBlock().staticFriction = $MarbleOriginalStaticFriction;
      %obj.getDataBlock().maxRollVelocity = $MarbleOriginalMaxRoll;
      %obj.getDataBlock().angularAcceleration = $MarbleOriginalAngular;
      %obj.getDataBlock().brakingAcceleration = $MarbleOriginalBraking;
      %obj.getDataBlock().kineticFriction = $MarbleOriginalKineticFriction;
      %obj.getDataBlock().maxDotSlide = $MarbleOriginalDotSlide;
      %obj.getDataBlock().bounceKineticFriction = $MarbleOriginalBounceKineticFriction;
      %obj.getDataBlock().maxForceRadius = $MarbleOriginalMaxRadius;
      
      echo("Restored Marble Data to default.");
   }
}

