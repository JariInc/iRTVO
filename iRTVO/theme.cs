﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// additional
using Ini;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.IO;

namespace iRTVO
{
    class Theme
    {

        public struct ObjectProperties
        {
            public int top;
            public int left;
            
            public int width;
            public int height;

            // sidepanel & results only
            public int size;
            public int itemHeight;

            public LabelProperties Num;
            public LabelProperties Name;
            public LabelProperties Diff;
            public LabelProperties Info;
        }

        public struct LabelProperties
        {
            public string text;

            /* Position */
            public int top;
            public int left;
            public int width;
            public int height;

            /* Font */
            public System.Windows.Media.FontFamily font;
            public int fontSize;
            public System.Windows.Media.SolidColorBrush fontColor;
            public System.Windows.FontWeight FontBold;
            public System.Windows.FontStyle FontItalic;
            public System.Windows.HorizontalAlignment TextAlign;
        }

        public enum overlayTypes
        {
            main = 0,
            driver = 1,
            sessionstate = 2,
            replay = 3,
            results = 4,
            sidepanel = 5,
            flaggreen = 6,
            flagyellow = 7,
            flagwhite = 8,
            flagcheckered = 9,
            lightsoff = 10,
            lightsred = 11,
            lightsgreen = 12,
            ticker = 13
        }

        public static string[] filenames = new string[13] {
            "main.png",
            "driver.png",
            "laptimer.png",
            "replay.png",
            "results.png",
            "sidepanel.png",
            "flag-green.png",
            "flag-yellow.png",
            "flag-white.png",
            "flag-checkered.png",
            "light-off.png",
            "light-red.png",
            "light-green.png"
        };

        public string name;
        public int width, height;
        public string path;
        private IniFile settings;

        public ObjectProperties driver;
        public ObjectProperties sidepanel;
        public ObjectProperties results;
        public ObjectProperties ticker;
        public LabelProperties resultsHeader;
        public LabelProperties resultsSubHeader;
        public LabelProperties sessionstateText;

        public Dictionary<string, string> translation = new Dictionary<string, string>();

        public Theme(string themeName)
        {
            path = "themes\\" + themeName;

            // if theme not found pick the first theme on theme dir
            if (!File.Exists(Directory.GetCurrentDirectory() + "\\" + path + "\\settings.ini"))
            {
                DirectoryInfo d = new DirectoryInfo(Directory.GetCurrentDirectory() + "\\themes\\");
                DirectoryInfo[] dis = d.GetDirectories();
                foreach (DirectoryInfo di in dis)
                {
                    if (File.Exists(Directory.GetCurrentDirectory() + "\\themes\\" + di.Name + "\\settings.ini"))
                    {
                        themeName = di.Name;
                        break;
                    }
                }
            }

            settings = new IniFile(path + "\\settings.ini");

            name = themeName;
            width = Int32.Parse(getIniValue("General", "width"));
            height = Int32.Parse(getIniValue("General", "height"));

            sidepanel = loadProperties("Sidepanel");

            driver = loadProperties("Driver");

            results = loadProperties("Results");
            resultsHeader = loadLabelProperties("Results", "header");
            resultsSubHeader = loadLabelProperties("Results", "subheader");

            sessionstateText = loadLabelProperties("Sessionstate", "text");

            ticker = loadProperties("Ticker");

            string[] translations = new string[13] {
                    "lap",
                    "laps",
                    "minutes",
                    "of",
                    "race",
                    "qualify",
                    "practice",
                    "out",
                    "remaining",
                    "gridding",
                    "pacelap",
                    "finallap",
                    "finishing"
            };

            foreach (string word in translations)
            {
                string translatedword = getIniValue("Translation", word);
                if(translatedword == "0") // default is the name of the property
                    translation.Add(word, word);
                else
                    translation.Add(word, translatedword);
            }

            // signs
            if (getIniValue("General", "switchsign") == "true")
            {
                translation.Add("ahead", "+");
                translation.Add("behind", "-");
            }
            else
            {
                translation.Add("ahead", "-");
                translation.Add("behind", "+");
            }

        }

        private ObjectProperties loadProperties(string prefix)
        {
            ObjectProperties o = new ObjectProperties();

            o.left = Int32.Parse(getIniValue(prefix, "left"));
            o.top = Int32.Parse(getIniValue(prefix, "top"));
            o.size = Int32.Parse(getIniValue(prefix, "number"));
            o.width = Int32.Parse(getIniValue(prefix, "width"));
            if(Int32.Parse(getIniValue(prefix, "itemheight")) > 0)
                o.height = o.size * Int32.Parse(getIniValue(prefix, "itemheight"));
            else
                o.height = Int32.Parse(getIniValue(prefix, "height"));
            o.itemHeight = Int32.Parse(getIniValue(prefix, "itemheight"));

            o.Num = loadLabelProperties(prefix, "num");
            o.Name = loadLabelProperties(prefix, "name");
            o.Diff = loadLabelProperties(prefix, "diff");
            o.Info = loadLabelProperties(prefix, "info");

            return o;
        }

        private LabelProperties loadLabelProperties(string prefix, string suffix)
        {
            LabelProperties lp = new LabelProperties();

            lp.text = getIniValue(prefix + "-" + suffix, "text");
            if (lp.text == "0")
                lp.text = "{0}";

            lp.fontSize = Int32.Parse(getIniValue(prefix + "-" + suffix, "fontsize"));
            if (lp.fontSize == 0)
                lp.fontSize = 12;

            lp.left = Int32.Parse(getIniValue(prefix + "-" + suffix, "left"));
            lp.top = Int32.Parse(getIniValue(prefix + "-" + suffix, "top"));
            lp.width = Int32.Parse(getIniValue(prefix + "-" + suffix, "width"));
            lp.height = Int32.Parse(getIniValue(prefix + "-" + suffix, "fontsize")) * 3;

            if (File.Exists(@Directory.GetCurrentDirectory() + "\\" + path + "\\" + getIniValue(prefix + "-" + suffix, "font")))
            {
                lp.font = new System.Windows.Media.FontFamily(new Uri(Directory.GetCurrentDirectory() + "\\" + path + "\\" + getIniValue(prefix + "-" + suffix, "font")), getIniValue(prefix + "-" + suffix, "font"));
            }
            else if (getIniValue(prefix + "-" + suffix, "font") == "0")
                lp.font = new System.Windows.Media.FontFamily("Arial");
            else
                lp.font = new System.Windows.Media.FontFamily(getIniValue(prefix + "-" + suffix, "font"));
            
            if(getIniValue(prefix + "-" + suffix, "fontcolor") == "0")
                lp.fontColor = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);
            else
                lp.fontColor = (System.Windows.Media.SolidColorBrush)new System.Windows.Media.BrushConverter().ConvertFromString(getIniValue(prefix + "-" + suffix, "fontcolor"));

            if (getIniValue(prefix + "-" + suffix, "fontbold") == "true")
                lp.FontBold = System.Windows.FontWeights.Bold;
            else
                lp.FontBold = System.Windows.FontWeights.Normal;

            if (getIniValue(prefix + "-" + suffix, "fontitalic") == "true")
                lp.FontItalic = System.Windows.FontStyles.Italic;
            else
                lp.FontItalic = System.Windows.FontStyles.Normal;

            switch (getIniValue(prefix + "-" + suffix, "align"))
            {
                case "center":
                    lp.TextAlign = System.Windows.HorizontalAlignment.Center;
                    break;
                case "right":
                    lp.TextAlign = System.Windows.HorizontalAlignment.Right;
                    break;
                default:
                    lp.TextAlign = System.Windows.HorizontalAlignment.Left;
                    break;
            }

            return lp;
        }

        public string getIniValue(string section, string key)
        {
            string retVal = settings.IniReadValue(section, key);

            if (retVal.Length == 0)
                return "0";
            else
                return retVal;
        }

        public string[] getFormats(SharedData.Driver driver)
        {
            string[] output = new string[12] {
                driver.name,
                driver.shortname,
                driver.initials,
                driver.license,
                driver.club,
                driver.car,
                driver.carclass.ToString(),
                (driver.carId + 1).ToString(),
                iRTVO.Overlay.floatTime2String(driver.fastestlap, true, false),
                iRTVO.Overlay.floatTime2String(driver.previouslap, true, false),
                iRTVO.Overlay.floatTime2String((float)(DateTime.Now - driver.lastNewLap).TotalSeconds, true, false),
                driver.completedlaps.ToString()
            };

            return output;
        }
        /* unused
        public string[] getFormats(SharedData.LapInfo lapinfo)
        {
            string[] output = new string[2] {
                lapinfo.diff.ToString(),
                lapinfo.fastLap.ToString()
            };

            return output;
        }
        */
    }
}
