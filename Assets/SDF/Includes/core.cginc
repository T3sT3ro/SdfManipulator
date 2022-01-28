#pragma once
#pragma target 5.0

/**
 * This file contains macros used in writing shader templates with support of IDE linter
 */

/** mark start of template */
#define _BODY

/** All keys are collected and they can be set from inside the inspector */
#define _REF(key, default) default

/** Registers a template input. Use as <br><c>_IN(float p)</c> or similar*/
#define _IN

/** Registers a template output */
#define _OUT