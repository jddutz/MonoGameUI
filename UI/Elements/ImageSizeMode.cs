namespace MonoGameUI.Elements;

public enum ImageSizeMode
{
    // Center the image, do not scale, crop parts of the image outside the target rect
    CENTER,
    // Stretch the image to fit
    STRETCH,
    // Scale the image to fit inside the target, maintaining aspect ratio
    BEST_FIT,
    // Scale the image to fill horizontally, potentially cropping top and bottom edges
    FIT_HORIZONTAL,
    // Scale the image to fill vertically, potentially cropping left and right edges
    FIT_VERTICAL,
    // Tile the image to fill the target rect, starting in the top-left corner
    TILE,
    // Tile the image to fill the target rect, centering a tile in the center of the target rect
    TILE_CENTER,
    // Make the image a nine-patch, stretching the center and tiling the edges
    // Uses the element's Borders property to determine the nine-patch borders
    NINE_PATCH
}