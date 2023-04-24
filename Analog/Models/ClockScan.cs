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

            using var modelStream_stn = assembly.GetManifestResourceStream("Analog.Resources.analog_stn.onnx");
            using var modelMemoryStream_stn = new MemoryStream();

            modelStream_stn.CopyTo(modelMemoryStream_stn);
            byte[] _model_stn = modelMemoryStream_stn.ToArray();
            InferenceSession _session_stn = new InferenceSession(_model_stn);

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

            // Run inference
            //using IDisposableReadOnlyCollection<DisposableNamedOnnxValue> results = _session.Run(input);

            // Postprocess to get softmax vector
            //IEnumerable<float> output = results.First().AsEnumerable<float>();
            //float sum = output.Sum(x => (float)Math.Exp(x));
            //IEnumerable<float> softmax = output.Select(x => (float)Math.Exp(x) / sum);

            // Extract top 10 predicted classes
            //IEnumerable<Prediction> top10 = softmax.Select((x, i) => new Prediction { Label = LabelMap.Labels[i], Confidence = x })
                               //.OrderByDescending(x => x.Confidence)
                               //.Take(10);

            // Print results to console
           // Console.WriteLine("Top 10 predictions for ResNet50 v2...");
            //Console.WriteLine("--------------------------------------------------------------");
            //foreach (var t in top10)
            //{
                //Console.WriteLine($"Label: {t.Label}, Confidence: {t.Confidence}");
            //}
            // Create Tensor model input
            // The model expects input to be in the shape of (N x 3 x H x W) i.e.
            // mini-batches (where N is the batch size) of 3-channel RGB images with H and W of 224
            // https://onnxruntime.ai/docs/api/csharp-api#systemnumericstensor

            //var input = new DenseTensor<float>(channelData, new[] { DimBatchSize, DimNumberOfChannels, ImageSizeX, ImageSizeY });

            // Run inferencing
            // https://onnxruntime.ai/docs/api/csharp-api#methods-1
            // https://onnxruntime.ai/docs/api/csharp-api#namedonnxvalue

            var results = _session.Run(new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor("input.1", input) });

            // Resolve model output
            // https://github.com/onnx/models/tree/master/vision/classification/mobilenet#output
            // https://onnxruntime.ai/docs/api/csharp-api#disposablenamedonnxvalue

            var output = results.FirstOrDefault(i => i.Name == "495");



            // Postprocess output (get highest score and corresponding label)
            // https://github.com/onnx/models/tree/master/vision/classification/mobilenet#postprocessing

            var scores = output.AsTensor<float>().ToList();
            var highestScore = scores.Max();
            var time = Math.Abs(highestScore) / 0.22375; // returns time as string in format: hours:minutes

            //session.Dispose();

            return time.ToString();
        }
    }
}
