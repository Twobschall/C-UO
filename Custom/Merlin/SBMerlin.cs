using System;
using System.Collections.Generic;
using Server.Items;
using Server.Custom;
using Xanthos.Interfaces;
using Xanthos.ShrinkSystem;

namespace Server.Mobiles
{
    public class SBMerlin : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();

        public SBMerlin()
        {
        }

        public override IShopSellInfo SellInfo
        {
            get { return m_SellInfo; }
        }

        public override List<GenericBuyInfo> BuyInfo
        {
            get { return m_BuyInfo; }
        }

        public class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo()
            {                
                Add(new GenericBuyInfo(typeof(ItemBlessDeed), 1000, 999, 0x14F0, 0));                
                Add(new GenericBuyInfo(typeof(NightSightDeedWearable), 5000, 999, 0x14F0, 0));
                Add(new GenericBuyInfo(typeof(TravelBook), 10000, 999, 0x2D50, 0));
                Add(new GenericBuyInfo(typeof(BankStone), 10000, 999, 0x1F1C, 0));
                Add(new GenericBuyInfo(typeof(ArrowBag), 100000, 999, 0xE76, 0));
                Add(new GenericBuyInfo(typeof(UniversalDyeTub), 100000, 999, 0xFAB, 0));
                Add(new GenericBuyInfo(typeof(PetLeash), 200000, 999, 0x1374, 0));
                Add(new GenericBuyInfo(typeof(BlessBag), 200000, 999, 0xE76, 0));
                Add(new GenericBuyInfo(typeof(EtherealMountDeed), 250000, 999, 0x14F0, 0));
                Add(new GenericBuyInfo(typeof(OneHanderDeed), 250000, 999, 0x14F0, 0));
                Add(new GenericBuyInfo(typeof(WeaponRangeDeed), 250000, 999, 0x14F0, 0));
                Add(new GenericBuyInfo(typeof(RaiseMaxLevelScroll), 1000000, 999, 0x14F0, 0));
                Add(new GenericBuyInfo(typeof(ExtraHouseDeed), 1000000, 999, 0x14F0, 0));
                Add(new GenericBuyInfo(typeof(IncreaseFollowersDeed), 2000000, 999, 0x14F0, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                // You can add sell information if needed
            }
        }
    }
}
