using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace Analog.Models
{
    public class ClockScan
    {
        public async Task<string> GetClassificationAsync(byte[] image)
        {
            var model = LoadModelFromEmbeddedResource("Analog.Resources.analog.onnx");
            var session = new InferenceSession(model);

            // Reformat image to ResNet50 input
            var container = new DenseTensor<float>(new[] { 1, 3, 224, 224 });
            var imageTensor = new DenseTensor<float>(image, new[] { 1, 3, 224, 224 });
            container[0, 0, 0, 0] = imageTensor[0, 2, 0, 0];
            container[0, 1, 0, 0] = imageTensor[0, 1, 0, 0];
            container[0, 2, 0, 0] = imageTensor[0, 0, 0, 0];
            
            // Run model
            var inputs = new NamedOnnxValue[] { NamedOnnxValue.CreateFromTensor("data", container) };
            using IDisposableReadOnlyCollection<DisposableNamedOnnxValue> results = session.Run(inputs);

            // Get results
            var output = results.First().AsEnumerable<float>();
            var result = output.Max() > 0.5 ? "analog" : "digital";

            return result;
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
