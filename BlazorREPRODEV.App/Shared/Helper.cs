using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BlazorREPRODEV
{
    public static class Helper
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        // private static readonly string GMapAPIKey = "AIzaSyBqVbIjp8LaZUPFt4N4ecBB7yO4y1Yf3b8";

        public static long currentTimeMillis()
        {
            return (long)(DateTime.Now - UnixEpoch).TotalMilliseconds;
        }

        public static bool IsVideoFile(string path)
        {
            string[] mediaExtensions = {
                ".WAV", ".MID", ".MIDI", ".WMA", ".MP3", ".OGG", ".RMA", //etc
                ".AVI", ".MP4", ".DIVX", ".WMV", ".WEBM" //etc
            };
            return mediaExtensions.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase);
        }
        public static bool IsExcelFile(string path)
        {
            string[] mediaExtensions = {
                ".XLS", ".XLSX"//etc
            };
            return mediaExtensions.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase);
        }
        public static bool IsPictureFile(string path)
        {
            string[] mediaExtensions = {
                ".PNG", ".JPG", ".JPEG", ".BMP", ".GIF", //etc
            };
            return mediaExtensions.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase);
        }
        public static bool IsPDFFile(string path)
        {
            string[] mediaExtensions = {
                ".PDF" //etc
            };
            return mediaExtensions.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase);
        }
        //public static DateTime DateTimeFromMillis(long millis)
        //{
        //    return UnixEpoch.AddMilliseconds(millis);
        //}

        //public static long GetCurrentTimeSeconds()
        //{
        //    return (long)(DateTime.Now - UnixEpoch).TotalSeconds;
        //}

        //public static DateTime DateTimeFromSeconds(long seconds)
        //{
        //    return UnixEpoch.AddSeconds(seconds);
        //}

        public static DateTime? ToNullIfTooEarlyForDb(this DateTime date)
        {
            return (date >= (DateTime)SqlDateTime.MinValue) ? date : (DateTime?)null;
        }
        
        public static string toFriendlyDate(DateTime date)
        {
            long then = toUnixTime(date);
            long now = toUnixTime(DateTime.Now);
            long seconds = (now - then) / 1000;
            long minutes = seconds / 60;
            long hours = minutes / 60;
            long days = hours / 24;
            string friendly = "";
            long num = 0;
            if (days > 3)
            {
                return date.ToString("MMM dd, yyyy");
            }
            if (days > 0)
            {
                num = days;
                friendly = days + " day";
            }
            else if (hours > 0)
            {
                num = hours;
                friendly = hours + " hr";
            }
            else if (minutes > 0)
            {
                num = minutes;
                friendly = minutes + " min";
            }
            else
            {
                friendly = "just now";
                return friendly;
            }
            if (num > 1)
            {
                friendly += "s";
            }
            return friendly.ToLower() + " ago";
        }

        public static string GetMd5Hash(string input)
        {
            MD5 md5Hash = MD5.Create();
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            md5Hash.Dispose();
            return sBuilder.ToString();
        }
        public static int HashMD5toNumber(string s)
        {
            s = GetMd5Hash(s);
            MD5 md5hasher = MD5.Create();
            var hashed = md5hasher.ComputeHash(Encoding.UTF8.GetBytes(s));
            var ivalue = BitConverter.ToInt32(hashed, 3);
            return ivalue;
        }
        public static string toPesoSign(decimal value)
        {
            return "₱" + string.Format("{0:n2}", trimDecimal((double)value, 2));
        }
        public static string toPesoSign(double value)
        {
            return "₱" + string.Format("{0:n2}", trimDecimal(value, 2));
        }
        public static string toPesoSign(double? value)
        {
            return value.HasValue ? "₱" + string.Format("{0:n2}", trimDecimal(value.Value, 2)) : "₱0.00";
        }
        public static string toCommaSeparate(double value)
        {
            return string.Format("{0:n0}", trimDecimal(value, 0));
        }
        static bool VerifyMd5Hash(string input, string hash)
        {
            string hashOfInput = GetMd5Hash(input);
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private static int exifOrientationID = 0x112;
        public static Image fixRotation(Image img)
        {
            
            if (!img.PropertyIdList.Contains(exifOrientationID))
                return img;

            var prop = img.GetPropertyItem(exifOrientationID);
            int val = BitConverter.ToUInt16(prop.Value, 0);
            var rot = RotateFlipType.RotateNoneFlipNone;

            if (val == 3 || val == 4)
                rot = RotateFlipType.Rotate180FlipNone;
            else if (val == 5 || val == 6)
                rot = RotateFlipType.Rotate90FlipNone;
            else if (val == 7 || val == 8)
                rot = RotateFlipType.Rotate270FlipNone;

            if (val == 2 || val == 4 || val == 5 || val == 7)
                rot |= RotateFlipType.RotateNoneFlipX;

            if (rot != RotateFlipType.RotateNoneFlipNone)
                img.RotateFlip(rot);
            return img;
        }
        public static byte[] fixRotationFromByteArray(byte[] byteArray)
        {
            var img = ByteArrayToImage(byteArray);
            if (!img.PropertyIdList.Contains(exifOrientationID))
                return ImageToByteArray(img);

            var prop = img.GetPropertyItem(exifOrientationID);
            int val = BitConverter.ToUInt16(prop.Value, 0);
            var rot = RotateFlipType.RotateNoneFlipNone;

            if (val == 3 || val == 4)
                rot = RotateFlipType.Rotate180FlipNone;
            else if (val == 5 || val == 6)
                rot = RotateFlipType.Rotate90FlipNone;
            else if (val == 7 || val == 8)
                rot = RotateFlipType.Rotate270FlipNone;

            if (val == 2 || val == 4 || val == 5 || val == 7)
                rot |= RotateFlipType.RotateNoneFlipX;

            if (rot != RotateFlipType.RotateNoneFlipNone)
                img.RotateFlip(rot);
            return ImageToByteArray(img);
        }
        public static Image ResizeImage(Image img, int maxWidth, int maxHeight)
        {
            if (img.Height < maxHeight && img.Width < maxWidth) return img;
            using (img)
            {
                var xRatio = (double)img.Width / maxWidth;
                var yRatio = (double)img.Height / maxHeight;
                var ratio = Math.Max(xRatio, yRatio);
                var nnx = (int)Math.Floor(img.Width / ratio);
                var nny = (int)Math.Floor(img.Height / ratio);
                var cpy = new Bitmap(nnx, nny, PixelFormat.Format32bppArgb);
                using (var gr = Graphics.FromImage(cpy))
                {
                    gr.Clear(Color.Transparent);
                    gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    gr.DrawImage(img,
                        new Rectangle(0, 0, nnx, nny),
                        new Rectangle(0, 0, img.Width, img.Height),
                        GraphicsUnit.Pixel);
                }
                return cpy;
            }

        }
        public static byte[] ResizeImageFromByteArray(byte[] byteArray, int maxWidth, int maxHeight)
        {
            var img = ByteArrayToImage(byteArray);
            if (img.Height < maxHeight && img.Width < maxWidth) return ImageToByteArray(img);
            using (img)
            {
                var xRatio = (double)img.Width / maxWidth;
                var yRatio = (double)img.Height / maxHeight;
                var ratio = Math.Max(xRatio, yRatio);
                var nnx = (int)Math.Floor(img.Width / ratio);
                var nny = (int)Math.Floor(img.Height / ratio);
                var cpy = new Bitmap(nnx, nny, PixelFormat.Format32bppArgb);
                using (var gr = Graphics.FromImage(cpy))
                {
                    gr.Clear(Color.Transparent);
                    gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    gr.DrawImage(img,
                        new Rectangle(0, 0, nnx, nny),
                        new Rectangle(0, 0, img.Width, img.Height),
                        GraphicsUnit.Pixel);
                }
                return ImageToByteArray(cpy);
            }

        }
        public static Image ByteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }
        public static void SaveJpeg(string path, Image img)
        {
            if (System.IO.File.Exists(path))
            {
                var fileLength = new FileInfo(path).Length;
                do
                {
                    EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 99);
                    ImageCodecInfo jpegCodec = GetEncoderInfo("image/jpeg");
                    EncoderParameters encoderParams = new EncoderParameters(1);
                    encoderParams.Param[0] = qualityParam;
                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);
                    img.Save(path, jpegCodec, encoderParams);
                    fileLength = new FileInfo(path).Length;
                } while (fileLength > 1048576);

            }
        }

        /// <summary> 
        /// Returns the image codec with the given mime type 
        /// </summary> 
        public static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            // Get image codecs for all image formats 
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            // Find the correct image codec 
            for (int i = 0; i < codecs.Length; i++)
                if (codecs[i].MimeType == mimeType)
                    return codecs[i];

            return null;
        }
        /*public static byte[] toQRCodeImage(string str, int pixel, Color color)
        {
            //str = getHashString(str);
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(str, QRCodeGenerator.ECCLevel.H);

            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(pixel, color, Color.White, true);
            return toByteArray(qrCodeImage);
        }*/

        public static byte[] toByteArray(System.Drawing.Image img)
        {
            MemoryStream ms = new MemoryStream();
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            return ms.ToArray();
        }
        public static DateTime toDateTime(long unixTimeStamp)
        {
            //System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            //var dtDateTime = UnixEpoch.AddMilliseconds(unixTimeStamp).ToLocalTime();
            return UnixEpoch.AddMilliseconds(unixTimeStamp);
        }
        public static long toUnixTime(DateTime d)
        {
            //Int32 unixTimestamp = (Int32)(d.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            //return (long)(d - UnixEpoch).TotalMilliseconds;
            return (long)(d - UnixEpoch).TotalMilliseconds;
        }
        //public static long currentTimeMillis()
        //{
        //    return DateTimeOffset.Now.ToUnixTimeMilliseconds();
        //}
        public static byte[] getHash(string inputString)
        {
            HashAlgorithm algorithm = SHA256.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        public static string getHashString(string inputString)
        {
            if (string.IsNullOrEmpty(inputString))
            {
                return "";
            }
            StringBuilder sb = new StringBuilder();
            foreach (byte b in getHash(inputString))
                sb.Append(b.ToString("X2"));
            return sb.ToString().ToLower();
        }
        //public static List<ModelReport> GetPage(List<ModelReport> list, int page, int pageSize)
        //{
        //    return list.Skip(page * pageSize).Take(pageSize).ToList();
        //}
        //public static List<ModelProject> GetPage(List<ModelProject> list, int page, int pageSize)
        //{
        //    return list.Skip(page * pageSize).Take(pageSize).ToList();
        //}
        public class CollectorRank
        {
            public double percent { get; set; }
            public string fullname { get; set; }
        }

        public static string StringEncrypt(string clearText)
        {
            string EncryptionKey = "abc123";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                    ms.Dispose();
                    ms.Close();
                }
            }
            return clearText;
        }
        public static string StringDecrypt(string encryptedText)
        {
            string EncryptionKey = "abc123";
            encryptedText = encryptedText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(encryptedText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    encryptedText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return encryptedText;
        }


        public static string FirstCharToUpper(this string input)
        {
            switch (input)
            {
                case null: throw new ArgumentNullException(nameof(input));
                case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
                default: return input.First().ToString().ToUpper() + input.Substring(1);
            }
        }

        private static int Rand_gen(string x)
        {
            Random rnd = new Random();
            int random_num = 0;
            if (x == "even")
            {
                do
                {
                    random_num = rnd.Next(10);
                } while (random_num % 2 == 1);
            }
            else if (x == "odd")
            {
                do
                {
                    random_num = rnd.Next(10);
                } while (random_num % 2 == 0);
            }
            return random_num;
        }
        public static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
        public static byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            using var ms = new MemoryStream();
            imageIn.Save(ms, ImageFormat.Jpeg);
            return ms.ToArray();
        }
        public static string GenerateCode(int count)
        {
            string rs = string.Empty;
            string y = DateTime.Now.Year.ToString();
            string f = string.Format("{0:D4}", count);
            rs = $"{y}-{f}";
            return rs;
        }
        
        [Inject]
        public static NavigationManager nav { get; set; }
        
        public static string ToCombinedFileName(int Id, DateTime? DateOfDocs)
        {
            return $"{Id}_{(DateOfDocs.HasValue ? toUnixTime(DateOfDocs.Value) : 0)}_{ currentTimeMillis() }";
        }
        public static string[] GetDateFromImage(string root, string p, int ProjectId)
        {
            try
            {
                var dir = Path.Combine(new string[] { root, "user_files", p, ProjectId.ToString() });
                if (Directory.Exists(dir) && Directory.GetFiles(dir).Length > 0)
                {
                    var f = GetLastImageFromDIR(dir);
                    var str = f.Split('_');
                    var d1 = str[1] != "0" ? toDateTime(long.Parse(str[1])).ToString("MM-dd-yyyy") : string.Empty;
                    var a = new string[] { d1, toFriendlyDate(toDateTime(long.Parse(str[2]))) };
                    return a;
                }
            }
            catch (Exception) { }
            return new string[] { string.Empty, string.Empty };
        }
        public static string[] GetDateFromImage(string root, string p1, string p2, int ProjectId)
        {
            try
            {
                var dir = Path.Combine(new string[] { root, "user_files", p1, p2, ProjectId.ToString() });
                if (Directory.Exists(dir) && Directory.GetFiles(dir).Length > 0)
                {
                    var f = GetLastImageFromDIR(dir);
                    var str = f.Split('_');
                    var d1 = str[1] != "0" ? toDateTime(long.Parse(str[1])).ToString("MM-dd-yyyy") : string.Empty;
                    var a = new string[] { d1, toFriendlyDate(toDateTime(long.Parse(str[2]))) };
                    return a;
                }
            }
            catch (Exception) { }
            return new string[] { string.Empty, string.Empty };
        }
        public static string[] GetDateFromImage(string root, string p1, string p2, string p3, int ProjectId)
        {
            try
            {
                var dir = Path.Combine(new string[] { root, "user_files", p1, p2, p3, ProjectId.ToString() });
                if (Directory.Exists(dir) && Directory.GetFiles(dir).Length > 0)
                {
                    var f = GetLastImageFromDIR(dir);
                    var str = f.Split('_');
                    var d1 = str[1] != "0" ? toDateTime(long.Parse(str[1])).ToString("MM-dd-yyyy") : string.Empty;
                    var a = new string[] { d1, toFriendlyDate(toDateTime(long.Parse(str[2]))) };
                    return a;
                }
            }
            catch (Exception) { }
            return new string[] { string.Empty, string.Empty };
        }
        public static string GetLastImageFromDIR(string path)
        {
            var dir = Directory.GetFiles(path).OrderByDescending(x=>File.GetCreationTime(x)).ToArray();
            return dir.Length > 0 ? Path.GetFileNameWithoutExtension(dir[0]) : string.Empty;
        }
        
        public static void CleanSystem(string root)
        {
            string rootdir = Path.Combine(new string[] { root, "user_files"});
            foreach (string dir in Directory.GetDirectories(rootdir))
            {
                foreach (var d in Directory.GetDirectories(dir))
                {
                    var files = Directory.GetFiles(d);
                    foreach (var f in files)
                    {
                        if (File.Exists(f))
                            File.Delete(f);
                    }
                    /*if (Directory.Exists(d))
                        Directory.Delete(d);*/
                }
            }
            
        }
        public static double trimDecimal(double number, int zero)
        {
            return Math.Round(number, zero);
        }

        public static int GetAge(DateTime birthday)
        {
            return (DateTime.Now.Year - birthday.Year);
        }

        private readonly static Random _random = new Random();

        // Generates a random number within a range.      
        public static int RandomNumber(int min, int max)
        {
            return _random.Next(min, max);
        }

        public static void toClipboard(this string str)
        {
            TextCopy.ClipboardService.SetText(str);
        }

        static readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        public static string SizeSuffix(Int64 value, int decimalPlaces = 1)
        {
            if (decimalPlaces < 0) { throw new ArgumentOutOfRangeException("decimalPlaces"); }
            if (value < 0) { return "-" + SizeSuffix(-value, decimalPlaces); }
            if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }

            int mag = (int)Math.Log(value, 1024);
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}",
                adjustedSize,
                SizeSuffixes[mag]);
        }

    }

}
