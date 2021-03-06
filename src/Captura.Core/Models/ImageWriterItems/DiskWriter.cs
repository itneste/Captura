﻿using Captura.ViewModels;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Captura.Models
{
    public class DiskWriter : IImageWriterItem
    {
        DiskWriter() { }

        public static DiskWriter Instance { get; } = new DiskWriter();

        public void Save(Bitmap Image, ImageFormat Format, string FileName, TextLocalizer Status, RecentViewModel Recents)
        {
            try
            {
                Settings.Instance.EnsureOutPath();

                var extension = Format.Equals(ImageFormat.Icon) ? "ico"
                    : Format.Equals(ImageFormat.Jpeg) ? "jpg"
                    : Format.ToString().ToLower();

                var fileName = FileName ?? Path.Combine(Settings.Instance.OutPath, $"{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.{extension}");

                using (Image)
                    Image.Save(fileName, Format);

                Status.LocalizationKey = nameof(LanguageManager.ImgSavedDisk);
                Recents.Add(fileName, RecentItemType.Image, false);

                if (Settings.Instance.CopyOutPathToClipboard)
                    fileName.WriteToClipboard();

                ServiceProvider.SystemTray.ShowScreenShotNotification(fileName);
            }
            catch (Exception E)
            {
                ServiceProvider.MessageProvider.ShowError($"{nameof(LanguageManager.NotSaved)}\n\n{E}");

                Status.LocalizationKey = nameof(LanguageManager.NotSaved);
            }
        }

        public override string ToString() => LanguageManager.Disk;
    }
}
