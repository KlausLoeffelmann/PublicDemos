namespace Northwind.App
{
    internal static class ImageFactory
    {
        private static readonly Font s_iconFont = new("Segoe Fluent Icons", 20, FontStyle.Regular, GraphicsUnit.Pixel);

        public static Image CreateAddIcon(Size size, int padding) => CreateIcon("\uE710", size, padding);

        public static Image CreateEditIcon(Size size, int padding) => CreateIcon("\uE70F", size, padding);

        public static Image CreateCancelIcon(Size size, int padding) => CreateIcon("\uE711", size, padding);

        public static Image CreateSaveIcon(Size size, int padding) => CreateIcon("\uE74E", size, padding);

        private static Image CreateIcon(string glyph, Size size, int padding)
        {
            var bitmap = new Bitmap(size.Width, size.Height);
            using var graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.Transparent);
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

            var rect = new Rectangle(padding, padding, size.Width - padding * 2, size.Height - padding * 2);
            using var brush = new SolidBrush(Color.DimGray);
            using var format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            graphics.DrawString(glyph, s_iconFont, brush, rect, format);
            return bitmap;
        }
    }
}
