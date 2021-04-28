using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.RequestParams;
using System.Threading;
using System.Net;
using VkNet.Model.Attachments;
using VkNet.Utils;

namespace VkAPIMaybeFinal
{
    public partial class Form1 : Form
    {
        VkApi vkApi = new VkApi();

        public Form1()
        {
            InitializeComponent();

            string path = @"C:\Users\Olga\Desktop\MyFolder\универ\2 курс 3 семестр\Vk API приложение\VkAPIMaybeFinal\auth.txt";
            StreamReader sr = new StreamReader(path, System.Text.Encoding.Default);

            AuthParams.ApplicationId = ulong.Parse(sr.ReadLine());
            AuthParams.Login = sr.ReadLine();
            AuthParams.Password = sr.ReadLine();
            AuthParams.AccessToken = sr.ReadLine();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Authorize();
            InformationAboutUser();
        }

        public void Authorize()
        {
            try
            {
                new Thread(() =>
                {
                    vkApi.Authorize(new ApiAuthParams
                    {
                        ApplicationId = AuthParams.ApplicationId,
                        Login = AuthParams.Login,
                        Password = AuthParams.Password,
                        AccessToken = AuthParams.AccessToken,
                        Settings = Settings.All,
                        UserId = 152877114
                    });
                }).Start();
                Thread.Sleep(0);
            }
            catch
            {
                MessageBox.Show("Неверный логин или пароль");
            }
        }
        public void InformationAboutUser()
        {
            Authorize();
            var user = vkApi.Users.Get(new long[] {152877114}, ProfileFields.Photo200Orig).FirstOrDefault();

            label1.Text = user.FirstName;
            label2.Text = user.LastName;
            label2.Location = new Point(label1.Location.X + label1.Width, label1.Location.Y);
            pictureBox1.ImageLocation = user.Photo200Orig.ToString();
            pictureBox1.Show();
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            label1.Show();
            label2.Show();
            button1.Show();
            button3.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GetFriends();
        }

        public void GetFriends()
        {
            var friends = vkApi.Friends.Get(new FriendsGetParams
            {
                UserId = vkApi.UserId,
                Order = FriendsOrder.Hints,
                Fields = ProfileFields.LastName | ProfileFields.FirstName | ProfileFields.Online | ProfileFields.Photo400Orig | ProfileFields.Uid
            });
            
            List<String> imagelist = new List<string>();
            ImageList images = new ImageList();
            images.ImageSize = new Size(200, 200);
            images.ColorDepth = ColorDepth.Depth32Bit;
            for (int i = 0; i < friends.Count; i++)
            {                
                if (friends[i].Online == true)
                {
                    flowLayoutPanel2.Controls.Add(FriendsView(i, friends));
                }
                flowLayoutPanel1.Controls.Add(FriendsView(i, friends));           
            }
            flowLayoutPanel1.Show();
            flowLayoutPanel2.Show();
        }

        public FlowLayoutPanel FriendsView(int i, VkCollection<User> friends)
        {
            var panel = new FlowLayoutPanel();
            panel.Width = flowLayoutPanel1.Width - 30;
            panel.Height = 80;

            var name = new Label();
            var lastname = new Label();
            //var id = new Label();
            var im = new PictureBox();

            name.Text = friends[i].FirstName;
            name.AutoSize = true;
            name.Font = new Font("Yanone Kaffeesatz", 20, FontStyle.Bold);

            lastname.Text = friends[i].LastName;
            lastname.AutoSize = true;
            lastname.Font = new Font("Yanone Kaffeesatz", 20, FontStyle.Bold);

            /*id.Text = friends[i].Id.ToString();
            id.AutoSize = true;
            id.Font = new Font("Yanone Kaffeesatz", 16, FontStyle.Bold);*/

            if (friends[i].Photo400Orig != null)
            {
                WebClient w2 = new WebClient();
                byte[] imageByte2 = w2.DownloadData(friends[i].Photo400Orig.ToString());
                MemoryStream stream2 = new MemoryStream(imageByte2);
                System.Drawing.Image img2 = System.Drawing.Image.FromStream(stream2);
                im.Image = img2;
            }
            im.Width = 70;
            im.Height = 70;
            im.SizeMode = PictureBoxSizeMode.Zoom;

            panel.Controls.Add(im);
            panel.Controls.Add(name);
            panel.Controls.Add(lastname);
            //panel.Controls.Add(id);

            return panel;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form2 newForm = new Form2(this);
            newForm.Show();
        }
    }



    /*
     public Photo GetPhoto(long userId)
        {
            var photo = vkApi.Photo.Get(new PhotoGetParams()
            {
                OwnerId = userId,
                AlbumId = PhotoAlbumType.Profile,
                Reversed = true
            });

            return photo.FirstOrDefault();
        }
     */
}

