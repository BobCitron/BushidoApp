﻿using System;

using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;
using System.Xml.Linq;

public class Parsing
{
    public static IEnumerable<XElement> ParseCatalog()
    {
        XDocument file = new XDocument();
        file = XDocument.Load("Resources\\catalog\\factionList.xml");
    
        IEnumerable<XElement> factions = new List<XElement>();
        factions = file.Descendants("faction");
       
        return factions;
    }

    public static IEnumerable<XElement> ParseFaction(string path)
    {
        XDocument file = new XDocument();
        file = XDocument.Load(path);

        IEnumerable<XElement> faction = new List<XElement>();
        faction = file.Descendants("character");

        return faction;
    }

}
