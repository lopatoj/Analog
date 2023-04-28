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
using Python.Runtime;
using Numpy;

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
            Tensor<float> input = new DenseTensor<float>(new[] { 1, 224, 3, 224 });
            img.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < accessor.Height; y++)
                {
                    Span<Rgb24> pixelSpan = accessor.GetRowSpan(y);
                    for (int x = 0; x < accessor.Width; x++)
                    {
                        input[0, x, 0, y] = (pixelSpan[x].R / 255f);
                        input[0, x, 1, y] = (pixelSpan[x].G / 255f);
                        input[0, x, 2, y] = (pixelSpan[x].B / 255f);
                    }
                }
            });

            // Run inferencing
            // https://onnxruntime.ai/docs/api/csharp-api#methods-1
            // https://onnxruntime.ai/docs/api/csharp-api#namedonnxvalue

            var results = _session.Run(new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor("input.1", input) });

            // Resolve model output
            // https://github.com/onnx/models/tree/master/vision/classification/mobilenet#output
            // https://onnxruntime.ai/docs/api/csharp-api#disposablenamedonnxvalue

            //var output = results.LastOrDefault(i => i.Name == "495");

            // Postprocess output (get highest score and corresponding label)
            // https://github.com/onnx/models/tree/master/vision/classification/mobilenet#postprocessing

            IEnumerable<float> output = results.First(i => i.Name == "495").AsEnumerable<float>();
            //float sum = output.Sum(x => (float)Math.Exp(x));
            //Console.WriteLine(sum.ToString());
            //IEnumerable<float> softmax = output.Select(x => (float)Math.Exp(x) / sum);
            
            var scores = output.ToList();
            for(int i = 0; i < scores.Count; i++)
            {
                scores[i] = Math.Abs(scores[i]);
                Console.WriteLine(scores[i].ToString());
            }
            _session.Dispose();
            //float newtime = sum * 60;
            //string time = Math.Floor(newtime / 60) + ":" + Math.Round(newtime % 60);

            return scores.Average()+ "";
        }
    }
}
