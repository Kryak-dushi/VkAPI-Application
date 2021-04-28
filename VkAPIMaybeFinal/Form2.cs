using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.RequestParams;
using VkNet.Utils;
using static System.Windows.Forms.CheckedListBox;

namespace VkAPIMaybeFinal
{
    public partial class Form2 : Form
    {
        Form firstform;
        VkApi vkApi = new VkApi();
        long id;

        public Form2(Form1 f)
        {
            InitializeComponent();
            Authorize();
            this.firstform = f;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            Authorize();
            var friends = vkApi.Friends.Get(new FriendsGetParams
            {
                UserId = vkApi.UserId,
                Order = FriendsOrder.Hints,
                Fields = ProfileFields.LastName | ProfileFields.FirstName | ProfileFields.Uid
            });

            checkedListBox1.CheckOnClick = true;
            checkedListBox1.SelectionMode = SelectionMode.One;


            for (int i = 0; i < friends.Count; i++)
            {
                checkedListBox1.Items.Add(friends[i].FirstName + ' ' +  friends[i].LastName);
            }
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

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            flowLayoutPanel1.Controls.Clear();
            Authorize();
            var friends = vkApi.Friends.Get(new FriendsGetParams
            {
                UserId = vkApi.UserId,
                Order = FriendsOrder.Hints,
                Fields = ProfileFields.LastName | ProfileFields.FirstName | ProfileFields.Uid
            });

            var ind = checkedListBox1.SelectedIndex;
            this.id = friends[ind].Id;

            try
            {
                VkCollection<Group> groups = vkApi.Groups.Get(new GroupsGetParams
                {
                    UserId = id,
                    Extended = true,
                    Count = 7
                });

                var grs = groups.ToList();
                foreach (var gr in grs)
                {
                    var panel = new FlowLayoutPanel();
                    panel.Width = flowLayoutPanel1.Width - 30;
                    panel.Height = 80;

                    var name = new Label();
                    name.Height = 50;
                    name.Text = gr.Name;

                    var img = new PictureBox();
                    if (gr.Photo200 != null)
                    {
                        WebClient w2 = new WebClient();
                        byte[] imageByte2 = w2.DownloadData(gr.Photo200.ToString());
                        MemoryStream stream2 = new MemoryStream(imageByte2);
                        System.Drawing.Image img2 = System.Drawing.Image.FromStream(stream2);
                        img.Image = img2;
                    }
                    img.Width = 70;
                    img.Height = 70;
                    img.SizeMode = PictureBoxSizeMode.Zoom;

                    panel.Controls.Add(img);
                    panel.Controls.Add(name);

                    flowLayoutPanel1.Controls.Add(panel);
                }
            }
            catch
            {
                var lab = new Label();
                lab.Width = flowLayoutPanel1.Width - 30;
                lab.Height = 150;

                lab.Text = "Пользователь зануда и не доверяет друзьям :(";
                flowLayoutPanel1.Controls.Add(lab);
            }

            flowLayoutPanel1.Show();
        }
    }
}

/*
 var indices = checkedListBox1.CheckedIndices;
            foreach (int ix in indices)
            {
                this.ids.Add(friends[ix].Id);
            }

            var idsfr = vkApi.Friends.Get(new FriendsGetParams
            {
                UserId = this.ids.First(),
                Order = FriendsOrder.Hints,
                Fields = ProfileFields.LastName | ProfileFields.FirstName
            });

            for (int i = 0; i < idsfr.Count; i++)
            {
                var lab = new Label();
                lab.Width = flowLayoutPanel1.Width - 30;
                lab.Height = 50;
                lab.Text = idsfr[i].FirstName + ' ' + idsfr[i].LastName;
                flowLayoutPanel1.Controls.Add(lab);
            }
            long first = this.ids.First();
            this.ids.Remove(first);

            var idsMet = vkApi.Friends.GetMutual(new FriendsGetMutualParams
            {
                SourceUid = 152877114,
                TargetUid = first
            }) ;

            foreach (var item in idsMet)
            {

                Console.WriteLine(item.Id);
            }

            foreach (var item in idsMet)
            {
                var lab = new Label();
                lab.Width = flowLayoutPanel1.Width - 30;
                lab.Height = 50;
                lab.Text = item.Id.ToString();
                flowLayoutPanel1.Controls.Add(lab);
            }

            this.ids.Clear();*/
