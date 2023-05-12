using System;
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
        const string PATH = "https://flask-analog.up.railway.app/predict";
        public async Task<string> GetClassificationAsync(byte[] image, bool isScan)
        {
            var assembly = GetType().Assembly;

            // model
            using var modelStream = assembly.GetManifestResourceStream("Analog.Resources.analog.onnx");
            using var modelMemoryStream = new MemoryStream();
            modelStream.CopyTo(modelMemoryStream);
            byte[] model = modelMemoryStream.ToArray();
            InferenceSession session = new InferenceSession(model);

            // test image
            //using var imgStream = assembly.GetManifestResourceStream("Analog.Images.img.png");
            //using var imageMemoryStream = new MemoryStream();
            //imgStream.CopyTo(imageMemoryStream);
            //var testImg = imageMemoryStream.ToArray();

            using Image<Rgb24> img = Image.Load<Rgb24>(image, out IImageFormat format);

            //using Stream imageStream = new MemoryStream();
            img.Mutate(x =>
            {
                x.Resize(new ResizeOptions
                {
                    Size = new Size(224, 224),
                    Mode = ResizeMode.Crop
                });
                if (isScan)
                {
                    x.Rotate(RotateMode.Rotate90);
                }
            });
            //img.Save(imageStream, format);

            // Preprocess image 
            Tensor<float> input = new DenseTensor<float>(new[] { 1, 3, 224, 224 });
            //var mean = new[] { 0.485f, 0.456f, 0.406f };
            //var stddev = new[] { 0.229f, 0.224f, 0.225f };
            img.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < accessor.Height; y++)
                {
                    Span<Rgb24> pixelSpan = accessor.GetRowSpan(y);
                    for (int x = 0; x < accessor.Width; x++)
                    {
                        input[0, 0, y, x] = pixelSpan[x].R / 255f;
                        input[0, 1, y, x] = pixelSpan[x].G / 255f;
                        input[0, 2, y, x] = pixelSpan[x].B / 255f;
                    }
                }
            });

            var results = session.Run(new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor("input.1", input) });

            var output = results.FirstOrDefault(i => { Console.WriteLine(i.Name); return i.Name == "495"; }).AsEnumerable<float>();

            int count = 0;

            float max = float.MinValue;

            int maxindex = 0;

            float threeval = 0;

            foreach(var i in output.AsEnumerable<float>())
            {
                if(count == 180)
                {
                    threeval = i;
                }
                Console.WriteLine(i);
                if (i > max)
                {
                    max = i;
                    maxindex = count;
                }
                count++;
            }
            Console.WriteLine(maxindex);
            Console.WriteLine(max);
            Console.WriteLine(threeval);
            Console.WriteLine(count);
            string time = maxindex / 60 + ":" + (maxindex % 60 < 10 ? "0" : "") +  maxindex % 60.0;
                      
            return time + "";
        }

        public async Task<string> GetFromAPIAsync(byte[] image)
        {
            var client = new HttpClient();
            var response = await client.PostAsync(PATH, new ByteArrayContent(image));

            client.Dispose();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
