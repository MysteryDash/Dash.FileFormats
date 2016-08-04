// 
// This file is licensed under the terms of the Simple Non Code License (SNCL) 2.0.2.
// The full license text can be found in the file named License.txt.
// Written originally by Alexandre Quoniou in 2016.
//

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MysteryDash.FileFormats.IdeaFactory.PAC;

namespace MysteryDash.PacEditor
{
    public partial class Main : Form
    {
        private Pac pac;

        public Main()
        {
            InitializeComponent();
        }

        private void PacFiles_DragDrop(object sender, DragEventArgs e)
        {
            List<string> paths = ((string[])e.Data.GetData(DataFormats.FileDrop)).ToList();
            Console.WriteLine(Path.GetExtension(paths[0]));
            if (paths.Count == 1 && Path.GetExtension(paths[0] ?? "").Equals(".pac", StringComparison.InvariantCultureIgnoreCase))
            {
                pac?.Dispose();
                PacFiles.Items.Clear();

                pac = new Pac();
                pac.LoadFile(paths[0]);

                PacFiles.Items.AddRange(pac.Files.Select(entry => new ListViewItem(entry.Path.ZeroTerminatedString) {Tag = entry}).ToArray());
            }
            else
            {
                List<string> directories = new List<string>();
                List<string> files = new List<string>();
                paths.ForEach(path =>
                {
                    if (Directory.Exists(path))
                    {
                        directories.Add(path);
                        files.AddRange(Directory.GetFiles(path, "*", SearchOption.AllDirectories));
                    }
                });

                directories.ForEach(directory => paths.Remove(directory));
                paths.AddRange(files);

                if (pac == null)
                {
                    MessageBox.Show("Please load a .pac first !", "Warning !", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    foreach (var file in paths)
                    {
                        string correspondingPath = (from ListViewItem pacFile in PacFiles.Items select pacFile.Text).ToList().Find(path => file.Contains(path));
                        if (correspondingPath == null)
                        {
                            MessageBox.Show($"{file} isn't present in the currently opened .pac !", "Not found !", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            continue;
                        }

                        ListViewItem item = PacFiles.Items.Cast<ListViewItem>().FirstOrDefault(fileItem => fileItem.Text == correspondingPath);
                        item.ForeColor = Color.Red;

                        var entry = (PacEntry)item.Tag;
                        var fileStream = File.OpenRead(file);

                        entry.SetFile(false, (int)fileStream.Length, fileStream, 0, (int)fileStream.Length, false, false);
                    }
                }
            }
        }

        private void PacFiles_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            pac?.Dispose();
        }

        private void Save_Click(object sender, EventArgs e)
        {
            if (pac == null)
            {
                MessageBox.Show("Please load a .pac first !", "Warning !", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var saveDialog = new SaveFileDialog { Filter = "PAC (*.pac)|*.pac" })
            {
                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    MessageBox.Show("Writing the file will take some time and this window may freeze. Please do not panic !", "Exporting new .PAC", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    pac.WriteFile(saveDialog.FileName);
                    MessageBox.Show("Done !");
                }
            }
        }
    }
}
