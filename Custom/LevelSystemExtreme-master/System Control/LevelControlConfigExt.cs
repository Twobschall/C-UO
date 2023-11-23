using System;
using Server;
using System.Xml;
using Server.Misc;
using Server.Gumps;
using Server.Items;
using System.Text;
using Server.Mobiles;
using Server.Network;
using Server.Regions;
using Server.Accounting;
using System.Collections;
using Server.Engines.Craft;
using System.Collections.Generic;
using Server.Engines.PartySystem;
using Server.Engines.XmlSpawnerExtMod;


namespace Server
{
    public class LevelControlConfigExt
    {
		/*	Below Coords refer to location of ControlConfig Token!
			If you want to change the tokens location, move it to the
			spot then props it and put the exact coordinates below! If this
			is wrong then nothing will work!
		*/
		public static int x = 5427;
		public static int y = 1081;
		public static int z = 5;
		public static Map maps = Map.Trammel;
		
		public static bool FindItem(int x, int y, int z, Map map, Item test)
        {
            return FindItem(new Point3D(x, y, z), map, test);
        }
							
		public static bool FindItem(Point3D p, Map map, Item test)
        {
            IPooledEnumerable eable = map.GetItemsInRange(p);

            foreach (Item item in eable)
            {
                if (item.Z == p.Z && item.ItemID == test.ItemID)
                {
                    eable.Free();
                    return true;
                }
            }

            eable.Free();
            return false;
        }
		public static void ReturnHomeLevelControlItem()
		{
			Point3D p = new Point3D(5427, 1081, 5);
			
			LevelControlSysItem loccontrol = null;
			foreach (Item lister in World.Items.Values)
			{
				if (lister is LevelControlSysItem) loccontrol = lister as LevelControlSysItem;
				{
				}
				if (loccontrol != null)
				{
					loccontrol.Location = p;
				}
			}
		}
	}

}