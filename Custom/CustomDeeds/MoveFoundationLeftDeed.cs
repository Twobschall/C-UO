using System;
using Server;
using Server.Targeting;
using Server.Multis;

namespace Server.Custom
{
    public class MoveFoundationLeftDeed : Item
    {
        [Constructable]
        public MoveFoundationLeftDeed() : base(0x14F0)
        {
            Weight = 1.0;
            LootType = LootType.Blessed;
            Name = "Move Foundation Left 2 Spaces Deed";
        }

        public MoveFoundationLeftDeed(Serial serial) : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001);
            }
            else
            {
                from.SendMessage("Target the house that you want to move left 2 spaces.");
                from.Target = new MoveFoundationLeftTarget(this, from);
            }
        }

        private class MoveFoundationLeftTarget : Target
        {
            private readonly MoveFoundationLeftDeed m_Script;
            private readonly Mobile m_Player;

            public MoveFoundationLeftTarget(MoveFoundationLeftDeed script, Mobile player) : base(10, false, TargetFlags.None)
            {
                m_Script = script;
                m_Player = player;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                IPoint3D point = targeted as IPoint3D;

                if (point != null)
                {
                    BaseHouse house = FindHouseAt(point);

                    if (house != null)
                    {
                        //Console.WriteLine("DEBUG: House found. Owner: " + (house.Owner != null ? house.Owner.Name : "Unknown"));

                        // Check if the player is the owner of the house
                        if (house.Owner == from)
                        {
                            // Lower the house by 6 units
                            house.Location = new Point3D(house.Location.X - 2, house.Location.Y, house.Location.Z);

                            from.SendMessage("The house has been moved left 2 blocks.");
                            // Delete the deed
                            m_Script.Delete();
                        }
                        else
                        {
                            from.SendMessage("You can only move houses that you own.");
                        }
                    }
                    else
                    {
                        from.SendMessage("No house found at the targeted location.");
                    }
                }
                else
                {
                    from.SendMessage("Invalid target or the target is not a location.");
                }
            }

            private BaseHouse FindHouseAt(IPoint3D point)
            {
                foreach (Item item in World.Items.Values)
                {
                    if (item is BaseHouse && (item as BaseHouse).Contains(point))
                    {
                        return (BaseHouse)item;
                    }
                }

                return null;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
