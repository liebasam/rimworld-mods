﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimWorld
{
    public class Alert_OrbitalTrader : Alert
    {
        public override string GetLabel()
        {
            foreach (Map map in Find.Maps)
                if (map.passingShipManager.passingShips.Count > 1)
                    return string.Format("OrbitalTrader".Translate(), "s");
            return string.Format("OrbitalTrader".Translate(), "");
        }

        public override string GetExplanation()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (Map map in Find.Maps)
                foreach (PassingShip ship in map.passingShipManager.passingShips)
                    stringBuilder.AppendLine("    " + ship.FullTitle);
            return string.Format("OrbitalTraderDesc".Translate(), stringBuilder.ToString());
        }

        public override AlertReport GetReport()
        {
            foreach (Map map in Find.Maps)
                if (map.passingShipManager.passingShips.Count > 0)
                    if (map.listerBuildings.AllBuildingsColonistOfClass<Building_CommsConsole>().Count() > 0)
                        return (AlertReport)true;
            return (AlertReport)false;
        }
    }
}
