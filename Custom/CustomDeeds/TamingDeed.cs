using System;
using Server;
using Server.Items;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Custom
{
    public class TamingDeed : Item
    {
        [Constructable]
        public TamingDeed() : base(0x14F0)
        {
            Weight = 1.0;
            Name = "Taming Deed";
            Hue = 1150;
        }

        public TamingDeed(Serial serial) : base(serial)
        {
        }

                public override void OnSingleClick(Mobile from)
        {
            // This is the on-hover text description
            LabelTo(from, "Use this deed to tame a creature if your Animal Taming skill is 100.0 or higher.");

            base.OnSingleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.Skills.AnimalTaming.Base >= 100.0)
            {
                from.SendLocalizedMessage(1042537); // Select the creature to tame.
                from.Target = new TamingDeedTarget(this);
            }
            else
            {
                from.SendMessage("You need Animal Taming skill of 100.0 or higher to use this deed.");
            }
        }
        private class TamingDeedTarget : Target
        {
            private readonly TamingDeed m_Deal;

            public TamingDeedTarget(TamingDeed deed) : base(10, false, TargetFlags.None)
            {
                m_Deal = deed;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                BaseCreature creature = targeted as BaseCreature;
                if (creature != null)
                {
                    if (creature.Controlled || creature.ControlMaster != null)
                    {
                        from.SendMessage("This creature is already controlled.");
                    }
                    else if (creature.Tamable)
                    {
                        creature.ControlMaster = from;
                        creature.Controlled = true;
                        from.SendMessage("You have tamed the "+ creature.Name +".");
                        m_Deal.Delete(); // Remove the deed after use.
                    }
                    else
                    {
                        from.SendMessage("The "+ creature.Name +" cannot be tamed.");
                    }
                }
                else
                {
                    from.SendMessage("You must target a tamable creature.");
                }
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
