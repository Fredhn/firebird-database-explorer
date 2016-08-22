using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FirebirdSql.Data.FirebirdClient;
using Finder.Data;

namespace Solution_Finder.UI_Panel
{
    public partial class pnlUserControl : pnlSlider
    {
        Form pnl_owner;
        public pnlUserControl(Form owner) : base (owner)
        {
            InitializeComponent();
            mostraUsuarios();
            this.pnl_owner = owner;
        }
        #region PANEL : USER CONTROL

        public void mostraUsuarios()
        {
            //Carrega arquivo com configuração do banco
            ConfigBD config = new ConfigBD();
            if (FirebirdConnection.Conectar(config.CarregaConfigBD(true)))
            {
                FbCommand consultaUsuarios = new FbCommand("select ID_USUARIO as ID, USUARIO, NOME, SOBRENOME from USUARIO order by ID_USUARIO", FirebirdConnection.conexao);
                DataTable dtUsuarios = new DataTable();
                dtUsuarios.Load(consultaUsuarios.ExecuteReader());
                dtUsuarios.DefaultView.Sort= "ID asc";
                mgridUserControl.DataSource = dtUsuarios;
                FirebirdConnection.Desconectar();
            }
        }
        private void mgridUserControl_Click(object sender, EventArgs e)
        {
            int index = mgridUserControl.SelectedRows[0].Index;
            if (MetroFramework.MetroMessageBox.Show(this, "Deseja alterar o registro desse usuário?", "Controle de Acesso - Usuários",MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                pnlUserFile _pnlUserFile = new pnlUserFile(pnl_owner, index);
                _pnlUserFile.swipe(true);
            }
        }

        private void lnkReturnPnlUsersControl_Click(object sender, EventArgs e)
        {
            this.swipe(false);
        }

        private void lnk_NewUser_Click(object sender, EventArgs e)
        {
            pnlUserFile _pnlUserFile = new pnlUserFile(pnl_owner);
            _pnlUserFile.Closed += _pnlUseFile_Closed;
            _pnlUserFile.swipe(true);
        }

        private void _pnlUseFile_Closed(object sender, EventArgs e)
        {
            mostraUsuarios();
        }
        #endregion

        #region PANEL : USER FILE


        #endregion
    }
}
