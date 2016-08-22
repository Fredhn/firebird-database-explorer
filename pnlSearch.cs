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
    public partial class pnlSearch : pnlSlider
    {
        protected string userKeywords;
        protected string[] keywords;
        public pnlSearch(Form owner) : base (owner)
        {
            InitializeComponent();
        }

        #region PANEL : BUSCA
        private void mtbCampoBusca_Click(object sender, EventArgs e)
        {
            mtbCampoBusca.Text = "";
        }

        private void lnkReturnPnlSearch_Click(object sender, EventArgs e)
        {
            this.swipe(false);
            mtbCampoBusca.ResetText();
        }

        private void btnBuscaOK_Click(object sender, EventArgs e)
        {
            ConfigBD config = new ConfigBD();
            if (FirebirdConnection.Conectar(config.CarregaConfigBD()))
            {
                FbCommand consultaKeywords = new FbCommand("", FirebirdConnection.conexao);
                FirebirdConnection.Desconectar();
            }

        }
        #endregion

        #region PANEL : FILTERS
        public void montaFiltros ()
        {
            ConfigBD config = new ConfigBD();
            FirebirdConnection.Conectar(config.CarregaConfigBD());

            //MONTA COMBO BOX ATENDENTES
            FbCommand carregaCBAtendente = new FbCommand("select USUARIO from ATENDIMENTO",FirebirdConnection.conexao);
            DataTable dtAtendente = new DataTable();
            dtAtendente.Load(carregaCBAtendente.ExecuteReader());
            DataRow atendentes = dtAtendente.Rows[0];
            for (int i = 0; i < dtAtendente.Rows.Count; i++)
            {
                mcb_Attendant.Items.Add(dtAtendente.Rows[i][0].ToString());
            }

            //MONTA COMBO BOX TIPO DE ATENDIMENTOS
            FbCommand carregaCBAtendimento = new FbCommand("select T.DESCRICAO as TIPO from ATENDIMENTO A "
                                                          +"inner join TIPOATEND T on (A.TIPOATEND = T.CODIGO)", FirebirdConnection.conexao);
            DataTable dtAtendimento = new DataTable();
            dtAtendimento.Load(carregaCBAtendimento.ExecuteReader());
            DataRow atendimentos = dtAtendimento.Rows[0];
            for (int i = 0; i < dtAtendimento.Rows.Count; i++)
            {
                mcb_AttendanceType.Items.Add(dtAtendimento.Rows[i][0].ToString());
            }

            //MONTA COMBO BOX SUBTIPO DE ATENDIMENTOS
            FbCommand carregaCBSubAtendimento = new FbCommand("select S.DESCRICAO as SUBTIPO from ATENDIMENTO A "
                                              + "inner join TIPOATEND T on (A.TIPOATEND = T.CODIGO)"
                                              + "inner join SUBTIPO_ATEND S on (A.SUBTIPOATEND = S.CODIGO)", FirebirdConnection.conexao);
            DataTable dtSubAtendimento = new DataTable();
            dtSubAtendimento.Load(carregaCBSubAtendimento.ExecuteReader());
            DataRow subAtendimentos = dtSubAtendimento.Rows[0];
            for (int i = 0; i < dtSubAtendimento.Rows.Count; i++)
            {
                mcb_AttendanceSub.Items.Add(dtSubAtendimento.Rows[i][0].ToString());
            }
        }

        #endregion
    }
}
