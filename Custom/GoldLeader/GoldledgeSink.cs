using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Regions;
using Server.Network;
using System.Collections;
using System.Collections.Generic;

namespace Server.Items
{
    public class GoldledgeSink
    {
        public static void Initialize()
        {
            EventSink.Movement += EventSink_Movement;
			EventSink.CreatureDeath += OnCreatureDeath;
        }	
        public static void EventSink_Movement(MovementEventArgs e)
        {
			Mobile from = e.Mobile;
			PlayerMobile pm = from as PlayerMobile;

			if (pm != null)
			{
				/* Adding this here to prevent system earlier, leaving switch 
				in GoldLedger.cs in case it needs to be referenced differently 
				in the future */
				
				Item itemledger = pm.Backpack.FindItemByType(typeof(GoldLedger));

				GoldLedger ledger = itemledger as GoldLedger;

				if (ledger == null)
					return;

				if (!ledger.GoldSweeper || !GoldLedger.GoldSweeperAvailable)
					return;
			
				ArrayList list = new ArrayList();
				
				foreach (Item item in pm.GetItemsInRange(1))
				{
					if (item is Gold && item.Movable == true)
					{
						Gold golditem = item as Gold;
						list.Add( golditem );
					}
				}
				
				foreach ( Gold item in list )
				{
					if (item is Gold && item != null)
					{
						
						GiveGold.GoldSweep(pm, item);
					}
				}
			}
		}
		public static void OnCreatureDeath(CreatureDeathEventArgs e)
        {
            BaseCreature bc = e.Creature as BaseCreature;
            Container c = e.Corpse;
			PlayerMobile pm = e.Killer as PlayerMobile;

			if (bc != null && pm != null && c != null)
			{
				GiveGold.GoldTransfer(pm, c, bc);
			}
		}
	}
}