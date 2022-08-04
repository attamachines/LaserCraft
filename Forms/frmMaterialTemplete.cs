using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace laserCraft_Control
{
    public partial class frmMaterialTemplete : Form
    {
        private FormStateType _FormType = FormStateType.InputData;

        public FormStateType FormType
        {
            get { return _FormType; }
            set {
                _FormType = value;
                SetState();
            }

        }

        
        public frmMaterialTemplete()
        {
            InitializeComponent();
        }

        private void SetState()
        {
            if (_FormType == FormStateType.Dialog)
            {
                grbInfo.Visible = false;
                gdData.Location = new Point(12, 12);
                pnControl.Location = new Point(12, gdData.Location.Y + gdData.Height + 10);
                btnOk.Visible = true;
                this.Height = pnControl.Location.Y + pnControl.Height + 50;
            }
            else if (_FormType == FormStateType.InputData)
            {
                grbInfo.Visible = true;
                grbInfo.Location = new Point(12, 12);
                gdData.Location = new Point(12, grbInfo.Location.Y + grbInfo.Height + 10);
                pnControl.Location = new Point(12, gdData.Location.Y + gdData.Height + 10);
                btnOk.Visible = false;
                this.Height = pnControl.Location.Y + pnControl.Height + 50;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmMaterialTemplete_Load(object sender, EventArgs e)
        {
            clsMaterialTempleteModes model = clsMaterialTempleteModes.Instance;

            if (model != null)
            {
                gdData.DataSource = model.Data;
            }
        }

        private void gdData_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >0)
            {

            }
        }

        private void gdData_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex > 0)
            {

            }
        }
    }
}
