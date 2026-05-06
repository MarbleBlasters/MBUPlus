//---------------------------------------------------------------------------
// All HTTPObject functions related to MBU+ Online support, and a few other goodies.
// Written by Connie Sarah - with some code being taken from PlatinumQuest.
//---------------------------------------------------------------------------
//⠀⠀⠀⠀⢀⠠⠤⠀⢀⣿⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
//⠀⠀⠐⠀⠐⠀⠀⢀⣾⣿⡇⠀⠀⠀⠀⠀⢀⣼⡇⠀⠀⠀⠀
//⠀⠀⠀⠀⠀⠀⠀⣸⣿⣿⣿⠀⠀⠀⠀⣴⣿⣿⠇⠀⠀⠀⠀
//⠀⠀⠀⠀⠀⠀⢠⣿⣿⣿⣇⠀⠀⢀⣾⣿⣿⣿⠀⠀⠀⠀⠀
//⠀⠀⠀⠀⠀⣴⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡟⠀⠀⠐⠀⡀
//⠀⠀⠀⠀⢰⡿⠉⠀⡜⣿⣿⣿⡿⠿⢿⣿⣿⡃⠀⠀⠂⠄⠀
//⠀⠀⠒⠒⠸⣿⣄⡘⣃⣿⣿⡟⢰⠃⠀⢹⣿⡇⠀⠀⠀⠀⠀
//⠀⠀⠚⠉⠀⠊⠻⣿⣿⣿⣿⣿⣮⣤⣤⣿⡟⠁⠘⠠⠁⠀⠀
//⠀⠀⠀⠀⠀⠠⠀⠀⠈⠙⠛⠛⠛⠛⠛⠁⠀⠒⠤⠀⠀⠀⠀
//⠨⠠⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠑⠀⠀⠀⠀⠀⠀
//⠁⠃⠉⠀⠀
//---------------------------------------------------------------------------⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀

$Online::PlatformLink = "localhost";
$MaxPostLength = 1024 * 1024 * 7; //7MB

function CreateAccount::onLine(%this, %line)
{
	%this.connected = true;

	if (%line $= "NAMETAKEN")
	{
		XMessagePopupDlg.show(0, "This name is already taken. Try again.", $Text::OK, "Canvas.pushDialog(EnterNameCreateProfileGui);", $Text::Cancel, "");
	}
	else if (getWords(%line, 0, 1) $= "OK account_created")
	{
		$Account::UserToken = getWord(%line, 2);
		$OnlineSession = true;

		savePCUserProfile();
		XMessagePopupDlg.show(0, "Your Unique User Token has been generated and saved in account.cs. It will keep you logged in.", $Text::OK, "", "", "");

		%this.success = true;

		ESRBGui.onSignInComplete();
	}
}

function CreateAccount::onDisconnected(%this)
{
	if (!%this.connected)
	{
		XMessagePopupDlg.show(0, "Connection to MBU+ Online failed. Try again later.", $Text::OK);
	}
	else
	{
		if (!%this.success) {
			XMessagePopupDlg.show(0, "Failed to create account. Using offline profile for now.", $Text::OK);
		}
	}

    %this.delete();
}

function CheckOnlineStatus::onLine(%this, %line)
{
   if (%line $= "ONLINE")
   {
      %this.connection = true;
      OMBUPlusStatus.setText("<just:center>MBU+ Online is <color:32CD32>Available.");
      eval(%this.callback);
   }
}

function CheckOnlineStatus::onDisconnected(%this)
{
   if (!%this.connection)
   {
      if (%this.showpopup)
      {
         XMessagePopupDlg.show(0, "MBU+ Online is not available at the moment.", $Text::OK);
      }
      
      OMBUPlusStatus.setText("<just:center>MBU+ Online is <color:EE4B2B>Offline.");
   }

   %this.delete();
}

function CheckIfMBUOnline(%runifonline, %showpopup)
{
   if (isObject(CheckOnlineStatus))
      CheckOnlineStatus.delete();
   %http = new HTTPObject(CheckOnlineStatus);
   %http.callback = %runifonline;
   %http.showpopup = %showpopup;
   %http.get($Online::PlatformLink, "/api/CheckOnline.php");
}

function FetchScores::onLine(%this, %line)
{
   %this.connected = true;

	%parsed = jsonParse(%line);
   %scores = %parsed.getSize();

   // Insert Scores in the Leaderboards.
   for (%i = 0; %i < %scores; %i++)
   {
      %entry = %parsed.getEntry(%i);
      %name = %entry.username;
      %score = "";
      %rating = "";
      %isSeeded = "";

      if (%entry.best_score $= "")
      {
         if (%entry.scoretype $= "time")
         {
            %score = formatTime(%entry.overall_score);
         }
         else
         {
            %score = %entry.overall_score; 
         }

         %rating = %entry.overall_rating;
      }
      else
      {
         if (%entry.scoretype $= "time")
         {
            %score = formatTime(%entry.best_score);
         }
         else
         {
            %score = %entry.best_score; 
         }

         %rating = %entry.rating;
      }

      if (%entry.seeded $= "1")
      {
         %isSeeded = "common/ui/mission_area.png";
      }

      ScoreboardList.addRow(%i, %i + 1 TAB %name TAB %isSeeded TAB %score TAB %rating);
   }

   levelScoresGui.setLoadingDisplay(false);
}

function FetchScores::onDisconnected(%this)
{
   if (!%this.connected)
   {
      XMessagePopupDlg.show(0, "Connection to MBU+ Online failed.", $Text::OK);
   }
   
   %this.delete();
}

function GetAccDetails::onLine(%this, %line)
{
   %this.connected = true;

   if (%line $= "UNKNOWNMETHOD" || %line $= "INVALID" || %line $= "")
   {
      XMessagePopupDlg.show(0, "Something went wrong. Showing offline profile instead.", $Text::OK);
      MyProfileGui.offlineaccprotocol();
      %this.delete();
   }

	%username = getWord(%line, 0);
   %beg = getWord(%line, 1);
   %interm = getWord(%line, 2);
   %adv = getWord(%line, 3);
   %overall = getWord(%line, 4);
   %wins = getWord(%line, 6);
   %secondplaces = getWord(%line, 7);
   %thirdplaces = getWord(%line, 8);
   %fourthplaces = getWord(%line, 9);
   %firstplayed = getWord(%line, 5);
   %joindate = getWord(%line, 10);
   %status = getWord(%line, 11);

   if (%firstplayed $= "1990-01-01")
   {
      %firstplayed = "Haven\'t joined a lobby yet";
   }

   MyProfileName.setText(%username);

   switch (%status)
   {
      case -1:
      MyProfileStatus.setText("<spush><color:000000>Banned<spop>");
      case 0:
      MyProfileStatus.setText("<spush><color:FFFFFF>Normal User<spop>");
      case 1:
      MyProfileStatus.setText("<spush><color:4A66FF>Moderator<spop>");
   }

   MyProfileJoinDate.setText("<spush><font:Arial Bold:18>Join Date: " @ %joindate @ "<spop>");
   MyProfileStatisticsText.setText("<spush><font:Arial Bold:20>Singleplayer Statistics:" @
                                       "\n<font:Arial Bold:18>Beginner Rating: " @ %beg @
                                       "\nIntermediate Rating: " @ %interm @
                                       "\nAdvanced Rating: " @ %adv @
                                       "\nTotal Official Rating: " @ %overall @
                                       "\n" @
                                       "\n<font:Arial Bold:20>Multiplayer Statistics:" @
                                       "\n<font:Arial Bold:18>First Played: " @ %firstplayed @
                                       "\nWins: " @ %wins @
                                       "\n2nd places: " @ %secondplaces @
                                       "\n3rd places: " @ %thirdplaces @
                                       "\n4th+ places: " @ %fourthplaces);

   getAvatar(%username);
}

function GetAccDetails::onDisconnected(%this)
{
   if (!%this.connected)
   {
      XMessagePopupDlg.show(0, "Connecting to MBU+ Online failed. Online session has stopped.", $Text::OK);
      $OnlineSession = false;
      MyProfileGui.offlineaccprotocol();
   }

   %this.delete();
}

function AvatarGetter::onLine(%this, %line)
{
   if (%line !$= "NOAVATARFOUND")
   {
      %json = jsonParse(%line);
      %base64 = %json.data;
      %username = %json.username;

      %fo = new FileObject();
      %fo.openForWrite("marble/client/ui/menu/avatars/" @ %username @ "AVATAR.png");
		for (%i = 0; %i < %base64.getSize(); %i ++) {
			%fo.writeBase64(%base64.getEntry(%i));
		}
      %fo.close();
      %fo.delete();

      %json.delete();

      MyProfileIcon.setBitmap("marble/client/ui/menu/avatars/" @ %username @ "AVATAR");
   }
}

function AvatarGetter::onDisconnected(%this)
{
   %this.delete();
}

function AvatarGetterDlg::onLine(%this, %line)
{
   if (%line !$= "NOAVATARFOUND")
   {
		%json = jsonParse(%line);
		%base64 = %json.data;
		%username = %json.username;

		%fo = new FileObject();
		%fo.openForWrite("marble/client/ui/menu/avatars/" @ %username @ "AVATAR.png");
			for (%i = 0; %i < %base64.getSize(); %i ++) {
				%fo.writeBase64(%base64.getEntry(%i));
			}
		%fo.close();
		%fo.delete();

		%json.delete();

		ProfileDlgIcon.setBitmap("marble/client/ui/menu/avatars/" @ %username @ "AVATAR");
   }
}

function AvatarGetterDlg::onDisconnected(%this)
{
	%this.delete();
}

function SessionValidityCheck::onLine(%this, %line)
{
   if (!isObject(%this.client))
   {
      %this.delete();
      return;
   }

   if (%line $= "VALID")
   {
      %this.client.sessiontoken = %this.seshtoken;
      commandtoClient(%this.client, 'SessionTokenHeartbeat');
   }
   else if (%line $= "INVALIDEXPIRED" || %line $= "SESHNOTFOUND")
   {
      %this.client.sessiontoken = "";
      commandtoClient(%this.client, 'CreateSeshToken');
   }
}

function SessionValidityCheck::onDisconnect(%this)
{
   %this.delete();
}

function SessionCreation::onLine(%this, %line)
{
   if (getWords(%line, 0, 2) $= "OK session initiated")
   {
      $Account::SessionToken = getWord(%line, 3);
      commandtoServer('CheckSeshValidity', $Account::SessionToken);
   }
   else if (%line $= "SESSIONCREATIONFAILURE" || %line $= "BANNED" || %line $= "INVALIDINPUT")
   {
      error(">>> Session Creation Failure, reason " @ %line);
   }
}

function SessionCreation::onDisconnect(%this)
{
   %this.delete();
}

function SessionTokenHeartbeat::onLine(%this, %line)
{
   %this.connected = true;

   if (%line $= "VALIDITYEXTENDED")
   {
      echo("Session lenghtened successfully");
   }
   else if (%line $= "INVALID")
   {
      echo("Session token is invalid");
   }
   else if (%line $= "SESHNOTFOUND")
   {
      echo("Session token not found");
   }
}

function SessionTokenHeartbeat::onDisconnected(%this)
{
   if (!%this.connected)
   {
      XMessagePopupDlg.show(0, "Could not connect to MBU+ Online. Turning off online session", $Text::OK);
      LocalClientConnection.sessiontoken = "";
      commandtoServer('HandleOpenMBUConnectionFail'); // If you can somehow still get to the server.
      cancel($sessiontokenheartbeat);
   }

   %this.delete();
}

function RecordMatchRanks::onLine(%this, %line)
{
   %this.connected = true;
}

function RecordMatchRanks::onDisconnected(%this)
{
   if (!%this.connected)
   {
      XMessagePopupDlg.show(0, "Could not connect to MBU+ Online. Turning off online session", $Text::OK);
      $OnlineSession = false;
      cancel($sessiontokenheartbeat); // Since this is an online session, a session token might be used. ~ Connie
   }

   %this.delete();
}

function SubmitTime::onLine(%this, %line)
{
   %this.connected = true;

	if (%line $= "USERNOTFOUND")
	{
		XMessagePopupDlg.show(0, "You do not have a token stored.", $Text::OK, "", "", "");
	}
	else if (%line $= "SCORESUBMISIONFAIL")
	{
		XMessagePopupDlg.show(0, "Score submission failed.", $Text::OK, "", "", "");
	}
   else if (%line $= "OK scoresubmited")
   {
      XMessagePopupDlg.show(0, "Score submission succeeded.", $Text::OK, "", "", "");
   }
   else
   {
      XMessagePopupDlg.show(0, "Unknown error.", $Text::OK, "", "", "");
   }

   if (%line $= "BANNED")
   {
      XMessagePopupDlg.show(0, "You have been banned from online play.", $Text::OK, "", "", "");
      $OnlineSession = false;
   }
}

function SubmitTime::onDisconnect(%this)
{
   if (!%this.connected)
   {
      XMessagePopupDlg.show(0, "Could not connect to MBU+ Online. Turning off Online Session.", $Text::OK, "", "", "");
      $OnlineSession = false;
   }

   %this.delete();
}

function UploadAvatar::onLine(%this, %line)
{
   %this.connected = true;

   if (%line $= "AVATARCHANGEDONE")
   {
      XMessagePopupDlg.show(0, "Your avatar has succesfully been changed!", $Text::OK);
   }
}

function UploadAvatar::onDisconnected(%this)
{
   if (!%this.connected)
   {
      XMessagePopupDlg.show(0, "Connection to MBU+ Online failed. Online Session has been stopped.", $Text::OK);
      $OnlineSession = false;
   }

   %this.delete();
}

function GetAccDetailsDLG::onLine(%this, %line)
{
	%this.connected = true;

	%username = getWord(%line, 0);

	getAvatarDlg(%username); // Since we use the username here, just run the avatar finder now.

   %beg = getWord(%line, 1);
   %interm = getWord(%line, 2);
   %adv = getWord(%line, 3);
   %overall = getWord(%line, 4);
  	%wins = getWord(%line, 6);
  	%secondplaces = getWord(%line, 7);
  	%thirdplaces = getWord(%line, 8);
   %fourthplaces = getWord(%line, 9);
   %firstplayed = getWord(%line, 5);
	%joindate = getWord(%line, 10);
	%status = getWord(%line, 11);

   if (%firstplayed $= "1990-01-01")
   {
      %firstplayed = "Haven\'t joined a lobby yet";
   }

   ProfileDlgName.setText(%username);
	ProfileDlgName.username = %username;

	switch (%status)
	{
		case -1:
		ProfileDlgStatus.setText("<spush><color:000000>Banned<spop>");
		case 0:
		ProfileDlgStatus.setText("<spush><color:FFFFFF>Normal User<spop>");
		case 1:
		ProfileDlgStatus.setText("<spush><color:4A66FF>Moderator<spop>");
	}

   	ProfileDlgJoinDate.setText("<spush><font:Arial Bold:18>Join Date: " @ %joindate @ "<spop>");
   	ProfileDlgStatisticsText.setText("<spush><font:Arial Bold:20>Singleplayer Statistics:" @
                                       "\n<font:Arial Bold:18>Beginner Rating: " @ %beg @
                                       "\nIntermediate Rating: " @ %interm @
                                       "\nAdvanced Rating: " @ %adv @
                                       "\nTotal Official Rating: " @ %overall @
                                       "\n" @
                                       "\n<font:Arial Bold:20>Multiplayer Statistics:" @
                                       "\n<font:Arial Bold:18>First Played: " @ %firstplayed @
                                       "\nWins: " @ %wins @
                                       "\n2nd places: " @ %secondplaces @
                                       "\n3rd places: " @ %thirdplaces @
                                       "\n4th+ places: " @ %fourthplaces);

	if (%line $= "BANNED")
	{
		ProfileDlgStatus.setText("<spush><color:000000>Banned<spop>");
	}

   if ($Account::ModeratorStatus)
   {
      ProfileDialogPopupBBtnNew.setVisible(true);

      if (ProfileDlgName.username $= XBLiveGetUserName())
      {
         // Do not ban yourself stupid
         ProfileDialogPopupBBtnNew.setVisible(false);
      }

      // If the account is already banned, have the unban button show.
      if (%status == -1)
      {
         ProfileDialogPopupBBtnNew.setText("Unban");
         ProfileDialogPopupBBtnNew.action = "UNBAN";
      }
      else
      {
         ProfileDialogPopupBBtnNew.setText("Ban");
         ProfileDialogPopupBBtnNew.action = "BAN";
      }
   }
}

function GetAccDetailsDlg::onDisconnected(%this)
{
   	if (!%this.connected)
   	{
      	XMessagePopupDlg.show(0, "Connecting to MBU+ Online has failed.", $Text::OK);
		   $OnlineSession = false;
   	}

   %this.delete();
}

function RecoverAccount::onLine(%this, %line)
{
	%this.connected = true;

	if (%line $= "INVALIDINPUT")
	{
		XMessagePopupDlg.show(0, "Your input is invalid. Try again.", $Text::OK, "Canvas.pushDialog(EnterNameCreateProfileGui);", $Text::Cancel, "");
	}
   else if (%line $= "ERROR404")
   {
      XMessagePopupDlg.show(0, "Something went wrong. Try again.", $Text::OK, "Canvas.pushDialog(EnterNameCreateProfileGui);", $Text::Cancel, "");
   }
   else if (%line $= "WRONGPASSWORD")
   {
      XMessagePopupDlg.show(0, "Wrong password. Try again.", $Text::OK, "Canvas.pushDialog(EnterNameCreateProfileGui);", $Text::Cancel, "");
   }
	else if (getWord(%line, 0) $= "OKtokenrecovered")
	{
		$Account::UserToken = getWord(%line, 1);
		$OnlineSession = true;

		savePCUserProfile();
		XMessagePopupDlg.show(0, "You have been given a new Account Token and have been relogged in.", $Text::OK, "", "", "");

		%this.success = true;

		ESRBGui.onSignInComplete();
	}
}

function RecoverAccount::onDisconnected(%this)
{
	if (!%this.connected)
	{
		XMessagePopupDlg.show(0, "Connection to MBU+ Online failed. Try again later.", $Text::OK);
	}

   %this.delete();
}

function CheckifAccExists::onLine(%this, %line)
{
   %this.connected = true;

   if (%line $= "EXISTS")
   {
      $Account::ModeratorStatus = false;
      savePCUserProfile(); //Save our non-moderator status.
      ESRBGui.finishOnlineSignIn();
   }
   else if (%line $= "EXISTSMODERATOR")
   {
      $Account::ModeratorStatus = true;
      savePCUserProfile(); //Save our moderator status.
      ESRBGui.finishOnlineSignIn();
   }
   else if (%line $= "BANNED")
   {
      XMessagePopupDlg.show(0, "Your account has been banned from online play.", $Text::OK, "", "", "");
   }
   else if (%line $= "NOTFOUND" || %line $= "INVALIDINPUT")
   {
      XMessagePopupDlg.show(0, "Account not found. Do you want to recover your Account ID? The Account ID formerly stored has been cleared out.", "Yes", "ESRBGui.loginAcc(); $Account::UserToken = \"\";", $Text::Cancel, "");
   }
}

function CheckifAccExists::onDisconnected(%this)
{
   if (!%this.connected)
   {
      XMessagePopupDlg.show(0, "Connection to MBU+ Online failed. Try again later.", $Text::OK);
   }

   %this.delete();
}

// --------------------------------------------------------------------
// Taken from PQ
// Weak "encrypts" a string so it can't be seen in clear-text
function garbledeguck(%string) {
	%finish = "";
	for (%i = 0; %i < strlen(%string); %i ++) {
		%char = getSubStr(%string, %i, 1);
		%val = chrValue(%char);
		%val = 128 - %val;
		%hex = dec2hex(%val, 2);
		%finish = %hex @ %finish; //Why not?
	}
	return "gdg" @ %finish;
}

function deGarbledeguck(%string) {
	if (getSubStr(%string, 0, 3) !$= "gdg")
		return %string;
	%finish = "";
	for (%i = 3; %i < strLen(%string); %i += 2) {
		%hex = getSubStr(%string, %i, 2);
		%val = hex2dec(%hex);
		%char = chrForValue(128 - %val);
		%finish = %char @ %finish;
	}
	return %finish;
}

//-----------------------------------------------------------------------------
// http://www.garagegames.com/community/blogs/view/10202
//-----------------------------------------------------------------------------

function dec2hex(%val) {
	// Converts a decimal number into a 2 digit HEX number
	%digits = "0123456789ABCDEF";	//HEX digit table

	// To get the first number we divide by 16 and then round down, using
	// that number as a lookup into our HEX table.
	%firstDigit = getSubStr(%digits,mFloor(%val/16),1);

	// To get the second number we do a MOD 16 and using that number as a
	// lookup into our HEX table.
	%secDigit = getSubStr(%digits,%val % 16,1);

	// return our two digit HEX number
	return %firstDigit @ %secDigit;
}

function hex2dec(%val) {
	// Converts a decimal number into a 2 digit HEX number
	%digits = "0123456789ABCDEF";	//HEX digit table

	// To get the first number we divide by 16 and then round down, using
	// that number as a lookup into our HEX table.
	%firstDigit = strPos(%digits, getSubStr(%val, 0, 1));

	// To get the second number we do a MOD 16 and using that number as a
	// lookup into our HEX table.
	%secondDigit = strPos(%digits, getSubStr(%val, 1, 1));

	// return our two digit HEX number
	return (%firstDigit * 16) + %secondDigit;
}

function chrValue(%chr) {
	// So we don't have to do any C++ changes we approximate the function
	// to return ASCII Values for a character.  This ignores the first 31
	// characters and the last 128.

	// Setup our Character Table.  Starting with ASCII character 32 (SPACE)
	%charTable = " !\"#$%&\'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^\'_abcdefghijklmnopqrstuvwxyz{|}~\t\n\r";

	//Find the position in the string for the Character we are looking for the value of
	%value = strpos(%charTable,%chr);

	// Add 32 to the value to get the true ASCII value
	%value = %value + 32;

	//HACK:  Encode TAB, New Line and Carriage Return

	if (%value >= 127) {
		if (%value == 127)
			%value = 9;
		if (%value == 128)
			%value = 10;
		if (%value == 129)
			%value = 13;
	}

	//return the value of the character
	return %value;
}

function chrForValue(%chr) {
	// So we don't have to do any C++ changes we approximate the function
	// to return ASCII Values for a character.  This ignores the first 31
	// characters and the last 128.

	// Setup our Character Table.  Starting with ASCII character 32 (SPACE)
	%charTable = " !\"#$%&\'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^\'_abcdefghijklmnopqrstuvwxyz{|}~\t\n\r";

	//HACK:  Decode TAB, New Line and Carriage Return

	if (%chr == 9)
		%chr = 127;
	if (%chr == 10)
		%chr = 128;
	if (%chr == 13)
		%chr = 129;

	%chr -= 32;
	%value = getSubStr(%charTable,%chr, 1);

	return %value;
}