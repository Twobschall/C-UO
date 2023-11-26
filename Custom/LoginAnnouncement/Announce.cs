//Script by Arturus
using System;
using Server.Network;
using Server.Commands;
 
namespace Server
{
       public class Announce
    {
        private readonly static string m_LoginMessage = "{0} has logged in.";//Login Message
        private readonly static int m_LoginHue = 0x26;//Login Message Hue
        private readonly static string m_LogoutMessage = "{0} has logged out.";//Logout Message
        private readonly static int m_LogoutHue = 0x26;//Logout Message Hue		

        public static void Initialize()
        {
                   EventSink.Login += new LoginEventHandler( World_Login );
                   EventSink.Disconnected += new DisconnectedEventHandler( World_Disconnect );
        }
        private static void World_Login( LoginEventArgs args )
        {
               Mobile m = args.Mobile; 
 
        if (args.Mobile.AccessLevel < AccessLevel.Administrator)
        { 
              CommandHandlers.BroadcastMessage(AccessLevel.Counselor, m_LoginHue, String.Format(m_LoginMessage, args.Mobile.Name)); 
        }
               if (args.Mobile.AccessLevel >= AccessLevel.Administrator) 
        { 
              CommandHandlers.BroadcastMessage(AccessLevel.Administrator, m_LoginHue, String.Format(m_LoginMessage, args.Mobile.Name));
       }
       }
        private static void World_Disconnect( DisconnectedEventArgs args )
        {
               Mobile m = args.Mobile; 
 
        if (args.Mobile.AccessLevel < AccessLevel.Administrator) 
        { 
              CommandHandlers.BroadcastMessage(AccessLevel.Counselor, m_LogoutHue, String.Format(m_LogoutMessage, args.Mobile.Name)); 
        }
        if (args.Mobile.AccessLevel >= AccessLevel.Administrator) 
        { 
              CommandHandlers.BroadcastMessage(AccessLevel.Administrator, m_LogoutHue, String.Format(m_LogoutMessage, args.Mobile.Name));
        }
        }
    }
}
