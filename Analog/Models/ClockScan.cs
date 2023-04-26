using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Advanced;
using System.Collections.Generic;
using System.Linq;
using Emgu.CV;

namespace Analog.Models
{
    public class ClockScan
    {
        public async Task<string> GetClassificationAsync(byte[] image)
        {
            var assembly = GetType().Assembly;
            using var modelStream = assembly.GetManifestResourceStream("Analog.Resources.analog.onnx");
            using var modelMemoryStream = new MemoryStream();

            modelStream.CopyTo(modelMemoryStream);
            byte[] _model = modelMemoryStream.ToArray();
            InferenceSession _session = new InferenceSession(_model);

            Mat _img = new Mat();

            CvInvoke.Imdecode(image, Emgu.CV.CvEnum.ImreadModes.Unchanged, _img);
            CvInvoke.Resize(_img, _img / 255, new System.Drawing.Size(224, 224), 0, 0);

            using Image<Rgb24> img = Image.Load<Rgb24>(image, out IImageFormat format);

            using Stream imageStream = new MemoryStream();
            img.Mutate(x =>
            {
                x.Resize(new ResizeOptions
                {
                    Size = new Size(224, 224),
                    Mode = ResizeMode.Crop
                });
            });
            img.Save(imageStream, format);

            // Preprocess image
            Tensor<float> input = new DenseTensor<float>(new[] { 1, 3, 224, 224 });
            var mean = new[] { 0.485f, 0.456f, 0.406f };
            var stddev = new[] { 0.229f, 0.224f, 0.225f };
            img.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < accessor.Height; y++)
                {
                    Span<Rgb24> pixelSpan = accessor.GetRowSpan(y);
                    for (int x = 0; x < accessor.Width; x++)
                    {
                        input[0, 0, y, x] = ((pixelSpan[x].R / 255f) - mean[0]) / stddev[0];
                        input[0, 1, y, x] = ((pixelSpan[x].G / 255f) - mean[1]) / stddev[1];
                        input[0, 2, y, x] = ((pixelSpan[x].B / 255f) - mean[2]) / stddev[2];

                    }
                }
            });

            var results = _session.Run(new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor("input.1", input) });

            var output = results.FirstOrDefault(i => i.Name == "495");

            var scores = output.AsTensor<float>().ToList();
            var highestScore = scores[0];
            var time = scores.ToString(); // returns time as string in format: hours:minutes

            _session.Dispose();

            return time;
        }
    }
}
