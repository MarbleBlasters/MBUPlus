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

$Online::PlatformLink = "https://mbuplus.com/online";
$MaxPostLength = 1024 * 1024 * 7; //7MB


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