using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using SharpDX;
using Format = SharpDX.DXGI.Format;

namespace GameEngine.Utilities
{
    public class ShaderInformation
    {
        public static List<ShaderInformation> ShaderInfoList = new List<ShaderInformation>();

        public static void LoadAll()
        {
            DirectoryInfo di = new DirectoryInfo("Shaders");

            foreach (FileInfo fi in di.GetFiles())
            {
                if (fi.Extension == "desc")
                {
                    string[] data = File.ReadAllLines(fi.FullName);

                    ShaderInformation si = new ShaderInformation();

                    foreach (string s in data)
                    {
                        string key = s.Split(':')[0].ToLower();
                        string value = s.Split(':')[1];

                        switch (key)
                        {
                            case "filename":
                                si.Filename = value;
                                break;
                            case "name":
                                si.Name = value;
                                break;
                            case "vsfuncname":
                                si.VertexShaderFunctionName = value;
                                break;
                            case "psfuncname":
                                si.PixelShaderFunctionName = value;
                                break;
                            case "inputelementcount":
                                si.InputElementCount = int.Parse(value);
                                break;
                        }
                    }
                }
            }
        }

        public string Filename { get; set; }
        public string Name { get; set; }
        public string VertexShaderFunctionName { get; set; }
        public string PixelShaderFunctionName { get; set; }
        public int InputElementCount { get; set; }
        public string[] InputElementNames { get; set; }
        public Format[] InputElementFormats { get; set; }

    }
}
