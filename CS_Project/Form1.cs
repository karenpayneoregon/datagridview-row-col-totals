using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using CS_Example.Classes;

namespace CS_Example
{
    public partial class Form1 : Form
    {
        private readonly BindingSource _bindingSource = new BindingSource();
        private readonly DataOperations _dataOperations = new DataOperations();
        private string Identifier = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void UpDateTotal()
        {
            DataTable dt = (DataTable)_bindingSource.DataSource;

            if (_bindingSource.Position != -1)
            {
                if (!(((DataRowView)_bindingSource.Current).Row.RowState == DataRowState.Detached))
                {
                    dt.Rows[_bindingSource.Position].EndEdit();
                }
            }

            DataView dv = new DataView() { Table = dt, RowStateFilter = DataViewRowState.CurrentRows, AllowDelete = false };

            try
            {
                lblTotalSale.Text = "Grand Total: " + 
                    (
                        from T in dv.ToTable().AsEnumerable() 
                        select T.Field<int>(_dataOperations.RowTotalFieldName)).Sum().ToString();
            }
            catch (Exception ex)
            {
                //
                // If you land here most likely there is a value for a month that is null
                //
                MessageBox.Show("UpDateTotal throw an exception: " + ex.Message);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _dataOperations.LoadData();
            _bindingSource.DataSource = _dataOperations.DataTable;

            DataGridView1.DataSource = _bindingSource;

            DataGridView1.Columns[_dataOperations.RowTotalFieldName].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            DataGridView1.Columns[_dataOperations.RowTotalFieldName].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            DataGridView1.Columns[_dataOperations.RowTotalFieldName].SortMode = DataGridViewColumnSortMode.NotSortable;

            foreach (var item in _dataOperations.ColumnNames)
            {
                DataGridView1.Columns[item].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            DataGridView1.Columns["TheYear"].HeaderText = "Year";

            DataGridView1.ExpandColumns();
            DataGridView1.Columns[_dataOperations.RowTotalFieldName].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            _dataOperations.DataTable.RowChanged += _RowChanged;
            _dataOperations.DataTable.ColumnChanged += _ColumnChanged;
            _dataOperations.DataTable.RowDeleted += _RowDeleted;

            UpDateTotal();

            // lets make sure all data is shown without scrolling
            this.Width = 1250;

            BindingNavigator1.BindingSource = _bindingSource;
            
            DataGridView1.CellValidating += DataGridView1_CellValidating;
            DataGridView1.Sorted += DataGridView1_Sorted;

            _bindingSource.PositionChanged += bsData_PositionChanged;
        }

        private void DataGridView1_Sorted(object sender, EventArgs e)
        {
            _bindingSource.Position = _bindingSource.Find("Identifier", Identifier);
        }

        private void bsData_PositionChanged(object sender, EventArgs e)
        {
            Identifier = ((DataRowView)_bindingSource.Current)["Identifier"].ToString();
        }

        private void DataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (DataGridView1[e.ColumnIndex, e.RowIndex].IsInEditMode)
            {
                if (DataGridView1[e.ColumnIndex, e.RowIndex].ValueType == typeof(int))
                {
                    Control c = DataGridView1.EditingControl;
                    int tempVar = 0;
                    if (!(int.TryParse(c.Text, out tempVar)))
                    {
                        MessageBox.Show("Must be numeric");
                        e.Cancel = true;
                        DataGridView1.CancelEdit();
                    }
                }
            }
        }

        private void _RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            UpDateTotal();
        }

        private void _ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            if (e.Row.RowState == DataRowState.Deleted) return;
            if (!_dataOperations.ColumnNames.Contains(e.Column.ColumnName)) return;
            
            if (e.Row.RowState != DataRowState.Detached)
            {
                if (Convert.IsDBNull(e.Row[e.Column.ColumnName]))
                {
                    e.Row[e.Column.ColumnName] = 0;
                }

                e.Row.AcceptChanges();
            }

            UpDateTotal();
        }

        private void _RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Add || e.Action == DataRowAction.Change || e.Action == DataRowAction.Commit || e.Action == DataRowAction.Change)
            {
                UpDateTotal();
            }
        }
    }
}