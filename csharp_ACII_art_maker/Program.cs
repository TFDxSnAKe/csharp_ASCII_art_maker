using System;
using System.Drawing;
using System.Runtime.InteropServices.Marshalling;

namespace ascii
{
    class Program
    {
        static char[] gradient_1 = ['@', '%', '#', '*', '=', '-', '.', '.', ' ', ' ', ' '];
        static char[] gradient_2 = [' ', ' ', ' ', '.', '.', '-', '=', '*', '#', '%', '@'];

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Please input the full file path for your image");
                Console.WriteLine("Press [Q] quit");
                string input = Console.ReadLine();
                if (input == "Q" || input == "q")
                {
                    break;
                }
                input = new string((from c in input
                                    where !(c == '"')
                                    select c
                                   ).ToArray());
                MakeASCII(input);
            }
        }

        static void MakeASCII(string path)
        {
            if (!File.Exists(path))
            {
                Console.WriteLine("Err: Filepath is invalid");
            }
            else
            {
                Image image = Image.FromFile(path);

                Bitmap bitMap = ScaleImage(image, 130, 130);

                char[,] pixels = new char[bitMap.Width, bitMap.Height];
                for (int i = 0; i < bitMap.Width; i++)
                {
                    for (int j = 0; j < bitMap.Height; j++)
                    {
                        Color pixel = bitMap.GetPixel(i, j);
                        float greyScale = pixel.GetBrightness(); // a val between 0.0 and 1.0
                        pixels[i, j] = DetermineChar(greyScale);
                    }
                }
                // let the printing begin
                PrintASCII(pixels);
            }
        }

        static void PrintASCII(char[,] chars)
        {
            int x = 0;
            int y = 0;
            for (int j = 0; j < chars.GetLength(1); j++)
            {
                Console.Write("||  ");
                for (int i = 0; i < chars.GetLength(0); i++)
                {
                    Console.Write(chars[i, j]);
                    x = i; y = j;
                }
                Console.Write("  ||\n");
            }
            MakeFrame(chars.GetLength(0) + 4);
            // for debugging purposes
            Console.WriteLine($"\nAmount of i loops: {x}");
            Console.WriteLine($"Amount of j loops: {y}");
        }

        // make a beautiful frame around the generated ASCII art
        static void MakeFrame(int x)
        {
            Console.Write("||");
            RepeatedWriteChar(x, ' ');
            Console.Write("||\n");
            Console.Write("  ");
            RepeatedWriteChar(x, '=');
        }

        static void RepeatedWriteChar(int n, char c)
        {
            while (n > 0)
            {
                Console.Write(c);
                n--;
            }
        }

        static char DetermineChar(float f)
        {
            // 0.0 is black, 1.0 is white
            // use more filled characters for small f values to get black
            // the opposite for larger f values
            float index = 10f * (float)Math.Round(f, 2);
            return gradient_2[(int)index]; 
        }

        /// <summary>
        /// This is a function for scaling a bitmap to a certain width and height.
        /// Credit goes to Ryan Bosinger, https://gist.github.com/Boztown/8060706
        /// </summary>
        /// <param name="image"></param>
        /// <param name="maxWidth"></param>
        /// <param name="maxHeight"></param>
        /// <returns></returns>
        static public Bitmap ScaleImage(Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);
            Graphics.FromImage(newImage).DrawImage(image, 0, 0, newWidth, newHeight);
            Bitmap bmp = new Bitmap(newImage);

            return bmp;
        }
    }
}
