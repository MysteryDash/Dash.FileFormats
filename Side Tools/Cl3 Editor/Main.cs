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
using MysteryDash.FileFormats.IdeaFactory.CL3;

namespace MysteryDash.Cl3Editor
{
    public partial class Main : Form
    {
        private Cl3 cl3;

        public Main()
        {
            InitializeComponent();
        }

        private void Cl3Files_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void Cl3Files_DragDrop(object sender, DragEventArgs e)
        {
            List<string> paths = ((string[])e.Data.GetData(DataFormats.FileDrop)).ToList();
            Console.WriteLine(Path.GetExtension(paths[0]));
            if (paths.Count == 1 && Path.GetExtension(paths[0] ?? "").Equals(".cl3", StringComparison.InvariantCultureIgnoreCase))
            {
                cl3?.Dispose();
                Cl3Files.Items.Clear();

                cl3 = new Cl3();
                cl3.LoadFile(paths[0]);

                Cl3Files.Items.AddRange(cl3.FileEntries.Entries.Select(entry => new ListViewItem(entry.Name.ZeroTerminatedString) { Tag = entry }).ToArray());
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

                if (cl3 == null)
                {
                    MessageBox.Show("Please load a .cl3 first !", "Warning !", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    foreach (var file in paths)
                    {
                        string correspondingPath = (from ListViewItem cl3File in Cl3Files.Items select cl3File.Text).ToList().Find(path => file.Contains(path));
                        if (correspondingPath == null)
                        {
                            MessageBox.Show($"{file} isn't present in the currently opened .cl3 !", "Not found !", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            continue;
                        }

                        ListViewItem item = Cl3Files.Items.Cast<ListViewItem>().FirstOrDefault(fileItem => fileItem.Text == correspondingPath);
                        item.ForeColor = Color.Red;

                        var entry = (FileEntry)item.Tag;
                        entry.File = File.ReadAllBytes(file);
                    }
                }
            }
        }

        private void Save_Click(object sender, EventArgs e)
        {
            if (cl3 == null)
            {
                MessageBox.Show("Please load a .cl3 first !", "Warning !", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var saveDialog = new SaveFileDialog { Filter = "CL3 (*.cl3)|*.cl3" })
            {
                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    cl3.WriteFile(saveDialog.FileName);
                    MessageBox.Show("Done !");
                }
            }
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            cl3?.Dispose();
        }
    }
}
