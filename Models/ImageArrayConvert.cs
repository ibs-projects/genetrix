using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace genetrix.Models
{
    public class ImageArrayConvert
    {
        public static Image BytesToImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            MemoryStream ms = new MemoryStream(imageData);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }

        public static byte[] ImageToBytes(Image image)
        {
            try
            {
                ImageConverter imageConverter = new ImageConverter();
                return (Byte[])imageConverter.ConvertTo(image, typeof(Byte[]));
            }
            catch (Exception)
            { }

            return null;
        }
    }
}