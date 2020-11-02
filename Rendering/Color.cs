using System;

namespace Weary.Rendering
{
    public struct Color
    {
        #region Color definitions
        public static readonly Color Black = new Color((uint)0x00_00_00_ff);
        public static readonly Color White = new Color(0xff_ff_ff_ff);
        public static readonly Color Red = new Color(0xff_00_00_ff);
        public static readonly Color Green = new Color(0x00_ff_00_ff);
        public static readonly Color Blue = new Color(0x00_00_ff_ff);
        public static readonly Color Grey = new Color(0x7f_7f_7f_ff);
        public static readonly Color TransparentBlack = new Color((uint)0x00_00_00_00);
        public static readonly Color TransparentWhite = new Color(0xff_ff_ff_00);
        public static readonly Color AliceBlue = new Color(0xf0_f8_ff_ff);
        public static readonly Color AntiqueWhite = new Color(0xfa_eb_d7_ff);
        public static readonly Color Aqua = new Color(0x00_ff_ff_ff);
        public static readonly Color Aquamarine = new Color(0x7f_ff_d4_ff);
        public static readonly Color Azure = new Color(0xf0_ff_ff_ff);
        public static readonly Color Beige = new Color(0xf5_f5_dc_ff);
        public static readonly Color Bisque = new Color(0xff_e4_c4_ff);
        public static readonly Color BlanchedAlmond = new Color(0xff_eb_cd_ff);
        public static readonly Color BlueViolet = new Color(0x8a_2b_e2_ff);
        public static readonly Color Brown = new Color(0xa5_2a_2a_ff);
        public static readonly Color BurlyWood = new Color(0xde_b8_87_ff);
        public static readonly Color CadetBlue = new Color(0x5f_9e_a0_ff);
        public static readonly Color Chartreuse = new Color(0x7f_ff_00_ff);
        public static readonly Color Chocolate = new Color(0xd2_69_1e_ff);
        public static readonly Color Coral = new Color(0xff_7f_50_ff);
        public static readonly Color CornflowerBlue = new Color(0x64_95_ed_ff);
        public static readonly Color Cornsilk = new Color(0xff_f8_dc_ff);
        public static readonly Color Crimson = new Color(0xdc_14_3c_ff);
        public static readonly Color Cyan = new Color(0x00_ff_ff_ff);
        public static readonly Color DarkBlue = new Color(0x00_00_8b_ff);
        public static readonly Color DarkCyan = new Color(0x00_8b_8b_ff);
        public static readonly Color DarkGoldenrod = new Color(0xb8_86_0b_ff);
        public static readonly Color DarkGray = new Color(0xa9_a9_a9_ff);
        public static readonly Color DarkGreen = new Color(0x00_64_00_ff);
        public static readonly Color DarkKhaki = new Color(0xbd_b7_6b_ff);
        public static readonly Color DarkMagenta = new Color(0x8b_00_8b_ff);
        public static readonly Color DarkOliveGreen = new Color(0x55_6b_2f_ff);
        public static readonly Color DarkOrange = new Color(0xff_8c_00_ff);
        public static readonly Color DarkOrchid = new Color(0x99_32_cc_ff);
        public static readonly Color DarkRed = new Color(0x8b_00_00_ff);
        public static readonly Color DarkSalmon = new Color(0xe9_96_7a_ff);
        public static readonly Color DarkSeaGreen = new Color(0x8f_bc_8b_ff);
        public static readonly Color DarkSlateBlue = new Color(0x48_3d_8b_ff);
        public static readonly Color DarkSlateGray = new Color(0x2f_4f_4f_ff);
        public static readonly Color DarkTurquoise = new Color(0x00_ce_d1_ff);
        public static readonly Color DarkViolet = new Color(0x94_00_d3_ff);
        public static readonly Color DeepPink = new Color(0xff_14_93_ff);
        public static readonly Color DeepSkyBlue = new Color(0x00_bf_ff_ff);
        public static readonly Color DimGray = new Color(0x69_69_69_ff);
        public static readonly Color DodgerBlue = new Color(0x1e_90_ff_ff);
        public static readonly Color Firebrick = new Color(0xb2_22_22_ff);
        public static readonly Color FloralWhite = new Color(0xff_fa_f0_ff);
        public static readonly Color ForestGreen = new Color(0x22_8b_22_ff);
        public static readonly Color Fuchsia = new Color(0xff_00_ff_ff);
        public static readonly Color Gainsboro = new Color(0xdc_dc_dc_ff);
        public static readonly Color GhostWhite = new Color(0xf8_f8_ff_ff);
        public static readonly Color Gold = new Color(0xff_d7_00_ff);
        public static readonly Color Goldenrod = new Color(0xda_a5_20_ff);
        public static readonly Color GreenYellow = new Color(0xad_ff_2f_ff);
        public static readonly Color Honeydew = new Color(0xf0_ff_f0_ff);
        public static readonly Color HotPink = new Color(0xff_69_b4_ff);
        public static readonly Color IndianRed = new Color(0xcd_5c_5c_ff);
        public static readonly Color Indigo = new Color(0x4b_00_82_ff);
        public static readonly Color Ivory = new Color(0xff_ff_f0_ff);
        public static readonly Color Khaki = new Color(0xf0_e6_8c_ff);
        public static readonly Color Lavender = new Color(0xe6_e6_fa_ff);
        public static readonly Color LavenderBlush = new Color(0xff_f0_f5_ff);
        public static readonly Color LawnGreen = new Color(0x7c_fc_00_ff);
        public static readonly Color LemonChiffon = new Color(0xff_fa_cd_ff);
        public static readonly Color LightBlue = new Color(0xad_d8_e6_ff);
        public static readonly Color LightCoral = new Color(0xf0_80_80_ff);
        public static readonly Color LightCyan = new Color(0xe0_ff_ff_ff);
        public static readonly Color LightGray = new Color(0xd3_d3_d3_ff);
        public static readonly Color LightGreen = new Color(0x90_ee_90_ff);
        public static readonly Color LightPink = new Color(0xff_b6_c1_ff);
        public static readonly Color LightSalmon = new Color(0xff_a0_7a_ff);
        public static readonly Color LightSeaGreen = new Color(0x20_b2_aa_ff);
        public static readonly Color LightSkyBlue = new Color(0x87_ce_fa_ff);
        public static readonly Color LightSlateGray = new Color(0x77_88_99_ff);
        public static readonly Color LightSteelBlue = new Color(0xb0_c4_de_ff);
        public static readonly Color LightYellow = new Color(0xff_ff_e0_ff);
        public static readonly Color Lime = new Color(0x00_ff_00_ff);
        public static readonly Color LimeGreen = new Color(0x32_cd_32_ff);
        public static readonly Color Linen = new Color(0xfa_f0_e6_ff);
        public static readonly Color Magenta = new Color(0xff_00_ff_ff);
        public static readonly Color Maroon = new Color(0x80_00_00_ff);
        public static readonly Color MediumAquamarine = new Color(0x66_cd_aa_ff);
        public static readonly Color MediumBlue = new Color(0x00_00_cd_ff);
        public static readonly Color MediumOrchid = new Color(0xba_55_d3_ff);
        public static readonly Color MediumPurple = new Color(0x93_70_db_ff);
        public static readonly Color MediumSeaGreen = new Color(0x3c_b3_71_ff);
        public static readonly Color MediumSlateBlue = new Color(0x7b_68_ee_ff);
        public static readonly Color MediumSpringGreen = new Color(0x00_fa_9a_ff);
        public static readonly Color MediumTurquoise = new Color(0x48_d1_cc_ff);
        public static readonly Color MediumVioletRed = new Color(0xc7_15_85_ff);
        public static readonly Color MidnightBlue = new Color(0x19_19_70_ff);
        public static readonly Color MintCream = new Color(0xf5_ff_fa_ff);
        public static readonly Color MistyRose = new Color(0xff_e4_e1_ff);
        public static readonly Color Moccasin = new Color(0xff_e4_b5_ff);
        public static readonly Color MonoGameOrange = new Color(0xe7_3c_00_ff);
        public static readonly Color NavajoWhite = new Color(0xff_de_ad_ff);
        public static readonly Color Navy = new Color(0x00_00_80_ff);
        public static readonly Color OldLace = new Color(0xfd_f5_e6_ff);
        public static readonly Color Olive = new Color(0x80_80_00_ff);
        public static readonly Color OliveDrab = new Color(0x6b_8e_23_ff);
        public static readonly Color Orange = new Color(0xff_a5_00_ff);
        public static readonly Color OrangeRed = new Color(0xff_45_00_ff);
        public static readonly Color Orchid = new Color(0xda_70_d6_ff);
        public static readonly Color PaleGoldenrod = new Color(0xee_e8_aa_ff);
        public static readonly Color PaleGreen = new Color(0x98_fb_98_ff);
        public static readonly Color PaleTurquoise = new Color(0xaf_ee_ee_ff);
        public static readonly Color PaleVioletRed = new Color(0xdb_70_93_ff);
        public static readonly Color PapayaWhip = new Color(0xff_ef_d5_ff);
        public static readonly Color PeachPuff = new Color(0xff_da_b9_ff);
        public static readonly Color Peru = new Color(0xcd_85_3f_ff);
        public static readonly Color Pink = new Color(0xff_c0_cb_ff);
        public static readonly Color Plum = new Color(0xdd_a0_dd_ff);
        public static readonly Color PowderBlue = new Color(0xb0_e0_e6_ff);
        public static readonly Color Purple = new Color(0x80_00_80_ff);
        public static readonly Color RosyBrown = new Color(0xbc_8f_8f_ff);
        public static readonly Color RoyalBlue = new Color(0x41_69_e1_ff);
        public static readonly Color SaddleBrown = new Color(0x8b_45_13_ff);
        public static readonly Color Salmon = new Color(0xfa_80_72_ff);
        public static readonly Color SandyBrown = new Color(0xf4_a4_60_ff);
        public static readonly Color SeaGreen = new Color(0x2e_8b_57_ff);
        public static readonly Color SeaShell = new Color(0xff_f5_ee_ff);
        public static readonly Color Sienna = new Color(0xa0_52_2d_ff);
        public static readonly Color Silver = new Color(0xc0_c0_c0_ff);
        public static readonly Color SkyBlue = new Color(0x87_ce_eb_ff);
        public static readonly Color SlateBlue = new Color(0x6a_5a_cd_ff);
        public static readonly Color SlateGray = new Color(0x70_80_90_ff);
        public static readonly Color Snow = new Color(0xff_fa_fa_ff);
        public static readonly Color SpringGreen = new Color(0x00_ff_7f_ff);
        public static readonly Color SteelBlue = new Color(0x46_82_b4_ff);
        public static readonly Color Tan = new Color(0xd2_b4_8c_ff);
        public static readonly Color Teal = new Color(0x00_80_80_ff);
        public static readonly Color Thistle = new Color(0xd8_bf_d8_ff);
        public static readonly Color Tomato = new Color(0xff_63_47_ff);
        public static readonly Color Turquoise = new Color(0x40_e0_d0_ff);
        public static readonly Color Violet = new Color(0xee_82_ee_ff);
        public static readonly Color Wheat = new Color(0xf5_de_b3_ff);
        public static readonly Color WhiteSmoke = new Color(0xf5_f5_f5_ff);
        public static readonly Color Yellow = new Color(0xff_ff_00_ff);
        public static readonly Color YellowGreen = new Color(0x9a_cd_32_ff);
        #endregion

        public byte r;
        public byte g;
        public byte b;
        public byte a;

        public Color(Color copy)
        {
            r = copy.r;
            g = copy.g;
            b = copy.b;
            a = copy.a;
        }

        public Color(Color copy, byte a)
        {
            r = copy.r;
            g = copy.g;
            b = copy.b;
            this.a = a;
        }

        public Color(uint rgba)
        {   
            r = (byte)((rgba & 0xff000000U) >> 24);
            g = (byte)((rgba & 0x00ff0000U) >> 16);
            b = (byte)((rgba & 0x0000ff00U) >> 8);
            a = (byte)(rgba & 0x000000ffU);
        }

        public Color(byte x)
        {
            r = g = b = a = x;
        }

        public Color(byte r, byte g, byte b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            a = byte.MaxValue;
        }

        public Color(byte r, byte g, byte b, byte a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }
    }
}