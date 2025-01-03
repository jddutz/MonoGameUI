using System;

namespace MonoGameUI.Elements;

public enum SizeMode
{
    // The size of the element is fixed. The element will not be resized when the parent is resized.
    FIXED,

    // The element should fill the parent element. Relative Width and Height are ignored.
    FILL_PARENT,

    // Use a fixed size relative to the parent element's size
    // Relative Width and Height are truncated and added to the parent's width or height
    // e.g. Relative Width = -100 or Height=-100 will make the element 100 pixels smaller than its parent
    // Note that a positive value increases the size of the element so it is larger than the parent,
    // and causes unexpected behaviour
    RELATIVE,

    // The element should remain a certain precent of the parent element's size
    // The parent's width or height is multiplied by the relative width or height
    // e.g. width=f0.5 or height=f0.5 will make the element half the size its parent
    // Note that a value greater than 1.0 will make element larger than the parent
    // and causes unexpected behaviour
    PERCENT,

    // The element should maintain an aspect ratio defined by the relative width or height
    // Setting both size modes to ASPECT_RATIO will be treated as FILL_PARENT
    ASPECT_RATIO,

    // Automatically size the element based on its content
    FIT_CONTENT
}