using BioMid.Forms;
using BioMid.Models;
using BioMid.Properties;
using BioMid.SQLite;
using BioMid.Utility;
using IARCSC.Models;
using IARCSC.Utility;
using Neurotec.Biometrics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BioMid
{
    public partial class LoginForm : Form
    {
        private Point _mouseLoc;
        public static ResourceManager rmLocale;

        // List<User> users;
        public HttpClient client;

        public LoginForm()
        {
            InitializeComponent();
            //setLocale("en_locale");
            client = new HttpClient();
            setLocale(Settings.Default.locale);
        }




        private void LoginForm_MouseDown(object sender, MouseEventArgs e)
        {
            _mouseLoc = e.Location;

        }

        private void LoginForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int dx = e.Location.X - _mouseLoc.X;
                int dy = e.Location.Y - _mouseLoc.Y;
                this.Location = new Point(this.Location.X + dx, this.Location.Y + dy);
            }
        }



        public void Alert(string msg, AlertForm.enumType type)
        {
            AlertForm frm = new AlertForm();
            frm.showAlert(msg, type);
        }



        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }



        private void lblPashto_Click(object sender, EventArgs e)
        {
            updateLocalSetting("pa_locale");
        }

        private void lblEnglish_Click(object sender, EventArgs e)
        {
            updateLocalSetting("en_locale");

        }

        private void lblDari_Click(object sender, EventArgs e)
        {
            updateLocalSetting("dr_locale");
        }



        private void updateLocalSetting(string locale)
        {
            Settings.Default.locale = locale;
            Settings.Default.Save();

            setLocale(Settings.Default.locale);

        }



        private void setLocale(string locale)
        {
            rmLocale = new ResourceManager("BioMid." + locale, Assembly.GetExecutingAssembly());

            lblEmail.Text = rmLocale.GetString("email");
            lblPassword.Text = rmLocale.GetString("password");
            btnLogin.Text = rmLocale.GetString("login");

        }


        private void lblSettings_Click(object sender, EventArgs e)
        {
            SettingForm sf = new SettingForm();
            sf.ShowDialog();
        }

        private bool ValidateLogin()
        {
            bool result = true;
            if (string.IsNullOrEmpty(txtEmail.Text) || string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                txtEmail.BackColor = Color.Tomato;
                result = true;
            }
            else
            {
                txtEmail.BackColor = Color.White;
            }

            if (string.IsNullOrEmpty(txtPassword.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                txtPassword.BackColor = Color.Tomato;
                result = true;
            }
            else
            {
                txtPassword.BackColor = Color.White;
            }

            return result;
        }
        private async void btnLogin_Click(object sender, EventArgs e)
        {
            //client.Credentials = CredentialCache.DefaultNetworkCredentials;
            btnLogin.Enabled = false;
            //bool isSuccess = await performLogin();
            if (ValidateLogin())
            {
                LoginCredentials loginCredentials = new LoginCredentials();

                loginCredentials.email = txtEmail.Text;
                loginCredentials.password = txtPassword.Text;
                loginCredentials.appName = "biomid";

                LoginData loginData = await User.Login(loginCredentials);

                //if (loginData.user.usertype != "admin" || loginData.user.usertype != "superadmin" || loginData.user.appName != "biomid")
                //{
                //    Alert("You are unauthorized: ", AlertForm.enumType.Error);
                //    //return;
                //}
                //bool status = await performLogin(txtEmail.Text, txtPassword.Text);
                if (loginData.HttpResponse.IsSuccessStatusCode)
                {
                    Environment.SetEnvironmentVariable("access_token", loginData.token);
                    Environment.SetEnvironmentVariable("token_type", loginData.token_type);
                    Environment.SetEnvironmentVariable("expires_in", loginData.expires_in);
                    Environment.SetEnvironmentVariable("user_id", loginData.user.Id.ToString());
                    Environment.SetEnvironmentVariable("user_name", loginData.user.username.ToString());
                    Environment.SetEnvironmentVariable("user_email", loginData.user.email.ToString());
                    this.DialogResult = DialogResult.OK;
                  //  MainForm mf = new MainForm();
                    //mf.Show();
                    //this.Hide();
                }
                else
                {
                    Alert("Cant login to the system: " + loginData.HttpResponse.ReasonPhrase, AlertForm.enumType.Error);
                }
            }

            btnLogin.Enabled = true;
        }
    }
}
