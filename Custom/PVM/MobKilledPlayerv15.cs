using System;
using System.Collections;
using Server.Network;

namespace Server
{
    public class AnnounceDeath
    {
        public static void Initialize()
        {
            EventSink.PlayerDeath += new PlayerDeathEventHandler(OnDeath);
        }
        public static void OnDeath(PlayerDeathEventArgs args)
        {
            Mobile m = args.Mobile;
            Mobile from = args.Mobile;
            Mobile c = from.LastKiller;

            if (args.Mobile.AccessLevel < AccessLevel.GameMaster)
            {
                if (c == null)
                {
                    switch (Utility.Random(4))
                    {
                        case 0:
                            args.Mobile.PlaySound(256);
                            World.Broadcast(0x4B9, true, "Death Has Taken {0} May God Have Mercy On Thier Soul.", args.Mobile.Name);
                            break;

                        case 1:
                            args.Mobile.PlaySound(256);
                            World.Broadcast(0x4B9, true, "{0} Has Lost The Battle Yet Again.", args.Mobile.Name);
                            break;

                        case 2:
                            args.Mobile.PlaySound(256);
                            World.Broadcast(0x31, true, "Death Comes For Us All, But On This Day, For {0}", args.Mobile.Name);
                            break;

                        case 3:
                            args.Mobile.PlaySound(256);
                            World.Broadcast(0x4B9, true, "{0} Has Succumbed To Their Wounds And Has Perished!", args.Mobile.Name);
                            break;

                    }

                }

                else
                {

                    switch (Utility.Random(4))
                    {
                        case 0:
                            args.Mobile.PlaySound(256);
                            World.Broadcast(0x4B9, true, "Death Has Come For {0} May May The Seek Vengeance Against {1}", args.Mobile.Name, c.Name);
                            break;

                        case 1:
                            args.Mobile.PlaySound(256);
                            World.Broadcast(0x4B9, true, "{0} Has Lost their Life In A Battle With {1}", args.Mobile.Name, c.Name);
                            break;

                        case 2:
                            args.Mobile.PlaySound(256);
                            World.Broadcast(0x31, true, "Death Comes for Us all, But on this Day, For {0}.Thier Killer Is {1}", args.Mobile.Name, c.Name);
                            break;

                        case 3:
                            args.Mobile.PlaySound(256);
                            World.Broadcast(0x4B9, true, "{0} Has Succumbed To Their Wounds From Thier Epic Battle With {1}", args.Mobile.Name, c.Name);
                            break;

                    }
                }

            }
        }
    }
}
