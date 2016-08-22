using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Finder.Data;
using FirebirdSql.Data.FirebirdClient;

namespace Solution_Finder.UI_Panel
{
    public partial class pnlUserFile : pnlSlider
    {
        int indice = 0;
        bool altera = false;
        string aux1;
        string aux2;
        public pnlUserFile(Form owner) : base (owner)
        {
            InitializeComponent();
        }

        public pnlUserFile(Form owner, int index = 0) : base(owner)
        {
            InitializeComponent();
            this.indice = index;
            InitializeEditMode();
            altera = true;
        }

        public void InitializeEditMode ()
        {
            ConfigBD config = new ConfigBD();
            if (FirebirdConnection.Conectar(config.CarregaConfigBD(true)))
            {
                FbCommand consultaCadastros = new FbCommand("select * from USUARIO " +
                                                           "where ID_USUARIO = " + indice.ToString()
                                                           +" order by ID_USUARIO", FirebirdConnection.conexao);
                DataTable dtCadastros = new DataTable();
                dtCadastros.Load(consultaCadastros.ExecuteReader());
                dtCadastros.DefaultView.Sort = "ID_USUARIO asc";
                DataRow registro = dtCadastros.Rows[0];

                mtb_ID.Text = registro.Field<Int32>("ID_USUARIO").ToString();
                mtb_UserName.Text = registro.Field<string>("USUARIO");
                mtb_UserPassword.Text = registro.Field<string>("SENHA");
                mtb_UserFirstName.Text = registro.Field<string>("NOME");
                mtb_UserLastName.Text = registro.Field<string>("SOBRENOME");
                mtb_UserCharge.Text = registro.Field<Int32>("CARGO").ToString();
                string aux = registro.Field<string>("USUARIO_MASTER");
                if (aux == "T") mtgl_MasterUser.Checked = true;
                aux = registro.Field<string>("USUARIO_BLOQ");
                if (aux == "T") mtgl_BlockedUser.Checked = true;
                FirebirdConnection.Desconectar();
            }
            else
            {
                MetroFramework.MetroMessageBox.Show(this,"Sem conexão com banco de dados!","Controle de Acesso - Usuários",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }
        }

        private void lnkReturnPnlUserFile_Click(object sender, EventArgs e)
        {
            
            this.swipe(false);
            this.Dispose();
        }

        private void lnkUserFileSubmit_Click(object sender, EventArgs e)
        {
            if (altera == false)
            {
                ConfigBD config = new ConfigBD();
                if (FirebirdConnection.Conectar(config.CarregaConfigBD(true)))
                {
                    FbCommand consultaID = new FbCommand("select MAX (ID_USUARIO) as ID_USUARIO from USUARIO", FirebirdConnection.conexao);
                    DataTable dtID = new DataTable();
                    dtID.Load(consultaID.ExecuteReader());
                    DataRow registro = dtID.Rows[0];

                    Int32 register_ID = registro.Field<Int32>("ID_USUARIO");

                    FbCommand insereCadastro = new FbCommand("insert into USUARIO (ID_USUARIO, USUARIO, SENHA, NOME, SOBRENOME, USUARIO_MASTER, USUARIO_BLOQ, CARGO)" + 
                                                             " values (@id_usuario, @usuario, @senha, @nome, @sobrenome, @usuario_master, @usuario_bloq, @cargo)",FirebirdConnection.conexao);
                    insereCadastro.Parameters.Add("@id_usuario", mtb_ID).Value = (register_ID + 1);
                    insereCadastro.Parameters.Add("@usuario", mtb_UserName).Value = mtb_UserName.Text.ToUpper();
                    insereCadastro.Parameters.Add("@senha", mtb_UserPassword).Value = mtb_UserPassword.Text.ToUpper();
                    insereCadastro.Parameters.Add("@nome", mtb_UserFirstName).Value = mtb_UserFirstName.Text;
                    insereCadastro.Parameters.Add("@sobrenome", mtb_UserLastName).Value = mtb_UserLastName.Text;
                    if (mtgl_MasterUser.Text == "On")
                    {
                        aux1 = "T";
                    }
                    else
                    {
                        aux1 = "F";
                    }
                    insereCadastro.Parameters.Add("@usuario_master", mtgl_MasterUser).Value = aux1;
                    if (mtgl_BlockedUser.Text == "On")
                    {
                        aux2 = "T";
                    }
                    else
                    {
                        aux2 = "F";
                    }
                    insereCadastro.Parameters.Add("@usuario_bloq", mtgl_BlockedUser).Value = aux2;
                    insereCadastro.Parameters.Add("@cargo", mtb_UserCharge).Value = int.Parse(mtb_UserCharge.Text);

                    insereCadastro.ExecuteNonQuery();
                    FirebirdConnection.Desconectar();
                }
                else
                {
                    MetroFramework.MetroMessageBox.Show(this,"Não há comunicação com o banco de dados.","Atenção!",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                }
            }
            else
            {
                ConfigBD config = new ConfigBD();
                if (FirebirdConnection.Conectar(config.CarregaConfigBD(true)))
                {
                    FbCommand insereCadastro = new FbCommand("update USUARIO set USUARIO = @usuario," + 
                                                                                "SENHA = @senha," +
                                                                                "NOME = @nome," + 
                                                                                "SOBRENOME = @sobrenome," +
                                                                                "USUARIO_MASTER = @usuario_master," + 
                                                                                "USUARIO_BLOQ = @usuario_bloq," + 
                                                                                "CARGO = @cargo" + 
                                                             " where ID_USUARIO = " + (indice.ToString()), FirebirdConnection.conexao);
                    insereCadastro.Parameters.Add("@usuario", mtb_UserName).Value = mtb_UserName.Text.ToUpper();
                    insereCadastro.Parameters.Add("@senha", mtb_UserPassword).Value = mtb_UserPassword.Text.ToUpper();
                    insereCadastro.Parameters.Add("@nome", mtb_UserFirstName).Value = mtb_UserFirstName.Text;
                    insereCadastro.Parameters.Add("@sobrenome", mtb_UserLastName).Value = mtb_UserLastName.Text;
                    if (mtgl_MasterUser.Text == "On")
                    {
                        aux1 = "T";
                    }
                    else
                    {
                        aux1 = "F";
                    }
                    insereCadastro.Parameters.Add("@usuario_master", mtgl_MasterUser).Value = aux1;
                    if (mtgl_BlockedUser.Text == "On")
                    {
                        aux2 = "T";
                    }
                    else
                    {
                        aux2 = "F";
                    }
                    insereCadastro.Parameters.Add("@usuario_bloq", mtgl_BlockedUser).Value = aux2;
                    insereCadastro.Parameters.Add("@cargo", mtb_UserCharge).Value = int.Parse(mtb_UserCharge.Text);

                    insereCadastro.ExecuteNonQuery();
                    FirebirdConnection.Desconectar();
                }
                else
                {
                    MetroFramework.MetroMessageBox.Show(this, "Não há comunicação com o banco de dados.", "Atenção!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            this.swipe(false);
        }
    }
}
