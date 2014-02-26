using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace GenClass
{
    public partial class Home : Form
    {
        public Home()
        {
            InitializeComponent();
        }

        private void Home_Load(object sender, EventArgs e)
        {
        }

        private void FileMenu_click(object sender, EventArgs e)
        {
            ToolStripMenuItem clicked = (ToolStripMenuItem)sender;
            if (clicked.Equals(FileSaveDataMenuItem))
            {
                saveData();
            }
            else if (clicked.Equals(FileLoadDataMenuItem))
            {
                loadData();
                
            }
        }

        private void FileNewMenu_click(object sender, EventArgs e)
        {
            ToolStripMenuItem clicked = (ToolStripMenuItem)sender;
            if(clicked.Equals(FileNewDatasetMenuItem))
            {
                readDataset();
            }

            else if (clicked.Equals(FileNewSongMenuItem))
            {

                OpenFileDialog of = new OpenFileDialog();
                of.Filter = "Sound Files (*.wav)|*.wav|(*.mp3)|*.mp3|(*.au)|*.au";
                of.ShowDialog();
                if (of.FileName != "")
                {
                    String dir = Path.GetDirectoryName(Directory.GetParent(Directory.GetParent(of.FileName).ToString()).ToString());
                    Song song = new Song(Path.GetFileName(of.FileName), dir, of.FileName);
                }
            }
            else if (clicked.Equals(FileNewFeatureMenuItem))
            {
                Add_Item_Popup popup = new Add_Item_Popup();
                popup.ShowDialog();
            }
        }


        public static void readDataset() 
        {
            FolderBrowserDialog browse = new FolderBrowserDialog();
            browse.ShowNewFolderButton = true;
            DialogResult rslt = browse.ShowDialog();
            if (rslt.Equals(DialogResult.OK) && Directory.Exists(browse.SelectedPath))
            {
                traverseDirectoryTree(browse.SelectedPath);
            }
        }

        public static void traverseDirectoryTree(String directory)
        {
            foreach (String file in Directory.GetFiles(directory))
            {
                String ext = Path.GetExtension(directory);
                if (ext.Equals(".wav") || ext.Equals(".mp3") || ext.Equals(".au"))
                {
                    Song song = new Song(Path.GetFileName(file), Path.GetFullPath(directory).TrimEnd(Path.DirectorySeparatorChar), file);
                }
            }
            foreach (String dir in Directory.GetDirectories(directory))
            {
                traverseDirectoryTree(dir);
            }
        }

        public static void loadData()
        {
            OpenFileDialog f = new OpenFileDialog();
                f.Filter = "Data Files (*.dat|*.dat";
                f.CheckFileExists = true;
                f.ShowDialog();
            if (f.FileName!="")
            {
                FileStream file = new FileStream(f.FileName, FileMode.Open);
                BinaryFormatter bf = new BinaryFormatter();
                try 
                {
                    List<Dataset> lst = (List<Dataset>)bf.Deserialize(file);
                    lst = null;
                }
                catch (SerializationException e)
                {
                    Console.WriteLine("Failed to serialize. Reason: " + e.Message);
                    throw;
                }
                finally
                {
                    file.Close();
                }
            }
        }

        public static void readSongs()
        {
            
        }

        public static void readFeatures(String dataset)
        {
            OpenFileDialog featureOpen = new OpenFileDialog();
            featureOpen.DefaultExt = "XML|*.xml";
            featureOpen.Title = "Open Feature Data";
            DialogResult rslt = featureOpen.ShowDialog();
            if (rslt.Equals(DialogResult.OK) && File.Exists(featureOpen.FileName))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(featureOpen.FileName);
                parseXML(doc.GetElementsByTagName("data_set"));
            }
        }

        public static void parseXML(XmlNodeList dataPoints)
        {
            String filename = "";
            String path = "";
            int dataset = -1;
            int song = -1;            
            foreach (XmlNode node in dataPoints)
            {
                if (node.Name.Equals("data_set_id"))
                {
                    filename = Path.GetFileName(node.Value);
                    path = Path.GetFullPath(node.Value);
                    KeyValuePair<int,int> pair = Dataset.searchForSong(filename);
                    dataset = pair.Key;
                    song = pair.Value;
                    if (dataset<0)
                    {
                        
                    }
                }
                else if (node.Name.Equals("feature"))
                {
                    XmlNodeList featureAttr = node.ChildNodes;
                    foreach (XmlNode attr in featureAttr)
                    {
                        Feature feature = null;
                        if (attr.Name.Equals("name"))
                        {
                            feature = new Feature(attr.Value, filename);
                            Dataset.DatasetList.ElementAt(dataset).Value.getSongList().ElementAt(song).Value.addFeature(feature);
                        }
                        else if (attr.Name.Equals("v"))
                        {
                            feature.addValue(float.Parse(attr.Value));
                        }
                    }
                }
            }
        }

        public static void saveData()
        {
            String fn = Microsoft.VisualBasic.Interaction.InputBox("Please enter filename for saved data.", "Filename Creation", "SongObject_all");
            if (fn != "")
            {
                FileStream file = new FileStream(fn + ".dat", FileMode.OpenOrCreate);
                BinaryFormatter bf = new BinaryFormatter();
                try
                {
                    List<Dataset> lst = Dataset.toList();
                    bf.Serialize(file, lst);
                    MessageBox.Show("Save.", "Data has been backed up to hard disk.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                catch (SerializationException e)
                {
                    Console.WriteLine("Failed to serialize. Reason: " + e.Message);
                    throw;
                }
                finally
                {
                    file.Close();
                }
            }
        }
    }
}
