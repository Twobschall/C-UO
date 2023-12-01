/*	System Configuration, changes to the system should be 
	made here instead of changes to the scripts
*/
using System;
using Server;
using Server.Items;
using Server.Guilds;
using Server.Mobiles;
using Server.Network;
using Server.Accounting;
using Server.Custom;
using System.Collections;
using Xanthos.Interfaces;
using Xanthos.ShrinkSystem;
using Daat99MasterLooterSystem;

namespace Server
{
    public class PlayerPackStart
    {		
		#region Add your entries of items you want to drop in pack
		public bool AddToBackpackOnAttach					= false;
		public static void CustomBackPackDrops(Mobile m)
		{
			/* Format;  
			m.AddToBackPack(new Item(ValueifAny));
			*/
			m.AddToBackpack(new RedBook());
			m.AddToBackpack(new Gold(10000));
			m.AddToBackpack(new Candle());
			m.AddToBackpack(new Dagger());
			//m.AddToBackpack(new MasterLooterBackpack());
		}
		#endregion
	}
}