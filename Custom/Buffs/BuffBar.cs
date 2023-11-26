using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Network;
using Server.Commands;
using Server.Targeting;
using Server.Mobiles;
using Server.Engines.XmlSpawnerExtMod;
using Server.Custom;

namespace Server.Custom
{
    public class BuffBar
    {
        private static Dictionary<Mobile, BuffBarGump> m_GumpInstances = new Dictionary<Mobile, BuffBarGump>();
        private static bool buffTimerStarted = false;

        public static void Initialize()
        {
            CommandSystem.Register("buffs", AccessLevel.Player, new CommandEventHandler(Buffs_OnCommand));

            // Start the timer only once
            if (!buffTimerStarted)
            {
                Console.WriteLine("Custom Buff Bar Started"); // Debug statement
                BuffTimer buffTimer = new BuffTimer();
                buffTimer.Start();
                buffTimerStarted = true;
            }
        }

        private static void Buffs_OnCommand(CommandEventArgs e)
        {
            //Console.WriteLine("Buffs_OnCommand called"); // Debug statement
            Mobile mobile = e.Mobile;

            BuffBarGump gump;

            gump = new BuffBarGump(mobile);
            // Always display the buff information, even if there are no buffs
            mobile.CloseGump(typeof(BuffBarGump));
            mobile.SendGump(gump);
        }

        public class BuffBarGump : Gump
        {
            private readonly Mobile m_Mobile;

            public BuffBarGump(Mobile mobile) : base(10, 10)
            {
                m_Mobile = mobile;

                Closable = true;
                Disposable = true;
                Dragable = true;
                Resizable = false;
                AddPage(0);

                //Console.WriteLine("BuffBarGump called"); // Debug statement

                UpdateBuffDuration();
            }

            private void UpdateBuffDuration()
            {
                //Console.WriteLine("UpdateBuffDuration Start"); // Debug statement

                int buffCount = 0; // Counter for active buffs
                int totalBuffHeight = 50; // Default height

                bool hasFoodBuff = FoodBuff.FoodBuffApplied.ContainsKey(m_Mobile) && FoodBuff.FoodBuffApplied[m_Mobile];
                bool hasChampionBuff = BuffOfTheChampion.BuffOfTheChampionApplied.ContainsKey(m_Mobile) && BuffOfTheChampion.BuffOfTheChampionApplied[m_Mobile];

                if (hasFoodBuff)
                {
                    buffCount++;
                }

                if (hasChampionBuff)
                {
                    buffCount++;
                }

                if (buffCount >= 0)
                {
                    // Adjust Gump size based on the total height of buffs (increase only if there's more than one buff)
                    if (buffCount > 1)
                    {
                        totalBuffHeight = Math.Max(50, 30 + 20 * buffCount); // Adjust the height based on the number of buffs
                    }

                    // Add the background after determining the totalBuffHeight
                    AddBackground(10, 10, 350, totalBuffHeight, 9270);
                    AddLabel(25, 25, 50, "Active Buff(s):");

                    // Display additional information based on active buffs
                    int yOffset = 25; // Initial Y-coordinate for the labels

                    if (hasFoodBuff)
                    {
                        // Get the remaining duration of the Food Buff
                        TimeSpan remainingTime = FoodBuff.GetRemainingBuffDuration(m_Mobile);
                        //Console.WriteLine("Food Buff Duration: (" + remainingTime.Hours.ToString("D2") + ":" + remainingTime.Minutes.ToString("D2") + ":" + remainingTime.Seconds.ToString("D2") + ")"); // Debug statement
                        AddLabel(120, yOffset, 3, "Food Buff (" + remainingTime.Hours.ToString("D2") + ":" + remainingTime.Minutes.ToString("D2") + ":" + remainingTime.Seconds.ToString("D2") + ")");
                        yOffset += 20; // Increase Y-coordinate for the next label
                    }

                    if (hasChampionBuff)
                    {
                        // Get the remaining duration of the Buff of the Champion
                        TimeSpan remainingTime = BuffOfTheChampion.GetRemainingBuffDuration(m_Mobile);
                        //Console.WriteLine("Champion Buff Duration: (" + remainingTime.Hours.ToString("D2") + ":" + remainingTime.Minutes.ToString("D2") + ":" + remainingTime.Seconds.ToString("D2") + ")"); // Debug statement
                        AddLabel(120, yOffset, 3, "Buff of the Champion (" + remainingTime.Hours.ToString("D2") + ":" + remainingTime.Minutes.ToString("D2") + ":" + remainingTime.Seconds.ToString("D2") + ")");
                        yOffset += 20; // Increase Y-coordinate for the next label
                    }
                }
            }
        }

        private class BuffTimer : Timer
        {
            public BuffTimer() : base(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10))
            {
            }

            protected override void OnTick()
            {
                //Console.WriteLine("BuffTimer Tick"); // Add this line for debugging

                // Update all gumps for online players
                foreach (Mobile mobile in World.Mobiles.Values)
                {
                    if (mobile is PlayerMobile && ((PlayerMobile)mobile).NetState != null)
                    {
                        PlayerMobile player = (PlayerMobile)mobile; // Cast once for clarity
                        BuffBarGump gump = new BuffBarGump(player);
                        player.CloseGump(typeof(BuffBarGump));
                        player.SendGump(gump);
                    }
                }
            }
        }
    }
}
