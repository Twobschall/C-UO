using System;
using Server;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using System.Xml;
using System.Collections.Generic;
using Server.Commands;
using Server.Gumps;
using Server.Accounting;

namespace PVMRanking
{
    public class PVMRankingPoints
    {
        private static Dictionary<PlayerMobile, Item> PendingPrizes = new Dictionary<PlayerMobile, Item>();

        public static void Initialize()
        {
            EventSink.CreatureDeath += CreatureDeath;
            CommandSystem.Register("EndRanking", AccessLevel.Administrator, new CommandEventHandler(EndRanking_OnCommand));
            CommandSystem.Register("Ranking", AccessLevel.Player, new CommandEventHandler(ShowTop10_OnCommand));
            EventSink.Login += OnLogin;
        }

        public static void ShowTop10_OnCommand(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            if (player != null)
            {
                List<PlayerData> topPlayers = GetTopPlayers(10);
                ShowTop10Gump(player, topPlayers);
            }
        }

        public class Top10Gump : Gump
        {
            public Top10Gump(PlayerMobile player, List<PlayerData> topPlayers) : base(0, 0)
            {
                Closable = true;
                Disposable = true;
                Dragable = true;
                Resizable = false;

                AddPage(0);
                AddBackground(50, 50, 400, 300, 9270);
                AddHtml(150, 70, 200, 20, "<basefont color=\"#DC143C\"><center>Top 10 PVM players</center></basefont>", false, false);

                int y = 100;

                for (int i = 0; i < topPlayers.Count; i++)
                {
                    PlayerData playerData = topPlayers[i];
                    int kills = GetCreatureKills("Data/Ranking.xml", playerData.Name);

                    AddLabel(70, y, 34, "{i + 1}. " + playerData.Name + "");
                    AddLabel(200, y, 34, "Kills: " + kills +"");
                    AddLabel(300, y, 34, "Points: " + playerData.Points + "");
                    y += 20;
                }
            }
        }

        public static void ShowTop10Gump(PlayerMobile player, List<PlayerData> topPlayers) 
        {
            if (player != null)
            {
                Top10Gump gump = new Top10Gump(player, topPlayers); 

                player.CloseGump(typeof(Top10Gump)); 

                player.SendGump(gump);
            }
        }

        public static List<PlayerData> GetTopPlayers(int count)
        {
            List<PlayerData> players = GetPlayersData("Data/Ranking.xml");
            players.Sort((a, b) => b.Points.CompareTo(a.Points));

            if (players.Count > count)
            {
                return players.GetRange(0, count);
            }
            else
            {
                return players;
            }
        }

        public static void OnLogin(LoginEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            if (player != null)
            {
                string xmlFilePath = "Data/PendingPrizes.xml"; 

                if (System.IO.File.Exists(xmlFilePath))
                {
                    XmlDocument xmlDoc = new XmlDocument();

                    try
                    {
                        xmlDoc.Load(xmlFilePath);
                        XmlNode root = xmlDoc.SelectSingleNode("PendingPrizes");

                        foreach (XmlNode prizeNode in root.ChildNodes)
                        {
                            if (prizeNode.Attributes != null && prizeNode.Attributes["PlayerName"] != null)
                            {
                                string playerName = prizeNode.Attributes["PlayerName"].Value;
                                
                                if (playerName == player.Name)
                                {
                                    string prizeName = prizeNode.Attributes["PrizeName"].Value;
                                    int prizeHue = int.Parse(prizeNode.Attributes["PrizeHue"].Value);

                                   
                                    Item prize = new Robe();
                                    prize.Name = prizeName;
                                    prize.Hue = prizeHue;

                                    
                                    player.AddToBackpack(prize);

                                    
                                    root.RemoveChild(prizeNode);
                                    xmlDoc.Save(xmlFilePath);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }
                }
            }
        }

        public static void EndRanking_OnCommand(CommandEventArgs e)
        {
            string xmlFilePath = "Data/Ranking.xml"; // Ruta del archivo XML

            ResetMonthlyData(xmlFilePath);
            e.Mobile.SendMessage("Monthly ranking has been reset.");
        }

        public static void CreatureDeath(CreatureDeathEventArgs e)
        {
            BaseCreature killed = e.Creature as BaseCreature;
            PlayerMobile from = DemonKnight.FindRandomPlayer(killed) as PlayerMobile;

            string xmlFilePath = "Data/Ranking.xml";

            if (IsFirstDayOfMonth())
            {
                ResetMonthlyData(xmlFilePath);
            }

            if (from != null && from is PlayerMobile && killed is BaseCreature)
            {
                XmlDocument xmlDoc = new XmlDocument();
                Account acct = (Account)from.Account;
                
                    try
                    {
                        xmlDoc.Load(xmlFilePath);
                    }
                    catch
                    {
                        XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                        xmlDoc.AppendChild(xmlDeclaration);
                        xmlDoc.AppendChild(xmlDoc.CreateElement("PlayerData"));
                    }

                    XmlNode root = xmlDoc.SelectSingleNode("PlayerData");
                    XmlNode playerNode = null;

                    foreach (XmlNode node in root.ChildNodes)
                    {
                        if (node.Attributes != null && node.Attributes["name"] != null && node.Attributes["name"].Value == from.Name)
                        {
                            playerNode = node;
                            break;
                        }
                    }

                    if (playerNode == null)
                    {
                        playerNode = xmlDoc.CreateElement("Player");
                        XmlAttribute playerNameAttr = xmlDoc.CreateAttribute("name");
                        playerNameAttr.Value = from.Name;
                        playerNode.Attributes.Append(playerNameAttr);
                        root.AppendChild(playerNode);
                    }

                    int creatureKills = GetCreatureKills(xmlFilePath, from.Name) + 1;

                    // Calculate points based on hitsMaxSeed
                    int hitsMaxSeed = killed.HitsMaxSeed;
                    int points = 1;

                    if (hitsMaxSeed >= 100)
                    {
                        points = (hitsMaxSeed / 100 * 5) / 2;
                    }

                    Console.WriteLine("Points to give: {0} for killing {1}", points, killed);

                    // Update the player's points
                    SetCreatureKillsAndPoints(playerNode, creatureKills, points);

                    xmlDoc.Save(xmlFilePath);
                
            }
        }

        public static int GetCreatureKills(string xmlFilePath, string playerName)
        {
            int creatureKills = 0;
            
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(xmlFilePath);
                XmlNode root = xmlDoc.SelectSingleNode("PlayerData");

                foreach (XmlNode node in root.ChildNodes)
                {
                    if (node.Attributes != null && node.Attributes["name"] != null && node.Attributes["name"].Value == playerName)
                    {
                        XmlNode killsElement = node.SelectSingleNode("CreatureKills");
                        if (killsElement != null)
                        {
                            creatureKills = int.Parse(killsElement.InnerText);
                        }
                        break;
                    }
                }
            }
            catch
            {
                
            }
        return creatureKills;
        }


        public static void SetCreatureKillsAndPoints(XmlNode playerNode, int creatureKills, int points)
        {
            XmlNode killsElement = playerNode.SelectSingleNode("CreatureKills");
            if (killsElement != null)
            {
                killsElement.InnerText = creatureKills.ToString();
            }
            else
            {
                killsElement = playerNode.OwnerDocument.CreateElement("CreatureKills");
                killsElement.InnerText = creatureKills.ToString();
                playerNode.AppendChild(killsElement);
            }

            XmlNode pointsElement = playerNode.SelectSingleNode("Points");
            if (pointsElement != null)
            {
               int currentPoints = int.Parse(pointsElement.InnerText);
                pointsElement.InnerText = (currentPoints + points).ToString(); // Accumulate points
            }
            else
            {
                pointsElement = playerNode.OwnerDocument.CreateElement("Points");
                pointsElement.InnerText = points.ToString();
                playerNode.AppendChild(pointsElement);
            }
        }



        public static bool IsFirstDayOfMonth()
        {
            DateTime currentDate = DateTime.UtcNow;
            if (currentDate.Day == 1)
                return true;
            else
            {
                return false;
            }
        }

        public static void ResetMonthlyData(string xmlFilePath)
        {
            List<PlayerData> playersOfMonth = GetPlayersData(xmlFilePath);

            
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(xmlFilePath);
                XmlNode root = xmlDoc.SelectSingleNode("PlayerData");
                if (root != null)
                {
                    root.RemoveAll(); // Esto eliminará todos los nodos de jugadores
                }
                xmlDoc.Save(xmlFilePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            GrantPrizesToPlayers(playersOfMonth, xmlFilePath);
        }

        public static void GrantPrizesToPlayers(List<PlayerData> playersOfMonth, string xmlFilePath)
        {
            playersOfMonth.Sort((a, b) => b.Points.CompareTo(a.Points)); // Clasifica por puntos en orden descendente

            int position = 1;

            List<PlayerData> playersFromXml = GetPlayersData(xmlFilePath);

            foreach (PlayerData playerData in playersOfMonth)
            {
                playerData.Rank = position;

                Item prize = new Robe();
                string prizeName = ""+playerData.Name+"'s Prize\n"+playerData.Month+"\nPVM rank: "+playerData.Rank+"\nPoints: "+playerData.Points+"";
                prize.Name = prizeName;
                prize.Hue = Utility.RandomMinMax(1, 2999);

                
                AddPrizeToXml(playerData.Name, prize, xmlFilePath);

                
                PlayerMobile recipient = playerData.Player;
                if (recipient != null)
                {
                    recipient.AddToBackpack(prize); // Agrega el premio al inventario del jugador
                    Console.WriteLine("Gifted: " + playerData.Name +"'s Prize " + playerData.Month + " Rank: " + playerData.Rank + " Points: " + playerData.Points + "");
                }
                else
                {
                    StorePendingPrize(playerData.Name, prize);
                }

                position++;
            }
        }

        private static void StorePendingPrize(string playerName, Item prize)
        {
            if (playerName == null || prize == null)
            {
                Console.WriteLine("Error: playerName or prize ----> null.");
                return;
            }

            string pendingPrizesXmlPath = "Data/PendingPrizes.xml"; 

            XmlDocument xmlDoc = new XmlDocument();

            try
            {
                if (!System.IO.File.Exists(pendingPrizesXmlPath))
                {
                    XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                    xmlDoc.AppendChild(xmlDeclaration);
                    xmlDoc.AppendChild(xmlDoc.CreateElement("PendingPrizes"));
                }
                else
                {
                    xmlDoc.Load(pendingPrizesXmlPath);
                }

                XmlNode root = xmlDoc.SelectSingleNode("PendingPrizes");
                XmlNode pendingPrizeNode = xmlDoc.CreateElement("PendingPrize");

                // Almacena el nombre y el color del premio
                pendingPrizeNode.Attributes.Append(xmlDoc.CreateAttribute("PlayerName")).Value = playerName;
                pendingPrizeNode.Attributes.Append(xmlDoc.CreateAttribute("PrizeName")).Value = prize.Name;
                pendingPrizeNode.Attributes.Append(xmlDoc.CreateAttribute("PrizeHue")).Value = prize.Hue.ToString();

                root.AppendChild(pendingPrizeNode);
                xmlDoc.Save(pendingPrizesXmlPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving pending prize: " + ex.Message);
            }
        }


        public static void AddPrizeToXml(string playerName, Item prize, string xmlFilePath)
        {
            if (playerName == null || prize == null)
            {
                Console.WriteLine("Error: playerName or prize --> null.");
                return;
            }

            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(xmlFilePath);
                XmlNode root = xmlDoc.SelectSingleNode("PlayerData");

                foreach (XmlNode node in root.ChildNodes)
                {
                    if (node.Attributes != null && node.Attributes["name"] != null && node.Attributes["name"].Value == playerName)
                    {
                        XmlNode prizesNode = node.SelectSingleNode("Prizes");
                        if (prizesNode == null)
                        {
                            prizesNode = xmlDoc.CreateElement("Prizes");
                            node.AppendChild(prizesNode);
                        }
                        XmlNode prizeNode = xmlDoc.CreateElement("Prize");

                        prizeNode.Attributes.Append(xmlDoc.CreateAttribute("Name")).Value = prize.Name;
                        prizeNode.Attributes.Append(xmlDoc.CreateAttribute("Hue")).Value = prize.Hue.ToString();

                        prizesNode.AppendChild(prizeNode);
                        xmlDoc.Save(xmlFilePath);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        public class PlayerData
        {
            public string Name { get; set; }
            public string Month { get; set; }
            public int Points { get; set; }
            public int Rank { get; set; }
            public PlayerMobile Player { get; set; }
        }

        public static List<PlayerData> GetPlayersData(string xmlFilePath)
        {
            List<PlayerData> playersOfMonth = new List<PlayerData>();

            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(xmlFilePath);
                XmlNode root = xmlDoc.SelectSingleNode("PlayerData");

                foreach (XmlNode node in root.ChildNodes)
                {
                    PlayerData playerData = new PlayerData();
                    if (node.Attributes != null)
                    {
                        playerData.Name = node.Attributes["name"].Value;
                    }
                    XmlNode pointsElement = node.SelectSingleNode("Points");
                    if (pointsElement != null)
                    {
                        playerData.Points = int.Parse(pointsElement.InnerText);
                    }
                    playerData.Month = DateTime.Now.AddMonths(-1).ToString("MMMM yyyy", new System.Globalization.CultureInfo("en-US"));

                    PlayerMobile player = null;
                    foreach (NetState state in NetState.Instances)
                    {
                        Mobile mobile = state.Mobile;
                        if (mobile is PlayerMobile && mobile.Name == node.Attributes["name"].Value)
                        {
                            player = (PlayerMobile)mobile;
                            break;
                        }
                    }

                    playerData.Player = player;
                    playersOfMonth.Add(playerData);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return playersOfMonth;
        }
    }
}
