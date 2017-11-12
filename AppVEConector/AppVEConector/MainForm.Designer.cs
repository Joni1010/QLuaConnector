using System.Windows.Forms;

namespace AppVEConector
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.настройкиToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.подключитьсяToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.panel2 = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.PanelPortfolios = new System.Windows.Forms.GroupBox();
            this.dataGridPortfolios = new System.Windows.Forms.DataGridView();
            this.Account = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LimitKind = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Balance = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CurBalance = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PosBalanse = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VarMargin = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Commision = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PanelPositions = new System.Windows.Forms.GroupBox();
            this.dataGridPositions = new System.Windows.Forms.DataGridView();
            this.NamePos = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Code = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ActPoss = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Orders = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PosVarMargin = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BtnGetDepth = new System.Windows.Forms.DataGridViewButtonColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tabPageTrades = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.groupBoxMyTrades = new System.Windows.Forms.GroupBox();
            this.dataGridViewMyTrades = new System.Windows.Forms.DataGridView();
            this.NumMyTrade = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IdMyTrade = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OrderMyTrade = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SecMyTrade = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TimeMyTrade = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PriceMyTrade = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VolumeMyTrade = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DirectionMyTrade = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBoxAllTrades = new System.Windows.Forms.GroupBox();
            this.dataGridViewAllTrade = new System.Windows.Forms.DataGridView();
            this.AllTradesNum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AllTradesId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AllTradesSec = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AllTradesTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AllTradesPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AllTradesVolume = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AllTradesDirection = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPageOrders = new System.Windows.Forms.TabPage();
            this.panel4 = new System.Windows.Forms.Panel();
            this.splitContainerListOrders = new System.Windows.Forms.SplitContainer();
            this.groupBoxOrders = new System.Windows.Forms.GroupBox();
            this.dataGridViewOrders = new System.Windows.Forms.DataGridView();
            this.NumOrders = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StatusOrders = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IDOrders = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OrdersObjSec = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SecOrders = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PriceOrders = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VolumeOrders = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BalanceOrders = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DirectionOrders = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NamesOrders = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel3 = new System.Windows.Forms.Panel();
            this.groupBoxCreateOrder = new System.Windows.Forms.GroupBox();
            this.buttonOrdersShowDepth = new System.Windows.Forms.Button();
            this.labelOrdersCountOrder = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonOrderCreateSell = new System.Windows.Forms.Button();
            this.labelPosSec = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.buttonOrdersCancelAll = new System.Windows.Forms.Button();
            this.labelOrdersSec = new System.Windows.Forms.Label();
            this.labelOrdersClass = new System.Windows.Forms.Label();
            this.textBoxOrderFindSec = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.OrdersLastPrice = new System.Windows.Forms.Label();
            this.buttonOrderCreateBuy = new System.Windows.Forms.Button();
            this.OrdersSetVolume = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.OrdersSetPrice = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPageStopOrders = new System.Windows.Forms.TabPage();
            this.splitContainerMainOrders = new System.Windows.Forms.SplitContainer();
            this.splitContainerTablesStopOrders = new System.Windows.Forms.SplitContainer();
            this.dataGridViewStopOrders = new System.Windows.Forms.DataGridView();
            this.StopOrdersNum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StopOrdersID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StopOrdersSec = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StopOrderыType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StopOrdersCondition = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StopOrdersStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StopOrdersPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StopOrdersVolume = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StopOrdersPriceStop1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StopOrdersPriceStop2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StopOrdersSpread = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StopOrdersOffset = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panelConditionStopList = new System.Windows.Forms.Panel();
            this.groupBoxFiltersStopOrders = new System.Windows.Forms.GroupBox();
            this.checkBoxSOExec = new System.Windows.Forms.CheckBox();
            this.checkBoxSOClosed = new System.Windows.Forms.CheckBox();
            this.checkBoxSOActive = new System.Windows.Forms.CheckBox();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.tabControlMain.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.PanelPortfolios.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridPortfolios)).BeginInit();
            this.PanelPositions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridPositions)).BeginInit();
            this.tabPageTrades.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.groupBoxMyTrades.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMyTrades)).BeginInit();
            this.groupBoxAllTrades.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAllTrade)).BeginInit();
            this.tabPageOrders.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerListOrders)).BeginInit();
            this.splitContainerListOrders.Panel1.SuspendLayout();
            this.splitContainerListOrders.SuspendLayout();
            this.groupBoxOrders.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewOrders)).BeginInit();
            this.panel3.SuspendLayout();
            this.groupBoxCreateOrder.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.OrdersSetVolume)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.OrdersSetPrice)).BeginInit();
            this.tabPageStopOrders.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMainOrders)).BeginInit();
            this.splitContainerMainOrders.Panel2.SuspendLayout();
            this.splitContainerMainOrders.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerTablesStopOrders)).BeginInit();
            this.splitContainerTablesStopOrders.Panel1.SuspendLayout();
            this.splitContainerTablesStopOrders.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewStopOrders)).BeginInit();
            this.panelConditionStopList.SuspendLayout();
            this.groupBoxFiltersStopOrders.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 401);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(933, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.AutoSize = false;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(400, 17);
            this.toolStripStatusLabel1.Text = "Добро пожаловать в терминал.";
            this.toolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.настройкиToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(933, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // настройкиToolStripMenuItem
            // 
            this.настройкиToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.подключитьсяToolStripMenuItem});
            this.настройкиToolStripMenuItem.Name = "настройкиToolStripMenuItem";
            this.настройкиToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.настройкиToolStripMenuItem.Text = "Main";
            // 
            // подключитьсяToolStripMenuItem
            // 
            this.подключитьсяToolStripMenuItem.Name = "подключитьсяToolStripMenuItem";
            this.подключитьсяToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.подключитьсяToolStripMenuItem.Text = "Подключиться";
            this.подключитьсяToolStripMenuItem.Click += new System.EventHandler(this.ConnectToolStripMenuItem_Click);
            // 
            // tabControlMain
            // 
            this.tabControlMain.Controls.Add(this.tabPage1);
            this.tabControlMain.Controls.Add(this.tabPageTrades);
            this.tabControlMain.Controls.Add(this.tabPageOrders);
            this.tabControlMain.Controls.Add(this.tabPageStopOrders);
            this.tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlMain.Location = new System.Drawing.Point(0, 24);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(933, 377);
            this.tabControlMain.TabIndex = 3;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.Transparent;
            this.tabPage1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tabPage1.Controls.Add(this.panel2);
            this.tabPage1.Controls.Add(this.panel1);
            this.tabPage1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(925, 351);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Главная";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.splitContainer1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(212, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(708, 343);
            this.panel2.TabIndex = 1;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.PanelPortfolios);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.PanelPositions);
            this.splitContainer1.Size = new System.Drawing.Size(708, 343);
            this.splitContainer1.SplitterDistance = 160;
            this.splitContainer1.TabIndex = 0;
            // 
            // PanelPortfolios
            // 
            this.PanelPortfolios.Controls.Add(this.dataGridPortfolios);
            this.PanelPortfolios.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PanelPortfolios.Location = new System.Drawing.Point(0, 0);
            this.PanelPortfolios.Name = "PanelPortfolios";
            this.PanelPortfolios.Size = new System.Drawing.Size(708, 160);
            this.PanelPortfolios.TabIndex = 3;
            this.PanelPortfolios.TabStop = false;
            this.PanelPortfolios.Text = "Портфели";
            // 
            // dataGridPortfolios
            // 
            this.dataGridPortfolios.AllowUserToOrderColumns = true;
            this.dataGridPortfolios.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridPortfolios.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Account,
            this.LimitKind,
            this.Balance,
            this.CurBalance,
            this.PosBalanse,
            this.VarMargin,
            this.Commision});
            this.dataGridPortfolios.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridPortfolios.Location = new System.Drawing.Point(3, 16);
            this.dataGridPortfolios.Name = "dataGridPortfolios";
            this.dataGridPortfolios.ReadOnly = true;
            this.dataGridPortfolios.RowHeadersVisible = false;
            this.dataGridPortfolios.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridPortfolios.RowTemplate.Height = 18;
            this.dataGridPortfolios.RowTemplate.ReadOnly = true;
            this.dataGridPortfolios.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridPortfolios.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridPortfolios.Size = new System.Drawing.Size(702, 141);
            this.dataGridPortfolios.TabIndex = 0;
            // 
            // Account
            // 
            this.Account.Frozen = true;
            this.Account.HeaderText = "Счет";
            this.Account.Name = "Account";
            this.Account.ReadOnly = true;
            this.Account.Width = 80;
            // 
            // LimitKind
            // 
            this.LimitKind.Frozen = true;
            this.LimitKind.HeaderText = "Тип";
            this.LimitKind.Name = "LimitKind";
            this.LimitKind.ReadOnly = true;
            this.LimitKind.Width = 30;
            // 
            // Balance
            // 
            this.Balance.Frozen = true;
            this.Balance.HeaderText = "Баланс";
            this.Balance.Name = "Balance";
            this.Balance.ReadOnly = true;
            this.Balance.Width = 90;
            // 
            // CurBalance
            // 
            this.CurBalance.Frozen = true;
            this.CurBalance.HeaderText = "Тек.Баланс";
            this.CurBalance.Name = "CurBalance";
            this.CurBalance.ReadOnly = true;
            // 
            // PosBalanse
            // 
            this.PosBalanse.Frozen = true;
            this.PosBalanse.HeaderText = "Баланс поз.";
            this.PosBalanse.Name = "PosBalanse";
            this.PosBalanse.ReadOnly = true;
            // 
            // VarMargin
            // 
            this.VarMargin.Frozen = true;
            this.VarMargin.HeaderText = "Вар. маржа";
            this.VarMargin.Name = "VarMargin";
            this.VarMargin.ReadOnly = true;
            // 
            // Commision
            // 
            this.Commision.Frozen = true;
            this.Commision.HeaderText = "Бир. сбор";
            this.Commision.Name = "Commision";
            this.Commision.ReadOnly = true;
            this.Commision.Width = 80;
            // 
            // PanelPositions
            // 
            this.PanelPositions.Controls.Add(this.dataGridPositions);
            this.PanelPositions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PanelPositions.Location = new System.Drawing.Point(0, 0);
            this.PanelPositions.Name = "PanelPositions";
            this.PanelPositions.Size = new System.Drawing.Size(708, 179);
            this.PanelPositions.TabIndex = 5;
            this.PanelPositions.TabStop = false;
            this.PanelPositions.Text = "Позиции";
            // 
            // dataGridPositions
            // 
            this.dataGridPositions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridPositions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.NamePos,
            this.Code,
            this.ActPoss,
            this.Orders,
            this.PosVarMargin,
            this.BtnGetDepth});
            this.dataGridPositions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridPositions.Location = new System.Drawing.Point(3, 16);
            this.dataGridPositions.Name = "dataGridPositions";
            this.dataGridPositions.RowHeadersVisible = false;
            this.dataGridPositions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridPositions.Size = new System.Drawing.Size(702, 160);
            this.dataGridPositions.TabIndex = 1;
            this.dataGridPositions.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridPositions_CellContentClick);
            // 
            // NamePos
            // 
            this.NamePos.Frozen = true;
            this.NamePos.HeaderText = "Назв.";
            this.NamePos.Name = "NamePos";
            this.NamePos.ReadOnly = true;
            // 
            // Code
            // 
            this.Code.Frozen = true;
            this.Code.HeaderText = "Код";
            this.Code.Name = "Code";
            this.Code.ReadOnly = true;
            this.Code.Width = 70;
            // 
            // ActPoss
            // 
            this.ActPoss.Frozen = true;
            this.ActPoss.HeaderText = "Кол. акт. поз.";
            this.ActPoss.Name = "ActPoss";
            this.ActPoss.ReadOnly = true;
            // 
            // Orders
            // 
            this.Orders.Frozen = true;
            this.Orders.HeaderText = "Заявки";
            this.Orders.Name = "Orders";
            this.Orders.ReadOnly = true;
            // 
            // PosVarMargin
            // 
            this.PosVarMargin.Frozen = true;
            this.PosVarMargin.HeaderText = "Вар. маржа";
            this.PosVarMargin.Name = "PosVarMargin";
            this.PosVarMargin.ReadOnly = true;
            // 
            // BtnGetDepth
            // 
            this.BtnGetDepth.Frozen = true;
            this.BtnGetDepth.HeaderText = "Раб. окно";
            this.BtnGetDepth.Name = "BtnGetDepth";
            this.BtnGetDepth.ReadOnly = true;
            this.BtnGetDepth.Text = "Get win";
            this.BtnGetDepth.UseColumnTextForButtonValue = true;
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(209, 343);
            this.panel1.TabIndex = 0;
            // 
            // tabPageTrades
            // 
            this.tabPageTrades.Controls.Add(this.splitContainer2);
            this.tabPageTrades.Location = new System.Drawing.Point(4, 22);
            this.tabPageTrades.Name = "tabPageTrades";
            this.tabPageTrades.Size = new System.Drawing.Size(925, 351);
            this.tabPageTrades.TabIndex = 1;
            this.tabPageTrades.Text = "Сделки";
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.groupBoxMyTrades);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.groupBoxAllTrades);
            this.splitContainer2.Size = new System.Drawing.Size(925, 351);
            this.splitContainer2.SplitterDistance = 176;
            this.splitContainer2.TabIndex = 0;
            // 
            // groupBoxMyTrades
            // 
            this.groupBoxMyTrades.Controls.Add(this.dataGridViewMyTrades);
            this.groupBoxMyTrades.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxMyTrades.Location = new System.Drawing.Point(0, 0);
            this.groupBoxMyTrades.Name = "groupBoxMyTrades";
            this.groupBoxMyTrades.Size = new System.Drawing.Size(925, 176);
            this.groupBoxMyTrades.TabIndex = 2;
            this.groupBoxMyTrades.TabStop = false;
            this.groupBoxMyTrades.Text = "Свои сделки";
            // 
            // dataGridViewMyTrades
            // 
            this.dataGridViewMyTrades.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewMyTrades.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.NumMyTrade,
            this.IdMyTrade,
            this.OrderMyTrade,
            this.SecMyTrade,
            this.TimeMyTrade,
            this.PriceMyTrade,
            this.VolumeMyTrade,
            this.DirectionMyTrade});
            this.dataGridViewMyTrades.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewMyTrades.Location = new System.Drawing.Point(3, 16);
            this.dataGridViewMyTrades.Name = "dataGridViewMyTrades";
            this.dataGridViewMyTrades.RowHeadersVisible = false;
            this.dataGridViewMyTrades.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewMyTrades.Size = new System.Drawing.Size(919, 157);
            this.dataGridViewMyTrades.TabIndex = 0;
            // 
            // NumMyTrade
            // 
            this.NumMyTrade.Frozen = true;
            this.NumMyTrade.HeaderText = "№";
            this.NumMyTrade.Name = "NumMyTrade";
            this.NumMyTrade.ReadOnly = true;
            this.NumMyTrade.Width = 80;
            // 
            // IdMyTrade
            // 
            this.IdMyTrade.Frozen = true;
            this.IdMyTrade.HeaderText = "ID";
            this.IdMyTrade.Name = "IdMyTrade";
            this.IdMyTrade.ReadOnly = true;
            // 
            // OrderMyTrade
            // 
            this.OrderMyTrade.Frozen = true;
            this.OrderMyTrade.HeaderText = "Номер заявки";
            this.OrderMyTrade.Name = "OrderMyTrade";
            this.OrderMyTrade.ReadOnly = true;
            this.OrderMyTrade.Width = 120;
            // 
            // SecMyTrade
            // 
            this.SecMyTrade.Frozen = true;
            this.SecMyTrade.HeaderText = "Инструмент";
            this.SecMyTrade.Name = "SecMyTrade";
            this.SecMyTrade.ReadOnly = true;
            // 
            // TimeMyTrade
            // 
            this.TimeMyTrade.Frozen = true;
            this.TimeMyTrade.HeaderText = "Время";
            this.TimeMyTrade.Name = "TimeMyTrade";
            this.TimeMyTrade.ReadOnly = true;
            // 
            // PriceMyTrade
            // 
            this.PriceMyTrade.Frozen = true;
            this.PriceMyTrade.HeaderText = "Цена";
            this.PriceMyTrade.Name = "PriceMyTrade";
            this.PriceMyTrade.ReadOnly = true;
            this.PriceMyTrade.Width = 90;
            // 
            // VolumeMyTrade
            // 
            this.VolumeMyTrade.Frozen = true;
            this.VolumeMyTrade.HeaderText = "Объем";
            this.VolumeMyTrade.Name = "VolumeMyTrade";
            this.VolumeMyTrade.ReadOnly = true;
            this.VolumeMyTrade.Width = 90;
            // 
            // DirectionMyTrade
            // 
            this.DirectionMyTrade.Frozen = true;
            this.DirectionMyTrade.HeaderText = "Направление";
            this.DirectionMyTrade.Name = "DirectionMyTrade";
            this.DirectionMyTrade.ReadOnly = true;
            // 
            // groupBoxAllTrades
            // 
            this.groupBoxAllTrades.Controls.Add(this.dataGridViewAllTrade);
            this.groupBoxAllTrades.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxAllTrades.Location = new System.Drawing.Point(0, 0);
            this.groupBoxAllTrades.Name = "groupBoxAllTrades";
            this.groupBoxAllTrades.Size = new System.Drawing.Size(925, 171);
            this.groupBoxAllTrades.TabIndex = 4;
            this.groupBoxAllTrades.TabStop = false;
            this.groupBoxAllTrades.Text = "Все сделки";
            // 
            // dataGridViewAllTrade
            // 
            this.dataGridViewAllTrade.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewAllTrade.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.AllTradesNum,
            this.AllTradesId,
            this.AllTradesSec,
            this.AllTradesTime,
            this.AllTradesPrice,
            this.AllTradesVolume,
            this.AllTradesDirection});
            this.dataGridViewAllTrade.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewAllTrade.Location = new System.Drawing.Point(3, 16);
            this.dataGridViewAllTrade.Name = "dataGridViewAllTrade";
            this.dataGridViewAllTrade.RowHeadersVisible = false;
            this.dataGridViewAllTrade.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewAllTrade.Size = new System.Drawing.Size(919, 152);
            this.dataGridViewAllTrade.TabIndex = 0;
            // 
            // AllTradesNum
            // 
            this.AllTradesNum.Frozen = true;
            this.AllTradesNum.HeaderText = "№";
            this.AllTradesNum.Name = "AllTradesNum";
            this.AllTradesNum.ReadOnly = true;
            // 
            // AllTradesId
            // 
            this.AllTradesId.Frozen = true;
            this.AllTradesId.HeaderText = "ID";
            this.AllTradesId.Name = "AllTradesId";
            this.AllTradesId.ReadOnly = true;
            // 
            // AllTradesSec
            // 
            this.AllTradesSec.Frozen = true;
            this.AllTradesSec.HeaderText = "Инструмент";
            this.AllTradesSec.Name = "AllTradesSec";
            this.AllTradesSec.ReadOnly = true;
            // 
            // AllTradesTime
            // 
            this.AllTradesTime.Frozen = true;
            this.AllTradesTime.HeaderText = "Время";
            this.AllTradesTime.Name = "AllTradesTime";
            this.AllTradesTime.ReadOnly = true;
            // 
            // AllTradesPrice
            // 
            this.AllTradesPrice.Frozen = true;
            this.AllTradesPrice.HeaderText = "Цена";
            this.AllTradesPrice.Name = "AllTradesPrice";
            this.AllTradesPrice.ReadOnly = true;
            this.AllTradesPrice.Width = 90;
            // 
            // AllTradesVolume
            // 
            this.AllTradesVolume.Frozen = true;
            this.AllTradesVolume.HeaderText = "Объем";
            this.AllTradesVolume.Name = "AllTradesVolume";
            this.AllTradesVolume.ReadOnly = true;
            this.AllTradesVolume.Width = 90;
            // 
            // AllTradesDirection
            // 
            this.AllTradesDirection.Frozen = true;
            this.AllTradesDirection.HeaderText = "Направление";
            this.AllTradesDirection.Name = "AllTradesDirection";
            this.AllTradesDirection.ReadOnly = true;
            // 
            // tabPageOrders
            // 
            this.tabPageOrders.Controls.Add(this.panel4);
            this.tabPageOrders.Controls.Add(this.panel3);
            this.tabPageOrders.Location = new System.Drawing.Point(4, 22);
            this.tabPageOrders.Name = "tabPageOrders";
            this.tabPageOrders.Size = new System.Drawing.Size(925, 351);
            this.tabPageOrders.TabIndex = 2;
            this.tabPageOrders.Text = "Заявки";
            this.tabPageOrders.UseVisualStyleBackColor = true;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.splitContainerListOrders);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(267, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(658, 351);
            this.panel4.TabIndex = 1;
            // 
            // splitContainerListOrders
            // 
            this.splitContainerListOrders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerListOrders.Location = new System.Drawing.Point(0, 0);
            this.splitContainerListOrders.Name = "splitContainerListOrders";
            this.splitContainerListOrders.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerListOrders.Panel1
            // 
            this.splitContainerListOrders.Panel1.Controls.Add(this.groupBoxOrders);
            this.splitContainerListOrders.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // splitContainerListOrders.Panel2
            // 
            this.splitContainerListOrders.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.splitContainerListOrders.Size = new System.Drawing.Size(658, 351);
            this.splitContainerListOrders.SplitterDistance = 219;
            this.splitContainerListOrders.TabIndex = 2;
            // 
            // groupBoxOrders
            // 
            this.groupBoxOrders.Controls.Add(this.dataGridViewOrders);
            this.groupBoxOrders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxOrders.Location = new System.Drawing.Point(0, 0);
            this.groupBoxOrders.Name = "groupBoxOrders";
            this.groupBoxOrders.Size = new System.Drawing.Size(658, 219);
            this.groupBoxOrders.TabIndex = 0;
            this.groupBoxOrders.TabStop = false;
            this.groupBoxOrders.Text = "Заявки";
            // 
            // dataGridViewOrders
            // 
            this.dataGridViewOrders.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewOrders.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.NumOrders,
            this.StatusOrders,
            this.IDOrders,
            this.OrdersObjSec,
            this.SecOrders,
            this.PriceOrders,
            this.VolumeOrders,
            this.BalanceOrders,
            this.DirectionOrders,
            this.NamesOrders});
            this.dataGridViewOrders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewOrders.Location = new System.Drawing.Point(3, 16);
            this.dataGridViewOrders.Name = "dataGridViewOrders";
            this.dataGridViewOrders.RowHeadersVisible = false;
            this.dataGridViewOrders.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewOrders.Size = new System.Drawing.Size(652, 200);
            this.dataGridViewOrders.TabIndex = 1;
            this.dataGridViewOrders.SelectionChanged += new System.EventHandler(this.dataGridViewOrders_SelectionChanged);
            this.dataGridViewOrders.DoubleClick += new System.EventHandler(this.dataGridViewOrders_DoubleClick);
            // 
            // NumOrders
            // 
            this.NumOrders.Frozen = true;
            this.NumOrders.HeaderText = "№";
            this.NumOrders.Name = "NumOrders";
            this.NumOrders.ReadOnly = true;
            this.NumOrders.Width = 40;
            // 
            // StatusOrders
            // 
            this.StatusOrders.Frozen = true;
            this.StatusOrders.HeaderText = "Статус";
            this.StatusOrders.Name = "StatusOrders";
            this.StatusOrders.ReadOnly = true;
            this.StatusOrders.Width = 50;
            // 
            // IDOrders
            // 
            this.IDOrders.Frozen = true;
            this.IDOrders.HeaderText = "Номер заяв.";
            this.IDOrders.Name = "IDOrders";
            this.IDOrders.ReadOnly = true;
            // 
            // OrdersObjSec
            // 
            this.OrdersObjSec.Frozen = true;
            this.OrdersObjSec.HeaderText = "ObjSec";
            this.OrdersObjSec.Name = "OrdersObjSec";
            this.OrdersObjSec.ReadOnly = true;
            this.OrdersObjSec.Visible = false;
            // 
            // SecOrders
            // 
            this.SecOrders.Frozen = true;
            this.SecOrders.HeaderText = "Инструмент";
            this.SecOrders.Name = "SecOrders";
            this.SecOrders.ReadOnly = true;
            // 
            // PriceOrders
            // 
            this.PriceOrders.Frozen = true;
            this.PriceOrders.HeaderText = "Цена";
            this.PriceOrders.Name = "PriceOrders";
            this.PriceOrders.ReadOnly = true;
            this.PriceOrders.Width = 50;
            // 
            // VolumeOrders
            // 
            this.VolumeOrders.Frozen = true;
            this.VolumeOrders.HeaderText = "Объем";
            this.VolumeOrders.Name = "VolumeOrders";
            this.VolumeOrders.ReadOnly = true;
            this.VolumeOrders.Width = 50;
            // 
            // BalanceOrders
            // 
            this.BalanceOrders.Frozen = true;
            this.BalanceOrders.HeaderText = "Баланс";
            this.BalanceOrders.Name = "BalanceOrders";
            this.BalanceOrders.ReadOnly = true;
            this.BalanceOrders.Width = 50;
            // 
            // DirectionOrders
            // 
            this.DirectionOrders.Frozen = true;
            this.DirectionOrders.HeaderText = "Направление";
            this.DirectionOrders.Name = "DirectionOrders";
            this.DirectionOrders.ReadOnly = true;
            this.DirectionOrders.Width = 90;
            // 
            // NamesOrders
            // 
            this.NamesOrders.Frozen = true;
            this.NamesOrders.HeaderText = "Название";
            this.NamesOrders.Name = "NamesOrders";
            this.NamesOrders.ReadOnly = true;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.groupBoxCreateOrder);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(267, 351);
            this.panel3.TabIndex = 0;
            // 
            // groupBoxCreateOrder
            // 
            this.groupBoxCreateOrder.BackColor = System.Drawing.Color.Transparent;
            this.groupBoxCreateOrder.Controls.Add(this.buttonOrdersShowDepth);
            this.groupBoxCreateOrder.Controls.Add(this.labelOrdersCountOrder);
            this.groupBoxCreateOrder.Controls.Add(this.label3);
            this.groupBoxCreateOrder.Controls.Add(this.buttonOrderCreateSell);
            this.groupBoxCreateOrder.Controls.Add(this.labelPosSec);
            this.groupBoxCreateOrder.Controls.Add(this.label5);
            this.groupBoxCreateOrder.Controls.Add(this.buttonOrdersCancelAll);
            this.groupBoxCreateOrder.Controls.Add(this.labelOrdersSec);
            this.groupBoxCreateOrder.Controls.Add(this.labelOrdersClass);
            this.groupBoxCreateOrder.Controls.Add(this.textBoxOrderFindSec);
            this.groupBoxCreateOrder.Controls.Add(this.label4);
            this.groupBoxCreateOrder.Controls.Add(this.OrdersLastPrice);
            this.groupBoxCreateOrder.Controls.Add(this.buttonOrderCreateBuy);
            this.groupBoxCreateOrder.Controls.Add(this.OrdersSetVolume);
            this.groupBoxCreateOrder.Controls.Add(this.label2);
            this.groupBoxCreateOrder.Controls.Add(this.OrdersSetPrice);
            this.groupBoxCreateOrder.Controls.Add(this.label1);
            this.groupBoxCreateOrder.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxCreateOrder.Location = new System.Drawing.Point(0, 0);
            this.groupBoxCreateOrder.Margin = new System.Windows.Forms.Padding(10);
            this.groupBoxCreateOrder.Name = "groupBoxCreateOrder";
            this.groupBoxCreateOrder.Size = new System.Drawing.Size(267, 150);
            this.groupBoxCreateOrder.TabIndex = 1;
            this.groupBoxCreateOrder.TabStop = false;
            this.groupBoxCreateOrder.Text = "Создать заявку";
            // 
            // buttonOrdersShowDepth
            // 
            this.buttonOrdersShowDepth.Location = new System.Drawing.Point(232, 92);
            this.buttonOrdersShowDepth.Name = "buttonOrdersShowDepth";
            this.buttonOrdersShowDepth.Size = new System.Drawing.Size(27, 23);
            this.buttonOrdersShowDepth.TabIndex = 19;
            this.buttonOrdersShowDepth.Text = "D";
            this.buttonOrdersShowDepth.UseVisualStyleBackColor = true;
            this.buttonOrdersShowDepth.Click += new System.EventHandler(this.buttonOrdersShowDepth_Click);
            // 
            // labelOrdersCountOrder
            // 
            this.labelOrdersCountOrder.AutoSize = true;
            this.labelOrdersCountOrder.Location = new System.Drawing.Point(153, 96);
            this.labelOrdersCountOrder.Name = "labelOrdersCountOrder";
            this.labelOrdersCountOrder.Size = new System.Drawing.Size(30, 13);
            this.labelOrdersCountOrder.TabIndex = 18;
            this.labelOrdersCountOrder.Text = "0 / 0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(103, 96);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 17;
            this.label3.Text = "Заявок";
            // 
            // buttonOrderCreateSell
            // 
            this.buttonOrderCreateSell.BackColor = System.Drawing.Color.Red;
            this.buttonOrderCreateSell.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonOrderCreateSell.Location = new System.Drawing.Point(89, 121);
            this.buttonOrderCreateSell.Name = "buttonOrderCreateSell";
            this.buttonOrderCreateSell.Size = new System.Drawing.Size(75, 23);
            this.buttonOrderCreateSell.TabIndex = 16;
            this.buttonOrderCreateSell.Text = "Sell";
            this.buttonOrderCreateSell.UseVisualStyleBackColor = false;
            this.buttonOrderCreateSell.Click += new System.EventHandler(this.buttonOrderCreateSell_Click);
            // 
            // labelPosSec
            // 
            this.labelPosSec.AutoSize = true;
            this.labelPosSec.Location = new System.Drawing.Point(44, 96);
            this.labelPosSec.Name = "labelPosSec";
            this.labelPosSec.Size = new System.Drawing.Size(13, 13);
            this.labelPosSec.TabIndex = 15;
            this.labelPosSec.Text = "0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 96);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(30, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Поз.";
            // 
            // buttonOrdersCancelAll
            // 
            this.buttonOrdersCancelAll.Location = new System.Drawing.Point(184, 121);
            this.buttonOrdersCancelAll.Name = "buttonOrdersCancelAll";
            this.buttonOrdersCancelAll.Size = new System.Drawing.Size(75, 23);
            this.buttonOrdersCancelAll.TabIndex = 13;
            this.buttonOrdersCancelAll.Text = "Снять все";
            this.buttonOrdersCancelAll.UseVisualStyleBackColor = true;
            this.buttonOrdersCancelAll.Click += new System.EventHandler(this.buttonOrdersCancelAll_Click);
            // 
            // labelOrdersSec
            // 
            this.labelOrdersSec.AutoSize = true;
            this.labelOrdersSec.Location = new System.Drawing.Point(162, 72);
            this.labelOrdersSec.Name = "labelOrdersSec";
            this.labelOrdersSec.Size = new System.Drawing.Size(24, 13);
            this.labelOrdersSec.TabIndex = 12;
            this.labelOrdersSec.Text = "sec";
            // 
            // labelOrdersClass
            // 
            this.labelOrdersClass.AutoSize = true;
            this.labelOrdersClass.Location = new System.Drawing.Point(208, 72);
            this.labelOrdersClass.Name = "labelOrdersClass";
            this.labelOrdersClass.Size = new System.Drawing.Size(31, 13);
            this.labelOrdersClass.TabIndex = 11;
            this.labelOrdersClass.Text = "class";
            this.labelOrdersClass.Click += new System.EventHandler(this.labelOrdersClass_Click);
            // 
            // textBoxOrderFindSec
            // 
            this.textBoxOrderFindSec.Location = new System.Drawing.Point(82, 69);
            this.textBoxOrderFindSec.Name = "textBoxOrderFindSec";
            this.textBoxOrderFindSec.Size = new System.Drawing.Size(74, 20);
            this.textBoxOrderFindSec.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 72);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Код инстр.";
            // 
            // OrdersLastPrice
            // 
            this.OrdersLastPrice.AutoSize = true;
            this.OrdersLastPrice.Location = new System.Drawing.Point(173, 16);
            this.OrdersLastPrice.Name = "OrdersLastPrice";
            this.OrdersLastPrice.Size = new System.Drawing.Size(13, 13);
            this.OrdersLastPrice.TabIndex = 8;
            this.OrdersLastPrice.Text = "0";
            this.OrdersLastPrice.Click += new System.EventHandler(this.OrdersLastPrice_Click);
            // 
            // buttonOrderCreateBuy
            // 
            this.buttonOrderCreateBuy.BackColor = System.Drawing.Color.Lime;
            this.buttonOrderCreateBuy.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonOrderCreateBuy.Location = new System.Drawing.Point(8, 121);
            this.buttonOrderCreateBuy.Name = "buttonOrderCreateBuy";
            this.buttonOrderCreateBuy.Size = new System.Drawing.Size(75, 23);
            this.buttonOrderCreateBuy.TabIndex = 6;
            this.buttonOrderCreateBuy.Text = "Buy";
            this.buttonOrderCreateBuy.UseVisualStyleBackColor = false;
            this.buttonOrderCreateBuy.Click += new System.EventHandler(this.buttonOrderCreateBuy_Click);
            // 
            // OrdersSetVolume
            // 
            this.OrdersSetVolume.Location = new System.Drawing.Point(56, 40);
            this.OrdersSetVolume.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.OrdersSetVolume.Name = "OrdersSetVolume";
            this.OrdersSetVolume.Size = new System.Drawing.Size(100, 20);
            this.OrdersSetVolume.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Объем";
            // 
            // OrdersSetPrice
            // 
            this.OrdersSetPrice.Location = new System.Drawing.Point(56, 14);
            this.OrdersSetPrice.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.OrdersSetPrice.Name = "OrdersSetPrice";
            this.OrdersSetPrice.Size = new System.Drawing.Size(100, 20);
            this.OrdersSetPrice.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Цена";
            // 
            // tabPageStopOrders
            // 
            this.tabPageStopOrders.Controls.Add(this.splitContainerMainOrders);
            this.tabPageStopOrders.Location = new System.Drawing.Point(4, 22);
            this.tabPageStopOrders.Name = "tabPageStopOrders";
            this.tabPageStopOrders.Size = new System.Drawing.Size(925, 351);
            this.tabPageStopOrders.TabIndex = 3;
            this.tabPageStopOrders.Text = "Стоп заявки";
            this.tabPageStopOrders.UseVisualStyleBackColor = true;
            // 
            // splitContainerMainOrders
            // 
            this.splitContainerMainOrders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMainOrders.Location = new System.Drawing.Point(0, 0);
            this.splitContainerMainOrders.Name = "splitContainerMainOrders";
            // 
            // splitContainerMainOrders.Panel2
            // 
            this.splitContainerMainOrders.Panel2.Controls.Add(this.splitContainerTablesStopOrders);
            this.splitContainerMainOrders.Panel2.Controls.Add(this.panelConditionStopList);
            this.splitContainerMainOrders.Size = new System.Drawing.Size(925, 351);
            this.splitContainerMainOrders.SplitterDistance = 162;
            this.splitContainerMainOrders.TabIndex = 1;
            // 
            // splitContainerTablesStopOrders
            // 
            this.splitContainerTablesStopOrders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerTablesStopOrders.Location = new System.Drawing.Point(0, 40);
            this.splitContainerTablesStopOrders.Name = "splitContainerTablesStopOrders";
            this.splitContainerTablesStopOrders.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerTablesStopOrders.Panel1
            // 
            this.splitContainerTablesStopOrders.Panel1.Controls.Add(this.dataGridViewStopOrders);
            this.splitContainerTablesStopOrders.Size = new System.Drawing.Size(759, 311);
            this.splitContainerTablesStopOrders.SplitterDistance = 194;
            this.splitContainerTablesStopOrders.TabIndex = 1;
            // 
            // dataGridViewStopOrders
            // 
            this.dataGridViewStopOrders.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewStopOrders.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.StopOrdersNum,
            this.StopOrdersID,
            this.StopOrdersSec,
            this.StopOrderыType,
            this.StopOrdersCondition,
            this.StopOrdersStatus,
            this.StopOrdersPrice,
            this.StopOrdersVolume,
            this.StopOrdersPriceStop1,
            this.StopOrdersPriceStop2,
            this.StopOrdersSpread,
            this.StopOrdersOffset});
            this.dataGridViewStopOrders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewStopOrders.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewStopOrders.Name = "dataGridViewStopOrders";
            this.dataGridViewStopOrders.ReadOnly = true;
            this.dataGridViewStopOrders.RowHeadersVisible = false;
            this.dataGridViewStopOrders.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewStopOrders.Size = new System.Drawing.Size(759, 194);
            this.dataGridViewStopOrders.TabIndex = 1;
            this.dataGridViewStopOrders.DoubleClick += new System.EventHandler(this.dataGridViewStopOrders_DoubleClick);
            // 
            // StopOrdersNum
            // 
            this.StopOrdersNum.HeaderText = "№";
            this.StopOrdersNum.Name = "StopOrdersNum";
            this.StopOrdersNum.ReadOnly = true;
            this.StopOrdersNum.Width = 50;
            // 
            // StopOrdersID
            // 
            this.StopOrdersID.HeaderText = "Номер";
            this.StopOrdersID.Name = "StopOrdersID";
            this.StopOrdersID.ReadOnly = true;
            this.StopOrdersID.Width = 80;
            // 
            // StopOrdersSec
            // 
            this.StopOrdersSec.HeaderText = "Инструмент";
            this.StopOrdersSec.Name = "StopOrdersSec";
            this.StopOrdersSec.ReadOnly = true;
            this.StopOrdersSec.Width = 80;
            // 
            // StopOrderыType
            // 
            this.StopOrderыType.HeaderText = "Тип";
            this.StopOrderыType.Name = "StopOrderыType";
            this.StopOrderыType.ReadOnly = true;
            // 
            // StopOrdersCondition
            // 
            this.StopOrdersCondition.HeaderText = "Направление";
            this.StopOrdersCondition.Name = "StopOrdersCondition";
            this.StopOrdersCondition.ReadOnly = true;
            this.StopOrdersCondition.Width = 90;
            // 
            // StopOrdersStatus
            // 
            this.StopOrdersStatus.HeaderText = "Статус";
            this.StopOrdersStatus.Name = "StopOrdersStatus";
            this.StopOrdersStatus.ReadOnly = true;
            this.StopOrdersStatus.Width = 80;
            // 
            // StopOrdersPrice
            // 
            this.StopOrdersPrice.HeaderText = "Цена";
            this.StopOrdersPrice.Name = "StopOrdersPrice";
            this.StopOrdersPrice.ReadOnly = true;
            this.StopOrdersPrice.Width = 70;
            // 
            // StopOrdersVolume
            // 
            this.StopOrdersVolume.HeaderText = "Объем";
            this.StopOrdersVolume.Name = "StopOrdersVolume";
            this.StopOrdersVolume.ReadOnly = true;
            this.StopOrdersVolume.Width = 60;
            // 
            // StopOrdersPriceStop1
            // 
            this.StopOrdersPriceStop1.HeaderText = "Стоп цена";
            this.StopOrdersPriceStop1.Name = "StopOrdersPriceStop1";
            this.StopOrdersPriceStop1.ReadOnly = true;
            this.StopOrdersPriceStop1.Width = 70;
            // 
            // StopOrdersPriceStop2
            // 
            this.StopOrdersPriceStop2.HeaderText = "Стоп цена 2";
            this.StopOrdersPriceStop2.Name = "StopOrdersPriceStop2";
            this.StopOrdersPriceStop2.ReadOnly = true;
            this.StopOrdersPriceStop2.Width = 70;
            // 
            // StopOrdersSpread
            // 
            this.StopOrdersSpread.HeaderText = "Спред";
            this.StopOrdersSpread.Name = "StopOrdersSpread";
            this.StopOrdersSpread.ReadOnly = true;
            this.StopOrdersSpread.Width = 70;
            // 
            // StopOrdersOffset
            // 
            this.StopOrdersOffset.HeaderText = "Защ. отступ";
            this.StopOrdersOffset.Name = "StopOrdersOffset";
            this.StopOrdersOffset.ReadOnly = true;
            this.StopOrdersOffset.Width = 50;
            // 
            // panelConditionStopList
            // 
            this.panelConditionStopList.Controls.Add(this.groupBoxFiltersStopOrders);
            this.panelConditionStopList.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelConditionStopList.Location = new System.Drawing.Point(0, 0);
            this.panelConditionStopList.Name = "panelConditionStopList";
            this.panelConditionStopList.Size = new System.Drawing.Size(759, 40);
            this.panelConditionStopList.TabIndex = 0;
            // 
            // groupBoxFiltersStopOrders
            // 
            this.groupBoxFiltersStopOrders.Controls.Add(this.checkBoxSOExec);
            this.groupBoxFiltersStopOrders.Controls.Add(this.checkBoxSOClosed);
            this.groupBoxFiltersStopOrders.Controls.Add(this.checkBoxSOActive);
            this.groupBoxFiltersStopOrders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxFiltersStopOrders.Location = new System.Drawing.Point(0, 0);
            this.groupBoxFiltersStopOrders.Name = "groupBoxFiltersStopOrders";
            this.groupBoxFiltersStopOrders.Size = new System.Drawing.Size(759, 40);
            this.groupBoxFiltersStopOrders.TabIndex = 0;
            this.groupBoxFiltersStopOrders.TabStop = false;
            this.groupBoxFiltersStopOrders.Text = "Фильтр стоп-заявок";
            // 
            // checkBoxSOExec
            // 
            this.checkBoxSOExec.AutoSize = true;
            this.checkBoxSOExec.Location = new System.Drawing.Point(172, 17);
            this.checkBoxSOExec.Name = "checkBoxSOExec";
            this.checkBoxSOExec.Size = new System.Drawing.Size(97, 17);
            this.checkBoxSOExec.TabIndex = 2;
            this.checkBoxSOExec.Text = "Выполненные";
            this.checkBoxSOExec.UseVisualStyleBackColor = true;
            this.checkBoxSOExec.CheckedChanged += new System.EventHandler(this.checkBoxSOExec_CheckedChanged);
            // 
            // checkBoxSOClosed
            // 
            this.checkBoxSOClosed.AutoSize = true;
            this.checkBoxSOClosed.Location = new System.Drawing.Point(88, 17);
            this.checkBoxSOClosed.Name = "checkBoxSOClosed";
            this.checkBoxSOClosed.Size = new System.Drawing.Size(78, 17);
            this.checkBoxSOClosed.TabIndex = 1;
            this.checkBoxSOClosed.Text = "Закрытые";
            this.checkBoxSOClosed.UseVisualStyleBackColor = true;
            this.checkBoxSOClosed.CheckedChanged += new System.EventHandler(this.checkBoxSOClosed_CheckedChanged);
            // 
            // checkBoxSOActive
            // 
            this.checkBoxSOActive.AutoSize = true;
            this.checkBoxSOActive.Location = new System.Drawing.Point(6, 17);
            this.checkBoxSOActive.Name = "checkBoxSOActive";
            this.checkBoxSOActive.Size = new System.Drawing.Size(76, 17);
            this.checkBoxSOActive.TabIndex = 0;
            this.checkBoxSOActive.Text = "Активные";
            this.checkBoxSOActive.UseVisualStyleBackColor = true;
            this.checkBoxSOActive.CheckedChanged += new System.EventHandler(this.checkBoxSOActive_CheckedChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(933, 423);
            this.Controls.Add(this.tabControlMain);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControlMain.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.PanelPortfolios.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridPortfolios)).EndInit();
            this.PanelPositions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridPositions)).EndInit();
            this.tabPageTrades.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.groupBoxMyTrades.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMyTrades)).EndInit();
            this.groupBoxAllTrades.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAllTrade)).EndInit();
            this.tabPageOrders.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.splitContainerListOrders.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerListOrders)).EndInit();
            this.splitContainerListOrders.ResumeLayout(false);
            this.groupBoxOrders.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewOrders)).EndInit();
            this.panel3.ResumeLayout(false);
            this.groupBoxCreateOrder.ResumeLayout(false);
            this.groupBoxCreateOrder.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.OrdersSetVolume)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.OrdersSetPrice)).EndInit();
            this.tabPageStopOrders.ResumeLayout(false);
            this.splitContainerMainOrders.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMainOrders)).EndInit();
            this.splitContainerMainOrders.ResumeLayout(false);
            this.splitContainerTablesStopOrders.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerTablesStopOrders)).EndInit();
            this.splitContainerTablesStopOrders.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewStopOrders)).EndInit();
            this.panelConditionStopList.ResumeLayout(false);
            this.groupBoxFiltersStopOrders.ResumeLayout(false);
            this.groupBoxFiltersStopOrders.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem настройкиToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem подключитьсяToolStripMenuItem;
        private TabControl tabControlMain;
        private TabPage tabPage1;
        private TabPage tabPageTrades;
        private TabPage tabPageOrders;
        private SplitContainer splitContainer2;
        private GroupBox groupBoxMyTrades;
        private DataGridView dataGridViewMyTrades;
        private DataGridViewTextBoxColumn NumMyTrade;
        private DataGridViewTextBoxColumn IdMyTrade;
        private DataGridViewTextBoxColumn OrderMyTrade;
        private DataGridViewTextBoxColumn SecMyTrade;
        private DataGridViewTextBoxColumn TimeMyTrade;
        private DataGridViewTextBoxColumn PriceMyTrade;
        private DataGridViewTextBoxColumn VolumeMyTrade;
        private DataGridViewTextBoxColumn DirectionMyTrade;
        private GroupBox groupBoxAllTrades;
        private DataGridView dataGridViewAllTrade;
        private DataGridViewTextBoxColumn AllTradesNum;
        private DataGridViewTextBoxColumn AllTradesId;
        private DataGridViewTextBoxColumn AllTradesSec;
        private DataGridViewTextBoxColumn AllTradesTime;
        private DataGridViewTextBoxColumn AllTradesPrice;
        private DataGridViewTextBoxColumn AllTradesVolume;
        private DataGridViewTextBoxColumn AllTradesDirection;
        private Panel panel2;
        private SplitContainer splitContainer1;
        private GroupBox PanelPortfolios;
        private DataGridView dataGridPortfolios;
        private DataGridViewTextBoxColumn Account;
        private DataGridViewTextBoxColumn LimitKind;
        private DataGridViewTextBoxColumn Balance;
        private DataGridViewTextBoxColumn CurBalance;
        private DataGridViewTextBoxColumn PosBalanse;
        private DataGridViewTextBoxColumn VarMargin;
        private DataGridViewTextBoxColumn Commision;
        private GroupBox PanelPositions;
        private DataGridView dataGridPositions;
        private Panel panel1;
        private Panel panel4;
        private SplitContainer splitContainerListOrders;
        private GroupBox groupBoxOrders;
        private DataGridView dataGridViewOrders;
        private Panel panel3;
        private GroupBox groupBoxCreateOrder;
        private Label labelOrdersCountOrder;
        private Label label3;
        private Button buttonOrderCreateSell;
        private Label labelPosSec;
        private Label label5;
        private Button buttonOrdersCancelAll;
        private Label labelOrdersSec;
        private Label labelOrdersClass;
        private TextBox textBoxOrderFindSec;
        private Label label4;
        private Label OrdersLastPrice;
        private Button buttonOrderCreateBuy;
        private NumericUpDown OrdersSetVolume;
        private Label label2;
        private NumericUpDown OrdersSetPrice;
        private Label label1;
        private TabPage tabPageStopOrders;
        private SplitContainer splitContainerMainOrders;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private Button buttonOrdersShowDepth;
        private SplitContainer splitContainerTablesStopOrders;
        private DataGridView dataGridViewStopOrders;
        private Panel panelConditionStopList;
        private DataGridViewTextBoxColumn NumOrders;
        private DataGridViewTextBoxColumn StatusOrders;
        private DataGridViewTextBoxColumn IDOrders;
        private DataGridViewTextBoxColumn OrdersObjSec;
        private DataGridViewTextBoxColumn SecOrders;
        private DataGridViewTextBoxColumn PriceOrders;
        private DataGridViewTextBoxColumn VolumeOrders;
        private DataGridViewTextBoxColumn BalanceOrders;
        private DataGridViewTextBoxColumn DirectionOrders;
        private DataGridViewTextBoxColumn NamesOrders;
        private GroupBox groupBoxFiltersStopOrders;
        private CheckBox checkBoxSOClosed;
        private CheckBox checkBoxSOActive;
        private CheckBox checkBoxSOExec;
        private DataGridViewTextBoxColumn StopOrdersNum;
        private DataGridViewTextBoxColumn StopOrdersID;
        private DataGridViewTextBoxColumn StopOrdersSec;
        private DataGridViewTextBoxColumn StopOrderыType;
        private DataGridViewTextBoxColumn StopOrdersCondition;
        private DataGridViewTextBoxColumn StopOrdersStatus;
        private DataGridViewTextBoxColumn StopOrdersPrice;
        private DataGridViewTextBoxColumn StopOrdersVolume;
        private DataGridViewTextBoxColumn StopOrdersPriceStop1;
        private DataGridViewTextBoxColumn StopOrdersPriceStop2;
        private DataGridViewTextBoxColumn StopOrdersSpread;
        private DataGridViewTextBoxColumn StopOrdersOffset;
        private DataGridViewTextBoxColumn NamePos;
        private DataGridViewTextBoxColumn Code;
        private DataGridViewTextBoxColumn ActPoss;
        private DataGridViewTextBoxColumn Orders;
        private DataGridViewTextBoxColumn PosVarMargin;
        private DataGridViewButtonColumn BtnGetDepth;
    }
}

