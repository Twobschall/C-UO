using System;
using Server.Accounting;
using Server.Targeting;

namespace Server.Custom
{
    public class ExtraHouseDeed : Item
    {
        private int m_ExtraHousesToAdd = 1;

        [CommandProperty (AccessLevel.Counselor , AccessLevel.Administrator)]
        public int ExtraHousesToAdd
        {
            get { return m_ExtraHousesToAdd; }
            set {
                if (value < 1)
                    value = 1;

                m_ExtraHousesToAdd = value;
            }
        }

        [Constructable]
        public ExtraHouseDeed()
            : base (0x14F0)
        {
            Weight = 1.0;
            LootType = LootType.Blessed;
        }

        public ExtraHouseDeed(Serial serial)
            : base (serial)
        {
        }

        [Constructable]
        public ExtraHouseDeed(int extraHouses)
            : base()
        {
            ExtraHousesToAdd = extraHouses;
        }

        public override string DefaultName
        {
            get {
                return "An extra house deed";
            }
        }
        public override bool DisplayLootType
        {
            get {
                return Core.ML;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize (writer);

            writer.Write ((int)0); // version
            writer.Write ((int)ExtraHousesToAdd);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize (reader);

            int version = reader.ReadInt ();
            ExtraHousesToAdd = reader.ReadInt ();

            LootType = LootType.Blessed;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack)) // Make sure it's in their pack
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
            else
            {
                if (from.Account is Account)
                {
                    Account account = from.Account as Account;
                    
                    // Check if the total allowed houses is less than 3 before allowing the use of the deed
                    int totalAllowedHouses = Multis.BaseHouse.GetAccountHouseLimit(from);
                    if (totalAllowedHouses < 3)
                    {
                        account.ExtraAccountHouses += ExtraHousesToAdd;

                        int newTotalAllowedHouses = Multis.BaseHouse.GetAccountHouseLimit(from);

                        from.SendMessage("Your account may now have " + newTotalAllowedHouses.ToString() + " houses. Max is 3 houses.");
                        
                        Consume();
                    }
                    else
                    {
                        from.SendMessage("You already have the maximum allowed number of houses.");
                    }
                }
                else
                {
                    from.SendMessage("You may not use this deed.");
                }
            }
        }
    }
}
