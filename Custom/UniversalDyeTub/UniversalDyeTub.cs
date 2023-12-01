using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Gumps;
using Server.Multis;
using Server.Targeting;
using Server.Items;

namespace Server.Custom
{
    public interface IDyable
    {
        bool Dye(Mobile from, UniversalDyeTub sender);
    }

    public class UniversalDyeTub : Item, ISecurable
    {
        private bool m_Redyable;
        private int m_DyedHue;
        private SecureLevel m_SecureLevel;

        [Constructable]
        public UniversalDyeTub()
            : base(0xFAB)
        {
            Weight = 10.0;
            m_Redyable = true;
			LootType = LootType.Blessed;
			Name = "Universal Dye Tub";
        }

        public UniversalDyeTub(Serial serial)
            : base(serial)
        {
        }

        public virtual CustomHuePicker CustomHuePicker { get { return null; } }
        public virtual bool AllowRunebooks { get { return false; } }
        public virtual bool AllowFurniture { get { return false; } }
        public virtual bool AllowStatuettes { get { return false; } }
        public virtual bool AllowLeather { get { return false; } }
        public virtual bool AllowDyables { get { return true; } }
        public virtual bool AllowMetal { get { return false; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Redyable
        {
            get { return m_Redyable; }
            set { m_Redyable = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int DyedHue
        {
            get { return m_DyedHue; }
            set
            {
                if (m_Redyable)
                {
                    m_DyedHue = value;
                    Hue = value;
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level
        {
            get { return m_SecureLevel; }
            set { m_SecureLevel = value; }
        }

        public virtual int TargetMessage { get { return 500859; } } // Select the clothing to dye.        
        public virtual int FailMessage { get { return 1042083; } } // You can not dye that.

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version

            writer.Write((int)m_SecureLevel);
            writer.Write((bool)m_Redyable);
            writer.Write((int)m_DyedHue);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        m_SecureLevel = (SecureLevel)reader.ReadInt();
                        goto case 0;
                    }
                case 0:
                    {
                        m_Redyable = reader.ReadBool();
                        m_DyedHue = reader.ReadInt();

                        break;
                    }
            }
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);
            SetSecureLevelEntry.AddTo(from, this, list);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(GetWorldLocation(), 1))
            {
                from.SendLocalizedMessage(TargetMessage);
                from.Target = new InternalTarget(this);
            }
            else
            {
                from.SendLocalizedMessage(500446); // That is too far away.
            }
        }

        private class InternalTarget : Target
        {
            private readonly UniversalDyeTub m_Tub;

            public InternalTarget(UniversalDyeTub tub)
                : base(1, false, TargetFlags.None)
            {
                m_Tub = tub;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is Item)
                {
                    Item item = (Item)targeted;

                    if (item is IDyable && m_Tub.AllowDyables)
                    {
                        if (!from.InRange(m_Tub.GetWorldLocation(), 1) || !from.InRange(item.GetWorldLocation(), 1))
                            from.SendLocalizedMessage(500446); // That is too far away.
                        else if (item.Parent is Mobile)
                            from.SendLocalizedMessage(500861); // Can't Dye clothing that is being worn.
                        else if (((IDyable)item).Dye(from, m_Tub))
                            from.PlaySound(0x23E);
                    }
                    // Add the following block of code to dye items in the player's backpack
                    else if (item.IsChildOf(from.Backpack) && m_Tub.AllowDyables)
                    {
                        if (item is IDyable)
                        {
                            if (((IDyable)item).Dye(from, m_Tub))
                                from.PlaySound(0x23E);
                        }
                        else
                        {
                            item.Hue = m_Tub.DyedHue;
                            from.PlaySound(0x23E);
                        }
                    }
                    else if ((item is BaseArmor && (((BaseArmor)item).MaterialType == ArmorMaterialType.Chainmail || ((BaseArmor)item).MaterialType == ArmorMaterialType.Ringmail || ((BaseArmor)item).MaterialType == ArmorMaterialType.Plate)) && m_Tub.AllowMetal)
                    {
                        if (!from.InRange(m_Tub.GetWorldLocation(), 1) || !from.InRange(item.GetWorldLocation(), 1))
                        {
                            from.SendLocalizedMessage(500446); // That is too far away.
                        }
                        else if (!item.Movable)
                        {
                            from.SendLocalizedMessage(1042419); // You may not dye leather items which are locked down.
                        }
                        else if (item.Parent is Mobile)
                        {
                            from.SendLocalizedMessage(500861); // Can't Dye clothing that is being worn.
                        }
                        else
                        {
                            item.Hue = m_Tub.DyedHue;
                            from.PlaySound(0x23E);
                        }
                    }
                    else
                    {
                        from.SendLocalizedMessage(m_Tub.FailMessage);
                    }
                }
                else
                {
                    from.SendLocalizedMessage(m_Tub.FailMessage);
                }
            }
        }
    }
}