using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Network;
using System.IO;
using Server.Targeting;

namespace Server.Custom
{
    public class ItemIDChangeDeed : Item
    {
        private readonly List<Layer> m_Layers = new List<Layer>();

        [Constructable]
        public ItemIDChangeDeed() : base(0x14F0)
        {
            Weight = 1.0;
            LootType = LootType.Blessed;
            Name = "Item ID Change Deed";

            // Load layers from the cfg file
            LoadLayersFromCfg();
        }

        public ItemIDChangeDeed(Serial serial) : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
            else
            {
                from.SendGump(new ItemIDChangeDeedGump(from, this));
            }
        }

private void LoadLayersFromCfg()
{
    // Read layers from the cfg file
    string filePath = Path.Combine("config", "ItemIDChangeDeed.cfg");

    try
    {
        if (File.Exists(filePath))
        {
            // Read all lines from the file
            string[] lines = File.ReadAllLines(filePath);

            // Process each line
            foreach (string line in lines)
            {
                // Split the line at '='
                string[] parts = line.Split('=');

                if (parts.Length > 1)
                {
                    string layerName = parts[0].Trim();
                    string value = parts[1].Trim();

                    if (layerName.Equals("Layer", StringComparison.OrdinalIgnoreCase))
                    {
                        // Handle layers
                        Layer layer;
                        if (Enum.TryParse<Layer>(value, out layer))
                        {
                            m_Layers.Add(layer);
                        }
                        else
                        {
                            Console.WriteLine("Invalid layer name in config: " + value + ".");
                        }
                    }
                    else if (layerName.Equals("LayerItemID", StringComparison.OrdinalIgnoreCase))
                    {
                        // Handle layer item IDs
                        string[] itemParts = value.Split(':');
                        if (itemParts.Length == 2)
                        {
                            string layerNameFromCfg = itemParts[0].Trim();
                            int itemID;

                            if (int.TryParse(itemParts[1].Trim(), out itemID))
                            {
                                // Associate the item ID with the layer
                                Console.WriteLine("Associate item ID " + itemID +" with layer " +layerNameFromCfg + ".");
                                // Add your logic to handle the association as needed.
                            }
                            else
                            {
                                Console.WriteLine("Invalid item ID in config:  " + itemParts[1].Trim() +".");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid format for LayerItemID in config: " + value + ".");
                        }
                    }
                }
            }
        }
        else
        {
            Console.WriteLine("Config file not found: " + filePath);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error reading config file: " + ex.Message);
    }
}
        private class ItemIDChangeDeedGump : Gump
        {
            private readonly Mobile m_Player;
            private readonly ItemIDChangeDeed m_ItemIDChangeDeed;

            public ItemIDChangeDeedGump(Mobile player, ItemIDChangeDeed itemIDChangeDeed) : base(50, 50)
            {
                m_Player = player;
                m_ItemIDChangeDeed = itemIDChangeDeed;

                AddPage(0);
                AddBackground(0, 0, 300, 500, 0x13BE);

                AddLabel(20, 20, 0x34, "Select Layer:");

                int buttonY = 50;

                foreach (Layer layer in m_ItemIDChangeDeed.m_Layers)
                {
                    AddButton(20, buttonY, 0xFAB, 0xFAD, (int)layer, GumpButtonType.Reply, 0);
                    AddLabel(60, buttonY, 0x64, layer.ToString());

                    buttonY += 30;
                }
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (info.ButtonID >= 0 && info.ButtonID < m_ItemIDChangeDeed.m_Layers.Count)
                {
                    Layer selectedLayer = m_ItemIDChangeDeed.m_Layers[info.ButtonID];

                    // Call a method to generate the next stage of the gump with item IDs for the selected layer
                    SendArmsGump(selectedLayer);
                }
            }

            private void SendArmsGump(Layer selectedLayer)
            {
                Console.WriteLine("SendArmsGump method called!");
                m_Player.SendGump(new ArmsGump(m_Player, selectedLayer));
            }
        }

        private class ArmsGump : Gump
{
    private Mobile m_Player;
    private Layer m_SelectedLayer;

    public ArmsGump(Mobile player, Layer selectedLayer) : base(50, 50)
    {
        Console.WriteLine("ArmsGump method opened!");
        m_Player = player;
        m_SelectedLayer = selectedLayer;

        AddPage(0);
        AddBackground(0, 0, 300, 200, 0x13BE);

        // Read the cfg file to get the item ID for the selected layer
        int armsItemID = GetItemIDForLayer(m_SelectedLayer);

        // Display the Arms item image
        AddItem(20, 20, armsItemID);

        // Display the Arms item ID
        AddLabel(20, 100, 0x34, "Item ID: " + armsItemID);

        // Add a button to target an item in the backpack
        AddButton(20, 150, 0xFAB, 0xFAD, 1, GumpButtonType.Reply, 0);
        AddLabel(60, 150, 0x64, "Target Item in Backpack");
    }

    public override void OnResponse(NetState sender, RelayInfo info)
    {
        if (info.ButtonID == 1)
        {
            // Call a method to handle targeting an item in the backpack
            HandleTargetItem();
        }
    }

    private void HandleTargetItem()
    {
        // Implement your logic to handle targeting an item in the backpack here...
        // Example: You can use Target to let the player select an item
        m_Player.Target = new ItemTarget(this);
    }

    private class ItemTarget : Target
    {
        private ArmsGump m_ArmsGump;

        public ItemTarget(ArmsGump armsGump) : base(1, false, TargetFlags.None)
        {
            m_ArmsGump = armsGump;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            // Check if the targeted object is an item in the backpack
            Item item = targeted as Item;
            if (item != null && item.IsChildOf(from.Backpack))
            {
                // Implement your logic to handle the targeted item here...
                from.SendMessage("You targeted item: " + item.ItemID);
            }
            else
            {
                from.SendMessage("Invalid target. Please target an item in your backpack.");
            }
        }
    }

    private int GetItemIDForLayer(Layer layer)
    {
        // Read the cfg file
        string filePath = Path.Combine("config", "ItemIDChangeDeed.cfg");

        try
        {
            if (File.Exists(filePath))
            {
                // Read all lines from the file
                string[] lines = File.ReadAllLines(filePath);

                // Find the line that corresponds to the selected layer
                foreach (string line in lines)
                {
                    // Split the line at '=' and take the second part as the layer name
                    string[] parts = line.Split('=');
                    if (parts.Length > 1)
                    {
                        string layerName = parts[0].Trim();

                        // Check if the layer name matches the selected layer
                        Layer cfgLayer;
                        if (Enum.TryParse<Layer>(layerName, out cfgLayer) && cfgLayer == m_SelectedLayer)
                        {
                            // Parse the item ID from the second part of the line
                            int itemID;
                            if (int.TryParse(parts[1].Trim(), out itemID))
                            {
                                return itemID;
                            }
                            else
                            {
                                Console.WriteLine("Invalid item ID in config for layer " + layerName + ".");
                            }
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Config file not found: " + filePath);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error reading config file: " + ex.Message);
        }

        // Return 0 or handle appropriately if the item ID is not found
        return 0;
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

