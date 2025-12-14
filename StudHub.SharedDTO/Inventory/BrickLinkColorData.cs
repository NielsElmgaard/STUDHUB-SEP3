using System.Drawing;
using System.Linq;

public static class BrickLinkColorData
{
    public struct ColorInfo
    {
        public string Name { get; set; }
        public string Hex { get; set; }
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }
    }

    // Helper method to convert Hex string (e.g., "720E0F") to ColorInfo
    private static ColorInfo CreateColorInfo(string name, string hex)
    {
        // Add '#' if missing
        string fullHex = hex.StartsWith("#") ? hex : $"#{hex}";

        try
        {
            Color color = ColorTranslator.FromHtml(fullHex);
            return new ColorInfo
            {
                Name = name,
                Hex = fullHex,
                R = color.R,
                G = color.G,
                B = color.B
            };
        }
        catch
        {
            // Fallback for invalid hex codes
            return new ColorInfo
                { Name = name, Hex = "#CCCCCC", R = 204, G = 204, B = 204 };
        }
    }

    public static readonly Dictionary<string, ColorInfo> ColorMap =
        new Dictionary<string, ColorInfo>(StringComparer.OrdinalIgnoreCase)
        {
            // (Not Applicable (N/A))
            { "(Not Applicable)", CreateColorInfo("(Not Applicable)", "#CCCCCC") },

            // --- Red/Pink/Purple Hues ---
            { "Dark Red", CreateColorInfo("Dark Red", "720E0F") },
            {
                "HO Dark Red", CreateColorInfo("HO Dark Red", "720E0F")
            }, // Alias for Dark Red
            { "Light Salmon", CreateColorInfo("Light Salmon", "FEBABD") },
            { "Light Pink", CreateColorInfo("Light Pink", "FECCCF") },
            { "Modulex Violet", CreateColorInfo("Modulex Violet", "BD7D85") },
            { "Duplo Pink", CreateColorInfo("Duplo Pink", "FF879C") },
            { "Pearl Red", CreateColorInfo("Pearl Red", "D60026") },
            { "Pink", CreateColorInfo("Pink", "FC97AC") },
            { "Trans-Neon Red", CreateColorInfo("Trans-Neon Red", "FF0040") },
            { "Coral", CreateColorInfo("Coral", "FF698F") },
            {
                "Medium Dark Pink",
                CreateColorInfo("Medium Dark Pink", "F785B1")
            },
            { "Modulex Pink", CreateColorInfo("Modulex Pink", "F785B1") },
            {
                "Glitter Trans-Dark Pink",
                CreateColorInfo("Glitter Trans-Dark Pink", "DF6695")
            },
            { "Trans-Dark Pink", CreateColorInfo("Trans-Dark Pink", "DF6695") },
            { "Clikits Pink", CreateColorInfo("Clikits Pink", "FE78B0") },
            { "Light Purple", CreateColorInfo("Light Purple", "CD6298") },
            { "Bright Pink", CreateColorInfo("Bright Pink", "E4ADC8") },
            { "Trans-Pink", CreateColorInfo("Trans-Pink", "E4ADC8") },
            {
                "Glitter Trans-Pink",
                CreateColorInfo("Glitter Trans-Pink", "E4ADC8")
            },
            { "Dark Pink", CreateColorInfo("Dark Pink", "C870A0") },
            {
                "Opal Trans-Dark Pink",
                CreateColorInfo("Opal Trans-Dark Pink", "CE1D9B")
            },
            { "Magenta", CreateColorInfo("Magenta", "923978") },
            { "Chrome Pink", CreateColorInfo("Chrome Pink", "AA4D8E") },
            { "Purple", CreateColorInfo("Purple", "81007B") },
            { "Sand Purple", CreateColorInfo("Sand Purple", "845E84") },
            { "Reddish Lilac", CreateColorInfo("Reddish Lilac", "8E5597") },
            {
                "Trans-Light Purple",
                CreateColorInfo("Trans-Light Purple", "96709F")
            },
            { "Medium Lavender", CreateColorInfo("Medium Lavender", "AC78BA") },
            {
                "Opal Trans-Purple",
                CreateColorInfo("Opal Trans-Purple", "8320B7")
            },
            {
                "Modulex Foil Violet",
                CreateColorInfo("Modulex Foil Violet", "4B0082")
            },
            { "Lavender", CreateColorInfo("Lavender", "E1D5ED") },
            {
                "Duplo Dark Purple",
                CreateColorInfo("Duplo Dark Purple", "5F27AA")
            },
            {
                "Glitter Trans-Medium Purple",
                CreateColorInfo("Glitter Trans-Medium Purple", "8D73B3")
            },
            {
                "Trans-Medium Purple",
                CreateColorInfo("Trans-Medium Purple", "8D73B3")
            },
            { "Modulex Black", CreateColorInfo("Modulex Black", "4D4C52") },
            { "Dark Purple", CreateColorInfo("Dark Purple", "3F3691") },
            { "Medium Violet", CreateColorInfo("Medium Violet", "9391E4") },
            {
                "Glitter Trans-Purple",
                CreateColorInfo("Glitter Trans-Purple", "A5A5CB")
            },
            { "Trans-Purple", CreateColorInfo("Trans-Purple", "A5A5CB") },
            { "Light Violet", CreateColorInfo("Light Violet", "C9CAE2") },
            { "Light Lilac", CreateColorInfo("Light Lilac", "9195CA") },
            {
                "Dark Blue-Violet",
                CreateColorInfo("Dark Blue-Violet", "2032B0")
            },
            {
                "Medium Bluish Violet",
                CreateColorInfo("Medium Bluish Violet", "6874CA")
            },
            { "Royal Blue", CreateColorInfo("Royal Blue", "4C61DB") },

            // --- Blue Hues ---
            {
                "Opal Trans-Dark Blue",
                CreateColorInfo("Opal Trans-Dark Blue", "0020A0")
            },
            { "Trans-Dark Blue", CreateColorInfo("Trans-Dark Blue", "0020A0") },
            { "Violet", CreateColorInfo("Violet", "4354A3") },
            {
                "Modulex Light Bluish Gray",
                CreateColorInfo("Modulex Light Bluish Gray", "AFB5C7")
            },
            { "Pearl Black", CreateColorInfo("Pearl Black", "0A1327") },
            { "Sand Blue", CreateColorInfo("Sand Blue", "6074A1") },
            { "Pearl Sand Blue", CreateColorInfo("Pearl Sand Blue", "7988A1") },
            { "HO Medium Blue", CreateColorInfo("HO Medium Blue", "7396C8") },
            { "Blue", CreateColorInfo("Blue", "0055BF") },
            { "Medium Blue", CreateColorInfo("Medium Blue", "5A93DB") },
            { "Dark Blue", CreateColorInfo("Dark Blue", "0A3463") },
            { "HO Dark Blue", CreateColorInfo("HO Dark Blue", "0A3463") },
            {
                "Trans-Light Royal Blue",
                CreateColorInfo("Trans-Light Royal Blue", "B4D4F7")
            },
            {
                "Trans-Medium Blue",
                CreateColorInfo("Trans-Medium Blue", "CFE2F7")
            },
            {
                "Modulex Medium Blue",
                CreateColorInfo("Modulex Medium Blue", "61AFFF")
            },
            { "HO Sand Blue", CreateColorInfo("HO Sand Blue", "6E8AA6") },
            {
                "Bright Light Blue",
                CreateColorInfo("Bright Light Blue", "9FC3E9")
            },
            {
                "Modulex Tile Blue",
                CreateColorInfo("Modulex Tile Blue", "0057A6")
            },
            {
                "Modulex Foil Dark Blue",
                CreateColorInfo("Modulex Foil Dark Blue", "0057A6")
            },
            { "Chrome Blue", CreateColorInfo("Chrome Blue", "6C96BF") },
            { "Pearl Blue", CreateColorInfo("Pearl Blue", "0059A3") },
            { "Light Blue", CreateColorInfo("Light Blue", "B4D2E3") },
            {
                "Trans-Very Lt Blue",
                CreateColorInfo("Trans-Very Lt Blue", "C1DFF0")
            },
            { "Maersk Blue", CreateColorInfo("Maersk Blue", "3592C3") },
            {
                "HO Metallic Sand Blue",
                CreateColorInfo("HO Metallic Sand Blue", "5F7D8C")
            },
            { "Dark Azure", CreateColorInfo("Dark Azure", "078BC9") },
            { "HO Azure", CreateColorInfo("HO Azure", "1591CB") },
            {
                "HO Metallic Blue",
                CreateColorInfo("HO Metallic Blue", "0D4763")
            },
            {
                "Modulex Foil Light Blue",
                CreateColorInfo("Modulex Foil Light Blue", "68AECE")
            },
            {
                "Modulex Pastel Blue",
                CreateColorInfo("Modulex Pastel Blue", "68AECE")
            },
            { "HO Cyan", CreateColorInfo("HO Cyan", "5B98B3") },
            {
                "Modulex Teal Blue",
                CreateColorInfo("Modulex Teal Blue", "467083")
            },
            { "Sky Blue", CreateColorInfo("Sky Blue", "7DBFDD") },
            { "HO Blue-gray", CreateColorInfo("HO Blue-gray", "354E5A") },
            {
                "Duplo Medium Blue",
                CreateColorInfo("Duplo Medium Blue", "3E95B6")
            },
            { "Duplo Blue", CreateColorInfo("Duplo Blue", "009ECE") },
            { "Vintage Blue", CreateColorInfo("Vintage Blue", "039CBD") },
            { "Pastel Blue", CreateColorInfo("Pastel Blue", "5AC4DA") },
            { "Medium Azure", CreateColorInfo("Medium Azure", "36AEBF") },
            { "Light Turquoise", CreateColorInfo("Light Turquoise", "55A5AF") },

            // --- Green Hues ---
            {
                "Glitter Trans-Light Blue",
                CreateColorInfo("Glitter Trans-Light Blue", "68BCC5")
            },
            {
                "Opal Trans-Light Blue",
                CreateColorInfo("Opal Trans-Light Blue", "68BCC5")
            },
            { "Dark Turquoise", CreateColorInfo("Dark Turquoise", "008F9B") },
            {
                "HO Dark Turquoise",
                CreateColorInfo("HO Dark Turquoise", "10929D")
            },
            {
                "Trans-Light Blue",
                CreateColorInfo("Trans-Light Blue", "AEEFEC")
            },
            {
                "Modulex Aqua Green",
                CreateColorInfo("Modulex Aqua Green", "27867E")
            },
            { "Light Aqua", CreateColorInfo("Light Aqua", "ADC3C0") },
            { "HO Aqua", CreateColorInfo("HO Aqua", "B3D7D1") },
            { "Aqua", CreateColorInfo("Aqua", "B3D7D1") },
            { "Duplo Turquoise", CreateColorInfo("Duplo Turquoise", "3FB69E") },
            { "HO Dark Aqua", CreateColorInfo("HO Dark Aqua", "A7DCCF") },
            { "HO Light Aqua", CreateColorInfo("HO Light Aqua", "A3D1C0") },
            { "HO Dark Green", CreateColorInfo("HO Dark Green", "184632") },
            { "Dark Green", CreateColorInfo("Dark Green", "184632") },
            { "Chrome Green", CreateColorInfo("Chrome Green", "3CB371") },
            { "Medium Green", CreateColorInfo("Medium Green", "73DCA1") },
            { "Pearl Green", CreateColorInfo("Pearl Green", "008E3C") },
            { "Sand Green", CreateColorInfo("Sand Green", "A0BCAC") },
            { "HO Sand Green", CreateColorInfo("HO Sand Green", "A0BCAC") },
            {
                "Duplo Medium Green",
                CreateColorInfo("Duplo Medium Green", "468A5F")
            },
            { "Green", CreateColorInfo("Green", "237841") },
            {
                "Trans-Light Green",
                CreateColorInfo("Trans-Light Green", "94E5AB")
            },
            {
                "Duplo Light Green",
                CreateColorInfo("Duplo Light Green", "60BA76")
            },
            {
                "Glitter Trans-Green",
                CreateColorInfo("Glitter Trans-Green", "84B68D")
            },
            { "Trans-Green", CreateColorInfo("Trans-Green", "84B68D") },
            {
                "Opal Trans-Bright Green",
                CreateColorInfo("Opal Trans-Bright Green", "84B68D")
            },
            {
                "Modulex Foil Dark Green",
                CreateColorInfo("Modulex Foil Dark Green", "006400")
            },
            { "Vintage Green", CreateColorInfo("Vintage Green", "1E601E") },
            { "Fabuland Lime", CreateColorInfo("Fabuland Lime", "78FC78") },
            {
                "HO Dark Sand Green",
                CreateColorInfo("HO Dark Sand Green", "627A62")
            },
            { "Bright Green", CreateColorInfo("Bright Green", "4B9F4A") },
            { "Light Green", CreateColorInfo("Light Green", "C2DAB8") },
            {
                "Modulex Foil Light Green",
                CreateColorInfo("Modulex Foil Light Green", "7DB538")
            },
            {
                "Modulex Pastel Green",
                CreateColorInfo("Modulex Pastel Green", "7DB538")
            },
            {
                "Glow In Dark Trans",
                CreateColorInfo("Glow In Dark Trans", "BDC6AD")
            },
            {
                "HO Metallic Green",
                CreateColorInfo("HO Metallic Green", "879867")
            },
            {
                "Speckle DBGray-Silver",
                CreateColorInfo("Speckle DBGray-Silver", "6C6E68")
            },
            {
                "Modulex Olive Green",
                CreateColorInfo("Modulex Olive Green", "7C9051")
            },
            {
                "Trans-Light Bright Green",
                CreateColorInfo("Trans-Light Bright Green", "C9E788")
            },
            { "Pearl Lime", CreateColorInfo("Pearl Lime", "6A7944") },
            { "Metallic Green", CreateColorInfo("Metallic Green", "899B5F") },
            {
                "Glitter Trans-Neon Green",
                CreateColorInfo("Glitter Trans-Neon Green", "C0F500")
            },
            { "Neon Green", CreateColorInfo("Neon Green", "D2FC43") },
            { "Lime", CreateColorInfo("Lime", "BBE90B") },
            { "Yellowish Green", CreateColorInfo("Yellowish Green", "DFEEA5") },
            {
                "Glitter Trans-Bright Green",
                CreateColorInfo("Glitter Trans-Bright Green", "D9E4A7")
            },
            { "Light Lime", CreateColorInfo("Light Lime", "D9E4A7") },
            {
                "Trans-Bright Green",
                CreateColorInfo("Trans-Bright Green", "D9E4A7")
            },
            { "Medium Lime", CreateColorInfo("Medium Lime", "C7D23C") },
            { "HO Light Yellow", CreateColorInfo("HO Light Yellow", "F5FAB7") },
            { "Modulex Lemon", CreateColorInfo("Modulex Lemon", "BDC618") },
            { "HO Dark Lime", CreateColorInfo("HO Dark Lime", "B2B955") },
            { "HO Dark Gray", CreateColorInfo("HO Dark Gray", "6D6E5C") },
            { "HO Olive Green", CreateColorInfo("HO Olive Green", "9B9A5A") },
            { "Olive Green", CreateColorInfo("Olive Green", "9B9A5A") },
            {
                "Dark Olive Green",
                CreateColorInfo("Dark Olive Green", "5D5C36")
            },
            { "Duplo Lime", CreateColorInfo("Duplo Lime", "FFF230") },
            {
                "Trans-Neon Green",
                CreateColorInfo("Trans-Neon Green", "F8F184")
            },

            // --- Yellow/Orange/Brown Hues ---
            { "Vibrant Yellow", CreateColorInfo("Vibrant Yellow", "EBD800") },
            {
                "Bright Light Yellow",
                CreateColorInfo("Bright Light Yellow", "FFF03A")
            },
            { "Chrome Gold", CreateColorInfo("Chrome Gold", "BBA53D") },
            {
                "Trans-Fire Yellow",
                CreateColorInfo("Trans-Fire Yellow", "FBE890")
            },
            {
                "Trans-Neon Yellow",
                CreateColorInfo("Trans-Neon Yellow", "DAB000")
            },
            { "Vintage Yellow", CreateColorInfo("Vintage Yellow", "F3C305") },
            { "Clikits Yellow", CreateColorInfo("Clikits Yellow", "FFCF0B") },
            { "Trans-Yellow", CreateColorInfo("Trans-Yellow", "F5CD2F") },
            {
                "Opal Trans-Yellow",
                CreateColorInfo("Opal Trans-Yellow", "F5CD2F")
            },
            { "Yellow", CreateColorInfo("Yellow", "F2CD37") },
            {
                "Modulex Light Yellow",
                CreateColorInfo("Modulex Light Yellow", "FFE371")
            },
            { "Light Yellow", CreateColorInfo("Light Yellow", "FBE696") },
            { "HO Gold", CreateColorInfo("HO Gold", "B4A774") },
            { "HO Light Gold", CreateColorInfo("HO Light Gold", "CDC298") },
            {
                "Modulex Ochre Yellow",
                CreateColorInfo("Modulex Ochre Yellow", "FED557")
            },
            {
                "Modulex Foil Yellow",
                CreateColorInfo("Modulex Foil Yellow", "FED557")
            },
            { "Metallic Gold", CreateColorInfo("Metallic Gold", "DBAC34") },
            {
                "Modulex Terracotta",
                CreateColorInfo("Modulex Terracotta", "5C5030")
            },
            {
                "Bright Light Orange",
                CreateColorInfo("Bright Light Orange", "F8BB3D")
            },
            { "Tan", CreateColorInfo("Tan", "E4CD9E") },
            { "HO Tan", CreateColorInfo("HO Tan", "E4CD9E") },
            { "Dark Tan", CreateColorInfo("Dark Tan", "958A73") },
            { "Pearl Gold", CreateColorInfo("Pearl Gold", "AA7F2E") },
            {
                "Pearl Light Gold",
                CreateColorInfo("Pearl Light Gold", "DCBC81")
            },
            { "Medium Orange", CreateColorInfo("Medium Orange", "FFA70B") },
            { "Modulex Buff", CreateColorInfo("Modulex Buff", "DEC69C") },
            { "Dark Brown", CreateColorInfo("Dark Brown", "352100") },
            { "Curry", CreateColorInfo("Curry", "DD982E") },
            {
                "Warm Yellowish Orange",
                CreateColorInfo("Warm Yellowish Orange", "FFCB78")
            },
            { "Light Tan", CreateColorInfo("Light Tan", "F3C988") },
            { "Pearl Copper", CreateColorInfo("Pearl Copper", "B46A00") },
            { "Earth Orange", CreateColorInfo("Earth Orange", "FA9C1C") },
            { "Ochre Yellow", CreateColorInfo("Ochre Yellow", "DD9E47") },
            { "Light Orange", CreateColorInfo("Light Orange", "F9BA61") },
            { "Reddish Gold", CreateColorInfo("Reddish Gold", "AC8247") },
            {
                "Very Light Orange",
                CreateColorInfo("Very Light Orange", "F3CF9B")
            },
            {
                "Chrome Antique Brass",
                CreateColorInfo("Chrome Antique Brass", "645A4C")
            },
            { "HO Earth Orange", CreateColorInfo("HO Earth Orange", "BB771B") },
            { "Modulex Brown", CreateColorInfo("Modulex Brown", "907450") },
            { "Trans-Orange", CreateColorInfo("Trans-Orange", "F08F1C") },
            {
                "Glitter Trans-Orange",
                CreateColorInfo("Glitter Trans-Orange", "F08F1C")
            },
            { "Fabuland Orange", CreateColorInfo("Fabuland Orange", "EF9121") },
            { "Warm Tan", CreateColorInfo("Warm Tan", "CCA373") },
            { "Light Nougat", CreateColorInfo("Light Nougat", "F6D7B3") },
            {
                "Trans-Flame Yellowish Orange",
                CreateColorInfo("Trans-Flame Yellowish Orange", "FCB76D")
            },
            { "Dark Orange", CreateColorInfo("Dark Orange", "A95500") },
            { "Orange", CreateColorInfo("Orange", "FE8A18") },
            {
                "Modulex Foil Orange",
                CreateColorInfo("Modulex Foil Orange", "F7AD63")
            },
            {
                "Modulex Light Orange",
                CreateColorInfo("Modulex Light Orange", "F7AD63")
            },
            { "Flat Dark Gold", CreateColorInfo("Flat Dark Gold", "B48455") },
            { "Two-tone Silver", CreateColorInfo("Two-tone Silver", "737271") },
            {
                "Trans-Neon Orange",
                CreateColorInfo("Trans-Neon Orange", "FF800D")
            },
            { "Fabuland Red", CreateColorInfo("Fabuland Red", "FF8014") },
            { "Medium Nougat", CreateColorInfo("Medium Nougat", "AA7D55") },
            { "Fabuland Brown", CreateColorInfo("Fabuland Brown", "B67B50") },
            { "Medium Brown", CreateColorInfo("Medium Brown", "755945") },
            { "Two-tone Gold", CreateColorInfo("Two-tone Gold", "AB673A") },
            { "Nougat", CreateColorInfo("Nougat", "D09168") },
            { "Modulex Orange", CreateColorInfo("Modulex Orange", "F47B30") },
            { "Sienna Brown", CreateColorInfo("Sienna Brown", "915C3C") },
            { "Brown", CreateColorInfo("Brown", "583927") },
            {
                "Opal Trans-Brown",
                CreateColorInfo("Opal Trans-Brown", "583927")
            },
            { "Copper", CreateColorInfo("Copper", "AE7A59") },
            { "Reddish Brown", CreateColorInfo("Reddish Brown", "5B2A20") },
            { "Reddish Orange", CreateColorInfo("Reddish Orange", "CA4C0B") },
            { "Light Brown", CreateColorInfo("Light Brown", "7C503A") },
            { "HO Light Brown", CreateColorInfo("HO Light Brown", "965336") },
            { "Dark Nougat", CreateColorInfo("Dark Nougat", "AD6140") },
            { "Metallic Copper", CreateColorInfo("Metallic Copper", "764D3B") },
            { "Pearl Brown", CreateColorInfo("Pearl Brown", "57392C") },
            { "Umber Brown", CreateColorInfo("Umber Brown", "5E3F33") },
            { "Neon Orange", CreateColorInfo("Neon Orange", "EC4612") },
            { "Rust Orange", CreateColorInfo("Rust Orange", "872B17") },
            {
                "Bright Reddish Orange",
                CreateColorInfo("Bright Reddish Orange", "EE5434")
            },
            {
                "Modulex Pink Red",
                CreateColorInfo("Modulex Pink Red", "F45C40")
            },
            { "Salmon", CreateColorInfo("Salmon", "F2705E") },
            { "Two-tone Copper", CreateColorInfo("Two-tone Copper", "945148") },
            { "Red", CreateColorInfo("Red", "C91A09") },
            { "Trans-Red", CreateColorInfo("Trans-Red", "C91A09") },
            { "Chrome Red", CreateColorInfo("Chrome Red", "CE3021") },
            { "Rust", CreateColorInfo("Rust", "B31004") },
            { "Modulex Red", CreateColorInfo("Modulex Red", "B52C20") },
            { "Sand Red", CreateColorInfo("Sand Red", "D67572") },
            {
                "Modulex Foil Red",
                CreateColorInfo("Modulex Foil Red", "8B0000")
            },
            {
                "Modulex Tile Brown",
                CreateColorInfo("Modulex Tile Brown", "330000")
            },

            // --- White/Gray/Black Hues ---
            { "Milky White", CreateColorInfo("Milky White", "FFFFFF") },
            { "Trans-Clear", CreateColorInfo("Trans-Clear", "FCFCFC") },
            { "Pearl White", CreateColorInfo("Pearl White", "F2F3F2") },
            { "HO Medium Red", CreateColorInfo("HO Medium Red", "C01111") },
            { "Very Light Gray", CreateColorInfo("Very Light Gray", "E6E3DA") },
            {
                "Very Light Bluish Gray",
                CreateColorInfo("Very Light Bluish Gray", "E6E3E0")
            },
            { "Chrome Silver", CreateColorInfo("Chrome Silver", "E0E0E0") },
            {
                "Glow In Dark Opaque",
                CreateColorInfo("Glow In Dark Opaque", "D4D5C9")
            },
            { "Metal", CreateColorInfo("Metal", "A5ADB4") },
            { "Metallic Silver", CreateColorInfo("Metallic Silver", "A5A9B4") },
            {
                "Pearl Very Light Gray",
                CreateColorInfo("Pearl Very Light Gray", "ABADAC")
            },
            {
                "Light Bluish Gray",
                CreateColorInfo("Light Bluish Gray", "A0A5A9")
            },
            {
                "Pearl Light Gray",
                CreateColorInfo("Pearl Light Gray", "9CA3A8")
            },
            { "Light Gray", CreateColorInfo("Light Gray", "9BA19D") },
            { "Flat Silver", CreateColorInfo("Flat Silver", "898788") },
            { "HO Rose", CreateColorInfo("HO Rose", "D06262") },
            { "Dark Gray", CreateColorInfo("Dark Gray", "6D6E5C") },
            {
                "Dark Bluish Gray",
                CreateColorInfo("Dark Bluish Gray", "6C6E68")
            },
            { "Trans-Brown", CreateColorInfo("Trans-Brown", "635F52") },
            { "Trans-Black", CreateColorInfo("Trans-Black", "635F52") },
            {
                "Trans-Black IR Lens",
                CreateColorInfo("Trans-Black IR Lens", "635F52")
            },
            {
                "Modulex Charcoal Gray",
                CreateColorInfo("Modulex Charcoal Gray", "595D60")
            },
            {
                "Modulex Foil Dark Gray",
                CreateColorInfo("Modulex Foil Dark Gray", "595D60")
            },
            { "Pearl Dark Gray", CreateColorInfo("Pearl Dark Gray", "575857") },
            { "Pearl Titanium", CreateColorInfo("Pearl Titanium", "3E3C39") },
            { "Chrome Black", CreateColorInfo("Chrome Black", "1B2A34") },
            {
                "Modulex Tile Gray",
                CreateColorInfo("Modulex Tile Gray", "6B5A5A")
            },
            {
                "Speckle Black-Gold",
                CreateColorInfo("Speckle Black-Gold", "05131D")
            },
            {
                "Speckle Black-Copper",
                CreateColorInfo("Speckle Black-Copper", "05131D")
            },
            {
                "Speckle Black-Silver",
                CreateColorInfo("Speckle Black-Silver", "05131D")
            },
            { "Black", CreateColorInfo("Black", "05131D") },
            {
                "Glitter Trans-Clear",
                CreateColorInfo("Glitter Trans-Clear", "FFFFFF")
            },
            { "White", CreateColorInfo("White", "FFFFFF") },
            {
                "Glitter Milky White",
                CreateColorInfo("Glitter Milky White", "FFFFFF")
            },
            { "Modulex Clear", CreateColorInfo("Modulex Clear", "FFFFFF") },
            {
                "Opal Trans-Clear",
                CreateColorInfo("Opal Trans-Clear", "FCFCFC")
            },
            { "Modulex White", CreateColorInfo("Modulex White", "F4F4F4") },
            {
                "Glow in Dark White",
                CreateColorInfo("Glow in Dark White", "D9D9D9")
            },
            {
                "Modulex Light Gray",
                CreateColorInfo("Modulex Light Gray", "9C9C9C")
            },
            {
                "Modulex Foil Light Gray",
                CreateColorInfo("Modulex Foil Light Gray", "9C9C9C")
            },
            { "HO Titanium", CreateColorInfo("HO Titanium", "616161") },
            {
                "HO Metallic Dark Gray",
                CreateColorInfo("HO Metallic Dark Gray", "5E5E5E")
            }
        };

    // Helper method to get the hex code
    public static string GetHex(string colorName)
    {
        return ColorMap.TryGetValue(colorName, out var info)
            ? info.Hex
            : "#CCCCCC";
    }

    // Helper method to get all color info (used by GetContrastColor)
    public static bool TryGetInfo(string colorName, out ColorInfo info)
    {
        return ColorMap.TryGetValue(colorName, out info);
    }

    // Helper method to get the list of names for the dropdown
    public static List<string> GetNames()
    {
        const string specialColorName = "(Not Applicable)";

        var colorNames = ColorMap.Keys
            .Where(n => n != specialColorName)
            .ToList();

        colorNames.Sort(StringComparer.OrdinalIgnoreCase);

        // Insert "Not Applicable" at the top
        colorNames.Insert(0, specialColorName);

        return colorNames;
    }
}