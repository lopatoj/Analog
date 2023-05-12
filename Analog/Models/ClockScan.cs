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
        public async Task<string> GetClassificationAsync(byte[] image, bool isScan)
        {
            var assembly = GetType().Assembly;

            // Initialize the model
            using var modelStream = assembly.GetManifestResourceStream("Analog.Resources.analog.onnx");
            using var modelMemoryStream = new MemoryStream();
            modelStream.CopyTo(modelMemoryStream);
            byte[] model = modelMemoryStream.ToArray();
            InferenceSession session = new InferenceSession(model);

            using Image<Rgb24> img = Image.Load<Rgb24>(image, out IImageFormat format);

            // Image preprocessing
            img.Mutate(x =>
            {
                // Size must be initialized to 224 by 244 for the model to read the image correctly
                x.Resize(new ResizeOptions
                {
                    Size = new Size(224, 224),
                    Mode = ResizeMode.Crop
                });

                // Using the cameraview output rotates the image 90 degrees for some reason, this corrects this issue
                if (isScan)
                {
                    x.Rotate(RotateMode.Rotate90);
                }
            });

            // Preprocess image 
            Tensor<float> input = new DenseTensor<float>(new[] { 1, 3, 224, 224 });

            img.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < accessor.Height; y++)
                {
                    Span<Rgb24> pixelSpan = accessor.GetRowSpan(y);
                    // Normalize the RGB values from 0 to 1
                    for (int x = 0; x < accessor.Width; x++)
                    {
                        input[0, 0, y, x] = pixelSpan[x].R / 255f;
                        input[0, 1, y, x] = pixelSpan[x].G / 255f;
                        input[0, 2, y, x] = pixelSpan[x].B / 255f;
                    }
                }
            });

            // Model is run, and we take the first output
            var results = session.Run(new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor("input.1", input) });

            var output = results.FirstOrDefault(i => { return i.Name == "495"; }).AsEnumerable<float>();

            // Time is the index of the largest output
            int count = 0;

            float max = float.MinValue;

            int maxindex = 0;

            foreach(var i in output.AsEnumerable<float>())
            {
                if (i > max)
                {
                    max = i;
                    maxindex = count;
                }
                count++;
            }

            // Ensures our resulting time is formated in hh:mm format
            string time = maxindex / 60 + ":" + (maxindex % 60 < 10 ? "0" : "") +  maxindex % 60.0;
                      
            return "The time is " + time;
        }
    }
}
