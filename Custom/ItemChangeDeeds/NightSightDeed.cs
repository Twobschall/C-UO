
using Server.Targeting;
using System;
using System.Text;

namespace Server.Items
{
    public class NightSightDeedWearable : Item
    {
        public static readonly int NightSight = 1;
        public static readonly int MaxNightSight = 1;
        public override string DefaultName { get { return "NightSight Deed"; } }
        public override double DefaultWeight { get { return 1; } }
        public override bool DisplayLootType { get { return false; } }

        private bool allowWeapon = true;
        private bool allowArmor = true;
        private bool allowShield = true;
        private bool allowJewelry = true;
        private bool allowClothing = true;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool AllowWeapon
        {
            get { return allowWeapon; }
            set { allowWeapon = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool AllowArmor
        {
            get { return allowArmor; }
            set { allowArmor = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool AllowShield
        {
            get { return allowShield; }
            set { allowShield = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool AllowJewelry
        {
            get { return allowJewelry; }
            set { allowJewelry = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool AllowClothing
        {
            get { return allowClothing; }
            set { allowClothing = value; InvalidateProperties(); }
        }

        [Constructable]
        public NightSightDeedWearable
() : base(0x14F0)
        {
            Hue = 1161;
        }

        public NightSightDeedWearable
(Serial serial) : base(serial)
        { }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add("<BASEFONT COLOR=#FFC300>\u0022" + GetAllowedTypeString() + "\u0022<BASEFONT COLOR=#FFC300>");
        }

        private string GetAllowedTypeString()
        {
            StringBuilder temp = new StringBuilder();

            if (AllowArmor)
                temp.Append("Armor");
            if (AllowJewelry)
            {
                if (temp.Length > 0) temp.Append(", ");
                temp.Append("Jewelry");
            }
            if (AllowShield)
            {
                if (temp.Length > 0) temp.Append(", ");
                temp.Append("Shields");
            }
            if (AllowWeapon)
            {
                if (temp.Length > 0) temp.Append(", ");
                temp.Append("Weapons");
            }
            if (AllowClothing)
            {
                if (temp.Length > 0) temp.Append(", ");
                temp.Append("Clothing");
            }

            if (temp.Length > 0)
            {
                int i;
                for (i = temp.Length - 1; i > 0; i--)
                {
                    if (temp[i] == ',')
                        break;
                }
                temp.Replace(",", " and", i, 1);
            }
            else
            {
                temp.Append("No Items allowed! (Disabled)");
            }

            return temp.ToString();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);

            writer.Write(allowArmor);
            writer.Write(allowJewelry);
            writer.Write(allowShield);
            writer.Write(allowWeapon);
            writer.Write(allowClothing);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version >= 1)
            {
                allowArmor = reader.ReadBool();
                allowJewelry = reader.ReadBool();
                allowShield = reader.ReadBool();
                allowWeapon = reader.ReadBool();
                allowClothing = reader.ReadBool();
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
                from.SendLocalizedMessage(1042001);
            else
            {
                from.SendMessage("What item would you like to add NightSight to?");
                from.Target = new NightSightTarget(this);
            }
        }
    }

    public class NightSightTarget : Target
    {
        private readonly NightSightDeedWearable
 m_Deed;

        public NightSightTarget(NightSightDeedWearable
 deed) : base(1, false, TargetFlags.None)
        {
            m_Deed = deed;
        }

        protected override void OnTarget(Mobile from, object target)
        {
            if (!(target is Item))
            {
                from.SendMessage("You can not put NightSight on that!");
                return;
            }
            if (((Item)target).RootParent != from)
            {
                from.SendMessage("You can not put NightSight on that there!");
                return;
            }

            AosAttributes attr = null;

            if (m_Deed.AllowWeapon && target is BaseWeapon)
            {
                attr = ((BaseWeapon)target).Attributes;
            }
            else if (target is BaseShield)
            {
                if (m_Deed.AllowShield)
                    attr = ((BaseShield)target).Attributes;
            }
            else if (m_Deed.AllowArmor && target is BaseArmor)
            {
                attr = ((BaseArmor)target).Attributes;
            }
            else if (m_Deed.AllowJewelry && target is BaseJewel)
            {
                attr = ((BaseJewel)target).Attributes;
            }
            else if (m_Deed.AllowClothing && target is BaseClothing)
            {
                attr = ((BaseClothing)target).Attributes;
            }

            if (attr == null)
            {
                from.SendMessage("You can not put NightSight on that item!");
                return;
            }

            if (attr.NightSight >= NightSightDeedWearable
    .MaxNightSight)
            {
                from.SendMessage("That already has NightSight!");
                return;
            }

            attr.NightSight = Math.Min(NightSightDeedWearable
    .MaxNightSight, attr.NightSight + NightSightDeedWearable
    .NightSight);
            from.SendMessage("You magically add NightSight to your item....");
            m_Deed.Delete();
        }
    }
}