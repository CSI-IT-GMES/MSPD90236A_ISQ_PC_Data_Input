using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraCharts;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using JPlatform.Client.Controls6;
using JPlatform.Client.CSIGMESBaseform6;
using JPlatform.Client.JBaseForm6;
using JPlatform.Client.Library6.interFace;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace CSI.GMES.PD
{
    public partial class MSPD90236A : CSIGMESBaseform6//JERPBaseForm
    {
        public bool _firstLoad = true, _allow_edit = true;
        public MyCellMergeHelper _Helper = null;
        public int _tab = 0;

        public MSPD90236A()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            _firstLoad = true;

            base.OnLoad(e);
            NewButton = false;
            AddButton = false;
            DeleteRowButton = false;
            SaveButton = true;
            DeleteButton = false;
            PreviewButton = false;
            PrintButton = false;

            cboDate.EditValue = DateTime.Now.ToString();
            panTop.BackColor = Color.FromArgb(240, 240, 240);
            tabControl.BackColor = Color.FromArgb(240, 240, 240);

            gbPlant.Visible = false;
            gbLine.Visible = false;
            gbMLine.Visible = false;
            gbStyle.Visible = false;
            gbModel.Visible = false;
            gbKey.Visible = false;
            gbYMD.Visible = false;
            gbHMS.Visible = false;
            gbItem.Visible = false;
            gbArea.Visible = false;

            gbPlant_New.Visible = false;
            gbLine_New.Visible = false;
            gbMLine_New.Visible = false;
            gbStyle_New.Visible = false;
            gbModel_New.Visible = false;
            gbKey_New.Visible = false;
            gbYMD_New.Visible = false;
            gbHMS_New.Visible = false;

            gbStand.Caption = gbStand_New.Caption = "Standard\n(Tiêu chuẩn)";
            gbPeriod.Caption = gbPeriod_New.Caption = "Period\n(Giai đoạn)";
            gbArea_Name.Caption = "Area\n(Khu vực)";
            gbProcess.Caption = "Process\n(Công đoạn)";

            Formart_Grid_Product();
            Formart_Grid_Process();

            InitCombobox();

            _firstLoad = false;
        }

        #region [Start Button Event Code By UIBuilder]

        public override void QueryClick()
        {
            try
            {
                pbProgressShow();
                DataTable _dtSource = null;
                _allow_edit = true;

                DataTable _dtCheck = GetData("Q_DAY_SHIFT");
                if(_dtCheck != null && _dtCheck.Rows.Count > 0)
                {
                    _allow_edit = _dtCheck.Rows[0]["RESULT"].ToString().Equals("Y") ? true : false;
                }
                else
                {
                    _allow_edit = false;
                }

                switch (_tab)
                {
                    case 0:
                        InitControls(grdProduct);
                        _dtSource = GetData("Q_PRODUCT");

                        if (_dtSource != null && _dtSource.Rows.Count > 0)
                        {
                            for (int iRow = 0; iRow < _dtSource.Rows.Count; iRow++)
                            {
                                if(_dtSource.Rows[iRow]["PLANT_CD"].ToString().Equals("TOTAL") ||
                                    _dtSource.Rows[iRow]["PLANT_CD"].ToString().Equals("RATE"))
                                {
                                    continue;
                                }

                                byte[] data = Convert.FromBase64String(_dtSource.Rows[iRow]["KEY_NAME_VN"].ToString());
                                _dtSource.Rows[iRow]["KEY_NM"] = _dtSource.Rows[iRow]["KEY_NAME_EN"].ToString() + "\n" + Encoding.UTF8.GetString(data);

                                byte[] dataStand = Convert.FromBase64String(_dtSource.Rows[iRow]["STANDARD_NAME_VN"].ToString());
                                _dtSource.Rows[iRow]["STANDARD_NM"] = _dtSource.Rows[iRow]["STANDARD_NAME_EN"].ToString() + "\n" + Encoding.UTF8.GetString(dataStand);
                            }

                            _dtSource = FormatDataTable(_dtSource);
                            SetData(grdProduct, _dtSource);
                            Formart_Grid_Product();
                            gvwProduct.TopRowIndex = 0;
                        }

                        break;
                    case 1:
                        InitControls(grdProcess);
                        _dtSource = GetData("Q_PROCESS");

                        if (_dtSource != null && _dtSource.Rows.Count > 0)
                        {
                            for (int iRow = 0; iRow < _dtSource.Rows.Count; iRow++)
                            {
                                if (_dtSource.Rows[iRow]["PLANT_CD"].ToString().Equals("TOTAL") ||
                                    _dtSource.Rows[iRow]["PLANT_CD"].ToString().Equals("RATE"))
                                {
                                    continue;
                                }

                                byte[] data = Convert.FromBase64String(_dtSource.Rows[iRow]["KEY_NAME_VN"].ToString());
                                _dtSource.Rows[iRow]["KEY_NM"] = _dtSource.Rows[iRow]["KEY_NAME_EN"].ToString() + "\n" + Encoding.UTF8.GetString(data);

                                byte[] dataStand = Convert.FromBase64String(_dtSource.Rows[iRow]["STANDARD_NAME_VN"].ToString());
                                _dtSource.Rows[iRow]["STANDARD_NM"] = _dtSource.Rows[iRow]["STANDARD_NAME_EN"].ToString() + "\n" + Encoding.UTF8.GetString(dataStand);

                                byte[] dataProcess = Convert.FromBase64String(_dtSource.Rows[iRow]["PROCESS_NAME_VN"].ToString());
                                _dtSource.Rows[iRow]["PROCESS_NM"] = _dtSource.Rows[iRow]["PROCESS_NAME_EN"].ToString() + "\n" + Encoding.UTF8.GetString(dataProcess);

                                byte[] dataCheckpoint = Convert.FromBase64String(_dtSource.Rows[iRow]["CHECKPOINT_NAME_VN"].ToString());
                                _dtSource.Rows[iRow]["CHECKPOINT_NM"] = _dtSource.Rows[iRow]["CHECKPOINT_NAME_EN"].ToString() + "\n" + Encoding.UTF8.GetString(dataCheckpoint);
                            }

                            _dtSource = FormatDataTable(_dtSource);
                            SetData(grdProcess, _dtSource);
                            Formart_Grid_Process();
                            gvwProcess.TopRowIndex = 0;
                        }

                        break;
                    default:
                        break;
                }
            }
            catch { }
            finally
            {
                pbProgressHide();
            }
        }

        public DataTable FormatDataTable(DataTable _dtSource)
        {
            DataTable _dtRef = _dtSource;

            for (int iRow = 0; iRow < _dtRef.Rows.Count; iRow++)
            {
                if (!string.IsNullOrEmpty(_dtRef.Rows[iRow]["RESULT_SCORE"].ToString()))
                {
                    _dtRef.Rows[iRow]["RESULT_SCORE"] = Math.Round(Double.Parse(_dtRef.Rows[iRow]["RESULT_SCORE"].ToString()), 1);
                }
            }

            return _dtRef;
        }

        public string FormatNumber(string value)
        {
            return string.IsNullOrEmpty(value) ? "0" : Math.Round(Double.Parse(value), 1).ToString();
        }

        public string FormatText(string value)
        {
            return string.IsNullOrEmpty(value) ? "" : value;
        }

        public override void SaveClick()
        {
            try
            {
                DialogResult dlr = new DialogResult();

                if (cboPeriod.EditValue.ToString() == "ALL")
                {
                    MessageBox.Show("Không chon Period = ALL");
                    return;
                }

                dlr = MessageBox.Show("Bạn có muốn Save không?", "Save", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (_allow_edit && dlr == DialogResult.Yes)
                {
                    string _argType = "";

                    switch (_tab)
                    {
                        case 0:
                            _argType = "SAVE_PRODUCT";
                            break;
                        case 1:
                            _argType = "SAVE_PROCESS";
                            break;
                        default:
                            break;
                    }

                    bool result = SaveData(_argType);
                    if (result)
                    {
                        MessageBoxW("Save successfully!", IconType.Information);
                        QueryClick();
                    }
                    else
                    {
                        MessageBoxW("Save failed!", IconType.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void Formart_Grid_Product()
        {
            try
            {
                grdProduct.BeginUpdate();

                for (int i = 0; i < gvwProduct.Columns.Count; i++)
                {
                    gvwProduct.Columns[i].OptionsColumn.AllowEdit = false;
                    gvwProduct.Columns[i].OptionsColumn.AllowMerge = DefaultBoolean.True;
                    gvwProduct.Columns[i].OptionsColumn.ReadOnly = true;
                    gvwProduct.Columns[i].OptionsColumn.AllowSort = DefaultBoolean.False;

                    gvwProduct.Columns[i].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    gvwProduct.Columns[i].AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
                    gvwProduct.Columns[i].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    gvwProduct.Columns[i].AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
                    gvwProduct.Columns[i].AppearanceCell.TextOptions.WordWrap = WordWrap.Wrap;
                    gvwProduct.Columns[i].AppearanceCell.Font = new System.Drawing.Font("Calibri", 12, FontStyle.Regular);

                    if (gvwProduct.Columns[i].FieldName.ToString().Equals("STT"))
                    {
                        gvwProduct.Columns[i].Width = 70;
                    }

                    if (gvwProduct.Columns[i].FieldName.ToString().Equals("WORK_HMS_NM"))
                    {
                        gvwProduct.Columns[i].Width = 100;
                    }

                    if (gvwProduct.Columns[i].FieldName.ToString().Equals("KEY_NM"))
                    {
                        gvwProduct.Columns[i].Width = 250;
                        gvwProduct.Columns[i].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        gvwProduct.Columns[i].ColumnEdit = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
                    }

                    if (gvwProduct.Columns[i].FieldName.ToString().Equals("STANDARD_NM"))
                    {
                        gvwProduct.Columns[i].Width = 250;
                        gvwProduct.Columns[i].ColumnEdit = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
                    }

                    if (gvwProduct.Columns[i].FieldName.ToString().Contains("RESULT_SCORE"))
                    {
                        gvwProduct.Columns[i].Width = 120;

                        if (_allow_edit)
                        {
                            gvwProduct.Columns[i].OptionsColumn.AllowEdit = true;
                            gvwProduct.Columns[i].OptionsColumn.ReadOnly = false;
                        }
                    }

                    if (gvwProduct.Columns[i].FieldName.ToString().Contains("MAX_SCORE"))
                    {
                        gvwProduct.Columns[i].Width = 130;
                    }
                }

                gvwProduct.RowHeight = 70;
                grdProduct.EndUpdate();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void Formart_Grid_Process()
        {
            try
            {
                grdProcess.BeginUpdate();

                for (int i = 0; i < gvwProcess.Columns.Count; i++)
                {
                    gvwProcess.Columns[i].OptionsColumn.AllowEdit = false;
                    gvwProcess.Columns[i].OptionsColumn.AllowMerge = DefaultBoolean.True;
                    gvwProcess.Columns[i].OptionsColumn.ReadOnly = true;
                    gvwProcess.Columns[i].OptionsColumn.AllowSort = DefaultBoolean.False;

                    gvwProcess.Columns[i].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    gvwProcess.Columns[i].AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
                    gvwProcess.Columns[i].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    gvwProcess.Columns[i].AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
                    gvwProcess.Columns[i].AppearanceCell.TextOptions.WordWrap = WordWrap.Wrap;
                    gvwProcess.Columns[i].AppearanceCell.Font = new System.Drawing.Font("Calibri", 12, FontStyle.Regular);

                    if (gvwProcess.Columns[i].FieldName.ToString().Equals("STT"))
                    {
                        gvwProcess.Columns[i].Width = 70;
                    }

                    if (gvwProcess.Columns[i].FieldName.ToString().Equals("WORK_HMS_NM"))
                    {
                        gvwProcess.Columns[i].Width = 100;
                    }

                    if (gvwProcess.Columns[i].FieldName.ToString().Equals("AREA_NM"))
                    {
                        gvwProcess.Columns[i].Width = 120;
                        gvwProcess.Columns[i].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        gvwProcess.Columns[i].ColumnEdit = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
                    }

                    if (gvwProcess.Columns[i].FieldName.ToString().Equals("KEY_NM"))
                    {
                        gvwProcess.Columns[i].Width = 250;
                        gvwProcess.Columns[i].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        gvwProcess.Columns[i].ColumnEdit = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
                    }

                    if (gvwProcess.Columns[i].FieldName.ToString().Equals("STANDARD_NM"))
                    {
                        gvwProcess.Columns[i].Width = 250;
                        gvwProcess.Columns[i].ColumnEdit = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
                    }

                    if (gvwProcess.Columns[i].FieldName.ToString().Equals("PROCESS_NM"))
                    {
                        gvwProcess.Columns[i].Width = 250;
                        gvwProcess.Columns[i].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
                        gvwProcess.Columns[i].ColumnEdit = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
                    }

                    if (gvwProcess.Columns[i].FieldName.ToString().Equals("CHECKPOINT_NM"))
                    {
                        gvwProcess.Columns[i].Width = 250;
                        gvwProcess.Columns[i].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
                        gvwProcess.Columns[i].ColumnEdit = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
                    }

                    if (gvwProcess.Columns[i].FieldName.ToString().Contains("RESULT_SCORE"))
                    {
                        gvwProcess.Columns[i].Width = 120;

                        if (_allow_edit)
                        {
                            gvwProcess.Columns[i].OptionsColumn.AllowEdit = true;
                            gvwProcess.Columns[i].OptionsColumn.ReadOnly = false;
                        }
                    }

                    if (gvwProcess.Columns[i].FieldName.ToString().Contains("MAX_SCORE"))
                    {
                        gvwProcess.Columns[i].Width = 130;
                    }
                }

                gvwProcess.RowHeight = 70;
                grdProcess.EndUpdate();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion [Start Button Event Code By UIBuilder] 

        #region [Grid]

        private DataTable GetData(string argType)
        {
            try
            {
                P_MSPD90236A_Q proc = new P_MSPD90236A_Q();
                DataTable dtData = null;

                string _factory = cboFactory.EditValue == null ? "" : cboFactory.EditValue.ToString();
                string _plant = cboPlant.EditValue == null ? "" : cboPlant.EditValue.ToString();
                string _line = cboLine.EditValue == null ? "" : cboLine.EditValue.ToString();
                string _model = cboModel.EditValue == null ? "" : cboModel.EditValue.ToString();
                string _period = cboPeriod.EditValue == null ? "" : cboPeriod.EditValue.ToString();

                dtData = proc.SetParamData(dtData, argType, _factory, _plant, _line, cboDate.yyyymmdd, "", _model, _period);

                ResultSet rs = CommonCallQuery(dtData, proc.ProcName, proc.GetParamInfo(), false, 90000, "", true);
                if (rs == null || rs.ResultDataSet == null || rs.ResultDataSet.Tables.Count == 0 || rs.ResultDataSet.Tables[0].Rows.Count == 0)
                {
                    return null;
                }
                return rs.ResultDataSet.Tables[0];
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        #endregion [Grid]

        #region [Combobox]

        private void InitCombobox()
        {
            LoadDataCbo(cboFactory, "Factory", "Q_FTY");
            LoadDataCbo(cboPlant, "Plant", "Q_LINE");
            LoadDataCbo(cboLine, "Line", "Q_MLINE");
            LoadDataCbo(cboPeriod, "Period", "Q_PERIOD");
            LoadDataCbo(cboModel, "Model", "Q_MODEL");
        }

        private void LoadDataCbo(LookUpEditEx argCbo, string _cbo_nm, string _type)
        {
            try
            {
                DataTable dt = GetData(_type);
                if (dt == null)
                {
                    argCbo.Properties.Columns.Clear();
                    argCbo.Properties.DataSource = dt;

                    return;
                }

                string columnCode = dt.Columns[0].ColumnName;
                string columnName = dt.Columns[1].ColumnName;
                string captionCode = "Code";
                string captionName = _cbo_nm;

                argCbo.Properties.Columns.Clear();
                argCbo.Properties.DataSource = dt;
                argCbo.Properties.ValueMember = columnCode;
                argCbo.Properties.DisplayMember = columnName;
                argCbo.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo(columnCode));
                argCbo.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo(columnName));
                argCbo.Properties.Columns[columnCode].Visible = _type.Equals("Q_MODEL") ? true : false;
                argCbo.Properties.Columns[columnCode].Width = 10;
                argCbo.Properties.Columns[columnCode].Caption = captionCode;
                argCbo.Properties.Columns[columnName].Caption = captionName;
                argCbo.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        #endregion [Combobox]

        #region Events

        private void gvwProduct_CellMerge(object sender, CellMergeEventArgs e)
        {
            try
            {
                if (grdProduct.DataSource == null || gvwProduct.RowCount <= 0) return;

                e.Merge = false;
                e.Handled = true;

                if (e.Column.FieldName.ToString() == "STT")
                {
                    string _value1 = gvwProduct.GetRowCellValue(e.RowHandle1, e.Column.FieldName.ToString()).ToString();
                    string _value2 = gvwProduct.GetRowCellValue(e.RowHandle2, e.Column.FieldName.ToString()).ToString();

                    if (_value1 == _value2 && !string.IsNullOrEmpty(_value1))
                    {
                        e.Merge = true;
                    }
                }

                if (e.Column.FieldName.ToString() == "KEY_NM" || e.Column.FieldName.ToString() == "STANDARD_NM")
                {
                    string _value1 = gvwProduct.GetRowCellValue(e.RowHandle1, "STT").ToString();
                    string _value2 = gvwProduct.GetRowCellValue(e.RowHandle2, "STT").ToString();
                    string _value3 = gvwProduct.GetRowCellValue(e.RowHandle1, e.Column.FieldName.ToString()).ToString();
                    string _value4 = gvwProduct.GetRowCellValue(e.RowHandle2, e.Column.FieldName.ToString()).ToString();

                    if (_value1 == _value2 && _value3 == _value4 && !string.IsNullOrEmpty(_value1) && !string.IsNullOrEmpty(_value3))
                    {
                        e.Merge = true;
                    }
                }
            }
            catch
            {

            }
        }

        private void gvwProduct_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                if (grdProduct.DataSource == null || gvwProduct.RowCount < 1) return;

                if (e.Column.FieldName.ToString().Equals("RESULT_SCORE"))
                {
                    e.Appearance.BackColor = Color.FromArgb(226, 239, 217);
                }

                if (gvwProduct.GetRowCellValue(e.RowHandle, "PLANT_CD").ToString().Equals("TOTAL"))
                {
                    e.Appearance.BackColor = Color.LightYellow;
                }

                if (gvwProduct.GetRowCellValue(e.RowHandle, "PLANT_CD").ToString().Equals("RATE"))
                {
                    e.Appearance.BackColor = Color.FromArgb(255, 228, 225);
                    e.Appearance.ForeColor = Color.Blue;
                }
            }
            catch { }
        }

        private void gvwProduct_CalcRowHeight(object sender, RowHeightEventArgs e)
        {
            try
            {
                if (grdProduct.DataSource == null || gvwProduct.RowCount < 1) return;

                if (e.RowHandle == gvwProduct.RowCount - 1 || gvwProduct.GetRowCellValue(e.RowHandle, "PLANT_CD").ToString().ToUpper().Equals("TOTAL"))
                {
                    e.RowHeight = 45;
                }
            }
            catch { }
        }

        private void gvwProcess_CalcRowHeight(object sender, RowHeightEventArgs e)
        {
            try
            {
                if (grdProcess.DataSource == null || gvwProcess.RowCount < 1) return;

                if (e.RowHandle == gvwProcess.RowCount - 1 || gvwProcess.GetRowCellValue(e.RowHandle, "PLANT_CD").ToString().ToUpper().Equals("TOTAL"))
                {
                    e.RowHeight = 45;
                }
            }
            catch { }
        }

        private void gvwProcess_CellMerge(object sender, CellMergeEventArgs e)
        {
            try
            {
                if (grdProcess.DataSource == null || gvwProcess.RowCount <= 0) return;

                e.Merge = false;
                e.Handled = true;

                if (e.Column.FieldName.ToString() == "STT")
                {
                    string _value1 = gvwProcess.GetRowCellValue(e.RowHandle1, e.Column.FieldName.ToString()).ToString();
                    string _value2 = gvwProcess.GetRowCellValue(e.RowHandle2, e.Column.FieldName.ToString()).ToString();

                    if (_value1 == _value2 && !string.IsNullOrEmpty(_value1))
                    {
                        e.Merge = true;
                    }
                }

                if (e.Column.FieldName.ToString() == "KEY_NM" || e.Column.FieldName.ToString() == "STANDARD_NM")
                {
                    string _value1 = gvwProcess.GetRowCellValue(e.RowHandle1, "STT").ToString();
                    string _value2 = gvwProcess.GetRowCellValue(e.RowHandle2, "STT").ToString();
                    string _value3 = gvwProcess.GetRowCellValue(e.RowHandle1, e.Column.FieldName.ToString()).ToString();
                    string _value4 = gvwProcess.GetRowCellValue(e.RowHandle2, e.Column.FieldName.ToString()).ToString();

                    if (_value1 == _value2 && _value3 == _value4 && !string.IsNullOrEmpty(_value1) && !string.IsNullOrEmpty(_value3))
                    {
                        e.Merge = true;
                    }
                }
            }
            catch
            {

            }
        }

        private void gvwProcess_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                if (grdProcess.DataSource == null || gvwProcess.RowCount < 1) return;

                if (e.Column.FieldName.ToString().Equals("RESULT_SCORE"))
                {
                    e.Appearance.BackColor = Color.FromArgb(226, 239, 217);
                }

                if (gvwProcess.GetRowCellValue(e.RowHandle, "PLANT_CD").ToString().Equals("TOTAL"))
                {
                    e.Appearance.BackColor = Color.LightYellow;
                }

                if (gvwProcess.GetRowCellValue(e.RowHandle, "PLANT_CD").ToString().Equals("RATE"))
                {
                    e.Appearance.BackColor = Color.FromArgb(255, 228, 225);
                    e.Appearance.ForeColor = Color.Blue;
                }
            }
            catch { }
        }

        private void tabControl_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            try
            {
                int _prev_tab = _tab;
                _tab = tabControl.SelectedTabPageIndex;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cboPeriod_EditValueChanged(object sender, EventArgs e)
        {
            InitControls(grdProcess);
            InitControls(grdProduct);
        }

        private void cboFactory_EditValueChanged(object sender, EventArgs e)
        {
            if (!_firstLoad)
            {
                LoadDataCbo(cboPlant, "Plant", "Q_LINE");
            }
        }

        private void cboPlant_EditValueChanged(object sender, EventArgs e)
        {
            if (!_firstLoad)
            {
                LoadDataCbo(cboLine, "Line", "Q_MLINE");
            }
        }

        private void cboLine_EditValueChanged(object sender, EventArgs e)
        {
            if (!_firstLoad)
            {
                LoadDataCbo(cboModel, "Model", "Q_MODEL");
            }
        }

        private void gvwProduct_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            try
            {
                if (grdProduct.DataSource == null || gvwProduct.RowCount < 1) return;

                if (gvwProduct.GetRowCellValue(e.RowHandle, "PLANT_CD").ToString().Equals("RATE"))
                {
                    if (e.Column.FieldName.ToString().Equals("RESULT_SCORE"))
                    {
                        if (!string.IsNullOrEmpty(e.CellValue.ToString()))
                        {
                            e.DisplayText = e.CellValue + "%";
                        }
                    }
                }
            }
            catch { }
        }

        private void gvwProcess_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            try
            {
                if (grdProcess.DataSource == null || gvwProcess.RowCount < 1) return;

                if (gvwProcess.GetRowCellValue(e.RowHandle, "PLANT_CD").ToString().Equals("RATE"))
                {
                    if (e.Column.FieldName.ToString().Equals("RESULT_SCORE"))
                    {
                        if (!string.IsNullOrEmpty(e.CellValue.ToString()))
                        {
                            e.DisplayText = e.CellValue + "%";
                        }
                    }
                }
            }
            catch { }
        }

        public bool SaveData(string _type)
        {
            try
            {
                bool _result = true;
                DataTable dtData = null;
                P_MSPD90236A_S proc = new P_MSPD90236A_S();
                string machineName = $"{SessionInfo.UserName}|{ Environment.MachineName}|{GetIPAddress()}";

                if (_type.Equals("SAVE_PRODUCT"))
                {
                    if (grdProduct.DataSource != null && gvwProduct.RowCount > 0)
                    {
                        DataTable _dtf = BindingData(grdProduct, true, false);
                        int iUpdate = 0, iCount = 0;

                        for (int iRow = 0; iRow < _dtf.Rows.Count; iRow++)
                        {
                            if(_dtf.Rows[iRow]["RowStatus"].ToString().Equals("U") || _dtf.Rows[iRow]["RowStatus"].ToString().Equals("N"))
                            {
                                if (!_dtf.Rows[iRow]["PLANT_CD"].ToString().Equals("TOTAL") &&
                                    !_dtf.Rows[iRow]["PLANT_CD"].ToString().Equals("RATE") &&
                                    !string.IsNullOrEmpty(_dtf.Rows[iRow]["RESULT_SCORE"].ToString()))
                                {
                                    iUpdate++;

                                    string _fty = _dtf.Rows[iRow]["PLANT_CD"].ToString();
                                    string _line = _dtf.Rows[iRow]["LINE_CD"].ToString();
                                    string _mline = _dtf.Rows[iRow]["MLINE_CD"].ToString();
                                    string _ymd = cboDate.yyyymmdd;
                                    string _hms = _dtf.Rows[iRow]["WORK_HMS"].ToString(); ;
                                    string _style = _dtf.Rows[iRow]["STYLE_CD"].ToString();
                                    string _model = _dtf.Rows[iRow]["MODEL_CD"].ToString();
                                    string _key_cd = _dtf.Rows[iRow]["KEY_CD"].ToString();

                                    string _item_cd = "";
                                    string _area_cd = "";

                                    string _max_val = _dtf.Rows[iRow]["MAX_SCORE"].ToString();
                                    string _score_val = _dtf.Rows[iRow]["RESULT_SCORE"].ToString();

                                    if (!string.IsNullOrEmpty(_score_val) && Int32.Parse(_score_val) > Int32.Parse(_max_val))
                                    {
                                        _score_val = _max_val;
                                    }

                                    dtData = proc.SetParamData(dtData,
                                                              _type,
                                                              _fty,
                                                              _line,
                                                              _mline,
                                                              _ymd,
                                                              _hms,
                                                              _style,
                                                              _model,
                                                              _key_cd,
                                                              _item_cd,
                                                              _area_cd,
                                                              _max_val,
                                                              _score_val,
                                                              machineName,
                                                              "CSI.GMES.PD.MSPD90236A");

                                    if (CommonProcessSave(dtData, proc.ProcName, proc.GetParamInfo(), grdProduct))
                                    {
                                        dtData = null;
                                        iCount++;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }

                        if (iUpdate == iCount)
                        {
                            _result = true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else if (_type.Equals("SAVE_PROCESS"))
                {
                    if (grdProcess.DataSource != null && gvwProcess.RowCount > 0)
                    {
                        DataTable _dtf = BindingData(grdProcess, true, false);
                        int iUpdate = 0, iCount = 0;

                        for (int iRow = 0; iRow < _dtf.Rows.Count; iRow++)
                        {
                            if (_dtf.Rows[iRow]["RowStatus"].ToString().Equals("U") || _dtf.Rows[iRow]["RowStatus"].ToString().Equals("N"))
                            {
                                if (!_dtf.Rows[iRow]["PLANT_CD"].ToString().Equals("TOTAL") &&
                                     !_dtf.Rows[iRow]["PLANT_CD"].ToString().Equals("RATE") &&
                                     !string.IsNullOrEmpty(_dtf.Rows[iRow]["RESULT_SCORE"].ToString()))
                                {
                                    iUpdate++;

                                    string _fty = _dtf.Rows[iRow]["PLANT_CD"].ToString();
                                    string _line = _dtf.Rows[iRow]["LINE_CD"].ToString();
                                    string _mline = _dtf.Rows[iRow]["MLINE_CD"].ToString();
                                    string _ymd = cboDate.yyyymmdd;
                                    string _hms = _dtf.Rows[iRow]["WORK_HMS"].ToString(); ;
                                    string _style = _dtf.Rows[iRow]["STYLE_CD"].ToString();
                                    string _model = _dtf.Rows[iRow]["MODEL_CD"].ToString();
                                    string _key_cd = _dtf.Rows[iRow]["KEY_CD"].ToString();

                                    string _item_cd = _dtf.Rows[iRow]["ITEM_CD"].ToString();
                                    string _area_cd = _dtf.Rows[iRow]["AREA_CD"].ToString();

                                    string _max_val = _dtf.Rows[iRow]["MAX_SCORE"].ToString();
                                    string _score_val = _dtf.Rows[iRow]["RESULT_SCORE"].ToString();

                                    if (!string.IsNullOrEmpty(_score_val) && Int32.Parse(_score_val) > Int32.Parse(_max_val))
                                    {
                                        _score_val = _max_val;
                                    }

                                    dtData = proc.SetParamData(dtData,
                                                              _type,
                                                              _fty,
                                                              _line,
                                                              _mline,
                                                              _ymd,
                                                              _hms,
                                                              _style,
                                                              _model,
                                                              _key_cd,
                                                              _item_cd,
                                                              _area_cd,
                                                              _max_val,
                                                              _score_val,
                                                              machineName,
                                                              "CSI.GMES.PD.MSPD90236A");

                                    if (CommonProcessSave(dtData, proc.ProcName, proc.GetParamInfo(), grdProduct))
                                    {
                                        dtData = null;
                                        iCount++;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }

                        if (iUpdate == iCount)
                        {
                            _result = true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }

                return _result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        #endregion

        #region Database

        public class P_MSPD90236A_Q : BaseProcClass
        {
            public P_MSPD90236A_Q()
            {
                // Modify Code : Procedure Name
                _ProcName = "LMES.P_MSPD90236A_Q";
                ParamAdd();
            }
            private void ParamAdd()
            {
                _ParamInfo.Add(new ParamInfo("@ARG_WORK_TYPE", "Varchar", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_PLANT", "Varchar", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_LINE", "Varchar", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_MLINE", "Varchar", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_YMD", "Varchar", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_STYLE", "Varchar", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_MODEL", "Varchar", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_PERIOD", "Varchar", 100, "Input", typeof(System.String)));
            }
            public DataTable SetParamData(DataTable dataTable,
                                        System.String ARG_WORK_TYPE,
                                        System.String ARG_PLANT,
                                        System.String ARG_LINE,
                                        System.String ARG_MLINE,
                                        System.String ARG_YMD,
                                        System.String ARG_STYLE,
                                        System.String ARG_MODEL,
                                        System.String ARG_PERIOD)
            {
                if (dataTable == null)
                {
                    dataTable = new DataTable(_ProcName);
                    foreach (ParamInfo pi in _ParamInfo)
                    {
                        dataTable.Columns.Add(pi.ParamName, pi.TypeClass);
                    }
                }
                // Modify Code : Procedure Parameter
                object[] objData = new object[] {
                                                ARG_WORK_TYPE,
                                                ARG_PLANT,
                                                ARG_LINE,
                                                ARG_MLINE,
                                                ARG_YMD,
                                                ARG_STYLE,
                                                ARG_MODEL,
                                                ARG_PERIOD
                };
                dataTable.Rows.Add(objData);
                return dataTable;
            }
        }

        public class P_MSPD90236A_S : BaseProcClass
        {
            public P_MSPD90236A_S()
            {
                // Modify Code : Procedure Name
                _ProcName = "LMES.P_MSPD90236A_S";
                ParamAdd();
            }
            private void ParamAdd()
            {
                _ParamInfo.Add(new ParamInfo("@ARG_TYPE", "Varchar", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_PLANT", "Varchar", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_LINE", "Varchar", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_MLINE", "Varchar", 100, "Input", typeof(System.String)));

                _ParamInfo.Add(new ParamInfo("@ARG_YMD", "Varchar2", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_HMS", "Varchar2", 100, "Input", typeof(System.String)));

                _ParamInfo.Add(new ParamInfo("@ARG_STYLE", "Varchar", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_MODEL", "Varchar", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_KEY_CD", "Varchar", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_ITEM_CD", "Varchar2", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_AREA_CD", "Varchar2", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_MAX_SCORE", "Varchar2", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_RESULT_SCORE", "Varchar2", 100, "Input", typeof(System.String)));

                _ParamInfo.Add(new ParamInfo("@ARG_CREATE_PC", "Varchar", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@ARG_CREATE_PROGRAM_ID", "Varchar", 100, "Input", typeof(System.String)));
            }
            public DataTable SetParamData(DataTable dataTable,
                                        System.String ARG_TYPE,
                                        System.String ARG_PLANT,
                                        System.String ARG_LINE,
                                        System.String ARG_MLINE,

                                        System.String ARG_YMD,
                                        System.String ARG_HMS,

                                        System.String ARG_STYLE,
                                        System.String ARG_MODEL,
                                        System.String ARG_KEY_CD,
                                        System.String ARG_ITEM_CD,
                                        System.String ARG_AREA_CD,
                                        System.String ARG_MAX_SCORE,
                                        System.String ARG_RESULT_SCORE,

                                        System.String ARG_CREATE_PC,
                                        System.String ARG_CREATE_PROGRAM_ID)
            {
                if (dataTable == null)
                {
                    dataTable = new DataTable(_ProcName);
                    foreach (ParamInfo pi in _ParamInfo)
                    {
                        dataTable.Columns.Add(pi.ParamName, pi.TypeClass);
                    }
                }
                // Modify Code : Procedure Parameter
                object[] objData = new object[] {
                    ARG_TYPE,
                    ARG_PLANT,
                    ARG_LINE,
                    ARG_MLINE,
                    ARG_YMD,
                    ARG_HMS,

                    ARG_STYLE,
                    ARG_MODEL,
                    ARG_KEY_CD,
                    ARG_ITEM_CD,
                    ARG_AREA_CD,
                    ARG_MAX_SCORE,
                    ARG_RESULT_SCORE,

                    ARG_CREATE_PC,
                    ARG_CREATE_PROGRAM_ID
                };
                dataTable.Rows.Add(objData);
                return dataTable;
            }
        }

        #endregion

        DataTable GetDataTable(GridView view)
        {
            DataTable dt = new DataTable();
            foreach (GridColumn c in view.Columns)
                dt.Columns.Add(c.FieldName, c.ColumnType);
            for (int r = 0; r < view.RowCount; r++)
            {
                object[] rowValues = new object[dt.Columns.Count];
                for (int c = 0; c < dt.Columns.Count; c++)
                    rowValues[c] = view.GetRowCellValue(r, dt.Columns[c].ColumnName);
                dt.Rows.Add(rowValues);
            }
            return dt;
        }

        private DataTable LINQResultToDataTable<T>(IEnumerable<T> Linqlist)
        {
            DataTable dt = new DataTable();
            PropertyInfo[] columns = null;
            if (Linqlist == null) return dt;
            foreach (T Record in Linqlist)
            {
                if (columns == null)
                {
                    columns = ((Type)Record.GetType()).GetProperties();
                    foreach (PropertyInfo GetProperty in columns)
                    {
                        Type colType = GetProperty.PropertyType;

                        if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition()
                        == typeof(Nullable<>)))
                        {
                            colType = colType.GetGenericArguments()[0];
                        }

                        dt.Columns.Add(new DataColumn(GetProperty.Name, colType));
                    }
                }
                DataRow dr = dt.NewRow();
                foreach (PropertyInfo pinfo in columns)
                {
                    dr[pinfo.Name] = pinfo.GetValue(Record, null) == null ? DBNull.Value : pinfo.GetValue
                    (Record, null);
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }
    }
}