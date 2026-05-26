namespace WinBaas.Services;

/// <inheritdoc cref="IFileTypeMap"/>
public sealed class FileTypeMap : IFileTypeMap
{
    private static readonly Dictionary<string, string> s_map = new(StringComparer.OrdinalIgnoreCase)
    {
        [".sln"] = "VS Solution",
        [".slnx"] = "VS V17+ Solution",
        [".cs"] = "C# Code File",
        [".vb"] = "VB Code File",
        [".csproj"] = "C# Project",
        [".vbproj"] = "VB Project",
        [".md"] = "Markdown File",
        [".txt"] = "Text File",
        [".rtf"] = "Rich Text",
        [".jpeg"] = "JPEG Image",
        [".jpg"] = "JPEG Image",
        [".png"] = "PNG Image",
        [".gif"] = "GIF Image",
        [".bmp"] = "BMP Image",
        [".heic"] = "HEIF Image",
        [".heif"] = "HEIF Image",
        [".mp4"] = "Video File",
        [".mov"] = "Video File",
        [".avi"] = "Video File",
        [".webm"] = "Video File",
        [".mkv"] = "Video File",
        [".wav"] = "Audio File",
        [".m4a"] = "Audio File",
        [".mp3"] = "Audio File",
        [".json"] = "Configuration File",
        [".xml"] = "Configuration File",
        [".config"] = "Configuration File",
        [".ini"] = "Configuration File",
        [".toml"] = "Configuration File",
        [".yaml"] = "Configuration File",
        [".yml"] = "Configuration File",
        [".url"] = "Edge Favorite",
        [".camproj"] = "Camtasia Recording",
        [".camrec"] = "Camtasia Recording",
        [".trec"] = "Camtasia Recording",
        [".lua"] = "Lua Script",
        [".ps1"] = "PowerShell Script",
        [".cmd"] = "Batch Script",
        [".bat"] = "Batch Script",
        [".sql"] = "SQL Script",
        [".bak"] = "SQL Backup",
        [".mdf"] = "SQL Data File",
        [".ldf"] = "SQL Log File",
        [".pdf"] = "PDF Document",
        [".doc"] = "Word Document",
        [".docx"] = "Word Document",
        [".xls"] = "Excel Workbook",
        [".xlsx"] = "Excel Workbook",
        [".ppt"] = "PowerPoint Presentation",
        [".pptx"] = "PowerPoint Presentation",
        [".zip"] = "ZIP Archive",
        [".7z"] = "7-Zip Archive",
        [".rar"] = "RAR Archive",
    };

    /// <inheritdoc />
    public string GetLabel(string path)
    {
        string ext = System.IO.Path.GetExtension(path);
        if (string.IsNullOrEmpty(ext))
        {
            return string.Empty;
        }

        return s_map.TryGetValue(ext, out string? label)
            ? label
            : ext.TrimStart('.').ToUpperInvariant() + " File";
    }
}
