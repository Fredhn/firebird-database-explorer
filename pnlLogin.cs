using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using FirebirdSql.Data.FirebirdClient;
using Finder.Data;

namespace Solution_Finder.UI_Panel
{
    public partial class pnlLogin : pnlSlider
    {
        public event EventHandler SettingsClosed;
        public string _connectionString;
        Form _owner;

        public pnlLogin(Form owner) : base (owner)
        {
            InitializeComponent();

            this._owner = owner;
        }

        #region PANEL : LOGIN SESSION
        private void mpnl_Login_Paint(object sender, PaintEventArgs e)
        {

        }

        //ClickEvent: LOGIN BUTTON 
        private void btnLogin_Click(object sender, EventArgs e)
        {
            //Conecta ao banco do Finder
            FirebirdConnection.Conectar(CarregaConfigBD(true));
            //Realiza consulta na tabela de usuários registrados no banco
            FbCommand consultaLogin = new FbCommand("select COUNT(*) as Total from USUARIO " + 
                                                    "where USUARIO = @usuario and SENHA = @senha", FirebirdConnection.conexao);
            consultaLogin.Parameters.Add("@usuario", mtb_UserLogin).Value = mtb_UserLogin.Text.ToUpper();
            consultaLogin.Parameters.Add("@senha", mtb_UserPassword).Value = mtb_UserPassword.Text.ToUpper();
            int i = (int)consultaLogin.ExecuteScalar();
            //List<FbParameter> _loginParam = new List<FbParameter>();
            //_loginParam.Add(new FbParameter("USUARIO", mtb_UserLogin.Text));
            //_loginParam.Add(new FbParameter("SENHA", mtb_UserPassword.Text));
            
            //DataTable dtLogin = new DataTable();
            //dtLogin.Load(consultaLogin.ExecuteReader());

            if (mtb_UserLogin.Text != "" && mtb_UserPassword.Text != "")
            {
                if (i > 0)
                {
                    this.swipe(false);
                    mtb_UserLogin.ResetText();
                    mtb_UserPassword.ResetText();
                }
                else
                {
                    MetroFramework.MetroMessageBox.Show(this,"Usuário e senha não encontrados.","Falha no login");
                    //MessageBox.Show("Usuário e senha não encontrados.");
                }
            }
            else
            {
                MetroFramework.MetroMessageBox.Show(this,"Usuário e senha não informados.", "Falha no login");
                //MessageBox.Show("Usuário e senha não informados.");
            }

            FirebirdConnection.Desconectar();
        }


        //TextBox: USUÁRIO
        private void mtb_UserLogin_Click(object sender, EventArgs e)
        {
            mtb_UserPassword.UseSystemPasswordChar = true;
            mtb_UserPassword.Refresh();
            mtb_UserLogin.Text = "";
        }

        //TextBox: SENHA
        private void mtb_UserPassword_Click(object sender, EventArgs e)
        {
            mtb_UserPassword.UseSystemPasswordChar = true;
            mtb_UserPassword.Refresh();
            mtb_UserPassword.Text = "";
        }
        #endregion

        #region PANEL : MENU AJUSTES
        //CARREGA PAINEL MENU AJUSTES
        public void MostraAjustes()
        {
            pnlSettings.Visible = true;
            CarregaCaminhoBD();
        }
        //CARREGA CAMINHO NO BANCO NA TELA DE AJUSTES
        public void CarregaCaminhoBD()
        {
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\" + "FinderBD.txt") && File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\" + "MinerBD.txt"))
            {
                try
                {
                    using (var leitor = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"\" + "FinderBD.txt"))
                    {
                        var jsonObjeto = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(leitor.ReadToEnd());
                        string dbpath = ((string)jsonObjeto.dbpath);
                        string showPath = Path.GetFileName(dbpath);
                        string path = showPath.Replace(@";", string.Empty);
                        mtbFinderDB.Text = path;
                    }
                    using (var leitor = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"\" + "MinerBD.txt"))
                    {
                        var jsonObjeto = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(leitor.ReadToEnd());
                        string dbpath = ((string)jsonObjeto.dbpath);
                        string showPath = Path.GetFileName(dbpath);
                        string path = showPath.Replace(@";", string.Empty);
                        mtbMinerDB.Text = path;
                    }
                }
                catch { }
            }
        }
        //SELECIONA DIRETÓRIO DO BANCO DE DADOS
        public void SelecionaDiretorio(bool finderDB = false)
        {
            using (OpenFileDialog dbBrowser = new OpenFileDialog())
            {
                if (DialogResult.OK == dbBrowser.ShowDialog())
                {
                    string dbPath = dbBrowser.FileName;
                    if (finderDB == true)
                    {
                        mtbFinderDB.Text = dbBrowser.FileName;
                    }
                    else
                    {
                        mtbMinerDB.Text = dbBrowser.FileName;
                    }
                }
            }
        }
        //SALVA ConfigBD.txt
        public void SalvaConfigBD(bool finderDB = false)
        {
            string db;
            if (finderDB == true)
            {
                db = "Database=" + mtbFinderDB.Text + ";";
            }
            else
            {
                db = "Database=" + mtbMinerDB.Text + ";";
            }

            var dadosConfigBD = new
            {
                user = "User=SYSDBA;",
                password = "Password=masterkey;",
                dbpath = db,
                port = "Port=3050;",
                dialect = "Dialect=3;",
                charset = "Charset=NONE;",
                role = "Role=;",
                conn_timeout = "Connection timeout=7;",
                conn_lifetime = "Connection lifetime=0;",
                pooling = "Pooling=true;",
                packet_size = "Packet Size=8192;",
                server_type = "Server Type=0;"
            };

            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dadosConfigBD);

            if (finderDB == true)
            {
                File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\" + "FinderBD.txt", jsonString);
            }
            else
            {
                File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\" + "MinerBD.txt", jsonString);
            }
        }
        //CARREGA ConfigBD.txt
        public string CarregaConfigBD(bool finderBD = false)
        {
            if (finderBD == true && File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\" + "FinderBD.txt"))
            {
                try
                {
                    using (var leitor = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"\" + "FinderBD.txt"))
                    {
                        var jsonObjeto = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(leitor.ReadToEnd());
                        string user = ((string)jsonObjeto.user);
                        string password = ((string)jsonObjeto.password);
                        string dbpath = ((string)jsonObjeto.dbpath);
                        string port = ((string)jsonObjeto.port);
                        string dialect = ((string)jsonObjeto.dialect);
                        string charset = ((string)jsonObjeto.charset);
                        string role = ((string)jsonObjeto.role);
                        string conn_timeout = ((string)jsonObjeto.conn_timeout);
                        string conn_lifetime = ((string)jsonObjeto.conn_lifetime);
                        string pooling = ((string)jsonObjeto.pooling);
                        string packet_size = ((string)jsonObjeto.packet_size);
                        string server_type = ((string)jsonObjeto.server_type);

                        _connectionString = user + password + dbpath + port + dialect + charset + role + conn_timeout + conn_lifetime + pooling + packet_size + server_type;
                    }
                }
                catch { }
            }
            else if (finderBD == false && File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\" + "MinerBD.txt"))
            {
                try
                {
                    using (var leitor = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"\" + "MinerBD.txt"))
                    {
                        var jsonObjeto = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(leitor.ReadToEnd());
                        string user = ((string)jsonObjeto.user);
                        string password = ((string)jsonObjeto.password);
                        string dbpath = ((string)jsonObjeto.dbpath);
                        string port = ((string)jsonObjeto.port);
                        string dialect = ((string)jsonObjeto.dialect);
                        string charset = ((string)jsonObjeto.charset);
                        string role = ((string)jsonObjeto.role);
                        string conn_timeout = ((string)jsonObjeto.conn_timeout);
                        string conn_lifetime = ((string)jsonObjeto.conn_lifetime);
                        string pooling = ((string)jsonObjeto.pooling);
                        string packet_size = ((string)jsonObjeto.packet_size);
                        string server_type = ((string)jsonObjeto.server_type);

                        _connectionString = user + password + dbpath + port + dialect + charset + role + conn_timeout + conn_lifetime + pooling + packet_size + server_type;
                    }
                }
                catch { }
            }

            return _connectionString;
        }
        //EventClick: VOLTAR A TELA LOGIN
        private void lnkSettingsBack_Click(object sender, EventArgs e)
        {
            pnlSettings.Visible = false;

            EventHandler handler = SettingsClosed;
            if (handler != null) handler(this, e);
        }
        //EventClick: INSERE CAMINHO DB (SOLUTION FINDER)
        private void mtbFinderDB_Click(object sender, EventArgs e)
        {
            SelecionaDiretorio(true);
            SalvaConfigBD(true);
            CarregaConfigBD(true);

            if (FirebirdConnection.Conectar(_connectionString))
            {
                pbStatusFinderDB.BackColor = Color.Green;
                FirebirdConnection.Desconectar();
            }
        }
        //EventClick: INSERE CAMINHO DB (MINERAÇÃO)
        private void mtbMinerDB_Click(object sender, EventArgs e)
        {
            SelecionaDiretorio(false);
            SalvaConfigBD(false);
            CarregaConfigBD(false);

            if (FirebirdConnection.Conectar(_connectionString))
            {
                pbStatusMinerBD.BackColor = Color.Green;
                FirebirdConnection.Desconectar();
            }
        }
        private void lnkTestFinder_Click(object sender, EventArgs e)
        {
            CarregaConfigBD(true);

            if (FirebirdConnection.Conectar(_connectionString))
            {
                pbStatusFinderDB.BackColor = Color.Green;
                FirebirdConnection.Desconectar();
            }
        }

        private void lnkTestMiner_Click(object sender, EventArgs e)
        {
            CarregaConfigBD(false);

            if (FirebirdConnection.Conectar(_connectionString))
            {
                pbStatusMinerBD.BackColor = Color.Green;
                FirebirdConnection.Desconectar();
            }
        }
        #endregion
    }
}
