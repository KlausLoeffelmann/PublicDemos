namespace WarpClock.App;

public partial class FoldersOptionsView : UserControl
{
    public FoldersOptionsView()
    {
        InitializeComponent();

        AutoScroll = true;
        AutoScrollMinSize = Size;
        _themesFolderPicker.AccessibleName = "Themes folder";
        _calendarFolderPicker.AccessibleName = "Calendar folder";
        _shortMessagesFolderPicker.AccessibleName = "Short messages folder";
        _picturesFolderPicker.AccessibleName = "Pictures folder";
    }

    public void LoadFrom(FolderOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _themesFolderPicker.FileOrFolderPath = options.ThemesFolder;
        _calendarFolderPicker.FileOrFolderPath = options.CalendarFolder;
        _shortMessagesFolderPicker.FileOrFolderPath = options.ShortMessagesFolder;
        _picturesFolderPicker.FileOrFolderPath = options.PicturesFolder;
    }

    public FolderOptions CreateOptions()
    {
        var options = new FolderOptions
        {
            ThemesFolder = _themesFolderPicker.FileOrFolderPath,
            CalendarFolder = _calendarFolderPicker.FileOrFolderPath,
            ShortMessagesFolder = _shortMessagesFolderPicker.FileOrFolderPath,
            PicturesFolder = _picturesFolderPicker.FileOrFolderPath,
        };

        options.Normalize();
        return options;
    }
}
