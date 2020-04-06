public static class Constants
{
    // From Live
    public const string LiveEqBandAddress = "/eq/band/*";
    public const string LiveTempoAddress = "/live_set/tempo";
    public const string LiveClipNameAddress = "/live_set/tracks/0/clip_slots/*/clip/name";
    public const string ClipColorsAddress = "/live_set/tracks/0/clip_slots/*/clip/color";
    public const string LivePlayingClipPositionAddress = "/live_set/tracks/*/playing_clip/playing_position";
    public const string PlayingClipLengthAddress = "/live_set/tracks/*/playing_clip/length";
    public const string PlayingSlotIndexAddress = "/live_set/tracks/*/playing_clip/name";
    
    // From Unity
    public const string CameraRotateAddress = "/camera_rotate";
    public const string CameraMoveAddress = "/camera_move";
    public const string GuiHueAddress = "/gui_hue";
    
    // From others
    public const string TimeAddress = "/central/time";
}
