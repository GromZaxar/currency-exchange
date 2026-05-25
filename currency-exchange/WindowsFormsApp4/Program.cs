using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace CurrencyExchangeApp
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public bool IsBlocked { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ExchangeRate
    {
        public string FromCurrency { get; set; }
        public string ToCurrency { get; set; }
        public decimal Rate { get; set; }
    }

    public class ExchangeSetting
    {
        public string FromCurrency { get; set; }
        public string ToCurrency { get; set; }
        public decimal MaxAmountPerTransaction { get; set; } = 1000000m;
        public decimal? SpecialThreshold { get; set; }
        public decimal? SpecialCommissionPercent { get; set; }
    }

    public class Transaction
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string FromCurrency { get; set; }
        public string ToCurrency { get; set; }
        public decimal FromAmount { get; set; }
        public decimal ToAmount { get; set; }
        public decimal CommissionPercent { get; set; }
        public DateTime Timestamp { get; set; }
    }

    // ======================== ХРАНИЛИЩЕ ДАННЫХ (JSON через JavaScriptSerializer) ========================
    public static class DataManager
    {
        private static readonly string DataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        private static readonly string UsersFile = Path.Combine(DataFolder, "users.json");
        private static readonly string ExchangeSettingsFile = Path.Combine(DataFolder, "exchange_settings.json");
        private static readonly string TransactionsFile = Path.Combine(DataFolder, "transactions.json");
        private static readonly string RatesFile = Path.Combine(DataFolder, "rates.json");

        private static JavaScriptSerializer _serializer = new JavaScriptSerializer();

        public static List<User> Users { get; private set; }
        public static List<ExchangeSetting> ExchangeSettings { get; private set; }
        public static List<Transaction> Transactions { get; private set; }
        public static List<ExchangeRate> Rates { get; private set; }

        public static readonly List<string> Currencies = new List<string> { "RUB", "USD", "EUR", "GBP", "CNY", "JPY" };

        static DataManager()
        {
            if (!Directory.Exists(DataFolder))
                Directory.CreateDirectory(DataFolder);

            LoadUsers();
            LoadExchangeSettings();
            LoadTransactions();
            LoadRates();
        }

        private static void LoadUsers()
        {
            if (File.Exists(UsersFile))
            {
                string json = File.ReadAllText(UsersFile);
                Users = _serializer.Deserialize<List<User>>(json) ?? new List<User>();
            }
            else
            {
                Users = new List<User>();
                for (int i = 1; i <= 10; i++)
                {
                    Users.Add(new User
                    {
                        Id = i,
                        Username = $"User{i}",
                        IsBlocked = false,
                        CreatedAt = DateTime.Now.AddDays(-i)
                    });
                }
                SaveUsers();
            }
        }

        public static void SaveUsers()
        {
            string json = _serializer.Serialize(Users);
            File.WriteAllText(UsersFile, json);
        }

        private static void LoadExchangeSettings()
        {
            if (File.Exists(ExchangeSettingsFile))
            {
                string json = File.ReadAllText(ExchangeSettingsFile);
                ExchangeSettings = _serializer.Deserialize<List<ExchangeSetting>>(json) ?? new List<ExchangeSetting>();
            }
            else
            {
                ExchangeSettings = new List<ExchangeSetting>();
                foreach (var from in Currencies)
                {
                    foreach (var to in Currencies)
                    {
                        if (from != to)
                        {
                            ExchangeSettings.Add(new ExchangeSetting
                            {
                                FromCurrency = from,
                                ToCurrency = to,
                                MaxAmountPerTransaction = 1000000m,
                                SpecialThreshold = null,
                                SpecialCommissionPercent = null
                            });
                        }
                    }
                }
                SaveExchangeSettings();
            }
        }

        public static void SaveExchangeSettings()
        {
            string json = _serializer.Serialize(ExchangeSettings);
            File.WriteAllText(ExchangeSettingsFile, json);
        }

        private static void LoadTransactions()
        {
            if (File.Exists(TransactionsFile))
            {
                string json = File.ReadAllText(TransactionsFile);
                Transactions = _serializer.Deserialize<List<Transaction>>(json) ?? new List<Transaction>();
            }
            else
            {
                Transactions = new List<Transaction>();
                SaveTransactions();
            }
        }

        public static void SaveTransactions()
        {
            string json = _serializer.Serialize(Transactions);
            File.WriteAllText(TransactionsFile, json);
        }

        private static void LoadRates()
        {
            if (File.Exists(RatesFile))
            {
                string json = File.ReadAllText(RatesFile);
                Rates = _serializer.Deserialize<List<ExchangeRate>>(json) ?? new List<ExchangeRate>();
            }
            else
            {
                Rates = new List<ExchangeRate>
                {
                    new ExchangeRate { FromCurrency = "RUB", ToCurrency = "USD", Rate = 0.011m },
                    new ExchangeRate { FromCurrency = "RUB", ToCurrency = "EUR", Rate = 0.010m },
                    new ExchangeRate { FromCurrency = "RUB", ToCurrency = "GBP", Rate = 0.0085m },
                    new ExchangeRate { FromCurrency = "RUB", ToCurrency = "CNY", Rate = 0.080m },
                    new ExchangeRate { FromCurrency = "RUB", ToCurrency = "JPY", Rate = 1.50m },
                    new ExchangeRate { FromCurrency = "USD", ToCurrency = "RUB", Rate = 90.0m },
                    new ExchangeRate { FromCurrency = "USD", ToCurrency = "EUR", Rate = 0.92m },
                    new ExchangeRate { FromCurrency = "USD", ToCurrency = "GBP", Rate = 0.78m },
                    new ExchangeRate { FromCurrency = "USD", ToCurrency = "CNY", Rate = 7.20m },
                    new ExchangeRate { FromCurrency = "USD", ToCurrency = "JPY", Rate = 145.0m },
                    new ExchangeRate { FromCurrency = "EUR", ToCurrency = "RUB", Rate = 98.0m },
                    new ExchangeRate { FromCurrency = "EUR", ToCurrency = "USD", Rate = 1.09m },
                    new ExchangeRate { FromCurrency = "EUR", ToCurrency = "GBP", Rate = 0.85m },
                    new ExchangeRate { FromCurrency = "EUR", ToCurrency = "CNY", Rate = 7.80m },
                    new ExchangeRate { FromCurrency = "EUR", ToCurrency = "JPY", Rate = 158.0m },
                    new ExchangeRate { FromCurrency = "GBP", ToCurrency = "RUB", Rate = 115.0m },
                    new ExchangeRate { FromCurrency = "GBP", ToCurrency = "USD", Rate = 1.28m },
                    new ExchangeRate { FromCurrency = "GBP", ToCurrency = "EUR", Rate = 1.18m },
                    new ExchangeRate { FromCurrency = "GBP", ToCurrency = "CNY", Rate = 9.20m },
                    new ExchangeRate { FromCurrency = "GBP", ToCurrency = "JPY", Rate = 186.0m },
                    new ExchangeRate { FromCurrency = "CNY", ToCurrency = "RUB", Rate = 12.5m },
                    new ExchangeRate { FromCurrency = "CNY", ToCurrency = "USD", Rate = 0.14m },
                    new ExchangeRate { FromCurrency = "CNY", ToCurrency = "EUR", Rate = 0.13m },
                    new ExchangeRate { FromCurrency = "CNY", ToCurrency = "GBP", Rate = 0.11m },
                    new ExchangeRate { FromCurrency = "CNY", ToCurrency = "JPY", Rate = 20.2m },
                    new ExchangeRate { FromCurrency = "JPY", ToCurrency = "RUB", Rate = 0.62m },
                    new ExchangeRate { FromCurrency = "JPY", ToCurrency = "USD", Rate = 0.0069m },
                    new ExchangeRate { FromCurrency = "JPY", ToCurrency = "EUR", Rate = 0.0063m },
                    new ExchangeRate { FromCurrency = "JPY", ToCurrency = "GBP", Rate = 0.0054m },
                    new ExchangeRate { FromCurrency = "JPY", ToCurrency = "CNY", Rate = 0.0495m },
                };
                SaveRates();
            }
        }

        public static void SaveRates()
        {
            string json = _serializer.Serialize(Rates);
            File.WriteAllText(RatesFile, json);
        }

        public static decimal GetRate(string from, string to)
        {
            var rate = Rates.FirstOrDefault(r => r.FromCurrency == from && r.ToCurrency == to);
            return rate?.Rate ?? 1.0m;
        }

        public static ExchangeSetting GetSetting(string from, string to)
        {
            return ExchangeSettings.FirstOrDefault(s => s.FromCurrency == from && s.ToCurrency == to);
        }

        public static void AddTransaction(Transaction transaction)
        {
            transaction.Id = Transactions.Count > 0 ? Transactions.Max(t => t.Id) + 1 : 1;
            Transactions.Add(transaction);
            SaveTransactions();
        }
    }

    // ======================== ФОРМА АВТОРИЗАЦИИ ========================
    public class FormAuth : Form
    {
        private TextBox txtLogin;
        private TextBox txtPassword;
        private Button btnLogin;
        private Label lblStatus;

        public FormAuth()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Авторизация - Система обмена валют";
            this.Size = new System.Drawing.Size(300, 200);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            Label lblLogin = new Label() { Text = "Логин:", Location = new System.Drawing.Point(20, 30), Size = new System.Drawing.Size(80, 25) };
            Label lblPassword = new Label() { Text = "Пароль:", Location = new System.Drawing.Point(20, 70), Size = new System.Drawing.Size(80, 25) };
            txtLogin = new TextBox() { Location = new System.Drawing.Point(110, 30), Size = new System.Drawing.Size(150, 23) };
            txtPassword = new TextBox() { Location = new System.Drawing.Point(110, 70), Size = new System.Drawing.Size(150, 23), UseSystemPasswordChar = true };
            btnLogin = new Button() { Text = "Войти", Location = new System.Drawing.Point(110, 110), Size = new System.Drawing.Size(100, 30) };
            lblStatus = new Label() { Text = "", Location = new System.Drawing.Point(20, 150), Size = new System.Drawing.Size(250, 25), ForeColor = System.Drawing.Color.Red };

            btnLogin.Click += BtnLogin_Click;

            this.Controls.Add(lblLogin);
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtLogin);
            this.Controls.Add(txtPassword);
            this.Controls.Add(btnLogin);
            this.Controls.Add(lblStatus);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Text;

            if (login == "admin" && password == "1234")
            {
                FormAdmin adminForm = new FormAdmin();
                adminForm.Show();
                this.Hide();
            }
            else if (login == "user" && password == "1234")
            {
                FormUser userForm = new FormUser("user");
                userForm.Show();
                this.Hide();
            }
            else
            {
                lblStatus.Text = "Неверный логин или пароль!";
            }
        }
    }

    // ======================== ФОРМА ПОЛЬЗОВАТЕЛЯ ========================
    public class FormUser : Form
    {
        private string currentUser;
        private ListBox lstFromCurrencies;
        private ListBox lstToCurrencies;
        private TextBox txtAmount;
        private Button btnExchange;
        private Label lblMinMax;
        private Label lblRate;
        private Label lblCommission;
        private Label lblSpecialCommission;
        private ListBox lstHistory;
        private Button btnSaveHistory;
        private Label lblResult;

        private string selectedFrom = "RUB";
        private string selectedTo = "USD";

        public FormUser(string username)
        {
            currentUser = username;
            InitializeComponent();
            LoadHistory();
            UpdateRequirements();
        }

        private void InitializeComponent()
        {
            this.Text = $"Пользователь: {currentUser} - Система обмена валют";
            this.Size = new System.Drawing.Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Левая панель
            Panel leftPanel = new Panel() { Location = new System.Drawing.Point(10, 10), Size = new System.Drawing.Size(550, 540), BorderStyle = BorderStyle.FixedSingle };

            Label lblFrom = new Label() { Text = "Отдаете:", Location = new System.Drawing.Point(10, 10), Size = new System.Drawing.Size(100, 25) };
            Label lblTo = new Label() { Text = "Получаете:", Location = new System.Drawing.Point(280, 10), Size = new System.Drawing.Size(100, 25) };
            lstFromCurrencies = new ListBox() { Location = new System.Drawing.Point(10, 40), Size = new System.Drawing.Size(250, 200) };
            lstToCurrencies = new ListBox() { Location = new System.Drawing.Point(280, 40), Size = new System.Drawing.Size(250, 200) };

            foreach (var curr in DataManager.Currencies)
            {
                lstFromCurrencies.Items.Add(curr);
                lstToCurrencies.Items.Add(curr);
            }
            lstFromCurrencies.SelectedItem = "RUB";
            lstToCurrencies.SelectedItem = "USD";
            lstFromCurrencies.SelectedIndexChanged += (s, e) => { selectedFrom = lstFromCurrencies.SelectedItem.ToString(); UpdateRequirements(); };
            lstToCurrencies.SelectedIndexChanged += (s, e) => { selectedTo = lstToCurrencies.SelectedItem.ToString(); UpdateRequirements(); };

            Label lblAmount = new Label() { Text = "Сумма для обмена:", Location = new System.Drawing.Point(10, 260), Size = new System.Drawing.Size(150, 25) };
            txtAmount = new TextBox() { Location = new System.Drawing.Point(170, 260), Size = new System.Drawing.Size(150, 23), Text = "1000" };
            txtAmount.TextChanged += (s, e) => UpdateRequirements();

            btnExchange = new Button() { Text = "Обменять", Location = new System.Drawing.Point(350, 260), Size = new System.Drawing.Size(100, 30) };
            btnExchange.Click += BtnExchange_Click;

            GroupBox reqGroup = new GroupBox() { Text = "Требования к обмену", Location = new System.Drawing.Point(10, 310), Size = new System.Drawing.Size(520, 150) };
            lblMinMax = new Label() { Location = new System.Drawing.Point(10, 25), Size = new System.Drawing.Size(500, 25) };
            lblRate = new Label() { Location = new System.Drawing.Point(10, 55), Size = new System.Drawing.Size(500, 25) };
            lblCommission = new Label() { Location = new System.Drawing.Point(10, 85), Size = new System.Drawing.Size(500, 25) };
            lblSpecialCommission = new Label() { Location = new System.Drawing.Point(10, 115), Size = new System.Drawing.Size(500, 25) };
            reqGroup.Controls.Add(lblMinMax);
            reqGroup.Controls.Add(lblRate);
            reqGroup.Controls.Add(lblCommission);
            reqGroup.Controls.Add(lblSpecialCommission);

            lblResult = new Label() { Location = new System.Drawing.Point(10, 480), Size = new System.Drawing.Size(520, 40), ForeColor = System.Drawing.Color.Green };

            leftPanel.Controls.Add(lblFrom);
            leftPanel.Controls.Add(lblTo);
            leftPanel.Controls.Add(lstFromCurrencies);
            leftPanel.Controls.Add(lstToCurrencies);
            leftPanel.Controls.Add(lblAmount);
            leftPanel.Controls.Add(txtAmount);
            leftPanel.Controls.Add(btnExchange);
            leftPanel.Controls.Add(reqGroup);
            leftPanel.Controls.Add(lblResult);

            // Правая панель: история
            Panel rightPanel = new Panel() { Location = new System.Drawing.Point(570, 10), Size = new System.Drawing.Size(310, 540), BorderStyle = BorderStyle.FixedSingle };
            Label lblHistory = new Label() { Text = "История операций", Location = new System.Drawing.Point(10, 10), Size = new System.Drawing.Size(150, 25), Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold) };
            lstHistory = new ListBox() { Location = new System.Drawing.Point(10, 40), Size = new System.Drawing.Size(280, 450) };
            btnSaveHistory = new Button() { Text = "Сохранить историю (.txt)", Location = new System.Drawing.Point(10, 500), Size = new System.Drawing.Size(280, 30) };
            btnSaveHistory.Click += BtnSaveHistory_Click;

            rightPanel.Controls.Add(lblHistory);
            rightPanel.Controls.Add(lstHistory);
            rightPanel.Controls.Add(btnSaveHistory);

            this.Controls.Add(leftPanel);
            this.Controls.Add(rightPanel);
        }

        private void UpdateRequirements()
        {
            if (selectedFrom == selectedTo)
            {
                lblMinMax.Text = "Выберите разные валюты";
                lblRate.Text = "";
                lblCommission.Text = "";
                lblSpecialCommission.Text = "";
                btnExchange.Enabled = false;
                return;
            }

            var setting = DataManager.GetSetting(selectedFrom, selectedTo);
            decimal maxAmount = setting?.MaxAmountPerTransaction ?? 1000000m;
            decimal minAmount = 0.01m;

            decimal rate = DataManager.GetRate(selectedFrom, selectedTo);
            decimal commissionPercent = 2.0m;
            string specialInfo = "Нет";

            decimal amount = 0;
            if (!decimal.TryParse(txtAmount.Text, out amount) || amount <= 0)
            {
                lblMinMax.Text = $"Мин: {minAmount:F2} {selectedFrom} | Макс: {maxAmount:F2} {selectedFrom}";
                lblRate.Text = $"Курс: 1 {selectedFrom} = {rate:F4} {selectedTo}";
                lblCommission.Text = $"Комиссия: {commissionPercent}% (стандартная)";
                lblSpecialCommission.Text = $"Уменьшенная комиссия от: {setting?.SpecialThreshold?.ToString("F2") ?? "не задано"} {selectedFrom}";
                btnExchange.Enabled = false;
                return;
            }

            if (amount > maxAmount)
            {
                lblMinMax.Text = $"Мин: {minAmount:F2} | Макс: {maxAmount:F2} (превышен лимит!)";
                btnExchange.Enabled = false;
            }
            else
            {
                lblMinMax.Text = $"Мин: {minAmount:F2} | Макс: {maxAmount:F2}";
                btnExchange.Enabled = true;
            }

            if (setting?.SpecialThreshold != null && amount >= setting.SpecialThreshold)
            {
                commissionPercent = setting.SpecialCommissionPercent ?? 2.0m;
                specialInfo = $"от {setting.SpecialThreshold:F2} {selectedFrom} комиссия {commissionPercent}%";
            }

            lblRate.Text = $"Курс: 1 {selectedFrom} = {rate:F4} {selectedTo}";
            lblCommission.Text = $"Комиссия: {commissionPercent}% ({(commissionPercent == 2.0m ? "стандартная" : "специальная")})";
            lblSpecialCommission.Text = $"Уменьшенная комиссия от: {setting?.SpecialThreshold?.ToString("F2") ?? "не задано"} {selectedFrom}";
        }

        private void BtnExchange_Click(object sender, EventArgs e)
        {
            if (selectedFrom == selectedTo)
            {
                MessageBox.Show("Выберите разные валюты для обмена.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtAmount.Text, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Введите корректную сумму обмена.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var setting = DataManager.GetSetting(selectedFrom, selectedTo);
            decimal maxAmount = setting?.MaxAmountPerTransaction ?? 1000000m;
            if (amount > maxAmount)
            {
                MessageBox.Show($"Превышен лимит обмена на одну транзакцию! Максимум: {maxAmount:F2} {selectedFrom}", "Лимит", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal rate = DataManager.GetRate(selectedFrom, selectedTo);
            decimal commissionPercent = 2.0m;
            if (setting?.SpecialThreshold != null && amount >= setting.SpecialThreshold)
            {
                commissionPercent = setting.SpecialCommissionPercent ?? 2.0m;
            }

            decimal amountAfterCommission = amount * (1 - commissionPercent / 100);
            decimal toAmount = amountAfterCommission * rate;

            Transaction trans = new Transaction
            {
                UserName = currentUser,
                FromCurrency = selectedFrom,
                ToCurrency = selectedTo,
                FromAmount = amount,
                ToAmount = toAmount,
                CommissionPercent = commissionPercent,
                Timestamp = DateTime.Now
            };
            DataManager.AddTransaction(trans);

            lblResult.Text = $"Обмен выполнен! Получено: {toAmount:F2} {selectedTo} (комиссия {commissionPercent}%)";
            LoadHistory();
        }

        private void LoadHistory()
        {
            lstHistory.Items.Clear();
            var userTransactions = DataManager.Transactions.Where(t => t.UserName == currentUser).OrderByDescending(t => t.Timestamp);
            foreach (var t in userTransactions)
            {
                lstHistory.Items.Add($"{t.Timestamp:dd.MM.yyyy HH:mm} | {t.FromAmount:F2} {t.FromCurrency} → {t.ToAmount:F2} {t.ToCurrency} (ком. {t.CommissionPercent}%)");
            }
        }

        private void BtnSaveHistory_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Текстовые файлы|*.txt";
            sfd.Title = "Сохранить историю";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                var userTransactions = DataManager.Transactions.Where(t => t.UserName == currentUser).OrderByDescending(t => t.Timestamp);
                using (StreamWriter sw = new StreamWriter(sfd.FileName))
                {
                    sw.WriteLine($"История операций пользователя {currentUser}\n");
                    foreach (var t in userTransactions)
                    {
                        sw.WriteLine($"{t.Timestamp:dd.MM.yyyy HH:mm} | {t.FromAmount:F2} {t.FromCurrency} → {t.ToAmount:F2} {t.ToCurrency} (ком. {t.CommissionPercent}%)");
                    }
                }
                MessageBox.Show("История сохранена.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }

    // ======================== ФОРМА АДМИНИСТРАТОРА ========================
    public class FormAdmin : Form
    {
        private TabControl tabControl;
        private DataGridView dgvUsers;
        private Button btnBlock, btnDelete;
        private ComboBox cmbFrom, cmbTo;
        private NumericUpDown nudMaxAmount, nudSpecialThreshold, nudSpecialCommission;
        private Button btnSaveSettings;
        private DataGridView dgvHistory;
        private Button btnSaveHistoryAdmin;

        public FormAdmin()
        {
            InitializeComponent();
            LoadUsersGrid();
            LoadSettingsCombo();
            LoadHistoryGrid();
        }

        private void InitializeComponent()
        {
            this.Text = "Администратор - Система обмена валют";
            this.Size = new System.Drawing.Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            tabControl = new TabControl() { Dock = DockStyle.Fill };

            // Вкладка Пользователи
            TabPage pageUsers = new TabPage("Пользователи");
            dgvUsers = new DataGridView() { Dock = DockStyle.Fill, AllowUserToAddRows = false, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect };
            dgvUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            Panel panelButtons = new Panel() { Height = 50, Dock = DockStyle.Bottom };
            btnBlock = new Button() { Text = "Заблокировать", Location = new System.Drawing.Point(10, 10), Size = new System.Drawing.Size(120, 30) };
            btnDelete = new Button() { Text = "Удалить аккаунт", Location = new System.Drawing.Point(140, 10), Size = new System.Drawing.Size(120, 30) };
            btnBlock.Click += BtnBlock_Click;
            btnDelete.Click += BtnDelete_Click;
            panelButtons.Controls.Add(btnBlock);
            panelButtons.Controls.Add(btnDelete);
            pageUsers.Controls.Add(dgvUsers);
            pageUsers.Controls.Add(panelButtons);

            // Вкладка Обмены
            TabPage pageExchanges = new TabPage("Обмены");
            GroupBox grpSettings = new GroupBox() { Text = "Настройка комиссий и лимитов для пары валют", Location = new System.Drawing.Point(10, 10), Size = new System.Drawing.Size(850, 200) };
            Label lblFrom = new Label() { Text = "Валюта отдачи:", Location = new System.Drawing.Point(10, 30), Size = new System.Drawing.Size(100, 25) };
            cmbFrom = new ComboBox() { Location = new System.Drawing.Point(120, 30), Size = new System.Drawing.Size(150, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            Label lblTo = new Label() { Text = "Валюта получения:", Location = new System.Drawing.Point(300, 30), Size = new System.Drawing.Size(120, 25) };
            cmbTo = new ComboBox() { Location = new System.Drawing.Point(430, 30), Size = new System.Drawing.Size(150, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            foreach (var curr in DataManager.Currencies) { cmbFrom.Items.Add(curr); cmbTo.Items.Add(curr); }
            cmbFrom.SelectedIndex = 0;
            cmbTo.SelectedIndex = 1;
            cmbFrom.SelectedIndexChanged += (s, e) => LoadCurrentSettings();
            cmbTo.SelectedIndexChanged += (s, e) => LoadCurrentSettings();

            Label lblMax = new Label() { Text = "Лимит на транзакцию:", Location = new System.Drawing.Point(10, 70), Size = new System.Drawing.Size(150, 25) };
            nudMaxAmount = new NumericUpDown() { Location = new System.Drawing.Point(170, 68), Size = new System.Drawing.Size(150, 23), Minimum = 0, Maximum = 10000000, DecimalPlaces = 2, ThousandsSeparator = true };
            Label lblSpecial = new Label() { Text = "Порог для спецкомиссии:", Location = new System.Drawing.Point(10, 110), Size = new System.Drawing.Size(150, 25) };
            nudSpecialThreshold = new NumericUpDown() { Location = new System.Drawing.Point(170, 108), Size = new System.Drawing.Size(150, 23), Minimum = 0, Maximum = 10000000, DecimalPlaces = 2 };
            Label lblComm = new Label() { Text = "Уменьшенная комиссия (%):", Location = new System.Drawing.Point(10, 150), Size = new System.Drawing.Size(170, 25) };
            nudSpecialCommission = new NumericUpDown() { Location = new System.Drawing.Point(190, 148), Size = new System.Drawing.Size(130, 23), Minimum = 0, Maximum = 100, DecimalPlaces = 2 };
            btnSaveSettings = new Button() { Text = "Сохранить настройки", Location = new System.Drawing.Point(350, 108), Size = new System.Drawing.Size(150, 40) };
            btnSaveSettings.Click += BtnSaveSettings_Click;

            grpSettings.Controls.Add(lblFrom);
            grpSettings.Controls.Add(cmbFrom);
            grpSettings.Controls.Add(lblTo);
            grpSettings.Controls.Add(cmbTo);
            grpSettings.Controls.Add(lblMax);
            grpSettings.Controls.Add(nudMaxAmount);
            grpSettings.Controls.Add(lblSpecial);
            grpSettings.Controls.Add(nudSpecialThreshold);
            grpSettings.Controls.Add(lblComm);
            grpSettings.Controls.Add(nudSpecialCommission);
            grpSettings.Controls.Add(btnSaveSettings);
            pageExchanges.Controls.Add(grpSettings);

            // Вкладка История
            TabPage pageHistory = new TabPage("История");
            dgvHistory = new DataGridView() { Dock = DockStyle.Fill, AllowUserToAddRows = false, ReadOnly = true };
            dgvHistory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            btnSaveHistoryAdmin = new Button() { Text = "Сохранить историю (.txt)", Dock = DockStyle.Bottom, Height = 40 };
            btnSaveHistoryAdmin.Click += BtnSaveHistoryAdmin_Click;
            pageHistory.Controls.Add(dgvHistory);
            pageHistory.Controls.Add(btnSaveHistoryAdmin);

            tabControl.TabPages.Add(pageUsers);
            tabControl.TabPages.Add(pageExchanges);
            tabControl.TabPages.Add(pageHistory);
            this.Controls.Add(tabControl);
        }

        private void LoadUsersGrid()
        {
            dgvUsers.DataSource = null;
            var users = DataManager.Users.Select(u => new { u.Id, u.Username, u.IsBlocked, u.CreatedAt }).ToList();
            dgvUsers.DataSource = users;
            if (dgvUsers.Columns["Id"] != null) dgvUsers.Columns["Id"].Visible = false;
        }

        private void LoadSettingsCombo() { }

        private void LoadCurrentSettings()
        {
            if (cmbFrom.SelectedItem == null || cmbTo.SelectedItem == null) return;
            string from = cmbFrom.SelectedItem.ToString();
            string to = cmbTo.SelectedItem.ToString();
            if (from == to)
            {
                nudMaxAmount.Enabled = false;
                nudSpecialThreshold.Enabled = false;
                nudSpecialCommission.Enabled = false;
                btnSaveSettings.Enabled = false;
                return;
            }
            nudMaxAmount.Enabled = true;
            nudSpecialThreshold.Enabled = true;
            nudSpecialCommission.Enabled = true;
            btnSaveSettings.Enabled = true;

            var setting = DataManager.GetSetting(from, to);
            if (setting != null)
            {
                nudMaxAmount.Value = setting.MaxAmountPerTransaction;
                nudSpecialThreshold.Value = setting.SpecialThreshold ?? 0;
                nudSpecialCommission.Value = setting.SpecialCommissionPercent ?? 0;
            }
            else
            {
                nudMaxAmount.Value = 1000000;
                nudSpecialThreshold.Value = 0;
                nudSpecialCommission.Value = 0;
            }
        }

        private void BtnSaveSettings_Click(object sender, EventArgs e)
        {
            string from = cmbFrom.SelectedItem.ToString();
            string to = cmbTo.SelectedItem.ToString();
            if (from == to)
            {
                MessageBox.Show("Нельзя настраивать обмен одинаковых валют.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var setting = DataManager.GetSetting(from, to);
            if (setting != null)
            {
                setting.MaxAmountPerTransaction = nudMaxAmount.Value;
                setting.SpecialThreshold = nudSpecialThreshold.Value > 0 ? nudSpecialThreshold.Value : (decimal?)null;
                setting.SpecialCommissionPercent = nudSpecialCommission.Value > 0 ? nudSpecialCommission.Value : (decimal?)null;
            }
            else
            {
                setting = new ExchangeSetting
                {
                    FromCurrency = from,
                    ToCurrency = to,
                    MaxAmountPerTransaction = nudMaxAmount.Value,
                    SpecialThreshold = nudSpecialThreshold.Value > 0 ? nudSpecialThreshold.Value : (decimal?)null,
                    SpecialCommissionPercent = nudSpecialCommission.Value > 0 ? nudSpecialCommission.Value : (decimal?)null
                };
                DataManager.ExchangeSettings.Add(setting);
            }
            DataManager.SaveExchangeSettings();
            MessageBox.Show("Настройки сохранены.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void LoadHistoryGrid()
        {
            var history = DataManager.Transactions.OrderByDescending(t => t.Timestamp).Select(t => new
            {
                t.UserName,
                t.Timestamp,
                t.FromCurrency,
                t.ToCurrency,
                t.FromAmount,
                t.ToAmount,
                t.CommissionPercent
            }).ToList();
            dgvHistory.DataSource = history;
        }

        private void BtnSaveHistoryAdmin_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Текстовые файлы|*.txt";
            sfd.Title = "Сохранить историю всех операций";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter sw = new StreamWriter(sfd.FileName))
                {
                    sw.WriteLine("История всех операций\n");
                    foreach (var t in DataManager.Transactions.OrderByDescending(t => t.Timestamp))
                    {
                        sw.WriteLine($"{t.Timestamp:dd.MM.yyyy HH:mm} | {t.UserName} | {t.FromAmount:F2} {t.FromCurrency} → {t.ToAmount:F2} {t.ToCurrency} (ком. {t.CommissionPercent}%)");
                    }
                }
                MessageBox.Show("История сохранена.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnBlock_Click(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count == 0) return;
            int userId = (int)dgvUsers.SelectedRows[0].Cells["Id"].Value;
            var user = DataManager.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                user.IsBlocked = !user.IsBlocked;
                DataManager.SaveUsers();
                LoadUsersGrid();
                MessageBox.Show($"Пользователь {(user.IsBlocked ? "заблокирован" : "разблокирован")}.", "Статус изменён", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count == 0) return;
            int userId = (int)dgvUsers.SelectedRows[0].Cells["Id"].Value;
            var user = DataManager.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null && MessageBox.Show($"Удалить пользователя {user.Username}? Все его операции останутся в истории.", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                DataManager.Users.Remove(user);
                DataManager.SaveUsers();
                LoadUsersGrid();
            }
        }
    }

    // ======================== ТОЧКА ВХОДА ========================
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormAuth());
        }
    }
}