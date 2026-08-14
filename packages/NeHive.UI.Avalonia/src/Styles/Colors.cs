using Avalonia.Media;

namespace NeHive.UI.Avalonia.Styles;

// https://tailwindcss.com/docs/colors
public static class Colors
{
    public static readonly Color Black = Color.FromRgb(0, 0, 0);
    public static readonly Color White = Color.FromRgb(255, 255, 255);
    public static readonly Color Transparent = Color.FromArgb(0, 0, 0, 0);

    public static readonly Color Red50 = Color.FromRgb(254, 242, 242);
    public static readonly Color Red100 = Color.FromRgb(254, 226, 226);
    public static readonly Color Red200 = Color.FromRgb(254, 202, 202);
    public static readonly Color Red300 = Color.FromRgb(252, 165, 165);
    public static readonly Color Red400 = Color.FromRgb(248, 113, 113);
    public static readonly Color Red500 = Color.FromRgb(239, 68, 68);
    public static readonly Color Red600 = Color.FromRgb(220, 38, 38);
    public static readonly Color Red700 = Color.FromRgb(185, 28, 28);
    public static readonly Color Red800 = Color.FromRgb(153, 27, 27);
    public static readonly Color Red900 = Color.FromRgb(127, 29, 29);
    public static readonly Color Red950 = Color.FromRgb(69, 10, 10);

    public static readonly Color Orange50 = Color.FromRgb(255, 247, 237);
    public static readonly Color Orange100 = Color.FromRgb(255, 237, 213);
    public static readonly Color Orange200 = Color.FromRgb(254, 215, 170);
    public static readonly Color Orange300 = Color.FromRgb(253, 186, 116);
    public static readonly Color Orange400 = Color.FromRgb(251, 146, 60);
    public static readonly Color Orange500 = Color.FromRgb(249, 115, 22);
    public static readonly Color Orange600 = Color.FromRgb(234, 88, 12);
    public static readonly Color Orange700 = Color.FromRgb(194, 65, 12);
    public static readonly Color Orange800 = Color.FromRgb(154, 52, 18);
    public static readonly Color Orange900 = Color.FromRgb(124, 45, 18);
    public static readonly Color Orange950 = Color.FromRgb(67, 20, 7);

    public static readonly Color Amber50 = Color.FromRgb(255, 251, 235);
    public static readonly Color Amber100 = Color.FromRgb(254, 243, 199);
    public static readonly Color Amber200 = Color.FromRgb(253, 230, 138);
    public static readonly Color Amber300 = Color.FromRgb(252, 211, 77);
    public static readonly Color Amber400 = Color.FromRgb(251, 191, 36);
    public static readonly Color Amber500 = Color.FromRgb(245, 158, 11);
    public static readonly Color Amber600 = Color.FromRgb(217, 119, 6);
    public static readonly Color Amber700 = Color.FromRgb(180, 83, 9);
    public static readonly Color Amber800 = Color.FromRgb(146, 64, 14);
    public static readonly Color Amber900 = Color.FromRgb(120, 53, 15);
    public static readonly Color Amber950 = Color.FromRgb(69, 26, 3);

    public static readonly Color Yellow50 = Color.FromRgb(254, 252, 232);
    public static readonly Color Yellow100 = Color.FromRgb(254, 249, 195);
    public static readonly Color Yellow200 = Color.FromRgb(254, 240, 138);
    public static readonly Color Yellow300 = Color.FromRgb(253, 224, 71);
    public static readonly Color Yellow400 = Color.FromRgb(250, 204, 21);
    public static readonly Color Yellow500 = Color.FromRgb(234, 179, 8);
    public static readonly Color Yellow600 = Color.FromRgb(202, 138, 4);
    public static readonly Color Yellow700 = Color.FromRgb(161, 98, 7);
    public static readonly Color Yellow800 = Color.FromRgb(133, 77, 14);
    public static readonly Color Yellow900 = Color.FromRgb(113, 63, 18);
    public static readonly Color Yellow950 = Color.FromRgb(66, 32, 6);

    public static readonly Color Lime50 = Color.FromRgb(247, 254, 231);
    public static readonly Color Lime100 = Color.FromRgb(236, 252, 203);
    public static readonly Color Lime200 = Color.FromRgb(217, 249, 157);
    public static readonly Color Lime300 = Color.FromRgb(190, 242, 100);
    public static readonly Color Lime400 = Color.FromRgb(163, 230, 53);
    public static readonly Color Lime500 = Color.FromRgb(132, 204, 22);
    public static readonly Color Lime600 = Color.FromRgb(101, 163, 13);
    public static readonly Color Lime700 = Color.FromRgb(77, 124, 15);
    public static readonly Color Lime800 = Color.FromRgb(63, 98, 18);
    public static readonly Color Lime900 = Color.FromRgb(54, 83, 20);
    public static readonly Color Lime950 = Color.FromRgb(26, 46, 5);

    public static readonly Color Green50 = Color.FromRgb(240, 253, 244);
    public static readonly Color Green100 = Color.FromRgb(220, 252, 231);
    public static readonly Color Green200 = Color.FromRgb(187, 247, 208);
    public static readonly Color Green300 = Color.FromRgb(134, 239, 172);
    public static readonly Color Green400 = Color.FromRgb(74, 222, 128);
    public static readonly Color Green500 = Color.FromRgb(34, 197, 94);
    public static readonly Color Green600 = Color.FromRgb(22, 163, 74);
    public static readonly Color Green700 = Color.FromRgb(21, 128, 61);
    public static readonly Color Green800 = Color.FromRgb(22, 101, 52);
    public static readonly Color Green900 = Color.FromRgb(20, 83, 45);
    public static readonly Color Green950 = Color.FromRgb(5, 46, 22);

    public static readonly Color Emerald50 = Color.FromRgb(236, 253, 245);
    public static readonly Color Emerald100 = Color.FromRgb(209, 250, 229);
    public static readonly Color Emerald200 = Color.FromRgb(167, 243, 208);
    public static readonly Color Emerald300 = Color.FromRgb(110, 231, 183);
    public static readonly Color Emerald400 = Color.FromRgb(52, 211, 153);
    public static readonly Color Emerald500 = Color.FromRgb(16, 185, 129);
    public static readonly Color Emerald600 = Color.FromRgb(5, 150, 105);
    public static readonly Color Emerald700 = Color.FromRgb(4, 120, 87);
    public static readonly Color Emerald800 = Color.FromRgb(6, 95, 70);
    public static readonly Color Emerald900 = Color.FromRgb(6, 78, 59);
    public static readonly Color Emerald950 = Color.FromRgb(2, 44, 34);

    public static readonly Color Teal50 = Color.FromRgb(240, 253, 250);
    public static readonly Color Teal100 = Color.FromRgb(204, 251, 241);
    public static readonly Color Teal200 = Color.FromRgb(153, 246, 228);
    public static readonly Color Teal300 = Color.FromRgb(94, 234, 212);
    public static readonly Color Teal400 = Color.FromRgb(45, 212, 191);
    public static readonly Color Teal500 = Color.FromRgb(20, 184, 166);
    public static readonly Color Teal600 = Color.FromRgb(13, 148, 136);
    public static readonly Color Teal700 = Color.FromRgb(15, 118, 110);
    public static readonly Color Teal800 = Color.FromRgb(17, 94, 89);
    public static readonly Color Teal900 = Color.FromRgb(19, 78, 74);
    public static readonly Color Teal950 = Color.FromRgb(4, 47, 46);

    public static readonly Color Cyan50 = Color.FromRgb(236, 254, 255);
    public static readonly Color Cyan100 = Color.FromRgb(207, 250, 254);
    public static readonly Color Cyan200 = Color.FromRgb(165, 243, 252);
    public static readonly Color Cyan300 = Color.FromRgb(103, 232, 249);
    public static readonly Color Cyan400 = Color.FromRgb(34, 211, 238);
    public static readonly Color Cyan500 = Color.FromRgb(6, 182, 212);
    public static readonly Color Cyan600 = Color.FromRgb(8, 145, 178);
    public static readonly Color Cyan700 = Color.FromRgb(14, 116, 144);
    public static readonly Color Cyan800 = Color.FromRgb(21, 94, 117);
    public static readonly Color Cyan900 = Color.FromRgb(22, 78, 99);
    public static readonly Color Cyan950 = Color.FromRgb(8, 51, 68);

    public static readonly Color Sky50 = Color.FromRgb(240, 249, 255);
    public static readonly Color Sky100 = Color.FromRgb(224, 242, 254);
    public static readonly Color Sky200 = Color.FromRgb(186, 230, 253);
    public static readonly Color Sky300 = Color.FromRgb(125, 211, 252);
    public static readonly Color Sky400 = Color.FromRgb(56, 189, 248);
    public static readonly Color Sky500 = Color.FromRgb(14, 165, 233);
    public static readonly Color Sky600 = Color.FromRgb(2, 132, 199);
    public static readonly Color Sky700 = Color.FromRgb(3, 105, 161);
    public static readonly Color Sky800 = Color.FromRgb(7, 89, 133);
    public static readonly Color Sky900 = Color.FromRgb(12, 74, 110);
    public static readonly Color Sky950 = Color.FromRgb(8, 47, 73);

    public static readonly Color Blue50 = Color.FromRgb(239, 246, 255);
    public static readonly Color Blue100 = Color.FromRgb(219, 234, 254);
    public static readonly Color Blue200 = Color.FromRgb(191, 219, 254);
    public static readonly Color Blue300 = Color.FromRgb(147, 197, 253);
    public static readonly Color Blue400 = Color.FromRgb(96, 165, 250);
    public static readonly Color Blue500 = Color.FromRgb(59, 130, 246);
    public static readonly Color Blue600 = Color.FromRgb(37, 99, 235);
    public static readonly Color Blue700 = Color.FromRgb(29, 78, 216);
    public static readonly Color Blue800 = Color.FromRgb(30, 64, 175);
    public static readonly Color Blue900 = Color.FromRgb(30, 58, 138);
    public static readonly Color Blue950 = Color.FromRgb(23, 37, 84);

    public static readonly Color Indigo50 = Color.FromRgb(238, 242, 255);
    public static readonly Color Indigo100 = Color.FromRgb(224, 231, 255);
    public static readonly Color Indigo200 = Color.FromRgb(199, 210, 254);
    public static readonly Color Indigo300 = Color.FromRgb(165, 180, 252);
    public static readonly Color Indigo400 = Color.FromRgb(129, 140, 248);
    public static readonly Color Indigo500 = Color.FromRgb(99, 102, 241);
    public static readonly Color Indigo600 = Color.FromRgb(79, 70, 229);
    public static readonly Color Indigo700 = Color.FromRgb(67, 56, 202);
    public static readonly Color Indigo800 = Color.FromRgb(55, 48, 163);
    public static readonly Color Indigo900 = Color.FromRgb(49, 46, 129);
    public static readonly Color Indigo950 = Color.FromRgb(30, 27, 75);

    public static readonly Color Violet50 = Color.FromRgb(245, 243, 255);
    public static readonly Color Violet100 = Color.FromRgb(237, 233, 254);
    public static readonly Color Violet200 = Color.FromRgb(221, 214, 254);
    public static readonly Color Violet300 = Color.FromRgb(196, 181, 253);
    public static readonly Color Violet400 = Color.FromRgb(167, 139, 250);
    public static readonly Color Violet500 = Color.FromRgb(139, 92, 246);
    public static readonly Color Violet600 = Color.FromRgb(124, 58, 237);
    public static readonly Color Violet700 = Color.FromRgb(109, 40, 217);
    public static readonly Color Violet800 = Color.FromRgb(91, 33, 182);
    public static readonly Color Violet900 = Color.FromRgb(76, 29, 149);
    public static readonly Color Violet950 = Color.FromRgb(46, 16, 101);

    public static readonly Color Purple50 = Color.FromRgb(250, 245, 255);
    public static readonly Color Purple100 = Color.FromRgb(243, 232, 255);
    public static readonly Color Purple200 = Color.FromRgb(233, 213, 255);
    public static readonly Color Purple300 = Color.FromRgb(216, 180, 254);
    public static readonly Color Purple400 = Color.FromRgb(192, 132, 252);
    public static readonly Color Purple500 = Color.FromRgb(168, 85, 247);
    public static readonly Color Purple600 = Color.FromRgb(147, 51, 234);
    public static readonly Color Purple700 = Color.FromRgb(126, 34, 206);
    public static readonly Color Purple800 = Color.FromRgb(107, 33, 168);
    public static readonly Color Purple900 = Color.FromRgb(88, 28, 135);
    public static readonly Color Purple950 = Color.FromRgb(59, 7, 100);

    public static readonly Color Fuchsia50 = Color.FromRgb(253, 244, 255);
    public static readonly Color Fuchsia100 = Color.FromRgb(250, 232, 255);
    public static readonly Color Fuchsia200 = Color.FromRgb(245, 208, 254);
    public static readonly Color Fuchsia300 = Color.FromRgb(240, 171, 252);
    public static readonly Color Fuchsia400 = Color.FromRgb(232, 121, 249);
    public static readonly Color Fuchsia500 = Color.FromRgb(217, 70, 239);
    public static readonly Color Fuchsia600 = Color.FromRgb(192, 38, 211);
    public static readonly Color Fuchsia700 = Color.FromRgb(162, 28, 175);
    public static readonly Color Fuchsia800 = Color.FromRgb(134, 25, 143);
    public static readonly Color Fuchsia900 = Color.FromRgb(112, 26, 117);
    public static readonly Color Fuchsia950 = Color.FromRgb(74, 4, 78);

    public static readonly Color Pink50 = Color.FromRgb(253, 242, 248);
    public static readonly Color Pink100 = Color.FromRgb(252, 231, 243);
    public static readonly Color Pink200 = Color.FromRgb(251, 207, 232);
    public static readonly Color Pink300 = Color.FromRgb(249, 168, 212);
    public static readonly Color Pink400 = Color.FromRgb(244, 114, 182);
    public static readonly Color Pink500 = Color.FromRgb(236, 72, 153);
    public static readonly Color Pink600 = Color.FromRgb(219, 39, 119);
    public static readonly Color Pink700 = Color.FromRgb(190, 24, 93);
    public static readonly Color Pink800 = Color.FromRgb(157, 23, 77);
    public static readonly Color Pink900 = Color.FromRgb(131, 24, 67);
    public static readonly Color Pink950 = Color.FromRgb(80, 7, 36);

    public static readonly Color Rose50 = Color.FromRgb(255, 241, 242);
    public static readonly Color Rose100 = Color.FromRgb(255, 228, 230);
    public static readonly Color Rose200 = Color.FromRgb(254, 205, 211);
    public static readonly Color Rose300 = Color.FromRgb(253, 164, 175);
    public static readonly Color Rose400 = Color.FromRgb(251, 113, 133);
    public static readonly Color Rose500 = Color.FromRgb(244, 63, 94);
    public static readonly Color Rose600 = Color.FromRgb(225, 29, 72);
    public static readonly Color Rose700 = Color.FromRgb(190, 18, 60);
    public static readonly Color Rose800 = Color.FromRgb(159, 18, 57);
    public static readonly Color Rose900 = Color.FromRgb(136, 19, 55);
    public static readonly Color Rose950 = Color.FromRgb(76, 5, 25);

    public static readonly Color Slate50 = Color.FromRgb(248, 250, 252);
    public static readonly Color Slate100 = Color.FromRgb(241, 245, 249);
    public static readonly Color Slate200 = Color.FromRgb(226, 232, 240);
    public static readonly Color Slate300 = Color.FromRgb(203, 213, 225);
    public static readonly Color Slate400 = Color.FromRgb(148, 163, 184);
    public static readonly Color Slate500 = Color.FromRgb(100, 116, 139);
    public static readonly Color Slate600 = Color.FromRgb(71, 85, 105);
    public static readonly Color Slate700 = Color.FromRgb(51, 65, 85);
    public static readonly Color Slate800 = Color.FromRgb(30, 41, 59);
    public static readonly Color Slate900 = Color.FromRgb(15, 23, 42);
    public static readonly Color Slate950 = Color.FromRgb(2, 6, 23);

    public static readonly Color Gray50 = Color.FromRgb(249, 250, 251);
    public static readonly Color Gray100 = Color.FromRgb(243, 244, 246);
    public static readonly Color Gray200 = Color.FromRgb(229, 231, 235);
    public static readonly Color Gray300 = Color.FromRgb(209, 213, 219);
    public static readonly Color Gray400 = Color.FromRgb(156, 163, 175);
    public static readonly Color Gray500 = Color.FromRgb(107, 114, 128);
    public static readonly Color Gray600 = Color.FromRgb(75, 85, 99);
    public static readonly Color Gray700 = Color.FromRgb(55, 65, 81);
    public static readonly Color Gray800 = Color.FromRgb(31, 41, 55);
    public static readonly Color Gray900 = Color.FromRgb(17, 24, 39);
    public static readonly Color Gray950 = Color.FromRgb(3, 7, 18);

    public static readonly Color Zinc50 = Color.FromRgb(250, 250, 250);
    public static readonly Color Zinc100 = Color.FromRgb(244, 244, 245);
    public static readonly Color Zinc200 = Color.FromRgb(228, 228, 231);
    public static readonly Color Zinc300 = Color.FromRgb(212, 212, 216);
    public static readonly Color Zinc400 = Color.FromRgb(161, 161, 170);
    public static readonly Color Zinc500 = Color.FromRgb(113, 113, 122);
    public static readonly Color Zinc600 = Color.FromRgb(82, 82, 91);
    public static readonly Color Zinc700 = Color.FromRgb(63, 63, 70);
    public static readonly Color Zinc800 = Color.FromRgb(39, 39, 42);
    public static readonly Color Zinc900 = Color.FromRgb(24, 24, 27);
    public static readonly Color Zinc950 = Color.FromRgb(9, 9, 11);

    public static readonly Color Neutral50 = Color.FromRgb(250, 250, 250);
    public static readonly Color Neutral100 = Color.FromRgb(245, 245, 245);
    public static readonly Color Neutral200 = Color.FromRgb(229, 229, 229);
    public static readonly Color Neutral300 = Color.FromRgb(212, 212, 212);
    public static readonly Color Neutral400 = Color.FromRgb(163, 163, 163);
    public static readonly Color Neutral500 = Color.FromRgb(115, 115, 115);
    public static readonly Color Neutral600 = Color.FromRgb(82, 82, 82);
    public static readonly Color Neutral700 = Color.FromRgb(64, 64, 64);
    public static readonly Color Neutral800 = Color.FromRgb(38, 38, 38);
    public static readonly Color Neutral900 = Color.FromRgb(23, 23, 23);
    public static readonly Color Neutral950 = Color.FromRgb(10, 10, 10);

    public static readonly Color Taupe50 = Color.FromRgb(251, 250, 249);
    public static readonly Color Taupe100 = Color.FromRgb(243, 241, 241);
    public static readonly Color Taupe200 = Color.FromRgb(232, 228, 227);
    public static readonly Color Taupe300 = Color.FromRgb(216, 210, 208);
    public static readonly Color Taupe400 = Color.FromRgb(171, 160, 156);
    public static readonly Color Taupe500 = Color.FromRgb(124, 109, 103);
    public static readonly Color Taupe600 = Color.FromRgb(91, 79, 75);
    public static readonly Color Taupe700 = Color.FromRgb(71, 60, 57);
    public static readonly Color Taupe800 = Color.FromRgb(43, 36, 34);
    public static readonly Color Taupe900 = Color.FromRgb(29, 24, 22);
    public static readonly Color Taupe950 = Color.FromRgb(12, 10, 9);

    public static readonly Color Mauve50 = Color.FromRgb(250, 250, 250);
    public static readonly Color Mauve100 = Color.FromRgb(243, 241, 243);
    public static readonly Color Mauve200 = Color.FromRgb(231, 228, 231);
    public static readonly Color Mauve300 = Color.FromRgb(215, 208, 215);
    public static readonly Color Mauve400 = Color.FromRgb(168, 158, 169);
    public static readonly Color Mauve500 = Color.FromRgb(121, 105, 123);
    public static readonly Color Mauve600 = Color.FromRgb(89, 76, 91);
    public static readonly Color Mauve700 = Color.FromRgb(70, 57, 71);
    public static readonly Color Mauve800 = Color.FromRgb(42, 33, 44);
    public static readonly Color Mauve900 = Color.FromRgb(29, 22, 30);
    public static readonly Color Mauve950 = Color.FromRgb(12, 9, 12);

    public static readonly Color Mist50 = Color.FromRgb(249, 251, 251);
    public static readonly Color Mist100 = Color.FromRgb(241, 243, 243);
    public static readonly Color Mist200 = Color.FromRgb(227, 231, 232);
    public static readonly Color Mist300 = Color.FromRgb(208, 214, 216);
    public static readonly Color Mist400 = Color.FromRgb(156, 168, 171);
    public static readonly Color Mist500 = Color.FromRgb(103, 120, 124);
    public static readonly Color Mist600 = Color.FromRgb(75, 88, 91);
    public static readonly Color Mist700 = Color.FromRgb(57, 68, 71);
    public static readonly Color Mist800 = Color.FromRgb(34, 41, 43);
    public static readonly Color Mist900 = Color.FromRgb(22, 27, 29);
    public static readonly Color Mist950 = Color.FromRgb(9, 11, 12);

    public static readonly Color Olive50 = Color.FromRgb(251, 251, 249);
    public static readonly Color Olive100 = Color.FromRgb(244, 244, 240);
    public static readonly Color Olive200 = Color.FromRgb(232, 232, 227);
    public static readonly Color Olive300 = Color.FromRgb(216, 216, 208);
    public static readonly Color Olive400 = Color.FromRgb(171, 171, 156);
    public static readonly Color Olive500 = Color.FromRgb(124, 124, 103);
    public static readonly Color Olive600 = Color.FromRgb(91, 91, 75);
    public static readonly Color Olive700 = Color.FromRgb(71, 71, 57);
    public static readonly Color Olive800 = Color.FromRgb(43, 43, 34);
    public static readonly Color Olive900 = Color.FromRgb(29, 29, 22);
    public static readonly Color Olive950 = Color.FromRgb(12, 12, 9);

    public static readonly Dictionary<string, Color> ColorDict = new()
    {
        ["black"] = Black,
        ["white"] = White,
        ["transparent"] = Transparent,

        ["red-50"] = Red50,
        ["red-100"] = Red100,
        ["red-200"] = Red200,
        ["red-300"] = Red300,
        ["red-400"] = Red400,
        ["red-500"] = Red500,
        ["red-600"] = Red600,
        ["red-700"] = Red700,
        ["red-800"] = Red800,
        ["red-900"] = Red900,
        ["red-950"] = Red950,

        ["orange-50"] = Orange50,
        ["orange-100"] = Orange100,
        ["orange-200"] = Orange200,
        ["orange-300"] = Orange300,
        ["orange-400"] = Orange400,
        ["orange-500"] = Orange500,
        ["orange-600"] = Orange600,
        ["orange-700"] = Orange700,
        ["orange-800"] = Orange800,
        ["orange-900"] = Orange900,
        ["orange-950"] = Orange950,

        ["amber-50"] = Amber50,
        ["amber-100"] = Amber100,
        ["amber-200"] = Amber200,
        ["amber-300"] = Amber300,
        ["amber-400"] = Amber400,
        ["amber-500"] = Amber500,
        ["amber-600"] = Amber600,
        ["amber-700"] = Amber700,
        ["amber-800"] = Amber800,
        ["amber-900"] = Amber900,
        ["amber-950"] = Amber950,

        ["yellow-50"] = Yellow50,
        ["yellow-100"] = Yellow100,
        ["yellow-200"] = Yellow200,
        ["yellow-300"] = Yellow300,
        ["yellow-400"] = Yellow400,
        ["yellow-500"] = Yellow500,
        ["yellow-600"] = Yellow600,
        ["yellow-700"] = Yellow700,
        ["yellow-800"] = Yellow800,
        ["yellow-900"] = Yellow900,
        ["yellow-950"] = Yellow950,

        ["lime-50"] = Lime50,
        ["lime-100"] = Lime100,
        ["lime-200"] = Lime200,
        ["lime-300"] = Lime300,
        ["lime-400"] = Lime400,
        ["lime-500"] = Lime500,
        ["lime-600"] = Lime600,
        ["lime-700"] = Lime700,
        ["lime-800"] = Lime800,
        ["lime-900"] = Lime900,
        ["lime-950"] = Lime950,

        ["green-50"] = Green50,
        ["green-100"] = Green100,
        ["green-200"] = Green200,
        ["green-300"] = Green300,
        ["green-400"] = Green400,
        ["green-500"] = Green500,
        ["green-600"] = Green600,
        ["green-700"] = Green700,
        ["green-800"] = Green800,
        ["green-900"] = Green900,
        ["green-950"] = Green950,

        ["emerald-50"] = Emerald50,
        ["emerald-100"] = Emerald100,
        ["emerald-200"] = Emerald200,
        ["emerald-300"] = Emerald300,
        ["emerald-400"] = Emerald400,
        ["emerald-500"] = Emerald500,
        ["emerald-600"] = Emerald600,
        ["emerald-700"] = Emerald700,
        ["emerald-800"] = Emerald800,
        ["emerald-900"] = Emerald900,
        ["emerald-950"] = Emerald950,

        ["teal-50"] = Teal50,
        ["teal-100"] = Teal100,
        ["teal-200"] = Teal200,
        ["teal-300"] = Teal300,
        ["teal-400"] = Teal400,
        ["teal-500"] = Teal500,
        ["teal-600"] = Teal600,
        ["teal-700"] = Teal700,
        ["teal-800"] = Teal800,
        ["teal-900"] = Teal900,
        ["teal-950"] = Teal950,

        ["cyan-50"] = Cyan50,
        ["cyan-100"] = Cyan100,
        ["cyan-200"] = Cyan200,
        ["cyan-300"] = Cyan300,
        ["cyan-400"] = Cyan400,
        ["cyan-500"] = Cyan500,
        ["cyan-600"] = Cyan600,
        ["cyan-700"] = Cyan700,
        ["cyan-800"] = Cyan800,
        ["cyan-900"] = Cyan900,
        ["cyan-950"] = Cyan950,

        ["sky-50"] = Sky50,
        ["sky-100"] = Sky100,
        ["sky-200"] = Sky200,
        ["sky-300"] = Sky300,
        ["sky-400"] = Sky400,
        ["sky-500"] = Sky500,
        ["sky-600"] = Sky600,
        ["sky-700"] = Sky700,
        ["sky-800"] = Sky800,
        ["sky-900"] = Sky900,
        ["sky-950"] = Sky950,

        ["blue-50"] = Blue50,
        ["blue-100"] = Blue100,
        ["blue-200"] = Blue200,
        ["blue-300"] = Blue300,
        ["blue-400"] = Blue400,
        ["blue-500"] = Blue500,
        ["blue-600"] = Blue600,
        ["blue-700"] = Blue700,
        ["blue-800"] = Blue800,
        ["blue-900"] = Blue900,
        ["blue-950"] = Blue950,

        ["indigo-50"] = Indigo50,
        ["indigo-100"] = Indigo100,
        ["indigo-200"] = Indigo200,
        ["indigo-300"] = Indigo300,
        ["indigo-400"] = Indigo400,
        ["indigo-500"] = Indigo500,
        ["indigo-600"] = Indigo600,
        ["indigo-700"] = Indigo700,
        ["indigo-800"] = Indigo800,
        ["indigo-900"] = Indigo900,
        ["indigo-950"] = Indigo950,

        ["violet-50"] = Violet50,
        ["violet-100"] = Violet100,
        ["violet-200"] = Violet200,
        ["violet-300"] = Violet300,
        ["violet-400"] = Violet400,
        ["violet-500"] = Violet500,
        ["violet-600"] = Violet600,
        ["violet-700"] = Violet700,
        ["violet-800"] = Violet800,
        ["violet-900"] = Violet900,
        ["violet-950"] = Violet950,

        ["purple-50"] = Purple50,
        ["purple-100"] = Purple100,
        ["purple-200"] = Purple200,
        ["purple-300"] = Purple300,
        ["purple-400"] = Purple400,
        ["purple-500"] = Purple500,
        ["purple-600"] = Purple600,
        ["purple-700"] = Purple700,
        ["purple-800"] = Purple800,
        ["purple-900"] = Purple900,
        ["purple-950"] = Purple950,

        ["fuchsia-50"] = Fuchsia50,
        ["fuchsia-100"] = Fuchsia100,
        ["fuchsia-200"] = Fuchsia200,
        ["fuchsia-300"] = Fuchsia300,
        ["fuchsia-400"] = Fuchsia400,
        ["fuchsia-500"] = Fuchsia500,
        ["fuchsia-600"] = Fuchsia600,
        ["fuchsia-700"] = Fuchsia700,
        ["fuchsia-800"] = Fuchsia800,
        ["fuchsia-900"] = Fuchsia900,
        ["fuchsia-950"] = Fuchsia950,

        ["pink-50"] = Pink50,
        ["pink-100"] = Pink100,
        ["pink-200"] = Pink200,
        ["pink-300"] = Pink300,
        ["pink-400"] = Pink400,
        ["pink-500"] = Pink500,
        ["pink-600"] = Pink600,
        ["pink-700"] = Pink700,
        ["pink-800"] = Pink800,
        ["pink-900"] = Pink900,
        ["pink-950"] = Pink950,

        ["rose-50"] = Rose50,
        ["rose-100"] = Rose100,
        ["rose-200"] = Rose200,
        ["rose-300"] = Rose300,
        ["rose-400"] = Rose400,
        ["rose-500"] = Rose500,
        ["rose-600"] = Rose600,
        ["rose-700"] = Rose700,
        ["rose-800"] = Rose800,
        ["rose-900"] = Rose900,
        ["rose-950"] = Rose950,

        ["slate-50"] = Slate50,
        ["slate-100"] = Slate100,
        ["slate-200"] = Slate200,
        ["slate-300"] = Slate300,
        ["slate-400"] = Slate400,
        ["slate-500"] = Slate500,
        ["slate-600"] = Slate600,
        ["slate-700"] = Slate700,
        ["slate-800"] = Slate800,
        ["slate-900"] = Slate900,
        ["slate-950"] = Slate950,

        ["gray-50"] = Gray50,
        ["gray-100"] = Gray100,
        ["gray-200"] = Gray200,
        ["gray-300"] = Gray300,
        ["gray-400"] = Gray400,
        ["gray-500"] = Gray500,
        ["gray-600"] = Gray600,
        ["gray-700"] = Gray700,
        ["gray-800"] = Gray800,
        ["gray-900"] = Gray900,
        ["gray-950"] = Gray950,

        ["zinc-50"] = Zinc50,
        ["zinc-100"] = Zinc100,
        ["zinc-200"] = Zinc200,
        ["zinc-300"] = Zinc300,
        ["zinc-400"] = Zinc400,
        ["zinc-500"] = Zinc500,
        ["zinc-600"] = Zinc600,
        ["zinc-700"] = Zinc700,
        ["zinc-800"] = Zinc800,
        ["zinc-900"] = Zinc900,
        ["zinc-950"] = Zinc950,

        ["neutral-50"] = Neutral50,
        ["neutral-100"] = Neutral100,
        ["neutral-200"] = Neutral200,
        ["neutral-300"] = Neutral300,
        ["neutral-400"] = Neutral400,
        ["neutral-500"] = Neutral500,
        ["neutral-600"] = Neutral600,
        ["neutral-700"] = Neutral700,
        ["neutral-800"] = Neutral800,
        ["neutral-900"] = Neutral900,
        ["neutral-950"] = Neutral950,

        ["taupe-50"] = Taupe50,
        ["taupe-100"] = Taupe100,
        ["taupe-200"] = Taupe200,
        ["taupe-300"] = Taupe300,
        ["taupe-400"] = Taupe400,
        ["taupe-500"] = Taupe500,
        ["taupe-600"] = Taupe600,
        ["taupe-700"] = Taupe700,
        ["taupe-800"] = Taupe800,
        ["taupe-900"] = Taupe900,
        ["taupe-950"] = Taupe950,

        ["mauve-50"] = Mauve50,
        ["mauve-100"] = Mauve100,
        ["mauve-200"] = Mauve200,
        ["mauve-300"] = Mauve300,
        ["mauve-400"] = Mauve400,
        ["mauve-500"] = Mauve500,
        ["mauve-600"] = Mauve600,
        ["mauve-700"] = Mauve700,
        ["mauve-800"] = Mauve800,
        ["mauve-900"] = Mauve900,
        ["mauve-950"] = Mauve950,

        ["mist-50"] = Mist50,
        ["mist-100"] = Mist100,
        ["mist-200"] = Mist200,
        ["mist-300"] = Mist300,
        ["mist-400"] = Mist400,
        ["mist-500"] = Mist500,
        ["mist-600"] = Mist600,
        ["mist-700"] = Mist700,
        ["mist-800"] = Mist800,
        ["mist-900"] = Mist900,
        ["mist-950"] = Mist950,

        ["olive-50"] = Olive50,
        ["olive-100"] = Olive100,
        ["olive-200"] = Olive200,
        ["olive-300"] = Olive300,
        ["olive-400"] = Olive400,
        ["olive-500"] = Olive500,
        ["olive-600"] = Olive600,
        ["olive-700"] = Olive700,
        ["olive-800"] = Olive800,
        ["olive-900"] = Olive900,
        ["olive-950"] = Olive950,
    };
}