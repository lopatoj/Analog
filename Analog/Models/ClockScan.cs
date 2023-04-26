using System.Threading.Tasks;
using Emgu.CV;

namespace Analog.Models
{
    public class ClockScan
    {
        public async Task<string> GetClassificationAsync(byte[] image)
        {
            Mat _img = new Mat();

            CvInvoke.Imdecode(image, Emgu.CV.CvEnum.ImreadModes.Unchanged, _img);
            CvInvoke.Resize(_img, _img / 255, new System.Drawing.Size(224, 224), 0, 0);
           

            return "";
        }
    }
}
