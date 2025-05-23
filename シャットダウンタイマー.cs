using System; // System.Text.RegularExpressions を使うために必要
using System.Windows.Forms;
using System.Text.RegularExpressions; // 正規表現を使うために必要
using System.Diagnostics; // Processクラスを使用するために必要

// GUIウィンドウとなるフォームクラスを定義します
public class TestForm : Form
{
    private TextBox inputTextBox; // 入力ボックス用のメンバ変数を追加
    private Button enterButton; // ボタン用のメンバ変数を追加
    private Button cancelButton; // 中止ボタン用のメンバ変数を追加

    // コンストラクタ
    public TestForm()
    {
        // ウィンドウのタイトルを設定します
        this.Text = "test";
        // オプション: ウィンドウの初期サイズを設定します
        this.Size = new System.Drawing.Size(300, 250); // ボタンが増えたので高さを調整

        // ラベルの作成と設定
        Label instructionLabel = new Label();
        instructionLabel.Location = new System.Drawing.Point(50, 30); // 入力ボックスの上に位置を設定
        instructionLabel.Size = new System.Drawing.Size(200, 20);    // サイズを設定
        instructionLabel.Text = "シャットダウンの予約時間(秒数指定)"; // ラベルのテキストを設定

        // ① 入力ボックスの作成と設定
        inputTextBox = new TextBox();
        inputTextBox.Location = new System.Drawing.Point(50, 50); // 位置を設定
        inputTextBox.Size = new System.Drawing.Size(200, 20);    // サイズを設定

        // テキストボックスでEnterキーが押されたときのイベントハンドラを追加
        inputTextBox.KeyPress += InputTextBox_KeyPress;


        // ② 入力確定ボタンの作成と設定
        enterButton = new Button();
        enterButton.Location = new System.Drawing.Point(50, 80); // 入力ボックスの下に位置を設定
        enterButton.Size = new System.Drawing.Size(200, 30);     // サイズを設定
        enterButton.Text = "入力確定"; // ボタンのテキストを設定

        // ボタンがクリックされたときのイベントハンドラを追加
        enterButton.Click += EnterButton_Click;

        // ③ 自動シャットダウン中止ボタンの作成と設定
        cancelButton = new Button();
        cancelButton.Location = new System.Drawing.Point(50, 120); // 入力確定ボタンの下に位置を設定
        cancelButton.Size = new System.Drawing.Size(200, 30);     // サイズを設定
        cancelButton.Text = "自動シャットダウン中止"; // ボタンのテキストを設定

        // ボタンがクリックされたときのイベントハンドラを追加
        cancelButton.Click += CancelButton_Click;

        // ④ フォームにコントロールを追加
        this.Controls.Add(inputTextBox);
        this.Controls.Add(enterButton);
        this.Controls.Add(cancelButton); // 中止ボタンを追加
    }

    // TextChanged イベントハンドラ 
    private void InputTextBox_TextChanged(object sender, EventArgs e)
    {
        TextBox textBox = sender as TextBox;
        if (textBox == null) return;

        string inputText = textBox.Text;

        if (Regex.IsMatch(inputText, @"\D"))
        {
            MessageBox.Show("半角数字のみを入力してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    // KeyPress イベントハンドラ 
    private void InputTextBox_KeyPress(object sender, KeyPressEventArgs e)
    {
        if (e.KeyChar == (char)Keys.Enter)
        {
            ProcessInput();
            e.Handled = true;
        }
    }

    // EnterButton Click イベントハンドラ 
    private void EnterButton_Click(object sender, EventArgs e)
    {
        ProcessInput();
    }

    // 新しいイベントハンドラ: 中止ボタンがクリックされたとき
    private void CancelButton_Click(object sender, EventArgs e)
    {
        // shutdown /a コマンドを実行してシャットダウンを中止
        string command = "/c shutdown /a";

        try
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "cmd.exe"; // 実行するプログラムとして cmd.exe を指定
            startInfo.Arguments = command; // cmd.exe に渡す引数 (shutdown /a)
            startInfo.UseShellExecute = false; // シェル機能を使用しない（ウィンドウ表示を制御するため）
            startInfo.CreateNoWindow = true; // 新しいウィンドウを作成しない（非表示にする）

            // cmd.exeを使ってコマンドを非表示で実行
            System.Diagnostics.Process.Start(startInfo);
            // 実行したことをユーザーに通知
            MessageBox.Show("自動シャットダウンの中止を試行しました。", "シャットダウン中止", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            // コマンド実行に失敗した場合
            MessageBox.Show(string.Format("シャットダウン中止コマンドの実行に失敗しました。\nエラー: {0}", ex.Message), "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }


    // ProcessInput メソッド 
    private void ProcessInput()
    {
        string inputText = inputTextBox.Text;

        if (Regex.IsMatch(inputText, @"^\d+$"))
        {
            int delayInSeconds = int.Parse(inputText);

            string command = string.Format("/c shutdown /s /t {0}", delayInSeconds); // string.Format に修正

            try
            {
                // プロセス開始情報を設定
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "cmd.exe"; // 実行するプログラムとして cmd.exe を指定
                startInfo.Arguments = command; // cmd.exe に渡す引数 (shutdown /s /t 秒数)
                startInfo.UseShellExecute = false; // シェル機能を使用しない
                startInfo.CreateNoWindow = true; // 新しいウィンドウを作成しない

                // cmd.exeを使ってコマンドを非表示で実行
                System.Diagnostics.Process.Start(startInfo);
                MessageBox.Show(string.Format("{0}秒後にPCをシャットダウンします。", delayInSeconds), "シャットダウン予約", MessageBoxButtons.OK, MessageBoxIcon.Information); // string.Format に修正
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("シャットダウンコマンドの実行に失敗しました。\nエラー: {0}", ex.Message), "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error); // string.Format に修正
            }
        }
        else
        {
            MessageBox.Show("有効な半角数字を入力してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}

// アプリケーションのエントリポイントとなるクラス
static class Program
{
    // WinFormsアプリケーションにはSTAThread属性が必要です
    // [STAThread]
    static void Main()
    {
        // コントロールのビジュアルスタイルを有効にします (推奨)
        Application.EnableVisualStyles();
        // テキストレンダリングの互換性モードを設定します (推奨)
        Application.SetCompatibleTextRenderingDefault(false);

        // 作成したフォームを実行します
        Application.Run(new TestForm());
    }
}
