using System;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Custom
{
    public class EtherealMountDeed : Item
    {
        private readonly Type[] m_MountTypes = {
            typeof(EtherealHorse),
            typeof(EtherealLlama),
            typeof(EtherealOstard),
            typeof(EtherealRidgeback),
            typeof(EtherealBeetle),
            typeof(EtherealUnicorn),
            typeof(EtherealKirin),
            typeof(EtherealSwampDragon),
            typeof(EtherealCuSidhe),
            typeof(EtherealHiryu)
        };

        [Constructable]
        public EtherealMountDeed() : base(0x14F0) // You can change the graphic ID as needed
        {
            Weight = 1.0;
            LootType = LootType.Blessed;
            Name = "Ethereal Mount Deed";
        }

        public EtherealMountDeed(Serial serial) : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
            else
            {
                from.SendGump(new EtherealMountDeedGump(from, this, m_MountTypes));
            }
        }

        private class EtherealMountDeedGump : Gump
        {
            private readonly Mobile m_Player;
            private readonly EtherealMountDeed m_EtherealMountDeed;
            private readonly Type[] m_MountTypes;

            public EtherealMountDeedGump(Mobile player, EtherealMountDeed etherealMountDeed, Type[] mountTypes) : base(50, 50)
            {
                m_Player = player;
                m_EtherealMountDeed = etherealMountDeed;
                m_MountTypes = mountTypes;

                AddPage(0);
                AddBackground(0, 0, 300, 480, 0x13BE);

                AddLabel(20, 20, 0x34, "Choose a Mount:");

                for (int i = 0; i < m_MountTypes.Length; i++)
                {
                    Type mountType = m_MountTypes[i];
                    Item mount = Activator.CreateInstance(mountType) as Item;

                    if (mount != null)
                    {
                        AddItem(10, 50 + i * 40, mount.ItemID);
                        
                        // Fix: Display the actual mount name next to the button
                        AddLabel(100, 50 + i * 40, 0x64, mount.Name ?? mount.GetType().Name); // Use the type name if the name is null
                        // Add a button next to each item
                        AddButton(260, 50 + i * 40, 0xFAB, 0xFAD, i + 1, GumpButtonType.Reply, 0);
                    }
                }
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (info.ButtonID > 0 && info.ButtonID <= m_MountTypes.Length)
                {
                    int selectedIndex = info.ButtonID - 1;

                    if (m_Player.Backpack != null)
                    {
                        Type mountType = m_MountTypes[selectedIndex];
                        Item mount = Activator.CreateInstance(mountType) as Item;

                        if (mount != null)
                        {
                            // Delete the deed
                            m_EtherealMountDeed.Delete();

                            // Give the item to the player
                            m_Player.Backpack.DropItem(mount);
                            m_Player.SendMessage("You have received " + mount.Name + ".");
                        }
                    }
                }
            }
        }
    }
}
