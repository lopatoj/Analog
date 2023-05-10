﻿using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms.Shapes;

namespace Analog.Models
{
    public class ClockScan
    {
        const string PATH = "https://flask-test-inky.vercel.app/predict";
        public async Task<string> GetClassificationAsync(byte[] image)
        {
            var assembly = GetType().Assembly;

            // model
            using var modelStream = assembly.GetManifestResourceStream("Analog.Resources.analog.onnx");
            using var modelMemoryStream = new MemoryStream();
            modelStream.CopyTo(modelMemoryStream);
            byte[] model = modelMemoryStream.ToArray();
            InferenceSession session = new InferenceSession(model);

            // test image
            using var imgStream = assembly.GetManifestResourceStream("Analog.Images.img.png");
            using var imageMemoryStream = new MemoryStream();
            imgStream.CopyTo(imageMemoryStream);
            var testImg = imageMemoryStream.ToArray();

            using Image<Rgb24> img = Image.Load<Rgb24>(image, out IImageFormat format);

            using Stream imageStream = new MemoryStream();
            img.Mutate(x =>
            {
                x.Resize(new ResizeOptions
                {
                    Size = new Size(224, 224),
                    Mode = ResizeMode.Stretch,
                });
            });
            img.Save(imageStream, format);

            // Preprocess image 
            Tensor<float> input = new DenseTensor<float>(new[] { 1, 3, 224, 224 });
            img.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < accessor.Height; y++)
                {
                    Span<Rgb24> pixelSpan = accessor.GetRowSpan(y);
                    for (int x = 0; x < accessor.Width; x++)
                    {
                        input[0, 0, x, y] = (float)pixelSpan[x].R / 255f;
                        Console.WriteLine((float)pixelSpan[x].R / 255f);
                        input[0, 1, x, y] = (float)pixelSpan[x].G / 255f;
                        input[0, 2, x, y] = (float)pixelSpan[x].B / 255f;
                    }
                }
            });

            var results = session.Run(new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor("input.1", input) });

            var output = results.Last(i => i.Name == "495").AsEnumerable<float>();

            foreach(var i in output)
            {
                Console.WriteLine(i);
            }

            //string time = Math.Floor(output.AsEnumerable().First() / 60) + ":" + Math.Round(output.AsEnumerable().First() % 60);

            return output.First() + "";
        }

        public async Task<string> GetFromAPIAsync(byte[] image)
        {
            HttpClient client = new HttpClient();

            ByteArrayContent content = new ByteArrayContent(image);

            HttpResponseMessage response = await client.GetAsync(PATH);

            client.Dispose();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
