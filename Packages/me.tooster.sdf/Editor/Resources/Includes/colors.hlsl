#pragma once
#include <HLSLSupport.cginc>

namespace colors {
    // TODO: express in terms of RGB/255

    #define RGB255(r, g, b) half4(r / 255.0, g / 255.0, b / 255.0, 1)

    // @formatter off
    static const half4 GREEN            = half4(0, 1, 0, 1);
    static const half4 RED              = half4(1, 0, 0, 1);
    static const half4 BLUE             = half4(0, 0, 1, 1);
    static const half4 WHITE            = half4(1, 1, 1, 1);
    static const half4 BLACK            = half4(0, 0, 0, 1);
    static const half4 GREY             = half4(0.5, 0.5, 0.5, 1);
    static const half4 LIGHT_GREY       = half4(0.75, 0.75, 0.75, 1);
    static const half4 DARK_GREY        = half4(0.25, 0.25, 0.25, 1);
    static const half4 YELLOW           = half4(1, 1, 0, 1);
    static const half4 CYAN             = half4(0, 1, 1, 1);
    static const half4 MAGENTA          = half4(1, 0, 1, 1);
    static const half4 PINK             = half4(1, 0.5, 0.5, 1);
    static const half4 ORANGE           = half4(1, 0.5, 0, 1);
    static const half4 PURPLE           = half4(0.5, 0, 0.5, 1);
    static const half4 BROWN            = half4(0.5, 0.25, 0, 1);
    static const half4 TURQUOISE        = half4(0, 0.5, 0.5, 1);

    static const half4 LIGHT_BLUE       = RGB255(154, 206, 223);
    static const half4 LIGHT_GREEN      = RGB255(181, 225, 174);
    static const half4 LIGHT_RED        = RGB255(252, 169, 133);
    static const half4 LIGHT_YELLOW     = RGB255(225, 237, 81);
    static const half4 LIGHT_PINK       = RGB255(253, 222, 238);
    static const half4 LIGHT_CYAN       = RGB255(179, 226, 221);
    static const half4 LIGHT_MAGENTA    = RGB255(249, 140, 182);
    static const half4 LIGHT_ORANGE     = RGB255(253, 138, 138);
    static const half4 LIGHT_PURPLE     = RGB255(221, 212, 232);
    static const half4 LIGHT_TURQUOISE  = RGB255(72,  181, 163);

    static const half4 DARK_BLUE        = half4(0.25, 0.25, 0.5, 1);
    static const half4 DARK_GREEN       = half4(0.25, 0.5, 0.25, 1);
    static const half4 DARK_RED         = half4(0.5, 0.25, 0.25, 1);
    static const half4 DARK_YELLOW      = half4(0.5, 0.5, 0.25, 1);
    static const half4 DARK_PINK        = half4(0.5, 0.375, 0.375, 1);
    static const half4 DARK_CYAN        = half4(0.375, 0.5, 0.5, 1);
    static const half4 DARK_MAGENTA     = half4(.5, 0, .5, 1);
    static const half4 DARK_ORANGE      = half4(half3(255, 140, 0)/255.0, 1);
    static const half4 DARK_PURPLE      = half4(0.375, 0.25, 0.375, 1);
    static const half4 DARK_BROWN       = half4(0.375, 0.3125, 0.1875, 1);
    static const half4 DARK_TURQUOISE   = half4(0.3125, 0.375, 0.35, 1);

    // @formatter off
    namespace palettes {
        static const half4 GRASS = half4(0, .7, .2, .5);
        static const half4 GOLD = half4(1, .7, .2, .5);
        static const half4 TOMATO = half4(1, 0, .2, .5);
    }}
