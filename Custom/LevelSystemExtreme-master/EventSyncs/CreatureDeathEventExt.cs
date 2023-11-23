using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Engines.XmlSpawnerExtMod;

namespace Server.Items
{
    public class CreatureDeathEventExt
    {
        public static void Initialize()
        {
            EventSink.CreatureDeath += OnCreatureDeath;
        }
        public static void OnCreatureDeath(CreatureDeathEventArgs e)
        {
            BaseCreature bc = e.Creature as BaseCreature;
            Container c = e.Corpse;

			/* LevelSystemExt */
			LevelControlSys m_ItemxmlSys = null;
			Point3D p = new Point3D(LevelControlConfigExt.x, LevelControlConfigExt.y, LevelControlConfigExt.z);
			Map map = LevelControlConfigExt.maps;
			foreach (Item item in map.GetItemsInRange(p,3))
			{
				if (item is LevelControlSysItem)
				{
					LevelControlSysItem controlitem1 = item as LevelControlSysItem;
					m_ItemxmlSys = (LevelControlSys)XmlAttachExt.FindAttachment(controlitem1, typeof(LevelControlSys));
				}
			}
			if (m_ItemxmlSys == null){return;}
			if (m_ItemxmlSys.PlayerLevels == false){return;}
			/* LevelSystemExt */
			
			if (m_ItemxmlSys.PlayerLevels == true)
			{
				if (e.Killer is PlayerMobile )
				{
					PlayerMobile pm = e.Killer as PlayerMobile;
					LevelHandlerExt.Set(pm, bc);
					DeleteBankPet(bc);
				}
				if (e.Killer is BaseCreature)
				{
					BaseCreature bcc = e.Killer as BaseCreature;
					if (bcc.Controlled == false)
						return;
					LevelHandlerExt.Set(bcc, bc);
					DeleteBankPet(bc);
				}
			}
        }		
		public static void DeleteBankPet (BaseCreature bc)
		{
			BankBox box = bc.BankBox;
			if (box != null && bc.IsBonded == false)
				box.Delete();
			else
				return;
		}
    }
}
