// Copyright (c) 2024 Alvorika

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;



namespace ImageCompressor
{
    public partial class Form1 : Form
    {
        string sourceFileName = null;
        public Form1()
        {
            InitializeComponent();

            // 将trackBar1的ValueChanged事件与trackBar1_ValueChanged方法进行关联
            trackBar1.ValueChanged += new EventHandler(trackBar1_ValueChanged);



            // 为textBox1的TextChanged事件添加事件处理程序
            textBox1.TextChanged += new EventHandler(textBox1_TextChanged);

            // 为textBox1的KeyPress事件添加事件处理程序
            textBox1.KeyPress += new KeyPressEventHandler(textBox1_KeyPress);



            // 允许pictureBox1接受拖拽操作
            pictureBox1.AllowDrop = true;

            // 为pictureBox1的DragEnter和DragDrop事件添加事件处理程序
            pictureBox1.DragEnter += new DragEventHandler(pictureBox1_DragEnter);
            pictureBox1.DragDrop += new DragEventHandler(pictureBox1_DragDrop);

            // 设置2个pictureBox的SizeMode属性为Zoom，以让图片自适应pictureBox1的大小
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;



            // 为button1_Click事件添加事件处理程序
            button1.Click += new EventHandler(button1_Click);



            // 设置窗体在屏幕中心显示
            this.StartPosition = FormStartPosition.CenterScreen;
        }


        private void trackBar1_ValueChanged(Object sender, EventArgs e)
        {
            // 获取trackBar1的值
            int value = trackBar1.Value;

            // 将trackBar1的值设置为textBox1的文本
            textBox1.Text = value.ToString();
        }



        private void textBox1_KeyPress(Object sender, KeyPressEventArgs e)
        {
            // 如果用户输入的字符不是数字，并且也不是退格键，取消输入
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '\b')
            {
                e.Handled = true;
            }

            // 如果textBox1的文本长度已经达到了2，并且用户输入的不是退格键，取消输入
            if (textBox1.Text.Length >= 2 && e.KeyChar != '\b')
            {
                e.Handled = true;
            }
        }



        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // 尝试将textBox1的文本转换为整数
            int value;
            if (int.TryParse(textBox1.Text, out value))
            {
                // 如果value小于1，则将value设置为1
                if (value < 1)
                {
                    value = 1;
                }

                // 如果value大于99则将value设置为99
                if (value > 99)
                {
                    value = 99;
                }
            }
            else
            {
                // 如果textBox1的文本不能转换为整数，将value设置为1
                value = 1;
            }

            // 将value设置为trackBar1的值
            trackBar1.Value = value;
        }



        private void pictureBox1_DragEnter(object sender, DragEventArgs e)
        {
            // 如果拖拽的数据包含文件，接受拖拽操作
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void pictureBox1_DragDrop(object sender, DragEventArgs e)
        {
            // 获取拖拽的文件
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            // 如果至少有一个文件被拖拽
            if (files.Length > 0)
            {
                // 将第一个文件显示在pictureBox1中
                pictureBox1.Image = Image.FromFile(files[0]);

                // 保存源文件的文件名
                sourceFileName = Path.GetFileNameWithoutExtension(files[0]);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // 创建一个OpenFileDialog实例
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // 设置OpenFileDialog的属性
            openFileDialog.Filter = "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.*";
            openFileDialog.Title = "Select an image file";

            // 显示OpenFileDialog
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // 如果用户选择了一个文件，将这个文件显示在PictureBox中
                pictureBox1.Image = Image.FromFile(openFileDialog.FileName);
            }
            sourceFileName = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
        }



        private void button1_Click(object sender, EventArgs e)
        {

            // 检查pictureBox1是否是初始的图片
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("Please select an image first.");
                return;
            }


            // 获取压缩比率
            int ratio;
            if (!int.TryParse(textBox1.Text, out ratio))
            {
                MessageBox.Show("Invalid compression ratio.");
                return;
            }




            // 获取JPEG编码器
            ImageCodecInfo jpegCodec = ImageCodecInfo.GetImageDecoders().FirstOrDefault(codec => codec.FormatID == ImageFormat.Jpeg.Guid);
            if (jpegCodec == null)
            {
                MessageBox.Show("未找到JPEG编码器。");
                return;
            }


            // 设置压缩参数
            EncoderParameters encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, ratio);

            // 创建一个SaveFileDialog实例
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                // 设置SaveFileDialog的属性
                FileName = sourceFileName + " - Compressed.jpg",
                Filter = "JPEG Files(*.jpg)|*.jpg|All files (*.*)|*.*",
                Title = "Save the compressed image"
            };


            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
           
                // 如果用户选择了一个文件，保存压缩后的图片到这个文件
                pictureBox1.Image.Save(saveFileDialog.FileName, jpegCodec, encoderParams);

                

                // 完成后提示
                MessageBox.Show("Compression completed.");
            }
            else
            {
                // 用户取消了保存，不做任何操作
                return;
            }
        }
        private void LoadImage(string fileName)
        {
            using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                pictureBox1.Image = Image.FromStream(stream);
            }
            sourceFileName = Path.GetFileNameWithoutExtension(fileName);
        }
    }
}

