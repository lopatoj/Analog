using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.ML.OnnxRuntime;
using System.Numerics.Tensors;

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


            // Create Tensor model input
            // The model expects input to be in the shape of (N x 3 x H x W) i.e.
            // mini-batches (where N is the batch size) of 3-channel RGB images with H and W of 224
            // https://onnxruntime.ai/docs/api/csharp-api#systemnumericstensor

            //var input = new DenseTensor<float>(channelData, new[] { DimBatchSize, DimNumberOfChannels, ImageSizeX, ImageSizeY });

            // Run inferencing
            // https://onnxruntime.ai/docs/api/csharp-api#methods-1
            // https://onnxruntime.ai/docs/api/csharp-api#namedonnxvalue

            // using var results = _session.Run(new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor(ModelInputName, input) });

            // Resolve model output
            // https://github.com/onnx/models/tree/master/vision/classification/mobilenet#output
            // https://onnxruntime.ai/docs/api/csharp-api#disposablenamedonnxvalue

            // var output = results.FirstOrDefault(i => i.Name == ModelOutputName);



            // Postprocess output (get highest score and corresponding label)
            // https://github.com/onnx/models/tree/master/vision/classification/mobilenet#postprocessing

            // var scores = output.AsTensor<float>().ToList();
            // var highestScore = scores.Max();
            // var time = Math.Floor(highestScore / 60) + ":" + highestScore % 60; // returns time as string in format: hours:minutes

            //session.Dispose();

            return "";
        }

        byte[] LoadModelFromEmbeddedResource(string path)
        {
            var assembly = typeof(ClockScan).Assembly;
            byte[] model = null;

            using (Stream stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{path}"))
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    model = memoryStream.ToArray();
                }
            }

            return model;
        }
    }
}
