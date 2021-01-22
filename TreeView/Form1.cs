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

namespace TreeView
{
    public partial class Form1 : Form
    {
        ImageList galery;
        string path;

        public Form1()
        {
            InitializeComponent();
             
            galery = new ImageList();
            galery.Images.Add(new Bitmap("1.png"));
            galery.Images.Add(new Bitmap("2.png"));
            galery.Images.Add(new Bitmap("3.png"));
            galery.Images.Add(new Bitmap("4.png"));
            galery.Images.Add(new Bitmap("5.png"));
            galery.Images.Add(new Bitmap("6.png"));
            galery.Images.Add(new Bitmap("7.png"));
            galery.ImageSize = new Size(30, 30);

            treeView1.ImageList = galery;
            listView1.LargeImageList = galery;
            listView1.SmallImageList = galery;

            treeView1.BeforeSelect += treeView1_BeforeSelect;
            treeView1.BeforeExpand += treeView1_BeforeExpand;
            // заполняем дерево дисками 
            FillDriveNodes();
            A();
        }

        void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            e.Node.Nodes.Clear();
            string[] dirs;
            try
            {
                if (Directory.Exists(e.Node.FullPath))
                {
                    dirs = Directory.GetDirectories(e.Node.FullPath);
                    if (dirs.Length != 0)
                    {
                        for (int i = 0; i < dirs.Length; i++)
                        {
                            TreeNode dirNode = new TreeNode(new DirectoryInfo(dirs[i]).Name);
                            FillTreeNode(dirNode, dirs[i]);
                            e.Node.Nodes.Add(dirNode);
                        }
                    }
                }
            }
            catch (Exception ex) {  }
        }
        // событие перед выделением узла
        void treeView1_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            e.Node.Nodes.Clear();
            string[] dirs;
            try
            {
                if (Directory.Exists(e.Node.FullPath))
                {
                    dirs = Directory.GetDirectories(e.Node.FullPath);
                    if (dirs.Length != 0)
                    {
                        for (int i = 0; i < dirs.Length; i++)
                        {
                            TreeNode dirNode = new TreeNode(new DirectoryInfo(dirs[i]).Name);
                            FillTreeNode(dirNode, dirs[i]);
                            e.Node.Nodes.Add(dirNode);
                        }
                    }
                }
            }
            catch (Exception ex) {  }
        }

        // получаем все диски на компьютере
        private void FillDriveNodes()
        {
            try
            {
                foreach (DriveInfo drive in DriveInfo.GetDrives())
                {
                    TreeNode driveNode = new TreeNode { Text = drive.Name , ImageIndex=1,
                        SelectedImageIndex=2};
                    FillTreeNode(driveNode, drive.Name);
                    treeView1.Nodes.Add(driveNode);
                }
            }
            catch (Exception ex) {  }
        }
        // получаем дочерние узлы для определенного узла
        private void FillTreeNode(TreeNode driveNode, string path)
        {
            try
            {
                string[] dirs = Directory.GetDirectories(path);
                foreach (string dir in dirs)
                {
                    TreeNode dirNode = new TreeNode();
                    dirNode.Text = dir.Remove(0, dir.LastIndexOf("\\") + 1);

                    driveNode.ImageIndex = 1;
                    driveNode.SelectedImageIndex = 2;
                    driveNode.Nodes.Add(dirNode);
                    
                }
            }
            catch (Exception ex) {  }
        }

        private void A()
        {
            foreach (DriveInfo item in DriveInfo.GetDrives())
            {
                listView1.Items.Add(item.Name, 0);
            }
        }

        private void Show(string path)
        {
            string[] files = Directory.GetFiles(path);
            string[] dirs = Directory.GetDirectories(path);

            // перебор полученных файлов
            foreach (string file in files)
            {
                ListViewItem lvi = new ListViewItem();
                // установка названия файла
                lvi.Text = file.Remove(0, file.LastIndexOf('\\') + 1);
                if (file.Contains(".txt"))
                {
                    lvi.ImageIndex = 5;
                }
                else if (file.Contains(".mp3"))
                {
                    lvi.ImageIndex = 6;
                }
                else if (file.Contains(".mp4"))
                {
                    lvi.ImageIndex = 4;
                }else
                {
                    lvi.ImageIndex = 0;
                }
                // добавляем элемент в ListView
                listView1.Items.Add(lvi);
            }

            foreach (string file in dirs)
            {
                ListViewItem lvi = new ListViewItem();
                // установка названия файла
                lvi.Text = file.Remove(0, file.LastIndexOf('\\') + 1);
                lvi.ImageIndex = 2; // установка картинки для файла
                // добавляем элемент в ListView
                listView1.Items.Add(lvi);
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            ListView.SelectedIndexCollection collection = listView1.SelectedIndices;
            int a = collection[0];

            if (path==null)
            {
                path = listView1.Items[a].Text;
            }
            else
            {
                path += listView1.Items[a].Text;
            }

            this.Text = listView1.Items[a].Text;

            if (path!=null)
            {
                listView1.Items.Clear();

                int num = path.Length;

                try
                {
                    if (num-1==path.LastIndexOf('\\'))
                    {
                        Show(path);
                    }
                    else
                    {
                        path += "\\";
                        Show(path);
                    }
                }
                catch (Exception ex) { }

                this.Text = path;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (path==null)
                {
                    return;
                }

                List<DriveInfo> drives = new List<DriveInfo>();

                foreach (DriveInfo item in DriveInfo.GetDrives())
                {
                    drives.Add(item);
                }

                int num = path.Length;

                foreach (var item in drives)
                {
                    if (path.Equals(item.Name))
                    {
                        listView1.Items.Clear();
                        path = null;
                        A();

                        return;
                    }
                }
                path = path.Remove(num - 1);

                num = path.LastIndexOf('\\');
                path = path.Remove(num + 1);
                listView1.Items.Clear();
                Show(path);

                this.Text = path;
            }
            catch (Exception ex){ }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                listView1.Items.Clear();
                path = treeView1.SelectedNode.FullPath;
                Show(path);
            }
            catch (Exception ex) { }
        }
    }
}
