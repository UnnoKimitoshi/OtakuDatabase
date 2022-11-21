using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Otaku_Database.Models
{
    enum Category { None, Comic, Voice, Video }
    public class ItemFactory
    {
        private static int OUT_IMG_W = 120;
        private static int OUT_IMG_H = 160;
      
        private static string[] V_EXTS = { ".mp4", ".wmv" };
        private static string[] M_EXTS = { ".flac", ".mp3", ".wav" };
        private static string[] I_EXTS = { ".jpeg", ".jpg", ".png" };

        public static Item? MakeFromPath(string path)
        {
            Category category = Category.None;
            string imgSrc = "";

            var ret = new Item();
            ret.Title = Path.GetFileName(path);
            ret.ItemPath = path;

            if (Directory.Exists(path))
            {
                string[] filePaths = Directory.GetFiles(path, "*", SearchOption.AllDirectories);

                foreach (var filePath in filePaths)
                {
                    var extension = Path.GetExtension(filePath);

                    if (V_EXTS.Contains(extension))
                    {
                        category = Category.Video;
                        //imgSrc = filePath;
                        if (imgSrc != "")
                            break;
                    }
                    else if ((int)category < 2 && M_EXTS.Contains(extension))
                    {
                        category = Category.Voice;
                        if (imgSrc != "")
                            break;
                    }
                    else if (imgSrc == "" && I_EXTS.Contains(extension))
                    {
                        imgSrc = filePath;
                        if ((int)category == 3)
                            break;
                        if ((int)category == 0)
                        {
                            category = Category.Comic;
                        }
                    }
                }

                if (category == Category.None) return null;


                switch (category)
                {
                    case Category.Video:
                        //ret.ImgBytes = VideoToImgBtyes(imgSrc);
                        break;
                    case Category.Voice:
                    case Category.Comic:
                        ret.ImgBytes = ImgToImgBytes(imgSrc);
                        break;
                }

                ret.Category = category.ToString();
            }
            else if (File.Exists(path))
            {
                return null;
            }
            else
            {
                MessageBox.Show($"{path}が存在しません");
                return null;
            }


            return ret;
        }

        private static byte[]? ImgToImgBytes(string imgSrc)
        {
            if (imgSrc == "") return null;
            var uri = new Uri(imgSrc);
            var bmImage = new BitmapImage(uri);
            // 元画像の幅
            int width = bmImage.PixelWidth;
            // 元画像の高さ
            int height = bmImage.PixelHeight;
            
            TransformedBitmap transImg;
            if (width > height)
            {
                var scale = (double)OUT_IMG_H / height;
                var scaleTrans = new ScaleTransform(scale, scale);
                transImg = new TransformedBitmap(bmImage, scaleTrans);
           
            }
            else
            {
                var scale = (double)OUT_IMG_W / width;
                var scaleTrans = new ScaleTransform(scale, scale);
                transImg = new TransformedBitmap(bmImage, scaleTrans);
             
            }

            using (var ms = new MemoryStream())
            {
                var encorder = new PngBitmapEncoder();
                encorder.Frames.Add(BitmapFrame.Create(transImg));
                encorder.Save(ms);
                ms.Seek(0, SeekOrigin.Begin);
                return ms.GetBuffer();
            }
        }

        // 未使用
        //private static byte[]? VideoToImgBtyes(string imgSrc)
        //{
        //    // ffmpegのパス
        //    //const string ffmpegPath = @"C:\FFmpeg\ffmpeg.exe";
        //    string ffmpegPath = Properties.Settings.Default.FfmpegPath;
        //    if (string.IsNullOrEmpty(imgSrc)) return null;

        //    // 抜き出したいフレーム(秒)
        //    var time = "10";
        //    // リサイズしたいサイズ
        //    var scaleSize = $"-1:{OUT_IMG_H}";
        //    // 切り抜きたいサイズ
        //    var cropSize = $"{OUT_IMG_W}:{OUT_IMG_H}";
        //    // ffmpegに送る文字列
        //    var argument = $"-ss {time} -i \"{imgSrc}\" -vf scale={scaleSize},crop={cropSize} -f image2 pipe:1";


        //    try
        //    {
        //        using (var process = new Process())
        //        {
        //            process.StartInfo = new ProcessStartInfo
        //            {
        //                FileName = ffmpegPath,
        //                Arguments = argument,
        //                CreateNoWindow = true,
        //                RedirectStandardOutput = true,
        //            };
        //            // stream出力
        //            process.Start();
        //            return ToBinary(process.StandardOutput.BaseStream);
        //        }

        //        byte[] ToBinary(Stream stream)
        //        {
        //            using (stream)
        //            {
        //                byte[] buf = new byte[32768];
        //                using (MemoryStream ms = new MemoryStream())
        //                {
        //                    while (true)
        //                    {
        //                        int read = stream.Read(buf, 0, buf.Length);

        //                        if (read > 0)
        //                        {
        //                            ms.Write(buf, 0, read);
        //                        }
        //                        else
        //                        {
        //                            break;
        //                        }
        //                    }
        //                    return ms.ToArray();
        //                }
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}

        public static async Task<Item>? MakeFromUrlAsync(string Url)
        {

            // Urlチェック
            Regex reg = new Regex(@"https?://www\.dlsite\.com/[^\s]+/RJ\d{6}.html");
            Match urlMatch = reg.Match(Url);

            if (!urlMatch.Success)
            {
                MessageBox.Show("対応したURLではありません。");
                return null;
            }

            var ret = new Item();

            string html;

            using (var client = new HttpClient())
            {
                html = await client.GetStringAsync(Url);
            }

            // イメージ
            var image = ExtractStr(html, @"<meta property=""og:image"" content=""(?<img>.*?)"">");
            // タイトルとサークル
            var titleAndCircle = ExtractStr(html, @"<meta property=""og:title"" content=""(?<title>.*?)\[(?<circle>.*?)\] | DLsite"">");
            // 声優群
            var voices = ExtractStr(html, @"<th>声優</th>\s*<td>(?<vas>.*?)</td>");
            // 声優
            var voice = ExtractStrs(voices.Groups["vas"].Value, @">(?<va>.*?)</a>");
            // タグ群
            var tagsMatche = ExtractStr(html, @"<div class=""main_genre"">(?<tags>.*?)</div>");
            // タグ
            var tagMatches = ExtractStrs(tagsMatche.Groups["tags"].Value, ">(?<tag>.*?)</a>");

            // 得られた情報をViewModelに適応
            ret.ImgBytes = await UrlToImageAsync(image.Groups["img"].Value);

            // サークルをタグに追加
            ret.Tags += "サークル：" + titleAndCircle.Groups["circle"].Value + ";";
            // 声優をタグに追加
            foreach (Match match in voice)
            {
                ret.Tags += "声優：" + match.Groups["va"].Value + ";";
            }
            foreach (Match match in tagMatches)
            {
                ret.Tags += match.Groups["tag"].Value + ";";
            }

            return ret;
        }

        private static Match ExtractStr(string input, string pattern)
        {
            return Regex.Match(input, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        }

        private static MatchCollection ExtractStrs(string input, string pattern)
        {
            return Regex.Matches(input, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        }

        private static async Task<byte[]> UrlToImageAsync(string url)
        {
            BitmapImage bmpImg;
            TransformedBitmap transImg;
            // 画像をダウンロード
            using (var client = new HttpClient())
            {
                var bytes = await client.GetByteArrayAsync(url).ConfigureAwait(false);

                using (var ms = new MemoryStream(bytes))
                {
                    bmpImg = new BitmapImage();
                    bmpImg.BeginInit();
                    bmpImg.StreamSource = ms;
                    bmpImg.CacheOption = BitmapCacheOption.OnLoad;
                    bmpImg.EndInit();
                    bmpImg.Freeze();
                }
            }

            // リサイズ
            var srcWidth = bmpImg.Width;
            var srcHeight = bmpImg.Height;

            if (srcWidth >= srcHeight)
            {
                var scale = (double)OUT_IMG_H / srcHeight;
                var scaleTrans = new ScaleTransform(scale, scale);
                // BitmapImageをリサイズ
                transImg = new TransformedBitmap(bmpImg, scaleTrans);
            }
            else
            {
                var scale = (double)OUT_IMG_W / srcWidth;
                var scaleTrans = new ScaleTransform(scale, scale);
                // BitmapImageをリサイズ
                transImg = new TransformedBitmap(bmpImg, scaleTrans);
            }

            using (var ms = new MemoryStream())
            {
                var encorder = new PngBitmapEncoder();
                encorder.Frames.Add(BitmapFrame.Create(transImg));
                encorder.Save(ms);
                ms.Seek(0, SeekOrigin.Begin);
                return ms.GetBuffer();
            }
        }
    }
}
