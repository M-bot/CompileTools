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

namespace CompileTools
{
    public partial class GameDialog : Form
    {
        public GameDialog()
        {
            InitializeComponent();
            /*
            CompressionMethod con = new LZ77CNX();
            Stream input2 = new FileStream("op_bg.gmp", FileMode.Open);
            Stream output2 = new FileStream("op_bg.cnx", FileMode.Create);
            con.Compress(input2, output2);
            output2.Flush();
            output2.Close();

            string fileName = "wander.fld";
            Stream input = new FileStream(fileName, FileMode.Open);
            ArchiveMethod converter = new FLDF0200();
            /*
            ArchiveMethod.ArchiveFile[] files = converter.Unpack(input);
            foreach(ArchiveMethod.ArchiveFile file in files) 
            {
                string dir = "output\\" + file.FileDirectory;
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                FileStream current = new FileStream(dir + file.FileName, FileMode.Create);
                file.File.Seek(0, SeekOrigin.Begin);
                for(int x = 0; x < file.File.Length; x++)
                {
                    current.WriteByte((byte)file.File.ReadByte());
                }
                current.Flush();
                current.Close();
            }


            string root = "output\\";
            string[] filenames = Directory.GetFiles(root,"*",SearchOption.AllDirectories);
            ArchiveMethod.ArchiveFile[] files2 = new ArchiveMethod.ArchiveFile[filenames.Length];
            for (int x = 0; x < files2.Length; x++)
            {
                files2[x] = new ArchiveMethod.ArchiveFile(new FileStream(filenames[x], FileMode.Open), Path.GetFileName(filenames[x]), "");
            }
            FileStream output = new FileStream("wander-edit.fld", FileMode.Create);
            converter.Pack(files2, output);
            output.Flush();
            output.Close();

            /*
            ConversionMethod converter2 = new GMP200();
            FileStream gmpi = new FileStream("intro2.gmp", FileMode.Open);
            FileStream gmpo = new FileStream("intro2.bmp", FileMode.Create);
            converter2.ConvertFrom(gmpi, gmpo);
            */
        }
    }
}
