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

        public static void Initialize()
        {
            CommandSystem.Register("buffs", AccessLevel.Player, new CommandEventHandler(Buffs_OnCommand));
        }

        private static void Buffs_OnCommand(CommandEventArgs e)
        {
            Mobile mobile = e.Mobile;

            BuffBarGump gump;
            if (!m_GumpInstances.TryGetValue(mobile, out gump))

            {
                // If the BuffBarGump instance doesn't exist for the player, create and store it
                gump = new BuffBarGump(mobile);
                m_GumpInstances[mobile] = gump;
            }

            // Always display the buff information, even if there are no buffs
            mobile.CloseGump(typeof(BuffBarGump));
            mobile.SendGump(gump);

            bool hasFoodBuff = FoodBuff.FoodBuffApplied.ContainsKey(mobile) && FoodBuff.FoodBuffApplied[mobile];
            bool hasChampionBuff = BuffOfTheChampion.BuffOfTheChampionApplied.ContainsKey(mobile) && BuffOfTheChampion.BuffOfTheChampionApplied[mobile];

            if (hasFoodBuff || hasChampionBuff)
            {
                // Display additional information based on active buffs
                if (hasFoodBuff)
                {
                    mobile.SendMessage("You have the Food Buff applied.");
                }

                if (hasChampionBuff)
                {
                    mobile.SendMessage("You have the Buff of the Champion applied.");
                }
            }
            else
            {
                mobile.SendMessage("You don't have any buffs applied.");
            }
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

                UpdateBuffDuration(); // Initial update
                if (!m_GumpInstances.ContainsKey(m_Mobile))
                {
                // Set up the timer to periodically update the buff duration but started only once
                Timer buffTimer = new BuffTimer(this);
                buffTimer.Start();
                }
            }

            private void UpdateBuffDuration()
            {
                int buffCount = 0; // Counter for active buffs
                int totalBuffHeight = 50; // Default height

                bool hasFoodBuff = FoodBuff.FoodBuffApplied.ContainsKey(m_Mobile) && FoodBuff.FoodBuffApplied[m_Mobile];
                bool hasChampionBuff = BuffOfTheChampion.BuffOfTheChampionApplied.ContainsKey(m_Mobile) && BuffOfTheChampion.BuffOfTheChampionApplied[m_Mobile];

                if (hasFoodBuff || hasChampionBuff)
                {
                    if (hasFoodBuff)
                    {
                        buffCount++;
                    }

                    if (hasChampionBuff)
                    {
                        buffCount++;
                    }

                    // Adjust Gump size based on the total height of buffs (increase only if there's more than one buff)
                    if (buffCount > 1)
                    {
                        totalBuffHeight = Math.Max(50, totalBuffHeight - 20 + (20 * buffCount)); // Ensure a minimum height of 50
                    }
                }

                // Add the background after determining the totalBuffHeight
                AddBackground(10, 10, 350, totalBuffHeight, 9270);
                AddLabel(25, 25, 50, "Active Buff(s):");
                if (hasFoodBuff || hasChampionBuff)
                {
                    // Display additional information based on active buffs
                    int yOffset = 25; // Initial Y-coordinate for the labels

                    if (hasFoodBuff)
                    {
                        // Get the remaining duration of the Food Buff
                        TimeSpan remainingTime = FoodBuff.GetRemainingBuffDuration(m_Mobile);
                        AddLabel(120, yOffset, 3, "Food Buff (" + remainingTime.Hours.ToString("D2") + ":" + remainingTime.Minutes.ToString("D2") + ":" + remainingTime.Seconds.ToString("D2") + ")");
                        yOffset += 20; // Increase Y-coordinate for the next label
                    }

                    if (hasChampionBuff)
                    {
                        // Get the remaining duration of the Buff of the Champion
                        TimeSpan remainingTime = BuffOfTheChampion.GetRemainingBuffDuration(m_Mobile);
                        AddLabel(120, yOffset, 3, "Buff of the Champion (" + remainingTime.Hours.ToString("D2") + ":" + remainingTime.Minutes.ToString("D2") + ":" + remainingTime.Seconds.ToString("D2") + ")");
                        yOffset += 20; // Increase Y-coordinate for the next label
                    }
                }
            }

            private class BuffTimer : Timer
            {
                private readonly BuffBarGump m_Gump;

                public BuffTimer(BuffBarGump gump) : base(TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(60))
                {
                    m_Gump = gump;
                    //Priority = TimerPriority.FiveSeconds;
                }

                protected override void OnTick()
                {
                    Console.WriteLine("BuffTimer Tick"); // Add this line for debugging

                    // Close and send the gump for the associated mobile
                    if (m_Gump != null && m_Gump.m_Mobile != null && !m_Gump.m_Mobile.Deleted && m_Gump.m_Mobile.NetState != null)
                    {
                        m_Gump.m_Mobile.CloseGump(typeof(BuffBarGump));
                        m_Gump.m_Mobile.SendGump(new BuffBarGump(m_Gump.m_Mobile));
                    }
                }
            }
        }
    }
}
