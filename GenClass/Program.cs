using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Xml.Serialization;


namespace GenClass
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]

        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Home());
            loadData();
        }

        static void loadData()
        {
            if (File.Exists("songs.xml"))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SortedList<String, Song>));
                FileStream file = new FileStream("songObject.xml", FileMode.Open);
                Song.songs = (SortedList<String, Song>)serializer.Deserialize(file);
            }
            else
            {
                readSongs();
                readFeatures();
            }
        }

        static void readSongs()
        {
            FolderBrowserDialog browse = new FolderBrowserDialog();
            browse.ShowNewFolderButton = true;
            DialogResult rslt = browse.ShowDialog();
            if (rslt.Equals(DialogResult.OK) && Directory.Exists(browse.SelectedPath))
            {
                traverseDirectoryTree(browse.SelectedPath);
            }
        }

        static void traverseDirectoryTree(String directory)
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

        static void readFeatures()
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

        static void parseXML(XmlNodeList datasets)
        {
            String filename = "";
            String path = "";
            Song song = null;
            foreach (XmlNode node in datasets)
            {
                if (node.Name.Equals("data_set_id"))
                {
                    filename = Path.GetFileName(node.Value);
                    path = Path.GetFullPath(node.Value);
                    if (Song.songs.ContainsKey(filename))
                    {
                        song = Song.songs[filename];
                    }
                    else
                    {
                        song = new Song(filename, "none", path);
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
                            song.addFeature(feature);
                        }
                        else if (attr.Name.Equals("v"))
                        {
                            feature.addValue(float.Parse(attr.Value));
                        }
                    }
                }
            }
        }

        static void saveData()
        {
            try
            {
                XmlSerializer mySerializer = new XmlSerializer(typeof(List<Song>));
                StreamWriter myWriter = new StreamWriter("songObject.xml");
                mySerializer.Serialize(myWriter, Song.songs);
            }
            catch (Exception ex)
            {
                //Log exception here
                MessageBox.Show(ex.ToString(), "Please Retry.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }

}