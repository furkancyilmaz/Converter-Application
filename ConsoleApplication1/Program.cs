using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Xml;


namespace ConsoleApplication1
{
    class Program : Form1
    {
        static void Main(string[] args)
        {
            Form1 f1 = new Form1();
            string path;
            Console.WriteLine("Lütfen pathi giriniz.");
            path = Console.ReadLine();
            f1.function0(path);
        }
    }
    public partial class Form1
    {
        string ARGS = "-e -f \"{0}\" -o \"{1}\"";


        public void function0(string path)//browse
        {
            List<string> items = new List<string>();
            {
                string[] files = Directory.GetFiles(path, "*.kmz", SearchOption.AllDirectories);
                string x;
                for (int i = 0; i < files.Length; i++)
                {
                    x = files[i];
                    x=x.Replace(@"\", "/");
                    String[] splitArray3 = x.Split('/');
                    String splitString3 = splitArray3.Take(splitArray3.Length).LastOrDefault();
                    items.Add(splitString3);
                    
                }

                function1(items,path);
            }
        }
        private void function1(List<string> files,string path) // "next" button
        {
            List<String> selectedFolders = new List<String>();

            List<String> daeFiles = new List<String>();

            foreach (object itemChecked in files)
             {
                
                 string zipPath = @path + "\\" + itemChecked.ToString();
                 string extractPath = @path + "\\" + (itemChecked.ToString().Remove(itemChecked.ToString().Length - 4));
                 if (!Directory.Exists(extractPath))
                     ZipFile.ExtractToDirectory(zipPath, extractPath);
                 selectedFolders.Add(extractPath);
             }
            foreach (var x in selectedFolders)       //C:\Users\furkan.yilmaz\Desktop\Yeni klasör 2\ISTANBUL+TARIM
                                                     //C:\Users\furkan.yilmaz\Desktop\Yeni klasör 2\Istanbul
                                                     //C:\Users\furkan.yilmaz\Desktop\Yeni klasör 2\new+new+york1
            {
                                                    //Console.WriteLine(x);
            }
            foreach (var folder in selectedFolders)
            {
                
                daeFiles.AddRange(Directory.GetFiles(folder, "*.dae", SearchOption.AllDirectories));
                //C:\Users\furkan.yilmaz\Desktop\Yeni klasör 2\ISTANBUL+TARIM
                //C: \Users\furkan.yilmaz\Desktop\Yeni klasör 2\Istanbul
                //C:\Users\furkan.yilmaz\Desktop\Yeni klasör 2\new+ new+ york1
                //C: \Users\furkan.yilmaz\Desktop\Yeni klasör 2\tip
            }
            foreach(var folder in daeFiles)
            {
                //Console.WriteLine(folder);
            }

            //C: \Users\furkan.yilmaz\Desktop\Yeni klasör 2\ISTANBUL + TARIM\models\untitled.dae
            //C:\Users\furkan.yilmaz\Desktop\Yeni klasör 2\Istanbul\models\untitled.dae
            //C:\Users\furkan.yilmaz\Desktop\Yeni klasör 2\new+ new+ york1\models\untitled.dae
            //C:\Users\furkan.yilmaz\Desktop\Yeni klasör 2\tip\models\untitled.dae
            ProcessStartInfo info = new ProcessStartInfo();
            foreach (var dae in daeFiles)
            {
                info.FileName = Environment.CurrentDirectory.Remove(Environment.CurrentDirectory.ToString().LastIndexOf("bin\\")) + "converter\\collada2gltf.exe";
                info.Arguments = string.Format(ARGS, dae, dae.Remove(dae.LastIndexOf("dae")) + "gltf").Replace(@"\", "/");
                info.CreateNoWindow = true;
                Process.Start(info);
            }
            createJSON(selectedFolders, daeFiles,path);
        }
        private void createJSON(List<string> folders, List<String> folders2,string path1) //getting data from kml(xml) and creating json
        {
            string id;
            List<string> pathList = new List<string>();
            List<string> gltfpathList = new List<string>();
            List<string> gltffolder = new List<string>();
            foreach (var folder in folders)
            {
                pathList.AddRange(Directory.GetFiles(folder, "*.kml", SearchOption.AllDirectories));
            }
            for (int i = 0; i < gltffolder.Count; i++)
            {
                gltffolder[i] = gltffolder[i] + "\\models";
            }
            foreach (var folder in gltffolder)
            {
                gltfpathList.AddRange(Directory.GetFiles(folder, "*.gltf", SearchOption.AllDirectories));
            }
            string[] paths = pathList.ToArray();
            string[] gltfpaths = gltfpathList.ToArray();
            string jsonPath = "";
            List<Model> objects = new List<Model>();
            string[] jsonFiles = Directory.GetFiles(path1, "models.json", System.IO.SearchOption.AllDirectories);
            for (int i = 0; i < paths.Length; i++)
            {
                id = Path.GetFileName(Path.GetDirectoryName(paths[i]));
                string xmlPath = paths[i];
                string xmlFile = File.ReadAllText(xmlPath);
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.LoadXml(xmlFile);
                XmlNode node = xmldoc.GetElementsByTagName("LookAt")[0];
                Model model;
                if (node != null)
                {
                    try
                    {
                        DirectoryInfo info = new DirectoryInfo(Directory.GetCurrentDirectory());
                        model = new Model();
                        model.lon = double.Parse(node["longitude"].InnerText);
                        model.lat = double.Parse(node["latitude"].InnerText);
                        model.heading = double.Parse(node["heading"].InnerText);
                        model.range = double.Parse(node["range"].InnerText);
                        model.id = id;
                        model.gitfpath = folders2[i].Replace(@"\", "/");
                        model.gitfpath = model.gitfpath.Replace(@"\", "/");
                        String[] splitArray2 = model.gitfpath.Split('/');
                        String splitString2 = "/" + splitArray2.Take(splitArray2.Length - 2).LastOrDefault()
                            + "/" + splitArray2.Take(splitArray2.Length - 1).LastOrDefault()
                            + "/" + splitArray2.Take(splitArray2.Length).LastOrDefault();

                        model.gitfpath = splitString2;
                        model.gitfpath = model.gitfpath.Replace(@".dae", ".gltf");
                        model.gitfpath = model.gitfpath.Replace(@"untitled", id);
                        model.alt = double.Parse(node["altitude"].InnerText);

                        string folders4 = folders2[i].Replace(@".dae", ".gltf");
                        string folders5 = folders4.Replace(@"untitled", id);
                        if (jsonFiles.Length != 0)
                        {
                            objects.Clear();
                        }
                        objects.Add(model);

                        if (File.Exists(folders4) && File.Exists(folders5))
                        {
                            File.Delete(folders4);
                        }
                        renameGltf(folders2[i], model.id);
                    }
                    catch { }
                }
                Dictionary<string, List<Model>> di = new Dictionary<string, List<Model>>();
                if (jsonFiles.Length == 0)
                {
                    jsonPath = path1 + "\\models.json";
                    di.Add("Models", objects);
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(jsonPath);
                    try
                    {
                        fileInfo.IsReadOnly = false;
                    }
                    catch (FileNotFoundException) { };

                    string jsonData1 = JsonConvert.SerializeObject(di);
                    JObject json = JObject.Parse(jsonData1);
                    string x = json.ToString();
                    System.IO.File.WriteAllText(jsonPath, x);
                }
                else
                {
                    Dictionary<string, List<Model>> di2 = new Dictionary<string, List<Model>>();
                    string filePath = jsonFiles[0];
                    var jsonData = System.IO.File.ReadAllText(filePath);
                    var jss = new JavaScriptSerializer();
                    var objList = jss.Deserialize<Dictionary<string, List<Model>>>(jsonData);
                    var li = objList["Models"];
                    li.AddRange(objects);
                    di.Add("Models", li);

                    jsonData = JsonConvert.SerializeObject(di);
                    JObject json = JObject.Parse(jsonData);
                    string t = json.ToString();
                    System.IO.File.WriteAllText(filePath, t);
                }
            }
        }
        private void renameGltf(string folders1, string id)
        {
            string processName = "collada2gltf";
            string folders3;
            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes)
            {
                if (process.ProcessName == processName)
                {
                    process.WaitForExit(1000 * 10);
                }
            }
            folders1 = folders1.Replace(@".dae", ".gltf");
            folders3 = folders1.Replace(@"untitled", id);
            File.Move(folders1, folders3);

        }
    }
    public class Model
    {
        public double lon { get; set; }
        public double lat { get; set; }
        public double heading { get; set; }
        public string gitfpath { get; set; }
        public string id { get; set; }
        public double range { get; set; }
        public double alt { get; set; }
    }
}
